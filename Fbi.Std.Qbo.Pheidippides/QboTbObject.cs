using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbi.Std.Qbo.Pheidippides
{
    public class Option
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Header
    {
        public DateTime Time { get; set; }
        public string ReportName { get; set; }
        public string DateMacro { get; set; }
        public string ReportBasis { get; set; }
        public string StartPeriod { get; set; }
        public string EndPeriod { get; set; }
        public string Currency { get; set; }
        public List<Option> Option { get; set; }
    }

    public class Column
    {
        public string ColTitle { get; set; }
        public string ColType { get; set; }
    }

    public class Columns
    {
        public List<Column> Column { get; set; }
    }

    public class ColData
    {
        public string value { get; set; }
        public string id { get; set; }
    }

    public class ColData2
    {
        public string value { get; set; }
    }

    public class Summary
    {
        public List<ColData2> ColData { get; set; }
    }

    public class Row
    {
        public List<ColData> ColData { get; set; }
        public Summary Summary { get; set; }
        public string type { get; set; }
        public string group { get; set; }
    }

    public class Rows
    {
        public List<Row> Row { get; set; }
    }

    public class QboTbObject
    {
        public Header Header { get; set; }
        public Columns Columns { get; set; }
        public Rows Rows { get; set; }
    }
}
