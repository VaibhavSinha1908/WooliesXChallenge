using Newtonsoft.Json;
using System.Collections.Generic;

namespace WoolworthsWebAPI.Models
{
    public class CustomerShoppingHistory
    {
        [JsonProperty("customerId")]
        public long CustomerId { get; set; }

        [JsonProperty("products")]
        public List<Product> Products { get; set; }
    }
}
