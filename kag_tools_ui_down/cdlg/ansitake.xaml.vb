' コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

Imports kag_tools_shared

Public NotInheritable Class ansitake
    Inherits ContentDialog

    Public Property src As Byte()
    Public Property cp As String

    Private Sub ContentDialog_PrimaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)

    End Sub

    ''' <summary>
    ''' 只截取前面 20 行内容，防止内容过多导致卡顿
    ''' </summary>
    ''' <param name="originaltext">文本内容</param>
    ''' <returns>截取后的内容</returns>
    Private Function shrinktext(originaltext As String) As String
        Dim shrinktexts As String = ""
        Dim lines20 As String() = originaltext.Split(vbCrLf)
        If lines20.Length > 20 Then
            Dim i As Integer
            For i = 0 To 19
                If i < 19 Then
                    shrinktexts = shrinktexts + lines20(i) + vbCrLf
                Else
                    shrinktexts = shrinktexts + lines20(i) + vbCrLf + "......" + vbCrLf + "预览到此结束"
                End If
            Next
            Return shrinktexts
        Else
            Return originaltext
        End If
    End Function

    Private Sub Codepage_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim cpe As String
        Dim previewtext As String
        Dim parse_bytes As New parse_bytes

        '手动选择字符编码
        Select Case codepage.SelectedIndex
            Case 1
                cpe = "gbk"
            Case 2
                cpe = "big5"
            Case 3
                cpe = "sjis"
            Case 4
                cpe = "euckr"
            Case Else
                cpe = ""
        End Select

        '预览显示效果
        If src IsNot Nothing Then
            previewtext = parse_bytes.byte2str(src, cpe)
            previewtext = shrinktext(previewtext)
            preview.Text = previewtext
            cp = cpe
        End If
    End Sub
End Class
