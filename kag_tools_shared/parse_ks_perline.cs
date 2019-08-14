using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace kag_tools_shared
{
    public class ks_perlines
    {
        public int line_num { get; set; }
        public string texts { get; set; }
        public string texts_dst { get; set; }
        public char texttype { get; set; }
        public SolidColorBrush textcolor { get; set; }
        public Color text_cd { get; set; }
        public Color text_cl { get; set; }
    }

    public class parse_ks_perline
    {
        public List<ks_perlines> parsestr(string src)
        {
            List<ks_perlines> perlinetext = new List<ks_perlines>();

            //按行拆分
            string[] perlinetext2 = src.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            string tmp;
            char texttypes;
            Color tmp_cd,tmp_cl;

            for(int i=0;i<perlinetext2.Length;i++)
            {
                tmp = perlinetext2[i].Trim();

                //代码
                if (tmp.StartsWith("[") && tmp.EndsWith("]"))
                {
                    texttypes = 'c';
                    tmp_cl = Colors.Gray;
                    tmp_cd = Colors.DarkGray;
                }

                //其他内容
                else if (tmp.StartsWith("*") || tmp.StartsWith("@"))
                {
                    texttypes = 'o';
                    tmp_cl = Colors.Gray;
                    tmp_cd = Colors.DarkGray;
                }

                //注释
                else if (tmp.StartsWith(";"))
                {
                    texttypes = 'n';
                    tmp_cl = Colors.DarkGreen;
                    tmp_cd = Colors.Green;
                }

                //空行
                else if (tmp == "")
                {
                    texttypes = 'e';
                    tmp_cl = Colors.Gray;
                    tmp_cd = Colors.DarkGray;
                }

                //对话
                else if (tmp.StartsWith("【") && tmp.EndsWith("】"))
                {
                    texttypes = 's';
                    tmp_cl = Colors.DeepPink;
                    tmp_cd = Colors.HotPink;
                }

                //以上皆非，则为文本
                else
                {
                    texttypes = 't';
                    tmp_cl = Colors.Black;
                    tmp_cd = Colors.White;
                }

                //整理好之后，添加到一个列表中
                perlinetext.Add(new ks_perlines { line_num=i, texts = tmp, texts_dst = tmp, texttype = texttypes, text_cd = tmp_cd, text_cl = tmp_cl });
            }
            return perlinetext;
        }

        public List<ks_perlines> filter_perline(List<ks_perlines> perline)
        {
            List<ks_perlines> filtered = new List<ks_perlines>();
            for (int i = 0; i < perline.Count; i++)
            {
                if (perline[i].texttype == 's' || perline[i].texttype == 't')
                    filtered.Add(perline[i]);
            }
            return filtered;
        }
    }
}
