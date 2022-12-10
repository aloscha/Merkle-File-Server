using System;
using System.Linq;

namespace MerkleFileServer.Helpers.MerkleTree
{
    public class MerkleLeaf : MerkleNode, IMerkleNode
    {
        protected byte[] Content { get; private set; }

        public MerkleLeaf(byte[] content) : base()
        {
            Content = content;
            Hash = content.Any()
                ? Encryptor.ToSha256(content)
                : Enumerable.Repeat((byte)0, 32).ToArray();
        }

        public string GetContentStr()
        {
            return Convert.ToBase64String(Content);
        }
    }
}
