using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fomin05
{
    static class ProcessDb
    {
        private static Thread _updateDbThread;
        private static Thread _updateEntriesThread;
        public static Dictionary<int, Process> Processes;

        static ProcessDb()
        {
            Processes = new Dictionary<int, Process>();
            _updateEntriesThread = new Thread(UpdateEntries);
            _updateDbThread = new Thread(UpdateDb);
            _updateDbThread.Start();
            _updateEntriesThread.Start();
        }

        private static async void UpdateDb()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    try
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
                                Processes[proc.Id] = new Process(proc);
                        }
                    }
                    catch (Exception)
                    {
                        return;
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
                        Processes[id].CpuTaken = $"{Processes[id].CpuCounter.NextValue()} %";
                        Processes[id].RamTaken = $"{Processes[id].RamCounter.NextValue() / 1024 / 1024} MB";
                        Processes[id].ThreadsNumber = pr.Threads.Count;
                    }
                });
                
                Thread.Sleep(2000);
            }
        }
    }
}
