using kag_tools_shared;
using kag_tools_ui.cdlg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace kag_tools_ui
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class Edit_ks : Page
    {
        List<perlines> perline;
        List<dictlist> dicts;
        bool textonly = true;   //仅显示文本
        List<perlines> filter_perline;

        public Edit_ks()
        {
            this.InitializeComponent();
            checkwidth(Window.Current.Bounds.Width, true);
            checkver();
        }

        #region 检测宽度
        /// <summary>
        /// 检测宽度
        /// </summary>
        /// <param name="size">获取宽度的变量</param>
        /// <param name="iswinsize">如果为 true，则作为窗口大小识别，反之为页面大小识别。</param>
        private void checkwidth(double size, bool iswinsize)
        {
            int page_size = (int)size;
            //int.TryParse(size.ToString(), out page_size);
            if (iswinsize == true)
            {

            }
            else
            {
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
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
            }
        }
        #endregion

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

        #region 打开 *.ks 文件
        private async void Import_ks_Click(object sender, RoutedEventArgs e)
        {
            loadsave loadsave = new loadsave();
            parse_bytes parse_bytes = new parse_bytes();
            parse_perline parse_perline = new parse_perline();

            // 打开文件
            files files = await loadsave.load_ksasync();

            if (files.filename != "empty" && files.srcode.Count() != 0)
            {
                string encoding = parse_bytes.DetectUnicode(files.srcode);
                string src2;

                //ANSI 处理
                if (encoding == "ansi")
                {
                    ansitake ansitake_dlg = new ansitake();
                    ansitake_dlg.src = files.srcode;
                    await ansitake_dlg.ShowAsync();
                    encoding = ansitake_dlg.cp;
                }

                //还原二进制为文本
                src2 = parse_bytes.byte2str(files.srcode, encoding);
                perline = parse_perline.parsestr(src2);

                //按行拆分文本

                for (int i = 0; i < perline.Count; i++)
                {

                    //根据每行内容，进行上色
                    if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5))
                        perline[i].textcolor = new SolidColorBrush(this.ActualTheme == ElementTheme.Dark ? perline[i].text_cd : perline[i].text_cl);
                    else
                        //由于 this.RequestedTheme 会返回 ElementTheme.Default，故原方法不可用
                        perline[i].textcolor = new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark ? perline[i].text_cd : perline[i].text_cl);
                }

                if (textonly == true)
                {
                    //过滤代码等内容，只留下文本
                    filter_perline = parse_perline.filter_perline(perline);
                    text_list.ItemsSource = filter_perline;
                    text_list2.ItemsSource = filter_perline;
                }
                else
                {
                    //原样输出
                    text_list.ItemsSource = perline;
                    text_list2.ItemsSource = perline;
                }


                file_info.Text = files.filename;
                bot_p_n.IsEnabled = true;
                bot_p_n2.IsEnabled = true;
                text_src.IsEnabled = true;
                text_src2.IsEnabled = true;
                text_dst.IsEnabled = true;
                text_dst2.IsEnabled = true;
            }
            else
            {
                //如果打开文件的过程被中断（比如：按下了“取消”键），会返回“empty”。
                //由于发现打开空白文件的时候会发生错误，所以这里追加判断打开文件的情况。
                string err_msg = files.filename == "empty" ? "没有打开文件。" : string.Format("打开了空白的文件：{0}。", files.filename);

                ContentDialog err_dlg = new ContentDialog()
                {
                    Title = "发生错误",
                    Content = err_msg,
                    PrimaryButtonText = "关闭"
                };
                await err_dlg.ShowAsync();
            }
        }
        #endregion

        #region 保存 .ks 文件
        private async void Output_ks_Click(object sender, RoutedEventArgs e)
        {
            loadsave loadsave = new loadsave();
            string status = await loadsave.save_ksasync(perline);
            ContentDialog result = new ContentDialog()
            {
                Title = "结果",
                Content = status,
                PrimaryButtonText = "关闭"
            };
            await result.ShowAsync();
        }

        #endregion

        #region 加载词典
        private async void Load_dict_Click(object sender, RoutedEventArgs e)
        {
            loaddict loaddict = new loaddict();
            dicts = await loaddict.loaddictasync();
        }
        #endregion

        #region 机器翻译
        private async void Tr_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as AppBarButton).Name == "t_bai" || (sender as AppBarButton).Name == "t_bai2")
            {
                await Launcher.LaunchUriAsync(new Uri("http://fanyi.baidu.com/#jp/zh/" + text_src.Text));
            }
            else if ((sender as AppBarButton).Name == "t_goo" || (sender as AppBarButton).Name == "t_goo2")
            {
                await Launcher.LaunchUriAsync(new Uri("https://translate.google.com/#ja/zh-CN/" + text_src.Text));
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri("https://translate.google.cn/#ja/zh-CN/" + text_src.Text));
            }
        }
        #endregion

        #region 检测屏幕宽度，debug 用
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool debug_mode = true;
            checkwidth((sender as Page).ActualWidth, false);
            if (debug_mode == true)
                win_page_size.Text = string.Format("窗口：{0} 像素，页面：{1} 像素", ((int)Window.Current.Bounds.Width).ToString(), ((int)(sender as Page).ActualWidth).ToString());
            else
                win_page_size.Text = "";
            win_page_size2.Text = win_page_size.Text;
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
            countword();
        }
        #endregion

        #region 字数统计
        private int countword()
        {
            win_page_size.Text = string.Format("原文：{0} 字，译文：{1} 字", text_src.Text.Count(), text_dst.Text.Count());
            win_page_size2.Text = win_page_size.Text;
            return text_dst.Text.Count();
        }
        #endregion

        #region 写入修改到列表
        private void Text_dst_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text != "")
            {
                //由于引入了“仅显示文本”，这里需要对索引值重新定位
                int realindex;
                
                int words = countword();

                //在写入修改之前，先确认有内容再进行写入
                if (words != 0)
                {
                    if (textonly == true)
                    {
                        realindex = filter_perline[text_list.SelectedIndex].line_num;
                    }
                    else
                    {
                        realindex = perline[text_list.SelectedIndex].line_num;
                    }
                    if ((sender as TextBox).Name == "text_dst")
                    {
                        perline[realindex].texts_dst = text_dst.Text;
                        text_dst2.Text = text_dst.Text;
                    }
                    else
                    {
                        perline[realindex].texts_dst = text_dst2.Text;
                        text_dst.Text = text_dst2.Text;
                    }
                }
            }
        }
        #endregion

        #region 列表同步、按钮控制等
        private void Text_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dictlist dictlist = new dictlist();

            if (text_list.Items.Count > 1)
            {
                string texts = "";

                //根据所选中的控件，对另一个隐藏的控件进行控制
                if ((sender as ListView).Name == "text_list" && (sender as ListView).SelectedItem != null)
                {
                    text_src.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts;
                    text_dst.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts_dst;
                    text_list2.SelectedIndex = (sender as ListView).SelectedIndex;
                    texts = text_src.Text;
                }
                else if ((sender as ListView).Name == "text_list2" && (sender as ListView).SelectedItem != null)
                {
                    text_src2.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts;
                    text_dst2.Text = ((kag_tools_shared.perlines)((Windows.UI.Xaml.Controls.Primitives.Selector)sender).SelectedItem).texts_dst;
                    text_list.SelectedIndex = (sender as ListView).SelectedIndex;
                    texts = text_src2.Text;
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

                //加载词典
                if (dicts != null)
                {
                    List<dictlist> filter_dict = dictlist.parse_filterdict(texts, dicts);
                    dicts1.ItemsSource = filter_dict;
                    dicts2.ItemsSource = filter_dict;
                }
                countword();
            }
        }
        #endregion
    }
}
