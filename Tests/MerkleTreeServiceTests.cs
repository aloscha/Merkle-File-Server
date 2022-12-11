using MerkleFileServer.Helpers.MerkleTree;
using MerkleFileServer.Managers;
using MerkleFileServer.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MerkleFileServerTests
{
    public class MerkleTreeServiceTests
    {
        private Mock<IAppConfigService> appConfigServiceMock;
        private IMerkleTreeService merkleTreeService;
        private IFileService fileService;
        private ITreeManager treeManager;

        private const string filePath = "icons_rgb_circle.png";
        private const string hashFromExample = "9b39e1edb4858f7a3424d5a3d0c4579332640e58e101c29f99314a12329fc60b";
        private const int piecesFromExample = 17;
        private const int selectedPieceFromExample = 8;
        private string[] proofFromExample = new string[]
        {
                "6a10a0b8c1bd3651cba6e5604b31df595e965be137650d296c05afc1084cfe1f",
                "956bf86d100b2f49a8d057ebafa85b8db89a0f19d5627a1226fea1cb3e23d3f3",
                "04284ddea22b003e6098e7dd1a421a565380d11530a35f2e711a8dd2b9b5e7f8",
                "c66a821b749e0576e54b89dbac8f71211a508f7916e3d6235900372bed6c6c22",
                "a8bd48117723dee92524c25730f9e08e5d47e78c87d17edb344d4070389d049e"
        };

        [SetUp]
        public void Setup()
        {
            appConfigServiceMock = new Mock<IAppConfigService>();
            appConfigServiceMock.SetupGet(x => x.FileSize).Returns(1024);
            merkleTreeService = new MerkleTreeService(appConfigServiceMock.Object);
            fileService = new FileService(appConfigServiceMock.Object);
            FileManager.GetInstance.Paths = new[] { filePath };
            treeManager = new TreeManager(fileService, merkleTreeService);
        }

        [Test]
        public void NormalizeCorrectAmoun_Should_FillWith0()
        {
            var fileSize = appConfigServiceMock.Object.FileSize;

            var simpleChunks = new List<byte[]>
            {
                Enumerable.Repeat((byte)1, fileSize).ToArray(),
                Enumerable.Repeat((byte)1, fileSize).ToArray(),
                new byte[] { 1, 1, 1 },
            };

            var chunkNormalized = merkleTreeService.NormalizeData(new List<byte[]>(simpleChunks));

            Assert.That(chunkNormalized.Count, Is.EqualTo(4));
            Assert.That(chunkNormalized[0], Is.EquivalentTo(simpleChunks[0]));
            Assert.That(chunkNormalized[1], Is.EquivalentTo(simpleChunks[1]));
            Assert.That(chunkNormalized[2], Is.Not.EquivalentTo(simpleChunks[2]));

            Assert.That(chunkNormalized[2][0], Is.EqualTo(simpleChunks[2][0]));
            Assert.That(chunkNormalized[2][1], Is.EqualTo(simpleChunks[2][1]));
            Assert.That(chunkNormalized[2][2], Is.EqualTo(simpleChunks[2][2]));
            Assert.That(chunkNormalized[2][3], Is.EqualTo(0));
            Assert.That(chunkNormalized[3].Length, Is.EqualTo(0));
        }

        [Test]
        public void BuildMerkleTreeFromFile_Should_ReturnCorrectHashAndPiece()
        {
            var fileChunks = fileService.ReadChunks(filePath).ToList();
            Assert.That(fileChunks.Count, Is.EqualTo(piecesFromExample));

            var merkleTree = new MerkleTree();
            fileChunks = merkleTreeService.NormalizeData(fileChunks);
            merkleTree.Build(fileChunks);
            var calculatedHash = merkleTree.GetRootHash();
            Assert.That(hashFromExample, Is.EqualTo(calculatedHash));

            var calculatedProof = merkleTreeService.GetProof(merkleTree, selectedPieceFromExample);
            Assert.That(proofFromExample, Is.EquivalentTo(calculatedProof));
        }

        [Test]
        public void BuildMerkleTreeFromFileAndGetProof_Should_ValidateCorrectly()
        {
            var fileChunks = fileService.ReadChunks(filePath).ToList();
            var merkleTree = new MerkleTree();
            fileChunks = merkleTreeService.NormalizeData(fileChunks);
            merkleTree.Build(fileChunks);
            var calculatedHash = merkleTree.GetRootHash();

            var interestedNodes = new int[] { 1, 10, 16 };

            foreach(var node in interestedNodes)
            {
                var leaf = merkleTree.GetNodeByIndex(node);
                var calculatedProof = merkleTreeService.GetProof(merkleTree, node).ToArray();
                var content = ((MerkleLeaf)leaf).GetContentStr();

                //All pieces should always calculate same hash result (root hash)
                var validationResult = treeManager.Validate(calculatedHash, content, node, calculatedProof);

                Assert.That(validationResult, Is.True);
            }
        }
    }
}