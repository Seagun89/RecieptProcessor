using Microsoft.AspNetCore.Mvc;
using ReceiptProcessor.Models;

namespace ReceiptProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptProcessorController : Controller
    {
        private string? Id;
        private static readonly Dictionary<string, ReceiptDto> dBContext = [];

        public ReceiptProcessorController()
        {
        }

        [HttpPost("receipts/process")]
        public IActionResult Process([FromBody] ReceiptDto reciept)
        {
            try
            {
                if (reciept.Retailer == null || reciept.Retailer == string.Empty)
                {
                    throw new Exception();
                }
                else
                {
                    Id = Guid.NewGuid().ToString();
                    dBContext[Id] = reciept;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(new { id = Id });
        }

        [HttpGet("receipts/{id}/getPoints")]
        public IActionResult GetPoints(string id)
        {
            if (dBContext.ContainsKey(id))
            {
                return Ok(new { points = pointCompiler(id) });
            }
            else
            {
                return NotFound();
            }
        }

        #region Helper Methods
        private int pointCompiler(string id)
        {
            var pointCounter = 0;
            var receipt = dBContext[id];

            if (receipt.Retailer != null)
            {
                // 1 point for every alphanumeric character
                pointCounter = receipt.Retailer.Count(c => char.IsLetterOrDigit(c));
            }
           
            if (receipt.Total != null)
            {
                // 50 points if the total is a round dollar amount with no cents
                if (double.Parse(receipt.Total) % 1 == 0)
                {
                    pointCounter += 50;
                }
                // 25 points if the total is a multiple of 0.25
                if (double.Parse(receipt.Total) % .25 == 0)
                {
                    pointCounter += 25;
                }
            }
           
            if (receipt.Items != null)
            {
                // 5 points for every two items on the receipt.
                if (receipt.Items.Count > 0)
                {
                    var pairItems = Math.Floor((decimal)receipt.Items.Count / 2);
                    pointCounter += (int)pairItems * 5;
                }

                foreach (var item in receipt.Items)
                {
                    var trimSpaces = item.ShortDescription?.Trim().Length;

                    // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer.
                    // The result is the number of points earned.
                    if (trimSpaces != 0 && item.Price != null && item.ShortDescription?.Trim().Length % 3 == 0)
                    {
                        var value = (int)Math.Ceiling(double.Parse(item.Price) * .2);
                        pointCounter += value;
                    }
                }
            }

            if (receipt.PurchaseDate != null)
            {
                int removingLastDigit = receipt.PurchaseDate.Length - 1;
                // removing last character since it's only neccessary to check for odd days
                var dayLastDigit = int.Parse(receipt.PurchaseDate.Substring(removingLastDigit));

                // 6 points if the day in the purchase date is odd
                if (dayLastDigit % 2 == 1)
                {
                    pointCounter += 6;
                }
            }

            if (receipt.PurchaseTime != null)
            {
                int amountToSubstring = 2; // first 2 digits in the string
                int beginTime = 14; // value for 2pm
                int endTime = 16; // value for 4pm
                var first2Digits = int.Parse(receipt.PurchaseTime.Substring(0, amountToSubstring));

                if (first2Digits >= beginTime && first2Digits <= endTime)
                {
                    pointCounter += 10;
                }
            }
            return pointCounter;
        }
        #endregion
    }
}
