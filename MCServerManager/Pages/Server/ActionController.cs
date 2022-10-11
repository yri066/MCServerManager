using MCServerManager.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static MCServerManager.Library.Data.Model.ServerStatus;

namespace MCServerManager.Pages.Server
{
	[Route("/Server/{id:guid}/[action]")]
	[ApiController]
	public class ActionController : ControllerBase
	{
		private readonly ServerService _serverService;

		public ActionController(ServerService serverService)
		{
			_serverService = serverService;
		}

		public string GetStatus(Guid id)
		{
			try
			{
				return _serverService.GetServer(id).State.ToString();
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return ex.Message;
			}
		}

		public string Start(Guid id)
		{
			try
			{
				_serverService.StartServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return ex.Message;
			}

			return GetStatus(id);
		}

		public string Restart(Guid id)
		{
			try
			{
				_serverService.Restart(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return ex.Message;
			}

			return GetStatus(id);
		}

		public string Stop(Guid id)
		{
			try
			{
				_serverService.StopServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return ex.Message;
			}

			return GetStatus(id);
		}

		public string Close(Guid id)
		{
			try
			{
				_serverService.CloseServer(id);
			}
			catch (Exception ex)
			{
				HttpContext.Response.StatusCode = 404;
				return ex.Message;
			}

			return GetStatus(id);
		}
	}
}
