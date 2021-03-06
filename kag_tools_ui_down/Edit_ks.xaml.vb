﻿' 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

Imports kag_tools_shared
Imports Windows.System
''' <summary>
''' それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
''' </summary>
Public NotInheritable Class Edit_ks
    Inherits Page

    Dim ks_perline As List(Of ks_perlines)
    Dim bgi_perline As List(Of bgi_perlines)
    Dim dicts As List(Of dictlist)
    Dim textonly As Boolean = True
    Dim filter_ks_perline As List(Of ks_perlines)

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        checkwidth(Window.Current.Bounds.Width, True)
    End Sub

#Region "检测宽度"
    ''' <summary>
    ''' 检测宽度
    ''' </summary>
    ''' <param name="size">获取宽度的变量</param>
    ''' <param name="iswinsize">如果为 true，则作为窗口大小识别，反之为页面大小识别。</param>
    Private Sub checkwidth(size As Double, iswinsize As Boolean)
        Dim page_size As Integer = CInt(size)
        If iswinsize = True Then

        Else
            If (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily = "Windows.Mobile") Then
                If page_size < 640 Then
                    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom
                    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom
                    large_view.Visibility = Visibility.Collapsed
                    small_view.Visibility = Visibility.Visible
                Else
                    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right
                    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right
                    large_view.Visibility = Visibility.Visible
                    small_view.Visibility = Visibility.Collapsed
                End If
            Else
                If page_size < 640 Then
                    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom
                    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Bottom
                    large_view.Visibility = Visibility.Collapsed
                    small_view.Visibility = Visibility.Visible
                Else
                    bot_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right
                    top_cmdbar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right
                    large_view.Visibility = Visibility.Visible
                    small_view.Visibility = Visibility.Collapsed
                End If
            End If
        End If
    End Sub

#End Region

    '系统版本检测似乎不需要，这里不放了

#Region "打开 .ks 文件"
    Private Async Sub Import_ks_Click(sender As Object, e As RoutedEventArgs)
        Dim loadsave As New loadsave()
        Dim parse_bytes As New parse_bytes()

        '打开文件
        Dim files As files = Await loadsave.load_ksasync()

        If files.filename <> "empty" AndAlso files.srcode.Count() <> 0 Then
            Dim encoding As String = parse_bytes.DetectUnicode(files.srcode)
            Dim src2 As String

            'ANSI 处理
            If encoding = "ansi" Then
                Dim ansitake_dlg As New ansitake()
                ansitake_dlg.src = files.srcode
                Await ansitake_dlg.ShowAsync()
                encoding = ansitake_dlg.cp
            End If

            '还原二进制为文本
            src2 = parse_bytes.byte2str(files.srcode, encoding)

            '判断 .ks 文件类型
            If files.filename.Contains(".ks") Then
                Dim parse_ks_perline As New parse_ks_perline()
                ks_perline = parse_ks_perline.parsestr(src2)

                '按行拆分文本
                For i = 0 To ks_perline.Count - 1
                    '根据每行内容，进行上色
                    '由于 Me.RequestedTheme 会返回 ElementTheme.Default，故原方法不可用
                    If Application.Current.RequestedTheme = ApplicationTheme.Dark Then
                        ks_perline(i).textcolor = New SolidColorBrush(ks_perline(i).text_cd)
                    Else
                        ks_perline(i).textcolor = New SolidColorBrush(ks_perline(i).text_cl)
                    End If
                Next

                file_info.Text = files.filename

                If textonly = True Then
                    '过滤代码等内容，只留下文本
                    filter_ks_perline = parse_ks_perline.filter_perline(ks_perline)
                    text_list.ItemsSource = filter_ks_perline
                    text_list2.ItemsSource = filter_ks_perline
                Else
                    '原样输出
                    text_list.ItemsSource = ks_perline
                    text_list2.ItemsSource = ks_perline
                End If

                bot_p_n.IsEnabled = True
                bot_p_n2.IsEnabled = True
                text_src.IsEnabled = True
                text_src2.IsEnabled = True
                text_dst.IsEnabled = True
                text_dst2.IsEnabled = True
            ElseIf files.filename.Contains(".out.txt") Then
                Dim parse_bgi_perline As New parse_bgi_perline()
                bgi_perline = parse_bgi_perline.parsestr(src2)

                file_info.Text = files.filename

                text_list.ItemsSource = bgi_perline
                text_list2.ItemsSource = bgi_perline

                Dim i As Integer
                For i = 0 To bgi_perline.Count - 1
                    If Application.Current.RequestedTheme = ApplicationTheme.Dark Then
                        bgi_perline(i).textcolor = New SolidColorBrush(bgi_perline(i).text_cd)
                    Else
                        bgi_perline(i).textcolor = New SolidColorBrush(bgi_perline(i).text_cl)
                    End If
                Next

                bot_p_n.IsEnabled = True
                bot_p_n2.IsEnabled = True
                text_src.IsEnabled = True
                text_src2.IsEnabled = True
                text_dst.IsEnabled = True
                text_dst2.IsEnabled = True
            Else
                '如果打开文件的过程被中断（比如：按下了“取消”键），会返回“empty”。
                '由于发现打开空白文件的时候会发生错误，所以这里追加判断打开文件的情况。
                Dim err_msg As String = If(files.filename = "empty", "没有打开文件。", String.Format("打开了空白的文件：{0}。", files.filename))

                Dim err_dlg As New ContentDialog With
                    {
                    .Title = "发生错误",
                    .Content = err_msg,
                    .PrimaryButtonText = "关闭"
                    }
                Await err_dlg.ShowAsync()
            End If
        End If
    End Sub
