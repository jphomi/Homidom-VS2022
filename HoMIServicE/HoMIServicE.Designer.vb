﻿Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HoMIServicE
    Inherits System.ServiceProcess.ServiceBase

    'UserService remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub


    ' Point d'entrée principal du processus
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' Plusieurs services NT s'exécutent dans le même processus. Pour ajouter
        ' un autre service à ce processus, modifiez la ligne suivante
        ' pour créer un second objet service. Par exemple,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '

        Try
            'ServicesToRun = New System.ServiceProcess.ServiceBase() {New HoMIServicE}
            'System.ServiceProcess.ServiceBase.Run(ServicesToRun)

            If (Environment.UserInteractive) Then

                'handle du CTRL+C pour eviter de fermer le process à la barbare
                AddHandler Console.CancelKeyPress, AddressOf myHandler

                Dim service As New HoMIServicE
                Dim args As String() = Nothing
                service.OnStart(args)
                'Console.WriteLine("Press any key to stop program")
                'Console.Read()
                'service.OnStop()
                Environment.Exit(0)
            Else
                ServicesToRun = New System.ServiceProcess.ServiceBase() {New HoMIServicE}
                System.ServiceProcess.ServiceBase.Run(ServicesToRun)
            End If
        Catch ex As Exception
            Dim myEventLog = New EventLog()
            myEventLog.Source = "HoMIDoM"
            myEventLog.WriteEntry("Erreur lors du lancement du service : " & ex.ToString, EventLogEntryType.Error, 1)
            myEventLog = Nothing
        End Try

    End Sub

    Shared Sub myHandler(ByVal sender As Object, ByVal args As ConsoleCancelEventArgs)
        args.Cancel = True
        Console.WriteLine(Now & " INFO   CTRL+C -> Arrêt du service")
    End Sub

    'Requise par le Concepteur de composants
    Private components As System.ComponentModel.IContainer

    ' REMARQUE : la procédure suivante est requise par le Concepteur de composants
    ' Elle peut être modifiée à l'aide du Concepteur de composants.  
    ' Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        '
        'HoMIServicE
        '
        Me.ServiceName = "HoMIServicE"
    End Sub

End Class
