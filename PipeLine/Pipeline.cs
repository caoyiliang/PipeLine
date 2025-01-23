﻿namespace PipeLine
{
    public class Pipeline
    {
        public event WorkCompletedEventHandler? WorkCompleted;

        private readonly List<Node> _nodes;

        public Pipeline(List<Node> nodeCapacities)
        {
            _nodes = nodeCapacities;
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (i + 1 < _nodes.Count)
                {
                    var nextNode = _nodes[i + 1];
                    _nodes[i].WorkCompleted += async (s, r) =>
                    {
                        await nextNode.AddSampleAsync(s, r);
                    };
                }
                else
                {
                    _nodes[i].WorkCompleted += async (s, r) =>
                    {
                        if (WorkCompleted != null) await WorkCompleted.Invoke(s, r);
                    };
                }
            }
        }

        public async Task AddSampleAsync(Sample sample, object? parameters)
        {
            await _nodes[0].AddSampleAsync(sample, parameters);
        }

        public async Task StartAsync()
        {
            foreach (var node in _nodes)
            {
                await node.StartAsync();
            }
        }

        public void ClearTasks()
        {
            foreach (var node in _nodes)
            {
                node.ClearTasks();
            }
        }

        public async Task StopWorkAsync()
        {
            foreach (var node in _nodes)
            {
                await node.StopWorkAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (var node in _nodes)
            {
                await node.StopAsync();
            }
        }
    }
}
