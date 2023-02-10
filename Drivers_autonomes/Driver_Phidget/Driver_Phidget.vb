﻿Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports Phidgets
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Text.RegularExpressions

Public Class Driver_Phidget
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variable Driver"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "10160480-24B7-11E1-ACF1-C9F34824019B"
    Dim _Nom As String = "Phidget InterfaceKit"
    Dim _Enable As Boolean = False
    Dim _Description As String = "PhidgetInterfaceKit USB"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "USB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "PhidgetInterfaceKit USB"
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
    Dim _idsrv As String
    Dim _DeviceCommandPlus As New List(Of HoMIDom.HoMIDom.Device.DeviceCommande)
    Dim _AutoDiscover As Boolean = False

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True
    Dim _DEBUG As Boolean = False

#End Region

#Region "Declaration"
    Dim WithEvents phidgetIFK As Phidgets.InterfaceKit
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
            WriteLog("ERR: " & Me.Nom & " ExecuteCommand, exception : " & ex.Message)
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


            End Select
            Return retour
        Catch ex As Exception
            Return "Une erreur est apparue lors de la vérification du champ " & Champ & ": " & ex.ToString
        End Try
    End Function

    Public Sub Start() Implements HoMIDom.HoMIDom.IDriver.Start
        'cree l'objet
        Try
            phidgetIFK = New Phidgets.InterfaceKit
            phidgetIFK.open()
            WriteLog("Phidget InterfaceKit Start, Carte connectée")
            _IsConnect = True
        Catch ex As Exception
            WriteLog("ERR: Phidget InterfaceKit Start, Erreur lors du démarrage du driver: " & ex.ToString)
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
            phidgetIFK.close()
            WriteLog("Phidget InterfaceKit Stop, Driver arrêté")
            _IsConnect = False
        Catch ex As Exception
            WriteLog("ERR: Phidget InterfaceKit Stop, Erreur lors de l'arrêt du driver: " & ex.ToString)
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

    Public Sub Read(ByVal Objet As Object) Implements HoMIDom.HoMIDom.IDriver.Read
        Try
            If _Enable = False Then Exit Sub
            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

            Try ' lecture de la variable debug, permet de rafraichir la variable debug sans redemarrer le service
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur de lecture de debug : " & ex.Message)
            End Try

            Dim typadr As String
            Dim adr1 As Integer

            typadr = Mid(Objet.Adresse1, 1, 1)
            adr1 = Mid(Objet.Adresse1, InStr(Objet.Adresse1, ":") + 1, 1)
            WriteLog("DBG: Read Composant " & Objet.Name & " --> " & Objet.Adresse1 & " / " & Objet.type)

            If typadr = "A" Then
                Select Case Objet.Type
                    Case "HUMIDITE"   'sensor 1107, 1125H
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.1906) - 40.2), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "TEMPERATURE"  ' sensor 1124, 1125T
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.2222) - 61.11), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "BAROMETRE"   'sensor 1115
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value / 4) + 10), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case "VOLTAGE"   'sensor 1117, 1123
                        Objet.Value = Regex.Replace(CStr((phidgetIFK.sensors(adr1).Value * 0.06) - 30), "[.,]", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    Case Else
                        Objet.Value = phidgetIFK.sensors(adr1).Value
                End Select
            Else
                Select Case Objet.Type
                    Case "PLUIECOURANT" ' 1mm par impulsion
                        If phidgetIFK.inputs(adr1) = True Then
                            Objet.Value = 1
                        Else
                            Objet.Value = 2
                        End If
                    Case Else
                        Objet.Value = phidgetIFK.sensors(adr1).Value
                End Select
            End If
        Catch ex As Exception
            WriteLog("ERR: READ, " & ex.ToString)
        End Try
    End Sub

    Public Sub Write(ByVal Objet As Object, ByVal Commande As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        If _Enable = False Then Exit Sub
        If _IsConnect = False Then
            WriteLog("ERR: WRITE, Le driver n'est pas démarré, impossible d'écrire sur le port")
            Exit Sub
        End If

        WriteLog("DBG: WRITE Device " & Objet.Name & " <-- " & Command())
        Try
            'Select Case Objet.Type
            '    Case "APPAREIL" Or "GENERIQUEBOOLEEN" 'APPAREIL Or GENERIQUEBOOLEAN
            If Commande = "ON" Then
                phidgetIFK.outputs(Objet.Adresse1) = True
                Objet.value = True
            End If
            If Commande = "OFF" Then
                phidgetIFK.outputs(Objet.Adresse1) = False
                Objet.value = False
            End If
            'End Select
        Catch ex As Exception
            WriteLog("ERR: Phidget InterfaceKit Write, Erreur: " & ex.ToString)
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
            x.DescriptionCommand = Description
            x.CountParam = NbParam
            _DeviceCommandPlus.Add(x)
        Catch ex As Exception
            WriteLog("ERR: " & Me.Nom & " add_devicecommande, Exception : " & ex.Message)
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
            WriteLog("ERR:" & Me.Nom & " add_devicecommande, Exception : " & ex.Message)
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
            WriteLog("ERR:" & Me.Nom & " add_devicecommande, Exception : " & ex.Message)
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
            WriteLog("ERR:" & Me.Nom & " add_devicecommande, Exception : " & ex.Message)
        End Try
    End Sub

    Public Sub New()
        _Version = Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString

        'Liste des devices compatibles
        _DeviceSupport.Add(ListeDevices.APPAREIL.ToString)
        _DeviceSupport.Add(ListeDevices.BAROMETRE.ToString)
        _DeviceSupport.Add(ListeDevices.BATTERIE.ToString)
        _DeviceSupport.Add(ListeDevices.COMPTEUR.ToString)
        _DeviceSupport.Add(ListeDevices.CONTACT.ToString)
        _DeviceSupport.Add(ListeDevices.DETECTEUR.ToString)
        _DeviceSupport.Add(ListeDevices.ENERGIEINSTANTANEE.ToString)
        _DeviceSupport.Add(ListeDevices.GENERIQUEBOOLEEN.ToString)
        _DeviceSupport.Add(ListeDevices.GENERIQUEVALUE.ToString)
        _DeviceSupport.Add(ListeDevices.HUMIDITE.ToString)
        _DeviceSupport.Add(ListeDevices.LAMPE.ToString)
        _DeviceSupport.Add(ListeDevices.SWITCH.ToString)
        _DeviceSupport.Add(ListeDevices.TELECOMMANDE.ToString)
        _DeviceSupport.Add(ListeDevices.TEMPERATURE.ToString)
        _DeviceSupport.Add(ListeDevices.TEMPERATURECONSIGNE.ToString)
        _DeviceSupport.Add(ListeDevices.UV.ToString)
        _DeviceSupport.Add(ListeDevices.VITESSEVENT.ToString)

        'Parametres avancés
        Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

        'Libellés Device
        Add_LibelleDevice("ADRESSE1", "Adresse", "")
        Add_LibelleDevice("ADRESSE2", "@", "")
        Add_LibelleDevice("SOLO", "@", "")
        Add_LibelleDevice("MODELE", "@", "")
        'Add_LibelleDevice("REFRESH", "Refresh (sec)", "Valeur de rafraîchissement de la mesure en secondes")
        'Add_LibelleDevice("LASTCHANGEDUREE", "LastChange Durée", "")
    End Sub
#End Region

#Region "Fonctions propres au driver"
    'attach event handler... here we'll display the interface kit details as well as determine how many output and input
    'fields to display as well as determine the range of values for the output simulator slider
    Private Sub phidgetIFK_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetIFK.Attach
        Dim a As String = ""
        WriteLog("Phidget InterfaceKit Details, Carte connectée: " & phidgetIFK.Attached.ToString())
        WriteLog("Phidget InterfaceKit Details, Nom: " & phidgetIFK.Name)
        WriteLog("Phidget InterfaceKit Details, Serial Number: " & sender.SerialNumber.ToString())
        WriteLog("Phidget InterfaceKit Details, Version: " & sender.Version.ToString())
        WriteLog("Phidget InterfaceKit Details, Nombre d'entrées: " & sender.inputs.Count.ToString())
        WriteLog("Phidget InterfaceKit Details, Nombre de sorties: " & phidgetIFK.outputs.Count.ToString())
    End Sub

    ''ifkit detach event handler... here we display the statu, which will be false as the device is not attached.  We
    ''will also clear the display fields and hide the inputs and outputs.
    Private Sub phidgetIFK_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetIFK.Detach
        WriteLog("Phidget InterfaceKit Details, Carte connectée: " & phidgetIFK.Attached.ToString())
    End Sub

    Private Sub phidgetIFK_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetIFK.Error
        WriteLog("ERR: Phidget InterfaceKit Error, Erreur: " & e.Description)
    End Sub

    'digital input change event handler... here we check or uncheck the corresponding input checkbox based on the index of
    'the digital input that generated the event
    Private Sub phidgetIFK_InputChange(ByVal sender As Object, ByVal e As Phidgets.Events.InputChangeEventArgs) Handles phidgetIFK.InputChange
        traitement(e.Value, e.Index)
        WriteLog("DBG: Phidget InterfaceKit InputChange, Entrée N°:" & e.Index & " Value:" & e.Value)
    End Sub

    ''digital output change event handler... here we check or uncheck the corresponding output checkbox based on the index of
    ''the output that generated the event
    Private Sub phidgetIFK_OutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.OutputChangeEventArgs) Handles phidgetIFK.OutputChange
        WriteLog("DBG: Phidget InterfaceKit OutputChange, Sortie N°:" & e.Index & " Value:" & e.Value)
    End Sub

    ''analog output change event handler... here we check or uncheck the corresponding output checkbox based on the index of
    ''the output that generated the event
    Private Sub phidgetIFK_ANAOutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.SensorChangeEventArgs) Handles phidgetIFK.SensorChange
        WriteLog("DBG: AnalogicInputChange Sortie Num: " & e.Index & " Value: " & e.Value)
    End Sub

    ''' <summary>Traite les paquets reçus</summary>
    ''' <remarks></remarks>
    Private Sub traitement(ByVal valeur As Boolean, ByVal adresse As String)
        Try
            'Recherche si un device affecté
            Dim listedevices As New ArrayList

            listedevices = _Server.ReturnDeviceByAdresse1TypeDriver(_idsrv, adresse, "CONTACT", Me._ID, True)
            'un device trouvé on maj la value
            If (listedevices.Count = 1) Then
                'correction valeur pour correspondre au type de value
                If TypeOf listedevices.Item(0).Value Is Integer Then
                    If valeur = True Then
                        valeur = 100
                    ElseIf valeur = False Then
                        valeur = 0
                    End If
                End If

                listedevices.Item(0).Value = valeur

            ElseIf (listedevices.Count > 1) Then
                WriteLog("ERR: Phidget InterfaceKit Process, Plusieurs devices correspondent à : " & adresse & ":" & valeur)
            Else
                WriteLog("ERR: Phidget InterfaceKit Process, Device non trouvé : " & adresse & ":" & valeur)
                'si autodiscover = true ou modedecouverte du serveur actif alors on crée le composant sinon on logue
                If _AutoDiscover Or _Server.GetModeDecouverte Then
                    WriteLog("DBG: Phidget InterfaceKit Process, Device non trouvé, AutoCreation du composant : " & adresse & ":" & valeur)
                    _Server.AddDetectNewDevice(adresse, _ID, "", "", valeur)
                Else
                    WriteLog("ERR: Phidget InterfaceKit Process, Device non trouvé : " & adresse & ":" & valeur)
                End If
            End If
        Catch ex As Exception
            WriteLog("ERR: Phidget InterfaceKit traitement, Exception : " & ex.Message & " --> " & adresse & " : " & valeur)
        End Try
    End Sub
    Private Sub WriteLog(ByVal message As String)
        Try
            'utilise la fonction de base pour loguer un event
            If STRGS.InStr(message, "DBG:") > 0 Then
                If _DEBUG Then
                    _Server.Log(TypeLog.DEBUG, TypeSource.DRIVER, "Phidget SBC", STRGS.Right(message, message.Length - 5))
                End If
            ElseIf STRGS.InStr(message, "ERR:") > 0 Then
                _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget SBC", STRGS.Right(message, message.Length - 5))
            Else
                _Server.Log(TypeLog.INFO, TypeSource.DRIVER, "Phidget SBC", message)
            End If
        Catch ex As Exception
            _Server.Log(TypeLog.ERREUR, TypeSource.DRIVER, "Phidget SBC WriteLog", ex.Message)
        End Try
    End Sub

#End Region

End Class
