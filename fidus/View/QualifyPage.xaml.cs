using System;
using System.Diagnostics;
using Syncfusion.SfRating.XForms;

using Xamarin.Forms;

namespace fidus
{
	public partial class QualifyPage : ContentPage
	{
		LoadAsync<History> _clientH = new LoadAsync<History>(MainViewModel._mclientH);
		SfRating ratingst = new SfRating();

		public QualifyPage(string place="Fidus", string puntos="0", string logo=Helpers.Settings.ImgSrvProd+"logofidus.png", string category="Resto Bar", History _history=null)
		{
			InitializeComponent();

			NavigationPage.SetTitleIcon(this, "fidus_text.png");
			this.Title = "Fidus";

			PImage.Source = logo;
			PImage.HeightRequest = 62;
			PImage.WidthRequest = 62;
			PImage.VerticalOptions = LayoutOptions.Center;

			PName.Text = place;
			PName.TextColor = Color.Black;
			PName.FontAttributes = FontAttributes.Bold;
			PName.FontSize = 18;
			PName.HorizontalOptions = LayoutOptions.StartAndExpand;
			PCat.Text = category;

			PPuntos.Text = "Sumaste " + puntos + " puntos";
			PPuntos.TextColor = Color.FromHex(Helpers.Settings.FidusColor);
			PPuntos.VerticalOptions = LayoutOptions.Center;
			PPuntos.FontFamily = Device.OnPlatform(Helpers.Settings.FidusIosFont, Helpers.Settings.FidusAndFont, "");
			PPuntos.FontAttributes = FontAttributes.Bold;
			PPuntos.FontSize = 20;
			PPuntos.Margin = new Thickness(0, 5, 0, 5);

			PGracias.Source = "graciasporvenir.png";
			PGracias.BackgroundColor = Color.Transparent;
			PGracias.VerticalOptions = LayoutOptions.Center;
			PGracias.HorizontalOptions = LayoutOptions.Center;
			PGracias.HeightRequest = 70;

			bt_Avanzar.Image = "btn_continuargris.png";
			bt_Avanzar.BackgroundColor = Color.FromHex("#646464");
			bt_Avanzar.BorderColor = Color.FromHex("#646464");

			ratingst.RatingSettings.RatedFill = Color.FromHex(Helpers.Settings.FidusColor);
			ratingst.RatingSettings.UnRatedFill = Color.Silver;
			ratingst.ItemSize = 40;
			ratingst.RatingSettings.RatedStroke = Color.Transparent;
			ratingst.RatingSettings.UnRatedStroke = Color.Transparent;
			ratingst.ItemCount = 5;
			ratingst.Precision = Precision.Standard;

			ratingctl.Children.Add (ratingst);


			bt_Avanzar.Clicked += async (sender, e) =>
			{
				bt_Avanzar.IsEnabled = false;
				Helpers.Settings.IsReturn = false;

				_history.Rating = Convert.ToInt32(ratingst.Value);
				_history.Comment = comment.Text;
				string temp="";
				string temp1="";
				var max = Helpers.Settings.qrdate.Count;

				for (int i = 0; i < max; i++)
				{
					temp += Helpers.Settings.qrdate.Dequeue().ToString();
					temp1 += Helpers.Settings.qrbranch.Dequeue().ToString();
				}
				Helpers.Settings.QRLastTimes = temp;
				Helpers.Settings.QRLastBranches = temp1;

				try
				{
					await _clientH.SaveHistory(_history);
				} catch (Exception exe)
				{
					await DisplayAlert("Error", "No se han podido almacenar lo datos de tu código, favor reintentá", "OK");
					Debug.WriteLine("Exception en InsertAsync del Qualify page" + exe.StackTrace);
				}

				await Navigation.PopToRootAsync();
				//await _itemsH.Save(_history);

			};
		}
		protected override bool OnBackButtonPressed()
		{
			
			return false;
		}


	}
}