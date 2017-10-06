namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Distributed_Answer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Distributed_Answer()
        {
            Response = new HashSet<Response>();
        }

        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [StringLength(1)]
        public string Mark { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public virtual Distributed_Question Distributed_Question { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Response> Response { get; set; }
    }
}
