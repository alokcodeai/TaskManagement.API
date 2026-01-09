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

        public async Task<IActionResult> Index(string? searchText, string? status)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TaskViewModel>>>("Tasks");
                var tasks = new List<TaskViewModel>();

                if (response != null && response.Success && response.Data != null)
                {
                    tasks = System.Text.Json.JsonSerializer.Deserialize<List<TaskViewModel>>(
                        System.Text.Json.JsonSerializer.Serialize(response.Data))!;
                }

                // 🔍 SEARCH BY TEXT
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    tasks = tasks.Where(x =>
                            (x.Title != null && x.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (x.Description != null && x.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (x.Status != null && x.Status.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                }

                // 🎯 FILTER BY STATUS
                if (!string.IsNullOrWhiteSpace(status))
                {
                    tasks = tasks
                        .Where(x => x.Status != null && x.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                ViewBag.SearchText = searchText;
                ViewBag.Status = status;

                return View(tasks);
            }
            catch
            {
                return View(new List<TaskViewModel>());
            }
        }


        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<TaskViewModel>>>("Tasks");

        //        if (response != null && response.Success && response.Data != null)
        //        {
        //            return View(response.Data);
        //        }

        //        return View(new List<TaskViewModel>());
        //    }
        //    catch
        //    {
        //        return View(new List<TaskViewModel>());
        //    }
        //}
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
