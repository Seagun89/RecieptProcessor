using RecieptProcessor.Models;

namespace RecieptProcessor.Models
{
    public class RecieptDto
    {
        public string? Retailer { get; set; }
        public string? PurchaseDate { get; set; }
        public string? PurchaseTime { get; set; }
        public string? Total { get; set; }
        public List<ItemDto>? Items { get; set; }
    }
}
