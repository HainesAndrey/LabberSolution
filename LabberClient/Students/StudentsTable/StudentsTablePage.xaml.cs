using LabberClient.VMStuff;
using LabberLib.DataBaseContext.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LabberClient.Students.StudentsTable
{
    public partial class StudentsTablePage : Page
    {
        public StudentsTablePage(ResponseHandler responseEvent, PageEnabledHandler pageEnabledEvent, LoadingStateHandler loadingStateEvent, CompleteStateHanlder completeStateEvent)
        {
            InitializeComponent();
            DataContext = new StudentsTablePageVM(responseEvent, pageEnabledEvent, loadingStateEvent, completeStateEvent);
        }

        private void page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as LabberVMBase).LoadData();
        }
    }
}
