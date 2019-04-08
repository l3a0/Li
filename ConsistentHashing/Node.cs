namespace ConsistentHashing
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Murmur;

    internal sealed class Node
    {
        public Node(string name)
        {
            this.Name = name;
            this.Murmur = MurmurHash.Create32();
            this.Items = new List<Item>();
        }

        public List<Item> Items { get; }

        public string Name { get; }

        private Murmur32 Murmur { get; }

        public override int GetHashCode()
        {
            var data = Encoding.ASCII.GetBytes(this.Name);
            var hash = this.Murmur.ComputeHash(data);

            return BitConverter.ToInt32(hash, 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Node otherNode))
            {
                return false;
            }

            return this.Name == otherNode.Name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}