using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Dtos;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductEFController : ControllerBase
{
    private IProductRepository _productRepository;
    private IMapper _mapper;

    public ProductEFController(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductToAddDto, Product>();
        }));
    }

    [HttpGet("GetProducts")]
    public IEnumerable<Product> GetProducts()
    {
        IEnumerable<Product> products = _productRepository.GetProducts();
        return products;
    }

    [HttpGet("GetSingleProduct/{productId}")]
    public Product GetSingleProduct(int productId)
    {
        Product product = _productRepository.GetSingleProduct(productId);
        return product;
    }

    [HttpPut("EditProduct")]
    public IActionResult EditProduct(Product product)
    {
        Product productDb = _productRepository.GetSingleProduct(product.ProductId);

        if (productDb != null)
        {
            productDb.Name = product.Name;
            productDb.Description = product.Description;
            productDb.Price = product.Price;
            productDb.Active = product.Active;

            if (_productRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Edit Product");
        }
        throw new Exception("Failed to Get Product");
    }

    [HttpPost("AddProduct")]
    public IActionResult AddProduct(ProductToAddDto product)
    {
        Product productDb = _mapper.Map<Product>(product);

        _productRepository.AddEntity<Product>(productDb);

        if (_productRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add Product");
    }

    [HttpDelete("DeleteProduct")]
    public IActionResult DeleteProduct(int productId)
    {
        Product product = _productRepository.GetSingleProduct(productId);

        if (product != null)
        {
            _productRepository.RemoveEntity<Product>(product);

            if (_productRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete Product");
        }
        throw new Exception("Failed to Get Product");
    }
}