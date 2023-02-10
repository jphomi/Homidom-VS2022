Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
'Imports System.Collections.Generic
Imports STRGS = Microsoft.VisualBasic.Strings
'Imports System.Linq
'Imports System.Threading.Tasks
'Imports System.IO

'Imports Serilog
Imports ZigBeeNet
Imports ZigBeeNet.App.Basic
Imports ZigBeeNet.App.Discovery
Imports ZigBeeNet.App.IasClient
Imports ZigBeeNet.Database
Imports ZigBeeNet.DataStore.Json
Imports ZigBeeNet.DataStore.MongoDb
Imports ZigBeeNet.Hardware.TI.CC2531
Imports ZigBeeNet.Hardware.ConBee
Imports ZigBeeNet.Hardware.Digi
Imports ZigBeeNet.Hardware.Ember
Imports ZigBeeNet.Security
Imports ZigBeeNet.Tranport.SerialPort
Imports ZigBeeNet.Transaction
Imports ZigBeeNet.Transport
Imports ZigBeeNet.Util
Imports ZigBeeNet.ZCL
Imports ZigBeeNet.ZCL.Clusters
Imports ZigBeeNet.ZCL.Clusters.ColorControl
Imports ZigBeeNet.ZCL.Clusters.General
Imports ZigBeeNet.ZCL.Clusters.LevelControl
Imports ZigBeeNet.ZCL.Clusters.OnOff
Imports ZigBeeNet.ZDO.Command
Imports ZigBeeNet.ZDO.Field
Imports ZigBeeNet.Serialization

' Auteur : JPHomi
' Date : 01/05/2021

