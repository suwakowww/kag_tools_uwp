using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class Edit_ks : Page
    {
        public Edit_ks()
        {
            this.InitializeComponent();
            checkwidth(Window.Current.Bounds.Width, true);
            checkver();
        }

        private void checkver()
        {
            ulong v = ulong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            if (v3 >= 16299)
            {
                //AcrylicBrush acrylic = new AcrylicBrush();
                //acrylic.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                //acrylic.TintOpacity = 0.6;
                //acrylic.TintColor = (this.ActualTheme == ElementTheme.Dark ? Colors.Black : Colors.White);
                //top_cmdbar.Background = acrylic;
            }

        }

        /// <summary>
        /// 检测宽度
        /// </summary>
        /// <param name="size">获取宽度的变量</param>
        /// <param name="iswinsize">如果为 true，则作为窗口大小识别，反之为页面大小识别。</param>
        private void checkwidth(double size, bool iswinsize)
        {
            int page_size=(int)size;
            //int.TryParse(size.ToString(), out page_size);
            if (iswinsize==true)
            {
                
            }
            else
            {
                if(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    if (page_size < 640) 
                    {
                        top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                        bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                        large_view.Visibility = Visibility.Collapsed;
                        small_view.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                        top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                        large_view.Visibility = Visibility.Visible;
                        small_view.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (page_size < 640)
                    {
                        top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                        bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                        large_view.Visibility = Visibility.Collapsed;
                        small_view.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                        top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                        large_view.Visibility = Visibility.Visible;
                        small_view.Visibility = Visibility.Collapsed;
                    }
                }
                //if (page_size<640 && Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                //{
                //    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                //    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom;
                //    win_page_size.Visibility = Visibility.Collapsed;
                //    //bot_p_n.Visibility = Visibility.Collapsed;
                //    bot_p_p.Visibility = Visibility.Collapsed;
                //    bot_p_r.Visibility = Visibility.Collapsed;
                //    large_view.Visibility = Visibility.Collapsed;
                //    small_view.Visibility = Visibility.Visible;
                //}
                //else if(page_size<640)
                //{


                //}
                //else
                //{
                //    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                //    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
                //    //bot_p_n.Visibility = Visibility.Visible;
                //    bot_p_p.Visibility = Visibility.Visible;
                //    bot_p_r.Visibility = Visibility.Visible;
                //    win_page_size.Visibility = Visibility.Visible;
                //    large_view.Visibility = Visibility.Visible;
                //    small_view.Visibility = Visibility.Collapsed;
                //}
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkwidth((sender as Page).ActualWidth,false);
            win_page_size.Text = string.Format("窗口：{0} 像素，页面：{1} 像素",((int)Window.Current.Bounds.Width).ToString(),((int)(sender as Page).ActualWidth).ToString());
            win_page_size2.Text = win_page_size.Text;
        }
    }
}
