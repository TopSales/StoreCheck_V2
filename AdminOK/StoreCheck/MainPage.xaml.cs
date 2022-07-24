using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoreCheck
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

      protected override void OnDisappearing()
      {
         base.OnDisappearing();

         Application.Current.Quit();
      }

	}
}
