﻿Public Class FrmManagement
    Public SQL As New SQLControl
    Dim selection As Integer
    Dim oldID As String
    Dim oldType As String
    Dim oldSize As String
    Dim oldColor As String
    Dim oldStts As String
    Private Sub FrmManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'FrmSearch.Show()
        FrmSeachSimple.Show()
        LblVer.Text = String.Format("Ver: {0}", Application.ProductVersion)
        ResizeAndCenter()
        SetupDGV()
        LoadDatatoDGV()
        Me.Show()
        dgvFeeder.Focus()
        'FrmSearch.Hide()
        FrmSeachSimple.Hide()
    End Sub

    Private Sub FrmManagement_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        FrmMain.Show()
    End Sub
    Private Sub ResizeAndCenter()
        LblVer.Left = (LblVer.Parent.Width \ 2) - (LblVer.Width \ 2)
        LblVer.Top = (LblVer.Parent.Height \ 2) - (LblVer.Height \ 2) + 410

        ImgHero.Left = (ImgHero.Parent.Width \ 2) - (ImgHero.Width \ 2)
        'ImgHero.Top = (ImgHero.Parent.Height \ 2) - (ImgHero.Height \ 2) + 10
        btnBack.Left = (btnBack.Parent.Width \ 2) - (btnBack.Width \ 2)
        lblDetails.Left = (lblDetails.Parent.Width \ 2) - (lblDetails.Width \ 2)

    End Sub

    Private Sub SetupDGV()
        With dgvFeeder
            .Left = (.Parent.Width \ 2) - (.Width \ 2)
            .RowHeadersVisible = False
            .EnableHeadersVisualStyles = False
            .ColumnCount = 8
            .AllowUserToResizeRows = False
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            Dim columns As String() = {"No.", "Feeder ID", "Feeder Type", "Gear Size", "Colour Code", "Status", "Update Time", "Updater"}
            Dim widths As Integer() = {40, 100, 100, 85, 85, 90, 100, 82}

            For i As Integer = 0 To columns.Length - 1
                .Columns(i).Name = columns(i)
            Next

            For i As Integer = 0 To widths.Length - 1
                .Columns(i).Width = widths(i)
            Next
        End With
    End Sub

    Private Sub LoadDatatoDGV()
        dgvFeeder.Rows.Clear()

        SQL.ExecQuery("SELECT * FROM FeederManagement ORDER BY FeederNumber")
        If SQL.HasException(True) Then Exit Sub

        Dim status As String

        If SQL.RecordCount > 0 Then
            For i As Integer = 1 To SQL.DBDT.Rows.Count
                Dim colorCode As String = SQL.DBDT.Rows(i - 1)("FColorCode").ToString()
                Dim colorFeeder As Color = ColorTranslator.FromHtml(colorCode)
                Dim statusSQL As Boolean = SQL.DBDT.Rows(i - 1)("FStatus")

                If statusSQL Then
                    status = "GOOD"
                Else
                    status = "NO GOOD"
                End If

                dgvFeeder.Rows.Add(New Object() {i.ToString + ".", SQL.DBDT.Rows(i - 1)("FeederNumber"), SQL.DBDT.Rows(i - 1)("FeederType"),
                                          SQL.DBDT.Rows(i - 1)("FGearSize"), colorCode,
                                          status, SQL.DBDT.Rows(i - 1)("UpdateTime"), SQL.DBDT.Rows(i - 1)("Updater")})

                If status = "NO GOOD" Then
                    For j As Integer = 1 To dgvFeeder.Columns.Count
                        dgvFeeder.Rows(dgvFeeder.Rows.Count - 1).Cells(j - 1).Style.BackColor = Color.FromArgb(255, 192, 192)
                    Next
                End If
                dgvFeeder.Rows(dgvFeeder.Rows.Count - 1).Cells(4).Style.BackColor = colorFeeder
            Next
        Else
            ClearInput()
        End If

    End Sub

    Private Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Me.Close()
        FrmMain.Show()
    End Sub

    Private Function GetSelectedIndex(comboBox As ComboBox, targetValue As String) As Integer
        For i As Integer = 0 To comboBox.Items.Count - 1
            If comboBox.Items(i).ToString() = targetValue Then
                Return i
            End If
        Next

        Return -1
    End Function

    Private Sub dgvFeeder_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvFeeder.CellClick
        txtFeederID.Text = dgvFeeder.CurrentRow.Cells(1).Value.ToString()
        cbxType.SelectedIndex = GetSelectedIndex(cbxType, dgvFeeder.CurrentRow.Cells(2).Value.ToString())
        cbxSize.SelectedIndex = GetSelectedIndex(cbxSize, dgvFeeder.CurrentRow.Cells(3).Value.ToString())
        cbxColor.SelectedIndex = GetSelectedIndex(cbxColor, dgvFeeder.CurrentRow.Cells(4).Value.ToString())
        cbxStatus.SelectedIndex = GetSelectedIndex(cbxStatus, dgvFeeder.CurrentRow.Cells(5).Value.ToString())
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadDatatoDGV()
    End Sub

    Private Sub ClearInput()
        txtFeederID.Enabled = True
        txtFeederID.Clear()
        txtFeederID.Text = ""
        cbxType.SelectedIndex = 0
        cbxSize.SelectedIndex = 0
        cbxColor.SelectedIndex = 0
        cbxStatus.SelectedIndex = 0
        txtFeederID.Enabled = False
    End Sub

    Private Sub EnableInput()
        txtFeederID.Enabled = True
        cbxType.Enabled = True
        cbxSize.Enabled = True
        cbxColor.Enabled = True
        cbxStatus.Enabled = True
        txtEmployeeID.Enabled = True
        btnCancel.Enabled = True
    End Sub
    Private Sub DisableInput()
        txtFeederID.Enabled = False
        cbxType.Enabled = False
        cbxSize.Enabled = False
        cbxColor.Enabled = False
        cbxStatus.Enabled = False
        txtEmployeeID.Enabled = False
        btnCancel.Enabled = False
    End Sub

    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        selection = 1
        ClearInput()
        dgvFeeder.Enabled = False
        EnableInput()
        txtFeederID.Focus()
        btnCreate.Enabled = False
        btnUpdate.Enabled = False
        btnDelete.Enabled = False
        btnConfirm.Enabled = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        selection = 0
        btnCreate.Enabled = True
        btnUpdate.Enabled = True
        btnDelete.Enabled = True
        If dgvFeeder.Rows.Count > 0 Then
            dgvFeeder_CellClick(dgvFeeder, New DataGridViewCellEventArgs(0, 0))
        End If

        DisableInput()
        dgvFeeder.Enabled = True
        btnConfirm.Enabled = False
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        selection = 2
        oldType = cbxType.Text.Trim
        oldSize = cbxSize.Text.Trim
        oldColor = cbxColor.Text.Trim
        oldStts = cbxStatus.Text.Trim
        If dgvFeeder.SelectedRows.Count = 0 Then
            MessageBox.Show("A selection is required.", "Selection required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            dgvFeeder.Focus()
            Exit Sub
        End If

        EnableInput()
        txtFeederID.Enabled = False
        dgvFeeder.Enabled = False
        txtEmployeeID.Focus()
        btnUpdate.Enabled = False
        btnCreate.Enabled = False
        btnDelete.Enabled = False
        btnConfirm.Enabled = True
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        If txtEmployeeID.Text.Length < 6 Then
            MessageBox.Show("Incorrect Empolyee ID. Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            txtEmployeeID.Focus()
            txtEmployeeID.SelectAll()
            Exit Sub
        End If

        If selection = 1 Then
            If txtFeederID.Text = "" Then
                MessageBox.Show("The Feeder ID information is required.", "Feeder ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                txtFeederID.Focus()
            ElseIf cbxType.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Type information is required.", "Feeder Type", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxType.Focus()
                cbxType.DroppedDown = True
            ElseIf cbxSize.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Size information is required.", "Feeder Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxSize.Focus()
                cbxSize.DroppedDown = True
            ElseIf cbxColor.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Colour information is required.", "Feeder Colour", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxColor.Focus()
                cbxColor.DroppedDown = True
            ElseIf txtEmployeeID.Text = "" Then
                MessageBox.Show("Incorrect Empolyee ID. Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtEmployeeID.Focus()
            Else
                Dim chkdup As Boolean = Not CheckDupID(txtFeederID.Text.Trim)
                If chkdup Then
                    Dim stts As Boolean = If(cbxStatus.SelectedItem.ToString() = "GOOD", True, False)

                    SQL.AddParam("@id", txtFeederID.Text.Trim)
                    SQL.AddParam("@type", cbxType.Text.Trim)
                    SQL.AddParam("@size", cbxSize.Text.Trim)
                    SQL.AddParam("@color", cbxColor.Text.Trim)
                    SQL.AddParam("@status", stts)
                    SQL.AddParam("@uid", txtEmployeeID.Text.Trim)

                    SQL.ExecQuery("INSERT INTO FeederManagement (FeederNumber, FeederType, FGearSize, FColorCode, FStatus, UpdateTime, Updater) VALUES(@id, @type, @size, @color, @status, GETDATE(), @uid);")
                    If SQL.HasException(True) Then Exit Sub

                    MessageBox.Show("Feeder ID '" + txtFeederID.Text + "' has been added.", "Feeder Added", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    dgvFeeder.Enabled = True
                    oldID = txtFeederID.Text.Trim
                    LoadDatatoDGV()

                    DisableInput()
                    btnConfirm.Enabled = False
                    btnCreate.Enabled = True
                    btnUpdate.Enabled = True
                    btnDelete.Enabled = True
                    Highlight(oldID)
                Else
                    MessageBox.Show("Feeder ID '" + txtFeederID.Text + "' already exists in the database.", "Duplicate Feeder ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    txtFeederID.Focus()
                    txtFeederID.SelectAll()
                End If
            End If
        ElseIf selection = 2 Then
            If txtFeederID.Text = "" Then
                MessageBox.Show("The Feeder ID information is required.", "Feeder ID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                txtFeederID.Focus()
            ElseIf oldType = cbxType.Text.Trim And oldSize = cbxSize.Text.Trim And oldColor = cbxColor.Text.Trim And oldStts = cbxStatus.Text.Trim Then
                MessageBox.Show("No changes were made to the Feeder information." & vbCrLf & "Updating is not required.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxType.DroppedDown = True
            ElseIf cbxType.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Type information is required.", "Feeder Type", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxType.Focus()
                cbxType.DroppedDown = True
            ElseIf cbxSize.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Size information is required.", "Feeder Size", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxSize.Focus()
                cbxSize.DroppedDown = True
            ElseIf cbxColor.SelectedIndex = 0 Then
                MessageBox.Show("The Feeder Colour information is required.", "Feeder Colour", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                cbxColor.Focus()
                cbxColor.DroppedDown = True
            ElseIf txtEmployeeID.Text = "" Then
                MessageBox.Show("Incorrect Empolyee ID. Please try again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtEmployeeID.Focus()
            Else
                Dim stts As Boolean = If(cbxStatus.SelectedItem.ToString() = "GOOD", True, False)

                SQL.AddParam("@id", txtFeederID.Text.Trim)
                SQL.AddParam("@type", cbxType.Text.Trim)
                SQL.AddParam("@size", cbxSize.Text.Trim)
                SQL.AddParam("@color", cbxColor.Text.Trim)
                SQL.AddParam("@status", stts)
                SQL.AddParam("@uid", txtEmployeeID.Text.Trim)

                SQL.ExecQuery("UPDATE FeederManagement SET FeederType = @type, FGearSize = @size, FColorCode = @color, FStatus = @status, UpdateTime = GETDATE(), Updater = @uid WHERE FeederNumber = @id;")
                If SQL.HasException(True) Then Exit Sub

                MessageBox.Show("Feeder ID '" + txtFeederID.Text + "' has been updated.", "Feeder Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)

                dgvFeeder.Enabled = True
                oldID = txtFeederID.Text.Trim

                LoadDatatoDGV()

                DisableInput()

                btnConfirm.Enabled = False
                btnCreate.Enabled = True
                btnUpdate.Enabled = True
                btnDelete.Enabled = True
                Highlight(oldID)
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvFeeder.SelectedRows.Count = 0 Then
            MessageBox.Show("A selection is required.", "Selection required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            dgvFeeder.Focus()
        Else
            If MessageBox.Show("Confirm to delete below feeder details?" & vbCrLf & vbCrLf & "Feeder ID: " & dgvFeeder.SelectedRows(0).Cells("Feeder ID").Value & vbCrLf & "Feeder Type: " & dgvFeeder.SelectedRows(0).Cells("Feeder Type").Value & vbCrLf & "Gear Size: " & dgvFeeder.SelectedRows(0).Cells("Gear Size").Value & vbCrLf & "Colour Code: " & dgvFeeder.SelectedRows(0).Cells("Colour Code").Value, "Delete Feeder Details", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                SQL.AddParam("@id", txtFeederID.Text)
                SQL.ExecQuery("DELETE FROM FeederManagement WHERE FeederNumber = @id")
                If SQL.HasException(True) Then Exit Sub

                LoadDatatoDGV()
            End If
        End If
    End Sub

    Private Function CheckDupID(FID As String) As Boolean
        SQL.AddParam("@id", FID)
        SQL.ExecQuery("SELECT * FROM FeederManagement WHERE FeederNumber = @id")
        Dim result As Boolean = False
        If SQL.RecordCount > 0 Then
            result = True
        End If
        Return result
    End Function

    Private Sub Highlight(FID As String)
        For Each row As DataGridViewRow In dgvFeeder.Rows
            If row.Cells(1).Value = FID Then
                dgvFeeder.ClearSelection()
                row.Selected = True

                txtFeederID.Text = row.Cells(1).Value.ToString()
                cbxType.SelectedIndex = GetSelectedIndex(cbxType, row.Cells(2).Value.ToString())
                cbxSize.SelectedIndex = GetSelectedIndex(cbxSize, row.Cells(3).Value.ToString())
                cbxColor.SelectedIndex = GetSelectedIndex(cbxColor, row.Cells(4).Value.ToString())
                cbxStatus.SelectedIndex = GetSelectedIndex(cbxStatus, row.Cells(5).Value.ToString())

                dgvFeeder.FirstDisplayedScrollingRowIndex = row.Index
                Exit For
            End If
        Next
    End Sub

    Private Sub dgvFeeder_SelectionChanged(sender As Object, e As EventArgs) Handles dgvFeeder.SelectionChanged
        If dgvFeeder.Rows.Count > 0 Then
            dgvFeeder_CellClick(dgvFeeder, New DataGridViewCellEventArgs(0, 0))
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        'FrmSearch.ShowDialog()
        FrmSeachSimple.ShowDialog()
    End Sub

    Private Sub dgvFeeder_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvFeeder.KeyDown
        If e.KeyCode = Keys.F AndAlso e.Control Then
            ' Ctrl+F is pressed
            btnSearch.PerformClick()
            e.Handled = True ' Set handled to True to prevent further processing of the key press event
        End If

        If e.KeyCode = Keys.R AndAlso e.Control Then
            ' Ctrl+F is pressed
            btnRefresh.PerformClick()
            e.Handled = True ' Set handled to True to prevent further processing of the key press event
        End If
    End Sub
End Class