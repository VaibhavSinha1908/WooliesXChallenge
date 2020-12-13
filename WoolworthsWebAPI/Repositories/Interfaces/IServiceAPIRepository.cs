using System.Collections.Generic;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;

namespace WoolworthsWebAPI.Repositories
{
    public interface IServiceAPIRepository
    {
        Task<List<Product>> GetProductListAsync();
        Task<List<CustomerShoppingHistory>> GetCustomerShoppingHistoryAsync();
        Task<decimal> GetLowestPrice(CustomerTrolleyRequest request);
    }
}
