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
        private readonly Action<bool> _showLoaderAction;
        private readonly Thread _updateThread;

        public ObservableCollection<Process> Processes
        {
            get => _processes;
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
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            List<Process> toRemove = new List<Process>(Processes.Where(proc => !ProcessDb.Processes.ContainsKey(proc.Id)));
                            foreach (Process proc in toRemove)
                            {
                                Processes.Remove(proc);
                            }
                            List<Process> toAdd = new List<Process>(ProcessDb.Processes.Values.Where(proc => !Processes.Contains(proc)));
                            foreach (Process proc in toAdd)
                            {
                                Processes.Add(proc);
                            }
                        }
                        catch (NullReferenceException)
                        {
                            return;
                        }
                        //catch (NullE)
                        //{
                        //    return;
                        // }
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

        internal void Close()
        {
            _updateThread.Join(100);
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
