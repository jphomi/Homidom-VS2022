﻿Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports ActiveHomeScriptLib

Public Class Driver_X10_CM15
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "1DA44C90-353F-11E1-9B3D-78FC4824019B"
    Dim _Nom As String = "X10 CM15"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Driver X10 CM15"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "CM15"
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
#End Region

#Region "Declaration"
    Dim WithEvents ActiveHomeObj As ActiveHome
#End Region

#Region "Fonctions génériques"

    Public WriteOnly Property IdSrv As String Implements HoMIDom.HoMIDom.IDriver.IdSrv
        Set(ByVal value As String)
            _idsrv = value
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

        Try

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Read", "Erreur: " & ex.ToString)
        End Try
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
                Case "ADRESSE1"
                    If String.IsNullOrEmpty(Value) Then
                        retour = "l'adresse du module est obligatoire"
                    ElseIf Len(Value) < 2 Then
                        retour = "l'adresse doit à minima comporter une lettre (House) et un chiffre (Code)"
                    ElseIf IsNumeric(Mid(Value, 1, 1)) Then
                        retour = "l'adresse doit commencer par une lettre (House), ex: A"
                    ElseIf Mid(Value, 1, 1) < "A" Or Mid(Value, 1, 1) > "P" Then
                        retour = "l'adresse House doit être compris entre Ax et Px (x numéro de Code)"
                    ElseIf IsNumeric(Mid(Value, 2, Len(Value) - 1)) = False Then
                        retour = "l'adresse doit être associée au House puis le Code qui doit être compris entre 1 et 16, ex: C3"
                    ElseIf CInt(Mid(Value, 2, Len(Value) - 1)) < 1 Or CInt(Mid(Value, 2, Len(Value) - 1)) > 16 Then
                        retour = "l'adresse doit être assciée au House puis le Code qui doit être compris entre 1 et 16, ex: C3"
                    End If
                Case "ADRESSE2"
                    If String.IsNullOrEmpty(Value) Then
                        retour = "Le type du module est obligatoire"
                    ElseIf IsNumeric(Value.ToString) = False Then
                        retour = "le type doit être numérique et doit être 0 (plc) ou 1 (RF)"
                    ElseIf CInt(Value.ToString) < 0 Or CInt(Value.ToString) > 1 Then
                        retour = "le type doit être 0 (plc) ou 1 (RF)"
                    End If
            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            ActiveHomeObj = CreateObject("X10.ActiveHome")
            If ActiveHomeObj IsNot Nothing Then
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Start", "Connectée")
                _IsConnect = True
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors de la connexion au CM15")
                _IsConnect = False
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Start", "Erreur lors du démarrage du driver: " & ex.ToString)
            _IsConnect = False
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
        'cree l'objet
        Try
            ActiveHomeObj = Nothing
            _Server.Log(TypeLog.INFO, TypeSource.DRIVER, Me.Nom & " Stop", "Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Stop", "Erreur lors de l'arrêt du driver: " & ex.ToString)
            _IsConnect = False
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
        If _Enable = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'exécuter la commande car le driver n'est pas activé (Enable)")
            Exit Sub
        End If
        If _IsConnect = False Then
            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write", "Impossible d'exécuter la commande car le driver n'est pas démarré")
            Exit Sub
        End If


        Try
            Dim TypeDev As String = ""
            Dim idx As Integer = -1

            'On vérifie que l'adresse2 est bien 0 ou 1 (type de device)
            If IsNumeric(Objet.Adresse2) Then
                idx = CInt(Objet.Adresse2)
                If idx < 0 Or idx > 1 Then
                    _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse2 du composant " & Objet.Name & " doit être compris entre 0 et 1")
                    Exit Sub
                End If
            Else
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: l'adresse2 du composant " & Objet.Name & " doit être numérique et compris entre 0 et 1")
                Exit Sub
            End If

            'On prépare la commande suivant le type de device
            Select Case idx
                Case 0
                    TypeDev = "sendplc"
                Case 1
                    TypeDev = "sendrf"
                Case 2
                    TypeDev = "sendsecurerf"
                Case 3
                    TypeDev = "sendsecurehomecontrolrf"
            End Select

            If Commande = "ON" Then
                ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " on")
                Objet.Value = 100
            End If
            If Commande = "OFF" Then
                ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " off")
                Objet.Value = 0
            End If
            If Commande = "DIM" Then
                If Parametre1 IsNot Nothing Then
                    Dim _old As Integer = Objet.ValueLast
                    Dim _new As Integer = CInt(Parametre1)
                    Dim _cmd As String = ""
                    If _old > _new Then _cmd = "dim"
                    If _old <= _new Then _cmd = "bright"
                    ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " " & Parametre1)
                    Objet.Value = Parametre1
                End If
            End If
            If Commande = "BRIGHT" Then
                If Parametre1 IsNot Nothing Then
                    ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " bright " & Parametre1)
                    Objet.Value = Parametre1
                End If
            End If
            If Commande = "OUVERTURE" Then
                If Parametre1 IsNot Nothing Then
                    'Try
                    '    Dim _var As Double = Parametre1
                    '    _var = _var * 0.25
                    '    Dim _var2 As Byte = CByte(_var)
                    '    ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " Extcode 1 " & Hex(_var2))
                    '    Objet.Value = Parametre1
                    'Catch ex As Exception
                    '    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write OUVERTURE ExtCode 1", "Erreur: " & ex.ToString)
                    '    Try
                    '        Dim hex As String
                    '        hex = Conversion.Hex(Parametre1 * 0.64)
                    '        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " extcode 31 " & ex.ToString)
                    '        Objet.Value = Parametre1
                    '    Catch ex2 As Exception
                    '        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " Write OUVERTURE ExtCode 1", "Erreur: " & ex.ToString)
                    '    End Try
                    'End Try
                    Dim _old As Integer = Objet.ValueLast
                    Dim _new As Integer = CInt(Parametre1)
                    Dim _diff As Integer = _new - _old
                    Dim _cmd As String = ""
                    If _diff < 0 Then
                        _diff = _diff * -1
                        _cmd = "dim"
                    Else
                        _cmd = "bright"
                    End If
                    If _new <= 15 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " off")
                        Objet.Value = 0
                    ElseIf _diff > 15 And _diff <= 30 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Objet.Value = Parametre1
                    ElseIf _diff > 30 And _diff <= 45 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Objet.Value = Parametre1
                    ElseIf _diff > 45 And _diff <= 60 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Objet.Value = Parametre1
                    ElseIf _diff > 60 And _diff <= 75 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Threading.Thread.Sleep(1000)
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " " & _cmd & " 80")
                        Objet.Value = Parametre1
                    ElseIf _new > 75 Then
                        ActiveHomeObj.SendAction(TypeDev, LCase(Objet.adresse1) & " on")
                        Objet.Value = 100
                    End If
                End If
            End If

        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " Write", "Erreur: " & ex.ToString)
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

        _DeviceSupport.Add(ListeDevices.SWITCH)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN)
        _DeviceSupport.Add(ListeDevices.CONTACT)
        _DeviceSupport.Add(ListeDevices.APPAREIL)
        _DeviceSupport.Add(ListeDevices.LAMPE)
        _DeviceSupport.Add(ListeDevices.VOLET)
        _DeviceSupport.Add(ListeDevices.DETECTEUR)

        'Libellé Device
        Add_LibelleDevice("ADRESSE1", "Adresse du module", "Adresse HouseCode du module (ex: C3)")
        Add_LibelleDevice("ADRESSE2", "Type de module (0-1)", "Type de module 0:sendplc (courant porteur), 1:sendrf (RF)")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"
    'events from ActiveHome: write out received event
    Sub ActiveHome_RecvAction(ByVal bszRecv As Object _
                            , ByVal vParm1 As Object _
                            , ByVal vParm2 As Object _
                            , ByVal vParm3 As Object _
                            , ByVal vParm4 As Object _
                            , ByVal vParm5 As Object _
                            , ByVal vReserved As Object) Handles ActiveHomeObj.RecvAction

        _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, Me.Nom & " RecvAction", "RecvAction: " & bszRecv & " " & vParm1 & " " & vParm2 & " " & vParm3 & " " & vParm4 & " " & vParm5)

        If String.IsNullOrEmpty(vParm1) = False Then
            Try
                'Recherche si un device affecté
                Dim listedevices As New ArrayList
                listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, vParm1, "", Me._ID, True)
                'un device trouvé on maj la value
                If listedevices IsNot Nothing Then
                    If (listedevices.Count = 1) Then
                        'correction valeur pour correspondre au type de value
                        If TypeOf listedevices.Item(0).Value Is Integer Then
                            If String.IsNullOrEmpty(vParm2) = False Then
                                If UCase(vParm2) = "ON" Then
                                    listedevices.Item(0).Value = 100
                                ElseIf UCase(vParm2) = "OFF" Then
                                    listedevices.Item(0).Value = 0
                                ElseIf UCase(vParm2) = "DIM" And IsNumeric(vParm3) Then
                                    listedevices.Item(0).Value += vParm3
                                ElseIf UCase(vParm2) = "BRIGHT" And IsNumeric(vParm3) Then
                                    listedevices.Item(0).Value -= vParm3
                                End If
                            End If
                        ElseIf TypeOf listedevices.Item(0).Value Is Boolean Then
                            If String.IsNullOrEmpty(vParm2) = False Then
                                If UCase(vParm2) = "ON" Then
                                    listedevices.Item(0).Value = True
                                ElseIf UCase(vParm2) = "OFF" Then
                                    listedevices.Item(0).Value = False
                                Else
                                    listedevices.Item(0).Value = True
                                End If
                            End If
                        End If
                    ElseIf (listedevices.Count > 1) Then
                        _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RecvAction", "Aucun ou Plusieurs devices correspondent à : " & vParm2 & ":" & vParm3)
                    Else
                        'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                        If _AutoDiscover Or _Server.GetModeDecouverte Then
                            _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "X10 traitement", "Device non trouvé, AutoCreation du composant : " & vParm1 & ":" & vParm2)
                            _Server.AddDetectNewDevice(vParm1, _ID, "", "")
                        Else
                            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "X10 traitement", "Device non trouvé : " & vParm1 & ":" & vParm2)
                        End If
                    End If
                End If
            Catch ex As Exception
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, Me.Nom & " RecvAction", "Erreur : " & ex.ToString)
            End Try
        End If
    End Sub
#End Region

End Class
