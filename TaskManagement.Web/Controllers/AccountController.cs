using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using TaskManagement.Web.Models;

namespace TaskManagement.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("TaskApi");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", model);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterViewModel>>();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Invalid username or password";
                    return View();
                }
                else
                {
                    if (result.StatusCode == 200)
                    {

                        HttpContext.Session.SetString("token", result.Data.Tokken);
                        HttpContext.Session.SetString("username", result.Data.UserName);
                        HttpContext.Session.SetString("role", result.Data.Role);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = result.Message;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
           return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", model);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterViewModel>>();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Registration failed";
                    return View();
                }
                else
                {
                    if (result.StatusCode == 200)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.Error = result.Message;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
