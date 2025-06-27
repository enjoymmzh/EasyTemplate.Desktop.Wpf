using System;
using System.Net;
using System.Threading.Tasks;
using EasyTemplate.Desktop.Wpf.Models;
using RestSharp;

namespace EasyTemplate.Desktop.Wpf.Common;

public class ApiService
{
    public static async Task<string> GetShopifyCookieAsync(string host)
    {
        try
        {
            using var client = new RestClient();
            var request = new RestRequest(host, Method.Get);
            var response = await client.ExecuteAsync(request);
            if (response.Cookies?.Count > 0)
            {
                string value = "";
                foreach (Cookie cook in response.Cookies)
                {
                    value += cook.ToString() + ";";
                }
                return value;
            }
            return "";
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static ShopifyProduct GetShopifyProduct(string url, string cookie = "")
    {
        try
        {
            using var client = new RestClient();
            var request = new RestRequest(url, Method.Get);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                client.AddDefaultHeader("Cookie", cookie);
            }
            var response = client.Execute(request);
            return response.Content.ToModel<ShopifyProduct>();
        }
        catch (Exception)
        {
            return null;
        }
    }

}
