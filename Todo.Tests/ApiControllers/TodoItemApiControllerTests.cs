using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Todo.Controllers;
using Todo.Data;
using Todo.Data.Entities;
using Xunit;

namespace Todo.Tests.Controllers
{
    public class TodoItemApiControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly TodoItemApiController _controller;

        public TodoItemApiControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TodoDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new TodoItemApiController(_context);
        }

        [Fact]
        public async Task Create_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new TodoItemRequest
            {
                TodoListId = 1,
                Title = "Test Title",
                ResponsiblePartyId = "user1",
                Importance = Importance.Medium,
                Rank = 1
            };

            // Act
            var result = await _controller.Create(request) as ActionResult;

            // Assert
            Assert.NotNull(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var createdItem = okResult.Value as TodoItem;
            Assert.NotNull(createdItem);
            Assert.Equal(request.Title, createdItem.Title);
            Assert.Equal(request.ResponsiblePartyId, createdItem.ResponsiblePartyId);
            Assert.Equal(request.Importance, createdItem.Importance);
            Assert.Equal(request.Rank, createdItem.Rank);
        }

        [Fact]
        public async Task Create_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new TodoItemRequest
            {
                TodoListId = 1,
                Title = "", // Invalid title
                ResponsiblePartyId = "user1",
                Importance = Importance.Medium,
                Rank = 1
            };

            // Act
            var result = await _controller.Create(request) as ActionResult;

            // Assert
            Assert.NotNull(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid data.", badRequestResult.Value);
        }
    }
}