using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.DebtsTab
{
    public partial class DebtsTabPage : Page
    {
        public DebtsTabPage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new DebtsTabPageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
            (DataContext as DebtsTabPageVM).UpdateTable += DebtsTabPage_UpdateTable;
        }

        private void DebtsTabPage_UpdateTable(System.Data.DataTable datatable)
        {
            table.ItemsSource = datatable.DefaultView;
            foreach (var item in table.Columns)
            {
                item.Header = datatable.Columns[item.Header.ToString()].Caption;
            }
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }
    }
}
