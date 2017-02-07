using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace fidus
{
    public partial class UserMenuPage : MasterDetailPage
    {
        public UserMenuPage()
        {
            InitializeComponent();
            ((NavigationPage)Detail).BarBackgroundColor = Color.FromHex(Settings.FidusColor);
            masterPage.ListView.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
				((NavigationPage)Detail).BarBackgroundColor = Color.FromHex(Settings.FidusColor);
				masterPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
