using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    public class ListModel : PageModel
    {
        /// <summary>
        /// ���������� ������� �� �������� �� ���������.
        /// </summary>
        public const int DefaultPageSize = 10;
        
        /// <summary>
        /// ������ ��������.
        /// </summary>
        public List<GameServer> Servers { get; private set; }

        /// <summary>
        /// ������������� ������������.
        /// </summary>
        public readonly string? UserId;

        /// <summary>
        /// ����� ������� ��������.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// ���������� �������.
        /// </summary>
        public readonly int PageCount;

        public ListModel(GameServerService service, UserService userService)
        {
            Servers = service.Servers
                      .Where(list => list.State != GameServer.Status.Deleting)
                      .ToList();
            UserId = userService.UserId;
            PageCount = (int)Math.Ceiling((decimal)Servers.Count / DefaultPageSize);
        }

        public IActionResult OnGet(int PageIndex = 1)
        {
            if (PageIndex <= 0)
            {
                return NotFound();
            }

            return SetPageList(PageIndex);
        }

        /// <summary>
        /// ��������� ������ �� ��������.
        /// </summary>
        /// <param name="PageIndex">����� ������� ��������.</param>
        private IActionResult SetPageList(int PageIndex)
        {
            this.PageIndex = PageIndex--;

            if (PageIndex > PageCount)
            {
                return NotFound();
            }

            if (PageIndex != 0)
            {
                Servers = Servers.Skip(PageIndex * DefaultPageSize).ToList();
            }

            Servers = Servers.Take(DefaultPageSize).ToList();
            return Page();
        }

    }
}
