﻿using Utils;

namespace PipeLine
{
    public class Node
    {
        public string Name { get; set; }

        public event WorkCompletedEventHandler? WorkCompleted;

        private readonly List<Worker> _workers;
        private PushQueue<(Sample sample, object? parameters)> _samplesQueue;
        private bool _isActive = false;
        private CancellationTokenSource cancellationToken = new();
        private Task? _workTask;
        public Node(string name, List<Worker> workers)
        {
            Name = name;
            _workers = workers;
            foreach (var worker in _workers)
            {
                worker.WorkCompleted += Work_WorkCompleted;
            }
            _samplesQueue = InitQueue();
        }

        private PushQueue<(Sample sample, object? parameters)> InitQueue()
        {
            var samplesQueue = new PushQueue<(Sample sample, object? parameters)>()
            {
                MaxCacheCount = int.MaxValue
            };
            samplesQueue.OnPushData += SamplesQueue_OnPushData;
            return samplesQueue;
        }

        private async Task SamplesQueue_OnPushData((Sample sample, object? parameters) task)
        {
            var worker = _workers.FirstOrDefault(_ => _.IsBusy == false);

            while (worker == null)
            {
                await Task.Delay(100);
                if (_isActive) worker = _workers.FirstOrDefault(_ => _.IsBusy == false);
            }
            worker.IsBusy = true;
            try
            {
                _workTask = Task.Run(async () => await worker.DoWorkAsync(task.sample, task.parameters, cancellationToken), cancellationToken.Token);
            }
            catch
            {
            }
        }

        private async Task Work_WorkCompleted(Worker worker, Sample sample, object? result)
        {
            worker.IsBusy = false;
            if (WorkCompleted != null) await WorkCompleted.Invoke(sample, result);
        }

        public async Task AddSampleAsync(Sample sample, object? parameters) => await _samplesQueue.PutInDataAsync((sample, parameters));

        public async Task StartAsync()
        {
            foreach (var worker in _workers)
            {
                worker.IsBusy = false;
            }
            _isActive = true;
            cancellationToken = new();
            await _samplesQueue.StartAsync();
        }

        public void ClearTasks()
        {
            _samplesQueue.Clear();
        }

        public async Task StopAsync()
        {
            _isActive = false;
            await Task.CompletedTask;
        }

        internal async Task StopWorkAsync()
        {
            cancellationToken.Cancel();
            if (_workTask != null) await _workTask;
        }
    }
}
