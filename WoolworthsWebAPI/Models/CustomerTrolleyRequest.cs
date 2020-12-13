using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WoolworthsWebAPI.Models
{
    public class CustomerTrolleyRequest
    {
        [Required]
        [JsonProperty("products")]
        public List<TrolleyProduct> Products { get; set; }

        [Required]
        [JsonProperty("specials")]
        public List<Special> Specials { get; set; }

        [Required]
        [JsonProperty("quantities")]
        public List<ProductQuantities> Quantities { get; set; }
    }
}
