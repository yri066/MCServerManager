using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BackgroundService = MCServerManager.Library.Actions.BackgroundService;

namespace MCServerManager.Pages.Service
{
	public class ServiceModel : PageModel
	{
		/// <summary>
		/// Конфигурация.
		/// </summary>
		public readonly IConfiguration ButtonStyle;

		private readonly GameServerService _service;
		public BackgroundService Service { get; set; }

		public ServiceModel(GameServerService service, IConfiguration configuration)
		{
			_service = service;
			ButtonStyle = configuration.GetSection("Action:Application");
		}
		public void OnGet(Guid id)
		{
			Service = _service.GetService(id);
		}

		public async Task<IActionResult> OnGetDeleteAsync(Guid id)
		{
			try
			{
				var serverId = _service.GetServiceData(id).ServerId;
				await _service.DeleteServiceAsync((Guid)serverId!, id);
			}
			catch
			{}

			return RedirectToPage("/Server/List");
		}
	}
}
