using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Services
{
    internal class SeedIdentityData : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedIdentityData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // BUG: This doesn't work
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            await SeedDefaultUsers.SeedAsync(userManager);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal static class SeedDefaultUsers
    {
        /*
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedDefaultUsers(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        */
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            const string password = "Pa$$w0rd.";
            var defaultAdmin = new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
            };
            var defaultUser = new ApplicationUser()
            {
                UserName = "test@gmail.com",
                Email = "test@gmail.com"
            };

            var isDefaultAdminExists = userManager.FindByEmailAsync(defaultAdmin.Email);
            var isDefaultUserExists = userManager.FindByEmailAsync(defaultUser.Email);

            if (isDefaultAdminExists is null)
            {
                await userManager.CreateAsync(defaultAdmin, password);
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
            }

            if (isDefaultUserExists is null)
            {
                await userManager.CreateAsync(defaultUser, password);
                await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
            }
        }
    }
}