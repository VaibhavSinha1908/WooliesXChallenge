using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;
using WoolworthsWebAPI.Services.Interfaces;

namespace WoolworthsWebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly IShoppingService service;
        private readonly ILogger<ShoppingController> logger;

        public ShoppingController(IShoppingService service, ILogger<ShoppingController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        /// <summary>
        /// GET: To get the username and token for the user.
        /// </summary>
        /// <returns></returns>

        [HttpGet("user")]
        public async Task<IActionResult> GetAsync()
        {
            var result = await service.GetUserDataAsync();
            return Ok(result);

        }

        /// <summary>
        /// GET: To get sorted product list based on QueryString param sortOption.
        /// </summary>
        /// <remarks>
        /// sortOptions: {Low, High, Ascending, Descending, Recommended}
        /// </remarks>
        /// <param name="sortOption"></param>
        /// <returns>Sorted order of the products.</returns>
        /// <response code = "200">Sorted Order of the Product list</response>

        [HttpGet("sort")]
        public async Task<IActionResult> GetSortedProductOrderAsync([FromQuery] SortOptionRequest sortOptionRequest)
        {
            var result = await service.GetOrderedProductListAysnc(sortOptionRequest.SortOption);
            return Ok(result);
        }

        // POST: api/trolleyTotal
        /// <summary>
        /// POST: Calculates the least trolley total for the given shopping cart.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The least possible trolleyCost for the given list of Products, Specials, Quantities</returns>
        /// <response code = "200">Least Cost of the Trolley</response>
        /// <response code = "400">Bad Response</response>
        /// <response code = "500">Internal server error.(in case of exception)</response>
        [HttpPost("trolleytotal")]
        public async Task<IActionResult> PostAsync([FromBody] CustomerTrolleyRequest request)
        {
            var result = await service.GetLowestTrolleyTotalAsync3(request);
            return Ok(result);
        }
    }
}
