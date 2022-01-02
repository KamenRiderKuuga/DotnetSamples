using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Samples.LittleAspNetCoreBook.Models;

namespace Samples.LittleAspNetCoreBook.Services
{
    public interface ITodoItemService
    {
        Task<TodoItem[]> GetIncompleteItemsAsync(IdentityUser user);

        Task<bool> AddItemAsync(TodoItem newItem, IdentityUser user);

        Task<bool> MarkDoneAsync(Guid id, IdentityUser user);
    }
}
