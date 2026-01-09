using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TasksController(TaskDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<ApiResponse> GetAllTask()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var tasks = await _context.Tasks.ToListAsync();
                if (tasks != null && tasks.Count > 0)
                {
                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Tasks retrieved successfully.";
                    response.Data = tasks;
                }
                else
                {
                    response.StatusCode = 200;
                    response.Success = false;
                    response.Message = "No tasks found.";
                    response.Data = null;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetTaskById(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var tasks = await _context.Tasks.FindAsync(id);
                if (tasks is not null)
                {
                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Tasks retrieved successfully.";
                    response.Data = tasks;
                }
                else
                {
                    response.StatusCode = 200;
                    response.Success = false;
                    response.Message = "No tasks found.";
                    response.Data = null;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }

        }

        [HttpPost]
        public async Task<ApiResponse> CreateTask(Models.Task task)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                _context.Tasks.Add(task);
                int a = _context.SaveChanges();
                if (a > 0)
                {
                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Task created successfully.";
                    response.Data = task;
                }
                else
                {
                    response.StatusCode = 400;
                    response.Success = false;
                    response.Message = "Task creation failed.";
                    response.Data = null;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateTask(int id, Models.Task task)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var existing = _context.Tasks.Find(id);
                if (existing == null)
                {
                    response.StatusCode = 200;
                    response.Success = false;
                    response.Message = "Task not found.";
                    response.Data = null;
                    return response;
                }
                else
                {
                    existing.Title = task.Title;
                    existing.Description = task.Description;
                    existing.Status = task.Status;
                    existing.DueDate = task.DueDate;
                    existing.Remarks = task.Remarks;
                    existing.UpdatedOn = DateTime.Now;
                    existing.UpdatedBy = task.UpdatedBy;
                    existing.CreatedBy = task.CreatedBy;

                   int a = _context.SaveChanges();
                    if (a > 0)
                    {
                        response.StatusCode = 200;
                        response.Success = true;
                        response.Message = "Task updated successfully.";
                        response.Data = existing;
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.Success = false;
                        response.Message = "Task update failed.";
                        response.Data = null;
                    }

                }
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteTask(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var task = _context.Tasks.Find(id);
                if (task == null)
                {
                    response.StatusCode = 200;
                    response.Success = false;
                    response.Message = "Task not found.";
                    response.Data = null;

                    return response;
                }
                else
                {
                    _context.Tasks.Remove(task);
                    int a = _context.SaveChanges();
                    if (a > 0)
                    {
                        response.StatusCode = 200;
                        response.Success = true;
                        response.Message = "Task deleted successfully.";
                        response.Data = null;
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.Success = false;
                        response.Message = "Task deletion failed.";
                        response.Data = null;
                    }
                 }
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }
        }
    }
}
