Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings

'Imports System.Web
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports System.String
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
'Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO
Imports System.Web
Imports System.Web.UI
Imports System.ComponentModel.Design

<Serializable()> Public Class Driver_WifiTic
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "B7239980-2AE2-11ED-9B72-BF8FAFD2C500" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "WifiTic" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver WifiTic" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 30 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
    Dim _Modele As String = "" 'Modèle du driver/interface
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


    ' differerente url
    ' /rest/configuration -> config
    ' /rest/teleinfo -> datas teleinfo

    Dim _IPAdressWifiTic As String = "192.168.8.1"
    Dim _IPPortWifiTic As String = "80"
    Dim _UrlWifiTic As String = ""
    Dim _UserWifiTic As String = "WifiTic"
    Dim _PasswordWifiTic As String = "password"

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Public StatutTeleInfo As New TeleInfoStatut
    Public Phases As New List(Of Phase)

    Public Class TeleInfoStatut
        Public DeviceName As String
        Public Version As String
        Public ApiVersion As String
        Public Authentification As String
        Public PSouscrite As String
        Public Teleinfo As Boolean = False
        Public modulation As String = "Historique"
    End Class

    Public Class Phase
        Public Numero As Integer
        Public Periode As Integer
        Public app As Integer
        Public iinst As Integer
        Public voltage As Integer
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
            If _Enable = False Then
                WriteLog("ERR: Read, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Return False
            End If

            If MyDevice IsNot Nothing Then
                If Command = "" Then
                    Return False
                Else
                    'mise a jour de la configuration
                    WriteLog("DBG: ExecuteCommand, Passage de la commande " & UCase(Command) & " Param(0) -> " & Param(0) & " / Param(1) -> " & Param(1))

                    ' Traitement de la commande 
                    Select Case UCase(Command)
                    End Select
                    Return False
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
                        If String.IsNullOrEmpty(Value) Then
                            retour = "Veuillez renseigner un numéro de phase " & Value
                        End If
                    End If
                Case "ADRESSE2"

            End Select
            Return retour
        Catch ex As Exception
            Return "ERR: Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
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
                _IPAdressWifiTic = _Parametres.Item(1).Valeur
                _IPPortWifiTic = _Parametres.Item(2).Valeur
                _UserWifiTic = _Parametres.Item(3).Valeur
                _PasswordWifiTic = _Parametres.Item(4).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _UrlWifiTic = "http://" & _IPAdressWifiTic & ":" & _IPPortWifiTic
            WriteLog("Start, Tentative de connection au serveur " & _UrlWifiTic)

            If GetConfig(_UserWifiTic, _PasswordWifiTic) Then
                WriteLog("Modem " & StatutTeleInfo.DeviceName & " - softversion " & StatutTeleInfo.Version & " - Puis. souscrite " & StatutTeleInfo.PSouscrite & " A")
                WriteLog("Driver démarré avec succés à l'adresse " & _UrlWifiTic)
                _IsConnect = True

                GetTeleinfo(_UserWifiTic, _PasswordWifiTic)

                'lance le time du driver, mini toutes les 1 minutes
                If _Refresh = 0 Then _Refresh = 60
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick

            Else
                _IsConnect = False
                WriteLog("ERR: Start, Erreur démarrage")
            End If

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Start, Erreur démarrage " & ex.Message)
            WriteLog("Driver non démarré")
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
            MyTimer.Enabled = False
            WriteLog("Driver arrêté")
        Catch ex As Exception
            WriteLog("ERR: Stop " & " Erreur arrêt " & ex.Message)
        End Try
    End Sub

    ''' <summary>Re-Démarrer le du driver</summary>
    ''' <remarks></remarks>
    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    ''' <summary>Intérroger un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
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
                WriteLog("ERR: READ, Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Dim valeur As Object = Nothing

            '|Puissance apparente en Kva|Intensité instantanée en A|Tension en V|
            Dim numphase As Integer = Mid(Objet.adresse1, 1, InStr(Objet.adresse1, "#") - 1)
            'WriteLog("dbg: READ, numphase " & Phases(numphase).app)
            Select Case Objet.Type
                Case "GENERIQUESTRING"
                    Select Case True
                        Case InStr(Objet.adresse2, "téléinfo") > 0
                            Objet.Value = StatutTeleInfo.modulation
                    End Select
                Case "COMPTEUR"
                    Select Case True
                        Case InStr(Objet.adresse2, "apparente") > 0
                            Objet.Value = Regex.Replace(CStr(Phases(numphase).app), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Case InStr(Objet.adresse2, "Intensité") > 0
                            Objet.Value = Regex.Replace(CStr(Phases(numphase).iinst), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                        Case InStr(Objet.adresse2, "Tension") > 0
                            Objet.Value = Regex.Replace(CStr(Phases(numphase).voltage), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End Select
                Case "ENERGIEINSTANTANEE"
                    Select Case True
                        Case InStr(Objet.adresse2, "Intensité") > 0
                            Objet.Value = Regex.Replace(CStr(Phases(numphase).iinst), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    End Select
            End Select
        Catch ex As Exception
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
                WriteLog("ERR: WRITE, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If
            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: WRITE,Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: WRITE, Pas de réseau! Lecture du périphérique impossible")
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
            WriteLog("ERR: Add_DeviceCommande, Exception :" & ex.Message)
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
            WriteLog("ERR: Add_LibelleDriver, Exception : " & ex.Message)
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
            WriteLog("ERR: Add_LibelleDevice, Exception : " & ex.Message)
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
            WriteLog("ERR: Add_ParamAvance, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Creation d'un objet de type</summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

            'liste des devices compatibles
            _DeviceSupport.Add(ListeDevices.COMPTEUR)
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("IPAdress", "Adresse IP", "192.168.1.1")
            Add_ParamAvance("IPPort", "Port IP", "80")
            Add_ParamAvance("User", "User", "admin")
            Add_ParamAvance("Password", "Password", "wifitic")

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Phase", "")
            Add_LibelleDevice("ADRESSE2", "Paramètre", "")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes", _Refresh.ToString)
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

        _DEBUG = _Parametres.Item(0).Valeur
        If (_Enable = False) Or (_IsConnect = False) Then
            'arrete le timer du driver
            MyTimer.Enabled = False
            Exit Sub
        End If

        GetTeleinfo(_UserWifiTic, _PasswordWifiTic)

    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)

    Private Function GetConfig(user As String, password As String) As Boolean
        ' recupere les configurations 
        ' http://192.168.x.xxx/rest/configuration
        ' mode historique : {"hostname""wifiTIC_E738D4","IP":"192.168.x.xxx","Gateway":"192.168.x.xxx","Netmask":"255.255.255.0","mac":"C4:XX:XX:XX:XX:D4","Version":"1.3.4","APIVersion":"1.3.1","partitionUsed":0.42662874411163454,"runningTime":85804372,"authentication":false,"localLog":false,"coredump":false}

        Try

            Dim responsebodystr As String = ""
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim adrs As String = ""

            'recherche de la configuration
            'http://192.168.0.176/rest/configuration
            adrs = "http://" & _IPAdressWifiTic & "/rest/configuration"

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            WriteLog("DBG: GetConfig, recherche de la configuration a l'adresse -> " & request.Address.ToString)

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            responsebodystr = HttpUtility.HtmlDecode(responsebodystr)
            'WriteLog("DBG: GetData, responsebodystr -> " & responsebodystr)

            Dim jsonObj As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)
            'WriteLog("DBG: GetData, jsonobj.count -> " & jsonObj.Count)

            StatutTeleInfo.DeviceName = jsonObj.Item("hostname").ToString
            StatutTeleInfo.Version = jsonObj.Item("Version").ToString
            StatutTeleInfo.ApiVersion = jsonObj.Item("APIVersion").ToString

            'recherche des info teleinfo
            adrs = "http://" & _IPAdressWifiTic & "/rest/teleinfo"

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            'WriteLog("DBG: GetConfig, recherche de teleinfo a l'adresse -> " & request.Address.ToString)

            response = request.GetResponse()
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            responsebodystr = HttpUtility.HtmlDecode(responsebodystr)
            'WriteLog("DBG: GetData, responsebodystr -> " & responsebodystr)

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)
            'WriteLog("DBG: GetData, jsonobj.count -> " & jsonObj.Count)

            If jsonObj.Item("counter").Item("modulation").ToString = "2" Then
                StatutTeleInfo.modulation = "Standard"
                StatutTeleInfo.PSouscrite = jsonObj.Item("counter").Item("psous").ToString
            Else
                StatutTeleInfo.PSouscrite = jsonObj.Item("counter").Item("isous").ToString
            End If

            Dim IdAdr1 As String = ""
            Dim IdAdr2 As String = ""


            For i = 0 To jsonObj.Item("consumption").Item("phases").Count - 1
                IdAdr1 += i & " # Phase " & i + 1 & "|"
                IdAdr2 += i & " #; |Puissance apparente en Kva|Intensité instantanée en A|Tension en V|Type téléinfo|"
            Next

            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            For i As Integer = 0 To _LabelsDevice.Count - 1
                ld0 = _LabelsDevice(i)
                Select Case ld0.NomChamp
                    Case "ADRESSE1"
                        IdAdr1 = Mid(IdAdr1, 1, Len(IdAdr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = IdAdr1
                        _LabelsDevice(i) = ld0
                    Case "ADRESSE2"
                        IdAdr2 = Mid(IdAdr2, 1, Len(IdAdr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = IdAdr2
                        _LabelsDevice(i) = ld0
                End Select
            Next


            Return True
        Catch ex As Exception

            WriteLog("ERR: GETConfig, " & ex.Message)
            WriteLog("ERR: GETConfig, Url: " & _UrlWifiTic)
            Return False
        End Try
    End Function

    Private Function GetTeleinfo(user As String, password As String) As Boolean
        ' recupere les teleinfos 
        ' http://192.168.x.xxx/rest/teleinfo
        ' mode historique : {"time":"2022-09-22T20:30:04+0200","consumption":{"period":0,"phases":[{"app":670,"iinst":2,"imax":90}],"counters":[{"value":30206854}]},"counter":{"modulation":1,"state":"ok","id":"811875xxxxx","contract":1,"type":1,"isous":45}}
        ' mode standard : {"time":"2022-10-08T10:24:41+0200","consumption":{"period":0,"phases":[{"app":0,"iinst":4,"imax":0,"voltage":235}],"counters":[{"value":30269650}],"relay":{"r1":false,"r2":false,"r3":false,"r4":false,"r5":false,"r6":false,"r7":false,"r8":false},"currentDaySupplier":0,"nextDaySupplier":0},"counter":{"modulation":2,"state":"ok","id":"8118753xxxxx","contract":1,"type":1,"psous":9,"pcoup":9}}

        Try

            Dim responsebodystr As String = ""
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim adrs As String = ""

            'recherche des info teleinfo
            adrs = "http://" & _IPAdressWifiTic & "/rest/teleinfo"

            request = CType(WebRequest.Create(adrs), HttpWebRequest)
            WriteLog("DBG: GetTeleInfo, recherche de teleinfo a l'adresse -> " & request.Address.ToString)

            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            responsebodystr = HttpUtility.HtmlDecode(responsebodystr)
            'WriteLog("DBG: GetData, responsebodystr -> " & responsebodystr)

            Dim jsonObj As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)
            'WriteLog("DBG: GetData, jsonobj.count -> " & jsonObj.Count)

            Phases.Clear()

            For i = 0 To jsonObj.Item("consumption").Item("phases").Count - 1
                Dim tmpphase As New Phase
                tmpphase.Numero = i
                tmpphase.Periode = jsonObj.Item("consumption").Item("period").ToString
                tmpphase.iinst = jsonObj.Item("consumption").Item("phases")(i).Item("iinst").ToString
                tmpphase.app = jsonObj.Item("consumption").Item("phases")(i).Item("app").ToString
                tmpphase.voltage = 0
                'mode standard
                If StatutTeleInfo.modulation = "Standard" Then
                    tmpphase.voltage = jsonObj.Item("consumption").Item("phases")(i).Item("voltage").ToString
                    ' WriteLog("DBG: GetData, tmpphase.voltage -> " & tmpphase.voltage)
                End If

                Phases.Add(tmpphase)
            Next
            Return True
        Catch ex As Exception

            WriteLog("ERR: GETTeleInfo, " & ex.Message)
            WriteLog("ERR: GETTeleInfo, Url: " & _UrlWifiTic)
            Return False
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
