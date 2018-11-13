using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler
{
    public static class Schedule
    {
        class ScheduleData
        {
            public bool Canceled;
            public TimeSpan Interval;

            public ScheduleData(TimeSpan interval)
            {
                Interval = interval;
            }
        }

        private static Dictionary<object, ScheduleData> tasks = new Dictionary<object, ScheduleData>();

        public static bool Stop(object tag)
        {
            if (tasks.ContainsKey(tag))
            {
                tasks[tag].Canceled = true;
                return true;
            }
            return false;
        }

        public static bool SetInterval(object tag, double interval)
        {
            return SetInterval(tag, TimeSpan.FromSeconds(interval));
        }
        public static bool SetInterval(object tag, TimeSpan interval)
        {
            if (tasks.ContainsKey(tag))
            {
                tasks[tag].Interval = interval;
                return true;
            }
            return false;
        }

        public static void Add(double sec, Action act)
        {
            Add(TimeSpan.FromSeconds(sec), act);
        }
        public static void Add(TimeSpan span, Action act)
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(span);
                act();
            });
        }
        public static void Add(double sec, object tag, Action act)
        {
            Add(TimeSpan.FromSeconds(sec), tag, act);
        }
        public static void Add(TimeSpan span, object tag, Action act)
        {
            if (tasks.ContainsKey(tag)) return;
            tasks.Add(tag, new ScheduleData(span));
            var task = Task.Run(() =>
            {
                var t = tag;
                Thread.Sleep(span);
                if (tasks[t].Canceled)
                {
                    tasks.Remove(t);
                    return;
                }
                tasks.Remove(t);
                act();
            });
        }

        public static async Task<T> Add<T>(double sec, Func<T> func)
        {
            return await Add(TimeSpan.FromSeconds(sec), func);
        }
        public static async Task<T> Add<T>(TimeSpan span, Func<T> func)
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(span);
                return func();
            });

            return await task;
        }
        public static async Task<T> Add<T>(double sec, object tag, Func<T> func)
        {
            return await Add(TimeSpan.FromSeconds(sec), tag, func);
        }
        public static async Task<T> Add<T>(TimeSpan span, object tag, Func<T> func)
        {
            if (tasks.ContainsKey(tag)) return default(T);
            tasks.Add(tag, new ScheduleData(span));
            var task = Task.Run(() =>
            {
                var t = tag;
                Thread.Sleep(span);
                if (tasks[t].Canceled)
                {
                    tasks.Remove(t);
                    return default(T);
                }
                tasks.Remove(t);
                return func();
            });

            return await task;
        }

        public static void Repeat(double waitSec, double intervalSec, long count, Action<int> act)
        {
            Repeat(TimeSpan.FromSeconds(waitSec), TimeSpan.FromSeconds(intervalSec), count, act);
        }
        public static void Repeat(TimeSpan wait, TimeSpan interval, long count, Action<int> act)
        {
            var task = Task.Run(() =>
            {
                Thread.Sleep(wait);
                var sw = new Stopwatch();
                var idx = 0;
                while (true)
                {
                    sw.Restart();
                    act(idx++);
                    if (count > 0) count--;
                    if (count == 0) break;
                    sw.Stop();
                    var d = interval - sw.Elapsed;
                    if (d.TotalSeconds > 0.0) Thread.Sleep(d);
                }
            });
        }
        public static void Repeat(double waitSec, double intervalSec, long count, object tag, Action<int> act)
        {
            Repeat(TimeSpan.FromSeconds(waitSec), TimeSpan.FromSeconds(intervalSec), count, tag, act);
        }
        public static void Repeat(TimeSpan wait, TimeSpan interval, long count, object tag, Action<int> act)
        {
            if (tasks.ContainsKey(tag)) return;
            tasks.Add(tag, new ScheduleData(interval));
            var task = Task.Run(() =>
            {
                var t = tag;
                Thread.Sleep(wait);
                if (tasks[t].Canceled)
                {
                    tasks.Remove(t);
                    return;
                }
                var sw = new Stopwatch();
                var idx = 0;
                while (true)
                {
                    sw.Restart();
                    act(idx++);
                    if (tasks[t].Canceled)
                    {
                        tasks.Remove(t);
                        return;
                    }
                    if (count > 0) count--;
                    if (count == 0) break;
                    sw.Stop();
                    var d = tasks[t].Interval - sw.Elapsed;
                    if (d.TotalSeconds > 0.0) Thread.Sleep(d);
                    if (tasks[t].Canceled)
                    {
                        tasks.Remove(t);
                        return;
                    }
                }
                
                tasks.Remove(tag);
            });

            
        }

        private static Dictionary<object, Stopwatch> timers = new Dictionary<object, Stopwatch>();

        public static void StartTimer(object tag)
        {
            if (timers.ContainsKey(tag))
            {
                timers[tag].Start();
            }
            else
            {
                var sw = new Stopwatch();
                sw.Start();
                timers.Add(tag, sw);
            }
            
        }
        public static void StopTimer(object tag)
        {
            if (!timers.ContainsKey(tag)) return;
            timers[tag].Stop();
        }
        public static double GetElapsed(object tag)
        {
            if (!timers.ContainsKey(tag)) return 0.0;
            return timers[tag].Elapsed.TotalSeconds;
        }
    }
}
