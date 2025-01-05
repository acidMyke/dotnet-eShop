
using eShop.ServiceDefaults.Extensions;
using Microsoft.AspNetCore.Identity;

namespace eShop.IdentityApi.Data;

public class DataSeed(ILogger<DataSeed> logger, UserManager<IdentityUser> userManager) : IDbSeeder<ApplicationDbContext>
{
  private readonly IdentityUser _systemUser = new()
  {
    Id = Guid.NewGuid().ToString(),
    UserName = "SYSTEM",
    Email = "system@acidmyke.link",
    EmailConfirmed = true
  };

  private readonly IdentityUser _userAlice = new()
  {
    Id = Guid.NewGuid().ToString(),
    UserName = "Alice",
    Email = "alice@email.com",
    EmailConfirmed = true
  };

  public async Task SeedAsync(ApplicationDbContext context)
  {
    IdentityUser[] users = [_systemUser, _userAlice];

    foreach (var user in users)
    {
      var existingUser = await userManager.FindByNameAsync(user.UserName!);
      if (existingUser != null) continue;
      try
      {
        var createResult = await userManager.CreateAsync(user, "Pass123$");
        if (createResult.Succeeded)
        {
          logger.LogDebug("User {UserName} created", user.UserName);
          continue;
        }
        else
        {
          throw new Exception(createResult.Errors.First().Description);
        }
      }
      catch (Exception)
      {
        logger.LogDebug("Unable to create user {UserName}", user.UserName);
        throw;
      }
    }
  }
}