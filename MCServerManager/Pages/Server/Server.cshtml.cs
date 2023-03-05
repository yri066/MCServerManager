using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	[Authorize]
	public class ServerModel : PageModel
	{
		/// <summary>
		/// Конфигурация.
		/// </summary>
		public readonly IConfiguration ButtonStyle;

		private readonly GameServerService _serverService;
		public GameServer Exemplar { get; private set; }
        private readonly UserService _userService;

        public ServerModel(GameServerService serverService, IConfiguration configuration, UserService userService)
		{
			_serverService = serverService;
            _userService = userService;
            ButtonStyle = configuration.GetSection("Action:GameServer");
        }

		public IActionResult OnGet(Guid serverId)
		{
			try
			{
				var server = _serverService.GetServer(serverId);
                var userId = _userService.UserId;

                if (userId != server.Data.UserId) return Forbid();

                Exemplar = server;
			}
			catch
			{
				return NotFound();
			}

			return Page();
		}

		public async Task<IActionResult> OnGetDeleteAsync(Guid serverId)
		{
			try
			{
                var server = _serverService.GetServer(serverId);
                var userId = _userService.UserId;

                if (userId != server.Data.UserId) return Forbid();

                await _serverService.DeleteServerAsync(serverId);
			}
			catch
			{
                return NotFound();
            }
			
			return RedirectToPage("List");
		}
	}
}
