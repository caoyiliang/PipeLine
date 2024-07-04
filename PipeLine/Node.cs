using System.Xml.Linq;
using Utils;

namespace PipeLine
{
    public class Node
    {
        public string Name { get; set; }

        public event WorkCompletedEventHandler? WorkCompleted;

        private readonly List<Worker> _workers;
        private readonly PushQueue<Sample> _samplesQueue;
        public Node(string name, List<Worker> workers)
        {
            Name = name;
            _workers = workers;
            foreach (var worker in _workers)
            {
                worker.WorkCompleted += Work_WorkCompleted;
            }
            _samplesQueue = new()
            {
                MaxCacheCount = int.MaxValue
            };
            _samplesQueue.OnPushData += _samplesQueue_OnPushData;
        }

        public async Task StartAsync()
        {
            await _samplesQueue.StartAsync();
        }

        private async Task _samplesQueue_OnPushData(Sample sample)
        {
            Console.WriteLine($"节点{Name}:样品[{sample.Name}] 等工人");
            var worker = _workers.FirstOrDefault(_ => _.IsBusy == false);

            while (worker == null)
            {
                await Task.Delay(100);
                worker = _workers.FirstOrDefault(_ => _.IsBusy == false);
            }
            worker.IsBusy = true;
            _ = Task.Run(async () => await worker.DoWorkAsync(sample));
            Console.WriteLine($"节点{Name}:样品[{sample.Name}] 分到工人");
        }

        private async Task Work_WorkCompleted(Worker worker, Sample sample)
        {
            worker.IsBusy = false;
            if (WorkCompleted != null) await WorkCompleted.Invoke(sample);
        }

        public void AddSample(Sample sample)
        {
            _samplesQueue.PutInData(sample);
        }
    }
}