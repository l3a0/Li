namespace ConsistentHashing
{
    using System;
    using System.Text;

    using Murmur;

    internal sealed class Item
    {
        public Item(string name)
        {
            this.Name = name;
            this.Murmur = MurmurHash.Create32();
        }

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
            if (!(obj is Item otherItem))
            {
                return false;
            }

            return this.Name == otherItem.Name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}