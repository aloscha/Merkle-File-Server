using MerkleFileServer.Helpers;
using MerkleFileServer.Helpers.MerkleTree;
using MerkleFileServer.Managers;
using MerkleFileServer.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace MerkleFileServerTests
{
    public class MerkleTreeTests
    {
        private MerkleTree merkleTree { get; set; }

        //We are not testing here hash correct, it's different test
        private readonly List<byte[]> SimpleChunks = new List<byte[]>
        {
            new byte[] { 1, 1, 1 },
            new byte[] { 1, 1, 1 },
            new byte[] { 1, 1, 1 },
            new byte[] { 1, 1, 1 },
        };

        [SetUp]
        public void Setup()
        {
            merkleTree = new MerkleTree();
            merkleTree.Build(SimpleChunks);
        }

        [Test]
        public void MerkleTreeDepth_Should_Be3()
        {
            var firstElement = merkleTree.GetNodeByIndex(0);
            for(var i = 0; i < 2; ++i)
            {
                if (firstElement.Parent == null)
                {
                    Assert.Fail();
                }
                firstElement = firstElement.Parent;
            }

            //Root checking
            Assert.That(firstElement.Parent, Is.EqualTo(null));
            Assert.Pass();
        }

        [Test]
        public void MerkleTreeContent_Should_BeSameForAllLeafs()
        {
            var contentCompare = Convert.ToBase64String(SimpleChunks[0]);
            for (var i = 0; i < 4; ++i)
            {
                var node = merkleTree.GetNodeByIndex(i);
                Assert.That(((MerkleLeaf)node).GetContentStr(), Is.EqualTo(contentCompare));
            }
        }

        [Test]
        public void MerkleTreeRootHash_Should_BeSameAsManualCalculated()
        {
            var lP = Encryptor.ToSha256(SimpleChunks[0]).Concat(Encryptor.ToSha256(SimpleChunks[1])).ToArray();
            var lPHash = Encryptor.ToSha256(lP);

            var RP = Encryptor.ToSha256(SimpleChunks[2]).Concat(Encryptor.ToSha256(SimpleChunks[3])).ToArray();
            var rPHash = Encryptor.ToSha256(RP);

            var rH = lPHash.Concat(rPHash).ToArray();
            var rHash = Encryptor.ToSha256(rH);
            var calculatedRH = Encryptor.ByteArrayToString(rHash);

            var rootHash = merkleTree.GetRootHash();

            Assert.That(calculatedRH, Is.EqualTo(rootHash));
        }
    }
}