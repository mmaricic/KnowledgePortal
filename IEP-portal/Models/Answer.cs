namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Answer")]
    public partial class Answer
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [StringLength(1)]
        public string Mark { get; set; }

        public bool IsCorrect { get; set; }

        public int Number { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }
    }
}
