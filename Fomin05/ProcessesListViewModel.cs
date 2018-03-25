using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fomin05
{
    internal class ProcessesListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Process> _processes;
        private Action<bool> _showLoaderAction;
        private Thread _updateThread;
        public ObservableCollection<Process> Processes
        {
            get
            {
                return _processes;
            }
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal ProcessesListViewModel(Action<bool> showLoaderAction)
        {
           _showLoaderAction = showLoaderAction;
           
            _updateThread = new Thread(UpdateUsers);
            InitializeUsers();
            _updateThread.Start();
            
        }

        private async void UpdateUsers()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        List<Process> toAdd;
                        List<Process> toRemove;
                        try
                        {
                            toRemove = new List<Process>(Processes.Where(proc => !ProcessDb.Processes.ContainsKey(proc.Id)));
                            foreach (Process proc in toRemove)
                            {
                                Processes.Remove(proc);
                            }
                            toAdd = new List<Process>(ProcessDb.Processes.Values.Where(proc => !Processes.Contains(proc)));
                            foreach (Process proc in toAdd)
                            {
                                Processes.Add(proc);
                            }
                        }
                        catch (NullReferenceException)
                        {
                           return;
                        }
                        
                    });

                });

                Thread.Sleep(4000);
            }
        }

        private async void InitializeUsers()
        {
            _showLoaderAction.Invoke(true);
            await Task.Run(() => { Processes = new ObservableCollection<Process>(ProcessDb.Processes.Values); });
            _showLoaderAction.Invoke(false);
        }

        #region Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
