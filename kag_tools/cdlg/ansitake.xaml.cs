using kag_tools_shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace kag_tools.cdlg
{
    public sealed partial class ansitake : ContentDialog
    {
        public byte[] src { get; set; }
        public string cp { get; set; }

        public ansitake()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void Codepage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string cpe;
            string previewtext;
            parse_bytes parse_bytes = new parse_bytes();

            //手动选择字符编码
            switch(codepage.SelectedIndex)
            {
                case 1:
                    {
                        cpe = "gbk";
                        break;
                    }
                case 2:
                    {
                        cpe = "big5";
                        break;
                    }
                case 3:
                    {
                        cpe = "sjis";
                        break;
                    }
                case 4:
                    {
                        cpe = "euckr";
                        break;
                    }
                default:
                    {
                        cpe = "";
                        break;
                    }
            }

            //预览显示效果
            if (src != null)
            {
                previewtext = parse_bytes.byte2str(src, cpe);
                previewtext = shrinktext(previewtext);
                preview.Text = previewtext;
                cp = cpe;
            }
        }

        /// <summary>
        /// 只截取前面 20 行内容，防止内容过多导致卡顿
        /// </summary>
        /// <param name="originaltext">文本内容</param>
        /// <returns>截取后的内容</returns>
        private string shrinktext(string originaltext)
        {
            string shrinktext = "";
            string[] lines20 = originaltext.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (lines20.Length > 20)
            {

                for (int i = 0; i < 20; i++)
                {
                    if (i < 19)
                        shrinktext = shrinktext + lines20[i] + "\r\n";
                    else
                        shrinktext = shrinktext + lines20[i] + "\r\n......\r\n预览到此结束";
                }
                return shrinktext;
            }
            else
                return originaltext;
        }
    }
}
