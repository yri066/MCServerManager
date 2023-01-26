using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BackgroundService = MCServerManager.Library.Actions.BackgroundService;

namespace MCServerManager.Pages.Service
{
    [Authorize]
    public class ServiceModel : PageModel
    {
        /// <summary>
        /// Конфигурация.
        /// </summary>
        public readonly IConfiguration ButtonStyle;

        private readonly GameServerService _service;
        public BackgroundService Service { get; set; }
        private readonly UserService _userService;

        public ServiceModel(GameServerService service, IConfiguration configuration, UserService userService)
        {
            _service = service;
            _userService = userService;
            ButtonStyle = configuration.GetSection("Action:Application");
        }

        public IActionResult OnGet(Guid id)
        {

            try
            {
                var service = _service.GetService(id);
                var userId = _userService.UserId;

                if (userId != service.Data.UserId) return Forbid();

                Service = service;
            }
            catch
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDeleteAsync(Guid id)
        {
            try
            {
                var service = _service.GetService(id);
                var userId = _userService.UserId;

                if (userId != service.Data.UserId) return Forbid();

                await _service.DeleteServiceAsync(service.ServiceId!, id);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("/Server/List");
        }
    }
}
