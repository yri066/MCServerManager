namespace MCServerManager.Library.Data.Interface
{
    /// <summary>
	/// Список пользователей
	/// </summary>
	public interface IUsersListServer<T>
    {
        /// <summary>
        /// Версия списка
        /// </summary>
        public Guid Version { get; }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public IEnumerable<T> UserList { get; }

        /// <summary>
        /// Количество пользователей
        /// </summary>
        public int Count { get; }
    }
}
