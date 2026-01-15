using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.API.Models;
using TaskManagement.API.Models.Dto;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TaskDbContext _context;
        private readonly IConfiguration configuration;

        public AuthController(TaskDbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ApiResponse> GetUser()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var users = await _context.Users.ToListAsync();
                if (users != null && users.Count > 0)
                {
                    response.StatusCode = 200;
                    response.Success = true;
                    response.Message = "Users retrieved successfully.";
                    response.Data = users;
                }
                else
                {
                    response.StatusCode = 200;
                    response.Success = false;
                    response.Message = "No users found.";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                response.Data = null;
                return response;
            }

            return response;
        }

        [HttpPost("register")]
        public async Task<ApiResponse> Register(RegisterDto dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (_context.Users.Any(x => x.UserName == dto.UserName))
                {
                    response.StatusCode = 400;
                    response.Success = false;
                    response.Message = "Username already exists.";
                    return response;
                }
                else
                {
                    var user = new User
                    {
                        UserName = dto.UserName,
                        Email = dto.Email,
                        Role = dto.Role,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                        CreatedOn = DateTime.Now
                    };
                    await _context.Users.AddAsync(user);
                    int a = _context.SaveChanges();

                    if (a > 0)
                    {
                        response.StatusCode = 200;
                        response.Success = true;
                        response.Message = "User registered successfully.";
                        response.Data = user;   
                    }
                    else
                    {
                        response.StatusCode = 500;
                        response.Success = false;
                        response.Message = "Error registering user.";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                return response;
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("ChangeRole")]
        public async Task<ApiResponse> ChangeRole(int id,string role)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user != null)
                {
                    
                    user.Role = role;
                    _context.Users.Update(user);
                   int a = _context.SaveChanges();
                    if(a > 0)
                    {
                        response.StatusCode = 200;
                        response.Success = true;
                        response.Message = "Role updated successfully.";
                        response.Data = user;
                    }
                    else
                    {       
                        response.StatusCode = 400;
                        response.Success = false;
                        response.Message = "Role update failed.";
                        response.Data = user;
                    }
                    return response;
                }
                else
                {
                    response.StatusCode = 400;
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                return response;
            }
        }


        [HttpPost("login")]
        public async Task<ApiResponse> Login(LogInDto dto)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    response.StatusCode = 400;
                    response.Success = false;
                    response.Message = "Invalid username or password.";
                    return response;
                }
                else
                {
                    var token = GenerateJwtToken(user);
                    RegisterDto logInDto = new RegisterDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = user.Role,
                        Tokken = token,
                        CreatedOn = user.CreatedOn
                    };

                    response.Success = true;
                    response.StatusCode = 200;
                    response.Message = "Login successful.";
                    response.Data = logInDto;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Success = false;
                response.Message = "Exception " + ex.Message;
                return response;
            }
           
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
             new Claim(ClaimTypes.Name, user.UserName),
              new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }





    }
}
