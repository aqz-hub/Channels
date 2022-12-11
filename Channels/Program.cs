using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Channels
{
    internal class Program
    {
        private static List<int> numbers = new List<int>();
        private static List<int> tens = new List<int>();
        private static int _lastNumber;
        public static int LastNumber
        {
            get { return _lastNumber; }
            set
            {
                if (_lastNumber != value)
                {
                    _lastNumber = value;
                    numbers.Add(value);
                }
            }
        }
        private static int _lastTen;
        public static int LastTen
        {
            get { return _lastTen; }
            set
            {
                if(_lastTen != value)
                {
                    _lastTen = value;
                    tens.Add(value);
                }
            }
        }
        static async Task Main(string[] args)
        {
            var task = WriteAsync();
            Read();
            task.Wait();
        }
        public static void Read()
        {
            int number = 0;
            int ten = 0;
            while (true)
            {
                if(number == 0 && ten == 0)
                {
                    number = LastNumber;
                    ten = LastTen;
                    Console.WriteLine(number.ToString() + " " + ten.ToString());
                }
                if (number != LastNumber && ten != LastTen)
                {
                    Console.WriteLine(LastNumber.ToString() + " " + LastTen.ToString());
                    number = LastNumber;
                    ten = LastTen;
                }
            }
        }
        public static void WriteNumber(int value)
        {
            LastNumber = value;
        }
        public static void WriteTen(int value)
        {
            LastTen = value;
        }
        public static int GetDelay()
        {
            return new Random().Next(500, 1000);
        }
        public static async Task WriteNumbersAsync()
        {
            int number = 0;
            while(number < 10)
            {
                number++;
                WriteNumber(number);
                await Task.Delay(GetDelay());
                await Task.Yield();
            }
        }
        public static async Task WriteTensAsync()
        {
            int number = 0;
            while(number < 100)
            {
                number += 10;
                WriteTen(number);
                await Task.Delay(GetDelay());
                await Task.Yield();
            }
        }

        static async Task WriteAsync()
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
    }
}