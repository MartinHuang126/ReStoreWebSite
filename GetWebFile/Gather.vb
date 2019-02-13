Imports System.IO
Imports System.Net
Imports HtmlAgilityPack
Public Class Gather

    Private regImg As New System.Text.RegularExpressions.Regex(".*\.(jpg|png|gif|ico|bpm)")
    Private regJslink As New System.Text.RegularExpressions.Regex("\('*.*\.(jsp|html|htm|asp|php)")
    Private regJs As New System.Text.RegularExpressions.Regex(".*\.(js|sc)")
    Private regCss As New System.Text.RegularExpressions.Regex(".*\.css")
    Private regSwf As New System.Text.RegularExpressions.Regex(".*\.swf")
    Private regpdf As New System.Text.RegularExpressions.Regex(".*\.pdf$")
    'Private WebBrowser1 As New WebBrowser
    'Friend WithEvents WebBrowser1 As WebBrowser
    Public ExisturlList As List(Of String) = New List(Of String) '已经爬到过的url
    Public CurrentUrlList As List(Of String) = New List(Of String) '当前层次的urllist
    Public NextUrlList As List(Of String) = New List(Of String) '下一层次的

    Public postDoc As HtmlDocument()
    Public htmlNodeList As HtmlNodeCollection
    Public client As WebClient

    Public Delegate Sub WriteToText(ByVal text As String) '写全部log到窗体的委托
    Private writeToForm As WriteToText
    Private _Filepath As String
    Private _currentUrl As String
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
    Private threadLock As Object = New Object()
    Public Property Filepath As String
        Get
            Return _Filepath
        End Get
        Set
            _Filepath = Value
        End Set
    End Property

    'Public Property currentUrl As String
    '    Get
    '        Return _currentUrl.Substring(0, _currentUrl.LastIndexOf("/") + 1)
    '    End Get
    '    Set
    '        _currentUrl = Value
    '    End Set
    'End Property

    'Public Property Imgpath As String

    Public Sub New(ByVal Url As String, ByVal filepath As String, level As Integer)
        _Filepath = filepath
        Urlmain = Url
        TatolLevel = level
        CurrentUrlList.Clear()
        NextUrlList.Clear()
        ExisturlList.Clear()
        Dim index As Integer = Url.IndexOf("/http")
        If (index < 0) Then
            Return
        End If
        UrlStart = Url.Substring(0, index).Replace("if_", "").Replace("fw_", "") '去掉由Iframe开始的主页的if_标签
        UrlStartjs = UrlStart & "js_/"
        UrlStartim = UrlStart & "im_/"
        UrlStartcs = UrlStart & "cs_/"
        UrlStartoe = UrlStart & "oe_/"
        UrlEnd = Url.Substring(index + 1)
        UrlEnd = UrlEnd.TrimEnd("/") & "/"
        UrlEnd = UrlEnd.Substring(0, UrlEnd.IndexOf("/", 10))
        'If UrlEnd.Contains(".php") OrElse UrlEnd.Contains(".html") OrElse UrlEnd.Contains(".asp") _
        '    OrElse UrlEnd.Contains(".jsp") OrElse UrlEnd.Contains(".htm") Then
        '    UrlEnd = UrlEnd.Substring(0, UrlEnd.IndexOf("/", 10))
        'End If

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

    Public Sub ExcuteTask()
        'DownloadFile(Urlmain, IndexPath)
        CurrentUrlList.Add(Urlmain)
        saveAllFile()
        writeMessage("Complete: " & Urlmain & vbCrLf)
    End Sub

    Public Sub DownloadFileForHtml(ByVal url As String, ByVal fullpath As String)
        writeMessage(DateTime.Now.ToString & " star: " & url & vbCrLf)

        Dim strHtml As String = GetMainPageHtml(url)
        addExisturlList(url)

        If strHtml.Contains("404 Not Found") OrElse
                strHtml.Contains("Your generous donation will be matched 2-to-1 right now. Your $5 becomes $15!") Then
            Throw New Exception
        End If
        strHtml = strHtml.Replace("https://web.archive.org", "") 'strHtml.Replace(UrlStart, "A_A_A_A")
        'strHtml = strHtml.Replace(UrlStart.Substring(UrlStart.IndexOf("/web/")), UrlStart)
        strHtml = strHtml.Replace("/web/", "https://web.archive.org/web/")
        'strHtml = strHtml.Replace("A_A_A_A", UrlStart)
        'strHtml = strHtml.Replace(SiteName & ":80", SiteName)
        strHtml = strHtml.Replace("<fb:like action=""like"" show_faces=""false"" width=""140""></fb:like>", "")

        '获取页面所有链接放在UrlList
        Try
            GetUrlList(strHtml, url)
        Catch ex As Exception
            writeMessage("GetUrlList error URL: " & url & ",filePath: " & fullpath & vbCrLf)
        End Try

        If fullpath.Contains(".css") Then
            Try
                GetUrlListcss(strHtml, UrlStartim)
            Catch ex As Exception
                writeMessage("GetUrlListcss error URL: " & url & ",filePath: " & fullpath & vbCrLf)
            End Try
            Dim reg As New System.Text.RegularExpressions.Regex("/web/.*_/" + UrlEnd.TrimEnd("/"))
            strHtml = reg.Replace(strHtml, "")
        End If
        If fullpath.Contains("pdf") AndAlso fullpath.Contains(".html") Then

            GetUrlListPdf(strHtml)

        End If
        strHtml = strHtml.Replace(":80", "").Replace(UrlEnd.TrimEnd("/"), "").Replace(UrlEnd.Replace("www.", ""), "").Replace(UrlEnd.Replace("//", "//www."), "")
        strHtml = strHtml.Replace("https://web.archive.org", "").Replace("http://www.10times10.com", "").Replace("http://perso.wanadoo.fr","")
        Dim fileDirectory As String = fullpath.Substring(0, fullpath.LastIndexOf("/"))
        '下载首页
        If Not IO.Directory.Exists(fileDirectory) Then IO.Directory.CreateDirectory(fileDirectory)

        Try
            If File.Exists(fullpath) AndAlso Not url.Contains("if_") Then
                writeMessage(DateTime.Now.ToString & " jump a html file: " & fullpath & " URL: " & url & vbCrLf)
                Return
            End If
            SyncLock threadLock
                File.WriteAllText(fullpath, strHtml, System.Text.Encoding.UTF8)
            End SyncLock
        Catch ex As Exception
            writeMessage(DateTime.Now.ToString & " writeFileError: " & fullpath & " , url: " & url & vbCrLf)
        End Try
        writeMessage(DateTime.Now.ToString & " done one html file: " & url & vbCrLf)

    End Sub

    Public Sub saveAllFile()

        Dim level As Integer = 0
        Dim paraOption As New ParallelOptions
        paraOption.MaxDegreeOfParallelism = 5

        While CurrentUrlList.Count > 0 AndAlso level <= TatolLevel
            writeToForm(DateTime.Now.ToString & " MainSite: " & Urlmain & " CurrentUrlList's count: " & CurrentUrlList.Count & "Level: " & level & vbCrLf)
            LogHelp.writeLog(DateTime.Now.ToString & " MainSite: " & Urlmain & " CurrentUrlList's count: " & CurrentUrlList.Count & "Level: " & level & vbCrLf)
