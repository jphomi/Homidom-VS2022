Imports HoMIDom
Imports HoMIDom.HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Linq
Imports System.Threading.Tasks
Imports System.IO
'Imports System.IO.Ports

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
    Dim _Server As HoMIDom.HoMIDom.Server
    Dim _Device As HoMIDom.HoMIDom.Device
    Dim _DeviceSupport As New ArrayList
    Dim _Parametres As New ArrayList
    Dim _LabelsDriver As New ArrayList
    Dim _LabelsDevice As New ArrayList
    Dim MyTimer As New Timers.Timer
    Dim _IdSrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'parametres avancés
    Dim _DEBUG As Boolean = False

    'Private z_seriLogger As New LoggerConfiguration()
    Private z_serialport As ZigBeeSerialPort
    Private z_flowcontrol As FlowControl = FlowControl.FLOWCONTROL_OUT_NONE
    Private z_manager As ZigBeeNetworkManager
    Private z_jsondatastore As IZigBeeNetworkDataStore
    Private z_discoveryextention As ZigBeeDiscoveryExtension
    Private z_dongle As IZigBeeTransportTransmit
    Dim dongle_type As String = "TiCc2531"
    Dim resetnetwork As Boolean = False
    Dim panId As Integer = 1
    Private ExtendedPanId = New ExtendedPanId
    Private channel = ZigBeeChannel.CHANNEL_11
    Private networkKey = ZigBeeKey.CreateRandom()
    Private linkKey = New ZigBeeKey

    Dim MyAppliRep As String = ""

    Private WithEvents Port As New IO.Ports.SerialPort
    Private port_name As String = ""
    Dim _baudspeed As Integer = 115200
    Dim _nbrebit As Integer = 8
    Dim _parity As IO.Ports.Parity = IO.Ports.Parity.None
    Dim _nbrebitstop As IO.Ports.StopBits = IO.Ports.StopBits.One

#End Region

