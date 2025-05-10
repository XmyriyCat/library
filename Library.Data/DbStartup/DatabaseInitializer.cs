using System.Security.Claims;
using Library.Data.Exceptions;
using Library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Data.DbStartup
{
    public class DatabaseInitializer
    {
        public async Task InitializeAsync(IdentityDataContext dataContext, IConfiguration config,
            UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var initialData = new InitialDataStorage(dataContext, userManager, roleManager);

            await InitializeRolesAsync(initialData, dataContext, roleManager);
            await InitializeUsersAsync(initialData, dataContext, userManager);
            await InitializeAuthorsAsync(initialData, dataContext);
            await InitializeBooksAsync(initialData, dataContext);

            CopyBookImagesToRoot(config);
        }

        // Generic initialize method
        private static async Task InitializeGenericAsync<T>(IdentityDataContext dataContext, IEnumerable<T> values)
            where T : class
        {
            if (!await dataContext.Set<T>().AnyAsync())
            {
                await dataContext.Set<T>().AddRangeAsync(values);
            }
        }

        private static async Task InitializeRolesAsync(InitialDataStorage storage, IdentityDataContext dataContext
            , RoleManager<Role> roleManager)
        {
            var rolesExist = await dataContext.Set<Role>().AnyAsync(); 
            
            if (rolesExist)
            {
                return;
            }
            
            var roles = storage.Roles;

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            await dataContext.SaveChangesAsync();
        }

        private async Task InitializeUsersAsync(InitialDataStorage storage, IdentityDataContext dataContext,
            UserManager<User> userManager)
        {
            var usersExist = await dataContext.Set<User>().AnyAsync(); 
            
            if (usersExist)
            {
                return;
            }
            
            var users = storage.Users;

            var adminUser = storage.Users.FirstOrDefault(u => u.UserName == users[0].UserName)!;
            
            await userManager.CreateAsync(adminUser, "password");
            await userManager.AddToRoleAsync(adminUser, Variables.Roles.Admin);
            await userManager.AddClaimAsync(adminUser, new Claim(Variables.Claims.Role, Variables.Roles.Admin));
            await userManager.AddClaimAsync(adminUser, new Claim(Variables.Claims.Email, adminUser.Email!));

            var managerUser1 = storage.Users.FirstOrDefault(u => u.UserName == users[1].UserName)!;
            
            await userManager.CreateAsync(managerUser1, "password");
            await userManager.AddToRoleAsync(managerUser1, Variables.Roles.Manager);
            await userManager.AddClaimAsync(managerUser1, new Claim(Variables.Claims.Role, Variables.Roles.Manager));
            await userManager.AddClaimAsync(managerUser1, new Claim(Variables.Claims.Email, adminUser.Email!));
            
            var managerUser2 = storage.Users.FirstOrDefault(u => u.UserName == users[2].UserName)!;
            
            await userManager.CreateAsync(managerUser2, "password");
            await userManager.AddToRoleAsync(managerUser2, Variables.Roles.Manager);
            await userManager.AddClaimAsync(managerUser2, new Claim(Variables.Claims.Role, Variables.Roles.Manager));
            await userManager.AddClaimAsync(managerUser2, new Claim(Variables.Claims.Email, adminUser.Email!));
            
            await dataContext.SaveChangesAsync();
        }

        private async Task InitializeAuthorsAsync(InitialDataStorage storage, IdentityDataContext dataContext)
        {
            var authors = storage.Authors;

            await InitializeGenericAsync(dataContext, authors);

            await dataContext.SaveChangesAsync();
        }

        private async Task InitializeBooksAsync(InitialDataStorage storage, IdentityDataContext dataContext)
        {
            var authors = storage.Books;

            await InitializeGenericAsync(dataContext, authors);

            await dataContext.SaveChangesAsync();
        }
        
        private static void CopyBookImagesToRoot(IConfiguration config)
        {
            var apiDir = Directory.GetCurrentDirectory();
            var projectDir = Directory.GetParent(apiDir)!.FullName;
                
            var sourceDirectory = Path.Combine(projectDir, config["RootDirectories:BooksInitSeed"]!);
            var targetDirectory = Path.Combine(apiDir, config["RootDirectories:Books"]!);
            
            try
            {
                Directory.CreateDirectory(targetDirectory);
                
                foreach (var filePath in Directory.GetFiles(sourceDirectory))
                {
                    var fileName = Path.GetFileName(filePath);
                    var destPath = Path.Combine(targetDirectory, fileName);
                    
                    if (!File.Exists(destPath))
                    {
                        File.Copy(filePath, destPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CopyFailingException(ex.Message, ex);
            }
        }
    }
}