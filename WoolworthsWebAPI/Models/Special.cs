using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WoolworthsWebAPI.Models
{
    public class Special
    {
        [Required]
        [JsonProperty("quantities")]
        public List<ProductQuantities> Quantities { get; set; }

        [IgnoreDataMember]
        public bool IsValid { get; set; }

        [IgnoreDataMember]
        public long TotalValidQty { get; set; }

        [Required]
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}
