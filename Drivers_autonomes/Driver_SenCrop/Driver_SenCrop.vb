Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports System.Reflection
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath
Imports Newtonsoft.Json.Linq
Imports MySql.Data


<Serializable()> Public Class Driver_SenCrop
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "39A8DAA6-7881-11E7-B68F-4F379AC7F8C8" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "SenCrop" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver SenCrop" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 180 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "@" 'Modèle du driver/interface
    Dim _Version As String = My.Application.Info.Version.ToString 'Version du driver
    Dim _OsPlatform As String = "3264" 'plateforme compatible 32 64 ou 3264 bits
    Dim _Picture As String = "" 'Image du driver (non utilisé actuellement)
    Dim _Server As HoMIDom.HoMIDom.Server 'Objet Reflètant le serveur
    Dim _DeviceSupport As New ArrayList 'Type de Device supporté par le driver
    Dim _Device As HoMIDom.HoMIDom.Device 'Image reflétant un device
    Dim _Parametres As New ArrayList 'Paramètres supplémentaires associés au driver
    Dim _LabelsDriver As New ArrayList 'Libellés, tooltip associés au driver
    Dim _LabelsDevice As New ArrayList 'Libellés, tooltip des devices associés au driver
    Dim MyTimer As New Timers.Timer 'Timer du driver
    Dim _IdSrv As String 'Id du Serveur (pour autoriser à utiliser des commandes)
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande) 'Liste des commandes avancées du driver
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes
    Dim _UrlSencrop As String = "https://app.sencrop.com"
    Dim _UrlSencropApi As String = "https://api.sencrop.com"

    Dim _UserName As String = ""
    Dim _Password As String = ""
    Dim _Obj As Object = Nothing
    Dim cookieToken As CookieContainer

    Dim authenconnec As New Sign_In
    Dim detailorganisation As New Organisation
    Dim listestat As New List(Of Stations)

    Public Class Sign_In
        Public userId As Integer
        Public organisationsIds As List(Of Integer)
        Public token As String
        Public expirationDate As String
        Public roles As List(Of String)
    End Class

    Public Class Organisation
        Public id As Integer
        Public name As String
        Public crops As String
    End Class
    Public Class Stations
        Public id As Integer
        Public identification As String
        Public serial As String
        Public name As String
        Public firmware As String
        Public signal As Double
        Public battery As Double
        Public calibration As String
        Public user_id As Integer
        Public active As Integer
        Public turn_off As Object
        Public latitude As Double
        Public altitude As Integer
        Public longitude As Double
        Public datas As New DataStation

    End Class

    Public Class DataStation
        Public time As Integer
        Public summary As String
        Public icon As String
        Public precipIntensity As Double
        Public precipProbability As Double
        Public precipitation As Double
        Public precipitation7j As Double
        Public temperature As Double
        Public temperatureMin As Double
        Public temperatureMax As Double
        Public dewPoint As Double
        Public humidity As Double
        Public pressure As Double
        Public windSpeed As Double
        Public windGust As Double
        Public windBearing As Double
        Public cloudCover As Double
        Public uvIndex As Double
        Public Today As New PrevisionDetail
        Public TodayJ1 As New PrevisionDetail
        Public TodayJ2 As New PrevisionDetail
        Public TodayJ3 As New PrevisionDetail
    End Class

    Public Class PrevisionDetail
        Public time As Double
        Public summary As String
        Public icon As String
        Public precipIntensity As Double
        Public precipProbability As Double
        Public precipType As String
        Public temperatureMin As Double
        Public temperatureMax As Double
        Public dewPoint As Double
        Public humidity As Double
        Public pressure As Double
        Public windSpeed As Double
        Public windGust As Double
        Public windBearing As Double
        Public cloudCover As Double
        Public uvIndex As Double
    End Class

#End Region

