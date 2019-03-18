using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kag_tools.inc
{
    class nav_item
    {
        public string nav_name { get; set; }
        public string nav_glyph { get; set; }
        public Type nav_page { get; set; }
    }

    class nav_itemlist
    {
        public static List<nav_item> get_navs()
        {
            List<nav_item> nav_items = new List<nav_item>();
            //,U+E10F / Home
            //,U+E70F / Edit
            nav_items.Add(new nav_item { nav_name = "开始", nav_glyph = "", nav_page = typeof(Start) });
            nav_items.Add(new nav_item { nav_name = "文本编辑", nav_glyph = "", nav_page = typeof(Edit_ks) });
            return nav_items;
        }
    }
}
