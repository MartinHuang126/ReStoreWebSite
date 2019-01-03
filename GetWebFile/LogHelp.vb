Imports System.IO
Public Class LogHelp
    Private _LogPath As String

    Public Property LogPath As String
        Get
            Return _LogPath
        End Get
        Set
            _LogPath = Value
        End Set
    End Property

    Public Sub New(ByVal logPath As String)
        _LogPath = logPath
        If Not IO.Directory.Exists(logPath) Then IO.Directory.CreateDirectory(logPath)

    End Sub
    Public Sub writeLog(ByVal text As String)
        Try
            File.AppendAllText(Path.Combine(LogPath, "loginfo.txt"), text, System.Text.Encoding.UTF8)
        Catch ex As Exception
        End Try
    End Sub
End Class
