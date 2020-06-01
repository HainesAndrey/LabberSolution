using LabberLib.DataBaseContext;
using MvvmCross.ViewModels;

namespace LabberClient.VMStuff
{
    public abstract class LabberVMBase : MvxViewModel, ILabberVM
    {
        public event ResponseHandler ResponseEvent;
        public event PageEnabledHandler PageEnabledEvent;
        public event LoadingStateHandler LoadingStateEvent;
        public event CompleteStateHanlder CompleteStateEvent;

        protected DBWorker db;

        public LabberVMBase(ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            this.ResponseEvent = ResponseEvent;
            this.PageEnabledEvent = PageEnabledEvent;
            this.LoadingStateEvent = LoadingStateEvent;
            this.CompleteStateEvent = CompleteStateEvent;
        }

        public virtual void LoadData() { }

        public void InvokeResponseEvent(ResponseType responseType, string msg)
        {
            ResponseEvent?.Invoke(responseType, msg);
        }

        public void InvokePageEnabledEvent(bool state)
        {
            PageEnabledEvent?.Invoke(state);
        }

        public void InvokeLoadingStateEvent(bool state)
        {
            LoadingStateEvent?.Invoke(state);
        }

        public void InvokeCompleteStateEvent(string parameter)
        {
            CompleteStateEvent?.Invoke(parameter);
        }
    }
}