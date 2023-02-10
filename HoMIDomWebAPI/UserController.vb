﻿
Imports System.Web.Http
Imports HoMIDom
Imports HoMIDom.HoMIDom

Public Class UserController
    Inherits BaseHoMIdomController

    Public Function [Get]() As List(Of Users.User)
        Return HoMIDomAPI.CurrentServer.GetAllUsers(Me.ServerKey)
    End Function

    Public Function [Get](id As String) As Users.User
        Return HoMIDomAPI.CurrentServer.ReturnUserByUsername(Me.ServerKey, id)
    End Function


    <HttpGet()>
    Public Function GetValue(id As String, field As String) As Object
        Return Me.GetField(Me.Get(id), field)
    End Function
End Class
