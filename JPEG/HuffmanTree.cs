using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    class HuffmanTree
    {
        private List<Node> nodes = new List<Node>();
        public Node root { get; set; }
        public KeyValuePair<KeyValuePair<int, int>, int> frequencies = new KeyValuePair<KeyValuePair<int, int>, int>();

        public void Build(List<KeyValuePair<int, int>> RLEList, List<int> numberOfElement)
        {
            for (int i = 0; i < RLEList.Count; ++i)
                if (numberOfElement[i] != 0)
                    nodes.Add(new Node(RLEList[i], numberOfElement[i]));

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.frequence).ToList<Node>();

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node(new KeyValuePair<int, int>(-1, -1), taken[0].frequence + taken[1].frequence, taken[0], taken[1]);

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                this.root = nodes.FirstOrDefault();
            }
        }

        public BitArray Encode(List<KeyValuePair<int, int>> RLEList)
        {
            List<bool> encodedRLEList = new List<bool>();

            for (int i = 0; i < RLEList.Count; ++i) {
                List<bool> encodedSymbol = this.root.Traverse(RLEList[i], new List<bool>());
                encodedRLEList.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedRLEList.ToArray());

            return bits;
        }

        public List<KeyValuePair<int, int>> Decode(BitArray bits)
        {
            Node current = this.root;            
            List<KeyValuePair<int, int>> decoded = new List<KeyValuePair<int, int>>();

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.rightChild != null)
                    {
                        current = current.rightChild;
                    }                        
                }
                else
                {
                    if (current.leftChild != null)
                    {
                        current = current.leftChild;
                    }                        
                } 

                if (IsLeaf(current))
                {
                    decoded.Add(current.pair);
                    current = this.root;
                }                
            }
            return decoded;

        }

        public bool IsLeaf(Node node)
        {
            return (node.leftChild == null && node.rightChild == null);
        }

        public class Node
        {
            public KeyValuePair<int, int> pair { get; set; }
            public int frequence { get; set; }
            public Node leftChild { get; set; }
            public Node rightChild { get; set; }

            public Node(KeyValuePair<int, int> pair, int frequence)
            {
                this.pair = pair;
                this.frequence = frequence;
            }
            public Node(KeyValuePair<int, int> pair, int frequence, Node leftChild, Node rightChild)
            {
                this.pair = pair;
                this.frequence = frequence;
                this.leftChild = leftChild;
                this.rightChild = rightChild;
            }

            public Node() {}

            public List<bool> Traverse(KeyValuePair<int, int> pair, List<bool> data)
            {
                // Leaf
                if (leftChild == null && rightChild == null)
                {
                    if (pair.Equals(this.pair))                    
                        return data;                    
                    else                    
                        return null;                    
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (leftChild != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);

                        left = leftChild.Traverse(pair, leftPath);
                    }

                    if (rightChild != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);

                        right = rightChild.Traverse(pair, rightPath);
                    }

                    if (left != null)
                        return left;
                    else
                        return right;
                }
            }
        }
    }
}
