using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WStoreDataManagement.Library.Internal.DataAccess;
using WStoreDataManagement.Library.Models;

namespace WStoreDataManagement.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration _configuration;
        public SaleData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            // TODO: make it more SOLID (just separate pieces of logic)
            // TODO: create a dependency injection against theese direct dependencies

            // TODO: create a dependency injection against theese direct dependencies
            ProductData productData = new ProductData(_configuration);
            var taxRate = ConfigHelper.GetTaxRate();
            
            // Start filling in the models we will save in database
            // 1. Fill the available information 
            List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                // Get the actual information about this ProductId
                var ProductInfo = productData.GetProductById(item.ProductId);
                if (ProductInfo == null)
                {
                    throw new Exception($"The product with the Id = {item.ProductId} not found in database");
                }

                detail.PurchasePrice = ProductInfo.RetailPrice * item.Quantity;
                if (ProductInfo.IsTaxable)
                    detail.Tax = taxRate * detail.PurchasePrice;

                details.Add(detail);
            }

            // 2. Create the SaleModel
            SaleDBModel sale = new SaleDBModel()
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId,

            };

            sale.Total = sale.SubTotal + sale.Tax;

            using (SQLDataAccess sql = new SQLDataAccess(_configuration))
            {
                try
                {
                    sql.StartTransaction("WStoreData");

                    // 3. Save the Sale model
                    sql.SaveDataInTransation<SaleDBModel>("dbo.spSale_Insert", sale);

                    // 4. Get ID for Sale model
                    sale.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_GetByCashierIdAndSaleDate", new { sale.CashierId, sale.SaleDate })
                        .FirstOrDefault();

                    // 5. Finish filling the details of Sale model
                    // And save the sale model details
                    foreach (var item in details)
                    {
                        item.SaleId = sale.Id;
                        sql.SaveDataInTransation<SaleDetailDBModel>("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }

        public List<SaleReportModel> GetSaleReport()
        {
            using (SQLDataAccess sql = new SQLDataAccess(_configuration))
            {
                var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "WStoreData");

                return output;
            }
        }
    }
}
