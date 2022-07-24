using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZPF;
using ZPF.SQL;
using ZPF.WPF.Compos;

public class EditReferentielViewModel : BaseViewModel
{
   public EditReferentielViewModel()
   {
      _Instance = this;

      _Instance.CheckRights = true;
      _Instance.Tables = new List<NameValue>();
   }

   public void Dispose()
   {
      _Instance = null;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static EditReferentielViewModel _Instance = null;

   public static EditReferentielViewModel Current
   {
      get
      {
         return _Instance ?? new EditReferentielViewModel();
      }

      set
      {
         _Instance = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   StackPanel _StackPanelDetails = null;
   DataGrid _DataGridReferentiel = null;

   public void Init(StackPanel sdDetails, DataGrid dgReferentiel)
   {
      _StackPanelDetails = sdDetails;
      _DataGridReferentiel = dgReferentiel;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public enum ActionType { Insert, Update, Delete }
   /// <summary>
   /// bool OnAction(ActionType actionType, string TableName, DataRow selectedRecord)
   /// </summary>
   public Func<ActionType, string, DataRow, bool> OnAction { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   public List<NameValue> Tables
   {
      get;
      set;
   }
   public DataView TableDataView { get; set; }

   TStrings Fields = null;

   public void UpdateDataGrid(DataGrid dataGrid)
   {
      if (SelectedTable == null)
      {
         return;
      };

      Fields = Tables.Where(x => x.Value == SelectedTable.Value).Select(x => x).FirstOrDefault().Tag as TStrings;

      if (Fields == null)
      {
         if (Debugger.IsAttached)
         {
            Debugger.Break();
         };

         return;
      };

      foreach (var c in dataGrid.Columns)
      {
         if (Fields[c.SortMemberPath] != "")
         {
            if (Fields[c.SortMemberPath].StartsWith("_"))
            {
               c.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
               c.Header = Fields[c.SortMemberPath].Split(new char[] { ',' })[0];
            }
         }
         else
         {
            c.Visibility = System.Windows.Visibility.Hidden;
         };


         // var col = Columns.Where(x => x.

         //if( c.Caption != "")
         //   Field field = ReportEngine.Current.CurrentReport.Fields.Where(x => x.FieldName == c.SortMemberPath).Select(x => x).FirstOrDefault();

         //if (field != null)
         //{
         //   c.Visibility = System.Windows.Visibility.Visible;
         //   c.Header = field.HeaderName;

         //   if (c is DataGridTextColumn)
         //   {
         //      Style columnStyle = new Style(typeof(TextBlock));

         //      switch (field.Alignment)
         //      {
         //         case ALIGN.LEFT: columnStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left)); break;
         //         case ALIGN.CENTER: columnStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center)); break;
         //         case ALIGN.RIGHT: columnStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right)); break;
         //      };

         //      (c as DataGridTextColumn).ElementStyle = columnStyle;
         //   };
         //}
         //else
         //{
         //   try
         //   {
         //      c.Visibility = System.Windows.Visibility.Hidden;
         //   }
         //   catch
         //   {
         //   };
         //};
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   Action<string> _OnChanges = null;

   public Action<string> OnChanges
   {
      get { return _OnChanges; }
      set { _OnChanges = value; }
   }

   public void ReferentielLoadData()
   {
      if (OnChanges != null)
      {
         OnChanges("Update");
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   NameValue _SelectedTable = null;
   public NameValue SelectedTable
   {
      get
      {
         return _SelectedTable;
      }

      set
      {
         if (_SelectedTable != value)
         {
            _SelectedTable = value;
            SelectedRecord = null;

            LoadTable();

            OnPropertyChanged();

            if (_StackPanelDetails != null)
            {
               DisplayRecord(_StackPanelDetails);
            };

            if (_DataGridReferentiel != null)
            {
               UpdateDataGrid(_DataGridReferentiel);
            };
         };
      }
   }

   // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

   public void DisplayRecord(StackPanel sdDetails)
   {
      sdDetails.Children.Clear();

      if (SelectedRecord == null) return;

      var Columns = SelectedRecord.Table.Columns;
      var ItemArray = SelectedRecord.ItemArray;

      for (int i = 0; i < Columns.Count; i++)
      {
         var item = ItemArray[i];

         TextBoxEx.FilterType _Filter = TextBoxEx.FilterType.None;

         switch (Columns[i].DataType.ToString())
         {
            case "System.Boolean":
            case "System.UInt32":
            case "System.UInt64":
            case "System.Int32":
            case "System.Int64":
               _Filter = TextBoxEx.FilterType.Int; break;

            case "System.Double":
            case "System.Decimal":
            case "System.Float":
               _Filter = TextBoxEx.FilterType.Float; break;

            case "System.String":
               _Filter = TextBoxEx.FilterType.None; break;

            case "System.DateTime":
               _Filter = TextBoxEx.FilterType.Date; break;

            default:
               Debug.WriteLine(item.GetType().ToString());

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
               break;
         };

         int nbRows = 1;
         double maxWidth = 500;
         double indent = 140;
         double width = indent + 100;
         double height = 24;
         VerticalAlignment verticalcontentalignment = VerticalAlignment.Center;

         if (Columns[i].MaxLength > 0)
         {
            width = indent + Columns[i].MaxLength * 7;
         };

         nbRows = (int)(width / maxWidth);
         nbRows = (nbRows < 1 ? 1 : nbRows);
         nbRows = (nbRows > 5 ? 5 : nbRows);

         if ((nbRows > 1) && (_Filter == TextBoxEx.FilterType.None))
         {
            _Filter = TextBoxEx.FilterType.Memo;
            verticalcontentalignment = VerticalAlignment.Top;
         };

         height = nbRows * height;

         width = (width > maxWidth ? maxWidth : width);
         width = (width < indent + 15 ? indent + 15 : width);

         string _Label = Columns[i].Caption;

         if (Fields != null && _Label != "")
         {
            if (Fields.GetObject(Fields.IndexOfName(Columns[i].ColumnName)) == null)
            {
               if (Columns[i].DataType.ToString() == "System.Boolean")
               {
                  sdDetails.Children.Add(new CheckBoxEx()
                  {
                     Label = _Label,
                     IsChecked = item.ToString() == "True",
                     Indent = indent,
                     Width = width,
                     Height = height,
                     Tag = i,
                     IsEnabled = !Columns[i].AutoIncrement,
                     Margin = new Thickness(0, 0, 0, 4),
                     VerticalContentAlignment = verticalcontentalignment,
                     HorizontalAlignment = HorizontalAlignment.Left,
                  });
               }
               else
               {
                  //ToDo: IsEnabled SQLite
                  sdDetails.Children.Add(new TextBoxEx()
                  {
                     Label = _Label,
                     Text = item.ToString().TrimEnd(),
                     Indent = indent,
                     Width = width,
                     Height = height,
                     Filter = _Filter,
                     Tag = i,
                     IsEnabled = !Columns[i].AutoIncrement && !(Columns[i].ColumnName == "PK" && Columns[i].Unique && !Columns[i].AllowDBNull),
                     MaxLength = Columns[i].MaxLength,
                     Margin = new Thickness(0, 0, 0, 4),
                     VerticalContentAlignment = verticalcontentalignment,
                     HorizontalAlignment = HorizontalAlignment.Left,
                  });
               }
            }
            else
            {
               if (Fields.GetObject(Fields.IndexOfName(Columns[i].ColumnName)) is TStrings)
               {
                  TStrings items = (Fields.GetObject(Fields.IndexOfName(Columns[i].ColumnName)) as TStrings);
                  List<NameValue> cItems = new List<NameValue>();

                  NameValue SelectedItem = null;

                  for (int ind = 0; ind < items.Count; ind++)
                  {
                     cItems.Add(new NameValue() { Name = items.Names(ind), Value = items.ValueFromIndex(ind) });

                     if (items.ValueFromIndex(ind) == item.ToString().TrimEnd())
                     {
                        SelectedItem = cItems.Last();
                     };
                  };

                  var cb = new ComboBoxEx()
                  {
                     Label = _Label,
                     Indent = indent,
                     Width = width + 50,
                     Height = height,
                     Tag = i,
                     IsEnabled = !Columns[i].AutoIncrement,
                     Margin = new Thickness(0, 0, 0, 4),
                     VerticalContentAlignment = verticalcontentalignment,
                     ItemsSource = cItems,
                     DisplayMemberPath = "Name",
                     SelectedValuePath = "Value",
                     HorizontalAlignment = HorizontalAlignment.Left,
                  };

                  cb.ComboBox.SelectedItem = SelectedItem;

                  sdDetails.Children.Add(cb);
               };
            };
         };
      };
   }

   // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

   public void UpdateRecord(StackPanel sdDetails)
   {
      var Columns = SelectedRecord.Table.Columns;
      var ItemArray = SelectedRecord.ItemArray;

      foreach (var c in sdDetails.Children)
      {
         if (c is TextBoxEx)
         {
            TextBoxEx tb = c as TextBoxEx;

            int i = (int)tb.Tag;
            var item = ItemArray[i];

            if (!Columns[i].AutoIncrement)
            {
               SelectedRecord[i] = SetValue(Columns[i], tb.Text);
            }
         };

         if (c is CheckBoxEx)
         {
            CheckBoxEx cb = c as CheckBoxEx;

            int i = (int)cb.Tag;
            var item = ItemArray[i];

            if (!Columns[i].AutoIncrement)
            {
               SelectedRecord[i] = SetValue(Columns[i], (cb.IsChecked == true ? "1" : "0"));
            }
         };

         if (c is ComboBoxEx)
         {
            ComboBoxEx cb = (c as ComboBoxEx);

            int i = (int)cb.Tag;
            var item = ItemArray[i];

            if (cb.SelectedValue == null)
            {
               SelectedRecord[i] = DBNull.Value;
            }
            else
            {
               SelectedRecord[i] = SetValue(Columns[i], cb.SelectedValue.ToString());
            }
         };
      };
   }

   private object SetValue(DataColumn dataColumn, string text)
   {
      switch (dataColumn.DataType.ToString())
      {
         case "System.Boolean":
         case "System.Int32":
         case "System.Int64":
         case "System.UInt32":
         case "System.UInt64":
            if (text != "")
            {
               return Int64.Parse(text);
            }
            else
            {
               return DBNull.Value;
            }

         case "System.Decimal":
         case "System.Double":
            if (text != "")
            {
               return Double.Parse(text.Replace(",", "."), CultureInfo.InvariantCulture);
            }
            else
            {
               return DBNull.Value;
            }

         case "System.String":
            return text;

         default:
            // Debug.WriteLine(item.GetType().ToString());

            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
            break;
      };

      return null;
   }

   // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

   private void LoadTable()
   {
      if (_SelectedTable != null)
      {
         try
         {
            //ToDo: select only Active ??? where clause ???

            // TableCountItems = int.Parse(DB_SQL.QuickQuery(string.Format("select count(*) from {0} where PK > 0", _SelectedTable.Value)));
            //TableDataView = (DB_SQL.QuickQueryView(string.Format("select * from {0} where PK > 0", _SelectedTable.Value), false) as DataTable).AsDataView();
            TableDataView = (DB_SQL.QuickQueryView(string.Format("select * from {0} where PK > 0", _SelectedTable.Value)) as DataTable).AsDataView();
            TableCountItems = (TableDataView != null ? TableDataView.Count : 0);

            if (SelectedTable.Tag != null)
            {
               if (SelectedTable.Tag is TStrings)
               {
                  for (int i = 0; i < TableDataView.Table.Columns.Count; i++)
                  {
                     string _Label = (SelectedTable.Tag as TStrings)[TableDataView.Table.Columns[i].ColumnName];

                     string[] st = _Label.Split(new char[] { ',' });

                     TableDataView.Table.Columns[i].Caption = st[0].TrimStart(new char[] { '_' });

                     if (st.Length > 1)
                     {
                        TableDataView.Table.Columns[i].MaxLength = int.Parse(st[1]);
                     };
                  };
               };
            };
         }
         catch
         {
            TableCountItems = 0;
            TableDataView = null;

            MessageBox.Show(DB_SQL._ViewModel.LastError, "Oups ...");
         };
      }
      else
      {
         // - - - _SelectedTable == null  - - - 
         TableCountItems = 0;
         TableDataView = null;
      };

      OnPropertyChanged("TableDataView");
   }

   // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

   DataRow _SelectedRecord = null;
   public DataRow SelectedRecord
   {
      get
      {
         return _SelectedRecord;
      }
      set
      {
         if (value != _SelectedRecord)
         {
            _PreviousRecord = _SelectedRecord;
            _SelectedRecord = value;

            OnPropertyChanged();
         }
      }
   }

   DataRow _PreviousRecord = null;
   DataRow OldRecordState = null;

   // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

   public void CancelChanges()
   {
      if ((SelectedRecord != null) && (OldRecordState != null))
      {
         ObjectExtensions.CopyPropertyValues(OldRecordState, SelectedRecord);
      }
      else
      {
         SelectedRecord = _PreviousRecord;
      };

      OnPropertyChanged("SelectedRecord");
   }

   public void SaveRecord(object sender)
   {
      if (SelectedRecord != null)
      {
         //SelectedRecord.RecordName = SelectedRecord.RecordName.Trim();
         //SelectedRecord.Login = SelectedRecord.Login.Trim();
         //SelectedRecord.Updated = DateTime.Now;

         if (SelectedRecord.RowState == DataRowState.Detached)
         {
            //ToDo: OnInsert
            if (OnAction != null && !OnAction(ActionType.Insert, SelectedTable.Value, SelectedRecord))
            {
               if (!DB_SQL.Insert(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               }
               else
               {
                  OldRecordState = TableDataView.Table.NewRow();
                  ObjectExtensions.CopyPropertyValues(OldRecordState, SelectedRecord);
               };
            }
            else
            {
               if (!DB_SQL.Insert(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               }
               else
               {
                  OldRecordState = TableDataView.Table.NewRow();
                  ObjectExtensions.CopyPropertyValues(OldRecordState, SelectedRecord);
               };
            };
         }
         else
         {
            //ToDo: OnUpdate
            if (OnAction != null && !OnAction(ActionType.Update, SelectedTable.Value, SelectedRecord))
            {
               if (!DB_SQL.Update(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               };
            }
            else
            {
               if (!DB_SQL.Update(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               };
            };
         };

         LoadTable();

         if (_DataGridReferentiel != null && sender != _DataGridReferentiel)
         {
            UpdateDataGrid(_DataGridReferentiel);
         };
      }
   }

   public void NewRecord()
   {
      _PreviousRecord = _SelectedRecord;

      SelectedRecord = TableDataView.Table.NewRow();

      if (_StackPanelDetails != null)
      {
         DisplayRecord(_StackPanelDetails);
      };
   }

   public void CopyRecord()
   {
      _PreviousRecord = _SelectedRecord;

      SelectedRecord = TableDataView.Table.NewRow();

      // ObjectExtensions.CopyPropertyValues(SelectedRecord, _PreviousRecord);


      if (_StackPanelDetails != null)
      {
         DisplayRecord(_StackPanelDetails);
      };
   }

   public void DeleteRecord()
   {
      if (SelectedRecord != null)
      {
         if (SelectedRecord.RowState != DataRowState.Detached)
         {
            //ToDo: OnDelete
            if (OnAction != null && !OnAction(ActionType.Delete, SelectedTable.Value, SelectedRecord))
            {
               if (!DB_SQL.Delete(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               };
            }
            else
            {
               if (!DB_SQL.Delete(DB_SQL._ViewModel, SelectedTable.Value, SelectedRecord))
               {
                  DB_SQL.MessageBoxSQL();
               };
            };

            SelectedRecord = _PreviousRecord;
         }

         LoadTable();
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public void DisplayRecord(DataRow selectedItem)
   {
      if (selectedItem == null) return;

      _PreviousRecord = null;

      SelectedRecord = selectedItem;
      OldRecordState = selectedItem.Table.NewRow();
      ObjectExtensions.CopyPropertyValues(OldRecordState, selectedItem);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   int _TableCountItems = 0;
   public int TableCountItems
   {
      get
      {
         return _TableCountItems;
      }

      set
      {
         if (_TableCountItems != value)
         {
            _TableCountItems = value;
            OnPropertyChanged();
         };
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   bool _IsGridReadOnly = true;
   public bool IsGridReadOnly
   {
      get { return _IsGridReadOnly; }
      set { SetField(ref _IsGridReadOnly, value); }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public bool CheckRights { get; set; }
   public string IniFileName { get; set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public void TablesRefresh()
   {
      Tables = Tables.ToList();
      OnPropertyChanged("Tables");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}

