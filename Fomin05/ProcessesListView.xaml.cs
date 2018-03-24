using System.Windows.Controls;

namespace Fomin05
{
    /// <summary>
    /// Interaction logic for ProcessesListView.xaml
    /// </summary>
    internal partial class ProcessesListView : UserControl
    {
        public ProcessesListView()
        {
            InitializeComponent();
            DataContext = new ProcessesListViewModel();
        }
    }
}
