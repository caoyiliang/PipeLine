namespace PipeLine
{
    public abstract class Worker
    {
        public event WorkerCompletedEventHandler? WorkCompleted;

        private volatile bool _isBusy;
        public bool IsBusy { get => _isBusy; set => _isBusy = value; }

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task DoWorkAsync(Sample sample)
        {
            await _semaphore.WaitAsync();
            try
            {
                await ProcessActionAsync(sample);
            }
            finally
            {
                _semaphore.Release();
                if (WorkCompleted != null) await WorkCompleted.Invoke(this, sample);
            }
        }

        protected abstract Task ProcessActionAsync(Sample sample);
    }
}
