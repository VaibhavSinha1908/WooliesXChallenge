
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WoolworthsWebAPI.Models;
using WoolworthsWebAPI.Repositories;
using WoolworthsWebAPI.Services;

namespace WoolworthsWebAPI.Test
{
    public class Tests
    {
        private ServiceAPIRepository APIRepository { get; set; }
        private ShoppingService ShoppingService { get; set; }

        public ILogger<ShoppingService> Logger { get; private set; }
        [SetUp]
        public void Setup()
        {
            Logger = new Mock<ILogger<ShoppingService>>().Object;

            //Get the configuration information by creating an instance of the IConfiguration Object.
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfiguration config = new ConfigurationBuilder()
               .SetBasePath(projectPath)
               .AddJsonFile("appsettings.json")
               .Build();

            Mock<IConfiguration> configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);


            var ApiLogger = new Mock<ILogger<ServiceAPIRepository>>().Object;

            APIRepository = new ServiceAPIRepository(new System.Net.Http.HttpClient(), ApiLogger, config);
            ShoppingService = new ShoppingService(Logger, config, APIRepository);
        }

        [Test]
        public async Task GetUserDataAsync_ShouldReturn_UserTokenAndName()
        {
            var result = await ShoppingService.GetUserDataAsync();

            Assert.NotNull(result);
            Assert.That(result.name == "Vaibhav Kumar" && result.token == "634ed577-a0f6-4da0-80db-27e3d17646ac");
        }


        [TestCase("low")]
        [TestCase("high")]
        [TestCase("ascending")]
        [TestCase("descending")]
        [TestCase("recommended")]
        public async Task GetOrderedProductListAysnc_ShouldReturn_SortedProductList(string sortOption)
        {
            var result = await ShoppingService.GetOrderedProductListAysnc(sortOption);

            Assert.NotNull(result);
            Assert.That(result.Count > 0);
            if (sortOption == "low")
            {
                Assert.That(result[0].Price < result[1].Price);
            }
            if (sortOption == "high")
            {
                Assert.That(result[0].Price > result[1].Price);
            }
        }


        [TestCase("")]
        [TestCase("12344")]
        [TestCase("aabbccdd")]
        [TestCase("#@$!@")]
        public async Task GetOrderedProductListAysnc_ShouldNOTReturn_SortedProductList(string sortOption)
        {
            var result = await ShoppingService.GetOrderedProductListAysnc(sortOption);

            Assert.IsNull(result);
        }


        [TestCase(3, 3, 4, 25)]
        [TestCase(3, 3, 3, 15)]
        [TestCase(3, 3, 0, 10)]
        [TestCase(0, 2, 2, 15)]
        public async Task GetOrderedProductListAysnc_Scenario1_ShouldReturn_LeastTrolleyCost(int qtyA, int qtyB, int qtyC, decimal expectedOutput)
        {
            var request = new CustomerTrolleyRequest();
            request.Products = new List<TrolleyProduct>
            {
                new TrolleyProduct { Name = "A", Price = 10 },
                new TrolleyProduct { Name = "B", Price = 10 },
                new TrolleyProduct { Name = "C", Price = 10 },
            };

            request.Specials = new List<Special> {
                new Special
                {
                    Quantities = new List<ProductQuantities>{
                        new ProductQuantities {Name="A", Quantity=qtyA},
                        new ProductQuantities {Name="B", Quantity=qtyB},
                        new ProductQuantities {Name="C", Quantity=0}
                    },
                    Total = 10

                },
                new Special
                {
                    Quantities = new List<ProductQuantities>{
                        new ProductQuantities {Name="A", Quantity=qtyA},
                        new ProductQuantities {Name="B", Quantity=qtyB},
                        new ProductQuantities {Name="C", Quantity=qtyB}
                    },
                    Total = 15
                },
                new Special
                {
                    Quantities = new List<ProductQuantities>{
                        new ProductQuantities {Name="A", Quantity=qtyA-1},
                        new ProductQuantities {Name="B", Quantity=qtyB-2},
                        new ProductQuantities {Name="C", Quantity=qtyB-1}
                    },
                    Total = 5
                },
            };

            request.Quantities = new List<ProductQuantities> {
                new ProductQuantities{ Name = "A", Quantity=qtyA},
                new ProductQuantities{ Name = "B", Quantity=qtyB},
                new ProductQuantities{ Name = "C", Quantity=qtyC}
            };

            var result = await ShoppingService.GetLowestTrolleyTotalAsync(request);

            Assert.That(result == expectedOutput);
        }




        [TestCase(3, 3, 25)]
        [TestCase(4, 1, 20)]
        [TestCase(101, 100, 25)]
        [TestCase(1, 1, 20)]
        public async Task GetOrderedProductListAysnc_Scenario2_ShouldReturn_LeastTrolleyCost(int qtyA, int qtyB, decimal expectedOutput)
        {
            var request = new CustomerTrolleyRequest();
            request.Products = new List<TrolleyProduct>
            {
                new TrolleyProduct { Name = "A", Price = 10 },
                new TrolleyProduct { Name = "B", Price = 10 }
            };

            request.Specials = new List<Special> {
                new Special
                {
                    Quantities = new List<ProductQuantities>{
                        new ProductQuantities {Name="A", Quantity=qtyA},
                        new ProductQuantities {Name="B", Quantity=0}

                    },
                    Total = 10

                },
                new Special
                {
                    Quantities = new List<ProductQuantities>{
                        new ProductQuantities {Name="A", Quantity=0},
                        new ProductQuantities {Name="B", Quantity=qtyB}

                    },
                    Total = 15
                }
            };

            request.Quantities = new List<ProductQuantities> {
                new ProductQuantities{ Name = "A", Quantity=qtyA},
                new ProductQuantities{ Name = "B", Quantity=qtyB}

            };

            var result = await ShoppingService.GetLowestTrolleyTotalAsync(request);

            Assert.That(result == expectedOutput);
        }
    }
}