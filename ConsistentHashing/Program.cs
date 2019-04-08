namespace ConsistentHashing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MathNet.Numerics.Statistics;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var numberOfNodes = 6;
            var numberOfItems = 10000;

            RunModuloNExperiment(numberOfNodes, numberOfItems);
            RunConsistentHashingExperiment(numberOfNodes, numberOfItems);
            RunConsistentHashingExperiment(numberOfNodes, numberOfItems, replicationFactor: 500);
        }

        private static void RunConsistentHashingExperiment(int numberOfNodes, int numberOfItems, int replicationFactor = 100)
        {
            Console.WriteLine($"===== {nameof(RunConsistentHashingExperiment)}(replicationFactor: {replicationFactor}) =====");
            var hashRings = new List<HashRing<Node>>();

            for (var i = 0; i < numberOfNodes; i++)
            {
                var hashRing = new HashRing<Node>(replicationFactor: replicationFactor);
                hashRings.Add(hashRing);

                for (var j = 0; j < numberOfNodes; j++)
                {
                    hashRing.AddNode(new Node($"Node{j}"));
                }
            }

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = new Item(Guid.NewGuid().ToString());
                var itemKey = item.GetHashCode();

                foreach (var hashRing in hashRings)
                {
                    var targetNode = hashRing.GetNode(itemKey);
                    targetNode.Items.Add(item);
                }
            }

            foreach (var hashRing in hashRings)
            {
                foreach (var node in hashRing.Nodes.Distinct().OrderBy(node => node.Name))
                {
                    Console.WriteLine($"Node '{node.Name}' has '{node.Items.Count}' items.");
                }

                Console.WriteLine();
            }

            var itemCounts = hashRings.First().Nodes.Distinct().Select(node => (double)node.Items.Count).ToList();
            var averageItemsPerNode = itemCounts.Average();
            var standardDeviation = itemCounts.StandardDeviation();

            Console.WriteLine($"Total number of items is '{itemCounts.Sum()}'.");
            Console.WriteLine($"Average number of items per node is '{averageItemsPerNode}' with standard deviation '{standardDeviation}'.");
        }

        private static void RunModuloNExperiment(int numberOfNodes, int numberOfItems)
        {
            Console.WriteLine($"===== {nameof(RunModuloNExperiment)} =====");
            var nodes = new List<Node>();

            for (var i = 0; i < numberOfNodes; i++)
            {
                nodes.Add(new Node($"Node{i}"));
            }

            for (var i = 0; i < numberOfItems; i++)
            {
                var item = new Item(Guid.NewGuid().ToString());
                var itemKey = Math.Abs(item.GetHashCode() % nodes.Count);
                var targetNode = nodes[itemKey];
                targetNode.Items.Add(item);
            }

            foreach (var node in nodes.OrderBy(node => node.Name))
            {
                Console.WriteLine($"Node '{node.Name}' has '{node.Items.Count}' items.");
            }

            var itemCounts = nodes.Select(node => (double)node.Items.Count).ToList();
            var averageItemsPerNode = itemCounts.Average();
            var standardDeviation = itemCounts.StandardDeviation();

            Console.WriteLine();
            Console.WriteLine($"Total number of items is '{itemCounts.Sum()}'.");
            Console.WriteLine($"Average number of items per node is '{averageItemsPerNode}' with standard deviation '{standardDeviation}'.");
            Console.WriteLine();
        }
    }
}