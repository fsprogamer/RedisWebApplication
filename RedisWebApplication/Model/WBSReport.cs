using System.Collections.Generic;

namespace RedisWebApplication.Model
{
    public class WBSReport
    {
        public string status { get; set; }
        public List<ReportData> reportData { get; set; }
    }

    public class Label
    {
        public string name { get; set; }
    }

    public class Month
    {
        public int year { get; set; }
        public int month { get; set; }
        public double value { get; set; }
    }

    public class ReportData
    {
        public string id { get; set; }
        public int level { get; set; }
        public Label label { get; set; }
        public string type { get; set; }
        public double total { get; set; }
        public List<Month> months { get; set; }
        public bool inScope { get; set; }
    }
}
