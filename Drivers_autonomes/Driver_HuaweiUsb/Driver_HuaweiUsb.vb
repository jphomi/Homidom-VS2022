Imports HoMIDom.HoMIDom.Server
Imports HoMIDom.HoMIDom.Device

Imports System.Net
Imports STRGS = Microsoft.VisualBasic.Strings
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Security.Cryptography
Imports Newtonsoft.Json.JsonSerializer


<Serializable()> Public Class Driver_HuaweiUsb
    Implements HoMIDom.HoMIDom.IDriver

#Region "Variables génériques"
    '!!!Attention les variables ci-dessous doivent avoir une valeur par défaut obligatoirement
    'aller sur l'adresse http://www.somacon.com/p113.php pour avoir un ID
    Dim _ID As String = "81E9ADDC-DCDC-11EA-8BCB-F916FC2CA371" 'ne pas modifier car utilisé dans le code du serveur
    Dim _Nom As String = "HuaweiUsb" 'Nom du driver à afficher
    Dim _Enable As Boolean = False 'Activer/Désactiver le driver
    Dim _Description As String = "Driver HuaweiUsb" 'Description du driver
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


    ' differerente url
    ' /api/webserver/SesTokInfo -> nouveau token
    ' /api/monitoring/status -> status du modem
    ' /api/api/device/basic_information -> info
    ' api/user/login -> login user
    ' api/sms/send-sms -> envoi sms
    ' api/sms/sms-count -> nbre sms
    ' api/sms/sms-list -> liste des sms
    ' api/sms/delete-sms -> suppresion sms
    ' api/sms/set-read -> marquer sms comme lu

    'boxtype = 1 : Elements recus
    'boxtype = 2 : Elements envoyés

    Dim _IPAdressHuaweiUsb As String = "192.168.8.1"
    Dim _IPPortHuaweiUsb As String = "80"
    Dim _UrlHuaweiUsb As String = ""
    Dim _UserHuaweiUsb As String = "Huawei"
    Dim _PasswordHuaweiUsb As String = "password"
    Dim _DelSmsRead As Boolean = True
    Dim _NbSmsSendNoDeleted As Integer = 100
    Dim _SessionID As String = ""
    Dim _TokenID As String = ""
    Dim _DateDernierSms As String = ""
    '    Dim _NouveauSms As Boolean = False

#End Region

