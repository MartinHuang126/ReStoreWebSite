Imports System.IO
Imports System.Net
Imports HtmlAgilityPack
Public Class Gather

    Private regImg As New System.Text.RegularExpressions.Regex(".*\.(jpg|png|gif|ico|bpm)")
    Private regJslink As New System.Text.RegularExpressions.Regex("\('*.*\.(jsp|html|htm|asp|php)")
    Private regJs As New System.Text.RegularExpressions.Regex(".*\.js")
    Private regCss As New System.Text.RegularExpressions.Regex(".*\.css")
    Private regSwf As New System.Text.RegularExpressions.Regex(".*\.swf")
    Private regpdf As New System.Text.RegularExpressions.Regex(".*\.pdf$")
    'Private WebBrowser1 As New WebBrowser
    'Friend WithEvents WebBrowser1 As WebBrowser
    Public ExisturlList As List(Of String) = New List(Of String) '已经爬到过的url
    Public CurrentUrlList As List(Of String) = New List(Of String) '当前层次的urllist
    Public NextUrlList As List(Of String) = New List(Of String) '下一层次的

    Public postDoc As New HtmlDocument()
    Public htmlNodeList As HtmlNodeCollection
    Public client As New WebClient

    Public Delegate Sub WriteToText(ByVal text As String) '写全部log到窗体的委托
    Private writeToForm As WriteToText
    Private _Filepath As String

    Public Property LogHelp As LogHelp
    Public Property TatolLevel As Integer
    Public Property IndexPath As String
    Public Property Urlmain As String
    Public Property UrlStart As String
    Public Property UrlEnd As String
    Public Property UrlStartjs As String
    Public Property UrlStartim As String
    Public Property UrlStartcs As String
    Public Property UrlStartoe As String
    Public Property SiteName As String
    Public Property Filepath As String
        Get
            Return _Filepath
        End Get
        Set
            _Filepath = Value
        End Set
    End Property

    Public Property Imgpath As String

    Public Sub New(ByVal Url As String, ByVal filepath As String, level As Integer)
        _Filepath = filepath
        Urlmain = Url
        TatolLevel = level
        CurrentUrlList.Clear()
        NextUrlList.Clear()
        ExisturlList.Clear()
        Dim index As Integer = Url.IndexOf("/http")
        If (index > 0) Then
            UrlStart = Url.Substring(0, index)
            UrlStartjs = UrlStart & "js_/"
            UrlStartim = UrlStart & "im_/"
            UrlStartcs = UrlStart & "cs_/"
            UrlStartoe = UrlStart & "oe_/"
            UrlEnd = Url.Substring(index + 1)
            If UrlEnd.Contains(".php") OrElse UrlEnd.Contains(".html") OrElse UrlEnd.Contains(".asp") _
                OrElse UrlEnd.Contains(".jsp") OrElse UrlEnd.Contains(".htm") Then
                UrlEnd = UrlEnd.Substring(0, UrlEnd.LastIndexOf("/"))
            End If
        End If
        UrlEnd = UrlEnd.Replace(":80", "").TrimEnd("/")
        SiteName = GetWebSite(UrlEnd) '主文件夹名
        IndexPath = filepath & "/" & SiteName & "/index.html"
        LogHelp = New LogHelp(Path.Combine(filepath, SiteName)) '初始化log对象
        writeToForm = AddressOf GatherWebFrm.writeLog '绑定委托方法
        '获取通用的url重写配置
        If Not IO.Directory.Exists(LogHelp.LogPath) Then IO.Directory.CreateDirectory(LogHelp.LogPath)
        If Not File.Exists(Path.Combine(filepath, SiteName, "web.config")) Then
            File.Copy("web.config", Path.Combine(filepath, SiteName, "web.config"))
        End If
        'WebBrowser1 = New WebBrowser()
    End Sub

    Public Function ExcuteTask()
        'DownloadFile(Urlmain, IndexPath)
        CurrentUrlList.Add(Urlmain)
        saveAllFile()
        writeToForm("Complete: " & Urlmain & vbCrLf)
        LogHelp.writeLog("Complete: " & Urlmain & vbCrLf)
    End Function

    Public Sub DownloadFile(ByVal url As String, ByVal fullpath As String)
        writeToForm("star: " & url & vbCrLf)
        LogHelp.writeLog("star: " & url & vbCrLf)
        Dim strHtml As String = GetMainPageHtml(url)
        If Not ExisturlList.Contains(url) Then
            ExisturlList.Add(url)
        End If
        If strHtml.Contains("404 Not Found") OrElse
                strHtml.Contains("Your generous donation will be matched 2-to-1 right now. Your $5 becomes $15!") Then
            Throw New Exception
        End If
        strHtml = strHtml.Replace(UrlStart, "A_A_A_A")
        strHtml = strHtml.Replace(UrlStart.Substring(UrlStart.IndexOf("/web/")), UrlStart)
        strHtml = strHtml.Replace("A_A_A_A", UrlStart)
        strHtml = strHtml.Replace(SiteName & ":80", SiteName)
        strHtml = strHtml.Replace("<fb:like action=""like"" show_faces=""false"" width=""140""></fb:like>", "")
        '获取页面所有链接放在UrlList

        GetUrlList(strHtml)
        If fullpath.Contains(".css") Then
            Try

                GetUrlListcss(strHtml, UrlStartim)
            Catch ex As Exception
                LogHelp.writeLog("GetUrlListcss error URL: " & url & ",filePath: " & fullpath & vbCrLf)
            End Try
            Dim reg As New System.Text.RegularExpressions.Regex("/web/.*_/" + UrlEnd.TrimEnd("/"))
            strHtml = reg.Replace(strHtml, "")
        End If
        If fullpath.Contains("pdf") AndAlso fullpath.Contains(".html") Then

            GetUrlListPdf(strHtml)

        End If
        strHtml = strHtml.Replace(UrlStartjs, "").Replace(UrlStartim, "").Replace(UrlStartoe, "").Replace(UrlStartcs, "").Replace(UrlStart, "").Replace(UrlEnd.TrimEnd("/"), "").Replace(UrlEnd.Replace("www.", ""), "").Replace("https://web.archive.org", "")
        Dim fileDirectory As String = fullpath.Substring(0, fullpath.LastIndexOf("/"))
        '下载首页
        If Not IO.Directory.Exists(fileDirectory) Then IO.Directory.CreateDirectory(fileDirectory)
        'fullpath = fullpath.Substring(0, fullpath.LastIndexOf("/")) & fullpath.Substring(fullpath.LastIndexOf("/")).Replace(".php", "html")
        Try
            If File.Exists(fullpath) Then
                LogHelp.writeLog("jump a file: " & fullpath & " URL: " & url & vbCrLf)
                writeToForm("jump a file: " & fullpath & " URL: " & url & vbCrLf)
                Return
            End If
            File.WriteAllText(fullpath, strHtml, System.Text.Encoding.UTF8)
        Catch ex As Exception
            LogHelp.writeLog("writeFileError: " & fullpath & " , url: " & url & vbCrLf)
        End Try
        writeToForm("done one html file: " & url & vbCrLf)
        LogHelp.writeLog("done one html file: " & url & vbCrLf)
    End Sub

    Public Sub saveAllFile()
        Dim indexhttp As Integer = 0
        Dim imgpath As String = String.Empty
        Dim indexend As Integer = 0
        Dim imgDirectory As String = String.Empty
        Dim imgName As String = String.Empty
        Dim phyimgDirectory As String = String.Empty
        Dim level As Integer = 0
        While CurrentUrlList.Count > 0 OrElse level <= TatolLevel

            For Each Uri In CurrentUrlList
                If Uri.StartsWith("/web/") Then
                    Uri = "https://web.archive.org" & Uri
                End If
                If Not Uri.Contains("web.archive.org") OrElse Not Uri.Contains(SiteName) Then
                    Continue For
                End If
                If regImg.IsMatch(Uri) Then 'Uri.Contains(UrlStartim) Then '下载图片
                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)

                    indexend = imgpath.LastIndexOf("/")
                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1)
                    If imgName.Contains("?") Then
                        imgName = imgName.Substring(0, imgName.IndexOf("?"))
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory
                    If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
                    Try

                        If File.Exists(phyimgDirectory & "/" & imgName) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                            LogHelp.writeLog("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            writeToForm("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            Continue For
                        End If
                        client.DownloadFile(Uri, phyimgDirectory & "/" & imgName)
                        writeToForm("download one img: " & Uri & vbCrLf)
                        LogHelp.writeLog("download one img: " & Uri & vbCrLf)
                    Catch ex As Exception
                        writeToForm("ERROR：" & Uri & vbCrLf)
                        LogHelp.writeLog("ERROR：" & Uri & vbCrLf)
                    End Try

                ElseIf regSwf.IsMatch(Uri) Then '下载SWF
                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)

                    indexend = imgpath.LastIndexOf("/")

                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1)
                    If imgName.Contains("?") Then
                        imgName = imgName.Substring(0, imgName.IndexOf("?"))
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory
                    If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
                    Try

                        If File.Exists(phyimgDirectory & "/" & imgName) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                            LogHelp.writeLog("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            writeToForm("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            Continue For
                        End If
                        client.DownloadFile(Uri, phyimgDirectory & "/" & imgName)

                        writeToForm("download one swf" & Uri & vbCrLf)
                        LogHelp.writeLog("download one swf: " & Uri & vbCrLf)
                    Catch ex As Exception
                        writeToForm("ERROR：" & Uri & vbCrLf)
                        LogHelp.writeLog("ERROR：" & Uri & vbCrLf)
                    End Try

                ElseIf regCss.IsMatch(Uri) Then  'Uri.Contains(urlStartcs) Then '下载css样式
                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)
                    indexend = imgpath.LastIndexOf("/")
                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1)
                    If imgName.Contains("?") Then
                        imgName = imgName.Substring(0, imgName.IndexOf("?"))
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory
                    If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
                    Try
                        If File.Exists(phyimgDirectory & "/" & imgName) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                            LogHelp.writeLog("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            writeToForm("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            Continue For
                        End If
                        DownloadFile(Uri, phyimgDirectory & "/" & imgName)

                        writeToForm("download one css file" & Uri & vbCrLf)
                        LogHelp.writeLog("download one css file: " & Uri & vbCrLf)
                    Catch ex As Exception
                        writeToForm("ERROR：" & Uri & vbCrLf)
                        LogHelp.writeLog("ERROR：" & Uri & vbCrLf)
                    End Try

                ElseIf regJs.IsMatch(Uri) Then   'Uri.Contains(UrlStartjs) Then '下载js文件
                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)

                    indexend = imgpath.LastIndexOf("/")

                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1)
                    If imgName.Contains("?") Then
                        imgName = imgName.Substring(0, imgName.IndexOf("?"))
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory
                    If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
                    Try
                        Dim pghtml As String = GetMainPageHtml(Uri)
                        If File.Exists(phyimgDirectory & "/" & imgName) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                            LogHelp.writeLog("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            writeToForm("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            Continue For
                        End If
                        File.WriteAllText(phyimgDirectory & "/" & imgName, pghtml, System.Text.Encoding.UTF8)
                        writeToForm("download one js file: " & Uri & vbCrLf)
                        LogHelp.writeLog("download one js file: " & Uri & vbCrLf)
                    Catch ex As Exception
                        writeToForm("ERROR：" & Uri & vbCrLf)
                        LogHelp.writeLog("ERROR：" & Uri & vbCrLf)
                    End Try

                ElseIf regpdf.IsMatch(Uri) Then 'Uri.Contains("pdf") Then '下载pdf文件

                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)
                    indexend = imgpath.LastIndexOf("/")
                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1)
                    If imgName.Contains("?") Then
                        imgName = imgName.Substring(0, imgName.IndexOf("?"))
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory & "/" & imgName
                    If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
                    Try
                        If File.Exists(phyimgDirectory & "/" & imgName) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                            LogHelp.writeLog("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            writeToForm("jump a file: " & phyimgDirectory & "/" & imgName & " URL: " & Uri & vbCrLf)
                            Continue For
                        End If
                        client.DownloadFile(Uri, phyimgDirectory & "/" & imgName)
                        writeToForm("download one pdf file: " & vbCrLf)
                        LogHelp.writeLog("download one pdf file: " & vbCrLf)
                    Catch ex As Exception
                        writeToForm("ERROR：" & Uri & vbCrLf)
                        LogHelp.writeLog("ERROR：" & Uri & vbCrLf)
                    End Try
                Else '下载跳转页面
                    Dim isContains = False '兼容问号后面带“.”的链接，也把它变为文件夹
                    Uri = Uri.Replace("amp;", "")
                    indexhttp = Uri.IndexOf(SiteName, 30)
                    imgpath = Uri.Substring(indexhttp)
                    If level <> 0 Then

                        If imgpath.EndsWith(SiteName) OrElse imgpath.EndsWith(SiteName & "/") Then
                            Continue For
                        End If
                    End If
                    If imgpath.Contains("?") Then
                        isContains = True
                    End If
                    imgpath = imgpath.Replace("?", "/").Replace("&", "/").Replace(":", "/").Replace("#", "") '带参数的url地址变成多重文件夹结构
                    If Not imgpath.Substring(imgpath.LastIndexOf("/")).Contains(".") OrElse isContains Then
                        imgpath = imgpath.TrimEnd("/") & "/"
                    End If
                    indexend = imgpath.LastIndexOf("/")
                    imgDirectory = imgpath.Substring(0, indexend)
                    imgName = imgpath.Substring(indexend + 1).Split(".")(0)
                    If String.IsNullOrEmpty(imgName) Then
                        imgName = "index"
                    End If
                    phyimgDirectory = Filepath & "/" & imgDirectory

                    Try
                        DownloadFile(Uri, phyimgDirectory.TrimEnd("/") & "/" & imgName & ".html")

                    Catch ex As Exception
                        writeToForm("404：" & Uri & vbCrLf)
                        LogHelp.writeLog("404：" & Uri & vbCrLf)

                    End Try

                End If
            Next
            CurrentUrlList.Clear()
            CurrentUrlList.AddRange(NextUrlList.ToArray())
            NextUrlList.Clear()
            level += 1
        End While
    End Sub

    '获取内嵌在网页中的pdf文件url
    Private Sub GetUrlListPdf(ByRef strHtml As String)
        Dim reg As New System.Text.RegularExpressions.Regex("<iframe.*[\n].*</iframe>")
        strHtml = strHtml.Replace(strHtml, reg.Match(strHtml).ToString)
        reg = New System.Text.RegularExpressions.Regex("style="".*""")
        strHtml = reg.Replace(strHtml, "style=""position:absolute;height:100%;width:100%;""")
        reg = New System.Text.RegularExpressions.Regex("src=""(.*?)""")
        Dim pdfUrl = reg.Match(strHtml).Groups(1).ToString
        NextUrlList.Add(pdfUrl)
        Dim itemStr = pdfUrl.Substring(pdfUrl.LastIndexOf("/")) + pdfUrl.Substring(pdfUrl.LastIndexOf("/"))
        itemStr = itemStr.TrimStart("/")
        strHtml = strHtml.Replace(pdfUrl, itemStr)
    End Sub
    '获取网站名，不包括www，作为主文件夹
    Private Function GetWebSite(strurl As String) As String
        If strurl.Contains("www.") Then
            strurl = strurl.Substring(strurl.IndexOf("www.") + 4)
        Else
            strurl = strurl.Substring(strurl.IndexOf("//") + 2)
        End If
        If strurl.EndsWith("/") Then
            strurl = strurl.Remove(strurl.Length - 1)
        End If
        If (strurl.Contains(":")) Then
            strurl = strurl.Split(":")(0)
        End If
        Return strurl
    End Function

    '根据网页中内容筛选出所有需要的链接
    Private Sub GetUrlList(ByRef strHtml As String)

        postDoc.LoadHtml(strHtml)

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//a")
        strHtml = getLinkForA(strHtml, htmlNodeList)

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//img")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//script")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//embed")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//link")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "href")

    End Sub

    Private Function getLinkByAttribute(strHtml As String, htmlNodeList As HtmlNodeCollection, itemAttribute As String) As String
        If htmlNodeList IsNot Nothing Then
            For Each link In htmlNodeList
                Dim uri As String = link.GetAttributeValue(itemAttribute, "")
                If Not ExisturlList.Contains(uri) Then
                    ExisturlList.Add(uri)
                    NextUrlList.Add(uri)
                End If
                If uri.Length < 20 Then
                    Continue For
                End If
                Dim urlindexhttp As Integer = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then
                    strHtml = strHtml.Replace(uri, uri.Substring(urlindexhttp))
                End If
            Next
        End If
        Return strHtml
    End Function

    '获取A标签中的链接
    Private Function getLinkForA(strHtml As String, htmlNodeList As HtmlNodeCollection) As String
        If htmlNodeList IsNot Nothing Then
            For Each a In htmlNodeList
                Dim uri As String = a.GetAttributeValue("href", "")
                If Not uri.StartsWith("http") AndAlso Not uri.StartsWith("/web/") AndAlso Not uri.StartsWith("/") Then
                    uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & "/" & uri
                End If

                If Not ExisturlList.Contains(uri) Then
                    ExisturlList.Add(uri)
                    NextUrlList.Add(uri)
                End If

                If uri.Length < 20 Then
                    Continue For
                End If
                Dim urlindexhttp As Integer = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then
                    strHtml = strHtml.Replace(uri, uri.Substring(urlindexhttp))
                End If
                '兼容用js打开的链接
                Dim itemStr = a.GetAttributeValue("onclick", "")
                If String.IsNullOrEmpty(itemStr) Then
                    Continue For
                End If
                uri = regJslink.Match(itemStr).Value.TrimStart("(").TrimStart("'")
                If Not uri.StartsWith("http") AndAlso Not uri.StartsWith("/web/") AndAlso Not uri.StartsWith("/") Then
                    uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & "/" & uri
                End If
                If Not ExisturlList.Contains(uri) Then
                    ExisturlList.Add(uri)
                    NextUrlList.Add(uri)
                End If
                If uri.Length < 20 Then
                    Continue For
                End If
                urlindexhttp = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then
                    strHtml = strHtml.Replace(uri, uri.Substring(urlindexhttp))
                End If
            Next
        End If
        Return strHtml
    End Function
    '获取css中所有background的图片路径
    Private Sub GetUrlListcss(ByRef strHtml As String, ByVal str As String)
        Dim background As String() = {"background"}
        Dim backgroundpost As String() = strHtml.Split(background, StringSplitOptions.RemoveEmptyEntries)
        If (backgroundpost.Count >= 2) Then
            For i As Integer = 1 To backgroundpost.Count - 1
                Dim postString As String = backgroundpost(i).Trim
                Try

                    If postString.Contains(str) Then
                        postString = postString.Substring(postString.IndexOf(str))
                        postString = regImg.Match(postString).ToString
                        If ExisturlList.Contains(postString) Then
                            Continue For
                        End If
                        NextUrlList.Add(postString)
                        ExisturlList.Add(postString)
                    End If
                    If postString.Contains("url('/web/") OrElse postString.Contains("url(/") OrElse postString.Contains("url(""/") Then
                        postString = postString.Substring(postString.IndexOf("/web/"))
                        postString = "https://web.archive.org" + regImg.Match(postString).ToString
                        If ExisturlList.Contains(postString) Then
                            Continue For
                        End If
                        NextUrlList.Add(postString)
                        ExisturlList.Add(postString)
                    End If

                Catch

                End Try
            Next
        End If
    End Sub

    'Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
    '    '避免webbroswer.DocumentCompleted被多次引发
    '    If Not e.Url = WebBrowser1.Url OrElse Not WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
    '        Return
    '    End If
    '    Dim doc As System.Windows.Forms.HtmlDocument = WebBrowser1.Document

    'End Sub
    '通过url获取内容
    Private Function GetMainPageHtml(pageurl As String) As String
        'WebBrowser1.Navigate(pageurl)
        Dim resultSting As String
        Dim request As HttpWebRequest = HttpWebRequest.Create(pageurl)

        request.UseDefaultCredentials = True 'New NetworkCredential("reasonable", "190854$")
        '设置代理
        ' request.Proxy = New WebProxy("127.0.0.1:19815", True)
        request.Timeout = 15000
        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"
        'request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch")'导致请求回来的数据是乱码
        request.Headers.Add("Accept-Language", "zh-CN")
        request.UserAgent = "Mozilla/5.0 (compatible;MSIE 9.0;Windows NT 6.1;WOW64;Trident/5.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.65 Safari/537.36"
        request.Method = "GET"
        'request.Headers.Add("Cookie", Cookie)
        request.AllowAutoRedirect = True
        'WebRequest.Create方法，返回WebRequest的子类HttpWebRequest

        Dim response As WebResponse = request.GetResponse()


        'WebRequest.GetResponse方法，返回对 Internet 请求的响应
        Dim resStream As Stream = response.GetResponseStream()
        'WebResponse.GetResponseStream 方法，从 Internet 资源返回数据流。

        Dim resStreamReader As StreamReader = New StreamReader(resStream, System.Text.Encoding.UTF8)
        resultSting = resStreamReader.ReadToEnd()
        Dim reg As New System.Text.RegularExpressions.Regex("<!-- BEGIN WAYBACK TOOLBAR INSERT -->(.|\n)*?<!-- END WAYBACK TOOLBAR INSERT -->")
        resultSting = reg.Replace(resultSting, "")
        reg = New System.Text.RegularExpressions.Regex("<!--(.|\n)*?-->") '去掉注释
        resultSting = reg.Replace(resultSting, "").Replace("</body>", "<!--begain seo for link-->
        <div>
        </div>
        <!--end seo for link-->
        </body>")
        Return resultSting

    End Function


End Class
