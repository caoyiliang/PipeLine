using Utils;

namespace PipeLine
{
    public class Node
    {
        public string Name { get; set; }

        public event WorkCompletedEventHandler? WorkCompleted;

        private readonly List<Worker> _workers;
        private readonly PushQueue<(Sample sample, object? parameters)> _samplesQueue;
        private bool _isActive = false;
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

        private async Task _samplesQueue_OnPushData((Sample sample, object? parameters) task)
        {
            var worker = _workers.FirstOrDefault(_ => _.IsBusy == false);

            while (worker == null)
            {
                await Task.Delay(100);
                if (_isActive) worker = _workers.FirstOrDefault(_ => _.IsBusy == false);
            }
            worker.IsBusy = true;
            _ = Task.Run(async () => await worker.DoWorkAsync(task.sample, task.parameters));
        }

        private async Task Work_WorkCompleted(Worker worker, Sample sample, object? result)
        {
            worker.IsBusy = false;
            if (WorkCompleted != null) await WorkCompleted.Invoke(sample, result);
        }

        public void AddSample(Sample sample, object? parameters) => _samplesQueue.PutInData((sample, parameters));

        public async Task StartAsync()
        {
            _isActive = true;
            await _samplesQueue.StartAsync();
        }

        public async Task StopAsync()
        {
            _isActive = false;
            await Task.CompletedTask;
        }
    }
}
