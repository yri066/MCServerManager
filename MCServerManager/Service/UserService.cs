using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MCServerManager.Service
{
    /// <summary>
    /// Информация о зарегистрированном пользователе.
    /// </summary>
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string? UserId
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? UserName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Identity?.Name;
            }
        }
    }
}
