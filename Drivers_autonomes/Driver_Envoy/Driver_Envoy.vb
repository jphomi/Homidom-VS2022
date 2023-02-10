Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.String
Imports System.Text
Imports System.Web
Imports System.Xml


<Serializable()> Public Class Driver_envoy
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "C7F22EF0-C6B4-11EA-ADA8-DFE1FB2CA371" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "Envoy" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver Envoy" 'Description du driver
    Dim _StartAuto As Boolean = False 'True si le driver doit démarrer automatiquement
    Dim _Protocol As String = "Http" 'Protocole utilisé par le driver, exemple: RS232
    Dim _IsConnect As Boolean = False 'True si le driver est connecté et sans erreur
    Dim _IP_TCP As String = "@" 'Adresse IP TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _Port_TCP As String = "@" 'Port TCP à utiliser, "@" si non applicable pour le cacher côté client
    Dim _IP_UDP As String = "@" 'Adresse IP UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Port_UDP As String = "@" 'Port UDP à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Com As String = "@" 'Port COM à utiliser, , "@" si non applicable pour le cacher côté client
    Dim _Refresh As Integer = 0 'Valeur à laquelle le driver doit rafraichir les valeurs des devices (ex: toutes les 200ms aller lire les devices)
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

    Dim _IPAdressenvoy As String = "192.168.1.1"
    Dim _IPPortenvoy As String = "80"
    Dim _Urlenvoy As String = ""
    Dim _Userenvoy As String = "envoy"
    Dim _Passwordenvoy As String = "admin"


