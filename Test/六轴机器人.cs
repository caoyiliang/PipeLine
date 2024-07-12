using PipeLine;

namespace Test
{
    internal class 六轴机器人(string name) : Worker
    {
        public string Name { get; set; } = name;
        protected override async Task<object?> ProcessActionAsync(Sample sample, object? parameters)
        {
            Console.WriteLine($"{Name}:{sample.Name} 开始干活");
            await Task.Delay(10000);
            Console.WriteLine($"{Name}:{sample.Name} 结束干活");

            return new 六轴返回();
        }
    }

    internal class 六轴返回
    {
        public 六轴返回()
        {
        }
    }
}
