﻿Imports System.IO
Public Class SEOLinksFrm

    Public Shared fileList As New List(Of String)
    Public itemReplaceStr As String = String.Empty
    Public ReplaceStr As String = String.Empty
    Private reg As New System.Text.RegularExpressions.Regex("<!--begain seo for link-->(.|\n)*?<!--end seo for link-->")
    Dim th As Threading.Thread
    Private Sub btn_choose_Click(sender As Object, e As EventArgs) Handles btn_choose.Click
        FolderBrowserDialog1.ShowDialog()
        txt_folderPath.Text = FolderBrowserDialog1.SelectedPath
    End Sub

    Private Sub SEOLinksFrm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        rdo_allFolder.Checked = True
    End Sub

    Private Sub btn_select_Click(sender As Object, e As EventArgs) Handles btn_select.Click
        txt_replace.Text = "Pls wait..."
        btn_select.Enabled = False
        fileList.Clear()
        th = New Threading.Thread(New Threading.ThreadStart(AddressOf SelectItemReplace))
        th.Start()

    End Sub

    Private Sub SelectItemReplace()
        getFiles(txt_folderPath.Text)
        itemReplaceStr = String.Empty
        Dim html As String = String.Empty
        If fileList.Count > 0 Then
            For Each str As String In fileList
                html = File.ReadAllText(str, System.Text.Encoding.UTF8)
                If reg.IsMatch(html) Then
                    txt_replace.Text = reg.Match(html).ToString
                    itemReplaceStr = reg.Match(html).ToString
                    btn_select.Enabled = True
                    th.Abort()
                    Return
                End If
            Next
            If String.IsNullOrEmpty(itemReplaceStr) Then
                txt_replace.Text = "html files no this tag<!--begain seo for link--><!--end seo for link-->"
            End If

        Else
            txt_replace.Text = "No html files"
        End If
        btn_select.Enabled = True
        th.Abort()
    End Sub

    Private Sub getFiles(ByVal path As String)
        If String.IsNullOrEmpty(path) Then
            Return
        End If
        Dim nextFloderPath As New List(Of String)
        nextFloderPath.AddRange(Directory.GetDirectories(path))
        fileList.AddRange(Directory.GetFiles(path, "*.html"))
        If rdo_current.Checked Then
            Return
        Else
            For Each nextPath In nextFloderPath
                getFiles(nextPath)
            Next

        End If
    End Sub

    Private Sub btn_update_Click(sender As Object, e As EventArgs) Handles btn_update.Click
        If Not txt_replace.Text.Trim.Contains("<!--begain seo for link-->") OrElse Not txt_replace.Text.Contains("<!--end seo for link-->") Then
            txt_replace.Text = "Pls add content in this tag<!--begain seo for link--><!--end seo for link-->"
            btn_update.Enabled = True
            Return
        End If
        ReplaceStr = txt_replace.Text.Trim
        txt_replace.Text = "Pls wait..."
        btn_update.Enabled = False
        th = New Threading.Thread(New Threading.ThreadStart(AddressOf UpdateHtml))
        th.Start()

    End Sub

    Private Sub UpdateHtml()
        Dim html As String = String.Empty
        If fileList.Count = 0 OrElse txt_replace.Text.Equals("html files no this tag<!--begain seo for link--><!--end seo for link-->") Then
            txt_replace.Text = "pls select first"
            btn_update.Enabled = True
            th.Abort()
            Return
        End If

        Try
            For Each filepath In fileList
                html = File.ReadAllText(filepath, System.Text.Encoding.UTF8)
                html = html.Replace(itemReplaceStr, ReplaceStr)
                File.WriteAllText(filepath, html, System.Text.Encoding.UTF8)
            Next
            txt_replace.Text = "Complete!"
        Catch ex As Exception
            txt_replace.Text = "error!"
        End Try
        btn_update.Enabled = True
        th.Abort()
    End Sub

    Private Sub btn_addTag_Click(sender As Object, e As EventArgs) Handles btn_addTag.Click
        If fileList.Count < 1 Then
            txt_replace.Text = "No html files..."
            Return
        End If
        txt_replace.Text = "Pls wait..."
        btn_addTag.Enabled = False
        th = New Threading.Thread(New Threading.ThreadStart(AddressOf addTag))
        th.Start()
    End Sub

    Private Sub addTag()
        Dim str As String = String.Empty
        For Each item As String In fileList
            str = File.ReadAllText(item)
            If reg.IsMatch(str) Then
                Continue For
            End If
            str = str.Replace("</body>", "<!--begain seo for link--><div></div><!--end seo for link--></body>")
            File.WriteAllText(item,str)
        Next
        txt_replace.Text = "Complete!"
        btn_addTag.Enabled = True
    End Sub
End Class