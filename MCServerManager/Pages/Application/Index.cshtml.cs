using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BackgroundService = MCServerManager.Library.Actions.BackgroundService;

namespace MCServerManager.Pages.Application
{
	public class IndexModel : PageModel
	{
		/// <summary>
		/// Конфигурация.
		/// </summary>
		public readonly IConfiguration ButtonStyle;

		private readonly GameServerService _service;
		public BackgroundService Service { get; set; }

		public IndexModel(GameServerService service, IConfiguration configuration)
		{
			_service = service;
			ButtonStyle = configuration.GetSection("Action:Application");
		}
		public void OnGet(Guid serviceId)
		{
			Service = _service.GetService(serviceId);
		}

		public IActionResult OnGetDelete(Guid serviceId)
		{
			try
			{
				var serverId = _service.GetServiceData(serviceId).GameServerId;
				_service.DeleteService(serverId, serviceId);
			}
			catch
			{}

			return RedirectToPage("/Server/List");
		}
	}
}
