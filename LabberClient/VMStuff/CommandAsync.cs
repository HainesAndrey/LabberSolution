using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Threading.Tasks;

namespace LabberClient.VMStuff
{
    public class CommandAsync : RelayCommand
    {
        public CommandAsync(Action asyncAction) : base(asyncAction) { }

        public override async void Execute(object parameter)
        {
            await Task.Run(() => base.Execute(parameter));
        }
    }

    public class CommandAsync<T> : RelayCommand<T>
    {
        public CommandAsync(Action<T> asyncAction) : base(asyncAction) { }

        public override async void Execute(object parameter)
        {
            await Task.Run(() => base.Execute(parameter));
        }
    }
}