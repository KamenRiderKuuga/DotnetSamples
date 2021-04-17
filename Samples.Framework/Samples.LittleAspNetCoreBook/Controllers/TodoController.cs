using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Samples.LittleAspNetCoreBook.Models;
using Samples.LittleAspNetCoreBook.Services;

namespace Samples.LittleAspNetCoreBook.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;

        public TodoController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _todoItemService.GetIncompleteItemsAsync();

            var model = new TodoViewModel
            {
                Items = items,
            };

            return View(model);
        }
    }
}
