using kag_tools.inc;
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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace kag_tools
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage_legacy : Page
    {
        private List<nav_item> nav_items = nav_itemlist.get_navs();

        public MainPage_legacy()
        {
            this.InitializeComponent();
            checkwidth();

            //获取导航菜单
            //List<nav_item> nav_items = nav_itemlist.get_navs();
            main_nav.ItemsSource = nav_items;

            main_nav.SelectedIndex = 0;
        }

        private void Split_switch_Click(object sender, RoutedEventArgs e)
        {
            main_split.IsPaneOpen = !main_split.IsPaneOpen;
        }

        //检测屏幕宽度，并在≤640像素时修改排版，并隐藏亚克力效果（手机会自动隐藏亚克力效果）
        private void checkwidth()
        {
            if (Window.Current.Bounds.Width <= 640)
            {
                main_split.Margin = new Thickness(0, 48, 0, 0);
                main_split.DisplayMode = SplitViewDisplayMode.Overlay;
                main_topbar.Visibility = Visibility.Visible;
                main_split.IsPaneOpen = false;
                //手机不存在标题栏
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                {
                    ShowTitleBar();
                }
                else
                {
                    acrylic_show.Visibility = Visibility.Collapsed;
                    acrylic_show2.Visibility = Visibility.Collapsed;
                }
            }
            else if (Window.Current.Bounds.Width <= 1024)
            {
                main_split.Margin = new Thickness(0, 48, 0, 0);
                main_topbar.Visibility = Visibility.Visible;
                main_split.DisplayMode = SplitViewDisplayMode.CompactInline;
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                    ShowTitleBar();
                else
                {
                    acrylic_show.Visibility = Visibility.Collapsed;
                    acrylic_show2.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                main_split.Margin = new Thickness(0, 0, 0, 0);
                try
                {
                    AcrylicBrush acrylicBrush = new AcrylicBrush();
                    //acrylicBrush.TintColor = Colors.WhiteSmoke;
                    acrylicBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                    acrylicBrush.TintOpacity = 0.6;
                    bool appcolors = true;
                    if (this.ActualTheme == ElementTheme.Dark) appcolors = true;
                    else if (this.ActualTheme == ElementTheme.Light) appcolors = false;
                    acrylicBrush.TintColor = (appcolors ? Colors.Black : Colors.White);
                    acrylic_show.Fill = acrylicBrush;
                    acrylic_show.Visibility = Visibility.Visible;
                    acrylic_show2.Fill = acrylicBrush;
                    acrylic_show2.Visibility = Visibility.Visible;
                    //ExtendAcrylicIntoTitleBar();
                }
                catch
                {
                    //不支持亚克力效果就直接不显示了
                    acrylic_show.Visibility = Visibility.Collapsed;
                    acrylic_show2.Visibility = Visibility.Collapsed;

                    //手机不存在标题栏
                    if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                        ShowTitleBar();
                }
                main_topbar.Visibility = Visibility.Collapsed;
                main_split.DisplayMode = SplitViewDisplayMode.Inline;
                main_split.IsPaneOpen = true;
            }
        }

        //从微软的亚克力材质说明这里抄下来的代码
        private void ExtendAcrylicIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            main_nav.Margin = new Thickness()
            {
                Top = 32
            };
        }

        //又是粗暴的还原代码
        private void ShowTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = (this.ActualTheme == ElementTheme.Dark ? Colors.Black : Colors.White);
            titleBar.ButtonInactiveBackgroundColor = (this.ActualTheme == ElementTheme.Dark ? Colors.Black : Colors.White);
            main_nav.Margin = new Thickness()
            {
                Top = 0
            };
        }

        private void Main_nav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select_index = (sender as ListView).SelectedIndex;
            main_frame.Navigate(nav_items[select_index].nav_page);
            //switch (select_index)
            //{
            //    case 0:
            //        {
            //            main_frame.Navigate(typeof(Start));
            //            break;
            //        }
            //    case 1:
            //        {
            //            main_frame.Navigate(typeof(Edit_ks));
            //            break;
            //        }
            //}
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkwidth();
        }
    }
}
