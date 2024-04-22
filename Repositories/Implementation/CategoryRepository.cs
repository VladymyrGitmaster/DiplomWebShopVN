using DiplomWebShopVN.Models;
using DiplomWebShopVN.Models.Domain;
using DiplomWebShopVN.Repositories.Abstract;

namespace DiplomWebShopVN.Repositories.Implementation
{
    public class CategoryRepository : IProductCategory
    {
        private readonly DatabaseContext appDbContext;

        public CategoryRepository(DatabaseContext _appDbContext)
        {
            this.appDbContext = _appDbContext;
        }
        public IEnumerable<Category> AllCategories => appDbContext.Categories;
    }
}
