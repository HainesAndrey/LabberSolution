using LabberClient.Students.StudentsTable;
using LabberClient.VMStuff;
using LabberLib.DataBaseContext;
using LabberLib.DataBaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LabberClient.Workspace.AdminTab.StudentsTab
{
    public class StudentsTabPageVM : LabberVMBase
    {
        public StudentsTablePage StudentsTablePage { get; set; }
        public List<Student> Students { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

        public StudentsTabPageVM(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
            : base(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent)
        {
            using (db = new DBWorker())
            {
                Students = db.Students.Include(x => x.Group).ToList();
                Groups = new ObservableCollection<Group>(db.Groups);
            }
            StudentsTablePage = new StudentsTablePage(Groups, Students, InvokeResponseEvent, InvokePageEnabledEvent, InvokeLoadingStateEvent, InvokeCompleteStateEvent );
        }
    }
}