#Region "Propriétés génériques"
    ''' <summary>
    ''' Evènement déclenché par le driver au serveur
    ''' </summary>
    ''' <param name="DriveName"></param>
    ''' <param name="TypeEvent"></param>
    ''' <param name="Parametre"></param>
    ''' <remarks></remarks>
    Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent

    ''' <summary>
    ''' ID du serveur
    ''' </summary>
    ''' <value>ID du serveur</value>
    ''' <remarks>Permet d'accéder aux commandes du serveur pour lesquels il faut passer l'ID du serveur</remarks>
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _IdSrv = value
        End Set
    End Property

    ''' <summary>
    ''' Port COM du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property COM() As String Implements HoMIDom.HoMIDom.IDriver.COM
        Get
            Return _Com
        End Get
        Set(ByVal value As String)
            _Com = value
        End Set
    End Property
    Public ReadOnly Property Description() As String Implements HoMIDom.HoMIDom.IDriver.Description
        Get
            Return _Description
        End Get
    End Property

    ''' <summary>
    ''' Retourne la liste des devices supportés par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
        Get
            Return _DeviceSupport
        End Get
    End Property

    ''' <summary>
    ''' Liste des paramètres avancés du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Voir Sub New</remarks>
    Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
        Get
            Return _Parametres
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _Parametres = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
        Get
            Return _LabelsDriver
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDriver = value
        End Set
    End Property

    ''' <summary>
    ''' Liste les libellés et tooltip des champs associés au device associé au driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
        Get
            Return _LabelsDevice
        End Get
        Set(ByVal value As System.Collections.ArrayList)
            _LabelsDevice = value
        End Set
    End Property

    ''' <summary>
    ''' Active/Désactive le driver
    ''' </summary>
    ''' <value>True si actif</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
        Get
            Return _Enable
        End Get
        Set(ByVal value As Boolean)
            _Enable = value
        End Set
    End Property

    ''' <summary>
    ''' ID du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
        Get
            Return _ID
        End Get
    End Property

    ''' <summary>
    ''' Adresse IP TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
        Get
            Return _IP_TCP
        End Get
        Set(ByVal value As String)
            _IP_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Adresse IP UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
        Get
            Return _IP_UDP
        End Get
        Set(ByVal value As String)
            _IP_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Permet de savoir si le driver est actif
    ''' </summary>
    ''' <value>Retourne True si le driver est démarré</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
        Get
            Return _IsConnect
        End Get
    End Property

    ''' <summary>
    ''' Modèle du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
        Get
            Return _Modele
        End Get
        Set(ByVal value As String)
            _Modele = value
        End Set
    End Property

    ''' <summary>
    ''' Nom du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
        Get
            Return _Nom
        End Get
    End Property

    ''' <summary>
    ''' Image du driver (non utilisé)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
        Get
            Return _Picture
        End Get
        Set(ByVal value As String)
            _Picture = value
        End Set
    End Property

    ''' <summary>
    ''' Port TCP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
        Get
            Return _Port_TCP
        End Get
        Set(ByVal value As String)
            _Port_TCP = value
        End Set
    End Property

    ''' <summary>
    ''' Port UDP du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
        Get
            Return _Port_UDP
        End Get
        Set(ByVal value As String)
            _Port_UDP = value
        End Set
    End Property

    ''' <summary>
    ''' Type de protocole utilisé par le driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
        Get
            Return _Protocol
        End Get
    End Property

    ''' <summary>
    ''' Valeur de rafraichissement des devices
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    ''' <summary>
    ''' Objet représentant le serveur
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
        End Set
    End Property

    ''' <summary>
    ''' Version du driver
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Version() As String Implements HoMIDom.HoMIDom.IDriver.Version
        Get
            Return _Version
        End Get
    End Property

    Public ReadOnly Property OsPlatform() As String Implements HoMIDom.HoMIDom.IDriver.OsPlatform
        Get
            Return _OsPlatform
        End Get
    End Property

    ''' <summary>
    ''' True si le driver doit démarrer automatiquement
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    Public Property AutoDiscover() As Boolean Implements HoMIDom.HoMIDom.IDriver.AutoDiscover
        Get
            Return _AutoDiscover
        End Get
        Set(ByVal value As Boolean)
            _AutoDiscover = value
        End Set
    End Property
#End Region

