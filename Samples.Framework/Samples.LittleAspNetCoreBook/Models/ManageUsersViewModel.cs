using Microsoft.AspNetCore.Identity;

namespace Samples.LittleAspNetCoreBook.Models
{
    public class ManageUsersViewModel
    {
        public IdentityUser[] Administrators { get; internal set; }
        public IdentityUser[] Everyone { get; internal set; }
    }
}
