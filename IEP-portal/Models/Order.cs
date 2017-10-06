namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int Id { get; set; }

        public int Tokens { get; set; }

        public decimal Price { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public DateTime? creationTime { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