#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Public Production As New List(Of Product)
    Public ListInverters As List(Of Inverters) = New List(Of Inverters)


    Public Class Product
        Public wattHoursToday As Integer
        Public wattHoursSevenDays As Integer
        Public wattHoursLifetime As Integer
        Public wattsNow As Integer
    End Class


    Public Class Inverters
        Public serialNumber As String
        Public lastReportDate As Double
        Public devType As Integer
        Public lastReportWatts As Integer
        Public maxReportWatts As Integer
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
                If Command = "" Then
                    Return False
                Else
                    'mise a jour de la configuration
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
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Then
                            retour = "Veuillez choisir l'équipement"
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
                _IPAdressenvoy = _Parametres.Item(1).Valeur
                _IPPortenvoy = _Parametres.Item(2).Valeur
                _Userenvoy = _Parametres.Item(3).Valeur
                _Passwordenvoy = _Parametres.Item(4).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _Urlenvoy = "http://" & _IPAdressenvoy & ":" & _IPPortenvoy
            WriteLog("Start, connection au serveur " & _Urlenvoy)

            Dim reponse As String = Get_Config(_Urlenvoy & "/info.xml", _Userenvoy, _Passwordenvoy)
            If reponse <> "" Then
                _IsConnect = True
                WriteLog("Driver " & " démarré avec succés à l'adresse " & _Urlenvoy)
                WriteLog("Envoy S/N " & reponse)

                Get_ValueProd(_Urlenvoy, _Userenvoy, _Passwordenvoy)

            Else
                _IsConnect = False
                WriteLog("ERR: Start " & " Erreur démarrage ")
            End If
        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Start " & " Erreur démarrage " & ex.Message)
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
                WriteLog("ERR: READ, Erreur de lecture de debug : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: READ, Pas de réseau! Lecture du périphérique impossible")
                Exit Sub
            End If

            Select Case Objet.Type
                Case "ENERGIEINSTANTANEE"   '"Production instantanée|Production Jour|Production Semaine|Production Lifetime"
                    If Get_ValueProd(_Urlenvoy, _Userenvoy, _Passwordenvoy) Then
                        Select Case True
                            Case InStr(Objet.adresse1, "Global") > 0
                                WriteLog("DBG: READ, production.Item(0).wattsNow " & Production.Item(0).wattsNow)
                                Select Case True
                                    Case InStr(Objet.adresse2, "Production instantanée") > 0
                                        Objet.Value = Regex.Replace(CStr(Production.Item(0).wattsNow), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                    Case InStr(Objet.adresse2, "Production Jour") > 0
                                        Objet.Value = Regex.Replace(CStr(Production.Item(0).wattHoursToday), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                    Case InStr(Objet.adresse2, "Production Semaine") > 0
                                        Objet.Value = Regex.Replace(CStr(Production.Item(0).wattHoursSevenDays), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                    Case InStr(Objet.adresse2, "Production Lifetime") > 0
                                        Objet.Value = Regex.Replace(CStr(Production.Item(0).wattHoursLifetime), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                End Select
                            Case InStr(Objet.adresse1, "Global") = 0
                                Dim i As Integer = Mid(Objet.adresse2, 1, 1)
                                Select Case True
                                    Case InStr(Objet.adresse2, "lastReportWatts") > 0
                                        Objet.Value = Regex.Replace(CStr(ListInverters.Item(i - 1).lastReportWatts), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                    Case InStr(Objet.adresse2, "maxReportWatts") > 0
                                        Objet.Value = Regex.Replace(CStr(ListInverters.Item(i - 1).maxReportWatts), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                End Select
                        End Select
                    End If
            End Select
        Catch ex As Exception
            '  WriteLog("ERR: Read, adresse1 : " & Objet.adresse1 & " - adresse2 : " & Objet.adresse2)
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
            _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("IPAdress", "Adresse IP", "127.0.0.1")
            Add_ParamAvance("IPPort", "Port IP", "80")
            Add_ParamAvance("User", "User", "envoy")
            Add_ParamAvance("Password", "Password", "1234")

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            'add_devicecommande("PRESETDIM", "permet de paramétrer le DIM : param1=niveau, param2=timer", 2)
            '            Add_DeviceCommande("RUN", "Run scenario", 0)
            '            Add_DeviceCommande("STOP", "Run scenario", 0)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Mesure", "Mesure à relever")
            Add_LibelleDevice("ADRESSE2", "Production", "Production à relever")
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
            'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")

        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)
    Private Function Get_Config(adrs As String, user As String, password As String) As String
        ' recupere les configarions des equipements et scenarios de envoy

        Try
            Dim str As String
            Dim reponse As String = ""

            Dim webclient As New WebClient
            webclient.Credentials = New NetworkCredential(user, password)

            Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(adrs))
            reader.WhitespaceHandling = WhitespaceHandling.Significant
            WriteLog("DBG: Fichier xml -> " & adrs & " acquis")
            While reader.Read()
                str = reader.ReadString
                WriteLog("DBG: " & reader.Name & " -> " & str)
                Select Case reader.Name
                    Case "sn"
                        reponse = str
                    Case "software"
                        reponse = reponse & ", version logiciel " & str
                End Select
            End While

            Return reponse
        Catch ex As Exception

            WriteLog("ERR: " & "GET_Config, " & ex.Message)
            WriteLog("ERR: " & "GET_Config, Url: " & _Urlenvoy)
            Return ""
        End Try
    End Function

    Function Get_ValueProd(adrs As String, user As String, password As String) As Boolean
        'retourne l'état de la commande d'un equipement
        Try

            WriteLog("DBG: " & "GET_ValueProd Url: " & adrs)

            Dim client As New Net.WebClient
            client.Credentials = New NetworkCredential(user, password)

            Dim responsebody As String = client.DownloadString(adrs & "/api/v1/production")
            While client.IsBusy
            End While

            WriteLog("DBG: Get_ValueProd  : " & responsebody.ToString)

            production.Clear()
            production.Add(Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Product)))

            '   Get_ValueOnduleur(adrs, user, password)

            ' renseignement des champs adresses
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            For i As Integer = 0 To _LabelsDevice.Count - 1
                ld0 = _LabelsDevice(i)
                Select Case ld0.NomChamp
                    Case "ADRESSE1"
                        ld0.Parametre = "0 # Global|"
                        For j = 0 To ListInverters.Count - 1
                            ld0.Parametre += j + 1 & " # " & ListInverters.Item(j).serialNumber & "|"
                        Next
                        _LabelsDevice(i) = ld0
                    Case "ADRESSE2"
                        ld0.Parametre = "0 #; Production instantanée|0 #; Production Jour|0 #; Production Semaine|0 #; Production Lifetime|"
                        For j = 0 To ListInverters.Count - 1
                            ld0.Parametre += j + 1 & " #; lastReportWatts|"
                            ld0.Parametre += j + 1 & " #; maxReportWatts|"
                        Next
                        ld0.Parametre = Mid(ld0.Parametre, 1, Len(ld0.Parametre) - 1) 'enleve le dernier | pour eviter d'avoir une ligne vide a la fin de la liste
                        _LabelsDevice(i) = ld0
                End Select
            Next

            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_ValueProd, " & ex.Message)
            Return False
        End Try
    End Function

    Function Get_ValueOnduleur(adrs As String, user As String, password As String) As Boolean
        'retourne l'état de la commande d'un equipement
        Try

            adrs = adrs & "/api/v1/production/inverters"
            WriteLog("DBG: " & "GET_ValueOnduleur Url: " & adrs)

            Dim client As New Net.WebClient
            client.Credentials = New NetworkCredential(user, password)

            Dim responsebody As String = client.DownloadString(adrs)
            While client.IsBusy
            End While

            'enleve les caractere html de debut et fin de page
            responsebody = Mid(responsebody, InStr(responsebody, "[") + 1, Len(responsebody))
            responsebody = Mid(responsebody, 1, InStr(responsebody, "]") - 1)
            '    responsebody = responsebody.Replace(Chr(13), "")
            '    responsebody = responsebody.Replace(Chr(10), "")
            '  responsebody = responsebody.Replace(" ", "")
            WriteLog("DBG: Get_ValueOnduleur  : " & responsebody.ToString)

            ListInverters = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebody, GetType(Inverters))

            Return True
        Catch ex As Exception
            WriteLog("ERR: " & "GET_ValueOnduleur, " & ex.Message)
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
