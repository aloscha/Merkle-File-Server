using MerkleFileServer.Helpers;
using MerkleFileServer.Helpers.MerkleTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MerkleFileServer.Services
{
    public class MerkleTreeService : IMerkleTreeService
    {
        private readonly IAppConfigService appConfigService;
        public MerkleTreeService(IAppConfigService appConfigService)
        {
            this.appConfigService = appConfigService;
        }

        public IEnumerable<string> GetProof(MerkleTree tree, int index)
        {
            var proofResult = new List<string>();
            var merkleLeaf = tree.GetNodeByIndex(index);
            var sibling = GetSibling(merkleLeaf);

            if (sibling != null)
            {
                proofResult.Add(sibling.GetHashStr());
            }

            while (merkleLeaf.Parent?.Parent != null)
            {
                merkleLeaf = GetUncle(merkleLeaf);
                proofResult.Add(merkleLeaf.GetHashStr());
            }

            return proofResult;
        }
        public string CalculateRootHash(MerkleTree tree, byte[] itemHash, int itemIndex, string[] proof)
        {
            var rootHash = itemHash;
            var baseItem = tree.GetNodeByIndex(itemIndex);
            var siblingProof = Encryptor.StringToByteArray(proof[0]);

            var newHash = (itemIndex % 2 == 0 ? rootHash.Concat(siblingProof) : siblingProof.Concat(rootHash)).ToArray();
            rootHash = Encryptor.ToSha256(newHash);

            for(var i = 1; i < proof.Length; ++i)
            {
                var uncleProof = Encryptor.StringToByteArray(proof[i]);
                var isUncleLeft = IsUncleLeft(baseItem);

                newHash = (isUncleLeft ? uncleProof.Concat(rootHash) : rootHash.Concat(uncleProof)).ToArray();
                rootHash = Encryptor.ToSha256(newHash);
                baseItem = baseItem.Parent;
            }

            return Encryptor.ByteArrayToString(rootHash);
        }

        public List<byte[]> NormalizeData(List<byte[]> chunks)
        {
            var basePow = Math.Ceiling(Math.Log(chunks.Count, 2));
            var allItemsCount = (int)Math.Pow(2, basePow);
            var missingItemsCount = allItemsCount - chunks.Count;
            var lastChunk = chunks[^1];
            if(lastChunk.Length != appConfigService.FileSize)
            {
                chunks[^1] = lastChunk.Concat(Enumerable.Repeat((byte)0, appConfigService.FileSize - lastChunk.Length).ToArray()).ToArray();
            }

            var missingItems = Enumerable.Range(0, missingItemsCount).Select(e => new byte[0]);
            return chunks.Concat(missingItems).ToList();
        }

        private IMerkleNode GetSibling(IMerkleNode node)
        {
            if(node.Parent == null) return null;

            var parent = node.Parent;
            return parent.Left.Hash.SequenceEqual(node.Hash) 
                ? parent.Right
                : parent.Left;
        }
        private IMerkleNode GetUncle(IMerkleNode node)
        {
            var parent = node.Parent;
            var uncle = GetSibling(parent);
            return uncle;
        }

        private bool IsUncleLeft(IMerkleNode node)
        {
            var parent = node.Parent;
            var grandParent = parent.Parent;

            return grandParent.Left != parent;
        }
    }
}
