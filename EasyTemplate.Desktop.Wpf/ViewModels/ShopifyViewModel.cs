using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HtmlAgilityPack;
using EasyTemplate.Desktop.Wpf.Models;
using Masuit.Tools;
using System.Linq;
using Microsoft.Web.WebView2.Wpf;
using System.Text.RegularExpressions;
using System.Net;
using EasyTemplate.Desktop.Wpf.Common;
using System.Threading.Tasks;
using System.IO;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace EasyTemplate.Desktop.Wpf.ViewModels;

public partial class ShopifyViewModel : ViewModelBase
{
    public ShopifyViewModel(WebView2 browser, TabControl tab, DataGrid datagrid, DataGrid websitegrid, ComboBox statuscombo, ComboBox hostcombo, Snackbar snackbar)
    {
        //Url = "https://www.my9.com.tw/collections/all";
        //Url = "https://buddhaandkarma.com/collections/all";
        Url = "https://rankly.cn/top_shopify";
        PageLoading = Visibility.Collapsed;
        Uploading = Visibility.Collapsed;
        _browser = browser;
        _datagrid = datagrid;
        
        _websitegrid = websitegrid;
        _snackbar = snackbar;
        _statuscombo = statuscombo;
        _statuscombo.SelectionChanged += _statuscombo_SelectionChanged;
        _hostcombo = hostcombo;
        _hostcombo.SelectionChanged += _hostcombo_SelectionChanged;
        _tab = tab;

        Global.productPath = $"{AppDomain.CurrentDomain.BaseDirectory}products\\";
        if (!Directory.Exists(Global.productPath))
        {
            Directory.CreateDirectory(Global.productPath);
        }

        if (File.Exists($"{Global.productPath}product.json"))
        {
            Load();
        }
        else
        {
            Statistics(Products.ToList());
        }
        _datagrid.ItemsSource = Products;

        Websites = new List<H5Product>();
        if (File.Exists($"{Global.productPath}website.json"))
        {
            var websites = File.ReadAllText($"{Global.productPath}website.json");
            Websites = websites.ToModel<List<H5Product>>();
        }

        Hosts.Add("全部");
        Hosts.AddRange(Websites.Select(s => s.Name).ToList());

        ComboEnable = true;
        Addr = $"獨立站商城";
    }

    private void ShowTip(string message)
    {
        if (_snackbar.MessageQueue is { } messageQueue)
        {
            Task.Factory.StartNew(() => messageQueue.Enqueue(message));
        }
    }

    #region 命令

    /// <summary>
    /// 页面跳转
    /// </summary>
    [RelayCommand]
    private void Jump()
    {
        if (_browser?.CoreWebView2 != null)
        {
            if (Validate.CheckIsUrlFormat(Url))
            {
                _browser.CoreWebView2.Navigate(Url);
                return;
            }
            this.ShowTip("請輸入合理的網路鏈接");
        }
    }

    /// <summary>
    /// 刪除
    /// </summary>
    [RelayCommand]
    private void Delete()
    {
        //_datagrid.BeginEdit();
        //var p = _datagrid.ItemsSource as List<H5Product>;
        var list = GetHandleList();
        foreach (var item in list)
        {
            //p.Remove(item);
            Products.Remove(item);
            //_datagrid.Items.Remove(item);
        }
       // _datagrid.CommitEdit();
    }

