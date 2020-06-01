using System.Windows;
using System.Windows.Input;

namespace LabberClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
        }

        private void ClearFocus(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}
