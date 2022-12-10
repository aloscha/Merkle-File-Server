using System.Collections.Generic;
using System.Linq;

namespace MerkleFileServer.Helpers.MerkleTree
{
    public class MerkleTree : MerkleTreeBase
    {
        public override void Build(List<byte[]> contents)
        {
            nodes = contents
                .Select(e => new MerkleLeaf(e))
                .ToList<IMerkleNode>();

            if(nodes.Count == 1)
            {
                root = nodes[0];
            } 
            else
            {
                root = InsertNodes(nodes);
            }
        }
        private IMerkleNode InsertNodes(List<IMerkleNode> nodes)
        {
            var nextLayer = new List<IMerkleNode>();
            for (var i = 0; i < nodes.Count; i += 2)
            {
                var leftNode = nodes[i];
                var rightNode = nodes[i + 1];

                var newParent = new MerkleNode(leftNode, rightNode);
                leftNode.Parent = newParent;
                rightNode.Parent = newParent;

                nextLayer.Add(newParent);
            }

            if (nextLayer.Count == 1) return nextLayer[0];
            return InsertNodes(nextLayer);
        }
    }
}
