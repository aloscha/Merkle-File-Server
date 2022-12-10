namespace MerkleFileServer.Helpers.MerkleTree
{
    public interface IMerkleNode
    {
        public IMerkleNode Left { get; }
        public IMerkleNode Right { get; }
        public IMerkleNode Parent { get; set; }
        public byte[] Hash { get; }
        public string GetHashStr();
    }
}
