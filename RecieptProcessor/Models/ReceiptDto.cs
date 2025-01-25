using ReceiptProcessor.Models;

namespace ReceiptProcessor.Models
{
    public class ReceiptDto
    {
        public string? Retailer { get; set; }
        public string? PurchaseDate { get; set; }
        public string? PurchaseTime { get; set; }
        public string? Total { get; set; }
        public List<ItemDto>? Items { get; set; }
    }
}
