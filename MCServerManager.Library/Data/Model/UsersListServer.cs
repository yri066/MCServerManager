namespace MCServerManager.Library.Data.Model
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

	/// <summary>
	/// Список пользователей
	/// </summary>
	public class UsersListServer<T> : IUsersListServer<T>
	{
		/// <summary>
		/// Версия списка
		/// </summary>
		public Guid Version { get; private set; } = Guid.NewGuid();

		/// <summary>
		/// Список пользователей
		/// </summary>
		private List<T> _userList = new();

		/// <summary>
		/// Список пользователей
		/// </summary>
		public IEnumerable<T> UserList { get { return _userList; } }

		/// <summary>
		/// Количество пользователей
		/// </summary>
		public int Count { get { return _userList.Count; } }

		/// <summary>
		/// Добавление пользователя в список
		/// </summary>
		/// <param name="user">Имя пользователя</param>
		public void Add(T user)
		{
			_userList.Add(user);
			Version = Guid.NewGuid();
		}

		/// <summary>
		/// Удаление пользователя из списка
		/// </summary>
		/// <param name="user">Имя пользователя</param>
		public void Remove(T user)
		{
			_userList.Remove(user);
			Version = Guid.NewGuid();
		}

		/// <summary>
		/// Очищает список пользователей.
		/// </summary>
		public void Clear()
		{
			_userList.Clear();
			Version = Guid.NewGuid();
		}
	}
}
