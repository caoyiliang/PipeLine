using PipeLine;

namespace Test
{
    internal class 四轴机器人(string name) : Worker
    {
        public string Name { get; set; } = name;
        protected override async Task ProcessActionAsync(Sample sample)
        {
            Console.WriteLine($"{Name}:{sample.Name} 开始干活");
            await Task.Delay(5000);
            Console.WriteLine($"{Name}:{sample.Name} 结束干活");
    }
    }
}
