using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using TaskManagement.Web.Models;

namespace TaskManagement.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AdminController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("role") == "Admin";
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var client = _factory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                HttpContext.Session.GetString("token"));

            var response = await client.GetFromJsonAsync<ApiResponse<List<RegisterViewModel>>>("Auth/GetUser");

            var users = JsonSerializer.Deserialize<List<RegisterViewModel>>(
                JsonSerializer.Serialize(response.Data));

            return View(users);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeRole(int userId, string role)
        {
            if (!IsAdmin())
                return Unauthorized();

            var client = _factory.CreateClient("TaskApi");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                HttpContext.Session.GetString("token"));

            var response = await client.PutAsync($"Auth/ChangeRole?id={userId}&role={role}",null);
            var content = await response.Content.ReadAsStringAsync();

            return RedirectToAction("Dashboard");
        }
    }
}
