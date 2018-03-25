using System.Windows;
using FontAwesome.WPF;

namespace Fomin05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessesListView _processesListView;
        private ImageAwesome _loader;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_processesListView == null)
                _processesListView = new ProcessesListView(ShowLoader);
            MainGrid.Children.Add(_processesListView);
        }

        private void ShowLoader(bool isShow)
        {
            LoaderHelper.OnRequestLoader(MainGrid, ref _loader, isShow);
        }
    }
}
