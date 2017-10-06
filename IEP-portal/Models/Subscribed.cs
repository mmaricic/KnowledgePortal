namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Subscribed")]
    public partial class Subscribed
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subscribed()
        {
            Response = new HashSet<Response>();
        }

        public DateTime? Time { get; set; }

        [Key]
        [Column(Order = 0)]
        public string StudentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Channelid { get; set; }

        public bool evaluation { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Channel Channel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Response> Response { get; set; }
    }
}