#Region "Variables Internes"
    'Insérer ici les variables internes propres au driver et non communes

    Public StatutModem As New ModemStatut
    Public ListeSms As New SmsList
    Public Class ModemStatut
        Public DeviceName As String
        Public SoftwareVersion As String
        Public ConnectionStatut As String
        Public TypeReseau As String
        Public SignalIcon As String
    End Class

    Public Class SmsList
        Public Messages As New List(Of Message)
        '     Public UnRead As Integer
    End Class

    Public Class Message
        Public Smstat As Integer
        Public Index As Integer
        Public Phone As String
        Public Content As String
        Public DateReception As String
        Public Sca As String
        Public Priority As Integer
        Public SaveType As Integer
        Public SmsType As Integer
    End Class


    Public Enum CodeError As Integer
        ERROR_NO_DEVICE = -2
        ERROR_DEFAULT = -1
        ERROR_FIRST_SEND = 1
        ERROR_UNKNOWN = 100001
        ERROR_NOT_SUPPORT = 100002
        ERROR_NO_RIGHT = 100003
        ERROR_BUSY = 100004
        ERROR_FORMAT_ERROR = 100005
        ERROR_PARAMETER_ERROR = 100006
        ERROR_SAVE_CONFIG_FILE_ERROR = 100007
        ERROR_GET_CONFIG_FILE_ERROR = 100008
        ERROR_NO_SIM_CARD_OR_INVALID_SIM_CARD = 101001
        ERROR_CHECK_SIM_CARD_PIN_LOCK = 101002
        ERROR_CHECK_SIM_CARD_PUN_LOCK = 101003
        ERROR_CHECK_SIM_CARD_CAN_UNUSEABLE = 101004
        ERROR_ENABLE_PIN_FAILED = 101005
        ERROR_DISABLE_PIN_FAILED = 101006
        ERROR_UNLOCK_PIN_FAILED = 101007
        ERROR_DISABLE_AUTO_PIN_FAILED = 101008
        ERROR_ENABLE_AUTO_PIN_FAILED = 101009
        ERROR_GET_NET_TYPE_FAILED = 102001
        ERROR_GET_SERVICE_STATUS_FAILED = 102002
        ERROR_GET_ROAM_STATUS_FAILED = 102003
        ERROR_GET_CONNECT_STATUS_FAILED = 102004
        ERROR_DEVICE_AT_EXECUTE_FAILED = 103001
        ERROR_DEVICE_PIN_VALIDATE_FAILED = 103002
        ERROR_DEVICE_PIN_MODIFFY_FAILED = 103003
        ERROR_DEVICE_PUK_MODIFFY_FAILED = 103004
        ERROR_DEVICE_GET_AUTORUN_VERSION_FAILED = 103005
        ERROR_DEVICE_GET_API_VERSION_FAILED = 103006
        ERROR_DEVICE_GET_PRODUCT_INFORMATON_FAILED = 103007
        ERROR_DEVICE_SIM_CARD_BUSY = 103008
        ERROR_DEVICE_SIM_LOCK_INPUT_ERROR = 103009
        ERROR_DEVICE_NOT_SUPPORT_REMOTE_OPERATE = 103010
        ERROR_DEVICE_PUK_DEAD_LOCK = 103011
        ERROR_DEVICE_GET_PC_AISSST_INFORMATION_FAILED = 103012
        ERROR_DEVICE_SET_LOG_INFORMATON_LEVEL_FAILED = 103013
        ERROR_DEVICE_GET_LOG_INFORMATON_LEVEL_FAILED = 103014
        ERROR_DEVICE_COMPRESS_LOG_FILE_FAILED = 103015
        ERROR_DEVICE_RESTORE_FILE_DECRYPT_FAILED = 103016
        ERROR_DEVICE_RESTORE_FILE_VERSION_MATCH_FAILED = 103017
        ERROR_DEVICE_RESTORE_FILE_FAILED = 103018
        ERROR_DEVICE_SET_TIME_FAILED = 103101
        ERROR_COMPRESS_LOG_FILE_FAILED = 103102
        ERROR_DHCP_ERROR = 104001
        ERROR_SAFE_ERROR = 106001
        ERROR_DIALUP_GET_CONNECT_FILE_ERROR = 107720
        ERROR_DIALUP_SET_CONNECT_FILE_ERROR = 107721
        ERROR_DIALUP_DIALUP_MANAGMENT_PARSE_ERROR = 107722
        ERROR_DIALUP_ADD_PRORILE_ERROR = 107724
        ERROR_DIALUP_MODIFY_PRORILE_ERROR = 107725
        ERROR_DIALUP_SET_DEFAULT_PRORILE_ERROR = 107726
        ERROR_DIALUP_GET_PRORILE_LIST_ERROR = 107727
        ERROR_DIALUP_GET_AUTO_APN_MATCH_ERROR = 107728
        ERROR_DIALUP_SET_AUTO_APN_MATCH_ERROR = 107729
        ERROR_LOGIN_NO_EXIST_USER = 108001
        ERROR_LOGIN_PASSWORD_ERROR = 108002
        ERROR_LOGIN_ALREADY_LOGINED = 108003
        ERROR_LOGIN_MODIFY_PASSWORD_FAILED = 108004
        ERROR_LOGIN_TOO_MANY_USERS_LOGINED = 108005
        ERROR_LOGIN_USERNAME_OR_PASSWORD_ERROR = 108006
        ERROR_LOGIN_TOO_MANY_TIMES = 108007
        ERROR_LANGUAGE_GET_FAILED = 109001
        ERROR_LANGUAGE_SET_FAILED = 109002
        ERROR_ONLINE_UPDATE_SERVER_NOT_ACCESSED = 110001
        ERROR_ONLINE_UPDATE_ALREADY_BOOTED = 110002
        ERROR_ONLINE_UPDATE_GET_DEVICE_INFORMATION_FAILED = 110003
        ERROR_ONLINE_UPDATE_GET_LOCAL_GROUP_COMMPONENT_INFORMATION_FAILED = 110004
        ERROR_ONLINE_UPDATE_NOT_FIND_FILE_ON_SERVER = 110005
        ERROR_ONLINE_UPDATE_NEED_RECONNECT_SERVER = 110006
        ERROR_ONLINE_UPDATE_CANCEL_DOWNLODING = 110007
        ERROR_ONLINE_UPDATE_SAME_FILE_LIST = 110008
        ERROR_ONLINE_UPDATE_CONNECT_ERROR = 110009
        ERROR_ONLINE_UPDATE_INVALID_URL_LIST = 110021
        ERROR_ONLINE_UPDATE_NOT_SUPPORT_URL_LIST = 110022
        ERROR_ONLINE_UPDATE_NOT_BOOT = 110023
        ERROR_ONLINE_UPDATE_LOW_BATTERY = 110024
        ERROR_USSD_ERROR = 111001
        ERROR_USSD_FUCNTION_RETURN_ERROR = 111012
        ERROR_USSD_IN_USSD_SESSION = 111013
        ERROR_USSD_TOO_LONG_CONTENT = 111014
        ERROR_USSD_EMPTY_COMMAND = 111016
        ERROR_USSD_CODING_ERROR = 111017
        ERROR_USSD_AT_SEND_FAILED = 111018
        ERROR_USSD_NET_NO_RETURN = 111019
        ERROR_USSD_NET_OVERTIME = 111020
        ERROR_USSD_XML_SPECIAL_CHARACTER_TRANSFER_FAILED = 111021
        ERROR_USSD_NET_NOT_SUPPORT_USSD = 111022
        ERROR_SET_NET_MODE_AND_BAND_WHEN_DAILUP_FAILED = 112001
        ERROR_SET_NET_SEARCH_MODE_WHEN_DAILUP_FAILED = 112002
        ERROR_SET_NET_MODE_AND_BAND_FAILED = 112003
        ERROR_SET_NET_SEARCH_MODE_FAILED = 112004
        ERROR_NET_REGISTER_NET_FAILED = 112005
        ERROR_NET_NET_CONNECTED_ORDER_NOT_MATCH = 112006
        ERROR_NET_CURRENT_NET_MODE_NOT_SUPPORT = 112007
        ERROR_NET_SIM_CARD_NOT_READY_STATUS = 112008
        ERROR_NET_MEMORY_ALLOC_FAILED = 112009
        ERROR_SMS_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 113017
        ERROR_SMS_OVERTIME = 113018
        ERROR_SMS_QUERY_SMS_INDEX_LIST_ERROR = 113020
        ERROR_SMS_SET_SMS_CENTER_NUMBER_FAILED = 113031
        ERROR_SMS_DELETE_SMS_FAILED = 113036
        ERROR_SMS_SAVE_CONFIG_FILE_FAILED = 113047
        ERROR_SMS_LOCAL_SPACE_NOT_ENOUGH = 113053
        ERROR_SMS_TELEPHONE_NUMBER_TOO_LONG = 113054
        ERROR_SD_FILE_EXIST = 114001
        ERROR_SD_DIRECTORY_EXIST = 114002
        ERROR_SD_FILE_OR_DIRECTORY_NOT_EXIST = 114004
        ERROR_SD_IS_OPERTED_BY_OTHER_USER = 114004
        ERROR_SD_FILE_NAME_TOO_LONG = 114005
        ERROR_SD_NO_RIGHT = 114006
        ERROR_SD_FILE_IS_UPLOADING = 114007
        ERROR_PB_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 115001
        ERROR_PB_OVERTIME = 115002
        ERROR_PB_CALL_SYSTEM_FUCNTION_ERROR = 115003
        ERROR_PB_WRITE_FILE_ERROR = 115004
        ERROR_PB_READ_FILE_ERROR = 115005
        ERROR_PB_LOCAL_TELEPHONE_FULL_ERROR = 115199
        ERROR_STK_NULL_ARGUMENT_OR_ILLEGAL_ARGUMENT = 116001
        ERROR_STK_OVERTIME = 116002
        ERROR_STK_CALL_SYSTEM_FUCNTION_ERROR = 116003
        ERROR_STK_WRITE_FILE_ERROR = 116004
        ERROR_STK_READ_FILE_ERROR = 116005
        ERROR_WIFI_STATION_CONNECT_AP_PASSWORD_ERROR = 117001
        ERROR_WIFI_WEB_PASSWORD_OR_DHCP_OVERTIME_ERROR = 117002
        ERROR_WIFI_PBC_CONNECT_FAILED = 117003
        ERROR_WIFI_STATION_CONNECT_AP_WISPR_PASSWORD_ERROR = 117004
        ERROR_CRADLE_GET_CRURRENT_CONNECTED_USER_IP_FAILED = 118001
        ERROR_CRADLE_GET_CRURRENT_CONNECTED_USER_MAC_FAILED = 118002
        ERROR_CRADLE_SET_MAC_FAILED = 118003
        ERROR_CRADLE_GET_WAN_INFORMATION_FAILED = 118004
        ERROR_CRADLE_CODING_FAILED = 118005
        ERROR_CRADLE_UPDATE_PROFILE_FAILED = 118006

    End Enum


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
                Exit Function
            End If

            If MyDevice IsNot Nothing Then
                If Command = "" Then
                    Return False
                Else
                    'mise a jour de la configuration
                    WriteLog("DBG: ExecuteCommand, Passage de la commande " & UCase(Command) & " Param(0) -> " & Param(0) & " / Param(1) -> " & Param(1))

                    ' Traitement de la commande 
                    Select Case UCase(Command)
                        Case "SENDSMS"
                            If InStr(Param(0), "#") > 0 Then
                                Dim param0 As String = Param(0)
                                Param(0) = Mid(param0, 1, InStr(param0, "#") - 1)
                                Param(1) = Mid(param0, InStr(param0, "#") + 1, Len(param0))
                            Else
                                Param(1) = "Test sms"
                            End If
                            Return SendSMS(Param(0), Param(1))
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
                            If String.IsNullOrEmpty(Value) Or IsNumeric(Value) = False Or Len(Value) < 10 Then
                                retour = "Veuillez renseigner un numéro de téléphone"
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
                _IPAdressHuaweiUsb = _Parametres.Item(1).Valeur
                _IPPortHuaweiUsb = _Parametres.Item(2).Valeur
                _UserHuaweiUsb = _Parametres.Item(3).Valeur
                _PasswordHuaweiUsb = _Parametres.Item(4).Valeur
                _DelSmsRead = _Parametres.Item(5).Valeur
                _NbSmsSendNoDeleted = _Parametres.Item(6).Valeur
            Catch ex As Exception
                _DEBUG = False
                _Parametres.Item(0).Valeur = False
                WriteLog("ERR: Erreur dans les paramétres avancés. utilisation des valeur par défaut : " & ex.Message)
            End Try

            _UrlHuaweiUsb = "http://" & _IPAdressHuaweiUsb & ":" & _IPPortHuaweiUsb
            WriteLog("Start, connection au serveur " & _UrlHuaweiUsb)

            If GetConfig(_UserHuaweiUsb, _PasswordHuaweiUsb) Then
                _IsConnect = True
                WriteLog("Driver démarré avec succés à l'adresse " & _UrlHuaweiUsb)
                WriteLog("Modem " & StatutModem.DeviceName & " , softversion " & StatutModem.SoftwareVersion)

                'lance le time du driver, mini toutes les 1 minutes
                If _Refresh = 0 Then _Refresh = 60
                MyTimer.Interval = _Refresh * 1000
                MyTimer.Enabled = True
                AddHandler MyTimer.Elapsed, AddressOf TimerTick
            Else
                _IsConnect = False
                WriteLog("ERR: Start, Erreur démarrage")
                Exit Sub
            End If

            GetInbox()

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

            Select Case Objet.Type
                Case "GENERIQUESTRING"
                    WriteLog("DBG: READ, Recherche des SMS non lus du numéro " & Objet.adresse1)
                    For i = 0 To ListeSms.Messages.Count - 1
                        If ListeSms.Messages.Item(i).Smstat = 0 Then
                            If InStr(ListeSms.Messages.Item(i).Phone, Objet.adresse1) > 0 Then
                                WriteLog("DBG: READ, SMS non lu trouvé à l'index " & ListeSms.Messages.Item(i).Index)
                                If (Objet.adresse2 = "") Or (InStr(ListeSms.Messages.Item(i).Content, Objet.adresse2) > 0) Then
                                    Objet.value = ListeSms.Messages.Item(i).Content
                                End If
                                If _DelSmsRead Then
                                        DeleteSms(ListeSms.Messages.Item(i).Index)
                                    Else
                                        SetReadSms(ListeSms.Messages.Item(i).Index)
                                    End If
                                End If
                            End If
                    Next
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
                _DeviceSupport.Add(ListeDevices.GENERIQUESTRING)

            'Parametres avancés
            Add_ParamAvance("Debug", "Activer le Debug complet (True/False)", False)
            Add_ParamAvance("IPAdress", "Adresse IP", "192.168.8.1")
            Add_ParamAvance("IPPort", "Port IP", "80")
            Add_ParamAvance("User", "User", "admin")
            Add_ParamAvance("Password", "Password", "huawei")
            Add_ParamAvance("DelSmsRead", "Suppression sms aprés lecture", True)
            Add_ParamAvance("NbSmsSendNoDeleted", "Nbre de sms restant dans les éléments envoyés", 100)

            'ajout des commandes avancées pour les devices
            'add_devicecommande("COMMANDE", "DESCRIPTION", nbparametre)
            Add_DeviceCommande("SENDSMS", "Parametres d'envoi Numero#texte", 1)

            'Libellé Driver
            Add_LibelleDriver("HELP", "Aide...", "Pas d'aide actuellement...")

            'Libellé Device
            Add_LibelleDevice("ADRESSE1", "Numéro téléphone", "Numéro téléphone sous la forme +XX123456789")
            Add_LibelleDevice("ADRESSE2", "Chaine recherchée", "")
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
        If (_Enable = False) Or (_IsConnect = False) Then
            'arrete le timer du driver
            MyTimer.Enabled = False
            Exit Sub
        End If

        _DEBUG = _Parametres.Item(0).Valeur
        GetInbox()
        'vide elements envoyés
        ClearOutbox(_NbSmsSendNoDeleted)
        'vide brouillon
        ClearDraft(_NbSmsSendNoDeleted)


    End Sub

