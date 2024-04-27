using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace MCServerManager.Pages.Service
{
    [Route("/Service/{serviceId:guid}/[action]")]
    public class ServiceController : Controller
    {
        private readonly GameServerService _serverService;

        public ServiceController(GameServerService serverService)
        {
            _serverService = serverService;
        }

        /// <summary>
        /// Получить информацию о сервисе.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        public object GetStatus(Guid serviceId)
        {
            try
            {
                var service = _serverService.GetService(serviceId);

                return new
                {
                    Status = service.State.ToString()
                };
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }
        }

        /// <summary>
        /// Запустить сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        public object Start(Guid serviceId)
        {
            try
            {
                _serverService.StartService(serviceId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serviceId);
        }

        /// <summary>
        /// Выключить сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <returns>Информация о сервисе.</returns>
        public object Close(Guid serviceId)
        {
            try
            {
                _serverService.CloseService(serviceId);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serviceId);
        }

        /// <summary>
        /// Открыть страницу консоли приложения.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервера.</param>
        /// <returns>Страница консоли.</returns>
        public IActionResult Console(Guid serviceId)
        {
            try
            {
                var service = _serverService.GetService(serviceId);

                ViewData["Name"] = service.Name;
                return View("/Pages/Application/Console.cshtml", service.ConsoleBuffer);
            }
            catch (Exception)
            {
                return Redirect("/List");
            }
        }

        /// <summary>
        /// Получить буфер вывода приложения.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <param name="bufferId">Версия буфера.</param>
        /// <returns>Буфер вывода приложения.</returns>
        [Route("/Service/{serviceId:guid}/[action]/{bufferId:guid}")]
        public object Console(Guid serviceId, Guid bufferId)
        {
            try
            {
                var service = _serverService.GetService(serviceId);

                return new
                {
                    Console = service.ConsoleBuffer.GetConsoleBuffer(bufferId),
                    service.ConsoleBuffer.Version
                };
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }
        }

        /// <summary>
        /// Отправить сообщение в сервис.
        /// </summary>
        /// <param name="serviceId">Идентификатор сервиса.</param>
        /// <param name="message">Сообщение.</param>
        /// <returns>Информация о сервисе.</returns>
        [HttpPost]
        public object Console(Guid serviceId, string message = "")
        {
            try
            {
                _serverService.SendServiceAppMessage(serviceId, message);
            }
            catch (Exception ex)
            {
                return SetErrorMessage(404, ex.Message);
            }

            return GetStatus(serviceId);
        }

        private object SetErrorMessage(int statusCode, string message)
        {
            HttpContext.Response.StatusCode = statusCode;
            return new { errorText = message };
        }
    }
}
