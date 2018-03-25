using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Fomin05
{
    internal static class ProcessDb
    {
        private static readonly Thread UpdateDbThread;
        private static readonly Thread UpdateEntriesThread;
        public static Dictionary<int, Process> Processes;

        static ProcessDb()
        {
            Processes = new Dictionary<int, Process>();
            UpdateEntriesThread = new Thread(UpdateEntries);
            UpdateDbThread = new Thread(UpdateDb);
            UpdateDbThread.Start();
            UpdateEntriesThread.Start();
        }

        internal static void Close()
        {
            UpdateDbThread.Join(100);
            UpdateEntriesThread.Join(100);
        }

        private static async void UpdateDb()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    List<System.Diagnostics.Process> processes = System.Diagnostics.Process.GetProcesses().ToList();
                    IEnumerable<int> keys = Processes.Keys.ToList().Where(id => processes.All(proc => proc.Id != id));
                    foreach (int key in keys)
                    {
                        Processes.Remove(key);
                    }
                    foreach (System.Diagnostics.Process proc in processes)
                    {
                        if (!Processes.ContainsKey(proc.Id))
                        {
                            try
                            {
                                Processes[proc.Id] = new Process(proc);
                            }
                            catch (InvalidOperationException)
                            {
                                continue;
                            }
                            catch (ManagementException)
                            {
                                continue;
                            }
                        }
                    }
                });
                Thread.Sleep(5000);
            }
        }

        private static async void UpdateEntries()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    foreach (int id in Processes.Keys.ToList())
                    {
                        System.Diagnostics.Process pr;
                        try
                        {
                            pr = System.Diagnostics.Process.GetProcessById(id);
                        }
                        catch (ArgumentException)
                        {
                            Processes.Remove(id);
                            continue;
                        }
                        Processes[id].CpuTaken = (int)Processes[id].CpuCounter.NextValue();
                        Processes[id].RamTaken = (int)(Processes[id].RamCounter.NextValue() / 1024 / 1024);
                        Processes[id].ThreadsNumber = pr.Threads.Count;
                    }
                });
                
                Thread.Sleep(2000);
            }
        }
    }
}
