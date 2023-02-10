﻿Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.IO

' Auteur : Seb
' Date : 10/02/2011

''' <summary>Driver Velleman k8056, le device doit dans son adresse 1 indiqué sa carte et son numéro de relais séparé par un x, exemple pour le relais 1 de la carte 1: 1x1</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_k8056
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "22FED268-34F6-11E0-A23E-80D9DED72085"
    Dim _Nom As String = "K8056"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Carte Velleman k8056"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "RS232"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = ""
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "K8056"
    Dim _Version As String = My.Application.Info.Version.ToString
    Dim _OsPlatform As String = "32"
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

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim rs232 As New System.IO.Ports.SerialPort
    Dim tRelais(7, 7) As Boolean
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " ExecuteCommand", "exception : " & ex.Message)
            Return False
        End Try
    End Function


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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        If _Enable = False Then Exit Sub
    End Sub

    Public Property Refresh() As Integer Implements HoMIDom.HoMIDom.IDriver.Refresh
        Get
            Return _Refresh
        End Get
        Set(ByVal value As Integer)
            _Refresh = value
        End Set
    End Property

    Public Sub Restart() Implements HoMIDom.HoMIDom.IDriver.Restart
        [Stop]()
        Start()
    End Sub

    Public Property Server() As HoMIDom.HoMIDom.Server Implements HoMIDom.HoMIDom.IDriver.Server
        Get
            Return _Server
        End Get
        Set(ByVal value As HoMIDom.HoMIDom.Server)
            _Server = value
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


            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        Try
            With rs232
                .PortName = _Com
                .Open()
            End With
            _IsConnect = True
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "K8056", "Driver démarré")
        Catch ex As Exception
            _IsConnect = False
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "K8056", "Driver en erreur lors du démarrage: " & ex.Message)
        End Try
    End Sub

    Public Property StartAuto() As Boolean Implements HoMIDom.HoMIDom.IDriver.StartAuto
        Get
            Return _StartAuto
        End Get
        Set(ByVal value As Boolean)
            _StartAuto = value
        End Set
    End Property

    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            rs232.Close()
            _IsConnect = False
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "k8056", "Driver arrêté")
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "k8056", "Erreur lors de l'arrêt du Driver: " & ex.Message)
        End Try
    End Sub

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

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub
            If Objet.type = "APPAREIL" Then
                Dim tabl() As String = Objet.adresse1.split("x")
                If tabl IsNot Nothing Then
                    If tabl.Length = 2 Then
                        Select Case UCase(Commande)
                            Case "ON"
                                SetRelais(tabl(0), tabl(1))
                                Objet.Value = True
                            Case "OFF"
                                ClearRelais(tabl(0), tabl(1))
                                Objet.Value = False
                            Case Else
                                _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "k8056", "Erreur la commande du device n'est pas supporté par ce driver - device: " & Objet.name & " commande:" & Commande)
                        End Select
                    End If
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "k8056", "Erreur le type de device n'est pas supporté par ce driver - device: " & Objet.name & " type:" & Objet.type.ToString)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "k8056", "Erreur lors de la traitement de la commande - device: " & Objet.name & " commande:" & Commande & " erreur: " & ex.Message)
        End Try
    End Sub

    Public Sub DeleteDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.DeleteDevice

    End Sub

    Public Sub NewDevice(ByVal DeviceId As String) Implements HoMIDom.HoMIDom.IDriver.NewDevice

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
            x.DescriptionCommand = description
            x.CountParam = nbparam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
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
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " add_devicecommande", "Exception : " & ex.Message)
        End Try
    End Sub

    Public Sub New()
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        _DeviceSupport.Add(ListeDevices.APPAREIL)

        Add_LibelleDevice("ADRESSE1", "Adresse", "")
        Add_LibelleDevice("ADRESSE2", "@", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
        'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
        'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"
    Private Sub MAJ(ByVal Carte As Integer, ByVal Relais As Integer, ByVal Value As Boolean)
        If Carte < 0 Then Carte = 0
        If Carte > 7 Then Carte = 7
        If Relais < 0 Then Relais = 0
        If Relais > 7 Then Relais = 7
        tRelais(Carte, Relais) = Value
    End Sub

    Private Function ArretUrgence() As String
        Dim i As Integer
        Dim j As Integer
        Call EnvoyerTrame(1, "E", 1)
        For i = 0 To 7
            For j = 0 To 7
                MAJ(i, j, False)
            Next
        Next
        Return "Arrêt d'urgence effectué"
    End Function

    Private Sub ClearRelais(ByVal Carte As Integer, ByVal Relais As Integer)
        Call EnvoyerTrame(Carte, "C", Relais)
        MAJ(Carte, Relais, False)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "k8056", "Envoi de la commande OFF sur le relais " & Relais & " de la carte " & Carte)
    End Sub

    Private Function GetEtatRelais(ByVal Carte As Integer, ByVal Relais As Integer) As Integer
        GetEtatRelais = tRelais(Carte, Relais)
    End Function

    Private Function ResetAll() As String
        Dim i As Integer
        For i = 1 To 255
            ResetCarte(i)
        Next
        Return "Reset effectué sur toutes les cartes"
    End Function

    Private Function ResetCarte(ByVal Carte As Integer) As String
        Call EnvoyerTrame(Carte, "C", 9)
        Dim i As Integer
        For i = 0 To 7
            MAJ(Carte, i, False)
        Next
        Return "Carte: " & Carte & " - Reset effectué"
    End Function

    Private Function SetAdresseCarte(ByVal Carte As Integer, ByVal NewAdresse As Integer) As String
        Call EnvoyerTrame(Carte, "A", NewAdresse)
        Return "Carte: " & Carte & " - Nouvelle adresse: " & NewAdresse
    End Function

    Private Function SetCarte(ByVal Carte As Integer) As String
        Call EnvoyerTrame(Carte, "S", 9)
        Dim i As Integer
        For i = 0 To 7
            MAJ(Carte, i, True)
        Next
        Return "Carte: " & Carte & " - Set effectué"
    End Function

    Private Sub SetRelais(ByVal Carte As Integer, ByVal Relais As Integer)
        Call EnvoyerTrame(Carte, "S", Relais)
        MAJ(Carte, Relais, True)
        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "k8056", "Envoi de la commande ON sur le relais " & Relais & " de la carte " & Carte)
    End Sub

    Private Function ShowAdrCarte() As String
        Call EnvoyerTrame(1, "D", 1)
        Return "Afficher les adresses des cartes"
    End Function

    'Envoi de la trame à la carte
    Private Sub EnvoyerTrame(ByVal AdresseCarte As String, ByVal Instruction As String, ByVal Adresse As String)
        Dim Entete As String = Chr(13)
        Dim m_Adresse As Integer
        Dim Trame As String = ""
        Dim i As Byte = 0
        Dim checksum As Integer

        m_Adresse = CInt(AdresseCarte)
        'Calcul du CheckSum
        checksum = ((255 - (13 + m_Adresse + Asc(Instruction) + Asc(Adresse)) + 1)) 'complément à 2 des 4 bytes
        Try
            Do While i < 3
                Trame = Entete & Chr(m_Adresse) & Instruction & Adresse & Chr(checksum)
                'Envoyer la trame
                rs232.Write(Trame)
                System.Threading.Thread.Sleep(10)
                i = i + 1
            Loop
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "k8056", "Erreur lors de l'envoi de la trame: " & ex.Message)
        End Try
    End Sub

#End Region

End Class
