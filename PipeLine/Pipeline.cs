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
                    _nodes[i].WorkCompleted += async s =>
                    {
                        nextNode.AddSample(s);
                        await Task.CompletedTask;
                    };
                }
                else
                {
                    _nodes[i].WorkCompleted += async s =>
                    {
                        if (WorkCompleted != null) await WorkCompleted.Invoke(s);
                    };
                }
            }
        }

        public void AddSample(Sample sample)
        {
            _nodes[0].AddSample(sample);
        }

        public async Task StartAsync()
        {
            foreach (var node in _nodes)
            {
                await node.StartAsync();
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
