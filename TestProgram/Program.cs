using Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Schedule.Repeat(0.0, 5.0, -1, "a", idx =>
            {
                Schedule.Repeat(0.0, 0.1, 10, "b", idx2 => 
                {
                    Console.WriteLine((idx + 1) + ":" + (idx2 + 1));
                });
            });

            string str = null;
            while ((str = Console.ReadLine()) != "q")
            {
                var interval = double.Parse(str);
                Schedule.SetInterval("b", interval);
            }
        }

        static async void Test()
        {
            var res = await Schedule.Add(3.0, () =>
            {
                return 10;
            });
            Console.WriteLine(res);
        }
    }
}
