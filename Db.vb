Imports MySql.Data.MySqlClient
Public Class Db

    Private Const str As String = "server=localhost;uid=glsuser;pwd=glspass;database=glsdb;Convert Zero Datetime=True"

    Private Const query As String = "INSERT INTO traces (" &
                                    "imei,event_name,date_time,latitude,longitude,speed,orientation,ip,port) VALUES " &
                                   "(@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9)"

    Private Const query2 As String = "INSERT INTO trace_errors (trace,ip,port,date_time) VALUES (@param1, @param2, @param3, @param4)"

    Dim cmd As MySqlCommand

    Public Sub insertTrace(ByVal imei As String, ByVal event_name As String, ByVal date_time As String,
                           ByVal latitude As String, ByVal longitude As String, ByVal speed As Integer,
                           ByVal orientation As Integer, ByVal ip As String, ByVal port As Integer)

        Try
            Using conn As New MySqlConnection(str)
                conn.Open()

                cmd = New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@param1", imei)
                cmd.Parameters.AddWithValue("@param2", event_name)
                cmd.Parameters.AddWithValue("@param3", date_time)
                cmd.Parameters.AddWithValue("@param4", latitude)
                cmd.Parameters.AddWithValue("@param5", longitude)
                cmd.Parameters.AddWithValue("@param6", speed)
                cmd.Parameters.AddWithValue("@param7", orientation)
                cmd.Parameters.AddWithValue("@param8", ip)
                cmd.Parameters.AddWithValue("@param9", port)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.ReadLine()
        End Try
    End Sub

    Public Sub insertTrace(ByVal trace As String, ByVal ip As String, ByVal port As Integer, ByVal date_time As String)
        Try
            Using conn As New MySqlConnection(str)
                conn.Open()

                cmd = New MySqlCommand(query2, conn)
                cmd.Parameters.AddWithValue("@param1", trace)
                cmd.Parameters.AddWithValue("@param2", ip)
                cmd.Parameters.AddWithValue("@param3", port)
                cmd.Parameters.AddWithValue("@param4", date_time)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Console.ReadLine()
        End Try
    End Sub
End Class