#Region "Variables Internes"

    Dim FrameStr As String = ""
    Dim FrameCompleted As Boolean = False
    Dim jsonObjDatas As Object

    Public InfoSystemStatus As New Systemstatus
    Public ListOfDevices As List(Of device) = New List(Of device)

    Public Class Systemstatus
        Public version As String
        Public mac As String
        Public jamming As String
        Public transmitter As New ListAvailable
        Public receiver As New ListAvailable
        Public receiverenabled As New ListEnabled
        Public repeater As New ListAvailable
        Public repeaterenabled As New ListEnabled
    End Class
    Public Class ListAvailable
        Public available As New ListP
    End Class
    Public Class ListEnabled
        Public enabled As New ListP
    End Class
    Public Class Infodetail
        Public n As String
        Public v As String
    End Class
    Public Class ListP
        Public p As New List(Of String)
    End Class
    Public Class Device
        Public id As String
        Public name As String
        Public protocol As String
        Public subTypeMeaning As String
        Public qualifier As String
    End Class

    Enum ListProtocol As Integer
            X10 = 1
            VISONIC433 = 2
            VISONIC868 = 2
            BLYSS = 3
            CHACON = 4
            DOMIA = 6
            X2D433 = 8
            X2D868 = 8
            XDSHUTTER = 8
            RTS = 9
            KD101 = 10
            PARROT = 11
        End Enum
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
        Try
            If MyDevice IsNot Nothing Then
                'Pas de commande demandée donc erreur
                If Command = "" Then
                    Return False
                Else
                    ' Traitement de la commande 
                    Select Case UCase(Command)
                        Case "ADD_ASSOCIATION"
                            ' CO_CR_LEARNMODE
                            Dim buf1 As Byte() = {&H55, &H0, &H1, &H0, &H5, &H23, &H1, &H0}
                            'port.Write(buf1, 0, 8)
                            WriteLog("ExecuteCommand, Passage par la commande ADD_ASSOCIATION ")
                        Case "STOP_ASSOCIATION"
                            ' CO_CR_LEARNMODE
                            Dim buf1 As Byte() = {&H55, &H0, &H1, &H0, &H5, &H23, &H0, &H0}
                            '       port.Write(buf1, 0, 8)
                            WriteLog("ExecuteCommand, Passage par la commande STOP_ASSOCIATION ")
                            Return True
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
                        ' Suppression des espaces inutiles
                        If InStr(Value, ":") Then
                            Dim ParaAdr2 = Split(Value, ":")
                            Value = Trim(ParaAdr2(0)) & ":" & Trim(ParaAdr2(1))
                        End If

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

            '  z_seriLogger.MinimumLevel.Debug()
            '  z_seriLogger.CreateLogger()

            If _Com <> "" Then
                WriteLog("Demarrage du pilote, ceci peut prendre plusieurs secondes")
                _IsConnect = ouvrir(_Com)
                If Not _IsConnect Then Exit Sub
                WriteLog("Driver " & Me.Nom & " démarré ")
            End If
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

                Dim ParaAdr1 = Split(Objet.Adresse1, "#")
                ParaAdr1(0) = Trim(ParaAdr1(0))
                ParaAdr1(1) = Trim(ParaAdr1(1))
                Dim qlif As String = "0"
                For i As Integer = 0 To ListOfDevices.Count - 1
                    If (ListOfDevices.Item(i).id = Objet.Adresse2) And (ListOfDevices.Item(i).protocol = ParaAdr1(0)) Then
                        qlif = ListOfDevices.Item(i).qualifier
                    End If
                Next


                Select Case True
                    Case (Objet.Type = "LAMPE" Or Objet.Type = "APPAREIL" Or Objet.Type = "SWITCH" Or Objet.Type = "VOLET")
                        texteCommande = UCase(Commande)
                        Select Case True
                        Case UCase(Commande) = "ON"
                            ' z_manager.Send(z_dongle.)
                            Select Case ParaAdr1(0)
                                    Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                                        'WriteInfoType0("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                                   '     WriteInfoType2("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                                    Case "4" 'Frame Protocol 4		CHACON, infotype 1
                                    Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                                    Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                                   '     WriteInfoType0("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "7" 'Frame Protocol 7		OWL, infotype 8
                                    Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                                    Case "9" 'Frame Protocol 9		RTS, infotype 3
                                        If qlif = "0" Then
                                        '        WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                    Else
                                        '        WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                    End If
                                    Case "10" 'Frame Protocol 10	KD101, infotype 1
                                    Case "11" 'Frame Protocol 11   PARROT, infotype 0
                                    '    WriteInfoType0("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                            End Select
                                If Objet.Type = "VOLET" Then
                                    Objet.value = 100
                                Else
                                    Objet.value = True
                                End If

                            Case UCase(Commande) = "OFF"
                                Select Case ParaAdr1(0)
                                    Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                                 '       WriteInfoType0("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                                 '       WriteInfoType2("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                                    Case "4" 'Frame Protocol 4		CHACON, infotype 1
                                    Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                                    Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                                  '      WriteInfoType0("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                Case "7" 'Frame Protocol 7		OWL, infotype 8
                                    Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                                    Case "9" 'Frame Protocol 9		RTS, infotype 3
                                        If qlif = "0" Then
                                        '       WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                    Else
                                        '      WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                    End If
                                    Case "10" 'Frame Protocol 10	KD101, infotype 1
                                    Case "11" 'Frame Protocol 11   PARROT, infotype 0
                                    '   WriteInfoType0("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                            End Select
                                If Objet.Type = "VOLET" Then
                                    Objet.value = 0
                                Else
                                    Objet.value = False
                                End If

                            Case UCase(Commande) = "DIM" Or UCase(Commande) = "OUVERTURE"
                                Select Case ParaAdr1(0)
                                    Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                                    Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                                    Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                                    Case "4" 'Frame Protocol 4		CHACON, infotype 1
                                    Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                                    Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                                    Case "7" 'Frame Protocol 7		OWL, infotype 8
                                    Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                                    Case "9" 'Frame Protocol 9		RTS, infotype 3
                                        ' If qlif = "0" Then
                                        'End If
                                        Select Case True
                                            Case Parametre1 < 1   ' traitement ON/OFF et non pas touche My
                                                If qlif = "0" Then
                                                '    WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                            Else
                                                '      WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                            End If
                                            Case Parametre1 > 99
                                                If qlif = "0" Then
                                                '      WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                            Else
                                                '       WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                            End If
                                            Case Else
                                            '   WriteInfoType3("ZIA++DIM %" & Parametre1 & " " & ParaAdr1(1) & " " & Objet.Adresse2)
                                    End Select
                                    Case "10" 'Frame Protocol 10	KD101, infotype 1
                                    Case "11" 'Frame Protocol 11   PARROT, infotype 0
                                End Select
                                Objet.value = Parametre1


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
                                    Objet.value = ValDimmer
                                End If

                            Case "ALL_LIGHT_ON"
                                Select Case ParaAdr1(0)
                                    Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                                    Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                                    Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                                    Case "4" 'Frame Protocol 4		CHACON, infotype 1
                                    Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                                    Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                                    Case "7" 'Frame Protocol 7		OWL, infotype 8
                                    Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                                    Case "9" 'Frame Protocol 9		RTS, infotype 3
                                        If qlif = "0" Then
                                        '  WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2)
                                    Else
                                        '  WriteInfoType3("ZIA++ON " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                    End If
                                    Case "10" 'Frame Protocol 10	KD101, infotype 1
                                    Case "11" 'Frame Protocol 11   PARROT, infotype 0
                                End Select
                                Objet.value = True

                            Case "ALL_LIGHT_OFF"
                                Select Case ParaAdr1(0)
                                    Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                                    Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                                    Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                                    Case "4" 'Frame Protocol 4		CHACON, infotype 1
                                    Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                                    Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                                    Case "7" 'Frame Protocol 7		OWL, infotype 8
                                    Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                                    Case "9" 'Frame Protocol 9		RTS, infotype 3
                                        If qlif = "0" Then
                                        ' WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2)
                                    Else
                                        '  WriteInfoType3("ZIA++OFF " & ParaAdr1(1) & " " & Objet.Adresse2 & " QUALIFIER " & qlif)
                                    End If
                                    Case "10" 'Frame Protocol 10	KD101, infotype 1
                                    Case "11" 'Frame Protocol 11   PARROT, infotype 0
                                End Select
                                Objet.value = False
                        End Select
                        WriteLog("DBG: " & "ExecuteCommand, Passage par la commande " & UCase(Commande))
                End Select
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
            Add_ParamAvance("Dongle type", "Type de clef", "TiCc2531")
            '        Add_ParamAvance("AfficheLog", "Afficher Log OpenZwave à l'écran (True/False)", True)
            '        Add_ParamAvance("StartIdleTime", "Durée durant laquelle le driver ne traite aucun message lors de son démarrage (en secondes).", 10)

            'ajout des commandes avancées pour les devices
            Add_DeviceCommande("ALL_LIGHT_ON", "", 0)
                Add_DeviceCommande("ALL_LIGHT_OFF", "", 0)
                Add_DeviceCommande("SETPOINT", "", 0)

                'Libellé Driver
                Add_LibelleDriver("HELP", "Aide...", "Ce module permet de recuperer les informations delivrées par un contrôleur Z-Wave ")

                'Libellé Device
                Add_LibelleDevice("ADRESSE1", "Protocole", "Protocole utilisé")
                Add_LibelleDevice("ADRESSE2", "Label de la donnée:Index", "'Temperature', 'Relative Humidity', 'Battery Level' suivi de l'index (si necessaire)")
                Add_LibelleDevice("SOLO", "@", "")
                Add_LibelleDevice("MODELE", "@", "")

            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " New", ex.Message)
            End Try
        End Sub

        ''' <summary>Si refresh >0 gestion du timer</summary>
        ''' <remarks>PAS UTILISE CAR IL FAUT LANCER UN TIMER QUI LANCE/ARRETE CETTE FONCTION dans Start/Stop</remarks>
        Private Sub TimerTick(ByVal source As Object, ByVal e As System.Timers.ElapsedEventArgs)

        End Sub

#End Region

#Region "Fonctions internes"

    '-----------------------------------------------------------------------------
    ''' <summary>Ouvrir le port Zigbee</summary>
    ''' <param name="numero">Nom/Numero du port COM: COM2</param>
    ''' <remarks></remarks>
    Private Function Ouvrir(ByVal numero As String) As Boolean
        Try
            Try
                If Not _IsConnect Then
                    ' Test d'ouveture du port Com du controleur 
                    Port.PortName = numero
                    Port.BaudRate = _baudspeed
                    WriteLog("Ouvrir - Ouverture du port " & Port.PortName & " à la vitesse " & Port.BaudRate & Port.DataBits & Port.Parity.ToString & Port.StopBits.ToString)
                    Port.Open()
                    ' Le port existe ==> le controleur est present
                    If Port.IsOpen() Then
                        Port.Close()
                    Else
                        WriteLog("ERR: Le controleur ZigBee n'est pas présent sur le port " & port_name)
                        Return False
                    End If

                    Select Case True
                        Case dongle_type = "Ember"
                            z_flowcontrol = FlowControl.FLOWCONTROL_OUT_XONOFF
                        Case Else
                            z_flowcontrol = FlowControl.FLOWCONTROL_OUT_NONE
                    End Select
                    z_flowcontrol = FlowControl.FLOWCONTROL_OUT_NONE

                    WriteLog("DBG: z_serialport = New ZigBeeSerialPort(" & Port.PortName & "," & Port.BaudRate & "," & z_flowcontrol & ")")
                    z_serialport = New ZigBeeSerialPort(Port.PortName, Port.BaudRate, z_flowcontrol)

                    Select Case True
                        Case dongle_type = "TiCc2531"
                            z_dongle = New ZigBeeDongleTiCc2531(z_serialport)
                        Case dongle_type = "DigiXbee"
                            z_dongle = New XBee.ZigBeeDongleXBee(z_serialport)
                        Case dongle_type = "ConBee"
                            z_dongle = New ZigbeeDongleConBee(z_serialport)
                        Case dongle_type = "Ember"
                            z_dongle = New ZigBeeDongleEzsp(z_serialport)
                        Case Else
                            z_dongle = New ZigBeeDongleTiCc2531(z_serialport)
                    End Select

                    z_jsondatastore = New JsonNetworkDataStore(My.Application.Info.DirectoryPath & "\drivers\Zigbee")

                    z_manager = New ZigBeeNetworkManager(z_dongle)
                    z_manager.SetNetworkDataStore(z_jsondatastore)

                    ' z_discoveryextention = New ZigBeeDiscoveryExtension
                    ' z_discoveryextention.SetUpdatePeriod(60)
                    ' z_manager.AddExtension(z_discoveryextention)

                    WriteLog("z_manager.Initialize")
                    z_manager.Initialize()

                    z_manager.AddCommandListener(New ZigBeeTransaction(z_manager))

                    z_manager.AddSupportedClientCluster(ZclOnOffCluster.CLUSTER_ID)
                    z_manager.AddSupportedClientCluster(ZclColorControlCluster.CLUSTER_ID)

                    z_manager.AddExtension(New ZigBeeBasicServerExtension())
                    z_manager.AddExtension(New ZigBeeIasCieExtension())

                    WriteLog("PAN ID = " & z_manager.ZigBeePanId)
                    WriteLog("Extended PAN ID = " & z_manager.ZigBeeExtendedPanId.ToString)
                    WriteLog("Channel = " & z_manager.ZigbeeChannel)
                    If dongle_type = "ConBee" Then
                        WriteLog("Network Key = " & z_manager.ZigBeeNetworkKey.Key.ToString)
                        WriteLog("Link Key = " & z_manager.ZigBeeLinkKey.Key.ToString)
                    End If

                    If resetnetwork Then
                        'TODO: make the network parameters configurable
                        Dim panId As Integer = 1
                        ExtendedPanId = New ExtendedPanId()
                        channel = ZigBeeChannel.CHANNEL_11
                        networkKey = ZigBeeKey.CreateRandom()
                        Dim keybyte As Byte() = {&H5A, &H69, &H67, &H42, &H65, &H65, &H41, &H6C, &H6C, &H69, &H61, &H6E, &H63, &H65, &H30, &H39}
                        linkKey = New ZigBeeKey(keybyte)

                        WriteLog("*** Resetting network")
                        WriteLog("  * PAN ID = " & panId)
                        WriteLog("  * Extended PAN ID  = " & ExtendedPanId)
                        WriteLog("  * Channel          = " & channel)
                        WriteLog("  * Network Key      = " & networkKey)
                        WriteLog("  * Link Key         = " & linkKey)

                        z_manager.SetZigBeeChannel(channel)
                        z_manager.SetZigBeePanId(panId)
                        z_manager.SetZigBeeExtendedPanId(ExtendedPanId)
                        z_manager.SetZigBeeNetworkKey(networkKey)
                        z_manager.SetZigBeeLinkKey(linkKey)
                    End If

                    '  Dim i As Integer
                    '   For i = 0 To z_manager.Nodes.Count - 1
                    '    WriteLog("Noeud " & z_manager.Nodes(i).LogicalType & ", Adresse : " & z_manager.Nodes(i).NetworkAddress)
                    '  Next

                    Dim startupSucceded = z_manager.Startup(resetnetwork)

                    Return startupSucceded
                    ' Dim coord = z_manager.GetNode(0)
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
    Sub StartInclusionMode(Optional ByVal NumProtocole As String = Nothing, Optional ByVal iddevice As String = Nothing)
            Select Case NumProtocole
                Case "1" 'Frame Protocol 1		X10, infotype 0, 1
                Case "2" 'Frame Protocol 2		VISONIC, infotype 2
                Case "3" 'Frame Protocol 3		BLYSS, infotype 1
                Case "4" 'Frame Protocol 4		CHACON, infotype 1
                Case "5" 'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                Case "6" 'Frame Protocol 6		DOMIA, infotype 0
                Case "7" 'Frame Protocol 7		OWL, infotype 8
                Case "8" 'Frame Protocol 8		X2D, infotype 10, 11
                Case "9" 'Frame Protocol 9		RTS, infotype 3

            Case "10" 'Frame Protocol 10	KD101, infotype 1
                Case "11" 'Frame Protocol 11   PARROT, infotype 0
            End Select

        End Sub

        Private Sub SendToSerial(frame As String, timeout As Integer)
            Try
                Dim utf8Encoding As New System.Text.UTF8Encoding
                Dim encodedString() As Byte
                encodedString = utf8Encoding.GetBytes(frame & vbCrLf)
                FrameCompleted = False
            'port.Write(utf8Encoding.GetString(encodedString))
            WriteLog("DBG: SendToSerial, " & frame)
                Dim i As Integer = 0
                While (timeout >= i)
                    System.Threading.Thread.Sleep(1000 * i)
                    i = i + 1
                    If FrameCompleted Then Exit While
                End While

            Catch ex As Exception
                WriteLog("ERR: SendToSerial Exception: " + ex.Message)
            End Try
            System.Threading.Thread.Sleep(100)
        End Sub

    Private Sub ReadConf(str As String)
        Try
            WriteLog("DBG: ReadConf à traiter : " & str)
            '      Dim jsonSystemStatus As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(str)

            ''     For Each stjson As Object In jsonSystemStatus("systemStatus")("info")
            'Select Case True
            '        Case InStr(stjson.ToString, """n""") > 0
            '            '              Dim jsoninfo As infodetail = Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(stjson), GetType(infodetail))
            '            Select Case True
            '                '        Case jsoninfo.n = "Version"
            '                '             InfoSystemStatus.version = jsoninfo.v
            '                '         Case jsoninfo.n = "Mac"
            '                '        InfoSystemStatus.mac = jsoninfo.v
            '                '        Case jsoninfo.n = "Jamming"
            '                '        InfoSystemStatus.jamming = jsoninfo.v
            '            End Select
            '        Case InStr(stjson.ToString, "transmitter") > 0
            '            For Each stjs As Object In stjson("transmitter")("available")("p")
            '                InfoSystemStatus.transmitter.available.p.Add(stjs.ToString)
            '            Next
            '            WriteLog("DBG: transmitter.available : " & InfoSystemStatus.transmitter.available.p.Count)
            '        Case (InStr(stjson.ToString, "receiver") > 0) And (InStr(stjson.ToString, "available") > 0)
            '            For Each stjs As Object In stjson("receiver")("available")("p")
            '                InfoSystemStatus.receiver.available.p.Add(stjs.ToString)
            '            Next
            '            WriteLog("DBG: receiver.available : " & InfoSystemStatus.receiver.available.p.Count)
            '        Case (InStr(stjson.ToString, "receiver") > 0) And (InStr(stjson.ToString, "enabled") > 0)
            '            For Each stjs As Object In stjson("receiver")("enabled")("p")
            '                InfoSystemStatus.receiverenabled.enabled.p.Add(stjs.ToString)
            '            Next
            '            WriteLog("DBG: receiverenabled.enabled : " & InfoSystemStatus.receiverenabled.enabled.p.Count)
            '        Case (InStr(stjson.ToString, "repeater") > 0) And (InStr(stjson.ToString, "available") > 0)
            '            For Each stjs As Object In stjson("repeater")("available")("p")
            '                InfoSystemStatus.repeater.available.p.Add(stjs.ToString)
            '            Next
            '            WriteLog("DBG: repeater.available : " & InfoSystemStatus.repeater.available.p.Count)
            '        Case (InStr(stjson.ToString, "repeater") > 0) And (InStr(stjson.ToString, "enabled") > 0)
            '            For Each stjs As Object In stjson("repeater")("enabled")("p")
            '                InfoSystemStatus.repeaterenabled.enabled.p.Add(stjs.ToString)
            '            Next
            '            WriteLog("DBG: repeaterenabled.enabled : " & InfoSystemStatus.repeaterenabled.enabled.p.Count)
            '    End Select
            'Next

            Dim _libelleadr1 As String = ""
            Dim ptc As String = ""
            For i As Integer = 0 To InfoSystemStatus.transmitter.available.p.Count - 1
                Select Case True
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "X10") > 0 'Frame Protocol 1		X10, infotype 0, 1
                        ptc = "1 # X10|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "VISONIC") > 0 'Frame Protocol 2		VISONIC, infotype 2
                        ptc = "2 # VISONIC|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "BLYSS") > 0 'Frame Protocol 3		BLYSS, infotype 1
                        ptc = "3 # BLYSS|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "CHACON") > 0  'Frame Protocol 4		CHACON, infotype 1
                        ptc = "4 # CHACON|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "OREGON") > 0  'Frame Protocol 5		Oregon, infotype 4, 5, 6, 7, 9
                        ptc = "5 # OREGON|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "DOMIA") > 0  'Frame Protocol 6		DOMIA, infotype 0
                        ptc = "6 # DOMIA|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "OWL") > 0  'Frame Protocol 7		OWL, infotype 8
                        ptc = "7 # OWL|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "X2D") > 0  'Frame Protocol 8		X2D, infotype 10, 11
                        ptc = "8 # X2D|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "RTS") > 0  'Frame Protocol 9		RTS, infotype 3
                        ptc = "9 # RTS|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "KD101") > 0  'Frame Protocol 10	KD101, infotype 1
                        ptc = "10 # KD101|"
                    Case InStr(InfoSystemStatus.transmitter.available.p.Item(i), "PARROT") > 0  'Frame Protocol 11   PARROT, infotype 0
                        ptc = "11 # PARROT|"
                End Select
                If (ptc <> "") And (InStr(_libelleadr1, ptc) = 0) Then _libelleadr1 += ptc

            Next

            'evite les doublons 
            Dim ld0 As New HoMIDom.HoMIDom.Driver.cLabels
            For j As Integer = 0 To _LabelsDevice.Count - 1
                ld0 = _LabelsDevice(j)
                Select Case ld0.NomChamp
                    Case "ADRESSE1"
                        _libelleadr1 = Mid(_libelleadr1, 1, Len(_libelleadr1) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        ld0.Parametre = _libelleadr1
                        _LabelsDevice(j) = ld0
                        'Case "ADRESSE2"
                        '    _libelleadr2 = Mid(_libelleadr2, 1, Len(_libelleadr2) - 1) 'enleve le dernier | pour eviter davoir une ligne vide a la fin
                        '    ld0.Parametre = _libelleadr2
                        '    _LabelsDevice(i) = ld0
                End Select
            Next

        Catch ex As Exception
            WriteLog("ERR: ReadConf Exception: " + ex.Message)
        End Try
    End Sub

    Private Sub ReadDatas(str As String)
            Try
                WriteLog("DBG: ReadDatas à traiter : " + str)
            '  Dim jsonObjConf As Object = Newtonsoft.Json.JsonConvert.DeserializeObject(str)

            '  Dim st As String = jsonObjConf("frame")("header")("infoType")
            'WriteLog("DBG: infoType: " & st)

            '    Select Case st
            '        Case "0" 'Frame infoType 0		ON/OFF
            '        Case "1" 'Frame infoType 1		ON/OFF   error in API receive id instead of id_lsb and id_msb
            '        Case "2" 'Frame infoType 2		Visonic
            '        Case "3" 'Frame infoType 3		RTS

            '    Case "4" 'Frame infoType 4		Oregon thermo/hygro sensors
            '        Case "5" 'Frame infoType 5		Oregon thermo/hygro/pressure sensors
            '        Case "6" 'Frame infoType 6		Oregon Wind sensors
            '        Case "7" 'Frame infoType 7		Oregon UV sensors
            '        Case "8" 'Frame infoType 8		OWL Energy/power sensors
            '        Case "9" 'Frame infoType 9		Oregon Rain sensors
            '        Case "10" 'Frame infoType 10	Thermostats  X2D protocol
            '        Case "11" 'Frame infoType 11   Alarm X2D protocol / Shutter
            '    End Select



        Catch ex As Exception
                WriteLog("ERR: ReadDatas Exception: " + ex.Message)
            End Try
        End Sub

    Private Sub WriteLog(message)
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



