using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProduct(long id);
        bool Delete(long id, string userId);
        Product AddProduct(Product product);
        Product UpdateProduct(long id, Product product);
    }
}