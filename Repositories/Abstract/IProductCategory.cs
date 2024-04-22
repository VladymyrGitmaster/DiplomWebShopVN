using DiplomWebShopVN.Models;

namespace DiplomWebShopVN.Repositories.Abstract
{
    public interface IProductCategory
    {
        IEnumerable<Category> AllCategories { get; }
    }
}
