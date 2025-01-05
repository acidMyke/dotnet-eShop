using System;
using eShop.IdentityApi.Dtos;
using eShop.IdentityApi.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace eShop.IdentityApi;

public class AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) : IAuthService
{
  public Task RegisterAsync(RegisterRequest request)
  {
    throw new NotImplementedException();
  }

  public Task SignInAsync(LoginRequest request)
  {
    throw new NotImplementedException();
  }

  public Task SignOutAsync()
  {
    throw new NotImplementedException();
  }
}