#End Region

#Region "保存 .ks 文件"
    Private Async Sub Output_ks_Click(sender As Object, e As RoutedEventArgs)
        Dim loadsave As New loadsave()
        Dim savetext As New List(Of String)
        If (file_info.Text.Contains(".ks")) Then
            Dim i As Integer
            For i = 0 To ks_perline.Count() - 1
                savetext.Add(ks_perline(i).texts_dst)
            Next
        ElseIf (file_info.Text.Contains(".out.txt")) Then
            Dim j As Integer
            For i = 0 To bgi_perline.Count() - 1
                savetext.Add(String.Format("{0}{1}", bgi_perline(i).text_address, bgi_perline(i).texts_dst))
            Next
        End If
        Dim status As String = Await loadsave.save_ksasync(savetext)
        Dim result As New ContentDialog() With
            {
            .Title = "结果",
            .Content = status,
            .PrimaryButtonText = "关闭"
            }
        Await result.ShowAsync()
    End Sub
#End Region

#Region "加载词典"
    Private Async Sub Load_dict_Click(sender As Object, e As RoutedEventArgs)
        Dim loaddict As New loaddict()
        dicts = Await loaddict.loaddictasync()
    End Sub
#End Region

#Region "机器翻译"
    Private Async Sub Tr_Click(sender As Object, e As RoutedEventArgs)
        If CType(sender, AppBarButton).Name = "t_bai" OrElse CType(sender, AppBarButton).Name = "t_bai2" Then
            Await Launcher.LaunchUriAsync(New Uri("http://fanyi.baidu.com/#jp/zh/" + text_src.Text))
        ElseIf CType(sender, AppBarButton).Name = "t_goo" OrElse CType(sender, AppBarButton).Name = "t_goo2" Then
            Await Launcher.LaunchUriAsync(New Uri("https://translate.google.com/#ja/zh-CN/" + text_src.Text))
        Else
            Await Launcher.LaunchUriAsync(New Uri("https://translate.google.cn/#ja/zh-CN/" + text_src.Text))
        End If
    End Sub
#End Region

#Region "检测屏幕宽度，debug 用"
    Private Sub Page_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim debug_mode As Boolean = True
        checkwidth(CType(sender, Page).ActualWidth, False)
        If debug_mode = True Then
            win_page_size.Text = String.Format("窗口：{0} 像素，页面：{1} 像素", CInt(Window.Current.Bounds.Width.ToString()), CInt(CType(sender, Page).ActualWidth).ToString())
        Else
            win_page_size.Text = ""
        End If
        win_page_size2.Text = win_page_size.Text
    End Sub
#End Region

