using System.Collections.Generic;

namespace MerkleFileServer.Managers
{
    public interface ITreeManager
    {
        public Dictionary<string, int> GellAllHashes();
        public TreeManagerItemDetail GetPiece(string hash, int index);
        public bool Validate(string hash, string content, int index, string[] proof);
    }
}