using System.Collections.Generic;

namespace MerkleFileServer.Services
{
    public interface IFileService
    {
        public IEnumerable<byte[]> ReadChunks(string path);
    }
}
