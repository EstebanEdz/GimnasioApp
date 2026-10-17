using System.Data;

namespace App.UI;

public partial class FormVistaPreviaImportacion : Form
{
    private System.Windows.Forms.DataGridView dgvPreview;
    private System.Windows.Forms.Label lblResumen;
    private System.Windows.Forms.TextBox txtErrores;
    private System.Windows.Forms.Button btnConfirmar;
    private System.Windows.Forms.Button btnCancelar;

    public FormVistaPreviaImportacion(DataTable vistaPrevia, List<string> erroresEstructura)
    {
        InitializeComponent();

        this.dgvPreview = new System.Windows.Forms.DataGridView();
        this.lblResumen = new System.Windows.Forms.Label();
        this.txtErrores = new System.Windows.Forms.TextBox();
        this.btnConfirmar = new System.Windows.Forms.Button();
        this.btnCancelar = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
        this.SuspendLayout();

        this.lblResumen.AutoSize = true;
        this.lblResumen.Location = new System.Drawing.Point(15, 15);
        this.lblResumen.Text = "Filas validas: 0    Filas con error: 0";

        this.dgvPreview.Location = new System.Drawing.Point(15, 40);
        this.dgvPreview.Size = new System.Drawing.Size(760, 300);
        this.dgvPreview.ReadOnly = true;
        this.dgvPreview.AllowUserToAddRows = false;
        this.dgvPreview.RowHeadersVisible = false;
        this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        this.txtErrores.Location = new System.Drawing.Point(15, 350);
        this.txtErrores.Size = new System.Drawing.Size(760, 90);
        this.txtErrores.Multiline = true;
        this.txtErrores.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtErrores.ReadOnly = true;
        this.txtErrores.ForeColor = System.Drawing.Color.Firebrick;

        this.btnConfirmar.Location = new System.Drawing.Point(600, 450);
        this.btnConfirmar.Size = new System.Drawing.Size(85, 30);
        this.btnConfirmar.Text = "Confirmar";
        this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);

        this.btnCancelar.Location = new System.Drawing.Point(690, 450);
        this.btnCancelar.Size = new System.Drawing.Size(85, 30);
        this.btnCancelar.Text = "Cancelar";
        this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(790, 495);
        this.Controls.Add(this.lblResumen);
        this.Controls.Add(this.dgvPreview);
        this.Controls.Add(this.txtErrores);
        this.Controls.Add(this.btnConfirmar);
        this.Controls.Add(this.btnCancelar);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "FormVistaPreviaImportacion";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Vista previa de importacion";
        ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    
        dgvPreview.DataSource = vistaPrevia;

        int validos = 0, invalidos = 0;
        foreach (DataRow fila in vistaPrevia.Rows)
        {
            if (fila.Table.Columns.Contains("Valido") && (bool)fila["Valido"]) validos++; else invalidos++;
        }

        lblResumen.Text = $"Filas validas: {validos}    Filas con error: {invalidos}";

        if (erroresEstructura.Count > 0)
        {
            txtErrores.Text = string.Join(Environment.NewLine, erroresEstructura);
            txtErrores.Visible = true;
        }
        else
        {
            txtErrores.Visible = false;
        }
    }

    private void btnConfirmar_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
