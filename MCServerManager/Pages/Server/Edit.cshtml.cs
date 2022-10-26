using MCServerManager.Models;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
	public class EditModel : PageModel
	{
		[BindProperty]
		public GameServerDetail Input { get; set; }
		private readonly GameServerService _service;

		public EditModel(GameServerService serverService)
		{
			_service = serverService;
		}

		/// <summary>
		/// ������������ Get ������.
		/// </summary>
		/// <param name="id">������������� �������.</param>
		/// <returns>��������������� �� ��������.</returns>
		public IActionResult OnGet(Guid id)
		{
			try
			{
				var server = _service.GetServerData(id);
				Input = new GameServerDetail
				{
					Name = server.Name,
					AutoStart = server.AutoStart,
					WorkDirectory = server.WorkDirectory,
					Programm = server.Programm,
					Arguments = server.Arguments,
					Addres = server.Addres,
					Port = server.Port
				};
			}
			catch
			{
				return NotFound();
			}

			return Page();
		}

		/// <summary>
		/// ������������ Post ������ �� ��������� ���������� � �������.
		/// </summary>
		/// <param name="id">������������� �������.</param>
		/// <returns>��������������� �� ��������.</returns>
		public IActionResult OnPost(Guid id)
		{
			try
			{
				if (ModelState.IsValid)
				{
					_service.UpdateServer(id, Input.GetGameServerData(id));
					return RedirectToPage("Index", new { id });
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(
					string.Empty,
					ex.Message
				);
			}

			return Page();
		}
	}
}
