using System;
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
            public string Value { get; set; }
            public int Weight { get; set; }
            public Node Left = null;
            public Node Right = null;
        }

        Dictionary<char, int> letterCount;
        public Dictionary<char, int> LetterCount => letterCount != null ? letterCount : new Dictionary<char, int>();

        public List<KeyValuePair<char, int>> SortedLetterCount
        {
            get
            {
                var list = letterCount.ToList();
                return SortList(list);
            }
        }

        private List<KeyValuePair<char, int>> SortList(List<KeyValuePair<char, int>> list)
        {
            list.Sort((p1, p2) => p2.Value.CompareTo(p1.Value));
            return list;
        }

        private List<Node> SortList(List<Node> list)
        {
            list.Sort((p1, p2) => p2.Weight.CompareTo(p1.Weight));
            return list;
        }

        string str;
        public string InnerString => str;

        public StringAnalizis() { letterCount = new Dictionary<char, int>(); }
        public StringAnalizis(string s) { str = s; letterCount = new Dictionary<char, int>(); }

        public Dictionary<char, int> CountLetters()
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
                    Value = e.Key.ToString()

                })).ToList();

            while(nodeList.Count > 1)
            {
                Node n = new Node();
                n.Right = nodeList[nodeList.Count - 1];
                n.Left = nodeList[nodeList.Count - 2];

                nodeList.RemoveRange(nodeList.Count - 2, 2);
                nodeList.Add(n);

                nodeList = SortList(nodeList);
            }

            return nodeList[0];
        }
    }
}
