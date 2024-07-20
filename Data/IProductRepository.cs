using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductRepository
    {
        public bool SaveChanges();

        public void AddEntity<T>(T entityToAdd);

        public void RemoveEntity<T>(T entityToRemove);

        public IEnumerable<Product> GetProducts();

        public Product GetSingleProduct(int productId);
    }
}