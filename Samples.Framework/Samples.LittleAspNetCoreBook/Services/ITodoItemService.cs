using System;
using System.Threading.Tasks;
using Samples.LittleAspNetCoreBook.Models;

namespace Samples.LittleAspNetCoreBook.Services
{
    public interface ITodoItemService
    {
        Task<TodoItem[]> GetIncompleteItemsAsync();

        Task<bool> AddItemAsync(TodoItem newItem);

        Task<bool> MarkDoneAsync(Guid id);
    }
}
