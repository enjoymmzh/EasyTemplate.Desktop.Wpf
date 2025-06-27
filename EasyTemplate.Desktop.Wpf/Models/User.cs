using System.Collections.Generic;

namespace EasyTemplate.Desktop.Wpf.Models;

public class Statistics
{
    public List<int> onlineUser { get; set; }
    public int totalGuest { get; set; }
    public int guestCart { get; set; }
    public int guestOrder { get; set; }
    public int completedOrder { get; set; }
    public int totalOrder { get; set; }
    public decimal totalAmount { get; set; }
}
