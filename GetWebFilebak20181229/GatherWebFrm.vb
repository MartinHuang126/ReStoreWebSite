Imports System.Configuration
Imports System.IO
Imports System.Net
Imports HtmlAgilityPack
Public Class GatherWebFrm
    Dim th As Threading.Thread
    Dim threadMaxCount As Integer
    Dim threadCount As Integer
    Dim waitList As List(Of String)
    Dim doingList As List(Of String)
    Dim completeList As List(Of String)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox2.AppendText("Start" & vbCrLf)
        Dim Url As String = TextBox1.Text.Trim
        Dim filepath = ConfigurationManager.AppSettings("filepath").ToString.Trim
        Dim level = NumericUpDown1.Value

        th = New Threading.Thread(New Threading.ParameterizedThreadStart(AddressOf excute))
        Try

            th.Start(New Gather(Url, filepath, level))
        Catch ex As Exception
            TextBox2.AppendText(ex.Message & vbCrLf)
        End Try


        'TextBox2.AppendText("complete: " & Url & vbCrLf)
    End Sub

    Private Sub excute(task As Gather)
        task.ExcuteTask()
        th.Abort()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        threadMaxCount = 6
        threadCount = 0
        waitList = New List(Of String)
        doingList = New List(Of String)
        completeList = New List(Of String)
        'Dim url As String = "https://web.archive.org/web/20060914111250/http://www.wulin.hk:80/"
        'WebBrowser1.Navigate(Url)
    End Sub

    Public Sub writeLog(ByVal text As String)
        TextBox2.AppendText(text)
    End Sub

    Private Sub GatherWebFrm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Me.Dispose()
        Me.Close()
    End Sub

    'Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
    '    If  Not WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
    '        Return
    '    End If
    '    Dim doc As System.Windows.Forms.HtmlDocument = WebBrowser1.Document
    '    Dim str = WebBrowser1.Document.Window.Frames("None").Document.Body.InnerHtml
    'End Sub
End Class
