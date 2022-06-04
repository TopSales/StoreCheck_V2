using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public Intervention_Params.Scann CurrentArticle { get; internal set; }
   public Intervention_Params.Scann PrevArticle { get; internal set; }
   public string PrevData { get; internal set; }
   public string CurrentData { get; internal set; }
   public Intervention_Params.Scann ReplacementArticle { get; internal set; }


   internal void UpdatePrevArticle()
   {
      OnPropertyChanged("PrevArticle");
   }

   public void UpdateCurrentArticle()
   {
      OnPropertyChanged("CurrentArticle");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
