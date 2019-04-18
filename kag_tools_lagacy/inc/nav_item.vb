Imports kag_tools_shared

Public Class nav_itemlist


    Public Shared Function get_navs() As List(Of nav_item)
        Dim nav_items As New List(Of nav_item)()
        ',U+E10F / Home
        ',U+E70F / Edit
        nav_items.Add(New nav_item With {.nav_name = "开始", .nav_glyph = "", .nav_page = GetType(kag_tools_ui_down.Start)})
        nav_items.Add(New nav_item With {.nav_name = "文本编辑", .nav_glyph = "", .nav_page = GetType(kag_tools_ui_down.Edit_ks)})
        Return nav_items
    End Function

End Class
