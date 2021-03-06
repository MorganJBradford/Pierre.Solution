using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Pierre.Models;
using Pierre.ViewModels;

namespace Pierre.Controllers
{
  public class AccountController : Controller
  {
    private readonly PierreContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController (UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, PierreContext db)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _db = db;
    }


    public async Task<ViewResult> Index(string sortOrder, string searchString)
    {
      if (User.Identity.IsAuthenticated)
      {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);

      var userTreats = _db.Treats.Where(entry => entry.User.Id == currentUser.Id).ToList();
      var userFlavors = _db.Flavors.Where(entry => entry.User.Id == currentUser.Id).ToList();

      ViewBag.MakeSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
      var sortedTreats = from t in userTreats.Where(entry => entry.User.Id == currentUser.Id).ToList()
        select t;
      var sortedFlavors = from f in userFlavors.Where(entry => entry.User.Id == currentUser.Id).ToList()
        select f;

      if (!String.IsNullOrEmpty(searchString))
      {
        sortedTreats = sortedTreats.Where(m => m.Name.Contains(searchString));
        sortedFlavors = sortedFlavors.Where(m => m.Name.Contains(searchString));
      }
        ViewBag.userTreats = sortedTreats.OrderBy(m => m.Name).ToList();
        ViewBag.userFlavors = sortedFlavors.OrderBy(m => m.Name).ToList();
      }
      return View();
    }

    public async Task<ActionResult> UserTreats()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userTreats = _db.Treats.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userTreats);
    }

    public async Task<ActionResult> UserFlavors()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userFlavors = _db.Flavors.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userFlavors);
    }

    public ActionResult Register()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register (RegisterViewModel model)
    {
      var user = new ApplicationUser { UserName = model.Email };
      IdentityResult result = await _userManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        return View();
      }
    }

    public ActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(LoginViewModel model)
    {
      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        return RedirectToAction("Index");
      }
      else
      {
        return View();
      }
    }

    [HttpPost]
    public async Task<ActionResult> LogOff()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index");
    }
  }
}