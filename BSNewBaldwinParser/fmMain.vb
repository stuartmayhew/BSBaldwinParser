Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Windows.Forms


Public Class fmMain
    Public dg As New clsDataGetter(My.Settings.BlueSheetsLocalConnectionString)

    Public StartInst As Integer
    Public EndInst As Integer

    Dim gotALTaxLien As Boolean = False
    Dim gotUSTaxLien As Boolean = False
    Dim gotForeclosureDeed As Boolean = False

    Dim origInst As String


    Private Sub fmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DeleteScanneddocs()
        'CheckForUpdate()
        Text = "CLOUD NEW AND IMPROVED Baldwin Doc Parser v." + My.Application.Info.Version.ToString
    End Sub

    Private Sub btnBaldwin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBaldwin.Click
        btnClose.Enabled = False
        If ValidateDupes(0) Then
            StartInst = CInt(Me.tbStartInst.Text)
            EndInst = CInt(Me.tbEndInst.Text)
            CurrTBSDate = ConvertDate(Me.dpTBSDate.Value)
            ParseBaldwinRange()
        End If

        MsgBox("Done parsing - you can close the parser")
        btnClose.Enabled = True
    End Sub
    Private Sub ParseBaldwinRange()
        Dim wr As New System.Net.WebClient
        Dim pagedata As Stream
        Dim reader As StreamReader
        Dim ss As String
        Dim i As Integer
        Dim j As Integer
        Dim EndCounter As Integer = 1

        CountyID = 0
        ShowStatus("Setting next instrument")
        ShowStatus("processing Bluesheet " + Str(TBSNO) + " from " + Str(StartInst))

        'Dim dr As MySql.Data.MySqlClient.MySqlDataReader
        'Dim testArray As New ArrayList

        'If StartInst > 0 Then
        '    dr = dg.GetDataReader("SELECT InstrumentNo FROM doctester WHERE instrumentno > " + tbStartInst.Text + " ORDER BY instrumentno")
        'Else
        '    dr = dg.GetDataReader("SELECT InstrumentNo FROM doctester ORDER BY instrumentno")
        'End If
        'While dr.Read
        '    testArray.Add(dr(0))
        'End While

        'dg.KillReader(dr)


        For i = StartInst To EndInst
            Try
                If isAborted Then
                    Exit For
                End If

                currInst = Str(i)

                ShowStatus("checking inst no " + Str(i))

                ss = BALDWINURL + Trim(Str(i)) + BALDWINURL2

                If File.Exists("c:\scanneddocs\" + Str(i) + "tPage.htm") Then
                    File.Delete("c:\scanneddocs\" + Str(i) + "tPage.htm")
                End If

                wr.DownloadFile(ss, "c:\scanneddocs\" + Str(i) + "tPage.htm")

                reader = New StreamReader(File.OpenRead("c:\scanneddocs\" + Str(i) + "tPage.htm"))

                Dim s As String



                While Not reader.EndOfStream
                    s = reader.ReadLine

                    While InStr(s, "Records Complete Thru") = 0
                        If s Is Nothing Then
                            Exit While
                        End If
                        If s.Contains("START-        Invalid Link") Then
                            EndCounter = EndCounter + 1
                            If EndCounter = 10 Then
                                Exit Sub
                            End If
                        End If
                        s = reader.ReadLine
                    End While
                    For j = 0 To 3
                        s = reader.ReadLine()
                        If s Is Nothing Then
                            Exit While
                        End If
                    Next

                    s = reader.ReadLine
                    reader = Nothing

                    GetBaldwinDocType(s, i)
                    Exit While
                End While

                'ShowStatus(ex.Message)

                'End Try
                wr.Dispose()
                pagedata = Nothing
            Catch ex As Exception

            End Try

        Next
    End Sub

    Private Function GetBaldwinDocType(ByVal s As String, ByVal inst As Integer) As Boolean


        If s.Contains("AGREEMENT") Or s.Contains("CERTIFICATE") Or s.Contains("TRUST") Or s.Contains("PROMISSORY NOTE") Or s.Contains("CONTRACT") Then
            ShowStatus("an agreement...")
            currInst = Str(inst)
            AG = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Agreement")
            AG.ProcessDocument()
            AG.TableName = "AG"
            AG.AuxTable = 1
            AG.Desc = "An Agreement between " + AG.Grantor + " and " + AG.Grantee
            AG.AddToDatabase()
            AG = Nothing
            Return True
        End If

        If s.Contains("AMENDMENT") Then
            ShowStatus("an amendment...")
            currInst = Str(inst)
            AM = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Amendment")
            AM.ProcessDocument()
            AM.TableName = "AM"
            AM.AuxTable = 1

            AM.AddToDatabase()
            AM = Nothing
            Return True
        End If

        If s.Contains("ART OF INCORP NON PR") Then
            ShowStatus("an art of incorp...")
            currInst = Str(inst)
            AI = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Articles of Incorporation, Non Profit")
            AI.ProcessDocument()
            AI.TableName = "AI"
            AI.AuxTable = 1
            AI.Desc = AI.Grantor + "-Purpose is to transact any and all lawful business"
            AI.AddToDatabase()
            AI = Nothing
            Return True
        End If

        If s.Contains("ART OF INCORP PROFIT") Then
            ShowStatus("an art of incorp...")
            currInst = Str(inst)
            AI = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Articles of Incorporation, Profit")
            AI.ProcessDocument()
            AI.TableName = "AI"
            AI.AuxTable = 1
            AI.Desc = AI.Grantor + "-Purpose is to transact any and all lawful business"
            AI.AddToDatabase()
            AI = Nothing
            Return True
        End If

        If s.Contains("ART OF ORGANIZATION") Or s.Contains("LIMITED LIABILITY PA") Then
            ShowStatus("an art. of organization...")
            currInst = Str(inst)
            AO = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Article of Organization")
            AO.ProcessDocument()
            AO.TableName = "AO"
            AO.AuxTable = 1
            AO.Desc = AO.Grantor + "-Purpose is to transact any and all lawful business"
            AO.AddToDatabase()
            AO = Nothing
            Return True
        End If

        If s.Contains("CONDO DEED") Or s.Contains("CONDO QUIT CLAIM DEE") Then
            ShowStatus("a condo deed...")
            currInst = Str(inst)
            Cdeed = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Condo Deed")
            Cdeed.ProcessDocument()
            Cdeed.Desc = Cdeed.Address
            Cdeed.Desc = Cdeed.Desc.Replace("Lot(s)", "Unit(s)")
            Cdeed.TableName = "C"
            Cdeed.DeedType = "C"
            Cdeed.AuxTable = 2
            PendingDeedInstrument = Val(currInst)
            Cdeed.AddToDatabase()
            Cdeed = Nothing
            Return True
        End If

        If s.Contains("FORECLOSURE DEED") Or s.Contains("AUCTIONEERS DEED") Or s.Contains("CONDO FORECLOSURE DE") Then
            ShowStatus("a foreclosure..")
            currInst = Str(inst)
            FO = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Foreclosure")
            FO.ProcessDocument()
            FO.TableName = "FO"
            FO.AuxTable = 1
            FO.AddToDatabase()
            FO = Nothing
            Return True
        End If

        If s.Contains("DECREE") Then
            ShowStatus("a decree...")
            currInst = Str(inst)
            DE = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Decree")
            DE.ProcessDocument()
            DE.TableName = "DE"
            DE.Desc = Trim(DE.Grantor) + " vs. " + Trim(DE.Grantee)
            DE.AuxTable = 1
            DE.AddToDatabase()
            DE = Nothing
            Return True
        End If

        If s.Contains("DEED") Then
            ShowStatus("a deed...")
            currInst = Str(inst)
            DEED = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Deed")
            DEED.ProcessDocument()
            DEED.TableName = "D"
            DEED.AuxTable = 2
            PendingDeedInstrument = Val(currInst)
            DEED.Desc = DEED.Address
            DEED.AddToDatabase()
            DEED = Nothing
            Return True
        End If

        If s.Contains("MAP") Then
            ShowStatus("an map...")
            currInst = Str(inst)
            MAP = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Agreement")
            MAP.ProcessDocument()
            MAP.TableName = "MAP"
            MAP.AuxTable = 1
            MAP.AddToDatabase()
            MAP = Nothing
            Return True
        End If


        If s.Contains("MORTGAGE") Or s.Contains("ACCOMODATION MORTGAG") Then
            ShowStatus("a mortgage...")
            currInst = Str(inst)
            MORT = New MortgageDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Mortgage")
            MORT.ProcessDocument()
            MORT.AuxTable = 2
            'ConvertFile(currInst)
            'MORT.Value = GetMortAmount(currInst)
            MORT.AddToDatabaseMort()
            MORT = Nothing
            Return True
        End If
        If s.Contains("JUDGEMENT") Then
            ShowStatus("a judgement...")
            currInst = Str(inst)
            JUDGE = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Judgement")
            JUDGE.ProcessDocument()
            JUDGE.TableName = "J"
            JUDGE.AuxTable = 5

            ConvertFile(currInst)
            CheckAndRotate(currInst, "J")
            JUDGE.Value = GetBCJudgeAmount(currInst)
            JUDGE.CaseNo = GetBCJudgeCase(currInst)

            JUDGE.AddToDatabase()
            JUDGE = Nothing
            Return True
        End If

        If s.Contains("VENDORS LIEN") Then
            ShowStatus("a vendor's lien..")
            currInst = Str(inst)
            V = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Vendors Lien")
            V.ProcessDocument()
            V.TableName = "V"
            V.AuxTable = 2
            V.DownPayment = FormatNumber(V.Value, 0) + "/" + FormatNumber(V.DownPayment, 0)

            V.AddToDatabase()
            V = Nothing
            Return True
        End If
        If s.Contains("RELEASE") Then 'three types
            ShowStatus("a release..")
            currInst = Str(inst)
            REL = New RDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Release")
            REL.ProcessDocument()
            REL.AuxTable = 3
            If REL.Grantor.ToUpper.Contains("SEWER") Then
                Return True
            End If
            ConvertFile(currInst, REL.TableName)
            Select Case REL.TableName
                Case "UR"
                    REL.PrevInst = GetUSRelPrevInst(currInst)
                Case "SR"
                    REL.PrevInst = GetALTaxRelPrevInst(currInst)
                Case "BR"
                    REL.PrevInst = GetBCTaxRelPrevInst(currInst)
                Case "JR"
                    REL.PrevInst = GetBCJudgePrevInst(currInst)
            End Select
            REL.AddToDatabaseRel()
            REL = Nothing
            Return True
        End If
        If s.Contains("TAX LIEN") Then 'two types
            ShowStatus("a tax lien..")
            currInst = Str(inst)
            TLien = New TaxLienDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Tax Lien")
            currInst = Str(inst)
            TLien.ProcessDocument()
            ConvertFile(currInst, TLien.TableName)
            Select Case TLien.TableName
                Case "US"
                    TLien.KindTax = GetUSKindTax(currInst)
                    TLien.Value = GetUSTaxAmount(currInst)
                    TLien.Address = GetUSTaxAddress(currInst)
                Case "ST"
                    TLien.Value = GetALTaxAmount(currInst)
                    TLien.KindTax = GetALKindTax(currInst)
                    TLien.Address = GetALTaxAddress(currInst)
                Case "BT"
                    TLien.Address = GetBCTaxAddress(currInst)
                    TLien.KindTax = GetBCKindTax(currInst)
                    TLien.Value = GetBCTaxAmount(currInst)
            End Select
            TLien.AuxTable = 3
            TLien.AddToDatabaseTaxLien()
            TLien = Nothing
            Return True
        End If
        If s.Contains("LIEN") Then
            ShowStatus("a lien..")
            currInst = Str(inst)
            LI = New LIDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Lien")
            LI.ProcessDocument()
            LI.AuxTable = 3
            LI.AddToDatabaseLien()
            LI = Nothing
            Return True
        End If
        If s.Contains("LIS PENDENS") Then 'three types
            ShowStatus("lis pendens..")
            currInst = Str(inst)
            LIS = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Lis Pendens")
            LIS.ProcessDocument()
            LIS.TableName = "LIS"
            LIS.AuxTable = 1
            LIS.AddToDatabase()
            LIS = Nothing
            Return True
        End If
        If s.Contains("DISSOLUTION") Then 'three types
            ShowStatus("a dissolution..")
            currInst = Str(inst)
            DI = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Dissolution")
            DI.ProcessDocument()
            DI.Desc = DI.Grantor
            DI.TableName = "DI"
            DI.AuxTable = 1
            DI.AddToDatabase()
            DI = Nothing
            Return True
        End If

        If s.Contains("BILL OF SALE") Then 'three types
            ShowStatus("a Fairhope bill of sale..")
            currInst = Str(inst)
            FB = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "FH Bill of Sale")
            FB.ProcessDocument()
            FB.TableName = "FB"
            FB.AuxTable = 1
            FB.AddToDatabase()
            FB = Nothing
            Return True
        End If

        If s.Contains("FST LEASE") Then 'three types
            ShowStatus("a Fairhope lease..")
            currInst = Str(inst)
            FL = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Fairhope Lease")
            FL.ProcessDocument()
            FL.TableName = "FL"
            FL.AuxTable = 1
            FL.AddToDatabase()
            FL = Nothing
            Return True
        End If

        If s.Contains("LEASE") Then 'three types
            ShowStatus("a lease..")
            currInst = Str(inst)
            LE = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Lease")
            LE.ProcessDocument()
            LE.TableName = "LE"
            LE.AuxTable = 1
            LE.AddToDatabase()
            LE = Nothing
            Return True
        End If

        If s.Contains("ORDER") Then 'three types
            ShowStatus("an order..")
            currInst = Str(inst)
            ORD = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Order")
            ORD.ProcessDocument()
            ORD.TableName = "ORD"
            ORD.AuxTable = 1
            ORD.AddToDatabase()
            ORD = Nothing
            Return True
        End If

        If s.Contains("DECLARATION OF CONDO") Then 'three types
            ShowStatus("a declaration..")
            currInst = Str(inst)
            DECL = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Declaration")
            DECL.ProcessDocument()
            DECL.TableName = "DECL"
            DECL.AuxTable = 1
            DECL.AddToDatabase()
            DECL = Nothing
            Return True
        End If

        If s.Contains("ORDINANCE") Then 'three types
            ShowStatus("an ordinance...")
            currInst = Str(inst)
            ORDI = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Ordinance")
            ORDI.ProcessDocument()
            ORDI.TableName = "ORDI"
            ORDI.AuxTable = 1
            ORDI.AddToDatabase()
            ORDI = Nothing
            Return True
        End If

        If s.Contains("NOTICE") Then 'three types
            ShowStatus("an other..")
            currInst = Str(inst)
            O = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "Notice")
            O.ProcessDocument()
            O.TableName = "O"
            O.AuxTable = 1
            O.AddToDatabase()
            O = Nothing
            Return True
        End If

        If s.Contains("LICENSE") Then 'three types
            ShowStatus("a license..")
            currInst = Str(inst)
            LIC = New MasterDoc(BSDocConstants.bsDocType.bsDeltaDoc, currInst, "License")
            LIC.ProcessDocument()
            LIC.TableName = "LIC"
            LIC.AuxTable = 1
            LIC.AddToDatabase()
            LIC = Nothing
            Return True
        End If


        If s.Contains("MORTGAGE AMENDMENT") Then
            Return True
        End If
        If s.Contains("TRANSFER") Then
            Return True
        End If
        If s.Contains("ASSIGNMENT") Then
            Return True
        End If
        If s.Contains("AFFIDAVIT") Then
            Return True
        End If
        If s.Contains("CORRECTION MORTGAGE") Then
            Return True
        End If
        If s.Contains("RIGHT OF WAY DEED") Then
            Return True
        End If
        If s.Contains("DEATH CERTIFICATE") Then
            Return True
        End If
        If s.Contains("PARTIAL RELEASE") Then
            Return True
        End If
        If s.Contains("MODIFICATION") Then
            Return True
        End If
        If s.Contains("POWER OF ATTORNEY") Then
            Return True
        End If
        If s.Contains("CEMETERY DEED") Then
            Return True
        End If
        If s.Contains("SURVEY") Then
            Return True
        End If
        If s.Contains("UCC CROSS REFERENCE") Then
            Return True
        End If
        If s.Contains("SUBORDINATION") Then
            Return True
        End If
        If s.Contains("EASEMENT") Then
            Return True
        End If
        If s.Contains("NOTARY BOND") Then
            Return True
        End If
        If s.Contains("OATH OF OFFICE") Then
            Return True
        End If
        If s.Contains("COVENANTS") Then
            Return True
        End If
        If s.Contains("MISCELLANEOUS REAL") Then
            Return True
        End If
        If s.Contains("COVENANTS") Then
            Return True
        End If
        If s.Contains("RE-RECORD") Then
            Return True
        End If
        If s.Contains("INTENTIONAL BLANK") Then
            Return True
        End If
        If s.Contains("FAIR CAMPAIGN FORMS") Then
            Return True
        End If
        If s.Contains("AS BUILTS") Then
            Return True
        End If
        If s.Contains("MISCELLANEOUS MISC") Then
            Return True
        End If
        If s.Contains("WILL") Then
            Return True
        End If
        If s.Contains("BY LAWS") Then
            Return True
        End If
        If s.Contains("CORR CONDO DEED") Then
            Return True
        End If
        If s.Contains("RESOLUTION") Then
            Return True
        End If
        If s.Contains("AMENDMENT REAL PROP") Then
            Return True
        End If
        If s.Contains("RESTRICTIONS") Then
            Return True
        End If
        If s.Contains("DD214") Then
            Return True
        End If
        If s.Contains("STATEMENT OF CHANGE") Then
            Return True
        End If

    End Function

    Private Function CheckForUSTaxLien(ByVal fName As String) As Boolean
        Dim reader As New StreamReader(fName)
        Dim s As String
        Dim isState As Boolean = False
        Dim isRel As Boolean = False

        s = reader.ReadLine

        While Not reader.EndOfStream
            s = reader.ReadLine
            If s.Contains("Grantor:") Then
                s = reader.ReadLine
                s = reader.ReadLine
                If s.Contains("UNITED STATES OF AMERICA") Then
                    isState = True
                End If
            End If
            If s.Contains("Document Type:") Then
                s = reader.ReadLine
                s = reader.ReadLine
                If s.Contains("RELEASE") Then
                    isRel = True
                End If
            End If
        End While
        If isState And Not isRel Then
            Return True
        End If
        Return False
    End Function

    Private Function CheckForALTaxLien(ByVal fName As String) As Boolean
        Dim reader As New StreamReader(fName)
        Dim s As String
        Dim isState As Boolean = False
        Dim isRel As Boolean = False

        s = reader.ReadLine

        While Not reader.EndOfStream
            s = reader.ReadLine
            If s.Contains("STATE OF ALABAMA DEPARTMENT OF") Then
                isState = True
            End If

            If s.Contains(origInst) Then
                s = reader.ReadLine
                s = reader.ReadLine
                If s.Contains("RELEASE") Then
                    isRel = True
                End If
            End If
        End While
        If isState And Not isRel Then
            Return True
        End If
        Return False
    End Function
    Private Function CheckForForeclosureDeed(ByVal fName As String) As Boolean
        Dim reader As New StreamReader(fName)
        Dim s As String
        Dim isState As Boolean = False
        Dim isRel As Boolean = False

        s = reader.ReadLine

        While Not reader.EndOfStream
            s = reader.ReadLine
            If s.Contains("Grantor:") Then
                s = reader.ReadLine
                s = reader.ReadLine
                If s.Contains("STATE OF ALABAMA DEPARTMENT OF") Then
                    isState = True
                End If
            End If
            If s.Contains("Document Type:") Then
                s = reader.ReadLine
                s = reader.ReadLine
                If s.Contains("RELEASE") Then
                    isRel = True
                End If
            End If
        End While
        If isState And Not isRel Then
            Return True
        End If
        Return False
    End Function

    Private Sub CheckForUpdate()
        Dim myVersion As String = My.Application.Info.Version.ToString
        Dim currVersion As String = Trim(dg.GetScalarString("SELECT currVersion FROM currVersions WHERE appName='BaldDocParser'"))
        If currVersion <> myVersion Then
            MsgBox("Doc Parser needs to be updated - YOU MUST RESTART THE APPLICATION AFTER UPDATE.")
            System.Diagnostics.Process.Start("http://www.thebluesheetonline.com/installs/NewBaldwinParser.msi")
        End If
    End Sub



    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Close()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        isAborted = True
    End Sub


    Private Function ValidateDupes(ByVal cid As Integer) As Boolean
        If tbStartInst.Text = "" Then Return False
        If cid = 0 Then
            If dg.HasData("SELECT * FROM instrumentmasterflat WHERE InstrumentNo BETWEEN " + Me.tbStartInst.Text + " AND " + Me.tbEndInst.Text) Then
                If MsgBox("Instrument range has existing Instruments in it - delete and reparse?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    dg.RunCommand("DELETE FROM instrumentmasterflat WHERE InstrumentNo BETWEEN '" + Me.tbStartInst.Text + "' AND '" + Me.tbEndInst.Text + "'")
                    Return True
                Else
                    Return False
                End If
            End If
            Return True
        End If
        dg.Close()
        Return False
    End Function

    Private Sub DeleteScanneddocs()
        Dim dir As New DirectoryInfo("c:\scanneddocs")
        Dim fl As FileInfo
        Dim fls() As FileInfo
        fls = dir.GetFiles("*.*")
        Try
            For Each fl In fls
                File.Delete(fl.FullName)
            Next
        Catch ex As Exception

        End Try

    End Sub


End Class


