using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;

namespace chessServer
{
    public partial class frmOperacion : Formulario
    {
        private Label[] lblCampo;
        private TextBox[] txbCampo;
        private ComboBox cbConn, cbPlay;
        private Label lblConn, lblPlay;
        private Button btnAceptar;
        private Button btnCancelar;
        private String uso;

        public frmOperacion(String r, int c, String u)
        {
            InitializeComponent();
            roll = r;
            uso = u;
            colums = c - 2;
            if (uso == "Buscar")
                for (int i = 0; i < 6; i++)
                    Sval[i] = "";
        }
        public frmOperacion(String r, int c, String[] v, String u)
        {
            InitializeComponent();
            roll = r;
            colums = c - 2;
            val = v;
            uso = u;
        }
        private void iniciaVentana()
        {
            int i;
            lblCampo = new Label[colums];
            txbCampo = new TextBox[colums];
            btnAceptar = new Button();
            btnCancelar = new Button();
            this.Controls.Add(btnAceptar);
            this.Controls.Add(btnCancelar);
            for (i = 0; i < colums; i++)
            {
                lblCampo[i] = new Label();
                this.Controls.Add(lblCampo[i]);
                lblCampo[i].AutoSize = true;
                lblCampo[i].Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                lblCampo[i].Location = new Point(10, i * 24 + 10);
                lblCampo[i].Name = "lblCampo" + i;
                lblCampo[i].Size = new Size(55, 13);
                txbCampo[i] = new TextBox();
                this.Controls.Add(txbCampo[i]);
                txbCampo[i].Location = new Point(73, i * 24 + 10);
                txbCampo[i].Name = "txbCampo" + i;
                txbCampo[i].Size = new Size(100, 20);
                txbCampo[i].TabIndex = i + 1;
            }
            if (roll == "usuarios")
            {
                this.Text = uso + " Usuarios";
                lblCampo[0].Text = "Usuario";
                txbCampo[0].MaxLength = 20;
                lblCampo[1].Text = "Nombre";
                txbCampo[1].MaxLength = 40;
                lblCampo[2].Text = "PassWord";
                txbCampo[2].MaxLength = 20;
                if (uso == "Buscar")
                {
                    lblConn = new Label();
                    this.Controls.Add(lblConn);
                    lblConn.AutoSize = true;
                    lblConn.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                    lblConn.Location = new Point(10, i * 24 + 10);
                    lblConn.Name = "lblConn";
                    lblConn.Size = new Size(55, 13);
                    lblConn.Text = "Conn";
                    cbConn = new ComboBox();
                    this.Controls.Add(cbConn);
                    cbConn.Location = new Point(73, i * 24 + 10);
                    cbConn.Name = "cbConn";
                    cbConn.Size = new Size(100, 20);
                    cbConn.TabIndex = i + 1;
                    cbConn.FormattingEnabled = true;
                    cbConn.Items.AddRange(new object[] { "True", "False" });
                    i++;
                    lblPlay = new Label();
                    this.Controls.Add(lblPlay);
                    lblPlay.AutoSize = true;
                    lblPlay.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
                    lblPlay.Location = new Point(10, i * 24 + 10);
                    lblPlay.Name = "lblPlay";
                    lblPlay.Size = new Size(55, 13);
                    lblPlay.Text = "Playing";
                    cbPlay = new ComboBox();
                    this.Controls.Add(cbPlay);
                    cbPlay.Location = new Point(73, i * 24 + 10);
                    cbPlay.Name = "cbConn";
                    cbPlay.Size = new Size(100, 20);
                    cbPlay.TabIndex = i + 1;
                    cbPlay.FormattingEnabled = true;
                    cbPlay.Items.AddRange(new object[] { "True", "False" });
                    i++;
                }
            }
            if(uso == "Modificar")
                for (i = 0; i < colums; i++)
                    txbCampo[i].Text = val[i];
            btnAceptar.Location = new Point(13, i * 24 + 15);
            btnAceptar.Name = "btnAceptar";
            btnAceptar.Size = new Size(75, 23);
            btnAceptar.TabIndex = 5;
            btnAceptar.Text = "Aceptar";
            btnAceptar.UseVisualStyleBackColor = true;
            btnAceptar.Click += new EventHandler(this.btnAceptar_Click);

            btnCancelar.Location = new Point(101, i * 24 + 15);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 6;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += new EventHandler(this.btnCancelar_Click);
            i++;
            this.ClientSize = new Size(188, i * 24 + 20);
        }
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (uso == "Agregar" || uso == "Modificar")
            {
                if (txbCampo[0].Text != "" && txbCampo[1].Text != "" && txbCampo[2].Text != "")
                {
                    if (roll == "usuarios")
                    {
                        try
                        {
                            My_SQL c = new My_SQL();
                            int nq;
                            if (uso == "Agregar")
                                nq = c.hazNoConsulta("insert into usuarios values('" + txbCampo[0].Text + "','" + txbCampo[1].Text + "','" + txbCampo[2].Text + "','0','0')");
                            if (uso == "Modificar")
                                nq = c.hazNoConsulta("update usuarios set user='" + txbCampo[0].Text + "',nombre='" + txbCampo[1].Text + "',pssw='" + txbCampo[2].Text + "' where user='" + val[0] + "'");
                            this.Close();
                        }
                        catch (Exception exc1)
                        {
                            MessageBox.Show(exc1.ToString());
                        }
                    }
                }
                else
                    MessageBox.Show("No se admiten campos vacios");
            }
            if (uso == "Buscar")
            {
                if (txbCampo[0].Text != "")
                    Sval[0] = txbCampo[0].Text.ToUpper();
                if (roll == "usuarios")
                {
                    if (txbCampo[1].Text != "")
                        Sval[1] = txbCampo[1].Text;
                    if (txbCampo[2].Text != "")
                        Sval[2] = txbCampo[2].Text;
                    if (cbConn.Text != "")
                    {
                        try
                        {
                            bool s1 = bool.Parse(cbConn.Text);
                            if(s1)
                                Sval[3] = "1";
                            else
                                Sval[3] = "0";
                        }
                        catch (FormatException formato31)
                        {
                            MessageBox.Show("Connected es de Tipo Bool " + formato31.ToString());
                        }
                        catch (Exception exc31)
                        {
                            MessageBox.Show(exc31.ToString());
                        }
                    }
                    if (cbPlay.Text != "")
                    {
                        try
                        {
                            bool s2 = bool.Parse(cbPlay.Text);
                            if (s2)
                                Sval[4] = "1";
                            else
                                Sval[4] = "0";
                        }
                        catch (FormatException formato310)
                        {
                            MessageBox.Show("Connected es de Tipo Bool " + formato310.ToString());
                        }
                        catch (Exception exc310)
                        {
                            MessageBox.Show(exc310.ToString());
                        }
                    }
                }
                this.Close();
            }
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmOperacion_Load(object sender, EventArgs e)
        {
            iniciaVentana();
        }
    }
}
