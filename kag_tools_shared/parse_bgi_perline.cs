using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace kag_tools_shared
{
    public class bgi_perlines
    {
        public int line_num { get; set; }
        public string text_address { get; set; }
        public string texts { get; set; }
        public string texts_dst { get; set; }
        public char texttype { get; set; }
        public SolidColorBrush textcolor { get; set; }
        public Color text_cd { get; set; }
        public Color text_cl { get; set; }
    }

    public class parse_bgi_perline
    {
        public List<bgi_perlines> parsestr(string src)
        {
            List<bgi_perlines> perlinetext = new List<bgi_perlines>();

            string[] perlinetext2 = src.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string tmptext,tmpaddress;
            char texttypes;
            Color tmp_cd, tmp_cl;
            for (int i=0;i<=perlinetext2.Length-1;i++)
            {
                tmpaddress = Regex.Replace(perlinetext2[i], "(<[0-9]+>).+","$1",RegexOptions.IgnoreCase);
                tmptext = Regex.Replace(perlinetext2[i], "<[0-9]+>(.+)", "$1", RegexOptions.IgnoreCase);
                if (tmptext.Contains("//"))
                {
                    texttypes = 'n';
                    tmp_cl = Colors.DarkGreen;
                    tmp_cd = Colors.Green;
                }
                else if (tmptext == "")
                {
                    texttypes = 'e';
                    tmp_cl = Colors.Gray;
                    tmp_cd = Colors.DarkGray;
                }
                else
                {
                    texttypes = 't';
                    tmp_cl = Colors.Black;
                    tmp_cd = Colors.White;
                }
                perlinetext.Add(new bgi_perlines { line_num = i, text_address = tmpaddress, texts = tmptext, texts_dst = tmptext, texttype = texttypes, text_cd = tmp_cd, text_cl = tmp_cl });
            }

            return perlinetext;
        }
    }
}
