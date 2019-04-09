using kag_tools.inc;
using kag_tools_shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace kag_tools
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<nav_item> nav_items = nav_itemlist.get_navs();

        public MainPage()
        {
            this.InitializeComponent();
            main_nav.MenuItemsSource = nav_items;
        }

        private void Main_nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected != true)
            {
                string item_name = ((kag_tools_shared.nav_item)sender.SelectedItem).nav_name;
                Type target = null;
                for (int i = 0; i < nav_items.Count; i++)
                {
                    if (nav_items[i].nav_name == item_name)
                    {
                        target = nav_items[i].nav_page;
                        break;
                    }
                }
                if (target != null)
                    main_frame.Navigate(target);
            }
        }
    }
}
