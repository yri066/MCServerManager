using MCServerManager.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MCServerManager.Pages.Server
{
    public class ListModel : PageModel
    {
        public readonly ServerService _service;

        public ListModel(ServerService service)
        {
            _service = service;
        }

        public void OnGet()
        {
        }
    }
}