#Region "上/下导航按钮控制"
    Private Sub Bot_p_n_Click(sender As Object, e As RoutedEventArgs)
        If CType(sender, AppBarButton).Name = "bot_p_n" OrElse CType(sender, AppBarButton).Name = "bot_p_n2" Then
            bot_p_p.IsEnabled = True
            bot_p_p2.IsEnabled = True
            If large_view.Visibility = Visibility.Visible Then
                While True
                    text_list.SelectedIndex = text_list.SelectedIndex + 1
                    If (file_info.Text.Contains(".ks")) Then
                        If TryCast(text_list.SelectedItem, kag_tools_shared.ks_perlines).texttype = "t" OrElse TryCast(text_list.SelectedItem, kag_tools_shared.ks_perlines).texttype = "s" OrElse text_list.SelectedIndex = text_list.Items.Count - 1 Then
                            Exit While
                        End If
                    ElseIf (file_info.Text.Contains("out.txt")) Then
                        If TryCast(text_list.SelectedItem, kag_tools_shared.bgi_perlines).texttype = "t" OrElse text_list.SelectedIndex = text_list.Items.Count - 1 Then
                            Exit While
                        End If
                    Else
                        Exit while
                    End If
                End While
                If text_list.SelectedIndex >= text_list.Items.Count - 1 Then
                    bot_p_n.IsEnabled = False
                    bot_p_n2.IsEnabled = False
                End If
            Else
                While True
                    text_list2.SelectedIndex = text_list2.SelectedIndex + 1
                    If (file_info.Text.Contains(".ks")) Then
                        If CType(text_list2.SelectedItem, kag_tools_shared.ks_perlines).texttype = "t" OrElse CType(text_list2.SelectedItem, kag_tools_shared.ks_perlines).texttype = "s" OrElse text_list2.Items.Count - 1 Then
                            Exit While
                        End If
                    ElseIf (file_info.Text.Contains(".out.txt")) Then
                        If CType(text_list2.SelectedItem, kag_tools_shared.bgi_perlines).texttype = "t" OrElse text_list2.Items.Count - 1 Then
                            Exit While
                        End If
                    Else
                        Exit While
                    End If
                End While
                If text_list2.SelectedIndex >= text_list2.Items.Count - 1 Then
                    bot_p_n.IsEnabled = False
                    bot_p_n2.IsEnabled = False
                End If
            End If
        Else
            bot_p_n.IsEnabled = True
            bot_p_n2.IsEnabled = True
            If large_view.Visibility = Visibility.Visible Then
                While True
                    text_list.SelectedIndex = text_list.SelectedIndex - 1
                    If (file_info.Text.Contains(".ks")) Then
                        If CType(text_list.SelectedItem, kag_tools_shared.ks_perlines).texttype = "t" OrElse CType(text_list.SelectedItem, kag_tools_shared.ks_perlines).texttype = "s" OrElse text_list.SelectedIndex <= 0 Then
                            Exit While
                        End If
                    ElseIf (file_info.text.contains(".out.txt")) Then
                        If CType(text_list.SelectedItem, kag_tools_shared.bgi_perlines).texttype = "t" OrElse text_list.SelectedIndex <= 0 Then
                            Exit While
                        End If
                    Else
                        Exit While
                    End if
                End While
                If text_list.SelectedIndex <= 0 Then
                    bot_p_p.IsEnabled = False
                    bot_p_p2.IsEnabled = False
                End If
            Else
                While True
                    text_list2.SelectedIndex = text_list2.SelectedIndex - 1
                    If (file_info.Text.Contains(".ks")) Then
                        If CType(text_list2.SelectedItem, kag_tools_shared.ks_perlines).texttype = "t" OrElse CType(text_list2.SelectedItem, kag_tools_shared.ks_perlines).texttype = "s" OrElse text_list2.SelectedIndex <= 0 Then
                            Exit While
                        End If
                    ElseIf (file_info.Text.Contains("out.txt")) Then
                        If CType(text_list2.SelectedItem, kag_tools_shared.bgi_perlines).texttype = "t" OrElse text_list2.SelectedIndex <= 0 Then
                            Exit While
                        End If
                    Else
                        Exit While
                    End If
                End While
                If text_list2.SelectedIndex <= 0 Then
                    bot_p_p.IsEnabled = False
                    bot_p_p2.IsEnabled = False
                End If
            End If
        End If
        countword(sender)
    End Sub


#End Region

#Region "字数统计"
    Private Sub countword(ByVal sender As Object)
        '判断sender的类型
        If sender.GetType() Is GetType(TextBox) Then
            '定义一个notsender，作为另外的一个控件。
            Dim notsender As Object
            If CType(sender, TextBox).Name = "text_dst" Then
                notsender = text_dst2
                win_page_size.Text = String.Format("原文：{0} 字，译文：{1} 字", text_src.Text.Count(), text_dst.Text.Count())
                win_page_size2.Text = win_page_size.Text
                CType(notsender, TextBox).Text = CType(sender, TextBox).Text
            ElseIf CType(sender, TextBox).Name = "text_dst2" Then
                notsender = text_dst
                win_page_size2.Text = String.Format("原文：{0} 字，译文：{1} 字", text_src2.Text.Count(), text_dst2.Text.Count())
                win_page_size.Text = win_page_size2.Text
                CType(notsender, TextBox).Text = CType(sender, TextBox).Text
            End If
        End If
    End Sub
#End Region

