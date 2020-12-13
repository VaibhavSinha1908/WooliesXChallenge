using System.Collections.Generic;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;

namespace WoolworthsWebAPI.Services.Interfaces
{
    public interface IShoppingService
    {
        Task<UserDataResponse> GetUserDataAsync();
        Task<List<Product>> GetOrderedProductListAysnc(string sortOption);
        Task<decimal> GetLowestTrolleyTotalAsync3(CustomerTrolleyRequest request);
        Task<decimal> GetLowestTrolleyTotalAsync(CustomerTrolleyRequest request);
    }
}
