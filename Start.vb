Imports System.ComponentModel
Imports System.Deployment.Application
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Timers

Module Start
#Region "VARIABLES GLOBALES"
    Dim glsIP() As IPAddress
    Dim pcIP() As IPAddress
    Dim aTimer As Timers.Timer
    Dim receivingUdpClient As UdpClient
    Dim RemoteIpEndPoint As New IPEndPoint(IPAddress.Any, 0)
    Dim threadReceive As Thread
    Dim threadTeclas As Thread
    Dim puertoEscucha As Integer = 18000
    Dim ipEscucha As String = "172.87.221.236"
    Dim ipCorrecta As Boolean = True
    Dim puertoCorrecto As Boolean = True
    Dim trace As New Trace
    Dim vehicles As New List(Of Vehicle)
    Dim udpsender As UdpClient
    Dim trsender As Thread
    Dim trEvent As Thread
    Dim detenidoPorUsuario As Boolean = False
    Dim contador As Integer = 0
    Private _isTransmittingPc As Boolean = False
    Private _isTransmittingGls As Boolean = False

    Public Property IsTransmittingPc() As Boolean
        Get
            Return _isTransmittingPc
        End Get
        Set(ByVal value As Boolean)
            _isTransmittingPc = value
        End Set
    End Property
    Public Property IsTransmittingGls() As Boolean
        Get
            Return _isTransmittingGls
        End Get
        Set(ByVal value As Boolean)
            _isTransmittingGls = value
        End Set
    End Property

#End Region
    Sub Main()
        Try
            Do Until puertoCorrecto
                setPort(0)
            Loop

            Do Until ipCorrecta
                setIP(0)
            Loop

            threadTeclas = New Thread(AddressOf leerTeclas)
            threadTeclas.Start()

            iniciarCaptura()
            SetTimer()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.ReadLine()
        End Try
    End Sub

