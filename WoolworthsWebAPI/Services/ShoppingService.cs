using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;
using WoolworthsWebAPI.Repositories;
using WoolworthsWebAPI.Services.Interfaces;

namespace WoolworthsWebAPI.Services
{
    public class ShoppingService : IShoppingService
    {
        private readonly ILogger<ShoppingService> logger;
        private readonly IConfiguration configuration;
        private readonly IServiceAPIRepository serviceAPIRepository;

        public ShoppingService(ILogger<ShoppingService> logger, IConfiguration configuration, IServiceAPIRepository serviceAPIRepository)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.serviceAPIRepository = serviceAPIRepository;
        }

        public async Task<UserDataResponse> GetUserDataAsync()
        {
            try
            {
                logger.LogInformation("Initiating GetUserData...");
                //Get Values from Configuration
                string appDataUserName = configuration.GetValue<string>("ApplicationData:AppUser");
                string appDataToken = configuration.GetValue<string>("ApplicationData:AppToken");

                return new UserDataResponse { name = appDataUserName, token = appDataToken };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public async Task<List<Product>> GetOrderedProductListAysnc(string sortOption)
        {
            try
            {
                logger.LogInformation($"Getting sorted product list for {sortOption}");

                List<Product> sortedProductList;
                if (sortOption.ToLower() == "Recommended".ToLower())
                {
                    //Get Shopping history.
                    var customerShoppingHistoryList = await serviceAPIRepository.GetCustomerShoppingHistoryAsync();
                    sortedProductList = SortProducts(customerShoppingHistoryList);
                }
                else
                {
                    //get product list.
                    var productsList = await serviceAPIRepository.GetProductListAsync();
                    sortedProductList = SortProducts(productsList, sortOption);
                }
                return sortedProductList;
            }

            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        private List<Product> SortProducts(List<CustomerShoppingHistory> customerShoppingHistoryList)
        {
            return ShoppingServicesUtil.SortRecommendedProducts(customerShoppingHistoryList);
        }

        private List<Product> SortProducts(List<Product> productList, string sortOption)
        {
            try
            {
                List<Product> sortedList = new List<Product>();
                switch (sortOption.ToLower())
                {
                    case "low":
                        sortedList = ShoppingServicesUtil.SortProductsLowToHigh(productList);
                        return sortedList;

                    case "high":
                        sortedList = ShoppingServicesUtil.SortProductsHighToLow(productList);
                        return sortedList;

                    case "ascending":
                        sortedList = ShoppingServicesUtil.SortProductsAToZ(productList);
                        return sortedList;

                    case "descending":
                        sortedList = ShoppingServicesUtil.SortProductsZToA(productList);
                        return sortedList;

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }

        }

        public async Task<decimal> GetLowestTrolleyTotalAsync(CustomerTrolleyRequest request)
        {
            logger.LogInformation($"Initiating the calculation for TrolleyRequest: {JsonConvert.SerializeObject(request, Formatting.Indented)}");
            var lowestCost = await serviceAPIRepository.GetLowestPrice(request);
            return lowestCost;
        }



        //custom implementation of the TrolleyCalculator (all Scenarios working)
        public async Task<decimal> GetLowestTrolleyTotalAsync3(CustomerTrolleyRequest request)
        {
            try
            {
                logger.LogInformation($"Initiating the calculation for TrolleyRequest: {JsonConvert.SerializeObject(request, Formatting.Indented)}");

                //Prepare dictionaries.
                var productDictionary = new Dictionary<string, decimal>();
                var quantityDictionary = new Dictionary<string, decimal>();
                foreach (var product in request.Products)
                {
                    productDictionary.Add(product.Name, product.Price);
                }

                foreach (var quantity in request.Quantities)
                {
                    quantityDictionary.Add(quantity.Name, quantity.Quantity);
                }
                return findCartValue(productDictionary, quantityDictionary, request.Specials);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private decimal findCartValue(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary, List<Special> specials)
        {
            return dfs(productDictionary, quantityDictionary, specials);
        }

        private decimal dfs(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary, List<Special> specials)
        {

            //Max retail price of the trolley before discounts.
            decimal maxMrp = CalculateMaxMrp(productDictionary, quantityDictionary);
            decimal currentMin = maxMrp;
            for (int i = 0; i < specials.Count; i++)
            {
                var special = specials[i];
                //check if the special is valid
                if (IsValid(special.Quantities, quantityDictionary))
                {
                    currentMin = Math.Min(dfs(productDictionary, updatedCart(special, quantityDictionary), specials) + special.Total, currentMin);
                    foreach (var item in special.Quantities)
                    {
                        quantityDictionary[item.Name] = quantityDictionary[item.Name] + item.Quantity;
                    }
                }

            }
            return Math.Min(maxMrp, currentMin);
        }

        private Dictionary<string, decimal> updatedCart(Special special, Dictionary<string, decimal> quantityDictionary)
        {
            foreach (var item in special.Quantities)
            {
                var productQty = quantityDictionary[item.Name];
                quantityDictionary[item.Name] = productQty - item.Quantity;
            }
            return quantityDictionary;
        }

        private bool IsValid(List<ProductQuantities> specialQuantities, Dictionary<string, decimal> quantityDictionary)
        {
            foreach (var item in specialQuantities)
            {
                var productQty = quantityDictionary[item.Name];
                if (item.Quantity > productQty)
                    return false;
            }
            return true;
        }

        private decimal CalculateMaxMrp(Dictionary<string, decimal> productDictionary, Dictionary<string, decimal> quantityDictionary)
        {
            decimal price = 0;
            foreach (var key in quantityDictionary.Keys)
            {
                price += productDictionary[key] * quantityDictionary[key];
            }

            return price;
        }



        //Custom Implementation of the TrolleyCalculator for split scenario.
        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<decimal> GetLowestTrolleyTotalAsync2(CustomerTrolleyRequest request)
        {
            try
            {
                logger.LogInformation("In the GetLowestTrolleyTotalAsync() method for Request:" + request);
                decimal lowestCost = 0, totalCost = 0;

                var productList = request.Products;
                var specials = request.Specials;
                var productQuantities = request.Quantities;
                //calculate totalPrice before specials.
                foreach (var productQuantity in request.Quantities)
                {
                    //get the Price from Products List.
                    var itemPrice = productList.FirstOrDefault(x => x.Name == productQuantity.Name).Price;
                    totalCost += itemPrice * productQuantity.Quantity;
                }

                foreach (var specialProducts in specials)
                {
                    if (ProductsExistInSpecials(productList, specialProducts.Quantities))
                    {
                        foreach (var splQuantity in specialProducts.Quantities)
                        {
                            if (productQuantities.Where(x => x.Name == splQuantity.Name).Any())
                            {
                                var productQty = productQuantities.Where(x => x.Name == splQuantity.Name).FirstOrDefault().Quantity;
                                if (productQty >= splQuantity.Quantity)
                                    specialProducts.IsValid = true;
                                else
                                    specialProducts.IsValid = false;
                            }
                        }
                    }
                }
                bool specialsQtyEqualsProdQty = false;
                //check if any of the specials are valid.
                if (specials.Where(x => x.IsValid == true).Count() > 0)
                {
                    //Take the least applicable total for the Specials.
                    var applicableSpecial = specials.Where(x => x.IsValid == true).OrderBy(x => x.Total).FirstOrDefault();

                    foreach (var item in applicableSpecial.Quantities)
                    {
                        if (productQuantities.Where(x => x.Name == item.Name).Any())
                        {
                            var productQty = productQuantities.Where(x => x.Name == item.Name).FirstOrDefault().Quantity;
                            if (productQty > item.Quantity)
                            {
                                var diffQty = productQty - item.Quantity;
                                //get the price of the product item.
                                var productPrice = productList.Where(x => x.Name == item.Name).FirstOrDefault().Price;
                                var costOfProduct = diffQty * productPrice;
                                lowestCost += costOfProduct;
                            }
                            else
                            {
                                if (productQty == item.Quantity)
                                    specialsQtyEqualsProdQty = true;
                            }
                        }
                    }
                    //if the product quantity is equal to specials quantity; just the specials price.
                    if (specialsQtyEqualsProdQty)
                    {
                        lowestCost = applicableSpecial.Total;
                    }
                    else
                    {
                        // lowestCost = await serviceAPIRepository.GetLowestPrice(request);
                        return lowestCost;
                    }
                }
                else
                {
                    //no valid specials.
                    lowestCost = totalCost;
                }
                return lowestCost;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw ex;
            }
        }


        private Special GetBestApplicableSpecial(IEnumerable<Special> validSpecials, List<ProductQuantities> productQuantities)
        {
            var totalQuantities = productQuantities.Sum(x => x.Quantity);

            List<Special> applicableSpecials = new List<Special>();

            var productNames = productQuantities.Select(x => x.Name);

            foreach (var special in validSpecials)
            {
                //count the quantity of products.
                var specialQtySum = special.Quantities.Where(x => productNames.Contains(x.Name)).Sum(x => x.Quantity);
                if (specialQtySum <= totalQuantities)
                {
                    special.TotalValidQty = specialQtySum;
                    applicableSpecials.Add(special);
                }
            }

            var orderedApplicableSpecials = applicableSpecials.OrderByDescending(x => x.TotalValidQty).FirstOrDefault();
            return orderedApplicableSpecials;
        }


        private bool ProductsExistInSpecials(List<TrolleyProduct> productList, List<ProductQuantities> quantities)
        {
            bool exists = false;
            foreach (var product in productList)
            {
                if (quantities.Where(x => x.Name == product.Name).Any())
                    exists = true;
                else
                    exists = false;
            }
            return exists;
        }
    }
}
