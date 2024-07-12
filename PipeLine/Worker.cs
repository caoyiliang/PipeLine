namespace PipeLine
{
    public abstract class Worker
    {
        public event WorkerCompletedEventHandler? WorkCompleted;

        private volatile bool _isBusy;
        public bool IsBusy { get => _isBusy; set => _isBusy = value; }

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task DoWorkAsync(Sample sample, object? parameters)
        {
            await _semaphore.WaitAsync();
            object? result = null;
            try
            {
                result = await ProcessActionAsync(sample, parameters);
            }
            finally
            {
                _semaphore.Release();
            }
            if (WorkCompleted != null) await WorkCompleted.Invoke(this, sample, result);
        }

        protected abstract Task<object?> ProcessActionAsync(Sample sample, object? parameters);
    }
}
