using LabberLib.DataBaseContext;

namespace LabberClient.VMStuff
{
    public abstract class LabberVMBase : VMBase, ILabberVM
    {
        public event ResponseHandler ResponseEvent;
        public event PageEnabledHandler PageEnabledEvent;
        public event LoadingStateHandler LoadingStateEvent;
        public event CompleteStateHanlder CompleteStateEvent;

        protected DBWorker db;

        public LabberVMBase(uint userId, string dbconnectionstring, ResponseHandler ResponseEvent, PageEnabledHandler PageEnabledEvent, LoadingStateHandler LoadingStateEvent, CompleteStateHanlder CompleteStateEvent)
        {
            this.ResponseEvent = ResponseEvent;
            this.PageEnabledEvent = PageEnabledEvent;
            this.LoadingStateEvent = LoadingStateEvent;
            this.CompleteStateEvent = CompleteStateEvent;
            db = new DBWorker(dbconnectionstring);
            db.UserId = userId;
        }

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

        public void InvokeCompleteStateEvent(object parameter)
        {
            CompleteStateEvent?.Invoke(parameter);
        }
    }
}