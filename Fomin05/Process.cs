using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace Fomin05
{
    class Process
    {
        #region Properties

        public PerformanceCounter RamCounter { get; }

        public PerformanceCounter CpuCounter { get; }

        public string Name
        {
            get;
        }

        public int Id
        {
            get;
        }

        public bool IsActive
        {
            get;
        }

        public string CpuTaken
        {
            get;
            set;
        }

        public string RamTaken
        {
            get;
            set;
        }

        public int ThreadsNumber
        {
            get;
            set;
        }

        public string Username
        {
            get;
        }

        public string FilePath
        {
            get;
        }

        public string RunOn
        {
            get;
        }
        #endregion

        internal Process(System.Diagnostics.Process systemProcess)
        {
            RamCounter = new PerformanceCounter("Process", "Working Set", systemProcess.ProcessName);
            CpuCounter = new PerformanceCounter("Process", "% Processor Time", systemProcess.ProcessName);
            Name = systemProcess.ProcessName;
            Id = systemProcess.Id;
            IsActive = systemProcess.Responding;
            CpuTaken = $"{CpuCounter.NextValue()} %";
            RamTaken = $"{RamCounter.NextValue() / 1024 / 1024} MB";
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
            catch (Win32Exception e)
            {
                RunOn = e.Message;
            }
        }

        private static string GetProcessOwner(int processId)
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

        /*internal static List<Process> GetProcesses()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            List<Process> myProcesses = new List<Process>(processes.Length);
            for (int i=0;i<processes.Length;++i)
            {
                myProcesses.Add(new Process(ref processes[i]));
            }
            return myProcesses;
        }*/
        
    }
}
