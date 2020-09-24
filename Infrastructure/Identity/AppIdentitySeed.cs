using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Bob",
                    Email = "bob@test.com",
                    UserName = "bob@test.com",
                    Address = new Address
                    {
                        FirstName = "Bob",
                        LastName = "Bobs",
                        Street = "10 The Street",
                        City = "New Town",
                        State = "The State",
                        ZipCode = "90210"
                    },
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
