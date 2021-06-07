using ImageEditor.MainApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageEditor.MainApp
{
    class RelayCommand<T> : ICommand
    {
        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            _canExecute = canExecute;
            _execute = execute ?? throw new ArgumentNullException("Missing execute parameter");
        }

        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }


    class RevertCommand : ICommand
    {
        public RevertCommand(Action initAction = null, Action completedAction = null)
        {
            this.initAction = initAction;
            this.completedAction = completedAction;
        }

        public readonly Action initAction;
        public readonly Action completedAction;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return ((ImageLayer)parameter) != null;
        }

        public async void Execute(object parameter)
        {
            initAction?.Invoke();
            ImageLayer layer = (ImageLayer)parameter;
            if (initAction != null && completedAction != null)
                await Task.Run(() => layer.RenderedBitmap = new Bitmap(layer.OriginalBitmap));
            else
                layer.RenderedBitmap = new Bitmap(layer.OriginalBitmap);
            completedAction?.Invoke();
        }
    }


    class PreviewCommand : ICommand
    {
        public PreviewCommand(Func<bool> canExecute, Func<Bitmap, Bitmap> execute, Action initAction = null, Action completedAction = null)
        {
            this.canExecute = canExecute;
            this.execute = execute;
            this.initAction = initAction;
            this.completedAction = completedAction;
        }

        private readonly Func<bool> canExecute;
        private readonly Func<Bitmap, Bitmap> execute;

        public readonly Action initAction;
        public readonly Action completedAction;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return ((ImageLayer)parameter) != null && canExecute.Invoke();
        }

        public async void Execute(object parameter)
        {
            initAction?.Invoke();
            ImageLayer layer = (ImageLayer)parameter;
            Bitmap result;
            if (initAction != null && completedAction != null)
                result = await Task.Run(() => execute.Invoke(layer.OriginalBitmap));
            else
                result = execute.Invoke(layer.OriginalBitmap);
            layer.RenderedBitmap = result;
            completedAction?.Invoke();
        }
    }


    class ApplyCommand : ICommand
    {
        public ApplyCommand(Func<bool> canExecute, Func<Bitmap, Bitmap> execute, Action initAction = null, Action completedAction = null)
        {
            this.canExecute = canExecute;
            this.execute = execute;
            this.initAction = initAction;
            this.completedAction = completedAction;
        }

        private readonly Func<bool> canExecute;
        private readonly Func<Bitmap, Bitmap> execute;

        public readonly Action initAction;
        public readonly Action completedAction;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return ((ImageLayer)parameter) != null && canExecute.Invoke();
        }

        public async void Execute(object parameter)
        {
            initAction?.Invoke();
            ImageLayer layer = (ImageLayer)parameter;
            Bitmap result;
            if (initAction != null && completedAction != null)
                result = await Task.Run(() => execute.Invoke(layer.OriginalBitmap));
            else
                result = execute.Invoke(layer.OriginalBitmap);
            layer.RenderedBitmap = result;
            layer.OriginalBitmap = new Bitmap(result);
            completedAction?.Invoke();
        }
    }
}
