using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows;

namespace ZPF.WPF
{
   public class DataGridToolsColumn : BaseViewModel
   {
      public DataGridToolsColumn()
      {
         //Width = DataGridLength.Auto;
         //Visibility = Visibility.Visible;
         Width = -1;
         Visibility = true;
         SortDirection = null;
      }

      public string Header { get; set; }
      public string ColumnName { get; set; }

      //public DataGridLength Width { get; set; }
      public double Width { get; set; }
      public int DisplayIndex { get; set; }
      //public Visibility Visibility { get; set; }
      public bool Visibility { get; set; }
      public ListSortDirection? SortDirection { get; set; }


      string _Operation = "";

      [JsonIgnore]
      public string Operation { get => _Operation; set => SetField(ref _Operation, value); }

      string _ColumnData = "";

      [JsonIgnore]
      public string ColumnData
      {
         get => _ColumnData;
         set => SetField(ref _ColumnData, value);
      }
   }
}
