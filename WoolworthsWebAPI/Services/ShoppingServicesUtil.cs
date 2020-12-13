using System.Collections.Generic;
using System.Linq;
using WoolworthsWebAPI.Models;

namespace WoolworthsWebAPI.Services
{
    public class ShoppingServicesUtil
    {
        public static List<Product> SortProductsLowToHigh(List<Product> prodList)
        {
            var result = prodList.OrderBy(x => x.Price).ToList();
            return result;
        }

        public static List<Product> SortProductsHighToLow(List<Product> prodList)
        {
            var result = prodList.OrderByDescending(x => x.Price).ToList();
            return result;
        }

        public static List<Product> SortProductsAToZ(List<Product> prodList)
        {
            var result = prodList.OrderBy(x => x.Name).ToList();
            return result;
        }


        public static List<Product> SortProductsZToA(List<Product> prodList)
        {
            var result = prodList.OrderByDescending(x => x.Name).ToList();
            return result;
        }

        /// <summary>
        /// A method to sort the shopping history of customers based on popularity i.e. quantity of each product item. The returns the products
        /// from highest numbers sold (most recommended) to least sold (least recommended).
        /// </summary>
        /// <param name="customerShoppingHistoryList"></param>
        /// <returns></returns>
        public static List<Product> SortRecommendedProducts(List<CustomerShoppingHistory> customerShoppingHistoryList)
        {
            List<Product> productList = new List<Product>();
            List<Product> sortedRecommendedProductList = new List<Product>();

            foreach (var item in customerShoppingHistoryList)
            {
                foreach (var product in item.Products)
                {
                    if (productList.Exists(x => x.Name == product.Name))
                    {
                        var existingProduct = productList.Where(x => x.Name == product.Name).FirstOrDefault();
                        existingProduct.Quantity += product.Quantity;
                    }
                    else
                    {
                        productList.Add(product);
                    }
                }
            }

            sortedRecommendedProductList = productList.OrderByDescending(x => x.Quantity).ToList();

            return sortedRecommendedProductList;
        }
    }
}
