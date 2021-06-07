using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEditor.MainApp.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for NotificationDialog.xaml
    /// </summary>
    public partial class NotificationDialog : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NotificationDialog(string notification)
        {
            InitializeComponent();
            Notification = notification;
            OnPropertyChanged("Notification");
            unloadedStoryboard = FindResource("DialogUnloadedSb") as Storyboard;
        }

        public string Notification { get; set; }

        private readonly Storyboard unloadedStoryboard;

        private async void Close(object sender, RoutedEventArgs e)
        {
            unloadedStoryboard.Begin();
            await Task.Delay(unloadedStoryboard.Duration.TimeSpan);
            ((ContentControl)Parent).Content = null;
        }
    }
}
