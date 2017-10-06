namespace IEP_portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Parameters
    {
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive number")]
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive number")]
        public int? K { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive number")]
        public int? M { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive number")]
        public int? E { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive number")]
        public int? S { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Paramater must be positive numberr")]
        public int? G { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "PParamater must be positive number")]
        public int? P { get; set; }
    }
}