    /// <summary>
    /// 下载
    /// </summary>
    [RelayCommand]
    private async void Download()
    {
        Downloading = Visibility.Visible;
        AddInfo = string.Empty;

        try
        {
            if (!Websites.Any(x => x.Name.Contains(_browser.Source.Host)))
            {
                //website.Cookie = ApiService.GetShopifyCookie($"{uri.Scheme}://{uri.Host}");
                var cookie = await ApiService.GetShopifyCookieAsync(_browser.Source.Host);
                var model = new H5Product() { Name = _browser.Source.Host, Link = _browser.Source.AbsoluteUri.TrimEnd('/'), Cookie = cookie };
                Websites.Add(model);
                File.WriteAllText($"{Global.productPath}website.json", Websites.ToJson());
                _websitegrid.Items.Refresh();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }

        string html = await _browser.ExecuteScriptAsync("document.documentElement.outerHTML");
        html = Regex.Unescape(html);
        html = html.Remove(0, 1);
        html = html.Remove(html.Length - 1, 1);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var total = 0;
        var exists = Products.Select(s => s.Name).ToList();
        var nodes = doc.DocumentNode.SelectNodes("//a/@href");
        var list = nodes?.Where(w => w.Attributes["href"] is not null && (w.Attributes["href"].Value.Contains("/products/"))).ToList();
        if (list?.Count > 0)
        {
            _datagrid.BeginEdit();
            foreach (var node in list)
            {
                var route = node.Attributes["href"].Value;
                if (route.Contains("http://") || route.Contains("https://"))
                {
                    var uri = new Uri(route);
                    route = uri.AbsolutePath;
                }
                var names = route.Split("/");
                var name = names[names.Length - 1];
                if (!exists.Contains(name) && !Products.Any(a => a.Name.Contains(name)))
                {
                    var link = $"{_browser.Source.Scheme}://{_browser.Source.Host}{route}.json";
                    var model = new H5Product() { Name = name, Link = link, IsSelected = true };
                    model.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(H5Product.IsSelected))
                            OnPropertyChanged(nameof(SelectAll));
                    };
                    Products.Add(model);
                    total++;
                }
            }
            _datagrid.CommitEdit();
            _datagrid.Items.Refresh();
            UpdateGrid();
        }
        else
        {
            Statistics(Products.ToList());
        }

