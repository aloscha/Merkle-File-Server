using System.Collections.Generic;

namespace MerkleFileServer.Helpers.MerkleTree
{
    public abstract class MerkleTreeBase
    {
        protected IMerkleNode root { get; set; }
        protected List<IMerkleNode> nodes { get; set; }
        public abstract void Build(List<byte[]> contents);

        public IMerkleNode GetNodeByIndex(int index)
        {
            return nodes.Count > index ? nodes[index] : null;
        }

        public string GetRootHash()
        {
            return root?.GetHashStr();
        }

        public string GetContent(int index)
        {
            return nodes.Count > index ? ((MerkleLeaf)nodes[index]).GetContentStr() : string.Empty;
        }
    }
}
