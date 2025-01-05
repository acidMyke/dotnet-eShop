using System;
using eShop.IdentityApi;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace IdentityApi.UnitTest;

public class AuthServiceUnitTest
{
  private const string TestEmail = "test@example.com";
  private const string TestPassword = "Test1234";
  private const string TestUserName = "Tester";

  [Fact]
  public async Task RegisterAsync_ShouldCall_UserManager_CreateAsync()
  {
    var mockUserManager = new Mock<UserManager<IdentityUser>>();
    var mockSignInManager = new Mock<SignInManager<IdentityUser>>();

    mockUserManager
      .Setup(o => o.CreateAsync(
        It.Is<IdentityUser>(u => u.Email == TestEmail && u.UserName == TestUserName),
        It.Is<string>(p => p == TestPassword)
      ))
      .ThrowsAsync(new FunctionCalledException());

    var authService = new AuthService(mockSignInManager.Object, mockUserManager.Object);

    await Assert.ThrowsAsync<FunctionCalledException>(() => authService.RegisterAsync(new()
    {
      Email = TestEmail,
      Password = TestPassword,
      UserName = TestUserName
    }));
  }

  [Fact]
  public async Task RegisterAsync_ShouldCall_SignInManager_SignInAsync()
  {
    var mockUserManager = new Mock<UserManager<IdentityUser>>();
    var mockSignInManager = new Mock<SignInManager<IdentityUser>>();

    mockUserManager
      .Setup(o => o.CreateAsync(
        It.Is<IdentityUser>(u => u.Email == TestEmail && u.UserName == TestUserName),
        It.Is<string>(p => p == TestPassword)
      ))
      // Can't create identity result, so we also mock it :)
      .ReturnsAsync(Mock.Of<IdentityResult>(r => r.Succeeded == true));

    mockSignInManager
      .Setup(o => o.SignInAsync(
        It.Is<IdentityUser>(u => u.Email == TestEmail && u.UserName == TestUserName),
        It.IsAny<bool>(),
        null
      ))
      .Throws(new FunctionCalledException());

    var authService = new AuthService(mockSignInManager.Object, mockUserManager.Object);

    await Assert.ThrowsAsync<FunctionCalledException>(() => authService.RegisterAsync(new()
    {
      Email = TestEmail,
      Password = TestPassword,
      UserName = TestUserName
    }));
  }

  [Fact]
  public async Task SignInAsync_ShouldCall_SignInManager_PasswordSignInAsync()
  {
    var mockUserManager = new Mock<UserManager<IdentityUser>>();
    var mockSignInManager = new Mock<SignInManager<IdentityUser>>();

    mockSignInManager
      .Setup(o => o.PasswordSignInAsync(
        It.Is<string>(u => u == TestEmail || u == TestUserName),
        It.Is<string>(p => p == TestPassword),
        It.IsAny<bool>(),
        It.IsAny<bool>()
      ))
      .Throws(new FunctionCalledException());

    var authService = new AuthService(mockSignInManager.Object, mockUserManager.Object);

    await Assert.ThrowsAsync<FunctionCalledException>(() => authService.SignInAsync(new()
    {
      UserNameOrEmail = TestEmail,
      Password = TestPassword
    }));
  }

  [Fact]
  public async Task SignOutAsync_ShouldCall_SignInManager_SignOutAsync()
  {
    var mockUserManager = new Mock<UserManager<IdentityUser>>();
    var mockSignInManager = new Mock<SignInManager<IdentityUser>>();

    mockSignInManager
      .Setup(o => o.SignOutAsync())
      .Throws(new FunctionCalledException());

    var authService = new AuthService(mockSignInManager.Object, mockUserManager.Object);

    await Assert.ThrowsAsync<FunctionCalledException>(() => authService.SignOutAsync());
  }

}

public class FunctionCalledException : Exception
{
  public FunctionCalledException() { }
}
