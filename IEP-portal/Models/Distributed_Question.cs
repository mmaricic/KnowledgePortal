namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Distributed_Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Distributed_Question()
        {
            Distributed_Answer = new HashSet<Distributed_Answer>();
            Response = new HashSet<Response>();
        }

        public int ChannelId { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public byte[] Picture { get; set; }

        public string Text { get; set; }

        public int? ParentId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Channel Channel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Distributed_Answer> Distributed_Answer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Response> Response { get; set; }

        public virtual Question Question { get; set; }
    }
}
