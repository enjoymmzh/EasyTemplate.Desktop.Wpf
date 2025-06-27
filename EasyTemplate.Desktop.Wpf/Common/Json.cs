using System;
using Newtonsoft.Json;

namespace EasyTemplate.Desktop.Wpf.Common;

public static class Json
{
    public static string ToJson(this object model)
    {
        return JsonConvert.SerializeObject(model);
    }

    public static T ToModel<T>(this string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception)
        {
            return default;
        }
    }
}
