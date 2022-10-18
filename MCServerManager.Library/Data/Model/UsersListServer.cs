namespace MCServerManager.Library.Data.Model
{
	/// <summary>
	/// Список игроков
	/// </summary>
	public class UsersListServer
	{
		/// <summary>
		/// Версия списка пользователей
		/// </summary>
		public Guid Version { get; private set; } = Guid.NewGuid();

		/// <summary>
		/// Список пользователей
		/// </summary>
		public List<string> UserList { get; private set; } = new();

		/// <summary>
		/// Количество пользователей
		/// </summary>
		public int Count { get { return UserList.Count; } }

		/// <summary>
		/// Добавление пользователя в список
		/// </summary>
		/// <param name="text">Имя пользователя</param>
		public void Add(string text)
		{
			UserList.Add(text);
			Version = Guid.NewGuid();
		}

		/// <summary>
		/// Удаление пользователя из списка
		/// </summary>
		/// <param name="text">Имя пользователя</param>
		public void Remove(string text)
		{
			UserList.Remove(text);
			Version = Guid.NewGuid();
		}
	}
}
