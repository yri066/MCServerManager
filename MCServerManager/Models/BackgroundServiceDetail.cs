using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCService = MCServerManager.Library.Data.Models.Service;
using MCServerManager.Library.Data.Models;

namespace MCServerManager.Models
{
    /// <summary>
    /// Данные о сервисе
    /// </summary>
    public class BackgroundServiceDetail : ApplicationDetail
    {
        /// <summary>
        /// Адрес сервера(ip)
        /// </summary>
        [StringLength(100), DisplayName("Адрес сервера:")]
        public string? Address { get; set; }

        /// <summary>
		/// Задержка до полного запуска
		/// </summary>
		[Required, Range(0, 7200), DisplayName("Задержка до полного запуска:")]
        public int Delay { get; set; } = 10;

        /// <summary>
        /// Используемый порт
        /// </summary>
        [DisplayName("Используемый порт:")]
        public int? Port { get; set; }

        /// <summary>
        /// Автовыключение вместе с сервером.
        /// </summary>
        [Required, DisplayName("Автовыключение вместе с сервером:")]
        public bool AutoClose { get; set; }

        public MCService GetBackgroundServiceData()
        {
            return new MCService
            {
                Name = Name,
                AutoStart = AutoStart,
                AutoClose = AutoClose,
                Delay = Delay,
                WorkDirectory = WorkDirectory,
                StartProgram = StartProgram,
                Arguments = Arguments,
                Address = Address,
                Port = Port
            };
        }

        public MCService GetBackgroundServiceData(MCService service)
        {
            var exemplar = GetBackgroundServiceData();
            exemplar.ServerId = service.ServerId;
            exemplar.ServiceId = service.ServiceId;
            exemplar.RatingNumber = service.RatingNumber;

            return service;
        }

        public MCService GetBackgroundServiceData(Guid serverId, string UserId)
        {
            var service = GetBackgroundServiceData();
            service.ServerId = serverId;
            service.UserId = UserId;

            return service;
        }

        public void UpdateData(MCService service)
        {
            Name = service.Name;
            AutoStart = service.AutoStart;
            AutoClose = service.AutoClose;
            Delay = service.Delay;
            WorkDirectory = service.WorkDirectory;
            StartProgram = service.StartProgram;
            Arguments = service.Arguments;
            Address = service.Address;
            Port = service.Port;
        }
    }
}
