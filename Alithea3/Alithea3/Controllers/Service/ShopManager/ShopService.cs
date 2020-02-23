using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Controllers.Respository.CategoryManager;
using Alithea3.Controllers.Respository.OrderManager;
using Alithea3.Controllers.Respository.ProductManager;
using Alithea3.Models;
using LinqKit;
using Microsoft.Ajax.Utilities;

namespace Alithea3.Controllers.Service.ShopManager
{
    public class ShopService : BaseService<BuyItem>
    {
        private OrderRespository _orderRespository = new OrderRespository();
        private CategoryRepository _categoryRepository = new CategoryRepository();
        private ProductRepository _productRepository = new ProductRepository();
        private MyDbContext _db = new MyDbContext();

        public Hashtable FilterProduct(List<int> id, int page, int limit, double? minPrice, double? maxPrice)
        {
            var hashtable = new Hashtable();
            var categoryFilter = new List<Category>();
            var productFilter = new List<Product>();
            var currentPara = "?";

            var predicate = PredicateBuilder.New<ProductCategory>();
            if (id != null)
            {
                for (int i = 0; i < id.Count; i++)
                {
                    int CatId = id[i];
                    predicate = predicate.Or(pc => pc.CategoryID == CatId);
                    categoryFilter.Add(_db.Categories.Find(CatId));

                    currentPara += "id=" + CatId + "&";
                }
            }

            //get category filter
            hashtable.Add("listCategory", categoryFilter);

            //get total product filter
            productFilter = _db.ProductCategories.Where(predicate.Compile()).Select(pc => pc)
                .GroupBy(pc => pc.ProductID).Where(pc => pc.Count() >= id.Count).Select(pc => pc.FirstOrDefault()?.Product).ToList();

            if (minPrice != null || maxPrice != null)
            {
                //filter product by price
                if (productFilter.Count == 0)
                {
                    productFilter = _db.Products.ToList();
                }

                if (minPrice != null && maxPrice == null)
                {
                    productFilter = productFilter.Where(p => p.UnitPrice >= minPrice).ToList();
                }
                else if (minPrice == null && maxPrice != null)
                {
                    productFilter = productFilter.Where(p => p.UnitPrice <= maxPrice).ToList();
                }
                else if (minPrice != null && maxPrice != null)
                {
                    productFilter = productFilter.Where(p => p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice).ToList();
                }

                currentPara += "MinPrice=" + minPrice + "&MaxPrice=" + maxPrice + "&";
            }

            //get listID
            hashtable.Add("currentPara", currentPara);
            //get total page filter
            hashtable.Add("totalPage", Math.Ceiling((double)productFilter.Count() / limit));

            //get product filter
            hashtable.Add("listProduct", productFilter.OrderByDescending(p => p.UpdatedAt).Skip((page - 1) * limit).Take(limit).ToList());

            return hashtable;
        }

        public Hashtable AddItem(List<Product> listShoppingCart, int proId, int quantity, int color, string nameColor, int size, string nameSize)
        {
            var hashtable = new Hashtable();
            if (listShoppingCart == null)
            {
                listShoppingCart = new List<Product>();
            }

            bool checkSp = true;
            for (var i = 0; i < listShoppingCart.Count; i++)
            {
                if (listShoppingCart[i].ProductID == proId && listShoppingCart[i].Color == color &&
                   listShoppingCart[i].Size == size)
                {
                   listShoppingCart[i].Quantity += quantity;
                   checkSp = false;
                }
            }

            if (checkSp)
            {
                var attr = _db.Attributes.FirstOrDefault(a => a.ProductID == proId && a.ColorID == color);
                var product = _productRepository.SelectById(proId);
                if (attr != null)
                {
                    product.ProductImage = attr.ProductImage;
                    product.UnitPrice = attr.UnitPrice;
                }

                if (product.VAT == null || product.VAT < 0)
                {
                    product.VAT = 0;
                }

                product.Quantity = quantity;
                product.Color = color;
                product.NameColor = nameColor;
                product.Size = size;
                product.NameSize = nameSize;
                product.totalPriceOne = product.Quantity * product.UnitPrice * product.VAT.Value / 100 +
                                        product.Quantity * product.UnitPrice;
                listShoppingCart.Add(product);
            }

            hashtable.Add(SessionName.ShoppingCart, listShoppingCart);

            var getTotal = this.getTotal(listShoppingCart);
            hashtable.Add(SessionName.TotalQuantity, getTotal[SessionName.TotalQuantity]);
            hashtable.Add(SessionName.TotalPrice, getTotal[SessionName.TotalPrice]);

            return hashtable;
        }

