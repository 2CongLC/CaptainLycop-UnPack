Imports System
Imports System.Collections
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports System.Text


Module Program

    Private br As BinaryReader
    Private des As String
    Private source As String

    Dim size As Integer = Nothing
    Dim n As Integer = Nothing
    Dim type As String


    <Obsolete>
    Sub Main(args As String())


        If args.Count = 0 Then
            Console.WriteLine("Tool UnPack - 2CongLC.vn :: 2024")
        Else
            source = args(0)
        End If

        Dim p As String = Nothing

        If IO.File.Exists(source) Then
            br = New BinaryReader(File.OpenRead(source))
            br.BaseStream.Position = 20

            n = 0

            des = Path.GetDirectoryName(source) & "\" & Path.GetFileNameWithoutExtension(source)
            Directory.CreateDirectory(des)

            While br.BaseStream.Position < br.BaseStream.Length
                Dim variable = br.ReadBytes(4)
                Array.Reverse(variable)
                size = BitConverter.ToInt32(variable, 0)
                type = GetTypes()
                If Type = ".zlib" Then
                    variable = br.ReadBytes(4)
                    Array.Reverse(variable)
                    Dim sizeUncompressed = BitConverter.ToInt32(variable, 0)

                    Dim unknow As Int16 = br.ReadInt16()
                    Dim ms As New MemoryStream()
                    Using ds = New DeflateStream(New MemoryStream(br.ReadBytes(size - 6)), CompressionMode.Decompress)
                        ds.CopyTo(ms)
                    End Using

                    Dim position = br.BaseStream.Position
                    size = sizeUncompressed

                    br = New BinaryReader(ms)
                    br.BaseStream.Position = 0
                    type = GetTypes()
                    WriteFile()
                    br = New BinaryReader(File.OpenRead(args(0)))
                    br.BaseStream.Position = position
                Else
                    WriteFile()
                End If
                n += 1
            End While
            Console.WriteLine("UnPack Done !")
        End If
        Console.ReadLine()
    End Sub
    Sub WriteFile()
        Dim bw As New BinaryWriter(File.Create(des & "\" & n & type))
        bw.Write(br.ReadBytes(size))
        bw.Close()
    End Sub

    <Obsolete>
    Function GetTypes() As String
        Dim magicBytes = br.ReadBytes(4)
        br.BaseStream.Position -= 4
        Dim magic = System.Text.Encoding.UTF7.GetString(magicBytes)
        Select Case magic
            Case "OggS"
                Return ".ogg"
            Case ChrW(&H89) & "PNG"
                Return ".png"
            Case "RIFF"
                Return ".wav"
            Case Else
                br.BaseStream.Position += 4
                magicBytes = br.ReadBytes(2)
                If magicBytes(0) <> &H78 OrElse magicBytes(1) <> &H9C Then
                    Console.WriteLine("Unrecognized subfile type.")
                End If

                br.BaseStream.Position -= 6
                Return ".zlib"
        End Select
    End Function
End Module