#End Region

#Region "Fonctions internes"
    'Insérer ci-dessous les fonctions propres au driver et nom communes (ex: start)
    Function GetToken() As Boolean
            'retourne le token

            Try
                Dim str As String = ""
                Dim webclient As New WebClient

                Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(_UrlHuaweiUsb & "/api/webserver/SesTokInfo"))
                reader.WhitespaceHandling = WhitespaceHandling.Significant
                While reader.Read()
                    str = reader.ReadString
                    '                WriteLog("DBG: GET_Token : " & reader.Name & " -> " & str)
                    Select Case reader.Name
                        Case "SesInfo"
                            _SessionID = str
                        Case "TokInfo"
                            _TokenID = str
                    End Select
                End While
                '            WriteLog("DBG: GetToken : SesInfo = " & _SessionID & " , TokInfo = " & _TokenID)
                Return True
            Catch ex As Exception
                WriteLog("ERR: GetToken, " & ex.Message)
                Return False
            End Try
        End Function

        Private Function GetConfig(user As String, password As String) As Boolean
        ' recupere les configurations des equipements et scenarios de Huawei

        Try
                Dim str As String = ""

                If Not GetToken() Then Return False

                Dim webclient As New WebClient
                webclient.Headers.Clear()
                webclient.Headers.Add("Cookie", _SessionID)
                webclient.Headers.Add("__RequestVerificationToken", _TokenID)

                Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(_UrlHuaweiUsb & "/api/device/basic_information"))
                reader.WhitespaceHandling = WhitespaceHandling.Significant
                While reader.Read()
                    str = reader.ReadString
                    Select Case reader.Name
                        Case "devicename"
                            StatutModem.DeviceName = str
                        Case "SoftwareVersion"
                            StatutModem.SoftwareVersion = str
                    End Select
                End While

                Return True
            Catch ex As Exception

                WriteLog("ERR: GETConfig, " & ex.Message)
                WriteLog("ERR: GETConfig, Url: " & _UrlHuaweiUsb)
                Return False
            End Try
        End Function

        Private Function CheckNotifications() As Integer

        Try
                Dim str As String = ""

                If Not GetToken() Then Return False

                Dim webclient As New WebClient
                webclient.Headers.Clear()
                webclient.Headers.Add("Cookie", _SessionID)
                webclient.Headers.Add("__RequestVerificationToken", _TokenID)

                Dim reader As XmlTextReader = New XmlTextReader(webclient.OpenRead(_UrlHuaweiUsb & "/api/monitoring/check-notifications"))
                reader.WhitespaceHandling = WhitespaceHandling.Significant
                While reader.Read()
                    str = reader.ReadString
                    Select Case reader.Name
                        Case "UnreadMessage"
                            Return str
                    End Select
                End While
                Return vbNull
            Catch ex As Exception

                WriteLog("ERR: GETConfig, " & ex.Message)
                WriteLog("ERR: GETConfig, Url: " & _UrlHuaweiUsb)
                Return vbNull
            End Try
        End Function
        Function LoginUser(user As String, password As String) As Boolean

            Try

                GetToken()

                Dim responsebodystr As String = ""
                Dim request As HttpWebRequest
                Dim response As HttpWebResponse
                Dim qUrl As String = _UrlHuaweiUsb & "/api/user/login"
                '    WriteLog("DBG: LoginUser, qurl -> " & qUrl)

                Dim st = b64_sha256(password)
                st = b64_sha256(user & st & _TokenID)

                Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><Username>" & user & "</Username><password_type>4</password_type><Password>" & st & "</Password></request>"

                request = CType(WebRequest.Create(qUrl), HttpWebRequest)
                request.Method = "POST"
                request.Headers.Add("Cookie", _SessionID)
                request.Headers.Add("__RequestVerificationToken", _TokenID)
                request.Headers.Add("X-Requested-With", "XMLHttpRequest")

                Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
                request.ContentLength = postBytes.Length

                Dim writer As New StreamWriter(request.GetRequestStream)
                writer.Write(reqparam)
                writer.Close()

                response = request.GetResponse()

                '  WriteLog("DBG: LoginUser, response status -> " & response.StatusCode & " / " & response.StatusDescription)
                If response.StatusCode <> 200 Then
                    WriteLog("ERR: LoginUser, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                    Return False
                End If

                Dim responsereader = New StreamReader(response.GetResponseStream())
                responsebodystr = responsereader.ReadToEnd()
                responsereader.Close()
                response.Close()

                _SessionID = response.Headers.Get("Set-Cookie")
                _TokenID = response.Headers.Get("__RequestVerificationToken")

                '    WriteLog("DBG: LoginUser, utilisateur connecté")
                Return True
            Catch ex As Exception
                WriteLog("ERR: LoginUser, " & ex.Message)
                Return False
            End Try
        End Function

    Function GetInbox() As Integer
        'Nombre de sms non lu

        Try
            If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return 0

            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/sms-list"
            WriteLog("DBG: GetInbox, qurl -> " & qUrl)
            Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><PageIndex>1</PageIndex><ReadCount>20</ReadCount><BoxType>1</BoxType><SortType>0</SortType><Ascending>0</Ascending><UnreadPreferred>0</UnreadPreferred></request>"

            request = CType(WebRequest.Create(qUrl), HttpWebRequest)
            request.Method = "POST"
            request.Headers.Add("Cookie", _SessionID)
            request.Headers.Add("__RequestVerificationToken", _TokenID)
            request.Headers.Add("X-Requested-With", "XMLHttpRequest")

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length

            Dim writer As New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            response = request.GetResponse()
            If response.StatusCode <> 200 Then
                WriteLog("ERR: GetInbox, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                Return 0
            End If

            Dim doc As New XmlDocument
            doc = New XmlDocument()
            doc.Load(response.GetResponseStream)

            response.Close()

            ListeSms.Messages.Clear()
            Dim UnRead As Integer = 0

            Dim nodesMsge As XmlNodeList
            nodesMsge = doc.SelectNodes("/response/Messages/Message")
            For Each nodemsg As XmlNode In nodesMsge
                Dim msg As New Message
                For Each _childMsg As XmlNode In nodemsg
                    If String.IsNullOrEmpty(_childMsg.InnerText) Then Continue For
                    Select Case _childMsg.Name
                        Case "Smstat" : msg.Smstat = _childMsg.FirstChild.Value  ' 0 = non lu
                        Case "Index" : msg.Index = _childMsg.FirstChild.Value
                        Case "Phone" : msg.Phone = _childMsg.FirstChild.Value
                        Case "Content" : msg.Content = _childMsg.FirstChild.Value
                        Case "Date" : msg.DateReception = _childMsg.FirstChild.Value
                        Case "Sca" : msg.Sca = _childMsg.FirstChild.Value
                        Case "SaveType" : msg.SaveType = _childMsg.FirstChild.Value
                        Case "Priority" : msg.Priority = _childMsg.FirstChild.Value
                        Case "SmsType" : msg.SmsType = _childMsg.FirstChild.Value
                    End Select
                Next
                If msg.Smstat = 0 Then UnRead += 1
                ListeSms.Messages.Add(msg)
            Next

            For i = 0 To ListeSms.Messages.Count - 1
                If _DateDernierSms = "" Then _DateDernierSms = ListeSms.Messages.Item(i).DateReception
                If DateDiff("s", DateTime.ParseExact(_DateDernierSms, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture), DateTime.ParseExact(ListeSms.Messages.Item(i).DateReception, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)) > 0 Then
                    _DateDernierSms = ListeSms.Messages.Item(i).DateReception
                End If
            Next

            WriteLog("DBG: GetInbox, Nbre sms non lu -> " & UnRead & " / " & ListeSms.Messages.Count)

            Return ListeSms.Messages.Count
        Catch ex As Exception
            WriteLog("ERR: GetInbox, " & ex.Message)
            Return vbNull
        End Try
    End Function

    Function ClearOutbox(nbsms As Integer) As Integer
        'Suppression de x sms dans les elements envoyés

        Try
            If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return 0

            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/sms-list"
            WriteLog("DBG: ClearOutbox, qurl -> " & qUrl)
            Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><PageIndex>" & Int(nbsms / 20) + 1 & "</PageIndex><ReadCount>20</ReadCount><BoxType>2</BoxType><SortType>0</SortType><Ascending>0</Ascending><UnreadPreferred>0</UnreadPreferred></request>"

            request = CType(WebRequest.Create(qUrl), HttpWebRequest)
            request.Method = "POST"
            request.Headers.Add("Cookie", _SessionID)
            request.Headers.Add("__RequestVerificationToken", _TokenID)
            request.Headers.Add("X-Requested-With", "XMLHttpRequest")

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length

            Dim writer As New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            response = request.GetResponse()
            If response.StatusCode <> 200 Then
                WriteLog("ERR: ClearOutbox, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                Return 0
            End If

            Dim doc As New XmlDocument
            doc = New XmlDocument()
            doc.Load(response.GetResponseStream)

            response.Close()

            Dim messdelete As New SmsList
            Dim UnRead As Integer = 0

            Dim nodesMsge As XmlNodeList
            nodesMsge = doc.SelectNodes("/response/Messages/Message")
            For Each nodemsg As XmlNode In nodesMsge
                Dim msg As New Message
                For Each _childMsg As XmlNode In nodemsg
                    If String.IsNullOrEmpty(_childMsg.InnerText) Then Continue For
                    Select Case _childMsg.Name
                        Case "Index" : msg.Index = _childMsg.FirstChild.Value
                    End Select
                Next
                If msg.Smstat = 0 Then UnRead += 1
                messdelete.Messages.Add(msg)
            Next

            For i = 0 To messdelete.Messages.Count - 1
                DeleteSms(messdelete.Messages.Item(i).Index)
            Next

            WriteLog("DBG: ClearOutbox, Nbre sms supprimés -> " & messdelete.Messages.Count)
            Return messdelete.Messages.Count
        Catch ex As Exception
            WriteLog("ERR: ClearOutbox, " & ex.Message)
            Return vbNull
        End Try
    End Function

    Function ClearDraft(nbsms As Integer) As Integer
        'Suppression de x sms dans les brouillons

        Try
            If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return 0

            Dim request As HttpWebRequest
            Dim response As HttpWebResponse
            Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/sms-list"
            WriteLog("DBG: ClearDraft, qurl -> " & qUrl)
            Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><PageIndex>" & Int(nbsms / 20) + 1 & "</PageIndex><ReadCount>20</ReadCount><BoxType>3</BoxType><SortType>0</SortType><Ascending>0</Ascending><UnreadPreferred>0</UnreadPreferred></request>"

            request = CType(WebRequest.Create(qUrl), HttpWebRequest)
            request.Method = "POST"
            request.Headers.Add("Cookie", _SessionID)
            request.Headers.Add("__RequestVerificationToken", _TokenID)
            request.Headers.Add("X-Requested-With", "XMLHttpRequest")

            Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
            request.ContentLength = postBytes.Length

            Dim writer As New StreamWriter(request.GetRequestStream)
            writer.Write(reqparam)
            writer.Close()

            response = request.GetResponse()
            If response.StatusCode <> 200 Then
                WriteLog("ERR: ClearDraft, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                Return 0
            End If

            Dim doc As New XmlDocument
            doc = New XmlDocument()
            doc.Load(response.GetResponseStream)

            response.Close()

            Dim messdelete As New SmsList
            Dim UnRead As Integer = 0

            Dim nodesMsge As XmlNodeList
            nodesMsge = doc.SelectNodes("/response/Messages/Message")
            For Each nodemsg As XmlNode In nodesMsge
                Dim msg As New Message
                For Each _childMsg As XmlNode In nodemsg
                    If String.IsNullOrEmpty(_childMsg.InnerText) Then Continue For
                    Select Case _childMsg.Name
                        Case "Index" : msg.Index = _childMsg.FirstChild.Value
                    End Select
                Next
                If msg.Smstat = 0 Then UnRead += 1
                messdelete.Messages.Add(msg)
            Next

            For i = 0 To messdelete.Messages.Count - 1
                DeleteSms(messdelete.Messages.Item(i).Index)
            Next

            WriteLog("DBG: ClearDraft, Nbre sms supprimés -> " & messdelete.Messages.Count)
            Return messdelete.Messages.Count
        Catch ex As Exception
            WriteLog("ERR: ClearDraft, " & ex.Message)
            Return vbNull
        End Try
    End Function

    Function SendSMS(phones As String, messag As String) As Boolean

            Try
            ' SendSMS(_UrlHuaweiUsb, "0123456789", "Youpi" & vbCrLf & "ca marche")

            If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return False

            Dim response As HttpWebResponse
            Dim reqparam As String = ""
            Dim postBytes As Byte()
            Dim writer As StreamWriter
            Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/send-sms"
            messag = messag.Replace("\r\n", vbCrLf) 'permet les sauts de ligne en ecrivant \r\n dans le message
            WriteLog("DBG: SendSMS, qurl -> " & qUrl)

            Dim phonetmp As String = ""
            Dim request As HttpWebRequest

            While phones <> ""
                If InStr(phones, ";") > 0 Then
                    phonetmp = phonetmp & "<Phone>" & Mid(phones, 1, InStr(phones, ";") - 1) & "</Phone>"
                    phones = phones.Replace(Mid(phones, 1, InStr(phones, ";")), "")
                Else
                    phonetmp = "<Phone>" & phones & "</Phone>"
                    phones = ""
                End If

                reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><Index>-1</Index><Phones>" & phonetmp & "</Phones><Sca></Sca><Content>" & messag & "</Content><Length>" & Len(messag) & "</Length><Reserved>1</Reserved><Date>" & DateTime.Parse(Now).ToString("yyyy-MM-dd HH:mm:ss") & "</Date><SendType>0</SendType></request>"
                WriteLog("DBG: SendSMS, reqparam -> " & reqparam)

                request = CType(WebRequest.Create(qUrl), HttpWebRequest)
                request.Method = "POST"
                request.Headers.Clear()
                request.Headers.Add("Cookie", _SessionID)
                request.Headers.Add("__RequestVerificationToken", _TokenID)
                request.Headers.Add("X-Requested-With", "XMLHttpRequest")

                postBytes = Encoding.UTF8.GetBytes(reqparam)
                request.ContentLength = postBytes.Length

                writer = New StreamWriter(request.GetRequestStream)
                writer.Write(reqparam)
                writer.Close()

                response = request.GetResponse()
                WriteLog("DBG: SendSMS, response status -> " & phonetmp & " : " & response.StatusCode & " / " & response.StatusDescription)
                If response.StatusCode <> 200 Then
                    WriteLog("ERR: SendSMS, response Error -> " & phonetmp & " : " & response.StatusCode & " / " & response.StatusDescription)
                    Return False
                End If
                response.Close()
            End While

            Return True

            Catch ex As Exception
                WriteLog("ERR: SendSMS, " & ex.Message)
                Return False
            End Try
        End Function

        Function SetReadSms(index As Integer) As Boolean
            'Nombre de sms non lu

            Try
                If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return False

                Dim request As HttpWebRequest
                Dim response As HttpWebResponse
                Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/set-read"
                '      WriteLog("DBG: SetReadSms, qurl -> " & qUrl)
                Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><Index>" & index & "</Index></request>"
                '     WriteLog("DBG: SetReadSms, reqparam -> " & reqparam)

                request = CType(WebRequest.Create(qUrl), HttpWebRequest)
                request.Method = "POST"
                request.Headers.Add("Cookie", _SessionID)
                request.Headers.Add("__RequestVerificationToken", _TokenID)
                request.Headers.Add("X-Requested-With", "XMLHttpRequest")

                Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
                request.ContentLength = postBytes.Length

                Dim writer As New StreamWriter(request.GetRequestStream)
                writer.Write(reqparam)
                writer.Close()

                response = request.GetResponse()
                WriteLog("DBG: SetReadSms, response status -> " & response.StatusCode & " / " & response.StatusDescription)
                If response.StatusCode <> 200 Then
                    WriteLog("ERR: SetReadSms, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                    Return False
                End If

                Return True
            Catch ex As Exception
                WriteLog("ERR: SetReadSms, " & ex.Message)
                Return False
            End Try
        End Function

        Function DeleteSms(index As Integer) As Boolean
            'Nombre de sms non lu

            Try
                If Not LoginUser(_UserHuaweiUsb, _PasswordHuaweiUsb) Then Return False

                Dim responsebodystr As String = ""
                Dim request As HttpWebRequest
                Dim response As HttpWebResponse
                Dim qUrl As String = _UrlHuaweiUsb & "/api/sms/delete-sms"
                WriteLog("DBG: DeleteSms, qurl -> " & qUrl)
                Dim reqparam = "<?xml version=""1.0"" encoding=""UTF-8""?><request><Index>" & index & "</Index></request>"

                request = CType(WebRequest.Create(qUrl), HttpWebRequest)
                request.Method = "POST"
                request.Headers.Add("Cookie", _SessionID)
                request.Headers.Add("__RequestVerificationToken", _TokenID)
                request.Headers.Add("X-Requested-With", "XMLHttpRequest")

                Dim postBytes As Byte() = Encoding.UTF8.GetBytes(reqparam)
                request.ContentLength = postBytes.Length

                Dim writer As New StreamWriter(request.GetRequestStream)
                writer.Write(reqparam)
                writer.Close()

                response = request.GetResponse()
                WriteLog("DBG: DeleteSms, response status -> " & response.StatusCode & " / " & response.StatusDescription)
                If response.StatusCode <> 200 Then
                    WriteLog("ERR: DeleteSms, response Error -> " & response.StatusCode & " / " & response.StatusDescription)
                    Return False
                End If
                response.Close()

                WriteLog("DBG: DeleteSms, index -> " & index)

                Return True
            Catch ex As Exception
                WriteLog("ERR: DeleteSms, " & ex.Message)
                Return vbNull
            End Try
        End Function

        Function b64_sha256(data As String) As String
            Try
                'chaine de test
                'data = "Hello World!" 
                'sha256 -> 7F83B1657FF1FC53B92DC18148A1D65DFC2D4B1FA3D677284ADDD200126D9069
                'code64 -> N0Y4M0IxNjU3RkYxRkM1M0I5MkRDMTgxNDhBMUQ2NURGQzJENEIxRkEzRDY3NzI4NEFEREQyMDAxMjZEOTA2OQ==

                ' cryptage du mot de passe
                Dim hasher = SHA256.Create()
                Dim hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(data))
                Dim sBuilder = New StringBuilder()
                For i = 0 To hashValue.Count - 1
                    sBuilder.Append(hashValue(i).ToString("x2"))
                Next
                Return Convert.ToBase64String(Encoding.UTF8.GetBytes(sBuilder.ToString), Base64FormattingOptions.None)

            Catch ex As Exception
                WriteLog("ERR: b64_sha256, " & ex.Message)
                Return ""
            End Try
        End Function

        Function GetErrorCode(code As String) As String
            Try
                ' Dim codeerr As Object = CodeError
                ' For Each codeerr In CodeError.GetValues(coderr)

                ' Next For

                Return ""
            Catch ex As Exception
                WriteLog("ERR: GetErrorCode, " & ex.Message)
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