#Region "Fonctions génériques"

    ''' <summary>Retourne la liste des Commandes avancées de type DeviceCommande</summary>
    ''' <remarks></remarks>
    Public Function GetCommandPlus() As List(Of DeviceCommande)
        Return _DeviceCommandPlus
    End Function

    ''' <summary>Execute une commande avancée</summary>
    ''' <param name="MyDevice">Objet représentant le Device </param>
    ''' <param name="Command">Nom de la commande avancée à éxécuter</param>
    ''' <param name="Param">tableau de paramétres</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteCommand(ByVal MyDevice As Object, ByVal Command As String, Optional ByVal Param() As Object = Nothing) As Boolean
        Dim retour As Boolean = False
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    'Write(deviceobject, Command, Param(0), Param(1))
                    Select Case UCase(Command)
                        Case ""
                        Case Else
                    End Select
                    Return True
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            WriteLog("ERR: ExecuteCommand exception : " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>Permet de vérifier si un champ est valide</summary>
    ''' <param name="Champ">Nom du champ à vérifier, ex ADRESSE1</param>
    ''' <param name="Value">Valeur à vérifier</param>
    ''' <returns>Retourne 0 si OK, sinon un message d'erreur</returns>
    ''' <remarks></remarks>
    Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
        Try
            Dim retour As String = "0"
            Select Case UCase(Champ)
                Case "ADRESSE1"
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez choisir l'équipement"
                        End If
                    End If
                Case "ADRESSE2"

            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    ''' <summary>Démarrer le driver</summary>
    ''' <remarks></remarks>
    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            'récupération des paramétres avancés
            If My.Computer.Network.IsAvailable = False Then
                _IsConnect = False
                WriteLog("ERR: Pas d'accés réseau! Vérifiez votre connection")
                WriteLog("Driver non démarré")
                Exit Sub
            End If

            Try
                _DEBUG = _Parametres.Item(0).Valeur
                _UserName = _Parametres.Item(1).Valeur
                _Password = _Parametres.Item(2).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Start, Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            WriteLog("Start, connection au serveur " & _UrlSencrop)

            If Get_Token(_UrlSencropApi & "/v1/users/sign_in", _UserName, _Password) Then
                _IsConnect = True
                WriteLog("Driver démarré avec succés à l'adresse " & _UrlSencrop)

                Get_ListStations(_UrlSencropApi & "/v1/organisations/" & detailorganisation.id & "/devices?limit=100&start=0")

                'lance le time du driver, mini toutes les minutes
                If _Refresh = 0 Then _Refresh = 300  ' 5mn
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick



                Dim id_stat As String = "15691"
                For i = 0 To listestat.Count - 1
                    If id_stat = listestat.Item(i).id Then
                        id_stat = i
                        Exit For
                    End If
                Next
            Else
                _IsConnect = False
                WriteLog("ERR: Start, Driver non démarré à l'adresse " & _UrlSencrop)
            End If
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Start, Driver Erreur démarrage " & ex.Message)
            WriteLog("Driver non démarré")
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            MyTimer.Enabled = False
            WriteLog("Driver " & Me.Nom & " arrêté")
        Catch ex As Exception
            WriteLog("ERR: Driver " & Me.Nom & " Erreur arrêt " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représetant le device à interroger</param>
    ''' <remarks>Le device demande au driver d'aller le lire suivant son adresse</remarks>
    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Dim id_stat As String = Mid(Objet.adresse1, 1, InStr(Objet.adresse1, "#") - 1)
            For i = 0 To listestat.Count - 1
                If id_stat = listestat.Item(i).id Then
                    id_stat = i
                    Exit For
                End If
            Next

            Select Case Objet.Type
                Case "METEO"
                    If InStr(Objet.adresse1, "All") <> 0 Then Exit Sub 'ne lance pas cette routine pour un all
                    If Not Get_Previsions(id_stat) Then Exit Sub

                    Objet.TemperatureActuel = Regex.Replace(CStr(listestat.Item(id_stat).datas.temperature), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.HumiditeActuel = Regex.Replace(CStr(listestat.Item(id_stat).datas.humidity), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

                    ' conversion den degre celsius
                    Objet.MaxToday = Regex.Replace(CStr(listestat.Item(id_stat).datas.Today.temperatureMax), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MaxJ1 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ1.temperatureMax), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MaxJ2 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ2.temperatureMax), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MaxJ3 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ3.temperatureMax), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MinToday = Regex.Replace(CStr(listestat.Item(id_stat).datas.Today.temperatureMin), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MinJ1 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ1.temperatureMin), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MinJ2 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ2.temperatureMin), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Objet.MinJ3 = Regex.Replace(CStr(listestat.Item(id_stat).datas.TodayJ3.temperatureMin), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

                    Dim st_icon As String
                    Objet.ConditionToday = listestat.Item(id_stat).datas.Today.summary
                    st_icon = listestat.Item(id_stat).datas.Today.icon.Replace("-day", "")
                    st_icon = st_icon.Replace("-", "_")
                    Objet.IconToday = st_icon
                    Objet.ConditionJ1 = listestat.Item(id_stat).datas.TodayJ1.summary
                    st_icon = listestat.Item(id_stat).datas.TodayJ1.icon.Replace("-day", "")
                    st_icon = st_icon.Replace("-", "_")
                    Objet.IconJ1 = st_icon
                    Objet.ConditionJ2 = listestat.Item(id_stat).datas.TodayJ2.summary
                    st_icon = listestat.Item(id_stat).datas.TodayJ2.icon.Replace("-day", "")
                    st_icon = st_icon.Replace("-", "_")
                    Objet.IconJ2 = st_icon
                    Objet.ConditionJ3 = listestat.Item(id_stat).datas.TodayJ3.summary
                    st_icon = listestat.Item(id_stat).datas.TodayJ3.icon.Replace("-day", "")
                    st_icon = st_icon.Replace("-", "_")
                    Objet.IconJ3 = st_icon

                    Objet.JourToday = TraduireJour(Mid(Now.DayOfWeek.ToString, 1, 3))
                    Objet.JourJ1 = TraduireJour(Mid(Now.AddDays(1).DayOfWeek.ToString, 1, 3))
                    Objet.JourJ2 = TraduireJour(Mid(Now.AddDays(2).DayOfWeek.ToString, 1, 3))
                    Objet.JourJ3 = TraduireJour(Mid(Now.AddDays(3).DayOfWeek.ToString, 1, 3))

                    Objet.VentActuel = Regex.Replace(CStr(listestat.Item(id_stat).datas.windSpeed), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)

                Case "BATTERIE"
                    Objet.Value = listestat.Item(id_stat).battery
                Case "TEMPERATURE"
                    Objet.Value = listestat.Item(id_stat).datas.temperature
                Case "HUMIDITE"
                    Objet.Value = listestat.Item(id_stat).datas.humidity
                Case "PLUIETOTAL"
                    Objet.Value = listestat.Item(id_stat).datas.precipitation7j
                Case "PLUIECOURANT"
                    Objet.Value = listestat.Item(id_stat).datas.precipitation
                Case "VITESSEVENT"
                    Objet.Value = listestat.Item(id_stat).datas.windSpeed
                Case "DIRECTIONVENT"
                    Objet.Value = listestat.Item(id_stat).datas.windGust

                Case "GENERIQUESTRING"
                    Dim jsonstr As String = "["
                    For i = 0 To listestat.Count - 1
                        If InStr(Objet.adresse1, "All") > 0 Then
                            jsonstr += "{""origine"":""Sencrop"",""ident"":""" & listestat.Item(i).serial & """,""lat_long"":""" & listestat.Item(i).latitude & ";" & listestat.Item(i).longitude & """,""altitude"":""" & listestat.Item(i).altitude & """,""parametre"":""TEMP-AIR_H1-FL0000_lastrecord"",""value"":""" & listestat.Item(i).datas.temperature & """}"
                        Else
                            If InStr(Objet.adresse1, listestat.Item(i).id) > 0 Then
                                jsonstr = "{""origine"":""Sencrop"",""ident"":""" & listestat.Item(i).serial & """,""lat_long"":""" & listestat.Item(i).latitude & ";" & listestat.Item(i).longitude & """,""altitude"":""" & listestat.Item(i).altitude & """,""parametre"":""TEMP-AIR_H1-FL0000_lastrecord"",""value"":""" & listestat.Item(i).datas.temperature & """}"
                                Exit For
                            End If
                        End If
                    Next
                    jsonstr += "]"
                    Objet.value = jsonstr
                Case Else
                    WriteLog("ERR: GetData=> Pas de valeur enregistrée")
                    Exit Sub
            End Select

        Catch ex As Exception
            WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
            WriteLog("ERR: Read, Exception : " & ex.Message)
        End Try

    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représetant le device à commander</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1">parametre 1 de la commande, optionnel</param>
    ''' <param name="Parametre2">parametre 2 de la commande, optionnel</param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
                _UserName = _Parametres.Item(1).Valeur
                _Password = _Parametres.Item(2).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

        Catch ex As Exception
            WriteLog("ERR: WRITE, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de la suppression d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice
        Try

        Catch ex As Exception
            WriteLog("ERR: DeleteDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Fonction lancée lors de l'ajout d'un device</summary>
    ''' <param name="DeviceId">Objet représetant le device à interroger</param>
    ''' <remarks></remarks>
    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice
        Try
        Catch ex As Exception
            WriteLog("ERR: NewDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout des commandes avancées pour les devices</summary>
    ''' <param name="nom">Nom de la commande avancée</param>
    ''' <param name="description">Description qui sera affichée dans l'admin</param>
    ''' <param name="nbparam">Nombre de parametres attendus</param>
    ''' <remarks></remarks>
    Private Sub Add_DeviceCommande(ByVal Nom As String, ByVal Description As String, ByVal NbParam As Integer)
        Try
            Dim x As New DeviceCommande
            x.NameCommand = Nom
            x.DescriptionCommand = Description
            x.CountParam = NbParam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_DeviceCommande, Exception :" & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout Libellé pour le Driver</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDriver(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim y0 As New HoMIDom.HoMIDom.Driver.cLabels
            y0.LabelChamp = Labelchamp
            y0.NomChamp = UCase(Nom)
            y0.Tooltip = Tooltip
            y0.Parametre = Parametre
            _LabelsDriver.Add(y0)
        Catch ex As Exception
            WriteLog("ERR: add_LibelleDriver, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Ajout Libellé pour les Devices</summary>
    ''' <param name="nom">Nom du champ : HELP</param>
    ''' <param name="labelchamp">Nom à afficher : Aide, si = "@" alors le champ ne sera pas affiché</param>
    ''' <param name="tooltip">Tooltip à afficher au dessus du champs dans l'admin</param>
    ''' <remarks></remarks>
    Private Sub Add_LibelleDevice(ByVal Nom As String, ByVal Labelchamp As String, ByVal Tooltip As String, Optional ByVal Parametre As String = "")
        Try
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            ld0.LabelChamp = Labelchamp
            ld0.NomChamp = UCase(Nom)
            ld0.Tooltip = Tooltip
            ld0.Parametre = Parametre
            _LabelsDevice.Add(ld0)
        Catch ex As Exception
            WriteLog("ERR: add_LibelleDevice, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>ajout de parametre avancés</summary>
    ''' <param name="nom">Nom du parametre (sans espace)</param>
    ''' <param name="description">Description du parametre</param>
    ''' <param name="valeur">Sa valeur</param>
    ''' <remarks></remarks>
    Private Sub Add_ParamAvance(ByVal nom As String, ByVal description As String, ByVal valeur As Object)
        Try
            Dim x As New HoMIDom.HoMIDom.Driver.Parametre
            x.Nom = nom
            x.Description = description
            x.Valeur = valeur
            _Parametres.Add(x)
        Catch ex As Exception
            WriteLog("ERR: add_ParamAvance, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.METEO)
            ' _DeviceSupport.Add(ListeDevices.BAROMETRE)
            _DeviceSupport.Add(ListeDevices.DIRECTIONVENT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            '            _DeviceSupport.Add(ListeDevices.UV)
            _DeviceSupport.Add(ListeDevices.VITESSEVENT)
            _DeviceSupport.Add(ListeDevices.TEMPERATURE)
            _DeviceSupport.Add(ListeDevices.BATTERIE)
            _DeviceSupport.Add(ListeDevices.PLUIECOURANT)
            _DeviceSupport.Add(ListeDevices.HUMIDITE)
            _DeviceSupport.Add(ListeDevices.PLUIETOTAL)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)
            '     _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE)


            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Username", "Nom utilisateur", "admin")
            Add_ParamAvance("Password", "Mot de passe", "homi123456")

            'ajout des commandes avancées pour les devices
            '            Add_DeviceCommande("ALL_LIGHT_ON", "@", 0)
 '           Add_DeviceCommande("ALL_LIGHT_OFF", "@", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("SOLO", "@", "Numéro de la station")
            Add_LibelleDevice("ADRESSE1", "Numéro de la station", "Numéro de la station")
            Add_LibelleDevice("ADRESSE2", "@", "Choix de la valeur")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        If (_Enable = False) Or (_IsConnect = False) Then
            'arrete le timer du driver
            MyTimer.Enabled = False
            Exit Sub
        End If

        Get_DatasStation(_UrlSencropApi)

    End Sub


#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver

    Function Get_Token(adrs As String, user As String, password As String) As Boolean
        ' recupere les configuration de sencrop

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim reqparam As String = ""
            Dim responsebodystr As String = ""

            '-------------------------------------------------------------------------------------------------------
            'recuperation d'un cookies
            request = CType(WebRequest.Create(_UrlSencrop & "/login"), HttpWebRequest)
            request.KeepAlive = True
            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()
            cookieToken = request.CookieContainer

            'recuperation du token
            reqparam = "{""email"":""" & user & """,""password"":""" & password & """}"
            reqparam = reqparam.PadRight(46)
            WriteLog("DBG: Get_Token, reqparam -> '" & reqparam & "' / " & reqparam.Length)
            Dim postBytes As Byte() = Encoding.ASCII.GetBytes(reqparam)
            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            request.ContentType = "application/json;charset=utf-8"
            Request.Method = "POST"
            Request.KeepAlive = True
            Request.ContentLength = reqparam.Length
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0"
            request.Accept = "application/json, text/plain, */*"
            request.Host = "api.sencrop.com"
            request.Referer = "https://app.sencrop.com"
            Request.Headers.Add("Origin", "https://app.sencrop.com")
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br")
            request.CookieContainer = cookieToken

            Dim writer As New StreamWriter(Request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            response = request.GetResponse()

            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            WriteLog("DBG: Get_Token, responsebodystr -> " & responsebodystr)

            authenconnec = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr, GetType(Sign_In))
            '--------------------------------------------------------------
            'lecture id organisation
            request = CType(WebRequest.Create(_UrlSencropApi & "/v1/users/" & authenconnec.userId & "/organisations"), HttpWebRequest)
            request.Method = "GET"
            request.KeepAlive = True
            request.Host = "api.sencrop.com"
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0"
            request.Accept = "application/json, text/plain, */*"
            request.Headers.Add("Accept-Language", "fr-FR")
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br")
            request.Headers.Add("Authorization", "Bearer 96cbaddd9804d4227bb472523bbb28c0ef89dab218d14a9537e2fe5b72f86882356ee458e0ccb93d54cb38c45a09677a411cd938e8b6c63ef704a937ec562558add3b893b5c75c111de73ca65fade574f68f40de07095e65e0ffbd76af6fdcf97dfded38be6d1dcec75f63155f1f8ad1f3be47b8451ea9736bcd240bf3f94422")
            request.Headers.Add("Origin", "https://app.sencrop.com")
            request.Referer = "https://app.sencrop.com"
            request.Headers.Add("TE", "Trailers")
            request.Headers.Add("DNT", "1")
            request.CookieContainer = cookieToken


            response = request.GetResponse()
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            ' WriteLog("DBG: Get_Token, responsebodystr -> " & responsebodystr)

            Dim jsonorganisations As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)
            Dim org As JProperty
            For Each org In jsonorganisations("organisations")
                If org.Name = authenconnec.organisationsIds(0) Then
                    detailorganisation.id = org.Value("id").ToString
                    detailorganisation.name = org.Value("contents")("name").ToString
                    detailorganisation.crops = org.Value("contents")("crops").ToString
                    WriteLog("Organisation -> " & org.Value("contents")("name").ToString)
                End If
            Next
            '--------------------------------------------------------------------------
            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_Token, " & ex.Message)
            WriteLog("ERR: " & "GET_Token, Url: " & adrs)
            Return False
        End Try
    End Function
    Function Get_ListStations(adrs As String) As Boolean
        ' recupere les device de sencrop
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim responsebodystr As String = ""

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            request.Method = "GET"
            request.KeepAlive = True
            request.Host = "api.sencrop.com"
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0"
            request.Accept = "application/json, text/plain, */*"
            request.Headers.Add("Accept-Language", "fr-FR")
            request.Headers.Add("Authorization", "Bearer 96cbaddd9804d4227bb472523bbb28c0ef89dab218d14a9537e2fe5b72f86882356ee458e0ccb93d54cb38c45a09677a411cd938e8b6c63ef704a937ec562558add3b893b5c75c111de73ca65fade574f68f40de07095e65e0ffbd76af6fdcf97dfded38be6d1dcec75f63155f1f8ad1f3be47b8451ea9736bcd240bf3f94422")
            request.Headers.Add("Origin", "https://app.sencrop.com")
            request.Referer = "https://app.sencrop.com"
            request.Headers.Add("TE", "Trailers")
            request.Headers.Add("DNT", "1")
            request.CookieContainer = cookieToken

            WriteLog("DBG: Get_ListStation, recherche des devices à l'adresse -> " & adrs)

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            WriteLog("DBG: Get_ListStation, responsebodystr -> " & responsebodystr)

            Dim jsonliststation As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)

            For Each jsonstation As JProperty In jsonliststation
                If InStr(jsonstation.Name.ToString, "items") > 0 Then
                    Dim str = jsonstation.Value.ToString
                    str = Replace(str, "[", "")
                    str = Replace(str, "]", ",")
                    While InStr(str, ",") > 0
                        Dim station As New Stations
                        station.id = Mid(str, 1, InStr(str, ",") - 1)
                        listestat.Add(station)
                        str = Mid(str, InStr(str, ",") + 1, Len(str))
                        station = Nothing
                    End While
                End If
            Next
            WriteLog(listestat.Count & " stations trouvées")

            For i = 0 To listestat.Count - 1
                For Each jsonstation As JProperty In jsonliststation("devices")(listestat.Item(i).id.ToString)
                    '                   WriteLog("DBG: " & jsonstation.Name.ToString & " -> " & jsonstation.Value.ToString)
                    Select Case True
                        Case jsonstation.Name.ToString = "identification"
                            listestat.Item(i).identification = jsonstation.Value.ToString
                        Case jsonstation.Name.ToString = "serial"
                            listestat.Item(i).serial = jsonstation.Value.ToString
                        Case jsonstation.Name.ToString = "contents"
                            listestat.Item(i).name = jsonstation.Value("name").ToString
                        Case jsonstation.Name.ToString = "location"
                            listestat.Item(i).latitude = jsonstation.Value("latitude").ToString
                            listestat.Item(i).longitude = jsonstation.Value("longitude").ToString
                            listestat.Item(i).altitude = jsonstation.Value("altitude").ToString
                        Case jsonstation.Name.ToString = "status"
                            listestat.Item(i).firmware = jsonstation.Value("firmware").ToString
                            listestat.Item(i).signal = jsonstation.Value("signal").ToString
                            If Not IsNothing(jsonstation.Value("battery")) Then listestat.Item(i).battery = jsonstation.Value("battery").ToString
                    End Select
                Next
            Next

            Dim _libelleadr1 As String = "0 # All|"
            Dim _libelleadr2 As String = "0 # Température|0 # Pluviometrie|0 # Humidité"

            For i = 0 To listestat.Count - 1
                _libelleadr1 += listestat.Item(i).id & " # " & listestat.Item(i).name & "|"
            Next

            ' evite les doublons 
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            For i As Integer = 0 To _LabelsDevice.Count - 1
                ld0 = _LabelsDevice(i)
                Select Case ld0.NomChamp
                    Case "ADRESSE1"
                        _libelleadr1 = Mid(_libelleadr1, 1, Len(_libelleadr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = _libelleadr1
                        _LabelsDevice(i) = ld0
                    Case "ADRESSE2"
                        _libelleadr2 = Mid(_libelleadr2, 1, Len(_libelleadr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = _libelleadr2
                        _LabelsDevice(i) = ld0
                End Select
            Next

            Get_DatasStation(_UrlSencropApi)

            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_Get_ListStation, " & ex.Message)
            WriteLog("ERR: " & "GET_Get_ListStation, Url: " & adrs)
            Return False
        End Try
    End Function
    Function Get_DatasStation(adrs As String) As Boolean
        ' recupere les device de sencrop

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim responsebodystr As String = ""

            Dim liststation As String = ""
            For i = 0 To listestat.Count - 1
                liststation += listestat.Item(i).id & ","
            Next
            liststation = HttpUtility.HtmlEncode(Mid(liststation, 1, Len(liststation) - 1))

            request = CType(WebRequest.Create(_UrlSencropApi & "/v1/organisations/" & detailorganisation.id & "/devices/liveAggregations?devicesIds=" & liststation & "&aggregations=TEMPERATURE_LAST%2CTEMPERATURE_MIN%2CTEMPERATURE_MAX%2CRELATIVE_HUMIDITY_LAST%2CRELATIVE_HUMIDITY_MIN%2CRELATIVE_HUMIDITY_MAX%2CWET_TEMPERATURE_LAST%2CRAIN_FALL_SUM%2CWIND_SPEED_LAST%2CWIND_GUST_MAX%2CWIND_DIRECTION_MEAN%2CLEAF_WETNESS%2CLEAF_WETNESS_MEDIUM%2CLEAF_WETNESS_HIGH&intervals=one_hour%2Ctoday%2Cyesterday%2Clast_seven_days&timeZone=Europe%2FParis&date=" & HttpUtility.HtmlEncode(Now.ToString("s")) & ".681%2B01%3A00"), HttpWebRequest)
            request.Method = "GET"
            request.KeepAlive = True
            request.Host = "api.sencrop.com"
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0"
            request.Accept = "application/json, text/plain, */*"
            request.Headers.Add("Accept-Language", "fr-FR")
            request.Headers.Add("Authorization", "Bearer 96cbaddd9804d4227bb472523bbb28c0ef89dab218d14a9537e2fe5b72f86882356ee458e0ccb93d54cb38c45a09677a411cd938e8b6c63ef704a937ec562558add3b893b5c75c111de73ca65fade574f68f40de07095e65e0ffbd76af6fdcf97dfded38be6d1dcec75f63155f1f8ad1f3be47b8451ea9736bcd240bf3f94422")
            request.Headers.Add("Origin", "https://app.sencrop.com")
            request.Referer = "https://app.sencrop.com"
            request.Headers.Add("TE", "Trailers")
            request.Headers.Add("DNT", "1")
            request.CookieContainer = cookieToken

            WriteLog("DBG: DatasStation, recherche des datas à l'adresse -> " & request.Address.AbsoluteUri.ToString)

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            WriteLog("DBG: DatasStation, responsebodystr -> " & responsebodystr)

            Dim jsondatastation As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)

            For i = 0 To listestat.Count - 1
                For Each jsondata As JProperty In jsondatastation("devicesLiveAggregations")(listestat.Item(i).id.ToString)
                    '                   WriteLog("DBG: DatasStation, name -> " & jsondata.Name.ToString & " -> " & jsondata.Value.ToString)
                    Select Case True
                        Case jsondata.Name.ToString = "TEMPERATURE_LAST"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.temperature = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "TEMPERATURE_MIN"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.temperatureMin = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "TEMPERATURE_MAX"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.temperatureMin = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "RELATIVE_HUMIDITY_LAST"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.humidity = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "RAIN_FALL_SUM"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.precipitation = jsondata.Value("today").ToString
                            If Not IsNothing(jsondata.Value("last_seven_days")) Then listestat.Item(i).datas.precipitation7j = jsondata.Value("last_seven_days").ToString
                        Case jsondata.Name.ToString = "WIND_SPEED_LAST"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.windSpeed = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "WIND_GUST_MAX"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.windGust = jsondata.Value("today").ToString
                        Case jsondata.Name.ToString = "WIND_DIRECTION_MEAN"
                            If Not IsNothing(jsondata.Value("today")) Then listestat.Item(i).datas.windBearing = jsondata.Value("today").ToString
                    End Select
                Next
            Next

            'WriteLog("DBG: DatasStation, terminé avec succés -> ")
            Return True

        Catch ex As Exception
            WriteLog("ERR: " & "GET_DatasStation, " & ex.Message)
            WriteLog("ERR: " & "GET_DatasStation, Url: " & adrs)
            Return False
        End Try
    End Function

    Function Get_Previsions(idstat As Integer) As Boolean

        Try
            'https://api.sencrop.com/v1/users/1138/forecasts?latitude=43.9127&longitude=1.98837&date=2019-12-26T12%3A08%3A31%2B01%3A00

            Dim adrs = _UrlSencropApi & "/v1/users/" & authenconnec.userId & "/forecasts?latitude=" & listestat.Item(idstat).latitude & "&longitude=" & listestat.Item(idstat).longitude & "&date=" & HttpUtility.HtmlEncode(Now.ToString("s")) & ".681%2B01%3A00"

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 'rajout pout TSL1.2 sinon plantage
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim responsebodystr As String = ""

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            request.Method = "GET"
            request.KeepAlive = True
            request.Host = "api.sencrop.com"
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0"
            request.Accept = "application/json, text/plain, */*"
            request.Headers.Add("Accept-Language", "fr-FR")
            request.Headers.Add("Authorization", "Bearer 96cbaddd9804d4227bb472523bbb28c0ef89dab218d14a9537e2fe5b72f86882356ee458e0ccb93d54cb38c45a09677a411cd938e8b6c63ef704a937ec562558add3b893b5c75c111de73ca65fade574f68f40de07095e65e0ffbd76af6fdcf97dfded38be6d1dcec75f63155f1f8ad1f3be47b8451ea9736bcd240bf3f94422")
            request.Headers.Add("Origin", "https://app.sencrop.com")
            request.Referer = "https://app.sencrop.com"
            request.Headers.Add("TE", "Trailers")
            request.Headers.Add("DNT", "1")
            request.CookieContainer = cookieToken

            WriteLog("DBG: Get_Previsions, recherche des prévisions à l'adresse -> " & request.Address.AbsoluteUri.ToString)

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            WriteLog("DBG: Get_Previsions, responsebodystr -> " & responsebodystr)

            Dim jsondataprevision As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)
            listestat.Item(idstat).datas.Today = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(jsondataprevision("daily")("data")(0)), GetType(PrevisionDetail))
            listestat.Item(idstat).datas.TodayJ1 = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(jsondataprevision("daily")("data")(1)), GetType(PrevisionDetail))
            listestat.Item(idstat).datas.TodayJ2 = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(jsondataprevision("daily")("data")(2)), GetType(PrevisionDetail))
            listestat.Item(idstat).datas.TodayJ3 = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(jsondataprevision("daily")("data")(3)), GetType(PrevisionDetail))

            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "Get_Previsions, " & ex.Message)
            Return False
        End Try
    End Function

    Private Function TraduireJour(ByVal Jour As String) As String
        Try
            TraduireJour = "?"
            Select Case Jour
                Case "Thu"
                    TraduireJour = "Jeu"
                Case "Fri"
                    TraduireJour = "Ven"
                Case "Sat"
                    TraduireJour = "Sam"
                Case "Sun"
                    TraduireJour = "Dim"
                Case "Mon"
                    TraduireJour = "Lun"
                Case "Tue"
                    TraduireJour = "Mar"
                Case "Wed"
                    TraduireJour = "Mer"
            End Select
        Catch ex As Exception
            WriteLog("ERR: TraduireJour, " & ex.ToString)
            Return "?"
        End Try
    End Function

    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom, STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom, message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " WriteLog", ex.Message)
        End Try

    End Sub
#End Region
End Class
