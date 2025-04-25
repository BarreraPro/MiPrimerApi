using MiPrimerAPI_.Models;
using MiPrimerAPI_.Repositories;
namespace MiPrimerAPI_.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Product>> GetAllProducts()
        {
            return _repo.GetAllAsync();
        }

        public Task<Product?> GetProductById(int id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task AddProduct(Product product)
        {
            return _repo.AddAsync(product);
        }
    }

}

