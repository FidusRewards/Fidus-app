using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fidus
{
    public class MasterPageItem
    {
        public string Title { get; set; }

        public Color TextColor { get; set; }

        public string IconSource { get; set; }

        public Type TargetType { get; set; }
    }
}
