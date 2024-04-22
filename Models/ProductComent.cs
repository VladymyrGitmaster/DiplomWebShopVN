using DiplomWebShopVN.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class ProductComent
    {
        [Key]
        public int? Id { get; set; }
        public int ProductId { get; set; }
        public string ProductUserDetail { get; set; } = null!;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public DateTime? DateComent { get; set; }
    }
}
