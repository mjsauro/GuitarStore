using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitarStore.Areas.Reports.Models
{
    public class SalesReportModel
    {
        public string SelectedState { get; set; }
        public string[] States { get; set; }

        public TopSaleByDollar[] TopSalesByDollar { get; set; }
        public TopSaleByQuantity[] TopSalesByQuantity { get; set; }

    }

    public class TopSaleByQuantity
    {
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public class TopSaleByDollar
    {
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public int Price { get; set; }
    }
}