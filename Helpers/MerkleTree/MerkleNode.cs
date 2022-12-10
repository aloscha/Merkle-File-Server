using System.Linq;

namespace MerkleFileServer.Helpers.MerkleTree
{
    public class MerkleNode : IMerkleNode
    {
        public IMerkleNode Left { get; private set; }
        public IMerkleNode Right { get; private set; }
        public IMerkleNode Parent { get; set; }
        public byte[] Hash { get; protected set; }

        protected MerkleNode()
        {
            Parent = null;
        }

        public MerkleNode(IMerkleNode left, IMerkleNode right) : base()
        {
            Left = left;
            Right = right;

            var newHash = left.Hash.Concat(right.Hash).ToArray();
            Hash = Encryptor.ToSha256(newHash);
        }

        public string GetHashStr()
        {
            return Encryptor.ByteArrayToString(Hash);
        }
    }
}
