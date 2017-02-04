using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace fidus
{
    public partial class MasterMenuPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterMenuPage()
        {
            InitializeComponent();
            var masterPageItem = new List<MasterPageItem>();
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Inicio",
                TextColor = Color.FromHex(Settings.FidusColor),
                IconSource = "",
                TargetType = typeof(MainPage)
            });
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Historial de Canjes",
                TextColor = Color.Black,
                IconSource = "",
                TargetType = typeof(HistoryPage)
            });
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Editar Perfil",
                TextColor = Color.Black,
                IconSource = ""
            });
            masterPageItem.Add(new MasterPageItem
            {
                Title = "Historial",
                TextColor = Color.Black,
                IconSource = ""
            });
            listView.ItemsSource = masterPageItem;
        }
    }
}