#Region "串行"
            'For Each Uri In CurrentUrlList
            '    If Uri.StartsWith("/web/") Then
            '        Uri = "https://web.archive.org" & Uri
            '    End If
            '    indexhttp = getIndexHttp(Uri, indexhttp)

            '    If Not Uri.Contains("web.archive.org") OrElse indexhttp < 0 Then
            '        Continue For
            '    End If

            '    If regpdf.IsMatch(Uri) OrElse regImg.IsMatch(Uri) OrElse regJs.IsMatch(Uri) OrElse regSwf.IsMatch(Uri) OrElse regCss.IsMatch(Uri) Then  '下载静态文件
            '        strFilePath = getFilePath(Uri, imgpath, indexhttp, indexend, imgDirectory, imgName, phyimgDirectory)
            '        Try
            '            If File.Exists(strFilePath) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
            '                LogHelp.writeLog(DateTime.Now.ToString & " jump a file: " & strFilePath & " URL: " & Uri & vbCrLf)
            '                writeToForm(DateTime.Now.ToString & " jump a file: " & strFilePath & " URL: " & Uri & vbCrLf)
            '                Continue For
            '            End If
            '            If regCss.IsMatch(Uri) Then
            '                DownloadFileForHtml(Uri, strFilePath)
            '                writeToForm(DateTime.Now.ToString & " download one css file" & Uri & vbCrLf)
            '                LogHelp.writeLog(DateTime.Now.ToString & " download one css file: " & Uri & vbCrLf)
            '                Continue For
            '            End If
            '            client.DownloadFile(Uri, strFilePath)
            '            writeToForm(DateTime.Now.ToString & " download one file: " & Uri & vbCrLf)
            '            LogHelp.writeLog(DateTime.Now.ToString & " download one file: " & Uri & vbCrLf)
            '        Catch ex As Exception
            '            writeToForm(DateTime.Now.ToString & " ERROR：" & Uri & vbCrLf)
            '            LogHelp.writeLog(DateTime.Now.ToString & " ERROR：" & Uri & vbCrLf)
            '        End Try

            '    Else '下载跳转页面
            '        Dim isContains = False '兼容问号后面带“.”的链接，也把它变为文件夹
            '        Uri = Uri.Replace("amp;", "").Replace("%20", " ")
            '        imgpath = Uri.Substring(indexhttp)
            '        If level <> 0 Then
            '            If imgpath.EndsWith(SiteName) OrElse imgpath.EndsWith(SiteName & "/") Then
            '                Continue For
            '            End If
            '        End If
            '        If imgpath.Contains("?") Then
            '            isContains = True
            '        End If
            '        imgpath = imgpath.Replace("?", "/").Replace("&", "/").Replace(":", "/").Replace("#", "") '带参数的url地址变成多重文件夹结构
            '        If Not imgpath.Substring(imgpath.LastIndexOf("/")).Contains(".") OrElse isContains Then
            '            imgpath = imgpath.TrimEnd("/") & "/"
            '        End If
            '        indexend = imgpath.LastIndexOf("/")
            '        imgDirectory = imgpath.Substring(0, indexend)
            '        imgName = imgpath.Substring(indexend + 1).Split(".")(0)
            '        If String.IsNullOrEmpty(imgName) Then
            '            imgName = "index"
            '        End If
            '        phyimgDirectory = Filepath & "/" & imgDirectory

            '        Try
            '            DownloadFileForHtml(Uri, phyimgDirectory.TrimEnd("/") & "/" & imgName & ".html")

            '        Catch ex As Exception
            '            writeToForm(DateTime.Now.ToString & " 404：" & Uri & vbCrLf)
            '            LogHelp.writeLog(DateTime.Now.ToString & " 404：" & Uri & vbCrLf)

            '        End Try

            '    End If
            'Next
