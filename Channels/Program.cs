using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;

namespace Channels
{
    internal class Program
    {
        public static Channel<int> TenChannel = Channel.CreateUnbounded<int>();
        public static ChannelWriter<int> TenWriter = TenChannel.Writer;
        public static ChannelReader<int> TenReader = TenChannel.Reader;

        public static Channel<int> NumberChannel = Channel.CreateUnbounded<int>();
        public static ChannelWriter<int> NumberWriter = NumberChannel.Writer;
        public static ChannelReader<int> NumberReader = NumberChannel.Reader;
        static async Task Main(string[] args)
        {
            var task = Run();

            task.Wait();
        }
        static async Task Run()
        {
            var taskNumber = WriteNumbersAsync();
            var taskTens = WriteTensAsync();
            var tasks = new List<Task> { taskNumber, taskTens };
            while (tasks.Count > 0)
            {
                var ready = await Task.WhenAny(tasks);
                tasks.Remove(ready);
            }
        }
        public static int GetDelay()
        {
            return new Random().Next(500, 1000);
        }
        public static async Task WriteAsync(ChannelWriter<int> writer, int value)
        {
            await writer.WriteAsync(value);
        }
        public static async Task WriteNumbersAsync()
        {
            int number = 0;
            while (number < 10)
            {
                number++;
                await WriteAsync(NumberWriter, number);
                await Task.Delay(GetDelay());
                int valueFromChannel = await NumberReader.ReadAsync();
                Console.WriteLine(valueFromChannel);
            }
            NumberWriter.Complete();
        }
        public static async Task WriteTensAsync()
        {
            int ten = 0;
            while(ten < 100)
            {
                ten += 10;
                await WriteAsync(TenWriter, ten);
                await Task.Delay(GetDelay());
                int valueFromChannel = await TenReader.ReadAsync();
                Console.WriteLine(valueFromChannel);
            }
            TenWriter.Complete();
        }
    }
}