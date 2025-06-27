using System.Collections.Generic;
using System.Net;

namespace EasyTemplate.Desktop.Wpf.Models;

public class PageList<T>
{
    public List<T> data { get; set; }
    public int total { get; set; }
}

public class ResponseBody
{
    public HttpStatusCode code { get; set; }
    public bool success { get; set; }
    public string message { get; set; }
    public object data { get; set; }
    public object extras { get; set; }
    public long timestamp { get; set; }
}

public class ResponseBody<T>
{
    public HttpStatusCode code { get; set; }
    public bool success { get; set; }
    public string message { get; set; }
    public T data { get; set; }
    public object extras { get; set; }
    public long timestamp { get; set; }
}
