namespace PipeLine;

public delegate Task WorkCompletedEventHandler(Sample sample);

public delegate Task WorkerCompletedEventHandler(Worker worker, Sample sample);
