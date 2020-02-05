using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Controllers.Respository.ProductManager;
using Alithea3.Models;

namespace Alithea3.Controllers.Service.ProductManager
{
    public class ProductService : BaseService<Product>
    {
        private ProductRepository _productRepository = new ProductRepository();

        public List<Product> listPagination(int? page, int? limit)
        {
            if (page == null)
            {
                page = 1;
            }

            if (limit == null)
            {
                limit = 10;
            }

            return _productRepository.listPagination((page.Value - 1) * limit.Value, limit.Value);
        }

        public bool AddProductAndListCategoryOfProduct(Product product, List<int> catId, List<int> idColor, List<int> quanityColor, List<double> priceColor, List<string> imageColor)
        {
            if (product.VAT == null || product.VAT < 0)
            {
                product.VAT = 0;
            }
            return _productRepository.AddProductAndListCategoryOfProduct(product, catId, idColor, quanityColor, priceColor, imageColor);
        }

        public bool UpdateProduct(Product product, List<int> catId, List<int> idColor, List<int> quanityColor, List<double> priceColor, List<string> imageColor)
        {
            if (product.VAT == null || product.VAT < 0)
            {
                product.VAT = 0;
            }
            return _productRepository.UpdateProduct(product, catId, idColor, quanityColor, priceColor, imageColor);
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var product = _productRepository.SelectById(id);
                product.DeletedAt = DateTime.Now;
                product.Status = Product.ProductStatus.Deleted;

                _productRepository.Update(product);
                _productRepository.Save();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}