using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IEP_portal.Models
{
    [MetadataType(typeof(QuestionMetaData))]
    public partial class Question
    {
        [NotMapped]
        [Display(Name = "Image")]
        public HttpPostedFileBase UploadImage { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public HttpPostedFileBase NewImage { get; set; }

        [NotMapped]
        public bool Distribute { get; set; }
    }

    public class QuestionMetaData
    {
        [Display(Name ="Locked")]
        public bool IsLocked { get; set; } = false;

        [Display(Name ="Created at")]
        public DateTime CreatedAt { get; set; }


        [Display(Name = "Locked at")]
        public DateTime? LockedAt { get; set; }


        [Display(Name = "Updated at")]
        public DateTime? UpdatedAt { get; set; }
    }

    [MetadataType(typeof(ResponseMetaData))]
    public partial class Response
    { }


    public class ResponseMetaData
    {
        [Display(Name = "Sent at")]
        public DateTime SentAt { get; set; }

        [Display(Name = " Correct answer")]
        public bool? Correct { get; set; }
    }

    [MetadataType(typeof(OrderMetaData))]
    public partial class Order
    { }

    public class OrderMetaData
    {
        [Display(Name = "Time of creation")]
        public DateTime creationTime { get; set; }
    }

    [MetadataType(typeof(DistributedQuestionMetaData))]
    public partial class Distributed_Question
    { }

    public class DistributedQuestionMetaData
    {
        [Display(Name = "Channel ID")]
        public int ChannelId { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }

    }

    [MetadataType(typeof(DistributedAnswerMetaData))]
    public partial class Distributed_Answer
    { }

    public class DistributedAnswerMetaData
    {
        [Display(Name = "Correct Answer")]
        public int IsCorrect { get; set; }
    }

    [MetadataType(typeof(AspNetUsersMetaData))]
    public partial class AspNetUsers
    { }

    public class AspNetUsersMetaData
    {
        [Display(Name = "Active account")]
        public bool isActive { get; set; }

    }

    [MetadataType(typeof(ChannelMetaData))]
    public partial class Channel
    { }

    public class ChannelMetaData
    {
        [Display(Name = "Opened at")]
        public DateTime? OpenedAt { get; set; }

        [Display(Name ="Closed at")]
        public DateTime? ClosedAt { get; set; }

        [Display(Name = "Students subscribed")]
        public int StudentsNum { get; set; }

        [Display(Name = "Auto-close channel(in minutes)")]
        public int? timeLimit { get; set; }
    }

    [MetadataType(typeof(AnswerMetaData))]
    public partial class Answer
    { }

    public class AnswerMetaData
    {
        [Display(Name = "Correct Answer")]
        public int IsCorrect { get; set; }

    }

    [MetadataType(typeof(ParametersMetaData))]
    public partial class Parameters
    { }

    public class ParametersMetaData
    {
        [Display(Name = "K - number of answers for question")]
        public int? K { get; set; }

        [Display(Name = "M - price for unlocking question")]
        public int? M { get; set; }

        [Display(Name = "E - price for auto-evaluation of response")]
        public int? E { get; set; }

        [Display(Name = "S - number of tokens in silver package")]
        public int? S { get; set; }

        [Display(Name = "G - number of tokens in gold package")]
        public int? G { get; set; }

        [Display(Name = "P - number of tokens in platinum package")]
        public int? P { get; set; }
    }

}