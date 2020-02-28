using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Alithea3.Models.ViewModel
{
    public class OrdersViewModel
    {
        public int OrderID { get; set; }

        [DisplayName("Mã đặt hàng")]
        public string RoleNumber { get; set; }

        [DisplayName("Ngày đặt hàng")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OrderDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RequireDate { get; set; }

        [DisplayName("Ngày giao hàng")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ShippedDate { get; set; }

        [DisplayName("Tổng số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Tổng đơn hàng")]
        [DataType(DataType.Currency)]
        public double TotalPrice { get; set; }

        [DisplayName("Chú thích")]
        public string Commnet { get; set; }

        [ForeignKey("UserAccount")]
        public int? UserID { get; set; }

        [DisplayName("Họ tên")]
        public string FullName { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }

        public string Email { get; set; }

        [DisplayName("Trạng thái")]
        public Order.StatusOrder Status { get; set; }
    }
}