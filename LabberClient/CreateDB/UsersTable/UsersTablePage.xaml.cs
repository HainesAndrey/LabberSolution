﻿using LabberClient.VMStuff;
using System.Windows.Controls;

namespace LabberClient.CreateDB.UsersTable
{
    public partial class UsersTablePage : Page
    {
        public UsersTablePage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new UsersTablePageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);
        }
    }
}
