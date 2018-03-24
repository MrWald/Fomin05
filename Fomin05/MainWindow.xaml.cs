using System.Windows;

namespace Fomin05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessesListView _processesListView;
        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processesListView == null)
                _processesListView = new ProcessesListView();
            MainGrid.Children.Add(_processesListView);
        }
    }
}
