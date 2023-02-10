Imports HoMIDom
Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device
Imports System.Net
Imports System.IO
Imports System.Text

Imports System.Text.RegularExpressions
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.Xml
Imports System.Web
Imports Newtonsoft.Json.Linq


' Auteur : jphomi 
' Date : 01/09/2015

''' <summary>Driver d'alertes meteo, allergie, pollution</summary>
''' <remarks></remarks>
<Serializable()> Public Class Driver_AlertesAtmo
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "E50A222A-3A8E-11E5-9823-1B471E5D46B0"
    Dim _Nom As String = "AlertesAtmo"
    Dim _Enable As Boolean = False
    Dim _Description As String = "Données d'alertes atmosphériques"
    Dim _StartAuto As Boolean = False
    Dim _Protocol As String = "WEB"
    Dim _IsConnect As Boolean = False
    Dim _IP_TCP As String = "@"
    Dim _Port_TCP As String = "@"
    Dim _IP_UDP As String = "@"
    Dim _Port_UDP As String = "@"
    Dim _Com As String = "@"
    Dim _Refresh As Integer = 0
    Dim _Modele As String = "AlertesAtmos"
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

    'A ajouter dans les ppt du driver
    Dim _tempsentrereponse As Integer = 1500
    Dim _ignoreadresse As Boolean = False
    Dim _lastetat As Boolean = True

    'param avancé
    Dim _DEBUG As Boolean = False

#End Region

