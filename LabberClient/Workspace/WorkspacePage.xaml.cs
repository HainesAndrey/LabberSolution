﻿using LabberClient.VMStuff;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace LabberClient.Workspace
{
    public partial class WorkspacePage : Page
    {
        //private bool _allowDirectNavigation = false;
        //private NavigatingCancelEventArgs _navArgs = null;
        //private Duration _duration = new Duration(TimeSpan.FromSeconds(0.25));

        public WorkspacePage(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            InitializeComponent();
            DataContext = new WorkspacePageVM(ResponseEvent, PageEnabledEvent, LoadingStateEvent, CompleteStateEvent);

            //NavigationCommands.BrowseBack.InputGestures.Clear();
            //NavigationCommands.BrowseForward.InputGestures.Clear();
        }

        //private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        //{
        //    if (Content != null && !_allowDirectNavigation)
        //    {
        //        e.Cancel = true;

        //        _navArgs = e;

        //        DoubleAnimation animation = new DoubleAnimation();
        //        animation.From = 1;
        //        animation.To = 0;
        //        animation.Duration = _duration;
        //        animation.Completed += SlideCompleted;
        //        (DataContext as WorkspacePageVM).PageEnabledState = false;
        //        frame.BeginAnimation(OpacityProperty, animation);
        //    }
        //    _allowDirectNavigation = false;
        //}

        //private void SlideCompleted(object sender, EventArgs e)
        //{
        //    _allowDirectNavigation = true;
        //    switch (_navArgs.NavigationMode)
        //    {
        //        case NavigationMode.New:
        //            if (_navArgs.Uri == null)
        //                frame.Navigate(_navArgs.Content);
        //            else
        //                frame.Navigate(_navArgs.Uri);
        //            break;
        //        case NavigationMode.Back:
        //            frame.GoBack();
        //            break;
        //        case NavigationMode.Forward:
        //            frame.GoForward();
        //            break;
        //        case NavigationMode.Refresh:
        //            frame.Refresh();
        //            break;
        //    }

        //    Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
        //        (ThreadStart)delegate ()
        //        {
        //            DoubleAnimation animation = new DoubleAnimation();
        //            animation.From = 0;
        //            animation.To = 1;
        //            animation.Duration = _duration;
        //            frame.BeginAnimation(OpacityProperty, animation);
        //            (DataContext as WorkspacePageVM).PageEnabledState = true;
        //        });
        //}

        //private void ClearFocus(object sender, MouseButtonEventArgs e)
        //{
        //    Keyboard.ClearFocus();
        //}
    }
}