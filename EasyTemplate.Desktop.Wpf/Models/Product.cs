using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EasyTemplate.Desktop.Wpf.Models;

public partial class H5Product : ViewModelBase
{
    [ObservableProperty] public int id;
    [ObservableProperty] public string name;
    [ObservableProperty] public string link;
    [ObservableProperty] public string json;
    [ObservableProperty] public string reason;
    [ObservableProperty] public H5ProductStatus status = H5ProductStatus.NONE;
    [ObservableProperty] public string description = "未上傳";
    [ObservableProperty] public bool isSelected;
    [ObservableProperty] public string cookie;
    public object data { get; set; }
}

public enum H5ProductStatus
{
    NONE,
    UPLOADING,
    DONE,
    FAIL
}

#region ShopifyProduct

public class ShopifyProduct
{
    public ShopifyProductInfo product { get; set; }
}

public class ShopifyProductInfo
{
    public long id { get; set; }
    public string title { get; set; }
    public string body_html { get; set; }
    public string vendor { get; set; }
    public string product_type { get; set; }
    public DateTime created_at { get; set; }
    public string handle { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime published_at { get; set; }
    public string template_suffix { get; set; }
    public string published_scope { get; set; }
    public string tags { get; set; }
    public List<ShopifyVariant> variants { get; set; }
    public List<ShopifyOption> options { get; set; }
    public List<ShopifyImage> images { get; set; }
    public ShopifyImage image { get; set; }
}

public class ShopifyImage
{
    public long id { get; set; }
    public long product_id { get; set; }
    public int position { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string alt { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string src { get; set; }
    public long[] variant_ids { get; set; }
}

public class ShopifyVariant
{
    public long id { get; set; }
    public long product_id { get; set; }
    public string title { get; set; }
    public string price { get; set; }
    public string sku { get; set; }
    public int position { get; set; }
    public string compare_at_price { get; set; }
    public string fulfillment_service { get; set; }
    public string inventory_management { get; set; }
    public string option1 { get; set; }
    public object option2 { get; set; }
    public object option3 { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public bool taxable { get; set; }
    public string barcode { get; set; }
    public int grams { get; set; }
    public string image_id { get; set; }
    public decimal weight { get; set; }
    public string weight_unit { get; set; }
    public bool requires_shipping { get; set; }
}

public class ShopifyOption
{
    public long id { get; set; }
    public long product_id { get; set; }
    public string name { get; set; }
    public int position { get; set; }
    public List<string> values { get; set; }
}

#endregion