''' <summary>Class Driver_Zigbee, permet de communiquer avec le controleur Zigbee</summary>
''' <remarks></remarks>
''' 
<Serializable()> Public Class Driver_Zigbee
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "DAD16084-96D6-11EB-A59C-C115FC2CA371"
    Dim _Nom As String = "Zigbee"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Controleur Zigbee"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "COM"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "COM1"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "@"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "3264"
    Dim _Picture As String = ""
    Shared _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Shared _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Shared _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'parametres avancés
    Shared _DEBUG As Boolean = False

    Private z_flowcontrol As FlowControl = FlowControl.FLOWCONTROL_OUT_NONE
    Shared z_manager As ZigBeeNetworkManager
    Shared z_ManagerNode0 As New ZigBeeNode
    Private z_jsondatastore As IZigBeeNetworkDataStore
    Private z_discoveryextention As New ZigBeeDiscoveryExtension
    Private z_dongle As IZigBeeTransportTransmit
    Private z_transportconfigoptions As New TransportConfigOption
    Dim dongle_type As String = "TiCc2531"
    Dim resetnetwork As Boolean = False
    Dim z_panId As Integer = 1
    Private z_ExtendedPanId = New ExtendedPanId
    Private z_channel = ZigBeeChannel.CHANNEL_11
    Private z_networkKey = ZigBeeKey.CreateRandom()
    Private z_linkKey = New ZigBeeKey

    Private z_serialport As New ZigBeeSerialPort("COM1")
    Dim _baudspeed As Integer = 115200

    Shared iddevice As String
    Shared zcommand As New CommandResult
    Shared adr1txt As New ArrayList

    Enum ManufacturerCode As Integer
        TUYA = 4098
        EBYTE = 4098
        PHILIPS = 4107
        ATMEL = 0
        XIAOMI = 0

    End Enum

#End Region

#Region "Variables Internes"

#End Region

#Region "Propriétés génériques"
    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
            Set(ByVal value As String)
                _IdSrv = value
            End Set
        End Property

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
        Public ReadOnly Property DeviceSupport() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.DeviceSupport
            Get
                Return _DeviceSupport
            End Get
        End Property

        Public Property Parametres() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.Parametres
            Get
                Return _Parametres
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _Parametres = value
            End Set
        End Property

        Public Property LabelsDriver() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDriver
            Get
                Return _LabelsDriver
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _LabelsDriver = value
            End Set
        End Property
        Public Property LabelsDevice() As System.Collections.ArrayList Implements HoMIDom.HoMIDom.IDriver.LabelsDevice
            Get
                Return _LabelsDevice
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _LabelsDevice = value
            End Set
        End Property

        Public Event DriverEvent(ByVal DriveName As String, ByVal TypeEvent As String, ByVal Parametre As Object) Implements HoMIDom.HoMIDom.IDriver.DriverEvent
        Public Property Enable() As Boolean Implements HoMIDom.HoMIDom.IDriver.Enable
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property
        Public ReadOnly Property ID() As String Implements HoMIDom.HoMIDom.IDriver.ID
            Get
                Return _ID
            End Get
        End Property
        Public Property IP_TCP() As String Implements HoMIDom.HoMIDom.IDriver.IP_TCP
            Get
                Return _IP_TCP
            End Get
            Set(ByVal value As String)
                _IP_TCP = value
            End Set
        End Property
        Public Property IP_UDP() As String Implements HoMIDom.HoMIDom.IDriver.IP_UDP
            Get
                Return _IP_UDP
            End Get
            Set(ByVal value As String)
                _IP_UDP = value
            End Set
        End Property
        Public ReadOnly Property IsConnect() As Boolean Implements HoMIDom.HoMIDom.IDriver.IsConnect
            Get
                Return _IsConnect
            End Get
        End Property
        Public Property Modele() As String Implements HoMIDom.HoMIDom.IDriver.Modele
            Get
                Return _Modele
            End Get
            Set(ByVal value As String)
                _Modele = value
            End Set
        End Property
        Public ReadOnly Property Nom() As String Implements HoMIDom.HoMIDom.IDriver.Nom
            Get
                Return _Nom
            End Get
        End Property
        Public Property Picture() As String Implements HoMIDom.HoMIDom.IDriver.Picture
            Get
                Return _Picture
            End Get
            Set(ByVal value As String)
                _Picture = value
            End Set
        End Property
        Public Property Port_TCP() As String Implements HoMIDom.HoMIDom.IDriver.Port_TCP
            Get
                Return _Port_TCP
            End Get
            Set(ByVal value As String)
                _Port_TCP = value
            End Set
        End Property
        Public Property Port_UDP() As String Implements HoMIDom.HoMIDom.IDriver.Port_UDP
            Get
                Return _Port_UDP
            End Get
            Set(ByVal value As String)
                _Port_UDP = value
            End Set
        End Property
        Public ReadOnly Property Protocol() As String Implements HoMIDom.HoMIDom.IDriver.Protocol
            Get
                Return _Protocol
            End Get
        End Property
        Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
            Get
                Return _Refresh
            End Get
            Set(ByVal value As Integer)
                If value >= 1 Then
                    _Refresh = value

                End If
            End Set
        End Property
        Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
            Get
                Return _Server
            End Get
            Set(ByVal value As HoMIDom.HoMIDom.Server)
                _Server = value
            End Set
        End Property
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
        ''' <summary>
        ''' Retourne la liste des Commandes avancées
        ''' </summary>
        ''' <returns></returns>
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

            ' Dim NodeTemp As New NodePermet de construire le message à afficher dans la console
            '            Dim texteCommande As String

            Try
                If MyDevice IsNot Nothing Then
                    'Pas de commande demandée donc erreur
                    If Command = "" Then
                        Return False
                    Else
                        'permet de gerer la compatibilité du driver avec listbox
                        Dim adr1 As String = ""
                        If InStr(MyDevice.Adresse1, "#") > 0 Then
                            adr1 = Trim(Left(MyDevice.Adresse1, InStr(MyDevice.Adresse1, "#") - 1))
                        Else
                            adr1 = MyDevice.Adresse1
                        End If

                        WriteLog("DBG: ExecuteCommand, Passage de la commande " & UCase(Command) & " Param(0) -> " & Param(0) & " / Param(1) -> " & Param(1))

                        ' Traitement de la commande 
                        Select Case UCase(Command)

                            Case "ALL_LIGHT_ON"
                                '                            m_manager.SwitchAllOn(m_homeId)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_ON")

                            Case "ALL_LIGHT_OFF"
                                '                           m_manager.SwitchAllOff(m_homeId)
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande ALL_LIGHT_OFF")

                            Case "TOGGLE"
                                If MyDevice.Value Then
                                    Write(MyDevice, "OFF")
                                Else
                                    Write(MyDevice, "ON")
                                End If
                                WriteLog("DBG: " & "ExecuteCommand, Passage par la commande Toggle " And MyDevice.value.ToString)

                            Case "SETPOINT"
                                Write(MyDevice, "SETPOINT", Param(0))
                                WriteLog("DBG: " & Me.Nom & " ExecuteCommand, commande " & UCase(Command) & "passée")

                            Case Else
                                WriteLog("ERR: " & "ExecuteCommand, La commande " & UCase(Command) & " n'existe pas")
                        End Select
                        Return True
                    End If
                Else
                    Return False
                End If
            Catch ex As Exception
                WriteLog("ERR: " & "ExecuteCommand, exception : " & ex.Message)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Permet de vérifier si un champ est valide
        ''' </summary>
        ''' <param name="Champ"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function VerifChamp(ByVal Champ As String, ByVal Value As Object) As String Implements HoMIDom.HoMIDom.IDriver.VerifChamp
            Try
                Dim retour As String = "0"

                Select Case UCase(Champ)
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

            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Start, Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            'ouverture du port suivant le Port Com
            Try
            dongle_type = _Parametres.Item(1).Valeur
            iddevice = Me._ID
            Dim node As New ZigBeeNode

            If _Com <> "" Then
                WriteLog("Demarrage du pilote, ceci peut prendre plusieurs secondes")
                _IsConnect = Ouvrir(_Com)
                If Not _IsConnect Then Exit Sub
            End If

            Update_Adrs1()

            WriteLog("Réseau, PAN ID = " & z_manager.ZigBeePanId & " / Channel = " & z_manager.ZigbeeChannel)
                ' WriteLog("Extended PAN ID = " & z_manager.ZigBeeExtendedPanId.ToString)
                Select Case True
                Case UCase(dongle_type) = "TICC2531"
                    WriteLog("Version soft dongle " & z_manager.TransportVersionString)
                Case UCase(dongle_type) = "CONBEE"
                    ' WriteLog("Network Key = " & z_manager.ZigBeeNetworkKey.Key.ToString & " / Link Key = " & z_manager.ZigBeeLinkKey.Key.ToString)
            End Select

            '   MyTimer.Interval = 600 * 1000
            '  MyTimer.Enabled = True
            '  AddHandler MyTimer.Elapsed, AddressOf TimerTick

            WriteLog("Driver " & Me.Nom & " démarré ")

        Catch ex As Exception
                WriteLog("ERR: Start, Erreur démarrage " & ex.Message)
                _IsConnect = False
            End Try
        End Sub

        ''' <summary>Arrêter le driver</summary>
        ''' <remarks></remarks>
        Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop

            Try
                If _IsConnect Then
                    z_manager.Shutdown()

                    _IsConnect = False
                    WriteLog("Driver " & Me.Nom & ", port fermé")
                    WriteLog("Driver " & Me.Nom & " arrêté")
                Else
                    WriteLog("Driver " & Me.Nom & "Port " & _Com & " est déjà fermé")
                End If

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
        ''' <remarks>pas utilisé</remarks>
        Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
            Try
                Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                    _DEBUG = _Parametres.Item(0).Valeur
                Catch ex As Exception
                    _DEBUG = False
                    _Parametres.Item(0).Valeur = False
                    WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
                End Try

                If _Enable = False Then Exit Sub

                If _IsConnect = False Then
                    WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                    Exit Sub
                End If
            Catch ex As Exception
                WriteLog("ERR: Read, Exception : " & ex.Message)
            End Try
        End Sub

        ''' <summary>Commander un device</summary>
        ''' <param name="Objet">Objet représetant le device à interroger</param>
        ''' <param name="Commande">La commande à passer</param>
        ''' <param name="Parametre1"></param>
        ''' <param name="Parametre2"></param>
        ''' <remarks></remarks>
        Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write


            Dim texteCommande As String
            Dim command1 As New Object
            Dim command2 As New Object

            _DEBUG = _Parametres.Item(0).Valeur

            If _Enable = False Then
                WriteLog("ERR: " & "Write, Erreur: Impossible de traiter la commande car le driver n'est pas activé (Enable)")
                Exit Sub
            End If

            If _IsConnect = False Then
                WriteLog("ERR: " & "Write, Erreur: Impossible de traiter la commande car le driver n'est pas connecté")
                Exit Sub
            End If
            Try

            'permet de gerer la compatibilité du driver avec listbox
            Dim adr1 As String = ""
            If InStr(Objet.Adresse1, "#") > 0 Then
                adr1 = Trim(Left(Objet.Adresse1, InStr(Objet.Adresse1, "#") - 1))
            Else
                adr1 = Objet.Adresse1
            End If

            Dim node = z_manager.GetNode(adr1)
            Dim endpoint = node.GetEndpoints().FirstOrDefault()
            Dim EndpointAddress As ZigBeeEndpointAddress = endpoint.GetEndpointAddress

            Select Case True
                Case (Objet.Type = "LAMPE" Or Objet.Type = "APPAREIL" Or Objet.Type = "SWITCH" Or Objet.Type = "VOLET")
                    texteCommande = UCase(Commande)
                    Select Case True
                        Case UCase(Commande) = "ON"
                            z_manager.Send(EndpointAddress, New OnCommand)
                            Objet.value = True
                        Case UCase(Commande) = "OFF"
                            z_manager.Send(EndpointAddress, New OffCommand)
                            Objet.value = False
                        Case UCase(Commande) = "DIM" Or UCase(Commande) = "OUVERTURE"
                            If Not (IsNothing(Parametre1)) Then
                                command1 = New MoveToLevelWithOnOffCommand()
                                command1.Level = Byte.Parse(Parametre1)
                                command1.TransitionTime = UShort.Parse(100)
                                z_manager.Send(EndpointAddress, command1)
                                Objet.value = Parametre1
                            End If
                        Case UCase(Commande) = "SETPOINT"   'Ecrire une valeur vers le device physique
                            If Not IsNothing(Parametre1) Then
                                Dim ValDimmer As Single
                                If IsNumeric(Parametre1) Then ValDimmer = Parametre1
                                Select Case True  ' gestion des modes de chauffage
                                    Case (UCase(Parametre1) = "CONFORT" Or UCase(Parametre1) = "CONF")
                                        ValDimmer = 95
                                    Case (UCase(Parametre1) = "CONFORT-1" Or UCase(Parametre1) = "CONF-1")
                                        ValDimmer = 45
                                    Case (UCase(Parametre1) = "CONFORT-2" Or UCase(Parametre1) = "CONF-2")
                                        ValDimmer = 35
                                    Case (UCase(Parametre1) = "ECO" Or UCase(Parametre1) = "EC")
                                        ValDimmer = 25
                                    Case (UCase(Parametre1) = "HORSGEL" Or UCase(Parametre1) = "HG")
                                        ValDimmer = 15
                                    Case UCase(Parametre1) = "ARRET"
                                        ValDimmer = 5
                                End Select
                                Select Case True
                                    Case InStr(Objet.adresse2, "Color:") > 0
                                        command1 = New MoveToColorCommand()
                                        z_manager.Send(EndpointAddress, command1)
                                        Objet.value = command1.level
                                    Case InStr(Objet.Adresse2, "Level:") > 0  'puissance de la lampe, mode chauffage
                                        command1 = New MoveToLevelWithOnOffCommand()
                                        command1.Level = Byte.Parse(Parametre1)
                                        command1.TransitionTime = UShort.Parse(100)
                                        z_manager.Send(EndpointAddress, command1)
                                        Objet.value = command1.level
                                    Case Else
                                        command1 = New MoveToLevelWithOnOffCommand()
                                        command1.Level = Byte.Parse(ValDimmer)
                                        command1.TransitionTime = UShort.Parse(100)
                                        z_manager.Send(EndpointAddress, command1)
                                End Select
                            End If

                        Case "ALL_LIGHT_ON"
                            Objet.value = True

                        Case "ALL_LIGHT_OFF"
                            Objet.value = False
                    End Select
                Case (Objet.Type = "TEMPERATURECONSIGNE" Or Objet.Type = "GENERIQUEVALUE")
                    texteCommande = UCase(Commande)
                Case UCase(Commande) = "SETPOINT"   'Ecrire une valeur vers le device physique
                    If Not IsNothing(Parametre1) Then
                        Dim ValDimmer As Single
                        If IsNumeric(Parametre1) Then ValDimmer = Parametre1
                        Select Case True  ' gestion des modes de chauffage
                            Case (UCase(Parametre1) = "CONFORT" Or UCase(Parametre1) = "CONF")
                                ValDimmer = 95
                            Case (UCase(Parametre1) = "CONFORT-1" Or UCase(Parametre1) = "CONF-1")
                                ValDimmer = 45
                            Case (UCase(Parametre1) = "CONFORT-2" Or UCase(Parametre1) = "CONF-2")
                                ValDimmer = 35
                            Case (UCase(Parametre1) = "ECO" Or UCase(Parametre1) = "EC")
                                ValDimmer = 25
                            Case (UCase(Parametre1) = "HORSGEL" Or UCase(Parametre1) = "HG")
                                ValDimmer = 15
                            Case UCase(Parametre1) = "ARRET"
                                ValDimmer = 5
                        End Select
                        command1 = New MoveToLevelWithOnOffCommand()
                        command1.Level = Byte.Parse(Parametre1)
                        command1.TransitionTime = UShort.Parse(100)
                        z_manager.Send(EndpointAddress, command1)
                        Objet.value = command1.level
                    End If
            End Select
            WriteLog("DBG: " & "ExecuteCommand, Passage par la commande " & UCase(Commande))
            Catch ex As Exception
                WriteLog("ERR: Write, Exception : " & ex.Message)
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
                WriteLog("ERR: Add_DeviceCommande, Exception :" & ex.Message)
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

                'liste des devices compatibles
                _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
                _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
                _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
                _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
                _DeviceSupport.Add(ListeDevices.DIRECTIONVENT.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
                _DeviceSupport.Add(ListeDevices.ENERGIETOTALE.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING.ToString)
                _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
                _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
                _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
                _DeviceSupport.Add(ListeDevices.LAMPERGBW.ToString)
                _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
                _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
                _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
                _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)
                _DeviceSupport.Add(ListeDevices.VOLET.ToString)

                'Paramétres avancés
                Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("Dongle type", "Type de clef (TiCc2531, Conbee, DigiXBee, Ember)", "TiCc2531")
            '        Add_ParamAvance("AfficheLog", "Afficher Log OpenZwave à l'écran (True/False)", True)

            'ajout des commandes avancées pour les devices
            ' Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
            'Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)
            Add_DeviceCommande("SETPOINT", "", 0)

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par un contrôleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Protocole", "Protocole utilisé")
            Add_LibelleDevice("ADRESSE2", "Intitulé mesure", "")
            Add_LibelleDevice("SOLO", "@", "")
                Add_LibelleDevice("MODELE", "@", "")

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
            End Try
        End Sub

    ''' <summary>Si refresh >0 gestion du timer</summary>
    ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
    Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)
        Try
            If z_manager.Nodes.Count > 0 Then
                Update_Adrs1()
            End If
        Catch ex As Exception
            WriteLog("ERR: TimerTick" & ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    '-----------------------------------------------------------------------------
    ''' <summary>Ouvrir le port Zigbee</summary>
    ''' <param name="port">Nom/Numero du port COM: COM2</param>
    ''' <remarks></remarks>
    Private Function Ouvrir(ByVal port As String) As Boolean
            Try
                Try
                    If Not _IsConnect Then
                        Select Case True
                        Case UCase(dongle_type) = "EMBER"
                            z_flowcontrol = FlowControl.FLOWCONTROL_OUT_XONOFF
                            Case Else
                                z_flowcontrol = FlowControl.FLOWCONTROL_OUT_NONE
                        End Select
                    '   z_flowcontrol = FlowControl.FLOWCONTROL_OUT_NONE

                    WriteLog("DBG: z_serialport = New ZigBeeSerialPort(" & port & ", " & _baudspeed & ", " & z_flowcontrol.ToString & "), type " & dongle_type)
                    z_serialport = New ZigBeeSerialPort(port, _baudspeed, z_flowcontrol)

                        Select Case True
                        Case UCase(dongle_type) = "TICC2531"
                            '   Dim tmp_dgl As New ZigBeeDongleTiCc2531(z_serialport)
                            'tmp_dgl.SetLedMode(2, False) 'led rouge.  1=verte
                            ' z_dongle = tmp_dgl
                            z_dongle = New ZigBeeDongleTiCc2531(z_serialport)
                        Case UCase(dongle_type) = "DIGIXBEE"
                            z_dongle = New XBee.ZigBeeDongleXBee(z_serialport)
                        Case UCase(dongle_type) = "CONBEE"
                            z_dongle = New ZigbeeDongleConBee(z_serialport)
                        Case UCase(dongle_type) = "EMBER"
                            z_dongle = New ZigBeeDongleEzsp(z_serialport)
                            Case Else
                                z_dongle = New ZigBeeDongleTiCc2531(z_serialport)
                        End Select

                    z_manager = New ZigBeeNetworkManager(z_dongle)

                    z_manager.AddSupportedClientCluster(ZclAlarmsCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclAnalogInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclBinaryInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclColorControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclCommissioningCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclDehumidificationControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclDemandResponseAndLoadControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclDiagnosticsCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclDoorLockCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclElectricalMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclFanControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclFlowMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclGreenPowerCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclGroupsCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIasAceCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIasWdCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIasZoneCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIdentifyCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIlluminanceLevelSensingCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclIlluminanceMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclKeyEstablishmentCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclLevelControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMessagingCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMeterIdentificationCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMeteringCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMultistateInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMultistateOutputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclMultistateValueBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclOccupancySensingCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclOnOffCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclOnOffSwitchConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclOtaUpgradeCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclPollControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclPowerConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclPrepaymentCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclPressureMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclPriceCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclRssiLocationCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclScenesCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclTemperatureMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclThermostatCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclThermostatUserInterfaceConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclTimeCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclWindowCoveringCluster.CLUSTER_ID)

                    z_manager.AddSupportedServerCluster(ZclAlarmsCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclAnalogInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclBinaryInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclColorControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclCommissioningCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclDehumidificationControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclDemandResponseAndLoadControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclDiagnosticsCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclDoorLockCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclElectricalMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclFanControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclFlowMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclGreenPowerCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclGroupsCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIasAceCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIasWdCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIasZoneCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIdentifyCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIlluminanceLevelSensingCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclIlluminanceMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclKeyEstablishmentCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclLevelControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMessagingCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMeterIdentificationCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMeteringCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMultistateInputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMultistateOutputBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclMultistateValueBasicCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclOccupancySensingCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclOnOffCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclOnOffSwitchConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclOtaUpgradeCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclPollControlCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclPowerConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclPrepaymentCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclPressureMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclPriceCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclRssiLocationCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclScenesCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclTemperatureMeasurementCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclThermostatCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclThermostatUserInterfaceConfigurationCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclTimeCluster.CLUSTER_ID)
                    z_manager.AddSupportedServerCluster(ZclWindowCoveringCluster.CLUSTER_ID)


                    z_jsondatastore = New JsonNetworkDataStore(My.Application.Info.DirectoryPath & "\drivers\Zigbee")
                    z_manager.SetNetworkDataStore(z_jsondatastore)

                    z_discoveryextention = New ZigBeeDiscoveryExtension
                    z_discoveryextention.SetUpdatePeriod(60)
                    z_manager.AddExtension(z_discoveryextention)

                    Dim zbstatus As ZigBeeStatus = z_manager.Initialize()
                    WriteLog("DBG: z_manager.Initialize " & zbstatus.ToString)
                    If InStr(zbstatus.ToString, UCase("error")) > 0 Then Return False

                    z_manager.AddCommandListener(New ZigBeeTransaction(z_manager))
                    z_manager.AddCommandListener(New ConsoleCommandListener)
                    z_manager.AddCommandListener(New ZigBeeNetworkDiscoverer(z_manager))
                    z_manager.AddAnnounceListener(New ConsoleAnnounceListener)
                    z_manager.AddNetworkNodeListener(New ConsoleNetworkNodeListener)
                    z_manager.AddNetworkStateListener(New ConsoleNetworkStateListener)

                    z_manager.AddExtension(New ZigBeeBasicServerExtension)
                    z_manager.AddExtension(New ZigBeeIasCieExtension())
                    z_manager.AddExtension(New ZigBeeDiscoveryExtension)

                    If resetnetwork Then
                            'TODO: make the network parameters configurable
                            Dim panId As Integer = Math.Floor(VBMath.Rnd * 65534)
                            z_ExtendedPanId = New ExtendedPanId("00124B001CCE1B5F")
                            z_channel = ZigBeeChannel.CHANNEL_11
                            z_networkKey = ZigBeeKey.CreateRandom()
                            Dim keybyte As Byte() = {&H5A, &H69, &H67, &H42, &H65, &H65, &H41, &H6C, &H6C, &H69, &H61, &H6E, &H63, &H65, &H30, &H39}
                            z_linkKey = New ZigBeeKey(keybyte)

                            WriteLog("*** Resetting network")
                            WriteLog("  * PAN ID = " & panId)
                            WriteLog("  * Extended PAN ID  = " & z_ExtendedPanId.ToString)
                            WriteLog("  * Channel          = " & z_channel)
                            WriteLog("  * Network Key      = " & z_networkKey.ToString)
                            WriteLog("  * Link Key         = " & z_linkKey.ToString)

                            z_manager.SetZigBeeChannel(z_channel)
                            z_manager.SetZigBeePanId(panId)
                            z_manager.SetZigBeeExtendedPanId(z_ExtendedPanId)
                            z_manager.SetZigBeeNetworkKey(z_networkKey)
                            z_manager.SetZigBeeLinkKey(z_linkKey)

                            zbstatus = z_manager.Startup(resetnetwork)
                        End If
                    Return (z_manager.Startup(False) = ZigBeeStatus.SUCCESS)
                End If
                Catch ex As Exception
                    WriteLog("ERR: Ouvrir, " & ex.Message)
                    Return False
                End Try
            Catch ex As Exception
                WriteLog("ERR: Ouvrir, " & ex.Message)
                Return False
            End Try
            Return False
        End Function

    ''' <summary>
    ''' Place le controller en mode "inclusion"
    ''' </summary>
    ''' <remarks></remarks>
    Sub StartInclusionMode()
        Try
            If _IsConnect Then
                z_ManagerNode0.PermitJoin(CByte(120))
                WriteLog("Début de la séquence d'inclusion")
            End If
        Catch ex As Exception
            WriteLog("ERR: StartInclusionMode Exception: " + ex.Message)
        End Try
    End Sub

    Sub StopInclusionMode()
            Try
                If _IsConnect Then
                z_ManagerNode0.PermitJoin(CByte(0))
                WriteLog("Fin de la séquence d'inclusion")
                End If
            Catch ex As Exception
                WriteLog("ERR: StopInclusionMode Exception: " + ex.Message)
            End Try
        End Sub

    Sub NommerDevice(NumDevice As String, Nom As String)

        Try
            If _IsConnect Then

            End If
        Catch ex As Exception
            WriteLog("ERR: NommerDevice Exception: " + ex.Message)
        End Try

    End Sub

    Shared Sub Update_Adrs1()
        Try
            z_ManagerNode0 = z_manager.GetNode(0)

            Dim _libelleadr1 As String = "" '" |" '"51223 # ST-TH0|"
            Dim _libelleadr2 As String = ""
            adr1txt.Clear()

            Dim tmp = ""
            For i = 0 To z_manager.Nodes.Count - 1
                For Each endpoint In z_manager.Nodes(i).GetEndpoints
                    If tmp = endpoint.Node.NetworkAddress.ToString Then Continue For
                    ' _libelleadr1 += endpoint.Node.NetworkAddress.ToString & " # " & " ST-TH01|"
                    ' WriteLog(_libelleadr1.ToString)
                    '  adr1txt.Add(endpoint.Node.NetworkAddress.ToString & " # " & " ST-TH01") ' stocke l'adresse des reseau
                    For Each InputClusterID In endpoint.GetInputClusterIds
                        Dim cluster = endpoint.GetInputCluster(InputClusterID)
                        Dim model As String

                        For Each attr In cluster.GetAttributes
                            If InStr(_libelleadr1, endpoint.Node.NetworkAddress.ToString) = 0 Then
                                model = cluster.ReadAttributeValue(ZclBasicCluster.ATTR_MODELIDENTIFIER).Result
                                _libelleadr1 += endpoint.Node.NetworkAddress.ToString & " # " & model & "|"
                                adr1txt.Add(endpoint.Node.NetworkAddress.ToString & " # " & model)
                                'cluster.ReadAttributeValue(ZclBasicCluster.ATTR_MODELIDENTIFIER).Wait()
                                ' WriteLog(_libelleadr1)
                            End If
                            '   If attr.Reportable = True Then
                            If attr.Readable Then
                                Select Case True
                                    Case InStr("IAS Zone", cluster.GetClusterName) > 0
                                        _libelleadr2 += endpoint.Node.NetworkAddress.ToString & " #; " & cluster.GetClusterName & " : " & attr.Name & "|"
                                        Continue For
                                    Case attr.Reportable = False
                                        Continue For
                                    Case Else
                                        _libelleadr2 += endpoint.Node.NetworkAddress.ToString & " #; " & cluster.GetClusterName & " : " & attr.Name & "|"
                                End Select
                            End If
                        Next
                    Next
                    tmp = endpoint.Node.NetworkAddress.ToString
                Next
            Next



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
                'WriteLog("DBG: _libelleadr " & ld0.Parametre)
            Next
        Catch ex As Exception
            WriteLog("ERR: Update_Adrs1: " + ex.Message)
        End Try
    End Sub


    Shared Sub WriteLog(message)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Zigbee", STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zigbee", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Zigbee", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Zigbee" & " WriteLog", ex.Message)
        End Try
    End Sub

    Private Shared WriteOnly Property ResultConsoleCommand As String

        Set(ByVal value As String)
            Try
                WriteLog("DBG: ResultConsoleCommand, value =>>> " & value)
                Select Case True
                    Case InStr(value, "deviceadded", vbTextCompare) > 0
                        'Update_Adrs1(value)
                    Case InStr(value, "attributeresponse", vbTextCompare) > 0
                        Update_Adrs1()

                    Case InStr(value, "reports", vbTextCompare) > 0
                        'value = "Power Configuration: 51224/1 -> 0/1, cluster=1, transId=64, Reports=Attribute Report: attributeDataType=Unsigned 8-bit integer, attributeIdentifier=32, attributeValue=29, Attribute Report: attributeDataType=Unsigned 8-bit integer, attributeIdentifier=33, attributeValue=192"

                        Dim m_devices As New ArrayList()
                        Dim Vrecu_IOName As String = ""
                        Dim Vrecu_reseau As String = ""
                        Dim vrecu_clusterid As String = ""
                        Dim vrecu_attribId As String = ""
                        Dim vrecu_datatype As String = ""
                        Dim vrecu_data As String = ""
                        Dim ArrayData(49) As Array
                        Dim adr1 As String = ""

                        Dim valuetmp = Mid(value, 1, InStr(value, ", Reports="))
                        VRecu_IOName = Mid(value, 1, InStr(value, ":") - 1)
                        VRecu_reseau = Mid(value, InStr(value, ":") + 2, InStr(value, "/") - InStr(value, ":") - 2)
                        vrecu_clusterid = Mid(value, InStr(value, "cluster=") + 8, (InStr(value, ", transId")) - InStr(value, "cluster=") - 8)

                        valuetmp = Mid(value, InStr(value, "Reports=") + 8, Len(value))
                        'Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "valuetmp: ", valuetmp & vbCrLf)

                        Dim vtmp As String = ""
                        Dim nbrevalue = 0
                        While InStr(valuetmp, "Attribute Report:") > 0
                            If InStr(valuetmp, ", Attribute Report:") > 0 Then
                                vtmp = Mid(valuetmp, InStr(valuetmp, "Attribute Report:"), InStr(valuetmp, ", Attribute Report:") - (InStr(valuetmp, "Attribute Report:")))
                            Else
                                vtmp = Mid(valuetmp, InStr(valuetmp, "Attribute Report:"), Len(valuetmp))
                            End If
                            'Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "vtmp: ", vtmp)

                            Dim arrayvtmp = Split(vtmp, ",")
                            vrecu_attribId = Mid(arrayvtmp(1).ToString, InStr(arrayvtmp(1).ToString, "=") + 1, Len(arrayvtmp(1).ToString))
                            vrecu_data = Mid(arrayvtmp(2), InStr(arrayvtmp(2), "=") + 1, Len(arrayvtmp(2)))
                            'Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "numvalue: " & nbrevalue & " - vrecu_attribId: ", vrecu_attribId & " - vrecu_data: " & vrecu_data)

                            ArrayData.SetValue({Vrecu_IOName, Vrecu_reseau, vrecu_clusterid, vrecu_attribId, vrecu_data}, nbrevalue)

                            If InStr(valuetmp, ", Attribute Report:") > 0 Then
                                valuetmp = valuetmp.Replace(vtmp & ", ", "")
                            Else
                                valuetmp = valuetmp.Replace(vtmp, "")
                            End If
                            nbrevalue += 1
                            arrayvtmp = Nothing
                        End While

                        For k = 0 To adr1txt.Count - 1
                            If InStr(adr1txt(k), VRecu_reseau & " # ", vbTextCompare) > 0 Then
                                adr1 = adr1txt(k)
                                Exit For
                            End If
                        Next
                        If adr1 = "" Then adr1 = Mid(VRecu_reseau, 1, InStr(VRecu_reseau, "# "))

                        Dim ioName As String = ""
                        For Each adata In ArrayData
                            If IsNothing(adata) Then Continue For
                            ' Vrecu_IOName = adata(0).ToString
                            ' Vrecu_reseau = adata(1).ToString
                            ' vrecu_clusterid = adata(2).ToString
                            vrecu_attribId = adata(3).ToString
                            vrecu_data = adata(4).ToString

                            ' Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "adata: ", adata(0).ToString & " - " & adata(1).ToString & " - " & adata(2).ToString & " - " & adata(3).ToString & " - " & adata(4).ToString)
                            Dim cluster As ZclCluster
                            ' recherche du cluster id de la mesure pour reproduire contenu de adr2
                            For i = 0 To z_manager.Nodes.Count - 1
                                If z_manager.Nodes.Item(i).NetworkAddress = Vrecu_reseau Then
                                    For Each endpoint In z_manager.Nodes(i).GetEndpoints
                                        For Each InputClusterID In endpoint.GetInputClusterIds
                                            cluster = endpoint.GetInputCluster(InputClusterID)
                                            ' Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "VRecu_clusterid: ", ArrayData(2, j))
                                            If cluster.GetClusterId = vrecu_clusterid.ToString Then
                                                For Each attr In cluster.GetAttributes
                                                    If attr.Id = vrecu_attribId Then
                                                        ' Dim attrib = cluster.GetClusterNameGetAttribute(vrecu_clusterid)
                                                        ioName = Trim(Vrecu_IOName.ToString & " : " & attr.Name)
                                                        vrecu_datatype = attr.DataType.Label
                                                        Exit For
                                                    End If
                                                Next
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                            Next

                            m_devices = Server.Instance.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adr1, "", iddevice, True)

                            If m_devices.Count > 0 Then
                                For Each LocalDevice As Object In m_devices
                                    ' Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "List device ", VRecu_reseau & " -> IOName " & ioName & " / LocalDevice.adresse2 = " & LocalDevice.adresse2 & " / vrecu_data = " & vrecu_data)
                                    If InStr(ioName, Trim(LocalDevice.adresse2), vbTextCompare) > 0 Then
                                        Select Case True
                                            Case (IsNumeric(vrecu_data)) And (InStr(vrecu_datatype, "integer", vbTextCompare) > 0)
                                                Select Case True
                                                    Case (InStr(vrecu_datatype, "8-bit", vbTextCompare) > 0)
                                                        If (InStr(vrecu_datatype, "unsigned", vbTextCompare) > 0) Then
                                                            LocalDevice.value = CByte(vrecu_data)
                                                        Else
                                                            LocalDevice.value = CSByte(vrecu_data)
                                                        End If
                                                    Case (InStr(vrecu_datatype, "16-bit", vbTextCompare) > 0)
                                                        vrecu_data = Regex.Replace(CStr(vrecu_data), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                        If (InStr(vrecu_datatype, "unsigned", vbTextCompare) > 0) Then
                                                            LocalDevice.value = CUShort(vrecu_data)
                                                        Else
                                                            LocalDevice.value = CShort(vrecu_data)
                                                        End If
                                                    Case (InStr(vrecu_datatype, "32-bit", vbTextCompare) > 0)
                                                        vrecu_data = Regex.Replace(CStr(vrecu_data), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                        If (InStr(vrecu_datatype, "unsigned", vbTextCompare) > 0) Then
                                                            LocalDevice.value = CUInt(vrecu_data)
                                                        Else
                                                            LocalDevice.value = CInt(vrecu_data)
                                                        End If
                                                    Case (InStr(vrecu_datatype, "64-bit", vbTextCompare) > 0)
                                                        vrecu_data = Regex.Replace(CStr(vrecu_data), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                                                        If (InStr(vrecu_datatype, "unsigned", vbTextCompare) > 0) Then
                                                            LocalDevice.value = CULng(vrecu_data)
                                                        Else
                                                            LocalDevice.value = CLng(vrecu_data)
                                                        End If
                                                End Select
                                            Case (InStr(vrecu_datatype, "enumeration", vbTextCompare) > 0) 'ne tient pas compte de la casse
                                                LocalDevice.value = CByte(vrecu_data)
                                            Case (InStr(vrecu_datatype, "boolean", vbTextCompare) > 0)
                                                LocalDevice.value = CBool(vrecu_data)
                                            Case Else
                                                LocalDevice.value = vrecu_data
                                        End Select
                                    End If
                                Next
                            End If
                            m_devices = Nothing
                        Next
                    Case InStr(value, "ZoneStatusChangeNotificationCommand", vbTextCompare) > 0
                        ' ZoneStatusChangeNotificationCommand: IAS Zone: 16172/1 -> 0/1, cluster=1280, transId=48, ZoneStatus=0, ExtendedStatus=0, ZoneId=0, Delay=0

                        value = value.Replace("ZoneStatusChangeNotificationCommand: ", "")
                        Dim m_devices As New ArrayList()
                        Dim adr1 As String = ""
                        Dim Vrecu_IOName As String = ""
                        Dim Vrecu_reseau As String = ""
                        Dim vrecu_clusterid As String = ""

                        Vrecu_IOName = Mid(value, 1, InStr(value, ":") - 1)
                        Vrecu_reseau = Mid(value, InStr(value, ":") + 2, InStr(value, "/") - InStr(value, ":") - 2)
                        vrecu_clusterid = Mid(value, InStr(value, "cluster=") + 8, (InStr(value, ", transId")) - InStr(value, "cluster=") - 8)
                        Dim valuetmp = Mid(value, InStr(value, "ZoneStatus=") + 11, InStr(value, ", ExtendedStatus") - InStr(value, "ZoneStatus=") - 11)

                        For k = 0 To adr1txt.Count - 1
                            If InStr(adr1txt(k), Vrecu_reseau & " # ", vbTextCompare) > 0 Then
                                adr1 = adr1txt(k)
                                Exit For
                            End If
                        Next
                        If adr1 = "" Then adr1 = Vrecu_reseau & " # "

                        m_devices = Server.Instance.ReturnDeviceByAdresse1TypeDriver(_IdSrv, adr1, "", iddevice, True)

                        If m_devices.Count > 0 Then
                            For Each LocalDevice As Object In m_devices
                                If InStr(Trim(LocalDevice.adresse2), "Zone Status", vbTextCompare) > 0 Then
                                    LocalDevice.value = CBool(valuetmp)
                                End If
                            Next
                        End If
                End Select
            Catch ex As Exception
                WriteLog("ERR: Property ResultConsoleCommand, " & ex.Message)
            End Try
        End Set
    End Property

    <Serializable()> Public Class ConsoleNetworkNodeListener
            Implements IZigBeeNetworkNodeListener

        Sub NodeAdded(node As ZigBeeNode) Implements IZigBeeNetworkNodeListener.NodeAdded
            Try
                If (node.LogicalType <> NodeDescriptor.LogicalType.COORDINATOR) And (node.NetworkAddress <> 0) Then
                    Dim str As String = "DeviceAdded: " & node.NetworkAddress & ":" & node.ToString
                    ResultConsoleCommand = str
                End If
            Catch ex As Exception
                WriteLog("ERR: NodeAddedd " & ex.Message)
            End Try
        End Sub

        Sub NodeRemoved(node As ZigBeeNode) Implements IZigBeeNetworkNodeListener.NodeRemoved
            Try
                WriteLog("NodeRemoved, Noeud " & node.IeeeAddress.Value & " retiré")
            Catch ex As Exception
                WriteLog("ERR: NodeAddedd " & ex.Message)
            End Try
        End Sub
        Sub NodeUpdated(node As ZigBeeNode) Implements IZigBeeNetworkNodeListener.NodeUpdated
            Try
                If (node.LogicalType <> NodeDescriptor.LogicalType.COORDINATOR) And (node.NetworkAddress <> 0) Then
                    Dim str As String = "DeviceUpdated: " & node.NetworkAddress & ":" & node.ToString
                    ResultConsoleCommand = str
                End If
            Catch ex As Exception
                WriteLog("ERR: NodeUpdated " & ex.Message)
            End Try
        End Sub
    End Class

        <Serializable()> Public Class ConsoleNetworkStateListener
            Implements IZigBeeNetworkStateListener

            Sub NetworkStateUpdated(state As ZigBeeNetworkState) Implements IZigBeeNetworkStateListener.NetworkStateUpdated
            WriteLog("NetworkStateUpdated " & state.ToString)
        End Sub

        End Class

        <Serializable()> Public Class ConsoleNetworkEndPointListener
            Implements IZigBeeNetworkEndpointListener

        Sub DeviceAdded(endpoint As ZigBeeEndpoint) Implements IZigBeeNetworkEndpointListener.DeviceAdded
            Try
                Dim endpt = endpoint
                Dim str As String = "EndPointAdded: " & endpoint.DeviceId
                ResultConsoleCommand = str
                ' Server.Instance.Log(TypeLog.DEBUG, TypeSource.SERVEUR, "EndpointAdded ", endpoint.ToString)
            Catch ex As Exception
                WriteLog("ERR: EndPointAddedd " & ex.Message)
            End Try
        End Sub
        Sub DeviceRemoved(endpoint As ZigBeeEndpoint) Implements IZigBeeNetworkEndpointListener.DeviceRemoved
            WriteLog("EndpointRemoved " & endpoint.ToString)
        End Sub
            Sub DeviceUpdated(endpoint As ZigBeeEndpoint) Implements IZigBeeNetworkEndpointListener.DeviceUpdated
            WriteLog("EndpointUpdated " & endpoint.ToString)
        End Sub

        End Class

        <Serializable()> Public Class ConsoleCommandListener
            Implements IZigBeeCommandListener


            Sub CommandReceived(command As ZigBeeCommand) Implements IZigBeeCommandListener.CommandReceived
                Try
                Dim str As String = command.ToString

                Select Case True
                    Case (InStr(command.ToString, "ReportAttributesCommand") > 0)
                        'nettoyage de la chaine pour etre clair
                        str = str.Replace("ReportAttributesCommand [", "")
                        str = str.Replace("]", "")
                        ResultConsoleCommand = str
                    Case (InStr(command.ToString, "ReadAttributesResponse") > 0)
                        str = str.Replace("ReportAttributesResponse [", "")
                        str = str.Replace("]", "")
                        ResultConsoleCommand = "AttributesResponse: " & str
                    Case (InStr(command.ToString, "ZoneStatusChangeNotificationCommand") > 0)
                        str = str.Replace("ZoneStatusChangeNotificationCommand [", "")
                        str = str.Replace("]", "")
                        ResultConsoleCommand = "ZoneStatusChangeNotificationCommand: " & str

                    Case Else
                        WriteLog("DBG: CommandReceived RAWSTR => " & command.ToString)
                End Select
                str = Nothing
                Catch ex As Exception
                WriteLog("ERR: CommandReceived " & ex.Message)
            End Try
            End Sub
        End Class
    <Serializable()> Public Class ConsoleAnnounceListener
        Implements IZigBeeAnnounceListener

        Sub AnnounceUnknownDevice(netwrk As UShort) Implements IZigBeeAnnounceListener.AnnounceUnknownDevice
            WriteLog("AnnounceUnknownDevice, réseau " & netwrk.ToString)
        End Sub
        Sub DeviceStatusUpdate(devicestatus As ZigBeeNodeStatus, netwrk As UShort, ieeeadress As IeeeAddress) Implements IZigBeeAnnounceListener.DeviceStatusUpdate
            WriteLog("DeviceStatusUpdate, réseau " & devicestatus.ToString & " / " & netwrk.ToString)
        End Sub

    End Class
#End Region






    'Dim FrameStr As String = ""
    'Dim FrameCompleted As Boolean = False
    'Dim jsonObjDatas As Object

    'Public InfoSystemStatus As New Systemstatus
    'Public ListOfDevices As List(Of Device) = New List(Of Device)

    'Public Class Systemstatus
    '    Public version As String
    '    Public mac As String
    '    Public jamming As String
    '    Public transmitter As New ListAvailable
    '    Public receiver As New ListAvailable
    '    Public receiverenabled As New ListEnabled
    '    Public repeater As New ListAvailable
    '    Public repeaterenabled As New ListEnabled
    'End Class
    'Public Class ListAvailable
    '    Public available As New ListP
    'End Class
    'Public Class ListEnabled
    '    Public enabled As New ListP
    'End Class
    'Public Class Infodetail
    '    Public n As String
    '    Public v As String
    'End Class
    'Public Class ListP
    '    Public p As New List(Of String)
    'End Class
    'Public Class Device
    '    Public id As String
    '    Public name As String
    '    Public protocol As String
    '    Public subTypeMeaning As String
    '    Public qualifier As String
    'End Class


    'Enum ListProtocol As Integer
    '    X10 = 1
    '    VISONIC433 = 2
    '    VISONIC868 = 2
    '    BLYSS = 3
    '    CHACON = 4
    '    DOMIA = 6
    '    X2D433 = 8
    '    X2D868 = 8
    '    XDSHUTTER = 8
    '    RTS = 9
    '    KD101 = 10
    '    PARROT = 11
    'End Enum




    'Private Sub ReadConf(str As String)
    '    Try
    '        WriteLog("DBG: ReadConf à traiter : " & str)
    '        '      Dim jsonSystemStatus As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(str)

    '        ''     For Each stjson As Object In jsonSystemStatus("systemStatus")("info")
    '        'Select Case True
    '        '        Case InStr(stjson.ToString, """n""") > 0
    '        '            '              Dim jsoninfo As infodetail = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(stjson), GetType(infodetail))
    '        '            Select Case True
    '        '                '        Case jsoninfo.n = "Version"
    '        '                '             InfoSystemStatus.version = jsoninfo.v
    '        '                '         Case jsoninfo.n = "Mac"
    '        '                '        InfoSystemStatus.mac = jsoninfo.v
    '        '                '        Case jsoninfo.n = "Jamming"
    '        '                '        InfoSystemStatus.jamming = jsoninfo.v
    '        '            End Select
    '        '        Case InStr(stjson.ToString, "transmitter") > 0
    '        '            For Each stjs As Object In stjson("transmitter")("available")("p")
    '        '                InfoSystemStatus.transmitter.available.p.Add(stjs.ToString)
    '        '            Next
    '        '            WriteLog("DBG: transmitter.available : " & InfoSystemStatus.transmitter.available.p.Count)
    '        '        Case (InStr(stjson.ToString, "receiver") > 0) And (InStr(stjson.ToString, "available") > 0)
    '        '            For Each stjs As Object In stjson("receiver")("available")("p")
    '        '                InfoSystemStatus.receiver.available.p.Add(stjs.ToString)
    '        '            Next
    '        '            WriteLog("DBG: receiver.available : " & InfoSystemStatus.receiver.available.p.Count)
    '        '        Case (InStr(stjson.ToString, "receiver") > 0) And (InStr(stjson.ToString, "enabled") > 0)
    '        '            For Each stjs As Object In stjson("receiver")("enabled")("p")
    '        '                InfoSystemStatus.receiverenabled.enabled.p.Add(stjs.ToString)
    '        '            Next
    '        '            WriteLog("DBG: receiverenabled.enabled : " & InfoSystemStatus.receiverenabled.enabled.p.Count)
    '        '        Case (InStr(stjson.ToString, "repeater") > 0) And (InStr(stjson.ToString, "available") > 0)
    '        '            For Each stjs As Object In stjson("repeater")("available")("p")
    '        '                InfoSystemStatus.repeater.available.p.Add(stjs.ToString)
    '        '            Next
    '        '            WriteLog("DBG: repeater.available : " & InfoSystemStatus.repeater.available.p.Count)
    '        '        Case (InStr(stjson.ToString, "repeater") > 0) And (InStr(stjson.ToString, "enabled") > 0)
    '        '            For Each stjs As Object In stjson("repeater")("enabled")("p")
    '        '                InfoSystemStatus.repeaterenabled.enabled.p.Add(stjs.ToString)
    '        '            Next
    '        '            WriteLog("DBG: repeaterenabled.enabled : " & InfoSystemStatus.repeaterenabled.enabled.p.Count)
    '        '    End Select
    '        'Next

    '        Dim _libelleadr1 As String = ""
    '        Dim ptc As String = ""
    '        For i As Integer = 0 To InfoSystemStatus.transmitter.available.p.Count - 1
    '            Select Case True
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "X10") > 0 'Frame Protocol 1		X10, infotype 0, 1
    '                    ptc = "1 # X10|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "VISONIC") > 0 'Frame Protocol 2		VISONIC, infotype 2
    '                    ptc = "2 # VISONIC|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "BLYSS") > 0 'Frame Protocol 3		BLYSS, infotype 1
    '                    ptc = "3 # BLYSS|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "CHACON") > 0  'Frame Protocol 4		CHACON, infotype 1
    '                    ptc = "4 # CHACON|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "OREGON") > 0  'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
    '                    ptc = "5 # OREGON|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "DOMIA") > 0  'Frame Protocol 6		DOMIA, infotype 0
    '                    ptc = "6 # DOMIA|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "OWL") > 0  'Frame Protocol 7		OWL, infotype 8
    '                    ptc = "7 # OWL|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "X2D") > 0  'Frame Protocol 8		X2D, infotype 10, 11
    '                    ptc = "8 # X2D|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "RTS") > 0  'Frame Protocol 9		RTS, infotype 3
    '                    ptc = "9 # RTS|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "KD101") > 0  'Frame Protocol 10	KD101, infotype 1
    '                    ptc = "10 # KD101|"
    '                Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "PARROT") > 0  'Frame Protocol 11   PARROT, infotype 0
    '                    ptc = "11 # PARROT|"
    '            End Select
    '            If (ptc <> "") And (InStr(_libelleadr1, ptc) = 0) Then _libelleadr1 += ptc

    '        Next

    '        'evite les doublons 
    '        Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
    '        For j As Integer = 0 To _LabelsDevice.Count - 1
    '            ld0 = _LabelsDevice(j)
    '            Select Case ld0.NomChamp
    '                Case "ADRESSE1"
    '                    _libelleadr1 = Mid(_libelleadr1, 1, Len(_libelleadr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
    '                    ld0.Parametre = _libelleadr1
    '                    _LabelsDevice(j) = ld0
    '                    'Case "ADRESSE2"
    '                    '    _libelleadr2 = Mid(_libelleadr2, 1, Len(_libelleadr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
    '                    '    ld0.Parametre = _libelleadr2
    '                    '    _LabelsDevice(i) = ld0
    '            End Select
    '        Next

    '    Catch ex As Exception
    '        WriteLog("ERR: ReadConf Exception: " + ex.Message)
    '    End Try
    'End Sub


End Class