#End Region

            Parallel.ForEach(Of String)(CurrentUrlList, paraOption, Function(uri)
                                                                        'For Each uri In CurrentUrlList
                                                                        Dim imgpath As String = String.Empty
                                                                        Dim indexend As Integer = 0
                                                                        Dim imgDirectory As String = String.Empty
                                                                        Dim imgName As String = String.Empty
                                                                        Dim phyimgDirectory As String = String.Empty
                                                                        Dim strFilePath As String = String.Empty
                                                                        If uri.StartsWith("/web/") Then
                                                                            uri = "https://web.archive.org" & uri
                                                                        End If
                                                                        Dim indexhttp = getIndexHttp(uri)

                                                                        If Not uri.Contains("web.archive.org") OrElse indexhttp < 0 Then
                                                                            Return Nothing
                                                                        End If

                                                                        If regpdf.IsMatch(uri) OrElse regImg.IsMatch(uri) OrElse regJs.IsMatch(uri) OrElse regSwf.IsMatch(uri) OrElse regCss.IsMatch(uri) Then  '下载静态文件
                                                                            uri = uri.Replace("amp;", "").Replace("%20", " ")
                                                                            strFilePath = getFilePath(uri, imgpath, indexhttp, indexend, imgDirectory, imgName, phyimgDirectory)
                                                                            Try
                                                                                If File.Exists(strFilePath) Then '跳过已存在文件，可以支持缺失文件从不同时间抓取
                                                                                    writeMessage(DateTime.Now.ToString & " jump a file: " & strFilePath & " URL: " & uri & vbCrLf)
                                                                                    Return Nothing
                                                                                End If
                                                                                If regCss.IsMatch(uri) Then

                                                                                    DownloadFileForHtml(uri, strFilePath)

                                                                                    writeMessage(DateTime.Now.ToString & " download one css file" & uri & vbCrLf)
                                                                                    Return Nothing
                                                                                End If
                                                                                DownloadStaticFile(uri, strFilePath)
                                                                                writeMessage(DateTime.Now.ToString & " download one file: " & uri & vbCrLf)
                                                                            Catch ex As Exception
                                                                                writeMessage(DateTime.Now.ToString & " ERROR：" & uri & ex.Message & vbCrLf)
                                                                                Return Nothing
                                                                            End Try

                                                                        Else '下载跳转页面
                                                                            Dim isContains = False '兼容问号后面带“.”的链接，也把它变为文件夹
                                                                            uri = uri.Replace("amp;", "").Replace("%20", " ")
                                                                            imgpath = uri.Substring(indexhttp).Replace(":80", "").Replace("perso.wanadoo.fr", SiteName) '特例二级域名改为主域名
                                                                            If level <> 0 AndAlso Not uri.Contains("if_") Then
                                                                                If imgpath.EndsWith(SiteName) OrElse imgpath.EndsWith(SiteName & "/") Then
                                                                                    Return Nothing
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
                                                                            If indexend < 0 Then
                                                                                Return Nothing
                                                                            End If
                                                                            imgDirectory = imgpath.Substring(0, indexend)
                                                                            imgName = imgpath.Substring(indexend + 1).Split(".")(0)
                                                                            If String.IsNullOrEmpty(imgName) Then
                                                                                imgName = "index"
                                                                            End If
                                                                            imgDirectory = getStringWithNoillegal(imgDirectory)
                                                                            phyimgDirectory = Filepath & "/" & imgDirectory

                                                                            imgName = getStringWithNoillegal(imgName)
                                                                            Try
                                                                                DownloadFileForHtml(uri, phyimgDirectory.TrimEnd("/") & "/" & imgName & ".html")

                                                                            Catch ex As Exception
                                                                                writeMessage(DateTime.Now.ToString & " 404：" & uri & vbCrLf)
                                                                                Return Nothing
                                                                            End Try

                                                                        End If
                                                                    End Function)
            CurrentUrlList.Clear()
            If NextUrlList.Count = 0 Then
                Return
            End If
            CurrentUrlList.AddRange(NextUrlList.ToArray())
            NextUrlList.Clear()
            level += 1
        End While
    End Sub

    Private Sub DownloadStaticFile(uri As String, strFilePath As String)
        SyncLock threadLock
            client = New WebClient()
            client.DownloadFile(uri, strFilePath)
            client.Dispose()
        End SyncLock
    End Sub

    Private Sub addExisturlList(url As String)
        SyncLock threadLock
            If Not ExisturlList.Contains(url) Then
                ExisturlList.Add(url)
            End If
        End SyncLock
    End Sub

    Private Sub addNextUrlList(url As String)
        SyncLock threadLock
            If Not ExisturlList.Contains(url) Then
                ExisturlList.Add(url)
                NextUrlList.Add(url)
            End If
        End SyncLock
    End Sub

    Private Sub writeMessage(ByVal message)
        SyncLock threadLock
            writeToForm(message)
            LogHelp.writeLog(message)
        End SyncLock
    End Sub

    '构造路径下载静态文件
    Private Function getFilePath(Uri As String, staticFilepath As String, indexhttp As Integer, indexend As Integer, staticFileDirectory As String, staticFileName As String, phyimgDirectory As String) As String
        staticFilepath = Uri.Substring(indexhttp).Replace("10times10.com", SiteName).Replace(":80", "").Replace("perso.wanadoo.fr", SiteName)
        indexend = staticFilepath.LastIndexOf("/")
        staticFileDirectory = staticFilepath.Substring(0, indexend)
        staticFileName = staticFilepath.Substring(indexend + 1)
        If staticFileName.Contains("?") Then
            staticFileName = staticFileName.Substring(0, staticFileName.IndexOf("?"))
        End If
        staticFileDirectory = getStringWithNoillegal(staticFileDirectory)
        staticFileName = getStringWithNoillegal(staticFileName)
        If regpdf.IsMatch(Uri) Then
            phyimgDirectory = Filepath & "/" & staticFileDirectory & "/" & staticFileName
        Else
            phyimgDirectory = Filepath & "/" & staticFileDirectory
        End If

        If Not IO.Directory.Exists(phyimgDirectory) Then IO.Directory.CreateDirectory(phyimgDirectory)
        Return Path.Combine(phyimgDirectory, staticFileName)
    End Function

    Private Function getStringWithNoillegal(ByRef str As String) As String
        ': * " < > | ? 文件不支持字符
        str = str.Replace(":", "").Replace("*", "").Replace("""", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("?", "")
        Return str
    End Function

    Private Function getIndexHttp(uri As String) As Integer
        If uri.Length > 30 Then
            Dim index = uri.IndexOf(SiteName, 30)
            If index > -1 Then
                Return index
            End If
            Return IIf(uri.IndexOf("10times10.com", 30) > -1, uri.IndexOf("10times10.com", 30), uri.IndexOf("perso.wanadoo.fr", 30))
        End If
        Return -1
    End Function

    '获取内嵌在网页中的pdf文件url
    Private Sub GetUrlListPdf(ByRef strHtml As String)

        Dim reg As New System.Text.RegularExpressions.Regex("<iframe.*[\n].*</iframe>")
        strHtml = strHtml.Replace(strHtml, reg.Match(strHtml).ToString)
        reg = New System.Text.RegularExpressions.Regex("style="".*""")
        strHtml = reg.Replace(strHtml, "style=""position:absolute;height:100%;width:100%;""")
        reg = New System.Text.RegularExpressions.Regex("src=""(.*?)""")
        Dim pdfUrl = reg.Match(strHtml).Groups(1).ToString
        addNextUrlList(pdfUrl)
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
    Private Sub GetUrlList(ByRef strHtml As String, url As String)
        Dim postDoc = New HtmlDocument()
        postDoc.LoadHtml(strHtml)

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//a")
        strHtml = getLinkForA(strHtml, url, htmlNodeList)
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//area")
        strHtml = getLinkForA(strHtml, url, htmlNodeList)

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//img")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//body")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "background")
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//table")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "background")
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//td")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "background")
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//div")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "background")
        htmlNodeList = postDoc.DocumentNode.SelectNodes("//input")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//script")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//embed")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "src")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//link")
        strHtml = getLinkByAttribute(strHtml, htmlNodeList, "href")

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//iframe")
        strHtml = getLinkForFrame(strHtml, htmlNodeList)

        htmlNodeList = postDoc.DocumentNode.SelectNodes("//frame")
        strHtml = getLinkForFrame(strHtml, htmlNodeList)

    End Sub

    Private Function getLinkByAttribute(strHtml As String, htmlNodeList As HtmlNodeCollection, itemAttribute As String) As String
        If htmlNodeList IsNot Nothing Then

            For Each link In htmlNodeList
                Dim uri As String = link.GetAttributeValue(itemAttribute, "")

                'If uri.StartsWith("/web/") Then
                '    uri = "https://web.archive.org" & uri
                'End If
                addNextUrlList(uri)
                If uri.Length < 20 Then
                    Continue For
                End If
                Dim urlindexhttp As Integer = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then
                    Dim itemstr = uri.Substring(urlindexhttp)
                    strHtml = strHtml.Replace(uri, getStringWithNoillegal(itemstr.Substring(itemstr.IndexOf("/", 10))))
                End If
            Next
        End If
        Return strHtml
    End Function

    '获取A标签中的链接
    Private Function getLinkForA(strHtml As String, currentUrl As String, htmlNodeList As HtmlNodeCollection) As String
        If htmlNodeList IsNot Nothing Then
            For Each a In htmlNodeList
                Dim uri As String = a.GetAttributeValue("href", "")
                If uri.Contains("javascript") Then
                    uri = regJslink.Match(uri).Value.TrimStart("(").TrimStart("'")
                End If

                If Not uri.StartsWith("http") AndAlso Not uri.StartsWith("/web/") AndAlso Not uri.StartsWith("/") Then
                    If currentUrl.Substring(currentUrl.LastIndexOf("/")).Contains(".") Then
                        uri = currentUrl.Substring(0, currentUrl.LastIndexOf("/") + 1) & uri 'UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & "/" & uri
                    Else
                        uri = currentUrl.TrimEnd("/") & "/" & uri
                    End If

                End If

                    If uri.StartsWith("/") Then
                    uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & uri
                End If

                addNextUrlList(uri)

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
                    uri = currentUrl.Substring(0, currentUrl.LastIndexOf("/") + 1) & uri 'uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & "/" & uri
                End If
                If uri.StartsWith("/") Then
                    uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & uri
                End If
                addNextUrlList(uri)
                If uri.Length < 20 Then
                    Continue For
                End If
                urlindexhttp = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then
                    strHtml = strHtml.Replace(uri, uri.Substring(urlindexhttp).Replace(":80", ""))
                End If
            Next
        End If
        Return strHtml
    End Function

    '获取frame标签中的链接
    Private Function getLinkForFrame(strHtml As String, htmlNodeList As HtmlNodeCollection) As String
        If htmlNodeList IsNot Nothing Then
            For Each a In htmlNodeList
                Dim uri As String = a.GetAttributeValue("src", "")
                If Not uri.StartsWith("http") AndAlso Not uri.StartsWith("/web/") AndAlso Not uri.StartsWith("/") Then
                    uri = UrlStart.TrimEnd("/") & "/" & UrlEnd.Replace(":80", "") & "/" & uri
                End If
                addNextUrlList(uri)

                If uri.Length < 20 Then
                    Continue For
                End If
                Dim urlindexhttp As Integer = uri.IndexOf("http", 10)
                If (urlindexhttp > 0) Then

                    strHtml = strHtml.Replace(uri, uri.Substring(urlindexhttp).Replace(":80", ""))

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
                        addNextUrlList(postString)

                    ElseIf postString.Contains("url('/web/") OrElse postString.Contains("url(/") OrElse postString.Contains("url(""/") Then
                        postString = postString.Substring(postString.IndexOf("/web/"))
                        postString = "https://web.archive.org" + regImg.Match(postString).ToString
                        addNextUrlList(postString)
                    End If

                Catch
                    Continue For
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
        Dim request As HttpWebRequest = Nothing
        Dim response As WebResponse = Nothing
        Dim resStream As Stream = Nothing
        Dim resStreamReader As StreamReader = Nothing

        Try
            getResponse(pageurl, request, response)
            resStream = response.GetResponseStream()

            resStreamReader = New StreamReader(resStream, System.Text.Encoding.UTF8)
            resultSting = resStreamReader.ReadToEnd()

            Dim reg As New System.Text.RegularExpressions.Regex("<!-- BEGIN WAYBACK TOOLBAR INSERT -->(.|\n)*?<!-- END WAYBACK TOOLBAR INSERT -->")
            resultSting = reg.Replace(resultSting, "")
            reg = New System.Text.RegularExpressions.Regex("<!--(.|\n)*?-->") '去掉注释
            resultSting = reg.Replace(resultSting, "").Replace("</body>", "<!--begain seo for link--><div></div><!--end seo for link--></body>")

        Catch ex As Exception
            resultSting = "404 Not Found"
        Finally
            If Not resStreamReader Is Nothing Then
                resStreamReader.Close()
            End If
            If Not resStream Is Nothing Then
                resStream.Close()
            End If
            If Not request Is Nothing Then
                request.Abort()
            End If
            If Not response Is Nothing Then
                response.Close()
            End If

        End Try

        Return resultSting

    End Function

    Private Shared Sub getResponse(pageurl As String, ByRef request As HttpWebRequest, ByRef response As WebResponse)
        request = HttpWebRequest.Create(pageurl)

        request.UseDefaultCredentials = True 'New NetworkCredential("reasonable", "190854$")
        '设置代理
        request.Proxy = New WebProxy("115.151.1.103:9999", True)
        request.ServicePoint.Expect100Continue = False ';//加快载入速度
        request.ServicePoint.UseNagleAlgorithm = False ';//禁止Nagle算法加快载入速度
        request.AllowWriteStreamBuffering = False ';//禁止缓冲加快载入速度
        request.KeepAlive = True ';//启用长连接
        request.ServicePoint.ConnectionLimit = Integer.MaxValue ';//定义最大连接数
        request.Timeout = 15000
        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"
        'request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch")'导致请求回来的数据是乱码
        request.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5")
        request.UserAgent = "Mozilla/5.0 (compatible;MSIE 9.0;Windows NT 6.1;WOW64;Trident/5.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.65 Safari/537.36"
        request.Method = "GET"
        'request.Headers.Add("Cookie", Cookie)
        request.AllowAutoRedirect = True
        'WebRequest.Create方法，返回WebRequest的子类HttpWebRequest
        response = request.GetResponse()
    End Sub

    'Private Function GetStaticFile(url As String) As Boolean

    '    Dim request As HttpWebRequest = Nothing
    '    Dim response As WebResponse = Nothing
    '    getResponse(url, request, response)


    '    Dim resStream As Stream = response.GetResponseStream()
    '    Dim buffer As Char = New Byte()

    '    Dim resStreamReader As StreamReader = New StreamReader(resStream)
    '    resultSting = resStreamReader.Read()

    '    Dim reg As New System.Text.RegularExpressions.Regex("<!-- BEGIN WAYBACK TOOLBAR INSERT -->(.|\n)*?<!-- END WAYBACK TOOLBAR INSERT -->")
    '    resultSting = reg.Replace(resultSting, "")
    '    reg = New System.Text.RegularExpressions.Regex("<!--(.|\n)*?-->") '去掉注释
    '    resultSting = reg.Replace(resultSting, "").Replace("</body>", "<!--begain seo for link--><div></div><!--end seo for link--></body>")


    '    resStreamReader.Close()
    '    resStream.Close()
    '    request.Abort()
    '    response.Close()

    '    Return True
    'End Function

End Class
