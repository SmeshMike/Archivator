using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archivating
{
    public class StringAnalizis
    {
        class Node
        {
            public byte? Value { get; set; }
            public int Weight { get; set; }
            public Node Left = null;
            public Node Right = null;
        }

        Dictionary<byte, int> letterCount;
        public Dictionary<byte, int> LetterCount => letterCount != null ? letterCount : new Dictionary<byte, int>();

        public List<KeyValuePair<byte, int>> SortedLetterCount
        {
            get
            {
                var list = letterCount.ToList();
                return SortList(list);
            }
        }

        private List<KeyValuePair<byte, int>> SortList(List<KeyValuePair<byte, int>> list)
        {
            list.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));
            return list;
        }

        private List<Node> SortList(List<Node> list)
        {
            list.Sort((p1, p2) => p2.Weight.CompareTo(p1.Weight));
            return list;
        }

        byte[] str;
        public byte[] InnerString
        {
            get => str;
            set => str = value;
        }

        public StringAnalizis() { letterCount = new Dictionary<byte, int>(); }
        public StringAnalizis(byte[] s) { str = s; letterCount = new Dictionary<byte, int>(); }

        public Dictionary<byte, int> CountLetters()
        {
            foreach(var c in str)
            {
                if (!letterCount.Keys.Contains(c))
                    letterCount.Add(c, 1);
                else
                    letterCount[c] = letterCount[c] + 1;
            }

            return letterCount;
        }

        private Node BuildTree()
        {
            var nodeList = (SortedLetterCount
                .Select(e => new Node()
                {
                    Weight = e.Value,
                    Value = e.Key

                })).ToList();

            while(nodeList.Count > 1)
            {
                Node n = new Node();
                n.Weight = nodeList[nodeList.Count - 1].Weight + nodeList[nodeList.Count - 2].Weight;
                n.Right = nodeList[nodeList.Count - 1];
                n.Left = nodeList[nodeList.Count - 2];

                nodeList.RemoveRange(nodeList.Count - 2, 2);
                nodeList.Add(n);

                nodeList = SortList(nodeList);
            }

            return nodeList[0];
        }

        private void MakeWord(Node n, BitArray word, ref Dictionary<byte,BitArray> dic)
        {
            if (n.Value != null)
            {
                if (word.Length == 0)
                    dic.Add(n.Value.Value, new BitArray(1, true));
                else
                    dic.Add(n.Value.Value, word);
                return;
            }

            var arrayLeft = new BitArray(word.Length + 1);
            var arrayRight = new BitArray(word.Length + 1);
            for (int i = 0; i < word.Length; i++)
            {
                arrayLeft.Set(i, word.Get(i));
                arrayRight.Set(i, word.Get(i));
            }
            arrayLeft.Set(arrayLeft.Length - 1, true);
            MakeWord(n.Left, arrayLeft, ref dic);
            arrayRight.Set(arrayRight.Length - 1, false);
            MakeWord(n.Right, arrayRight, ref dic);

        }

        public Dictionary<byte, BitArray> Encrypt()
        {
            var root = BuildTree();
            Dictionary<byte, BitArray> dic = new Dictionary<byte, BitArray>();
            MakeWord(root, new BitArray(0), ref dic);

            return dic;
        }
    }
}
