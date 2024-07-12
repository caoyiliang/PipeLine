namespace PipeLine;

public delegate Task WorkCompletedEventHandler(Sample sample, object? result);

public delegate Task WorkerCompletedEventHandler(Worker worker, Sample sample, object? result);
