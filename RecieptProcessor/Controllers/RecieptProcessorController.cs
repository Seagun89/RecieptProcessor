using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using RecieptProcessor.Models;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace RecieptProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecieptProcessorController : Controller
    {
        private string? Id;
        private static readonly Dictionary<string, RecieptDto> dBContext = [];

        public RecieptProcessorController()
        {
        }

        [HttpPost("receipts/process")]
        public IActionResult Process([FromBody] RecieptDto reciept)
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
                    var trimSpaces = item.ShortDescription?.Length - item.ShortDescription?.Trim().Length;

                    // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer.
                    // The result is the number of points earned.
                    if (trimSpaces != 0 && item.Price != null && item.ShortDescription?.Trim().Length % 3 == 0)
                    {
                        var value = (int)Math.Ceiling(double.Parse(item.Price) * .2);
                        pointCounter += value;
                    }
                }
            }

            return pointCounter;
        }
        #endregion
    }
}