#Region "写入修改到列表"
    Private Sub Text_dst_TextChanged(ByVal sender As Object, e As TextChangedEventArgs)
        'TextChanged 本来为检测到文本内容变化时触发
        '但由于有个叫做输入法的东西，导致触发过于频繁
        '于是转移写入修改的方法了。
        countword(sender)
    End Sub

    '由于频繁地修改 List 可能会导致崩溃，于是将写入修改改为按回车键触发。
    Private Sub Text_dst_KeyUp(sender As Object, e As KeyRoutedEventArgs)
        If CType(sender, TextBox).Text <> "" AndAlso e.Key.ToString() = "Enter" Then
            '由于引入了“仅显示文本”，这里需要对索引值重新定位
            Dim realindex As Integer
            If (file_info.Text.Contains(".ks")) Then
                If textonly = True Then
                    realindex = filter_ks_perline(text_list.SelectedIndex).line_num
                Else
                    realindex = ks_perline(text_list.SelectedIndex).line_num
                End If
                If CType(sender, TextBox).Name = "text_dst" Then
                    ks_perline(realindex).texts_dst = text_dst.Text
                    text_dst2.Text = text_dst.Text
                Else
                    ks_perline(realindex).texts_dst = text_dst2.Text
                    text_dst.Text = text_dst2.Text
                End If
            ElseIf (file_info.Text.Contains(".out.txt")) Then
                realindex = bgi_perline(text_list.SelectedIndex).line_num
                If CType(sender, TextBox).Name = "text_dst" Then
                    bgi_perline(realindex).texts_dst = text_dst.Text
                    text_dst2.Text = text_dst.Text
                Else
                    bgi_perline(realindex).texts_dst = text_dst2.Text
                    text_dst.Text = text_dst2.Text
                End If
            End If

            '写入修改后，自动跳转到下一行文本
            If text_list.SelectedIndex <> text_list.Items.Count - 1 Then
                    text_list.SelectedIndex = text_list.SelectedIndex + 1
                End If
            End If
    End Sub
#End Region

#Region "列表同步、按钮控制等"
    Private Sub Text_list_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim dictlist As New dictlist()

        If text_list.Items.Count > 1 Then
            Dim texts As String = ""

            '根据所选中的控件，对另一个隐藏的空间进行控制
            If CType(sender, ListView).Name = "text_list" AndAlso CType(sender, ListView).SelectedItem IsNot Nothing Then
                If (file_info.Text.Contains(".ks")) Then
                    text_src.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.ks_perlines).texts
                    text_dst.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.ks_perlines).texts_dst
                ElseIf (file_info.Text.Contains(".out.txt")) Then
                    text_src.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.bgi_perlines).texts
                    text_dst.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.bgi_perlines).texts_dst
                End If
                text_list2.SelectedIndex = CType(sender, ListView).SelectedIndex
                texts = text_src.Text
            ElseIf CType(sender, ListView).Name = "text_list2" AndAlso CType(sender, ListView).SelectedItem IsNot Nothing Then
                If (file_info.Text.Contains(".ks")) Then
                    text_src2.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.ks_perlines).texts
                    text_dst2.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.ks_perlines).texts_dst
                ElseIf (file_info.Text.Contains("out.txt")) Then
                    text_src2.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.bgi_perlines).texts
                    text_dst2.Text = TryCast(TryCast(sender, Windows.UI.Xaml.Controls.Primitives.Selector).SelectedItem, kag_tools_shared.bgi_perlines).texts_dst
                End If
                text_list.SelectedIndex = CType(sender, ListView).SelectedIndex
                texts = text_src2.Text
            End If

                '防止下标越界
                If CType(sender, ListView).SelectedIndex = 0 Then
                bot_p_n.IsEnabled = True
                bot_p_n2.IsEnabled = True
                bot_p_p.IsEnabled = False
                bot_p_p2.IsEnabled = False
            ElseIf CType(sender, listview).SelectedIndex = CType(sender, listview).Items.Count - 1 Then
                bot_p_n.IsEnabled = False
                bot_p_n2.IsEnabled = False
                bot_p_p.IsEnabled = True
                bot_p_p2.IsEnabled = True
            Else
                bot_p_n.IsEnabled = True
                bot_p_n2.IsEnabled = True
                bot_p_p.IsEnabled = True
                bot_p_p2.IsEnabled = True
            End If

            '加载词典
            If dicts IsNot Nothing Then
                Dim filter_dict As List(Of dictlist) = dictlist.parse_filterdict(texts, dicts)
                dicts1.ItemsSource = filter_dict
                dicts2.ItemsSource = filter_dict
            End If
            countword(sender)
        End If
    End Sub
#End Region


End Class