#Region "Variables internes"
    Dim _Obj As Object = Nothing

    Dim DatasPollen As New Datapollen
    Dim ValueIPX As New Microsoft.VisualBasic.Collection()
    Dim DataPollenRnsa As DataPollen
    'Dim ListRegionsEU As RegionsEU

    Public Class DataPollen
        Public httpCode As String
        Public errorMessage As String
        Public city As DataPollenCity
        Public data As DataPollenDetail
     End Class

    Public Class DataPollenCity
        Public departmentCode As String
        Public name As String
        Public zipCode As String
        Public inseeCode As String
        Public latitude As String
        Public longitude As String
    End Class
    Public Class DataPollenDetail
        Public alerts As List(Of DataPollenAlerts)
        Public warns As List(Of DataPollenWarns)
        Public zipCode As String
        Public inseeCode As String
        Public latitude As String
        Public longitude As String
    End Class

    Public Class DataPollenAlerts
        Public code As String
        Public geolocated As String
        Public pollens As List(Of PollensDetail)
    End Class

    Public Class DataPollenWarns
        Public code As String
        Public geolocated As String
        Public pollens As List(Of PollensDetail)
    End Class

    Public Class PollensDetail
        Public code As String
        Public name As String
        Public level As String
        Public advice As String
    End Class

    Public Class RegionsEU
        Public Regions As List(Of RegionEU)
    End Class

    Public Class RegionEU
        Public code As String
        Public name As String
    End Class

    Dim ListeDepFR As New Microsoft.VisualBasic.Collection()
    Dim ListeDepBE As New Microsoft.VisualBasic.Collection()
    Dim ListePollen As New Microsoft.VisualBasic.Collection()
    Public Sub CodeDeptFR()
        Try
            ListeDepFR.Clear()

            ListeDepFR.Add("Dep FR Inconnu", 0)
            ListeDepFR.Add("Ain", 1)
            ListeDepFR.Add("Aisne", 2)
            ListeDepFR.Add("Allier", 3)
            ListeDepFR.Add("Alpes se Haute Provence", 4)
            ListeDepFR.Add("Hautes Alpes", 5)
            ListeDepFR.Add("Alpes Maritimes", 6)
            ListeDepFR.Add("Ardèche", 7)
            ListeDepFR.Add("Ardennes", 8)
            ListeDepFR.Add("Ariège", 9)
            ListeDepFR.Add("Aube", 10)
            ListeDepFR.Add("Aude", 11)
            ListeDepFR.Add("Aveyron", 12)
            ListeDepFR.Add("Bouches du Rhône", 13)
            ListeDepFR.Add("Calvados", 14)
            ListeDepFR.Add("Cantal", 15)
            ListeDepFR.Add("Charente", 16)
            ListeDepFR.Add("Charente Maritime", 17)
            ListeDepFR.Add("Cher", 18)
            ListeDepFR.Add("Corrèze", 19)
            ListeDepFR.Add("Corse du Sud", "2A")
            ListeDepFR.Add("Haute Corse", "2B")
            ListeDepFR.Add("Côte d'Or", 21)
            ListeDepFR.Add("Côtes d'Armor", 22)
            ListeDepFR.Add("Creuse", 23)
            ListeDepFR.Add("Dordogne", 24)
            ListeDepFR.Add("Doubs", 25)
            ListeDepFR.Add("Drôme", 26)
            ListeDepFR.Add("Eure", 27)
            ListeDepFR.Add("Eure et Loir", 28)
            ListeDepFR.Add("Finistère", 29)
            ListeDepFR.Add("Gard", 30)
            ListeDepFR.Add("Haute Garonne", 31)
            ListeDepFR.Add("Gers", 32)
            ListeDepFR.Add("Gironde", 33)
            ListeDepFR.Add("Hérault", 34)
            ListeDepFR.Add("Ille et Vilaine", 35)
            ListeDepFR.Add("Indre", 36)
            ListeDepFR.Add("Indre et Loire", 37)
            ListeDepFR.Add("Isère", 38)
            ListeDepFR.Add("Jura", 39)
            ListeDepFR.Add("Landes", 40)
            ListeDepFR.Add("Loir et Cher", 41)
            ListeDepFR.Add("Loire", 42)
            ListeDepFR.Add("Haute Loire", 43)
            ListeDepFR.Add("Loire Atlantique", 44)
            ListeDepFR.Add("Loiret", 45)
            ListeDepFR.Add("Lot", 46)
            ListeDepFR.Add("Lot et Garonne", 47)
            ListeDepFR.Add("Lozère", 48)
            ListeDepFR.Add("Maine et Loire", 49)
            ListeDepFR.Add("Manche", 50)
            ListeDepFR.Add("Marne", 51)
            ListeDepFR.Add("Haute Marne", 52)
            ListeDepFR.Add("Mayenne", 53)
            ListeDepFR.Add("Meurthe et Moselle", 54)
            ListeDepFR.Add("Meuse", 55)
            ListeDepFR.Add("Morbihan", 56)
            ListeDepFR.Add("Moselle", 57)
            ListeDepFR.Add("Nièvre", 58)
            ListeDepFR.Add("Nord", 59)
            ListeDepFR.Add("Oise", 60)
            ListeDepFR.Add("Orne", 61)
            ListeDepFR.Add("Pas de Calais", 62)
            ListeDepFR.Add("Puy de Dôme", 63)
            ListeDepFR.Add("Pyrénées Atlantiques", 64)
            ListeDepFR.Add("Hautes Pyrénées", 65)
            ListeDepFR.Add("Pyrénées Orientales", 66)
            ListeDepFR.Add("Bas Rhin", 67)
            ListeDepFR.Add("Haut Rhin", 68)
            ListeDepFR.Add("Rhône", 69)
            ListeDepFR.Add("Haute Saone", 70)
            ListeDepFR.Add("Saône et Loire", 71)
            ListeDepFR.Add("Sarthe", 72)
            ListeDepFR.Add("Savoie", 73)
            ListeDepFR.Add("Haute Savoie", 74)
            ListeDepFR.Add("Paris", 75)
            ListeDepFR.Add("Seine Maritime", 76)
            ListeDepFR.Add("Seine et Marne", 77)
            ListeDepFR.Add("Yvelines", 78)
            ListeDepFR.Add("Deux Sèvres", 79)
            ListeDepFR.Add("Somme", 80)
            ListeDepFR.Add("Tarn", 81)
            ListeDepFR.Add("Tarn et Garonne", 82)
            ListeDepFR.Add("Var", 83)
            ListeDepFR.Add("Vaucluse", 84)
            ListeDepFR.Add("Vendée", 85)
            ListeDepFR.Add("Vienne", 86)
            ListeDepFR.Add("Haute Vienne", 87)
            ListeDepFR.Add("Vosges", 88)
            ListeDepFR.Add("Yonne", 89)
            ListeDepFR.Add("Territoire de Belfort", 90)
            ListeDepFR.Add("Essonne", 91)
            ListeDepFR.Add("Hauts de Seine", 92)
            ListeDepFR.Add("Seine Saint Denis", 93)
            ListeDepFR.Add("Val de Marne", 94)
            ListeDepFR.Add("Val d'Oise", 95)
            WriteLog("DBG: CodeDeptFR, " & ListeDepFR.Count & " chargé")
        Catch ex As Exception
            WriteLog("ERR: CodeDeptFR, Exception : " & ex.Message)
        End Try
    End Sub
    Public Sub CodeDeptBE()
        WriteLog("DBG: CodeDeptBE, " & " departements charges")
        Try
            ListeDepBE.Clear()

            ListeDepBE.Add("Dep BE Inconnu", "000")
            ListeDepBE.Add("Luxembourg / Luxemburg", "001")
            ListeDepBE.Add("Antwerpen / Anvers", "002")
            ListeDepBE.Add("Oost Vlaanderen/Fl.Orientale", "003")
            ListeDepBE.Add("Brabant", "004")
            ListeDepBE.Add("Hainaut / Henegouwen", "005")
            ListeDepBE.Add("Namur / Namen", "006")
            ListeDepBE.Add("Limburg / Limbourg", "007")
            ListeDepBE.Add("Liège / Luik", "008")
            ListeDepBE.Add("West Vlaanderen/Fl.Occidentale", "009")
            ListeDepBE.Add("Belgische Küste", "801")
            WriteLog("DBG: CodeDeptBE, " & ListeDepBE.Count & " chargé")
        Catch ex As Exception
            WriteLog("ERR: CodeDeptBE, Exception : " & ex.Message)
        End Try
    End Sub

    Public Sub CodePollen()
        Try
            ListePollen.Clear()

            ListePollen.Add("AMBR", "AMBROISIE")
            ListePollen.Add("ARMO", "ARMOISE")
            ListePollen.Add("AULN", "AULNE")
            ListePollen.Add("BOUL", "BOULEAU")
            ListePollen.Add("CHAR", "CHARME")
            ListePollen.Add("CHAT", "CHATAIGNIER")
            ListePollen.Add("CHEN", "CHENE")
            ListePollen.Add("CYPR", "CYPRES")
            ListePollen.Add("FREN", "FRENE")
            ListePollen.Add("HETR", "HETRE")
            ListePollen.Add("GRAM", "GRAMINEES")
            ListePollen.Add("GRAM", "GRAMINEE")
            ListePollen.Add("NOIS", "NOISETIER")
            ListePollen.Add("OLIV", "OLIVIER")
            ListePollen.Add("OSEI", "OSEILLE")
            ListePollen.Add("PARI", "PARIETAIRE")
            ListePollen.Add("PEUP", "PEUPLIER")
            ListePollen.Add("PLAN", "PLANTAIN")
            ListePollen.Add("PLAT", "PLATANE")
            ListePollen.Add("SAUL", "SAULE")
            ListePollen.Add("TILL", "TILLEUL")
            WriteLog("DBG: CodePollen, " & ListePollen.Count & " pollens chargés")
        Catch ex As Exception
            WriteLog("ERR: CodePollen, Exception : " & ex.Message)
        End Try
    End Sub
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
            _Refresh = value
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
                    If Value IsNot Nothing Then
                        If String.IsNullOrEmpty(Value) Or IsNumeric(Value) Or (Len(Value) - Len(Replace(Value, ":", "")) < 2) Then
                            retour = vbCrLf & "Veuillez saisir le type d'alerte, le departement en chiffre et la ville, le tout séparé par :" & vbCrLf & "ex : meteo:75:paris"
                        End If
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
        Try
            'récupération des paramétres avancés
            Try
                _DEBUG = _Parametres.Item(0).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: Pas de connection réseau, une connexion sera nécessaire : ")
            End If
            _IsConnect = True

            'charge les codes departement pour les alertes meteo
            CodeDeptFR()
            CodeDeptBE()
            ' GetMeteo(12, "")

        Catch ex As Exception
            _IsConnect = False
            WriteLog("ERR: Driver " & Me.Nom & " Erreur démarrage " & ex.Message)
        End Try
    End Sub

    ''' <summary>Arrêter le du driver</summary>
    ''' <remarks></remarks>
    Public Sub [Stop]() Implements HoMIDom.HoMIDom.IDriver.Stop
        Try
            _IsConnect = False
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

            'Si internet n'est pas disponible on ne mets pas à jour les informations
            If My.Computer.Network.IsAvailable = False Then
                WriteLog("ERR: Pas de connexion réseau, lecture des alertes impossible")
                Exit Sub
            End If

            Dim typealerte As String = ""
            Dim departement As String = ""
            Dim ville As String = ""
            Dim str As String = Objet.Adresse1

            typealerte = Trim(Mid(str, 1, InStr(str, ":") - 1))
            str = Trim(Mid(str, InStr(str, ":") + 1, Len(str)))
            departement = Trim(Mid(str, 1, InStr(str, ":") - 1))
            str = Trim(Mid(str, InStr(str, ":") + 1, Len(str)))
            ville = str

            Select Case UCase(typealerte)
                Case "POLLUTION"
                    Dim alrt As String = GetPollution(UCase(ville))
                    If alrt <> "" Then Objet.Value = alrt
                Case "POLLEN"
                    If Len(departement) = 1 Then departement = "0" + departement
                    Dim alrt As String = getpollen(departement, Objet.Adresse2)
                    If alrt <> "" Then Objet.Value = alrt
                Case "METEO"
                    Dim alrt As String = GetMeteo(departement, Objet.Adresse2)
                    If alrt <> "" Then Objet.Value = alrt
                Case "SANTE"
                    Dim alrt As String = Getsentinelle(departement, Objet.Adresse2)
                    If alrt <> "" Then Objet.Value = alrt
            End Select

        Catch ex As Exception
            WriteLog("ERR: Read, Exception : " & ex.Message)
        End Try
    End Sub

    ''' <summary>Commander un device</summary>
    ''' <param name="Objet">Objet représentant le device à interroger</param>
    ''' <param name="Command">La commande à passer</param>
    ''' <param name="Parametre1"></param>
    ''' <param name="Parametre2"></param>
    ''' <remarks></remarks>
    Public Sub Write(ByVal Objet As Object, ByVal Command As String, Optional ByVal Parametre1 As Object = Nothing, Optional ByVal Parametre2 As Object = Nothing) Implements HoMIDom.HoMIDom.IDriver.Write
        Try
            If _Enable = False Then Exit Sub

            If _IsConnect = False Then
                WriteLog("ERR: READ, Le driver n'est pas démarré, impossible d'écrire sur le port")
                Exit Sub
            End If

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
            _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Type d'alerte:Departement:ville", "Pollution:75:Paris", "")
            Add_LibelleDevice("ADRESSE2", "Nom alerte", "Nom de l'alerte si speciale ( crue, graminee, ... )", "")
            Add_LibelleDevice("REFRESH", "Refresh en sec", "Minimum 600, valeur rafraicissement station", "600")
            ' Libellés Device inutiles
            Add_LibelleDevice("SOLO", "@", "")
            Add_LibelleDevice("MODELE", "@", "")
            Add_LibelleDevice("LASTCHANGEDUREE", "@", "")
        Catch ex As Exception
            WriteLog("ERR: New, Exception : " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Fonctions internes"

    'Pollution
    ' https://www.lcsqa.org/maps-data-retriever.php?timestamp=1645114654&directory=json_episode_pollution&_=1645114654564

    Function GetPollution(ville As String) As String
        Try
            Dim doc As New XmlDocument
            Dim nodes As XmlNodeList
            Dim indice As String = ""
            Dim agglomeration As String = ""

            WriteLog("DBG: Recherche indice de pollution pour la ville => " & ville)

            doc = New XmlDocument()
            Dim url As New Uri("http://www.lcsqa.org/surveillance/indices/prevus/jour/xml/" & DateAndTime.Now.ToString("yyyy-MM-dd"))
            WriteLog("DBG: Chargement de http://www.lcsqa.org/surveillance/indices/prevus/jour/xml/" & DateAndTime.Now.ToString("yyyy-MM-dd"))
            Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)

            doc.Load(response.GetResponseStream)
            nodes = doc.SelectNodes("/root/node")
            For Each node As XmlNode In nodes
                indice = "0"
                agglomeration = ""
                For Each _child As XmlNode In node
                    Select Case _child.Name
                        Case "valeurIndice" : indice = _child.FirstChild.Value
                            WriteLog("DBG: ValeurIndice => " & indice)
                        Case "agglomeration" : agglomeration = _child.FirstChild.Value
                            WriteLog("DBG: Agglomeration => " & agglomeration)
                    End Select
                Next
                If (agglomeration = ville) Then
                    Exit For
                Else
                    indice = "0"
                End If
            Next

            Return indice

        Catch ex As Exception
            WriteLog("ERR: GetPollution, Exception : " & ex.Message)
            Return 0
        End Try
    End Function
    Function GetMeteo(departement As String, typeevent As String) As String

        Try
            Dim _lang As String = "fr-FR"
            Dim dept As String = ""

            If IsNumeric(departement) Then
                dept = ListeDepFR.Item(departement)
            Else
                departement = Trim(Mid(departement, 3, Len(departement)))
                dept = ListeDepBE.Item(departement)
            End If

            'dept = "seine maritime"
            WriteLog("DBG: GetMeteo, Departement demandé => " & dept)

            Dim responsebodystr As String = ""
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse

            Dim url = "https://hub.meteoalarm.org/api/v1/stream-buffers/all-warnings/warnings?startDate=" & Format(Now, "yyyy-MM-ddTHH:mm:ss") & ".000Z&endDate=" & Format(Now, "yyyy-MM-dd") & "T22:59:59.999Z&language=" & Mid(_lang, 1, InStr(_lang, "-") - 1) & "&include_geocodes=0&exclude_severity_minor=1"
            WriteLog("DBG: GetMeteo, url, " & url)
            request = CType(WebRequest.Create(url), HttpWebRequest)
            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            responsebodystr = HttpUtility.HtmlDecode(responsebodystr)
            ' WriteLog("DBG: GetMeteo, " & responsebodystr)
            Dim jsonObj As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)

            'prend les alertes du pays choisie
            Dim jalert As New List(Of JObject)
            For i = 0 To jsonObj.Item("warnings").Count - 1
                Dim tmp As String = ""
                If InStr(UCase(jsonObj.Item("warnings")(i)("alert")("identifier").ToString), UCase(Mid(_lang, 1, InStr(_lang, "-") - 1))) <> 0 Then
                    jalert.Add(jsonObj.Item("warnings")(i)("alert"))
                End If
                'For j = 0 To jsonObj.Item("warnings")(i)("alert")("info").Count - 1
                '    If InStr(jsonObj.Item("warnings")(i)("alert")("info")(j)("area")(0).ToString, "geocode") <> 0 Then
                '        If Not jsonObj.Item("warnings")(i)("alert")("info")(j)("area")(0)("geocode")(0).Contains("value") Then Continue For
                '        If InStr(UCase(jsonObj.Item("warnings")(i)("alert")("info")(j)("area")(0)("geocode")(0)("value").ToString), UCase(Mid(_lang, 1, InStr(_lang, "-") - 1))) <> 0 Then
                '            jalert.Add(jsonObj.Item("warnings")(i)("alert"))
                '        End If
                '    End If
                'Next
            Next
            'WriteLog("DBG: GetMeteo, jalert.count, " & jalert.Count)

            Dim jarea As JArray
            Dim nivAlert As String = ""
            For i = 0 To jalert.Count - 1
                '  WriteLog("DBG: GetMeteo, jalert(" & i & "), " & jalert.Item(i).ToString)
                For j = 0 To jalert.Item(i)("info").Count - 1
                    Dim datefin As DateTime = jalert.Item(i)("info")(j)("expires").ToString
                    ' WriteLog("DBG: GetMeteo, datefin, " & datefin.ToString)
                    If datefin > Now Then
                        If InStr(UCase(_lang), UCase(jalert.Item(i)("info")(j)("language").ToString)) <> 0 Then
                            jarea = jalert.Item(i)("info")(j)("area")
                            ' WriteLog("DBG: GetMeteo, jarea.alert(" & i & "), " & jalert.Item(i)("info").ToString)
                            For k = 0 To jarea.Count - 1
                                If (InStr(UCase(dept), UCase(jarea.Item(k)("areaDesc").ToString)) <> 0 And (InStr(nivAlert, jalert.Item(i)("info")(j)("event").ToString) = 0)) Then
                                    If nivAlert = "" Then
                                        nivAlert = jalert.Item(i)("info")(j)("event").ToString
                                    Else
                                        nivAlert = nivAlert & vbCrLf & jalert.Item(i)("info")(j)("event").ToString
                                    End If
                                End If
                            Next
                            jarea.Clear()
                        End If
                    End If
                Next
            Next

            If nivAlert = "" Then
                Return "Pas de vigilance"
            Else
                Return nivAlert
            End If

        Catch ex As Exception
            WriteLog("ERR: GetMeteo, Exception : " & ex.Message)
            Return "Donnée inconnue pour le département " & departement
        End Try
    End Function
    Private Function getpollen(departement As String, parampollen As String)
        'lien http
        'https://www.pollens.fr/risks/thea/counties/12
        Try
            Dim responsebodystr As String = ""
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse

            request = CType(WebRequest.Create("https://www.pollens.fr/risks/thea/counties/" & departement), HttpWebRequest)
            response = request.GetResponse()
            Dim responsereader = New StreamReader(response.GetResponseStream())
            responsebodystr = responsereader.ReadToEnd()
            responsereader.Close()
            response.Close()

            responsebodystr = HttpUtility.HtmlDecode(responsebodystr)
            WriteLog("DBG: GetMeteo, " & responsebodystr)
            Dim jsonObj As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responsebodystr)

            Dim listpollen As New List(Of String)
            While Len(parampollen) > 0
                If InStr(parampollen, ",") = 0 Then
                    listpollen.Add(parampollen)
                    parampollen = ""
                Else
                    listpollen.Add(Mid(parampollen, 1, InStr(parampollen, ",") - 1))
                    parampollen = Mid(parampollen, InStr(parampollen, ",") + 1, Len(parampollen))
                End If
                WriteLog("DBG: GetMeteo, parampollen : " & parampollen)
            End While

            Dim indicestr As String = ""
            Dim jpollen As String = ""
            For j = 0 To listpollen.Count - 1
                For i = 0 To jsonObj.Item("risks").Count - 1
                    jpollen = jsonObj.Item("risks")(i)("pollenName").ToString & " : " & jsonObj.Item("risks")(i)("level").ToString & " / 5"
                    ' WriteLog("DBG: GetMeteo, jregion : " & jpollen)
                    If InStr(UCase(jpollen), UCase(listpollen.Item(j))) <> 0 Then
                        If indicestr = "" Then
                            indicestr = jpollen
                        Else
                            indicestr = indicestr & vbCrLf & jpollen
                        End If
                    End If
                Next
            Next

            Return indicestr
        Catch ex As Exception
            WriteLog("ERR: GetPollen, Exception : " & ex.Message)
            Return ""
        End Try
    End Function

    Private Function Getsentinelle(departement As String, typeall As String)
        Try
            Return ""
        Catch ex As Exception
            WriteLog("ERR: GetSentinelle, Exception : " & ex.Message)
            Return ""
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
