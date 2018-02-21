Imports System.Net
Imports System.Net.Sockets

Public Class Vehicle
    Private _imei As String
    Public Property Imei() As String
        Get
            Return _imei
        End Get
        Set(ByVal value As String)
            _imei = value
        End Set
    End Property

    Private _date_time As Date
    Public Property DateTime() As Date
        Get
            Return _date_time
        End Get
        Set(ByVal value As Date)
            _date_time = value
        End Set
    End Property

    Private _end_point As IPEndPoint
    Public Property EndPoint() As IPEndPoint
        Get
            Return _end_point
        End Get
        Set(ByVal value As IPEndPoint)
            _end_point = value
        End Set
    End Property

    Private _udp_client As UdpClient
    Public Property UdpClient() As UdpClient
        Get
            Return _udp_client
        End Get
        Set(ByVal value As UdpClient)
            _udp_client = value
        End Set
    End Property
End Class
