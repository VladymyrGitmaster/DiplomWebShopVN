using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DiplomWebShopVN.Repositories.Implementation
{
    public class ProductRepository : IAllProducts
    {
        private readonly DatabaseContext appDbContext;

        public ProductRepository (DatabaseContext _appDbContext)
        {
            this.appDbContext = _appDbContext;
        }

        public IEnumerable<Product> ListProducts => appDbContext.Products.Include(c=>c.ProductCategory);

        public Product getObjectProdct(int productId) => appDbContext.Products.FirstOrDefault(p => p.Id == productId);
       
    }
}
