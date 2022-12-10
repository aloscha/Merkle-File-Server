using MerkleFileServer.Helpers;
using MerkleFileServer.Helpers.MerkleTree;
using MerkleFileServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MerkleFileServer.Managers
{
    public class TreeManager : ITreeManager
    {
        private readonly IFileService fileService;
        private readonly IMerkleTreeService merkleTreeService;
        private Dictionary<string, TreeManagerDetail> hashes { get; set; }

        public TreeManager(
            IFileService fileService,
            IMerkleTreeService merkleTreeService
            )
        {
            this.fileService = fileService;
            this.merkleTreeService = merkleTreeService;

            Build();
        }

        public Dictionary<string, int> GellAllHashes()
        {
            return hashes.ToDictionary(e => e.Key, f => f.Value.Pieces);
        }
        public TreeManagerItemDetail GetPiece(string hash, int index)
        {
            if (!hashes.ContainsKey(hash)) return null;
            if (hashes[hash].Pieces <= index) return null;

            return hashes[hash].Items[index];
        }
        public bool Validate(string hash, string content, int index, string[] proof)
        {
            var itemHash = Encryptor.ToSha256(Convert.FromBase64String(content));

            var merkleTree = new MerkleTree();
            var emptyChunks = Enumerable.Range(0, (int)Math.Pow(2, proof.Count())).Select(e => new byte[0]).ToList();
            merkleTree.Build(emptyChunks);

            var interestingItem = merkleTree.GetNodeByIndex(8);
            var calculatedNewHash = merkleTreeService.CalculateRootHash(merkleTree, itemHash, index, proof);

            return calculatedNewHash == hash;
        }
        private void Build()
        {
            hashes = new Dictionary<string, TreeManagerDetail>();
            var paths = FileManager.GetInstance.Paths;
            foreach (var path in paths)
            {
                BuildAndFillTree(path);
            }
        }
        private void BuildAndFillTree(string path)
        {
            var fileChunks = fileService.ReadChunks(path).ToList();
            var filePieces = fileChunks.Count;
            var merkleTree = new MerkleTree();
            fileChunks = merkleTreeService.NormalizeData(fileChunks);
            merkleTree.Build(fileChunks);

            var itemDetails = new TreeManagerItemDetail[filePieces];
            for(var i = 0; i < filePieces; ++i)
            {
                var proof = merkleTreeService.GetProof(merkleTree, i);
                var content = merkleTree.GetContent(i);
                itemDetails[i] = new TreeManagerItemDetail
                {
                    Content = content,
                    Proof = proof.ToArray(),
                };
            }

            var rootHash = merkleTree.GetRootHash();
            var managerDetail = new TreeManagerDetail
            {
                Pieces = filePieces,
                Items = itemDetails,
            };

            if (!hashes.ContainsKey(rootHash))
            {
                hashes.Add(rootHash, managerDetail);
            }
        }
    }
}