using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Alithea3.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên")]
        [DisplayName("Chức vụ")]
        public string RoleName { get; set; }

        public ICollection<UserAccountRole> UserAccountRoles { get; set; }
    }
}