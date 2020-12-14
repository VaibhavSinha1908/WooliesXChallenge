using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WoolworthsWebAPI.Models
{
    public class TrolleyProduct
    {
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }


        [JsonProperty("price")]
        [Required]
        public decimal Price { get; set; }
    }
}
