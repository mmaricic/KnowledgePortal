namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Channel")]
    public partial class Channel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Channel()
        {
            Subscribed = new HashSet<Subscribed>();
            Distributed_Question = new HashSet<Distributed_Question>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public DateTime? OpenedAt { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        public DateTime? ClosedAt { get; set; }

        public int StudentsNum { get; set; }

        [Required]
        [StringLength(128)]
        public string TeacherId { get; set; }

        public int? timeLimit { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscribed> Subscribed { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Distributed_Question> Distributed_Question { get; set; }
    }
}
