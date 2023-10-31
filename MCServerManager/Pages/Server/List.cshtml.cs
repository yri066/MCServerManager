using MCServerManager.Library.Actions;
using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    public class ListModel : PageModel
    {
        /// <summary>
        /// Количество записей на странице по умолчанию.
        /// </summary>
        public const int DefaultPageSize = 10;
        
        /// <summary>
        /// Список серверов.
        /// </summary>
        public List<GameServer> Servers { get; private set; }

        /// <summary>
        /// Номер текущей страницы.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Количество страниц.
        /// </summary>
        public readonly int PageCount;

        public ListModel(GameServerService service)
        {
            Servers = service.Servers
                      .Where(list => list.State != GameServer.Status.Deleting)
                      .ToList();
            PageCount = (int)Math.Ceiling((decimal)Servers.Count / DefaultPageSize);
        }

        public IActionResult OnGet(int PageIndex = 1)
        {
            if (PageIndex <= 0)
            {
                return NotFound();
            }

            try
            {
                return SetPageList(PageIndex);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return SetPageList(PageIndex);
        }

        /// <summary>
        /// Разбивает список на страницы.
        /// </summary>
        /// <param name="PageIndex">Номер текущей страницы.</param>
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
