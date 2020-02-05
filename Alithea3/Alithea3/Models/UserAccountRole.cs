using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Alithea3.Models
{
    public class UserAccountRole
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("UserAccount")]
        public int UserID { get; set; }
        public virtual UserAccount UserAccount { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}