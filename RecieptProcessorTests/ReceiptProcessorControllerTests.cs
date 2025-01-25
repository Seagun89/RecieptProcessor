using ReceiptProcessor.Controllers;
using ReceiptProcessor.Models;
using Microsoft.AspNetCore.Mvc;

namespace ReceiptProcessorTests
{
    public class ReceiptProcessorControllerTests
    {
        //Arrange
        private ReceiptDto _receiptDto = new ReceiptDto
        {
            Retailer = "Test",
            PurchaseDate = "test",
            PurchaseTime = "test",
            Total = "test",
            Items = new List<ItemDto>()
            {
                new ItemDto
                {
                    ShortDescription = "Test",
                    Price = "Test"
                }

            }
        };

        private ReceiptProcessorController ReceiptProcessorController = new ReceiptProcessorController();

        [Fact]
        public void ReceiptShouldHaveAnID()
        {
            //Act
            var processedReceipt = ReceiptProcessorController.Process(_receiptDto);

            //Assert
            Assert.IsType<OkObjectResult>(processedReceipt);
        }

        [Fact]
        public void ReceiptIsNull()
        {
            //Arrange 
            _receiptDto.Retailer = null;

            //Act 
            var processedReceipt = ReceiptProcessorController.Process(_receiptDto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(processedReceipt);
        }

        [Fact]
        public void RecieptIdNotFound()
        {
            //Arrange 
            _receiptDto.Retailer = null;

            //Act 
            var processedReceipt = ReceiptProcessorController.GetPoints("Test");

            //Assert
            Assert.IsType<NotFoundResult>(processedReceipt);
        }
    }
}