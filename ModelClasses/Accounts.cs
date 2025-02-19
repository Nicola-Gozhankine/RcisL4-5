namespace WpfApp1.ModelClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Accounts
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Login { get; set; }

        [Required]
        [StringLength(30)]
        public string Password { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Column(TypeName = "date")]
        public DateTime LastDateAuthorization { get; set; }

        public int RoleID { get; set; }

        public int StatusID { get; set; }

        public int? BadLoginTry { get; set; }

        public bool? NewUser { get; set; }

        public virtual Roles Roles { get; set; }

        public virtual Statuses Statuses { get; set; }
    }
}
