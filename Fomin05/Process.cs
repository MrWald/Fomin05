using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Windows.Documents;

namespace Fomin05
{
    class Process
    {
        public string Name { get; }
        public int Id { get; }
        public bool IsActive { get; }
        public float CpuTaken { get; }
        public string RamTaken { get; }
        public int ThreadsNumber { get; }
        public string Username { get; }
        public string FilePath { get; }
        public string RunOn { get; }

        internal Process(System.Diagnostics.Process systemProcess)
        {
            Name = systemProcess.ProcessName;
            Id = systemProcess.Id;
            IsActive = systemProcess.Responding;
            PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", systemProcess.ProcessName);
            PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", systemProcess.ProcessName);
            CpuTaken = cpuCounter.NextValue();
            //Thread.Sleep(500);
            CpuTaken = cpuCounter.NextValue();
            RamTaken = $"{ramCounter.NextValue() / 1024 / 1024} MB";
            ThreadsNumber = systemProcess.Threads.Count;
            Username = GetProcessOwner(systemProcess.Id);
            try
            {
                FilePath = systemProcess.MainModule.FileName;
            }
            catch (Win32Exception e)
            {
                FilePath = e.Message;
            }
            
            try
            {
                RunOn = systemProcess.StartTime.ToString();
            }
            catch (Win32Exception accessDeniedException)
            {
                RunOn = accessDeniedException.Message;
            }
        }
        public string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
        internal static IEnumerable<Process> GetProcesses()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            List<Process> myProcesses = new List<Process>(processes.Length);
            foreach (System.Diagnostics.Process process in processes)
            {
                myProcesses.Add(new Process(process));
            }
            return myProcesses;
        }
    }
}
