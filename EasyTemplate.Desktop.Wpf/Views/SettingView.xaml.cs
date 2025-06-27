using EasyTemplate.Desktop.Wpf.ViewModels;

namespace EasyTemplate.Desktop.Wpf.Views
{
    public partial class SettingView : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public SettingView()
        {
            InitializeComponent();
            this.DataContext = new SettingViewModel(snackbar1);
        }

    }
}
