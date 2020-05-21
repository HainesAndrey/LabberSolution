using MvvmCross.Commands;
using System;
using System.Threading.Tasks;

namespace LabberClient.VMStuff
{
    public class CommandAsync : MvxCommand
    {
        public CommandAsync(Action asyncAction) : base(asyncAction) { }

        public new async void Execute(object parameter)
        {
            await Task.Run(() => base.Execute(parameter));
        }
    }

    public class CommandAsync<T> : MvxCommand<T>
    {
        public CommandAsync(Action<T> asyncAction) : base(asyncAction) { }

        public new async void Execute(object parameter)
        {
            await Task.Run(() => base.Execute(parameter));
        }
    }
}