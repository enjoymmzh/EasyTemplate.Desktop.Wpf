using EasyTemplate.Desktop.Wpf.Models;
using EasyTemplate.Desktop.Wpf.ViewModels;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using System;
using EasyTemplate.Desktop.Wpf.Common;
using System.Threading.Tasks;

namespace EasyTemplate.Desktop.Wpf.Views
{
    public partial class ShopifyView : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ShopifyView()
        {
            InitializeComponent();
            this.DataContext = new ShopifyViewModel(browser, tab, datagrid1, datagrid2, combo1, combo2, snackbar1);
        }

        private void browser_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            var context = this.DataContext as ShopifyViewModel;
            context.PageLoading = System.Windows.Visibility.Visible;
        }

        private void browser_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            var context = this.DataContext as ShopifyViewModel;
            context.PageLoading = System.Windows.Visibility.Collapsed;
        }

        private void browser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;

            var uri = new Uri(e.Uri);
            var context = this.DataContext as ShopifyViewModel;
            context.Url = uri.ToString();
            context.Addr = $"獨立站商城({uri.Host})";
            browser.CoreWebView2.Navigate(e.Uri);
        }

        private void browser_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
        {
            var context = this.DataContext as ShopifyViewModel;
            context.Url = browser.Source.ToString();
            context.Addr = $"獨立站商城({browser.Source.Host})";
        }

        private void txtUrl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is System.Windows.Input.Key.Enter)
            {
                tab.SelectedIndex = 0;
                browser.CoreWebView2.Navigate(txtUrl.Text);
            }
        }

        private void copyBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                var item = btn.DataContext as H5Product;
                if (item != null)
                {
                    Clipboard.SetText(item.Link);
                }
            }
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn != null)
            {
                var item = btn.DataContext as H5Product;
                if (item != null)
                {
                    item.Cookie = await ApiService.GetShopifyCookieAsync(item.Link);
                    if (string.IsNullOrWhiteSpace(item.Cookie))
                    {
                        ShowTip("获取失败");
                    }
                    else
                    {
                        ShowTip("获取成功");
                    }
                }
            }
        }

        private void ShowTip(string message)
        {
            if (snackbar1.MessageQueue is { } messageQueue)
            {
                Task.Factory.StartNew(() => messageQueue.Enqueue(message));
            }
        }
    }
}
