Public Class Trace
    Dim db As New Db

    Public Sub transformTrace(ByVal trace As String, ByVal ip As String, ByVal port As Integer)
        Try
            '==============================================================================================
            'TRAMA DE PRUEBA
            'imei:863070018661701,tracker,1308151648,,F,211856.000,A,1024.3947,N,07126.9892,W,0.17,237.97,;
            '==============================================================================================

            'DIVIDIMOS LA TRAMA ENTRE LA PALABRA IMEI POR LOS DOS PUNTOS (:)...
            Dim arrOriginal() As String = trace.Split(":")

            If arrOriginal.Length > 1 Then

                'DIVIDIMOS LA TRAMA LIMPIA POR COMAS (,) PARA OBTENER LOS DIFERENTES PARAMETROS DEL GPS
                Dim tramaLimpia() As String = arrOriginal(1).Split(",")

                If tramaLimpia.Length > 11 Then
                    If Not tramaLimpia(11) = Nothing Or tramaLimpia(7).Length > 5 Then

                        'DECLARAMOS LAS VARIABLES DE LOS PARAMETROS
                        Dim imei As String = tramaLimpia(0)
                        Dim event_name As String = tramaLimpia(1)
                        Dim date_time As String = Now.Year & "-" &
                                                  CStr(Now.Month).Trim.PadLeft(2, "0") & "-" &
                                                  CStr(Now.Day).Trim.PadLeft(2, "0") & " " &
                                                  CStr(Now.Hour).Trim.PadLeft(2, "0") & ":" &
                                                  CStr(Now.Minute).Trim.PadLeft(2, "0") & ":" &
                                                  CStr(Now.Second).Trim.PadLeft(2, "0")
                        Dim latitude As String = convertirLatitud(tramaLimpia(7))
                        Dim longitude As String = convertirLongitud(tramaLimpia(9))
                        Dim speed As Integer = CInt(Convert.ToDouble(tramaLimpia(11), System.Globalization.CultureInfo.InvariantCulture) * 1.852)
                        Dim orientation As Integer = 0

                        If tramaLimpia(12).Length > 0 Then
                            orientation = CInt(Convert.ToDouble(tramaLimpia(12).Replace(";", ""), Globalization.CultureInfo.InvariantCulture))
                        End If

                        'ENVIAMOS LOS DATOS A LA CLASE QUE SE ENCARGARÁ DE ALMACENARLOS EN LA BASE DE DATOS...
                        db.insertTrace(imei, event_name, date_time, latitude, longitude, speed, orientation, ip, port)
                    End If
                End If
            End If
        Catch ex As Exception
            Dim fechaHora As String = Now.Year & "-" &
                                                  CStr(Now.Month).Trim.PadLeft(2, "0") & "-" &
                                                  CStr(Now.Day).Trim.PadLeft(2, "0") & " " &
                                                  CStr(Now.Hour).Trim.PadLeft(2, "0") & ":" &
                                                  CStr(Now.Minute).Trim.PadLeft(2, "0") & ":" &
                                                  CStr(Now.Second).Trim.PadLeft(2, "0")

            db.insertTrace(trace, ip, port, fechaHora)
        End Try
    End Sub

#Region "FUNCIONES Y RUTINAS"

    Private Function convertirLatitud(ByVal latOriginal As String)
        Dim tempGrados As String = Mid(latOriginal, 1, 2)
        Dim tempMinutos As String = Mid(latOriginal, 3)
        Dim _minLat As Double = Math.Round(Convert.ToDouble(tempMinutos, System.Globalization.CultureInfo.InvariantCulture) / 60, 6)
        Dim minLat() As String = CStr(_minLat).Split(",")
        Dim lat As String = CInt(tempGrados) & "." & minLat(1)

        Return lat
    End Function

    Private Function convertirLongitud(ByVal lngOriginal As String)
        Dim tempGrados As String = Mid(lngOriginal, 2, 2)
        Dim tempMinutos As String = Mid(lngOriginal, 4)
        Dim _minLng As Double = Math.Round(Convert.ToDouble(tempMinutos, System.Globalization.CultureInfo.InvariantCulture) / 60, 6)
        Dim minLng() As String = CStr(_minLng).Split(",")
        Dim lng As String = (CInt(tempGrados) * -1) & "." & minLng(1)

        Return lng
    End Function

#End Region
End Class
