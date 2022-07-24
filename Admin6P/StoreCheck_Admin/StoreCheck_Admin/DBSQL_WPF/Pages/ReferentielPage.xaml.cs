using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ZPF.WPF
{
   /// <summary>
   /// Interaction logic for ReferentielPage.xaml
   /// </summary>
   public partial class ReferentielPage : Page
   {
      public ReferentielPage()
      {
         this.DataContext = EditReferentielViewModel.Current;

         InitializeComponent();

         EditReferentielViewModel.Current.Init(sdDetails, dgReferentiel);
      }

      private void Page_Loaded(object sender, RoutedEventArgs e)
      {
         if (EditReferentielViewModel.Current.SelectedTable != null)
         {
            EditReferentielViewModel.Current.UpdateDataGrid(dgReferentiel);
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

      private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (EditReferentielViewModel.Current.CheckRights)
         {
            //ToDo: if (UserViewModel.Current.CheckRights("Referentiel.Consultation", true))
            {
               EditReferentielViewModel.Current.SelectedTable = (sender as ListBox).SelectedItem as NameValue;
            };
         }
         else
         {
            EditReferentielViewModel.Current.SelectedTable = (sender as ListBox).SelectedItem as NameValue;
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

      private void btnNew_Click(object sender, RoutedEventArgs e)
      {
         EditReferentielViewModel.Current.NewRecord();
      }

      private void btnCopy_Click(object sender, RoutedEventArgs e)
      {
         EditReferentielViewModel.Current.CopyRecord();
      }

      private void btnSave_Click(object sender, RoutedEventArgs e)
      {
         EditReferentielViewModel.Current.UpdateRecord(sdDetails);
         EditReferentielViewModel.Current.SaveRecord(sender);
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         EditReferentielViewModel.Current.CancelChanges();
      }

      private void btnDelete_Click(object sender, RoutedEventArgs e)
      {
         if ( BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Confirmation, "Etes-vous sur de vouloir supprimer cet élément?", "Confirmation") )
         {
            EditReferentielViewModel.Current.DeleteRecord();
            EditReferentielViewModel.Current.UpdateDataGrid(dgReferentiel);
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

      private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {

         if (sender is DataGrid)
         {
            var dg = (sender as DataGrid);

            sdDetails.Children.Clear();

            if (dg.SelectedItem == null)
            {
               return;
            };

            //if (e.RemovedItems.Count == 1)
            //{
            //}
            //else if (e.AddedItems.Count == 1 && ((System.Data.DataRowView)((object[])e.AddedItems)[0]).IsNew)
            //{
            //   // e.AddedItems.Remove(0);
            //   EditReferentielViewModel.Current.NewRecord();
            //}
            //else
            {
               EditReferentielViewModel.Current.DisplayRecord((dg.SelectedItem as System.Data.DataRowView).Row);
               EditReferentielViewModel.Current.DisplayRecord(sdDetails);
            };

            return;
         };
      }

      private void Page_Unloaded(object sender, RoutedEventArgs e)
      {
         EditReferentielViewModel.Current.ReferentielLoadData();
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
      private void dgReferentiel_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
      {
         return;

         if (e.EditAction == DataGridEditAction.Commit)
         {
            try
            {
               EditReferentielViewModel.Current.SaveRecord(EditReferentielViewModel.Current.SelectedRecord);
            }
            catch
            {
               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
            };
         };
      }

      //private bool isManualEditCommit;
      private void dgReferentiel_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
      {
         return;

         if (e.EditAction == DataGridEditAction.Commit)
         {
            try
            {
               for (int i = 0; i < EditReferentielViewModel.Current.SelectedRecord.Table.Columns.Count; i++)
               {
                  if (EditReferentielViewModel.Current.SelectedRecord.Table.Columns[i].ColumnName == e.Column.SortMemberPath)
                  {
                     EditReferentielViewModel.Current.SelectedRecord[i] = ((System.Windows.Controls.TextBox)e.EditingElement).Text;
                     break;
                  };
               }
            }
            catch
            {
               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
            };
         };

         //if (!isManualEditCommit)
         //{
         //   isManualEditCommit = true;
         //   DataGrid grid = (DataGrid)sender;
         //   grid.CommitEdit(DataGridEditingUnit.Row, true);
         //   isManualEditCommit = false;
         //}
      }

      private void dgData_PreviewExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
      {
         return;

         if (e.Command == DataGrid.DeleteCommand)
         {
            if (!(MessageBox.Show("Are You Sure you want to Delete ?", "Confirm Delete !", MessageBoxButton.YesNo) == MessageBoxResult.Yes))
            {
               e.Handled = true;
            }
            else
            {
               EditReferentielViewModel.Current.DeleteRecord();
            }
         }
      }

      private void dgReferentiel_Loaded(object sender, RoutedEventArgs e)
      {
         var dataGrid = sender as DataGrid;
         new DataGridTools(null, null, dataGrid);
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
   }
}
