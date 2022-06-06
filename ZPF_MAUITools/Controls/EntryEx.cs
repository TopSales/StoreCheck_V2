namespace ZPF.XF
{
    public class EntryEx : Entry
   {
      public EntryEx()
      {
         AutoSelect = false;
      }

      public bool AutoSelect { get; set; }

      // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 

      public static readonly BindableProperty HasBordersProperty = BindableProperty.Create(
              nameof(HasBorders),
              typeof(bool),
              typeof(EntryEx),
              true,
              propertyChanging: (bindable, oldValue, newValue) =>
              {
                 var control = bindable as EntryEx;

                 control.HasBorders = (bool)newValue;
              });

      public bool HasBorders { get => _HasBorders; set { _HasBorders = value; OnPropertyChanged("HasBorders"); } }
      bool _HasBorders = true;

      // - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - -   - - 
   }
}
