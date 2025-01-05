
using eShop.IdentityApi.Dtos;

namespace eShop.IdentityApi.Interfaces;

public interface IAuthService
{
  public Task SignInAsync(LoginRequest request);
  public Task SignOutAsync();
  public Task RegisterAsync(RegisterRequest request);
}
