using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace Fomin05
{
    class Process
    {
        private readonly PerformanceCounter _ramCounter;
        private readonly PerformanceCounter _cpuCounter;
        private readonly System.Diagnostics.Process _process;

        #region Properties
        public string Name
        {
            get { return _process.ProcessName; }
        }

        public int Id
        {
            get { return _process.Id; }
        }

        public bool IsActive
        {
            get { return _process.Responding; }
        }

        public string CpuTaken
        {
            get
            {
                return $"{_cpuCounter.NextValue()} %";
            }
        }

        public string RamTaken
        {
            get { return $"{_ramCounter.NextValue() / 1024 / 1024} MB"; }
        }

        public int ThreadsNumber
        {
            get { return _process.Threads.Count; }
        }

        public string Username
        {
            get { return GetProcessOwner(_process.Id); }
        }

        public string FilePath
        {
            get
            {
                try
                {
                    return _process.MainModule.FileName;
                }
                catch (Win32Exception e)
                {
                    return e.Message;
                }
            }
        }

        public string RunOn
        {
            get
            {
                try
                {
                    return _process.StartTime.ToString();
                }
                catch (Win32Exception e)
                {
                    return e.Message;
                }
            }
        }
#endregion

        internal Process(ref System.Diagnostics.Process systemProcess)
        {
            _process = systemProcess;
            _ramCounter = new PerformanceCounter("Process", "Working Set", _process.ProcessName);
            _cpuCounter = new PerformanceCounter("Process", "% Processor Time", _process.ProcessName);
        }

        private string GetProcessOwner(int processId)
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
            for (int i=0;i<processes.Length;++i)
            {
                myProcesses.Add(new Process(ref processes[i]));
            }
            return myProcesses;
        }
    }
}
