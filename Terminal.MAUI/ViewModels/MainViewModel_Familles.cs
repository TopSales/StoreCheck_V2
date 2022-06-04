using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<EAN_Family> Families { get; private set; } = new List<EAN_Family>();

   public EAN_Family SelectedFamily { get => _SelectedFamily; set => SetField(ref _SelectedFamily, value); }
   EAN_Family _SelectedFamily = null;

   public EAN_Family CurrentFamily { get; internal set; }
   public EAN_Family PrevFamily { get; internal set; }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
