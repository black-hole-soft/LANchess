using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Data.Odbc;

namespace Ajedrez
{
    public partial class frmPrincipal : frmConsultas
    {
        private Button[] btn;
        private DataGridView dgv;
        private int sel = -1, btns, op = -1;
        Thread hiloReg = null, hiloUDP = null, hiloVerifica = null, hiloLoseOP = null, hiloAquireOP = null;
        HiloComsUDP comsUDP = null;
        String oponent = "";
        frmAjedrez tablero = null;

        public frmPrincipal()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            frmLogin log = new frmLogin();
            if(log!=null)
                log.ShowDialog();
            if (conectado)
                limpiarVentana();
        }
        private void btnRegistro_Click(object sender, EventArgs e)
        {
            frmRegistro reg = new frmRegistro();
            if (reg != null)
                reg.ShowDialog();
            if (conectado)
                limpiarVentana();
        }
        private void iniciarCliente()
        {
            try
            {
                roll = "usuarios";
                usr = Susr;
                colums = 4;
                inicializaVentana();
                SetupDataGridView();
                PopulateDataGridView("");
                actualiza();
                cxnServidor = new TcpClient("pcgera", 5432);
                hcoms = new HiloComs(usr, dgv, cxnServidor);
                hcs = new Thread(new ThreadStart(hcoms.cnnRetadores));
                hcs.Start();
                hcoms.accion = "LISTO@AJUGAR";
                hiloReg = new Thread(new ThreadStart(hilo_LocalizaNumero));
                hiloReg.Start();
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.ToString());
                this.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                this.Close();
            }
        }
        private void inicializaVentana()
        {
            if (roll == "usuarios")
            {
                btns = 3;
                nombre = "Usuarios";
                cam = new String[4];
                val = new String[4];
                cam[0] = "user";
                cam[1] = "nombre";
                cam[2] = "pssw";
                cam[3] = "connected";
            }
            btn = new Button[btns];
            for (int i = 0; i < btns; i++)
            {
                btn[i] = new Button();
                btn[i].Location = new Point(55 * i + 12, 334);
                btn[i].Size = new Size(55, 23);
                btn[i].TabIndex = i;
                btn[i].UseVisualStyleBackColor = true;
                this.Controls.Add(btn[i]);
            }
            if (roll == "usuarios")
            {
                btn[0].Text = "Buscar";
                btn[1].Text = "Retar";
                btn[2].Text = "Actualizar";
                btn[0].Click += new EventHandler(this.btnBuscar_Click);
                btn[1].Click += new EventHandler(this.btnRetar_Click);
                btn[2].Click += new EventHandler(this.btnActualizar_Click);
            }
        }
        private void SetupDataGridView()
        {
            dgv = new DataGridView();
            this.Controls.Add(dgv);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToOrderColumns = true;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Location = new Point(13, 13);
            dgv.ReadOnly = true;
            dgv.TabIndex = 6;
            dgv.ColumnCount = colums;
            dgv.CellClick += new DataGridViewCellEventHandler(this.dgv_CellClick);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
            dgv.GridColor = Color.Black;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            this.Text = nombre;
            if (roll == "usuarios")
            {
                this.ClientSize = new Size(442, 369);
                dgv.Size = new Size(422, 314);
                dgv.Columns[0].Name = "Usuario";
                dgv.Columns[1].Name = "Nombre";
                dgv.Columns[2].Name = "Conectado";
                dgv.Columns[3].Name = "Jugando";
            }
        }
        private void PopulateDataGridView(String cond)
        {
            int i;
            String[] row = new String[colums];
            My_SQL mysql = new My_SQL();
            OdbcDataReader res = mysql.hazConsulta("select user,nombre,connected,playing from usuarios " + cond);
            if (res.HasRows)
            {
                while (res.Read())
                {
                    for (i = 0; i < colums; i++)
                        row[i] = res.GetString(i);
                    if(row[0] != usr)
                        dgv.Rows.Add(row);
                }
            }
        }
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView ob = (DataGridView)sender;
                for (int i = 0; i < colums; i++)
                    val[i] = (String)ob.Rows[e.RowIndex].Cells[i].Value;
                sel = e.RowIndex;
            }
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            frmOperacion b = new frmOperacion(roll, colums, "Buscar");
            b.ShowDialog();
            String[] c = new String[5];
            for (int i = 0; i < 5; i++)
                c[i] = "";
            if (Sval[0] != "")
            {
                c[0] = " where";
                c[1] = " " + cam[0] + " = '" + Sval[0] + "'";
            }
            if (Sval[1] != "")
            {
                if (c[1] != "")
                    c[2] = " and ";
                else
                    c[0] = " where";
                c[2] += " " + cam[1] + " = '" + Sval[1] + "'";
            }
            if (Sval[2] != "")
            {
                if (c[1] != "" || c[2] != "")
                    c[3] = " and ";
                else
                    c[0] = " where";
                c[3] += " " + cam[2] + " = '" + Sval[2] + "'";
            }
            if (Sval[3] != "")
            {
                if (c[1] != "" || c[2] != "" || c[3] != "")
                    c[4] = " and ";
                else
                    c[0] = " where";
                c[4] += " " + cam[3] + " = '" + Sval[3] + "'";
            }
            dgv.Hide();
            SetupDataGridView();
            PopulateDataGridView(c[0] + c[1] + c[2] + c[3] + c[4]);
            actualiza();
        }
        private void btnRetar_Click(object sender, EventArgs e)
        {
            if (sel >= 0)
            {
                My_SQL mysql = new My_SQL();
                OdbcDataReader res = mysql.hazConsulta("select connected,playing from usuarios where user='" + (String)dgv.Rows[sel].Cells[0].Value + "'");
                if (res.HasRows)
                {
                    if (res.Read())
                    {
                        if (res.GetString(0) == "1" && res.GetString(1) == "1")
                            MessageBox.Show((String)dgv.Rows[sel].Cells[0].Value + " ya se encuentra jugando");
                        else
                        {
                            if (res.GetString(0) == "0")
                                MessageBox.Show((String)dgv.Rows[sel].Cells[0].Value + " no se encuentra conectado");
                            else
                            {
                                hcoms.accion = "RETAR@" + (String)dgv.Rows[sel].Cells[0].Value;
                            }
                        }
                    }
                }
            }
        }
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            dgv.Hide();
            SetupDataGridView();
            PopulateDataGridView("");
            actualiza(); 
        }
        private void actualiza()
        {
            if (dgv.Rows.Count > 0)
            {
                sel = dgv.SelectedRows[0].Index;
                for (int i = 0; i < colums; i++)
                    val[i] = (String)dgv.Rows[sel].Cells[i].Value;
            }
            else
                sel = -1;
        }
        private void limpiarVentana()
        {
            btnLogin.Hide();
            btnLogin = null;
            btnRegistro.Hide();
            btnRegistro = null;
            iniciarCliente();
            conectado = false;
        }
        delegate void delCierraTablero(frmAjedrez ch);
        void CierraTablero(frmAjedrez ch)
        {
            if (ch.InvokeRequired)
                ch.Invoke(new delCierraTablero(CierraTablero), ch);
            else
            {
                ch.Close();
                ch = null;
            }
        }
        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tablero != null)
                CierraTablero(tablero);
            if (hiloAquireOP != null)
                hiloAquireOP.Abort();
            if (hiloLoseOP != null)
                hiloLoseOP.Abort();
            if (hiloVerifica != null)
                hiloVerifica.Abort();
            if (hiloReg != null)
                hiloReg.Abort();
            if (hcs != null)
                hcs.Abort();
            if (hcoms != null)
            {
                hcoms.cierraCxnS();
                hcoms = null;
            }
            if (comsUDP != null)
            {
                while (comsUDP.serverActive)
                    Thread.Sleep(10);
                if (hiloUDP != null)
                    hiloUDP.Abort();
                if (comsUDP != null)
                {
                    comsUDP.closeClient();
                    comsUDP = null;
                }
            }
        }
        private void hilo_LocalizaNumero()
        {
            while (hcoms.nCte == -1)
            {
                Thread.Sleep(10);
            }
            nCte = hcoms.nCte;
            comsUDP = new HiloComsUDP(201 + nCte);
            hiloUDP = new Thread(new ThreadStart(comsUDP.startplayer));
            hiloUDP.Start();
            comsUDP.enviaMensaje("identifica@" + usr + "@" + nCte.ToString());
            hiloVerifica = new Thread(new ThreadStart(hilo_VerificaServidorActivo));
            hiloVerifica.Start();
            hiloLoseOP = new Thread(new ThreadStart(hilo_VerificaLoseOP));
            hiloLoseOP.Start();
            hiloAquireOP = new Thread(new ThreadStart(hilo_VerificaAquireOP));
            hiloAquireOP.Start();
        }
        private void hilo_VerificaServidorActivo()
        {
            while (comsUDP.serverActive)
            {
                Thread.Sleep(10);
            }
            if (tablero != null)
                CierraTablero(tablero);
            if (comsUDP != null)
            {
                while (comsUDP.serverActive)
                    Thread.Sleep(10);
                if (hiloUDP != null)
                    hiloUDP.Abort();
                if (comsUDP != null)
                {
                    comsUDP.closeClient();
                    comsUDP = null;
                }
            }
            if (hcs != null)
                hcs.Abort();
            if (hcoms != null)
            {
                hcoms.cierraCxnS();
                hcoms = null;
            }
            if (hiloReg != null)
                hiloReg.Abort();
            if (hiloLoseOP != null)
                hiloLoseOP.Abort();
            if (hiloAquireOP != null)
                hiloAquireOP.Abort();
            MessageBox.Show("El Servidor Cerró la Conexion");
        }
        private void hilo_VerificaLoseOP()
        {
            while (true)
            {
                while (!comsUDP.loseOP)
                {
                    oponent = comsUDP.oponent;
                    op = comsUDP.nOp;
                    Thread.Sleep(10);
                }
                comsUDP.loseOP = false;
                hcoms.accion = "RENEW@LOSEOPONENT@" + oponent + "@" + op.ToString();
                oponent = "";
                op = -1;
            }
        }
        delegate void delEnableBTN(Button b, bool e);
        void EnableBTN(Button b, bool e)
        {
            if (b.InvokeRequired)
                b.Invoke(new delEnableBTN(EnableBTN), b, e);
            else 
                b.Enabled = e;
        }
        private void hilo_VerificaAquireOP()
        {
            bool aquire = false;
            while (true)
            {
                if (comsUDP.oponent != "" && aquire == false)
                {
                    aquire = true;
                    oponent = comsUDP.oponent;
                    op = comsUDP.nOp;
                    hcoms.accion = "AQUIRE_OP@" + oponent + "@" + op.ToString();
                    EnableBTN(btn[1], false);
                    tablero = new frmAjedrez(nCte, usr, comsUDP);
                    tablero.ShowDialog();
                    tablero = null;
                }
                else
                {
                    if (comsUDP.oponent == "" && aquire == true)
                    {
                        aquire = false;
                        EnableBTN(btn[1], true);
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
