using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WoolworthsWebAPI.Models
{
    public class Product
    {
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }


        [JsonProperty("price")]
        [Required]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        [Required]
        public long Quantity { get; set; }

    }


}
