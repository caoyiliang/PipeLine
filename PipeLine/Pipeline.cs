using System.Collections.Concurrent;
using System.Xml.Linq;

namespace PipeLine
{
    public class Pipeline
    {
        public event WorkCompletedEventHandler? WorkCompleted;

        private readonly List<Node> _nodes;
        private readonly ConcurrentQueue<Sample> _samplesQueue;

        public Pipeline(List<Node> nodeCapacities)
        {
            _nodes = nodeCapacities;
            _samplesQueue = new();
        }

        public void AddSample(Sample sample)
        {
            _samplesQueue.Enqueue(sample);
        }

        public async Task StartAsync()
        {
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
                    await _nodes[i].StartAsync();
                }
                else
                {
                    _nodes[i].WorkCompleted += async s =>
                    {
                        if (WorkCompleted != null) await WorkCompleted.Invoke(s);
                    };
                    await _nodes[i].StartAsync();
                }
            }
            while (_samplesQueue.TryDequeue(out var sample))
            {
                _nodes[0].AddSample(sample);
            }
        }
    }
}
