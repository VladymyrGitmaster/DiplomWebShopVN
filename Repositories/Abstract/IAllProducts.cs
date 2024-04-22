using DiplomWebShopVN.Models;

namespace DiplomWebShopVN.Repositories.Abstract
{
    public interface IAllProducts
    {
        IEnumerable<Product> ListProducts { get; }
        Product getObjectProdct(int productId); 

    }
}
