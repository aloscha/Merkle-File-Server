namespace MerkleFileServer.Managers
{
    public sealed class FileManager
    {
        public string[] Paths { get; set; }
        private static readonly object Instancelock = new object();
        private FileManager()
        {
        }

        private static FileManager instance = null;

        public static FileManager GetInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (Instancelock)
                    {
                        instance ??= new FileManager();
                    }
                }
                return instance;
            }
        }
    }
}