#Region "FUNCIONES Y RUTINAS"
    Sub setTitle()
        Try

            Console.Title = "<::: Capturador UDP TK103A - " & My.Settings._ip & ":" & My.Settings._port & " :::> Transmisión: " &
                            "Gls " & If(IsTransmittingGls, "ON", "OFF") & " - " &
                            "Local " & If(IsTransmittingPc, "ON", "OFF") &
                            " REV. " & ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString
        Catch ex As Exception
            'Console.WriteLine(ex.Message)
            Console.Title = "<::: Capturador UDP TK103A - " & My.Settings._ip & ":" & My.Settings._port & " :::> Transmisión: " &
                            "Gls " & If(IsTransmittingGls, "ON", "OFF") & " - " &
                            "Local " & If(IsTransmittingPc, "ON", "OFF") &
                            " REV. "
        End Try
    End Sub

    Function setPort(ByVal peticionDeCambio As Integer) As Boolean
        Dim tempPort As Integer = Nothing

        If peticionDeCambio = 1 Then
            Console.WriteLine("INTRODUZCA EL PUERTO DE ESCUCHA: ")
            Dim port As Object = Console.ReadLine()

            If Integer.TryParse(port, tempPort) Then
                My.Settings._port = tempPort
                My.Settings.Save()
                puertoEscucha = CInt(My.Settings._port)
                puertoCorrecto = True
                Return True
            Else
                puertoCorrecto = False
                Return False
            End If
        Else
            If Not My.Settings._port = "" Then
                puertoEscucha = CInt(My.Settings._port)
                puertoCorrecto = True
                Return True
            Else
                Console.WriteLine("INTRODUZCA EL PUERTO DE ESCUCHA: ")
                If Integer.TryParse(Console.ReadLine(), tempPort) Then
                    My.Settings._port = tempPort
                    My.Settings.Save()
                    puertoEscucha = CInt(My.Settings._port)
                    puertoCorrecto = True
                    Return True
                Else
                    puertoCorrecto = False
                    Return False
                End If
            End If
        End If
    End Function

    Function setIP(ByVal peticionDeCambio As Integer) As Boolean
        Dim tempIP As IPAddress = Nothing

        If peticionDeCambio = 1 Then

            Dim ipadd() As IPAddress = Dns.GetHostAddresses("gls.villasoftgps.com.ve")

            Return True
            Dim ip As Object = ipadd(0).ToString()

            If IPAddress.TryParse(ip, tempIP) Then
                My.Settings._ip = tempIP.ToString
                My.Settings.Save()
                ipEscucha = My.Settings._ip

                Try
                    Dim lep As New IPEndPoint(IPAddress.Parse(ipEscucha), puertoEscucha)
                    receivingUdpClient = New UdpClient(lep)
                    ipCorrecta = True
                    receivingUdpClient.Close()
                    Return True
                Catch ex As Exception
                    ipCorrecta = False
                    Return False
                End Try
            Else
                ipCorrecta = False
                Return False
            End If
        Else
            If Not My.Settings._ip = "" Then
                ipEscucha = ""

                Try
                    Dim lep As New IPEndPoint(IPAddress.Parse(ipEscucha), puertoEscucha)
                    receivingUdpClient = New UdpClient(lep)
                    ipCorrecta = True
                    receivingUdpClient.Close()
                    Return True
                Catch ex As Exception
                    Console.WriteLine("INTRODUZCA LA IP DE ESCUCHA: ")
                    If IPAddress.TryParse(Console.ReadLine, tempIP) Then
                        My.Settings._ip = tempIP.ToString
                        My.Settings.Save()
                        ipEscucha = My.Settings._ip

                        Try
                            Dim lep As New IPEndPoint(IPAddress.Parse(ipEscucha), puertoEscucha)
                            receivingUdpClient = New UdpClient(lep)
                            ipCorrecta = True
                            receivingUdpClient.Close()
                            Return True
                        Catch exe As Exception
                            ipCorrecta = False
                            Return False
                        End Try
                    Else
                        ipCorrecta = False
                        Return False
                    End If
                End Try
            Else
                Console.WriteLine("INTRODUZCA LA IP DE ESCUCHA: ")
                If IPAddress.TryParse(Console.ReadLine, tempIP) Then
                    My.Settings._ip = tempIP.ToString
                    My.Settings.Save()
                    ipEscucha = My.Settings._ip

                    Try
                        Dim lep As New IPEndPoint(IPAddress.Parse(ipEscucha), puertoEscucha)
                        receivingUdpClient = New UdpClient(lep)
                        ipCorrecta = True
                        receivingUdpClient.Close()
                        Return True
                    Catch ex As Exception
                        ipCorrecta = False
                        Return False
                    End Try
                Else
                    ipCorrecta = False
                    Return False
                End If
            End If
        End If
    End Function

    Private Sub SetTimer()
        ' Create a timer with a two second interval.
        aTimer = New Timers.Timer(2000)
        ' Hook up the Elapsed event for the timer. 
        AddHandler aTimer.Elapsed, AddressOf OnTimedEvent
        aTimer.AutoReset = True
        aTimer.Enabled = True
    End Sub

    Private Sub OnTimedEvent(source As Object, e As ElapsedEventArgs)
        If threadReceive IsNot Nothing Then
            If Not threadReceive.IsAlive Then
                If Not detenidoPorUsuario Then
                    iniciarCaptura()
                End If
            End If

            If contador < 30 Then
                contador += 1
            Else
                Try
                    glsIP = Dns.GetHostAddresses("gls.villasoftgps.com.ve")
                    pcIP = Dns.GetHostAddresses("mipc.villasoftgps.com.ve")
                    contador = 0
                Catch ex As Exception

                End Try
            End If
        Else
            iniciarCaptura()
        End If
    End Sub

    Function ReceiveMessages()
        Try
            Console.WriteLine("CAPTURA INICIADA CORRECTAMENTE AL PUERTO " & My.Settings._port)

            glsIP = Dns.GetHostAddresses("db.villasoftgps.com.ve")
            pcIP = Dns.GetHostAddresses("mipc.villasoftgps.com.ve")
            Dim receiveBytes As Byte() = Nothing
            Dim strReturnData As String = ""

            While True
                receiveBytes = receivingUdpClient.Receive(RemoteIpEndPoint)

                If IsTransmittingGls Then
                    Using sendingClient As New UdpClient(glsIP(0).ToString, 24000)
                        sendingClient.Send(receiveBytes, receiveBytes.Length)
                    End Using
                End If

                If IsTransmittingPc Then
                    Using sendingClient As New UdpClient(pcIP(0).ToString, 24000)
                        sendingClient.Send(receiveBytes, receiveBytes.Length)
                    End Using
                End If

                strReturnData = Encoding.ASCII.GetString(receiveBytes)

                strReturnData = strReturnData.Replace("$", "")

                ''ENVIAMOS LA TRAMA, LA IP Y EL PUERTO, A LA CLASE CORRESPONDIETE PARA SU TRATAMIENTO Y VALIDACION...
                trace.transformTrace(strReturnData, RemoteIpEndPoint.Address.ToString, RemoteIpEndPoint.Port.ToString)

                'ESCRIBIMOS EN LA CONSOLA LA TRAMA RECIBIDA POR EL GPS...
                Console.WriteLine(Now)
                Console.WriteLine(strReturnData)
                Console.WriteLine()
            End While

        Catch ex As Exception
            Console.WriteLine(Now)
            Console.WriteLine(ex.Message)
            Console.WriteLine()
            detenerCaptura()
            iniciarCaptura()
        End Try
        Return True
    End Function

    Function leerTeclas()
        Dim cki As ConsoleKeyInfo
        Dim combinacion As String = Nothing

        Console.TreatControlCAsInput = True

        Do
            cki = Console.ReadKey(True)
            combinacion = cki.Key.ToString

            Select Case combinacion
                Case "F4"
                    detenidoPorUsuario = True
                    detenerCaptura()
                Case "F5"
                    IsTransmittingGls = Not IsTransmittingGls
                    setTitle()
                Case "F6"
                    IsTransmittingPc = Not IsTransmittingPc
                    setTitle()
                Case "F10"
                    iniciarCaptura()
                Case "F11"
                    If threadReceive.IsAlive = False Then
                        cambiarPuerto()
                    End If
                Case "F12"
                    If threadReceive.IsAlive = False Then
                        cambiarIP()
                    End If
            End Select

            combinacion = Nothing
        Loop
    End Function

    Sub detenerCaptura()
        If threadReceive.IsAlive = True Then
            threadReceive.Abort()
            receivingUdpClient.Close()
            Console.WriteLine("CAPTURA DETENIDA POR EL USUARIO, PRESIONE F10 PARA INICIAR LA CAPTURA NUEVAMENTE")
        End If

        If Not threadTeclas.IsAlive = True Then
            threadTeclas.Start()
        End If
    End Sub

    Sub iniciarCaptura()
        Try
            Dim lep As New IPEndPoint(IPAddress.Any, puertoEscucha)
            receivingUdpClient = New UdpClient(lep)
            threadReceive = New Thread(AddressOf ReceiveMessages)
            threadReceive.Start()

            setTitle()
            detenidoPorUsuario = False
        Catch ex As Exception
            detenidoPorUsuario = False
            Console.WriteLine(vbNewLine & ex.Message & vbNewLine)
        End Try
    End Sub

    Sub cambiarPuerto()
        Try
            If threadReceive.IsAlive = True Then
                threadReceive.Abort()
                receivingUdpClient.Close()
            End If

            puertoCorrecto = False
            Do Until puertoCorrecto
                setPort(1)
            Loop

            iniciarCaptura()
        Catch ex As Exception
            Console.WriteLine(vbNewLine & ex.Message & vbNewLine)
            Console.ReadLine()
        End Try
    End Sub

    Sub cambiarIP()
        Try
            If threadReceive.IsAlive = True Then
                threadReceive.Abort()
                receivingUdpClient.Close()
            End If

            ipCorrecta = False
            Do Until ipCorrecta
                setIP(1)
            Loop

            iniciarCaptura()
        Catch ex As Exception
            Console.WriteLine(vbNewLine & ex.Message & vbNewLine)
            Console.ReadLine()
        End Try
    End Sub
#End Region
End Module