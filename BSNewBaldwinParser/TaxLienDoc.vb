Imports System.IO
Imports System.Text.RegularExpressions

Public Class TaxLienDoc

    Inherits MasterDoc

    Public Sub New(ByVal dType As bsDocType, ByVal currInst As String, ByVal dName As String)
        MyBase.New(dType, currInst, dName)
        DocType = dName
    End Sub
    Public Overloads Sub ProcessDocument()
        MyBase.ProcessDocument()

        If Grantor.ToUpper.Contains("UNITED STATES") Then
            TableName = "US"
        ElseIf Grantor.ToUpper.Contains("STATE OF ALABAMA") Then
            TableName = "ST"
        ElseIf Grantor.ToUpper.Contains("ALABAMA DEPARTMENT OF REVENUE") Then
            TableName = "ST"
        ElseIf Grantor.ToUpper.Contains("BALDWIN") Then
            TableName = "BT"
        Else
            dontAdd = True
        End If
    End Sub
    Public Sub AddToDatabaseTaxLien()
        If dontAdd Then Exit Sub
        Dim tTax As Double
        If DownPayment <> "" And DownPayment <> "*NA*" Then
            tTax = (DownPayment / 500) * 0.5
        End If


        Desc = Grantee + "  " + "$" + CStr(Value)

        Dim cmdStr As String = "INSERT INTO instrumentmasterflat("
        cmdStr = cmdStr + "InstrumentNo, County_ID, DocType,"
        cmdStr = cmdStr + "TableName, Grantor, Grantee,Amount, "
        cmdStr = cmdStr + "AuxTableType, NotaryDate, Remarks,"
        cmdStr = cmdStr + "DocFileName, Description, TBSNo,"
        cmdStr = cmdStr + "Address, KindTax)"
        'City, State, Zip, KindTax, 
        'CaseNo

        cmdStr = cmdStr + "VALUES('"
        cmdStr = cmdStr + CStr(currInst) + "',"
        cmdStr = cmdStr + CStr(CountyID) + ",'"
        cmdStr = cmdStr + DocType + "','"
        cmdStr = cmdStr + TableName + "','"
        cmdStr = cmdStr + Grantor + "','"
        cmdStr = cmdStr + Grantee + "','"
        cmdStr = cmdStr + Value + "',"
        cmdStr = cmdStr + "5,'"
        cmdStr = cmdStr + NotaryDate + "','"
        cmdStr = cmdStr + Remarks + "','"
        cmdStr = cmdStr + DocFile + "','"
        cmdStr = cmdStr + Desc + "',"
        cmdStr = cmdStr + CStr(TBSNo) + ",'"
        cmdStr = cmdStr + Address + "','"
        cmdStr = cmdStr + KindTax + "')"

        'Address, , , , 
        'DeedTax, , , , 
        ', , LienInstr, PrevInst, 
        'City, State, Zip, KindTax, 
        'CaseNo

        Try
            fmMain.dg.RunCommand(cmdStr)
        Catch ex As Exception
            LogError(ex.Message)
        End Try

    End Sub

End Class
