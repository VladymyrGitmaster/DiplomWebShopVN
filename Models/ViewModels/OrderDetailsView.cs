using DiplomWebShopVN.Models.Domain;

namespace DiplomWebShopVN.Models.ViewModels
{
    public class OrderDetailsView
    {
        public Order CurrentOrder { get; set; } = null!;
        public List<BasketElement> OrderProducts { get; set; } = null!;
        public int object_number { get; set; }
    }
}

