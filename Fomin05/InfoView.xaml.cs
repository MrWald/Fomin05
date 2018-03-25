using System.Windows.Controls;

namespace Fomin05
{
    /// <summary>
    /// Interaction logic for InfoView.xaml
    /// </summary>
    internal partial class InfoView : UserControl
    {
        internal InfoView(System.Diagnostics.Process process)
        {
            InitializeComponent();
            DataContext = new InfoViewModel(process.Modules, process.Threads);
        }
    }
}
