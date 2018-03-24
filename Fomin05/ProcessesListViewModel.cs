using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Fomin05
{
    internal class ProcessesListViewModel
    {
        public ObservableCollection<Process> Processes
        {
            get;
            private set;
        }

        internal ProcessesListViewModel()
        {
            InitializeUsers();
        }

        private async void InitializeUsers()
        {
            //await Task.Run(() =>
            //{
                Processes = new ObservableCollection<Process>(Process.GetProcesses());
                //Thread.Sleep(500);
           // });
        }
    }
}
