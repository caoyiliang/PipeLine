// See https://aka.ms/new-console-template for more information
using PipeLine;
using Test;
Console.WriteLine("Hello, World!");

Pipeline pipeline = new(
[
    new(
    "节点一",
    [
        new 六轴机器人("六轴机器人1"),
        new 六轴机器人("六轴机器人2")
    ]),
    new(
    "节点二",
    [
        new 四轴机器人("四轴机器人")
    ])
]);

pipeline.WorkCompleted += async s =>
{
    Console.WriteLine($"Pipeline 样品: {s.Name} 已完成");
    await Task.CompletedTask;
};

for (int i = 0; i < 10; i++)
{
    pipeline.AddSample(new Sample($"Sample{i}"));
}

await pipeline.StartAsync();

Console.ReadLine();