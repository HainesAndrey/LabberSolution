namespace LabberClient.VMStuff
{
    public delegate void ResponseHandler(ResponseType responseType, string msg);
    public delegate void PageEnabledHandler(bool state);
    public delegate void LoadingStateHandler(bool state);
    public delegate void CompleteStateHanlder(object parameter);

    public interface ILabberVM
    {
        event ResponseHandler ResponseEvent;
        event PageEnabledHandler PageEnabledEvent;
        event LoadingStateHandler LoadingStateEvent;
        event CompleteStateHanlder CompleteStateEvent;
    }
}