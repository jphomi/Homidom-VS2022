Public Class HoMIOAuth2

    Private _PortSOAP As String
    Private _IdSrv As String
    Private _nameSoft As String

    Sub New(ByVal IdSrv As String, PortSOAP As String, nameSoft As String)
        _PortSOAP = PortSOAP
        _IdSrv = IdSrv
        _nameSoft = nameSoft
    End Sub

    Public Function GetAuthorizationUrl(ByVal type As String) As String

        If _nameSoft <> "HoMIDoM" Then Return "error" : Exit Function

        Select Case type
            Case "Nest"
                Return String.Format("https://home.nest.com/login/oauth2?client_id=" _
                                                 & GetClientFile(type).web.client_id & "&state=" & Rnd())

            Case "Netatmo"
                Return String.Format("https://api.netatmo.net/oauth2/authorize?client_id=" _
                                                 & GetClientFile(type).web.client_id _
                                                 & "&redirect_uri=" & GetClientFile(type).web.redirect_uris(0) _
                                                 & "&scope=read_station" _
                                                 & "&state=" & Rnd())
            Case "GoogleCalendar"
                Return String.Format("https://accounts.google.com/o/oauth2/auth?scope=https://www.googleapis.com/auth/calendar&state=" _
                                                 & Rnd() & "&access_type=offline&redirect_uri=" _
                                                 & GetClientFile(type).web.redirect_uris(0) _
                                                 & "&response_type=code&client_id=" _
                                                 & GetClientFile(type).web.client_id _
                                                 & "&approval_prompt=force")
            Case Else
                Return ""
                Exit Function
        End Select
    End Function
    Public Function GetClientFile(ByVal type As String) As ClientOAuth2

        If _nameSoft <> "HoMIDoM" Then Return Nothing : Exit Function

        Dim clientFileTemp As ClientOAuth2 = New ClientOAuth2

        Select Case type
            Case "Nest"
                clientFileTemp.web.token_uri = "https://api.home.nest.com/oauth2/access_token?"
                clientFileTemp.web.client_id = "3611226e-07dd-4c50-af22-890c894c180c"
                clientFileTemp.web.client_secret = "Vbx7sfHxzBHBQ7jUSWBy4dyXg"

            Case "Netatmo"
                clientFileTemp.web.token_uri = "https://api.netatmo.net/oauth2/token"
                clientFileTemp.web.client_id = "56104dc549c75fb176505f06"
                clientFileTemp.web.client_secret = "7lqd7Hao7tNO9PLnOuBFdiNwswTiS6Qn"

            Case "GoogleCalendar"
                clientFileTemp.web.token_uri = "https://www.googleapis.com/oauth2/v3/token"
                clientFileTemp.web.client_id = "425861022935-i5tef67vcufp3g0p763ij9fdt1p27g7q.apps.googleusercontent.com"
                clientFileTemp.web.client_secret = "IZSCuokhUUKbAt7G68jjJaut"
            Case Else
                clientFileTemp.web.token_uri = ""
                clientFileTemp.web.client_id = ""
                clientFileTemp.web.client_secret = ""
        End Select

        clientFileTemp.Nom = type
        clientFileTemp.web.redirect_uris.Add("http://" & "localhost" & ":" & _PortSOAP & "/api/" & _IdSrv & "/command/Device/" & type & "/ON")

        Return clientFileTemp

    End Function


    Public Function GetToken(ByVal clientOauth As String, ByVal code As String) As String

        If _nameSoft <> "HoMIDoM" Then Return "error" : Exit Function

        Try

            Dim client As New Net.WebClient
            Dim reqparm As New Specialized.NameValueCollection
            reqparm.Add("code", code)
            reqparm.Add("client_id", GetClientFile(clientOauth).web.client_id)
            reqparm.Add("client_secret", GetClientFile(clientOauth).web.client_secret)
            reqparm.Add("redirect_uri", GetClientFile(clientOauth).web.redirect_uris(0))
            reqparm.Add("grant_type", "authorization_code")
            Dim responsebytes = client.UploadValues(GetClientFile(clientOauth).Nom, "POST", reqparm)
            Dim responsebody = (New System.Text.UTF8Encoding).GetString(responsebytes)
            Dim Oauth As Authentication = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Authentication))
            Dim stream = Newtonsoft.Json.JsonConvert.SerializeObject(Oauth)
            System.IO.File.WriteAllText(My.Application.Info.DirectoryPath & "\config\reponse_accesstoken_" & clientOauth & ".json", stream)
            Return "ok"

        Catch ex As Exception
            Return "ERR: " & ex.Message
        End Try
    End Function

End Class
