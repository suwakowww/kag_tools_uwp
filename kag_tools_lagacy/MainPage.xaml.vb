Imports kag_tools_shared
Imports kag_tools_lagacy.nav_itemlist

' 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

''' <summary>
''' それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Private nav_items As List(Of nav_item) = get_navs()

    Private Sub Page_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        checkwidth()
    End Sub

    Private Sub checkwidth()
        If Window.Current.Bounds.Width <= 640 Then
            main_split.Margin = New Thickness(0, 48, 0, 0)
            main_split.DisplayMode = SplitViewDisplayMode.Overlay
            main_topbar.Visibility = Visibility.Visible
            main_split.IsPaneOpen = False
            If Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily <> "Windows.Mobile" Then
                'ShowTitleBar()
            End If
        ElseIf Window.Current.Bounds.Width <= 1024 Then
            main_split.Margin = New Thickness(0, 48, 0, 0)
            main_topbar.Visibility = Visibility.Visible
            main_split.DisplayMode = SplitViewDisplayMode.CompactInline
            If Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily <> "Windows.Mobile" Then
                'ShowTitleBar()
            End If
        Else
            main_split.Margin = New Thickness(0, 0, 0, 0)
            If Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily <> "Windows.Mobile" Then
                'ShowTitleBar()
            End If
            main_topbar.Visibility = Visibility.Collapsed
            main_split.DisplayMode = SplitViewDisplayMode.Inline
            main_split.IsPaneOpen = True
        End If

    End Sub

    Private Sub Main_nav_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selected_index As Integer = CType(sender, ListView).SelectedIndex
        main_frame.Navigate(nav_items(selected_index).nav_page)
        'int select_index = (sender as ListView).SelectedIndex;
        '    main_frame.Navigate(nav_items[select_index].nav_page);
    End Sub

    Private Sub Split_switch_Click(sender As Object, e As RoutedEventArgs)
        main_split.IsPaneOpen = Not main_split.IsPaneOpen
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        checkwidth()
        main_nav.ItemsSource = nav_items
        'main_nav.SelectedIndex = 0
    End Sub
End Class
