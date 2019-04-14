using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kag_tools_shared
{
    public class dictlist
    {
        public string src { get; set; }
        public string spell { get; set; }
        public string types { get; set; }
        public string dst { get; set; }

        public List<dictlist> parse_csvdict(string src)
        {
            List<dictlist> dictlists = new List<dictlist>();
            string[] perline = src.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach(string perline2 in perline)
            {
                string[] perline3 = perline2.Split(new string[] { "," }, StringSplitOptions.None);
                dictlists.Add(new dictlist { src = perline3[0], spell = perline3[1], types = perline3[2], dst = perline3[3] });
            }
            return dictlists;
        }

        public List<dictlist> parse_filterdict(string src, List<dictlist> srcdicts)
        {
            List<dictlist> dictlists = new List<dictlist>();
            for (int i = 0; i < srcdicts.Count; i++)
            {
                if (src.Contains(srcdicts[i].src) && srcdicts[i].src != null)
                {
                    dictlists.Add(srcdicts[i]);
                }
            }
            return dictlists;
        }
    }

}
