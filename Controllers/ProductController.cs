using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using ProductApi.Data;
using ProductApi.Dtos;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    DataContextDapper _dapper;

    public ProductController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetProducts")]
    public IEnumerable<Product> GetProducts()
    {
        string sqlSelectProducts = @"
            SELECT [ProductId],
            [Name],
            [Description],
            [Price],
            [Active]
            FROM MyAppSchema.Products";
        IEnumerable<Product> products = _dapper.LoadData<Product>(sqlSelectProducts);
        return products;
    }

    [HttpGet("GetSingleProduct/{productId}")]
    public Product GetSingleProduct(int productId)
    {
        string sqlSelectSingleProduct = @"
            SELECT [ProductId],
            [Name],
            [Description],
            [Price],
            [Active]
            FROM MyAppSchema.Products
                WHERE ProductId = " + productId.ToString();

        Product product = _dapper.LoadDataSingle<Product>(sqlSelectSingleProduct);
        return product;
    }

    [HttpPut("EditProduct")]
    public IActionResult EditProduct(Product product)
    {
        string sqlUpdateProduct = @"
            UPDATE MyAppSchema.Products
                SET [Name] = '" + product.Name +
                "', [Description] = '" + product.Description +
                "', [Price] = '" + product.Price +
                "', [Active] = '" + product.Active +
                "' WHERE ProductId = " + product.ProductId.ToString();

        if (_dapper.ExecuteSql(sqlUpdateProduct))
        {
            return Ok();
        }
        throw new Exception("Failed to Update Product");
    }

    [HttpPost("AddProduct")]
    public IActionResult AddProduct(ProductToAddDto productToAdd)
    {
        string sqlInsertProduct = @"
            INSERT INTO MyAppSchema.Products(
                [Name],
                [Description],
                [Price],
                [Active]
                ) VALUES (" +
                    "'" + productToAdd.Name +
                    "', '" + productToAdd.Description +
                    "', '" + productToAdd.Price +
                    "', '" + productToAdd.Active +
                    "')";

        if (_dapper.ExecuteSql(sqlInsertProduct))
        {
            return Ok();
        }
        throw new Exception("Failed to Add Product");
    }

    [HttpDelete("DeleteProduct/{productId}")]
    public IActionResult DeleteProduct(int productId)
    {
        string sqlDelete = @"
            DELETE FROM MyAppSchema.Products
                WHERE productId = " + productId.ToString();

        if (_dapper.ExecuteSql(sqlDelete))
        {
            return Ok();
        }
        throw new Exception("Failed to Delete Product");
    }
}