namespace PipeLine
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
                        nextNode.AddSample(s, r);
                        await Task.CompletedTask;
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

        public void AddSample(Sample sample, object? parameters)
        {
            _nodes[0].AddSample(sample, parameters);
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

        public async Task StopAsync()
        {
            foreach (var node in _nodes)
            {
                await node.StopAsync();
            }
        }
    }
}
