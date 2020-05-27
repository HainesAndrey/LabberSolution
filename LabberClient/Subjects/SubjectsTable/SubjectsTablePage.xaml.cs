using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Subjects.SubjectsTable
{
    public partial class SubjectsTablePage : Page
    {
        public SubjectsTablePage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new SubjectsTablePageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }
    }
}
