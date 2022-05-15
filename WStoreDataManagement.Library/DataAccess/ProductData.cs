﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreDataManagement.Library.Internal.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Library.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration _configuration;

        public ProductData(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<ProductModel> GetProducts()
        {
            //TODO: create a dependency injection against theese direct dependencies
            SQLDataAccess sql = new SQLDataAccess(_configuration);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "WStoreData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            //TODO: create a dependency injection against theese direct dependencies
            SQLDataAccess sql = new SQLDataAccess(_configuration);

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "WStoreData")
                .FirstOrDefault();

            return output;
        }
    }
}