        public Hashtable DeleteItem(List<Product> listShoppingCart, int idItem)
        {
            var hashtable = new Hashtable();
            try
            {
                listShoppingCart?.RemoveAt(idItem);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            
            hashtable.Add(SessionName.ShoppingCart, listShoppingCart);

            var getTotal = this.getTotal(listShoppingCart);
            hashtable.Add(SessionName.TotalQuantity, getTotal[SessionName.TotalQuantity]);
            hashtable.Add(SessionName.TotalPrice, getTotal[SessionName.TotalPrice]);

            return hashtable;
        }

        public Hashtable UpdateItem(List<Product> listShoppingCart, int idItem, int quantity)
        {
            var hashtable = new Hashtable();
            double totalPriceOneProduct = 0;

            if (listShoppingCart != null)
            {
                try
                {
                    if (quantity < 1)
                    {
                        listShoppingCart[idItem].Quantity = 1;
                    }
                  
                    listShoppingCart[idItem].Quantity = quantity;
                    if (listShoppingCart[idItem].VAT == null && listShoppingCart[idItem].VAT < 0)
                    {
                        listShoppingCart[idItem].VAT = 0;
                    }

                    totalPriceOneProduct = listShoppingCart[idItem].totalPriceOne = listShoppingCart[idItem].Quantity * listShoppingCart[idItem].UnitPrice *
                                           listShoppingCart[idItem].VAT.Value / 100 + 
                                           listShoppingCart[idItem].Quantity * listShoppingCart[idItem].UnitPrice;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            hashtable.Add(SessionName.ShoppingCart, listShoppingCart);

            var getTotal = this.getTotal(listShoppingCart);
            hashtable.Add(SessionName.TotalQuantity, getTotal[SessionName.TotalQuantity]);
            hashtable.Add(SessionName.TotalPrice, getTotal[SessionName.TotalPrice]);
            hashtable.Add("totalPriceOneProduct", totalPriceOneProduct);

            return hashtable;
        }

        public Hashtable getTotal(List<Product> listShoppingCart)
        {
            var hashtable = new Hashtable();

            double totalPrice = 0;
            var totalQuantity = 0;

            if (listShoppingCart != null)
            {
                for (int i = 0; i < listShoppingCart.Count; i++)
                {
                    totalQuantity += listShoppingCart[i].Quantity;
                    if (listShoppingCart[i].VAT == null || listShoppingCart[i].VAT < 0)
                    {
                        listShoppingCart[i].VAT = 0;
                    }
                    listShoppingCart[i].totalPriceOne = listShoppingCart[i].UnitPrice * listShoppingCart[i].Quantity * listShoppingCart[i].VAT.Value / 100 + listShoppingCart[i].UnitPrice * listShoppingCart[i].Quantity;
                    totalPrice += listShoppingCart[i].UnitPrice * listShoppingCart[i].Quantity * listShoppingCart[i].VAT.Value / 100
                        + listShoppingCart[i].UnitPrice * listShoppingCart[i].Quantity;

                }

                Debug.WriteLine("van co item");
            }

            hashtable.Add(SessionName.TotalQuantity, totalQuantity);
            hashtable.Add(SessionName.TotalPrice, totalPrice);

            return hashtable;
        }

        public bool createOrder(List<Product> listShoppingCart, DateTime createAt, Customer customer, int totalQuantity, double totalPrice, 
            string comment, int? userId)
        {
            try
            {
                var address = "";
                if (!customer.Tinh.IsNullOrWhiteSpace())
                {
                    address += customer.Tinh + ", ";
                }

                if (!customer.Huyen.IsNullOrWhiteSpace())
                {
                    address += customer.Huyen + ", ";
                }
                address += customer.Address;

                var order = new Order()
                {
                    RoleNumber = createAt.ToFileTimeUtc().ToString(),
                    OrderDate = createAt,
                    RequireDate = null,
                    ShippedDate = null,
                    Quantity = totalQuantity,
                    TotalPrice = totalPrice,
                    Status = Order.StatusOrder.DeActive,
                    UserID = userId,
                    Commnet = comment,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    Address = address,
                    Phone = customer.Phone
                };

                return _orderRespository.createOrder(listShoppingCart, order);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}