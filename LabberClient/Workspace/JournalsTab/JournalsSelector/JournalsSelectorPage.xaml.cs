using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.Workspace.JournalsTab.JournalsSelector
{

    public partial class JournalsSelectorPage : Page
    {
        public JournalsSelectorPage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new JournalsSelectorPageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as JournalsSelectorPageVM).SelectJournal((e.NewValue as Node).IdJournal);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }
    }
}
