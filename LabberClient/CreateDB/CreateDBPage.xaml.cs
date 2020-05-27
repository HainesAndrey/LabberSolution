using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.CreateDB
{
    public partial class CreateDBPage : Page
    {
        public CreateDBPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new CreateDBPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }
    }
}