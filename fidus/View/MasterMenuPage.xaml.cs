using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlideOverKit;
using Xamarin.Forms;
using System.Diagnostics;


namespace fidus
{
    public partial class MasterMenuPage : SlideMenuView
    {
		private ListView listview = new ListView() { 
			VerticalOptions = LayoutOptions.FillAndExpand,
			SeparatorVisibility = SeparatorVisibility.None,
			BackgroundColor= Color.Transparent
		};
		private StackLayout MenuBar;
		private Image MenuIcon;
		public ListView ListView { get { return listview; } }

		public MasterMenuPage()
		{
			InitializeComponent();
			// You must set IsFullScreen in this case, 
			// otherwise you need to set HeightRequest, 
			// just like the QuickInnerMenu sample
			this.IsFullScreen = true;
			// You must set WidthRequest in this case
			this.WidthRequest = 250;
			this.MenuOrientations = MenuOrientation.RightToLeft;
			// You must set BackgroundColor, 
			// and you cannot put another layout with background color cover the whole View
			// otherwise, it cannot be dragged on Android
			this.BackgroundColor = Color.White;

			// This is shadow view color, you can set a transparent color
			this.BackgroundViewColor = Color.Transparent;//FromHex("#CE766C");


			var BkgImage = new Image()
			{
				Source = ImageSource.FromResource("fidus.giftly.png"),
				Aspect = Aspect.AspectFill
			};

			RelativeLayout MenuLayout = new RelativeLayout();
			MenuLayout.VerticalOptions = LayoutOptions.FillAndExpand;

			MenuLayout.Children.Add(BkgImage,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => { return parent.Width; }),
				Constraint.RelativeToParent((parent) => { return parent.Height; }));

			if (Device.OS != TargetPlatform.Android)
			{
				 MenuBar = new StackLayout()
				{
					HorizontalOptions = LayoutOptions.FillAndExpand,
					HeightRequest = 65,
					BackgroundColor = Color.FromHex(Settings.FidusColor)
				};

				 MenuIcon = new Image()
				{
					Margin = new Thickness(10, 22, 0, 12),
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Start,
					Source = "settings1.png",
					HeightRequest = 20,
					WidthRequest = 20
				};
				MenuBar.Children.Add(MenuIcon);
			}

			Image UserImg = new Image() { 
				Source="userimg.png",
			   	HeightRequest=70,
				WidthRequest=70,
				HorizontalOptions=LayoutOptions.Center,
				VerticalOptions=LayoutOptions.Center,
				Margin=new Thickness(0,10,0,0)			
			};

			Label UsrLab = new Label() { 
				Text=Settings.CurrentUser.Name,
				FontFamily=Device.OnPlatform("Helvetica-bold", "Roboto-bold",""),
				HorizontalOptions=LayoutOptions.Center,
				VerticalOptions=LayoutOptions.Center
			};
			Label Usrmail = new Label()
			{
				Text = Settings.CurrentUser.Email,
				FontFamily = Device.OnPlatform("Helvetica-bold", "Roboto-bold", ""),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			Image linea = new Image() { 
				Source="menuline.png",
			   	HeightRequest=20,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(10,10,10,0)
			};

			Label menutext = new Label()
			{
				Text = "Menú Principal",
				TextColor = Color.FromHex(Settings.FidusColor),
				FontFamily = Device.OnPlatform("Helvetica", "Roboto", ""),
				FontSize=14,
				HorizontalOptions=LayoutOptions.Start,
				VerticalOptions=LayoutOptions.Center,
				FontAttributes=FontAttributes.Bold,
				Margin=new Thickness(10,0,5,0)
			};
			var cell = new DataTemplate(typeof(Controls.CustomImageCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.TextColorProperty, "TextColor");
			cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");

			listview.ItemTemplate = cell;
			
			var masterPageItem = new List<MasterPageItem>();


            //masterPageItem.Add(new MasterPageItem
            //{
            //    Title = "Inicio",
            //    TextColor = Color.FromHex(Settings.FidusColor),
            //    IconSource = "geopin.png",
            //    TargetType = typeof(MainPage)
            //});
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Historial de Canjes",
                TextColor = Color.Black,
                IconSource = "historial.png",
                TargetType = typeof(HistoryPage)
            });
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Editar Perfil",
                TextColor = Color.Black,
                IconSource = "iconousuario.png"
            });
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Logout",
                TextColor = Color.Black,
                IconSource = "logout.png"
            });
            listview.ItemsSource = masterPageItem;
			StackLayout MenuObjects = new StackLayout()
			{
				Orientation=StackOrientation.Vertical
			};

			if (Device.OS != TargetPlatform.Android)
			{ 	
				MenuObjects.Children.Add(MenuBar); 
				var OnTap = new TapGestureRecognizer();
				OnTap.Tapped += (sender, e) => { this.IsEnabled = false; };

				MenuIcon.GestureRecognizers.Add(OnTap);
			}

			MenuObjects.Children.Add(UserImg);
			MenuObjects.Children.Add(UsrLab);
			MenuObjects.Children.Add(Usrmail);
			MenuObjects.Children.Add(linea);
			MenuObjects.Children.Add(menutext);

			MenuObjects.Children.Add(listview);

			MenuLayout.Children.Add(MenuObjects,
				Constraint.Constant(0),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent) => { return parent.Width; }),
				Constraint.RelativeToParent((parent) => { return parent.Height; }));

			this.Content = MenuLayout;
			ListView.ItemSelected += OnItemSelected;

		}

		internal async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as MasterPageItem;

			if (item != null)
			{
				if (item.TargetType != null)
				{
					Page Detail = (Page)Activator.CreateInstance(item.TargetType);
					await Navigation.PushAsync(Detail);

					Debug.WriteLine("Menu : " + item.Title);
					this.HideWithoutAnimations();
					ListView.SelectedItem = null;
				}
				else if (item.Title == "Logout")
				{
					try
					{
						this.HideWithoutAnimations();

						await Navigation.PopToRootAsync();
						//App.instance.ClearNavigationAndGoLogin();

					}
					catch (Exception ex)
					{
						Debug.WriteLine("MainPage: Exit stack " + ex);
					}

					Debug.WriteLine("Menu : Logout");
				}
			}
		}
    }
}
