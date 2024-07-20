using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductRepository : IProductRepository
    {
        DataContextEF _entityFramework;

        public ProductRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            IEnumerable<Product> products = _entityFramework.Products.ToList<Product>();
            return products;
        }

        public Product GetSingleProduct(int productId)
        {
            Product? product = _entityFramework.Products
                .Where(p => p.ProductId == productId)
                .FirstOrDefault<Product>();
            
            if(product != null)
            {
                return product;
            }

            throw new Exception("Failed To Get Product");
        }
    }
}