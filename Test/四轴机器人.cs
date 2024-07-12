using PipeLine;

namespace Test
{
    internal class 四轴机器人(string name) : Worker
    {
        public string Name { get; set; } = name;
        protected override async Task<object?> ProcessActionAsync(Sample sample, object? parameters)
        {
            Console.WriteLine($"{Name}:{sample.Name} 开始干活");
            var rs = (六轴返回)parameters!;
            await Task.Delay(5000);
            Console.WriteLine($"{Name}:{sample.Name} 结束干活");
            return null;
        }
    }
}
