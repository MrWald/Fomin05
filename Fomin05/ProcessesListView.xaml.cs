using System;
using System.Threading;
using System.Windows.Controls;

namespace Fomin05
{
    /// <summary>
    /// Interaction logic for ProcessesListView.xaml
    /// </summary>
    internal partial class ProcessesListView : UserControl
    {
        internal ProcessesListView(Action<bool> showLoaderAction)
        {
            InitializeComponent();
            DataContext = new ProcessesListViewModel(showLoaderAction);
        }

        internal void Close()
        {
            ((ProcessesListViewModel)DataContext).Close();
        }
    }
}
