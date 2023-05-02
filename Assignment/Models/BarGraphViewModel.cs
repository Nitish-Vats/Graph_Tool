using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;

namespace Assignment.Models
{
    public class BarGraphViewModel
    {
        public Dictionary<string,string> TopFive { get; set; }
        public Dictionary<string, string> BottomFive { get; set; }
    }
}