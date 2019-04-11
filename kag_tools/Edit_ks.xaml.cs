using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using kag_tools_shared;
using kag_tools.cdlg;
using System.Text;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace kag_tools
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class Edit_ks : Page
    {
        List<perlines> perline;

        public Edit_ks()
        {
            this.InitializeComponent();
            checkwidth(Window.Current.Bounds.Width, true);
            checkver();
        }

        #region 系统版本检测
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
        #endregion

        #region 检测宽度
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
        #endregion

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            checkwidth((sender as Page).ActualWidth,false);
            win_page_size.Text = string.Format("窗口：{0} 像素，页面：{1} 像素",((int)Window.Current.Bounds.Width).ToString(),((int)(sender as Page).ActualWidth).ToString());
            win_page_size2.Text = win_page_size.Text;
        }

        #region 打开 *.ks 文件
        private async void Import_ks_Click(object sender, RoutedEventArgs e)
        {

            // 打开文件
            FileOpenPicker fop = new FileOpenPicker();
            fop.FileTypeFilter.Add(".ks");
            StorageFile sf = await fop.PickSingleFileAsync();

            if(sf!=null)
            {

                //把文件转换为 ibuffer
                IBuffer buffer = await FileIO.ReadBufferAsync(sf);

                //以二进制方式读取文件
                DataReader datareader = DataReader.FromBuffer(buffer);
                byte[] src = new byte[datareader.UnconsumedBufferLength];
                datareader.ReadBytes(src);

                //判断字符编码
                string encoding = parse_bytes.DetectUnicode(src);
                string src2;

                //判断 ANSI 编码
                if (encoding == "ansi")
                {
                    //ContentDialog cdlg = new ContentDialog()
                    //{
                    //    Title = "检测到 ANSI 编码",
                    //    Content = "暂时无法解析 ANSI 编码。",
                    //    PrimaryButtonText = "关闭"
                    //};
                    //await cdlg.ShowAsync();
                    ansitake ansitake_dlg = new ansitake();
                    ansitake_dlg.src = src;
                    await ansitake_dlg.ShowAsync();
                    encoding = ansitake_dlg.cp;
                }

                //还原二进制为文本
                src2 = parse_bytes.byte2str(src, encoding);
                perline = parse_perline.parsestr(src2);
                //按行拆分文本

                for (int i=0;i<perline.Count;i++)
                {
                    
                    //根据每行内容，进行上色
                    perline[i].textcolor = new SolidColorBrush(this.ActualTheme == ElementTheme.Dark ? perline[i].text_cd : perline[i].text_cl);
                }
                text_list.ItemsSource = perline;
                text_list2.ItemsSource = perline;
                file_info.Text = sf.Name;
                bot_p_n.IsEnabled = true;
                bot_p_n2.IsEnabled = true;
            }
        }
        #endregion

        #region 上/下导航按钮控制
        private void Bot_p_n_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as AppBarButton).Name == "bot_p_n" || (sender as AppBarButton).Name == "bot_p_n2")
            {
                bot_p_p.IsEnabled = true;
                bot_p_p2.IsEnabled = true;
                if (large_view.Visibility == Visibility.Visible)
                {
                    while (true)
                    {
                        text_list.SelectedIndex = text_list.SelectedIndex + 1;
                        if (((kag_tools_shared.perlines)text_list.SelectedItem).texttype == 't' || ((kag_tools_shared.perlines)text_list.SelectedItem).texttype == 's' || text_list.SelectedIndex == text_list.Items.Count - 1)
                            break;
                    }
                    if (text_list.SelectedIndex >= text_list.Items.Count - 1)
                    {
                        bot_p_n.IsEnabled = false;
                        bot_p_n2.IsEnabled = false;
                    }
                }
                else
                {
                    while (true)
                    {
                        text_list2.SelectedIndex = text_list2.SelectedIndex + 1;
                        if (((kag_tools_shared.perlines)text_list2.SelectedItem).texttype == 't' || ((kag_tools_shared.perlines)text_list2.SelectedItem).texttype == 's' || text_list2.SelectedIndex == text_list2.Items.Count - 1)
                            break;
                    }
                    if (text_list2.SelectedIndex >= text_list2.Items.Count - 1)
                    {
                        bot_p_n.IsEnabled = false;
                        bot_p_n2.IsEnabled = false;
                    }
                }
            }
            else
            {
                bot_p_n.IsEnabled = true;
                bot_p_n2.IsEnabled = true;
                if (large_view.Visibility == Visibility.Visible)
                {
                    while (true)
                    {
                        text_list.SelectedIndex = text_list.SelectedIndex - 1;
                        if (((kag_tools_shared.perlines)text_list.SelectedItem).texttype == 't' || ((kag_tools_shared.perlines)text_list.SelectedItem).texttype == 's' || text_list.SelectedIndex <= 0)
                            break;
                    }
                    if (text_list.SelectedIndex <= 0)
                    {
                        bot_p_p.IsEnabled = false;
                        bot_p_p2.IsEnabled = false;
                    }
                }
                else
                {
                    while (true)
                    {
                        text_list2.SelectedIndex = text_list2.SelectedIndex - 1;
                        if (((kag_tools_shared.perlines)text_list2.SelectedItem).texttype == 't' || ((kag_tools_shared.perlines)text_list2.SelectedItem).texttype == 's' || text_list2.SelectedIndex <= 0)
                            break;
                    }
                    if (text_list2.SelectedIndex <= 0)
                    {
                        bot_p_p.IsEnabled = false;
                        bot_p_p2.IsEnabled = false;
                    }
                }
            }
        }
        #endregion

        #region 列表同步、按钮控制等
        private void Text_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (text_list.Items.Count > 1)
            {

                //根据所选中的控件，对另一个隐藏的控件进行控制
                if ((sender as ListView).Name == "text_list" && (sender as ListView).SelectedItem != null)
                {
                    text_src.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts;
                    text_dst.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts_dst;
                    text_list2.SelectedIndex = (sender as ListView).SelectedIndex;
                }
                else if ((sender as ListView).Name == "text_list2" && (sender as ListView).SelectedItem != null)
                {
                    text_src2.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts;
                    text_dst2.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts_dst;
                    text_list.SelectedIndex = (sender as ListView).SelectedIndex;
                }

                //防止下标、上标越界
                if ((sender as ListView).SelectedIndex == 0)
                {
                    bot_p_n.IsEnabled = true;
                    bot_p_n2.IsEnabled = true;
                    bot_p_p.IsEnabled = false;
                    bot_p_p2.IsEnabled = false;
                }
                else if ((sender as ListView).SelectedIndex == (sender as ListView).Items.Count - 1)
                {
                    bot_p_n.IsEnabled = false;
                    bot_p_n2.IsEnabled = false;
                    bot_p_p.IsEnabled = true;
                    bot_p_p2.IsEnabled = true;
                }
                else
                {
                    bot_p_n.IsEnabled = true;
                    bot_p_n2.IsEnabled = true;
                    bot_p_p.IsEnabled = true;
                    bot_p_p2.IsEnabled = true;
                }
            }
        }
        #endregion

        #region 写入修改到列表
        private void Text_dst_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Name == "text_dst")
            {
                perline[text_list.SelectedIndex].texts_dst = text_dst.Text;
                text_dst2.Text = text_dst.Text;
            }
            else
            {
                perline[text_list.SelectedIndex].texts_dst = text_dst2.Text;
                text_dst.Text = text_dst2.Text;
            }
        }
        #endregion

        #region 保存 .ks 文件
        private async void Output_ks_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker fos = new FileSavePicker();
            fos.FileTypeChoices.Add("KAG Script", new List<string> { ".ks" });
            StorageFile sf = await fos.PickSaveFileAsync();
            if (sf != null)
            {
                Encoding encoding = Encoding.Unicode;   //默认保存为 UTF-16 LE 编码
                byte[] datas;
                string save_file = null;
                for (int i = 0; i < perline.Count; i++)
                {
                    save_file = save_file + perline[i].texts_dst + "\r\n";
                }
                datas = encoding.GetBytes(save_file);
                await FileIO.WriteBytesAsync(sf, datas);
            }
        }
        #endregion

    }
}