        SelectAll = true;
        //_tab.SelectedIndex = Products.Count > 0 ? 1 : 0;
        AddInfo = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} 新增 {total} 條產品數據";
        Downloading = Visibility.Hidden;
    }

    [RelayCommand]
    private void Reset()
    {
        if (Products?.Count > 0)
        {
            Products.ForEach(item => {
                if (item.Status is H5ProductStatus.FAIL)
                {
                    item.Status = H5ProductStatus.NONE;
                    item.Description = "未上傳";
                    item.Reason = "";
                }
            });
        }
    }

    [RelayCommand]
    private async void RefreshCookie()
    {
        var item = _websitegrid.SelectedItem as H5Product;
        item.Cookie = await ApiService.GetShopifyCookieAsync(item.Link);
    }

    /// <summary>
    /// 保存历史记录
    /// </summary>
    [RelayCommand]
    private async void Upload(ComboBox combo)
    {
        if (_datagrid is null) return;

        ComboEnable = false;
        CancelUpload = false;
        Uploading = Visibility.Visible;

        var res = await TheUpload();
        if (res)
        {
            ShowTip("上傳完成");
            Save();
        }

        ComboEnable = true;
        CancelUpload = true;
        Uploading = Visibility.Collapsed;
    }

    private async Task<bool> TheUpload()
    {
        var list = GetHandleList();
        return await Task.Run(() => {
            try
            {
                foreach (var item in list)
                {
                    if (CancelUpload) break;
                    if (!item.IsSelected || item.Status != H5ProductStatus.NONE) { continue; }

                    var start = DateTime.Now;
                    item.Description = "上傳中";

                    var uri = new Uri(item.Link);
                    var website = Websites.Where(s => s.Name.Contains(uri.Host)).First();

                    item.Reason = "下載產品數據";
                    var info = ApiService.GetShopifyProduct(item.Link, website.Cookie);
                    if (info is not null)
                    {
                        var exist = false;// ApiService.IsProductExist(info.product.title);
                        if (!exist)
                        {
                            item.Status = H5ProductStatus.DONE;
                            item.Description = "成功";
                            item.Reason = $"上傳完成，耗時 {DateTime.Now.Subtract(start).TotalSeconds} 秒";
                            Statistics(list);
                        }
                        else
                        {
                            item.Status = H5ProductStatus.FAIL;
                            item.Description = "失敗";
                            item.Reason = "產品數據已存在";
                        }
                    }
                    else
                    {
                        item.Status = H5ProductStatus.FAIL;
                        item.Description = "失敗";
                        item.Reason = "下載數據時失敗";
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        });
    }

    private List<H5Product> GetHandleList()
    {
        if (Products.Count > 0)
        {
            var list = Products
                .WhereIf(_statuscombo.SelectedIndex > 0, w => w.Status == GetStatus())
                .WhereIf(_hostcombo.SelectedIndex > 0, w => w.Link.Contains(_hostcombo.SelectedValue.ToString()))
                .WhereIf(!string.IsNullOrWhiteSpace(SearchKeyword), w => w.Link.Contains(SearchKeyword) || w.Name.Contains(SearchKeyword))
                .Where(w => w.IsSelected)
                .ToList();
            Statistics(list);
            return list;
        }
        return Products.ToList();
    }

    private H5ProductStatus GetStatus()
    {
        H5ProductStatus status;
        switch (_statuscombo.SelectedIndex)
        {

            case 1: status = H5ProductStatus.DONE; break;
            case 2: status = H5ProductStatus.NONE; break;
            case 3: status = H5ProductStatus.FAIL; break;
            default: return H5ProductStatus.NONE;
        }
        return status;
    }

    private void Statistics(List<H5Product> list)
    {
        var success = list?.Count(s => s.Status is H5ProductStatus.DONE);
        var fail = list?.Count(s => s.Status is H5ProductStatus.FAIL);
        Info = $"共 {list?.Count} 個產品數據，已上傳 {success}，失敗 {fail}，未處理 {list?.Count - success - fail}。";
    }

    private void _hostcombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateGrid();

    private void _statuscombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateGrid();

    private void UpdateGrid()
    {
        var cvs = CollectionViewSource.GetDefaultView(_datagrid.ItemsSource);
        if (cvs != null && cvs.CanFilter)
        {
            cvs.Filter = OnFilterApplied;
        }
        Statistics(GetHandleList());
    }

    private bool OnFilterApplied(object obj)
    {
        if (obj is H5Product item)
        {
            var flag1 = true;
            if (_statuscombo.SelectedIndex > 0 )
            {
                var status = H5ProductStatus.NONE;
                switch (_statuscombo.SelectedIndex)
                {
                    case 1: status = H5ProductStatus.DONE; break;
                    case 2: status = H5ProductStatus.NONE; break;
                    case 3: status = H5ProductStatus.FAIL; break;
                }
                flag1 = item.Status == status;
            }

            var flag2 = true;
            if (_hostcombo.SelectedIndex > 0)
            {
                flag2 = item.Link.Contains(_hostcombo.SelectedValue?.ToString());
            }

            var flag3 = true;
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                flag3 = item.Link.ToLower().Contains(searchKeyword!.ToLower()) || item.Name.ToLower().Contains(searchKeyword!.ToLower());
            }

            var res =  flag1 && flag2 && flag3;
            return res;
        }
        return false;
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelUpload = true;
        ShowTip("將會在當前項上傳完成後結束");
    }

    [RelayCommand]
    private void Save()
    {
        if (Products.Count.Equals(0))
        {
            return;
        }

        File.WriteAllText($"{Global.productPath}product.json", Products.ToJson());
        File.WriteAllText($"{Global.productPath}website.json", Websites.ToJson());
        ShowTip("保存成功");
    }

    [RelayCommand]
    private void Load()
    {
        if (Products?.Count > 0)
        {
            WeakReferenceMessenger.Default.Send(new TheMessage() { message = "將覆蓋當前數據" });
        }
        _statuscombo.SelectedIndex = 0;
        _hostcombo.SelectedIndex = 0;
        var productsjson = File.ReadAllText($"{Global.productPath}product.json");
        var list = productsjson.ToModel<List<H5Product>>();
        list.ForEach(item => {
            item.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(H5Product.IsSelected))
                    OnPropertyChanged(nameof(SelectAll));
            };
        });
        Products = new ObservableCollection<H5Product>(list);
        //_datagrid.Items.Refresh();
        Statistics(Products.ToList());
        ShowTip("加載完成");
    }

    public string GetBase64FromWeb(string url)
    {
        using var client = new WebClient();
        var bytes = client.DownloadData(url);
        var base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    public byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);// 设置当前流的位置为流的开始 
        return bytes;
    }

    /// <summary>
    /// 浏览器-后退
    /// </summary>
    [RelayCommand]
    private void Back()
    {
        if (_browser.CoreWebView2 != null && _browser.CanGoBack)
        {
            _browser.GoBack();
        }
    }

    /// <summary>
    /// 浏览器-前进
    /// </summary>
    [RelayCommand]
    private void Forward()
    {
        if (_browser.CoreWebView2 != null && _browser.CanGoForward)
        {
            _browser.GoForward();
        }
    }

    /// <summary>
    /// 浏览器-刷新页面
    /// </summary>
    [RelayCommand]
    private void RefreshPage()
    {
        if (_browser.CoreWebView2 != null)
        {
            _browser.CoreWebView2.Reload();
        }
    }

    /// <summary>
    /// 浏览器-主页
    /// </summary>
    [RelayCommand]
    private void HomePage()
    {
        if (_browser.CoreWebView2 != null)
        {
            _browser.CoreWebView2.Navigate("https://rankly.cn/top_shopify");
        }
    }

    /// <summary>
    /// 浏览器-控制台
    /// </summary>
    [RelayCommand]
    private void ConsoleTool()
    {
        if (_browser.CoreWebView2 != null)
        {
            _browser.CoreWebView2.OpenDevToolsWindow();
        }
    }

    #endregion

    #region 字段

    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] private string addInfo;
    /// <summary>
    /// 网页链接
    /// </summary>
    [ObservableProperty] private string url;
    /// <summary>
    /// 下载进度
    /// </summary>
    [ObservableProperty] private string progress;
    /// <summary>
    /// 页面加载
    /// </summary>
    [ObservableProperty] private Visibility pageLoading;
    /// <summary>
    /// 上傳中
    /// </summary>
    [ObservableProperty] private Visibility uploading;
    /// <summary>
    /// 上傳中
    /// </summary>
    [ObservableProperty] private Visibility downloading;
    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] private bool cancelUpload;
    /// <summary>
    /// 识别出的产品清单
    /// </summary>
    public ObservableCollection<H5Product> Products = new ObservableCollection<H5Product>();
    /// <summary>
    /// 已採集站點
    /// </summary>
    [ObservableProperty] public List<H5Product> websites;
    /// <summary>
    /// 说明
    /// </summary>
    [ObservableProperty] public string info;
    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] public bool comboEnable;
    /// <summary>
    /// 
    /// </summary>
    private WebView2 _browser { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private Snackbar _snackbar { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private TabControl _tab { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private DataGrid _websitegrid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private DataGrid _datagrid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private ComboBox _statuscombo { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private ComboBox _hostcombo { get; set; }
    /// <summary>
    /// 
    /// </summary>a
    [ObservableProperty] private string addr;
    /// <summary>
    /// 页面加载
    /// </summary>
    private bool selectAll;
    public bool SelectAll
    {
        get
        {
            var selected = Products.Select(item => item.IsSelected).Distinct().ToList();
            var res = selected.Count == 1 ? selected.Single() : false;
            return res;
        }
        set
        {
            selectAll = value;
            SelectAllItem(value, Products);
            OnPropertyChanged();
            SetProperty(ref selectAll, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="select"></param>
    /// <param name="models"></param>
    private void SelectAllItem(bool select, IEnumerable<H5Product> models)
    {
        foreach (var model in models)
        {
            model.IsSelected = select;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] public List<string> uploadStatus = new List<string>() { "全部", "已上傳", "未上傳", "已失敗" };
    /// <summary>
    /// 
    /// </summary>
    [ObservableProperty] public List<string> hosts = new List<string>();
    /// <summary>
    /// 
    /// </summary>
    private string searchKeyword;
    public string SearchKeyword
    {
        get { return searchKeyword; }
        set 
        { 
            searchKeyword = value; 
            OnPropertyChanged();
            UpdateGrid();
        }
    }
    #endregion
}
