Imports System.Configuration
Imports System.IO
Imports System.Net
Imports HtmlAgilityPack
Public Class GatherWebFrm
    Dim th As Threading.Thread
    Dim threadMaxCount As Integer
    Dim threadCount As Integer
    Dim level As Integer
    Dim waitList As List(Of String)
    Dim doingList As List(Of String)
    Dim completeList As Dictionary(Of String, String)
    Dim filepath As String
    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


    '    'th = New Threading.Thread(New Threading.ParameterizedThreadStart(AddressOf excute))
    '    'Try
    '    '    th.Start(New Gather(Url, filepath, level))
    '    'Catch ex As Exception
    '    '    txt_Detail.AppendText(ex.Message & vbCrLf)
    '    'End Try


    '    'TextBox2.AppendText("complete: " & Url & vbCrLf)
    'End Sub

    Private Sub excute(task As Gather)
        Try
            task.ExcuteTask()
            doingList.Remove(task.Urlmain)
            completeList.Add(task.Urlmain, "complete")
            updateTxtComplete()
            'txt_Complete.AppendText(task.Urlmain & vbCrLf)
            'th.Abort()
        Catch ex As Exception
            txt_Detail.AppendText("Error:" & ex.Message)
            task.LogHelp.writeLog("Error:" & ex.Message)
            doingList.Remove(task.Urlmain)
            completeList.Add(task.Urlmain, "Error")
            updateTxtComplete()
            'txt_Complete.AppendText("Error:" & task.Urlmain & vbCrLf)
            th.Abort()
        End Try

    End Sub

    Private Sub updateTxtComplete()
        txt_Complete.Text = String.Empty
        For Each item As KeyValuePair(Of String, String) In completeList
            txt_Complete.AppendText(item.Value & " : " & item.Key & vbCrLf)
        Next

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        threadMaxCount = 6
        threadCount = 0
        waitList = New List(Of String)
        doingList = New List(Of String)
        completeList = New Dictionary(Of String, String)
        filepath = ConfigurationManager.AppSettings("filepath").ToString.Trim

        'Dim url As String = "https://web.archive.org/web/20060914111250/http://www.wulin.hk:80/"
        'WebBrowser1.Navigate(Url)
    End Sub

    Public Sub writeLog(ByVal text As String)
        txt_Detail.AppendText(text)
    End Sub

    Private Sub GatherWebFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        th.Abort()
        Dispose()
        Close()
    End Sub

    Private Sub btn_Add_Click(sender As Object, e As EventArgs) Handles btn_Add.Click
        'txt_Detail.AppendText("Start" & vbCrLf)
        level = NumericUpDown1.Value
        threadMaxCount = NumericUpDown2.Value
        For Each strWait As String In txt_Add.Text.Trim.Replace(vbCrLf, " ").Split(" ")
            If strWait.StartsWith("http") AndAlso Not waitList.Contains(strWait.Trim) _
                AndAlso Not doingList.Contains(strWait.Trim) AndAlso Not completeList.Keys.Contains(strWait.Trim) Then
                waitList.Add(strWait.Trim)
            End If
        Next
        updateTxtWaiting()
        txt_Add.Text = String.Empty
        If doingList.Count < threadMaxCount Then
            startWork()
        End If
        NumericUpDown1.Enabled = False
        NumericUpDown2.Enabled = False
    End Sub

    Private Sub startWork()
        While waitList.Count > 0 AndAlso doingList.Count < threadMaxCount
            doingList.Add(waitList(0))
            th = New Threading.Thread(New Threading.ParameterizedThreadStart(AddressOf excute))
            Try
                th.Start(New Gather(waitList(0), filepath, level))
            Catch ex As Exception
                txt_Detail.AppendText(ex.Message & vbCrLf)
            End Try

            waitList.RemoveAt(0)
        End While
        updateTxtDoing()
        updateTxtWaiting()
    End Sub

    Private Sub updateTxtDoing()
        txt_DoingList.Text = ""
        For Each str As String In doingList
            txt_DoingList.AppendText(str & vbCrLf)
        Next
    End Sub

    Private Sub updateTxtWaiting()
        txt_WaitList.Text = ""
        For Each str As String In waitList
            txt_WaitList.AppendText(str & vbCrLf)
        Next
    End Sub

    Private Sub txt_Complete_TextChanged(sender As Object, e As EventArgs) Handles txt_Complete.TextChanged
        startWork()
    End Sub

    Private Sub btn_retry_Click(sender As Object, e As EventArgs) Handles btn_retry.Click
        For Each item As KeyValuePair(Of String, String) In completeList
            If item.Value.Equals("Error") Then
                waitList.Add(item.Key)
                completeList.Remove(item.Key)
            End If
        Next
        updateTxtComplete()
    End Sub

    'Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
    '    If  Not WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
    '        Return
    '    End If
    '    Dim doc As System.Windows.Forms.HtmlDocument = WebBrowser1.Document
    '    Dim str = WebBrowser1.Document.Window.Frames("None").Document.Body.InnerHtml
    'End Sub
End Class
