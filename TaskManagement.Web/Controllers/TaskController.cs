using Microsoft.AspNetCore.Mvc;
using TaskManagement.Web.Models;

namespace TaskManagement.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly HttpClient _httpClient;

        public TaskController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("TaskApi");
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TaskViewModel>>>("Tasks");

                if (response != null && response.Success && response.Data != null)
                {
                    return View(response.Data);
                }

                return View(new List<TaskViewModel>());
            }
            catch
            {
                return View(new List<TaskViewModel>());
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ApiResponse<TaskViewModel>>($"Tasks/{id}");

            if (response == null || !response.Success)
                return NotFound();

            return View(response.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _httpClient.PostAsJsonAsync("Tasks", model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _httpClient.PutAsJsonAsync($"Tasks/{id}", model);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _httpClient.DeleteAsync($"Tasks/{id}");
            return RedirectToAction("Index");
        }
    }
}
