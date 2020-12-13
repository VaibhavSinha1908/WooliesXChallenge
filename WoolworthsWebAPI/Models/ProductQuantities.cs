using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WoolworthsWebAPI.Models
{
    public class ProductQuantities
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("quantity")]
        public long Quantity { get; set; }
    }
}
