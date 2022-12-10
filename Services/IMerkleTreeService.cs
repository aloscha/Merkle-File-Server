using MerkleFileServer.Helpers.MerkleTree;
using System.Collections.Generic;

namespace MerkleFileServer.Services
{
    public interface IMerkleTreeService
    {
        public IEnumerable<string> GetProof(MerkleTree tree, int index);
        public List<byte[]> NormalizeData(List<byte[]> chunks);
        public string CalculateRootHash(MerkleTree tree, byte[] itemHash, int itemIndex, string[] proof);
    }
}
