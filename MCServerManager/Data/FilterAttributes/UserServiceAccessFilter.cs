using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace MCServerManager.Data.FilterAttributes
{
    /// <summary>
    /// Проверка доступа пользователя к сервису.
    /// </summary>
    public class UserServiceAccessFilter : Attribute, IActionFilter
    {
        private readonly GameServerService _serverService;

        public string Key = "serviceId";
        public string ErrorMessage = "Доступ запрещен.";

        public UserServiceAccessFilter(GameServerService serverService)
        {
            _serverService = serverService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                context.ActionArguments.TryGetValue(Key, out object? value);
                var userId = context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var id = _serverService.GetServiceData((Guid)value!).UserId;

                if (id != userId)
                {
                    context.Result = new JsonResult(value: new { errorText = ErrorMessage }) { StatusCode = 403 };
                }
            }
            catch (Exception)
            {
                context.Result = new JsonResult(value: new { errorText = ErrorMessage }) { StatusCode = 403 };
            }

            return;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
