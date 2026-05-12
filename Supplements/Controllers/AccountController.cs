using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Supplements.Core.Entities;
using Supplements.Services;
using Supplements.ViewModels.Account;

namespace Supplements.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(AuthService authService, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _authService = authService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.Login(new Core.DTOs.LoginRequest
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message ?? "Invalid login attempt");
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        await _signInManager.SignInAsync(user!, model.RememberMe);

        TempData["Success"] = "Welcome back!";
        return RedirectToLocal(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.Register(new Core.DTOs.RegisterRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            PhoneNumber = model.PhoneNumber
        });

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Message ?? "Registration failed");
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        await _signInManager.SignInAsync(user!, isPersistent: false);

        TempData["Success"] = "Account created successfully!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Success"] = "You have been signed out.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }
}
