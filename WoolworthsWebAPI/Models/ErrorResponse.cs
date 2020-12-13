using System.Collections.Generic;

namespace WoolworthsWebAPI.Models
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
