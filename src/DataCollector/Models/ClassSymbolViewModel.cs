using System.Collections.Generic;

namespace DataCollector.Models
{
    public class ClassSymbolViewModel
    {
        public ClassSymbolViewModel()
        {
            UniqueItems = new List<UniqueSymbolItem>();
        }
        public int ClassId { get; set; }
        public double? MaxScale { get; set; }
        public bool HasFlow { get; set; }
        public string ClassType { get; set; }

        public SymbolViewModel Symbol { get; set; }

        public string UniqueField { get; set; }

        public List<UniqueSymbolItem> UniqueItems { get; set; }

    }
    public class SymbolViewModel
    {
        public string FillColor { get; set; }
        public string StrokeColor  { get; set; }
        public double Width { get; set; }
        public double StrokeWidth { get; set; }
    }

    public class UniqueSymbolItem
    {
        public string Value { get; set; }
        public SymbolViewModel Symbol { get; set; }
    }
}