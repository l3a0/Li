// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace ConsistentHashing
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class HashRing<T>
    {
        public HashRing(int replicationFactor = 100)
        {
            this.ReplicationFactor = replicationFactor;
            this.Ring = new SortedDictionary<long, T>();
        }

        public int Count => this.Ring.Count;

        public IEnumerable<T> Nodes => this.Ring.Select(pair => pair.Value);

        private int ReplicationFactor { get; }

        private SortedDictionary<long, T> Ring { get; }

        public void AddNode(T node)
        {
            for (var i = 0; i < this.ReplicationFactor; i++)
            {
                var nodeId = node.GetHashCode() + i.ToString().GetHashCode();
                var key = nodeId.GetHashCode();
                this.Ring[key] = node;
            }
        }

        public void RemoveNode(T node)
        {
            for (var i = 0; i < this.ReplicationFactor; i++)
            {
                var nodeId = node.GetHashCode() + i.GetHashCode();
                var key = nodeId.GetHashCode();
                this.Ring.Remove(key);
            }
        }

        public T GetNode(long itemKey)
        {
            if (this.Ring.ContainsKey(itemKey))
            {
                return this.Ring[itemKey];
            }

            var node = this.Ring.FirstOrDefault(pair => pair.Key >= itemKey).Value;

            if (node != null)
            {
                return node;
            }

            // What if item key range is > than node key range?
            return this.Ring.First().Value;
        }
    }
}