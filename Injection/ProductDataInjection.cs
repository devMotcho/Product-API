using Newtonsoft.Json;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi;

public class ProductDataInjection
{
    DataContextDapper _dapper;

    public ProductDataInjection(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    public void SetProductData()
    {
        IEnumerable<Product>? productsDb = _dapper.LoadData<Product>("SELECT * FROM MyAppSchema.Products");
        if (productsDb == null)
        {

            string productJson = System.IO.File.ReadAllText("Products.json");

            IEnumerable<Product>? products = JsonConvert.DeserializeObject<IEnumerable<Product>>(productJson);

            if (products != null)
            {
                string sqlProducts = @"
                    SET IDENTITY_INSERT MyAppSchema.Products ON;"
                    + "INSERT INTO MyAppSchema.Products (ProductId"
                    + ", Name"
                    + ", Description"
                    + ", Price"
                    + ", Active)"
                    + "VALUES";

                foreach (Product product in products)
                {
                    string sqlToAdd = "('" + product.ProductId
                        + "', '" + product.Name?.Replace("'", "''")
                        + "', '" + product.Description?.Replace("'", "''")
                        + "', '" + product.Price
                        + "', '" + product.Active
                        + "')";

                    string sqlFull = sqlProducts + sqlToAdd;

                    _dapper.ExecuteSql(sqlFull);
                    Console.Write(sqlFull);
                }
            }
        }
    }
}