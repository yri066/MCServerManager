namespace MCServerManager.Library.Data.Model
{
    public class UsersListServer
    {
        public Guid Version { get; private set; } = Guid.NewGuid();
        public List<string> UserList { get; private set; } = new();

        public int Count { get { return UserList.Count; } }

        public void Add(string text)
        {
            UserList.Add(text);
            Version = Guid.NewGuid();
        }

        public void Remove(string text)
        {
            UserList.Remove(text);
            Version = Guid.NewGuid();
        }
    }
}
