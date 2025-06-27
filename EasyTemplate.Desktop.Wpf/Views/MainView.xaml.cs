using System;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace EasyTemplate.Desktop.Wpf.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<Uri>(this, NewWindow);
        }


        private void NewWindow(object obj, Uri message)
        {
            tab1.SelectedIndex = 1;
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
