namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Response")]
    public partial class Response
    {
        public DateTime SentAt { get; set; }

        public int AnswerId { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QuestionId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string StudentId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Channelid { get; set; }

        public bool? Correct { get; set; }

        public virtual Distributed_Answer Distributed_Answer { get; set; }

        public virtual Distributed_Question Distributed_Question { get; set; }

        public virtual Subscribed Subscribed { get; set; }
    }
}
