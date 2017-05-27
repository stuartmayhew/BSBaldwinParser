Imports System.IO
Imports System.Text.RegularExpressions

Public Class MasterDoc
    Public TableName As String = ""
    Public AuxTable As String = ""
    Public TBSDate As String = ConvertDate(CurrTBSDate)
    Public DocType As String = ""
    Public NotaryDate As String = ""
    Public RecDate As String = ""
    Public Grantor As String = ""
    Public Grantee As String = ""
    Public Address As String = ""
    Public DeedBook As String = ""
    Public Value As String = ""
    Public MineralAcres As String = ""
    Public Lots As String = ""
    Public DownPayment As String = "0"
    Public STR As String = ""
    Public Section As String = ""
    Public TownRange As String = ""
    Public Subdivision As String = ""
    Public Lot As String = ""
    Public Block As String = ""
    Public Remarks As String = ""
    Public DocFile As String = ""
    Public DeedTax As String = ""
    Public LegalDescription As String = ""
    Public Desc As String = ""
    Public DeedType As String = ""
    Public City As String = ""
    Public State As String = ""
    Public Zip As String = ""
    Public KindTax As String = ""
    Public CaseNo As String = ""
    Public PrevInst As String = ""

    Public dontAdd As Boolean = False

    Public rdr As StreamReader

    Public ScannedText As String
    Public dg As New clsDataGetter(My.Settings.BlueSheetsLocalConnectionString)

    Public Sub New(ByVal dType As bsDocType, ByVal currInst As String, ByVal dName As String)
        DocType = dName
        currInst = currInst
    End Sub
    Public Sub ProcessDocument()
        Try
            rdr = New StreamReader(File.OpenRead("c:\scanneddocs\" + currInst + "tPage.htm"))
            ParseInstrumentDoc(rdr)
            Grantee = SwapNames(Grantee)
            Grantor = SwapNames(Grantor)
            Grantee = ReplaceFromTable(Grantee)
            Grantor = ReplaceFromTable(Grantor)

        Catch ex As Exception
            LogError(ex.Message)
        End Try
    End Sub

    Function GetLineTypeBaldwin(ByVal s As String) As BSDocConstants.bsLineType
        If s.Contains("INSTRUMENT DATE:") Then Return bsLineType.bsNotDate
        If s.Contains("DATE FILED:") Then Return bsLineType.bsRecDate
        If s.Contains("Grantor:") Then Return bsLineType.bsGrantor
        If s.Contains("Grantee:") Then Return bsLineType.bsGrantee
        If s.Contains("cgi-ilR3") Then Return bsLineType.bsDocImage
        If s.Contains("VALUE") Then Return bsLineType.bsValue
        If s.Contains("DOWN PAYMENT") Then Return bsLineType.bsDownPayment
        If s.Contains("MINERAL ACRES") Then Return bsLineType.bsMineralAcres
        If s.Contains("LEGALS") Then
            Return bsLineType.bsLotBlockSub
        End If
        If s.Contains("REMARKS") Then Return bsLineType.bsRemarks
    End Function
    Protected Sub ParseInstrumentDoc(ByVal rdr As StreamReader, Optional ByVal dontScan As Boolean = False)

        Dim lineType As BSDocConstants.bsLineType

        Dim s As String
        Dim nDate As String


        While Not rdr.EndOfStream
            s = rdr.ReadLine
            lineType = GetLineTypeBaldwin(s)

            Select Case lineType
                Case bsLineType.bsNotDate
                    s = rdr.ReadLine
                    nDate = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "")
                    NotaryDate = ConvertDate(nDate)
                    If defaultNotaryDate = "" Then
                        If Not NotaryDate.Contains("9999") Then
                            defaultNotaryDate = NotaryDate
                        End If
                    End If

                Case bsLineType.bsRecDate
                    s = rdr.ReadLine
                    nDate = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "")
                    RecDate = ConvertDate(nDate)
                Case bsLineType.bsGrantor
                    s = rdr.ReadLine
                    If Grantor <> "" Then
                        Grantor = Grantor + " and " + MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    Else
                        Grantor = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    End If
                    Grantor = ConvertToTitle(Grantor)
                Case bsLineType.bsGrantee
                    s = rdr.ReadLine
                    If Grantee <> "" Then
                        Grantee = Grantee + " and " + MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    Else
                        Grantee = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    End If
                    Grantee = ConvertToTitle(Grantee)

                Case bsLineType.bsDeedBook
                    s = rdr.ReadLine
                    DeedBook = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                Case bsLineType.bsDownPayment
                    s = rdr.ReadLine
                    DownPayment = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    If DownPayment = "*NA*" Then DownPayment = "0"
                Case bsLineType.bsValue
                    s = rdr.ReadLine
                    Value = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    If Value = "*NA*" Then Value = "0"
                Case bsLineType.bsMineralAcres
                    s = rdr.ReadLine
                    MineralAcres = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                    If MineralAcres = "*NA*" Then MineralAcres = "0"
                Case bsLineType.bsRemarks
                    s = rdr.ReadLine
                    Remarks = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                Case bsLineType.bsLotBlockSub
                    s = rdr.ReadLine
                    While Not s.Contains("Lots")
                        s = rdr.ReadLine
                    End While
                    s = rdr.ReadLine 'get the "</tr> line
                    s = rdr.ReadLine 'see if this is a </table> tag
                    If s.Contains("</TABLE>") Then Exit Select 'no legal desc.

                    s = rdr.ReadLine 'else read out the <tr> tag and start parsing
                    While Not s.Contains("</TABLE>")
                        Dim STRArray() As String
                        STRArray = Split(MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;"), "-")
                        If STRArray.Length = 1 Then Exit While 'we're done
                        If Section = "" Then
                            Section = STRArray(0)
                        Else
                            Section = Section + "," + STRArray(0)
                        End If

                        If TownRange = "" Then
                            TownRange = STRArray(1) + "/" + STRArray(2)
                        Else
                            TownRange = TownRange + "," + STRArray(1) + "/" + STRArray(2)
                        End If

                        'TODO
                        rdr.ReadLine()
                        rdr.ReadLine()
                        s = rdr.ReadLine
                        If Subdivision = "" Then
                            Subdivision = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                        End If

                        s = rdr.ReadLine()
                        If Block = "" Then
                            Block = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                        Else
                            Block = Lot + "," + MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                        End If
                        s = rdr.ReadLine()
                        If Lot = "" Then
                            Lot = MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                        Else
                            Lot = Lot + "," + MidString(s, "=" + Chr(34) + "2" + Chr(34) + ">", "&nbsp;")
                        End If
                        While Not s.Contains("<TR bgcolor")
                            s = rdr.ReadLine
                            If s.Contains("</TABLE>") Then
                                Exit While
                            End If
                        End While
                        If Not s.Contains("</TABLE>") Then
                            s = rdr.ReadLine()
                        End If
                    End While
                    Subdivision = ConvertToTitle(Subdivision)
                    If Block <> "" Then
                        Address = "Lot(s) " + Lot + ",Blck " + Block + " in " + Subdivision
                    Else
                        Address = "Lot(s) " + Lot + " in " + Subdivision
                    End If
                Case bsLineType.bsDocImage
                    ShowStatus("parsing complete - downloading actual document...")
                    DocFile = DownloadDeltaDocument(s)
                    Exit While
            End Select
        End While
        FixSingleQuotes()
        rdr.Close()
        rdr.Dispose()
        rdr = Nothing
    End Sub

    Protected Sub FixSingleQuotes()
        Grantor = FixSingleQuote(Grantor)
        Grantee = FixSingleQuote(Grantee)

        Desc = FixSingleQuote(Desc)
        Address = FixSingleQuote(Address)
        Subdivision = FixSingleQuote(Subdivision)
        Remarks = FixSingleQuote(Remarks)
        LegalDescription = FixSingleQuote(LegalDescription)
        Desc = FixSingleQuote(Desc)
    End Sub

    Public Sub AddToDatabase()

        If dontAdd Then Exit Sub
        Dim cmdStr As String = "INSERT INTO instrumentmasterflat("
        cmdStr = cmdStr + "InstrumentNo, County_ID, DocType,"
        cmdStr = cmdStr + "TableName, Grantor, Grantee, "
        cmdStr = cmdStr + "AuxTableType, NotaryDate, Remarks,"
        cmdStr = cmdStr + "DocFileName, Description, TBSNo,"
        cmdStr = cmdStr + "TBSDate,"
        cmdStr = cmdStr + "Address, Address2, DownPayment, Amount,"
        cmdStr = cmdStr + "DeedTax, Sect, TownRange, Subdivision,"
        cmdStr = cmdStr + "Lot, City, State, Zip, KindTax,CaseNo,PrevInst,DeedType)"

        cmdStr = cmdStr + "VALUES('"
        cmdStr = cmdStr + CStr(currInst) + "',"
        cmdStr = cmdStr + CStr(CountyID) + ",'"
        cmdStr = cmdStr + DocType + "','"
        cmdStr = cmdStr + TableName + "','"
        cmdStr = cmdStr + Grantor + "','"
        cmdStr = cmdStr + Grantee + "',"
        cmdStr = cmdStr + AuxTable + ",'"
        cmdStr = cmdStr + NotaryDate + "','"
        cmdStr = cmdStr + Remarks + "','"
        cmdStr = cmdStr + DocFile + "','"
        cmdStr = cmdStr + Desc + "',"
        cmdStr = cmdStr + CStr(TBSNo) + ",'"
        cmdStr = cmdStr + TBSDate + "','"
        cmdStr = cmdStr + Address + "','"
        cmdStr = cmdStr + "" + "','"
        cmdStr = cmdStr + DownPayment + "','"
        cmdStr = cmdStr + Value + "','"
        cmdStr = cmdStr + CalcDeedTax(DownPayment) + "','"
        cmdStr = cmdStr + Section + "','"
        cmdStr = cmdStr + TownRange + "','"
        cmdStr = cmdStr + Subdivision + "','"
        cmdStr = cmdStr + Lot + "','"
        cmdStr = cmdStr + City + "','"
        cmdStr = cmdStr + State + "','"
        cmdStr = cmdStr + Zip + "','"
        cmdStr = cmdStr + KindTax + "','"
        cmdStr = cmdStr + CaseNo + "','"
        cmdStr = cmdStr + PrevInst + "','"
        cmdStr = cmdStr + DeedType + "')"

        Try
            dg.RunCommand(cmdStr)
        Catch ex As Exception
            LogError(ex.Message)
        End Try
    End Sub
End Class
