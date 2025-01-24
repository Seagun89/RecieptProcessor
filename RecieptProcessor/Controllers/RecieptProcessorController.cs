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

            return Ok(new {id = Id});
        }

        [HttpGet("receipts/{id}/getPoints")]
        public IActionResult GetPoints(string id)
        {
            var pointCounter = 0;
            if (dBContext.ContainsKey(id))
            {
                var receipt = dBContext[id];
                if (receipt.Total != null)
                {
                    if (double.Parse(receipt.Total) % 1 == 0)
                    {
                        pointCounter += 50;
                    }
                    if (double.Parse(receipt.Total) % .25 == 0)
                    {
                        pointCounter += 25;
                    }
                }
                if (receipt.Items != null)
                {
                    var pairItems = Math.Floor((decimal)receipt.Items.Count / 2);
                    pointCounter += (int)pairItems * 5;
                }
            }
            else
            {
                return NotFound();
            }

            return Ok(new {points = pointCounter});
        }
    }
}
