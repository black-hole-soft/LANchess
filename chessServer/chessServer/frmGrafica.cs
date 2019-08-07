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
    public partial class frmGrafica : Formulario
    {
        private Button[] btn;
        private DataGridView dgv;
        private int sel = -1, btns;

        public frmGrafica(String r, int c)
        {
            InitializeComponent();
            roll = r;
            colums = c;
        }
        private void inicializaVentana()
        {
            if (roll == "usuarios")
            {
                btns = 5;
                nombre = "Usuarios";    
                cam = new String[5];
                val = new String[5];
                cam[0] = "user";
                cam[1] = "nombre";
                cam[2] = "pssw";
                cam[3] = "connected";
                cam[4] = "playing";
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
                btn[1].Text = "Agregar";
                btn[2].Text = "Modificar";
                btn[3].Text = "Eliminar";
                btn[4].Text = "Actualizar";
                btn[0].Click += new EventHandler(this.btnBuscar_Click);
                btn[1].Click += new EventHandler(this.btnAgregar_Click);
                btn[2].Click += new EventHandler(this.btnModificar_Click);
                btn[3].Click += new EventHandler(this.btnEliminar_Click);
                btn[4].Click += new EventHandler(this.btnActualizar_Click);
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
                this.ClientSize = new Size(542, 369);
                dgv.Size = new Size(522, 314);
                dgv.Columns[0].Name = "Usuario";
                dgv.Columns[1].Name = "Nombre";
                dgv.Columns[2].Name = "PassWord";
                dgv.Columns[3].Name = "Conectado";
                dgv.Columns[4].Name = "Jugando";
            }
        }
        private void PopulateDataGridView(String cond)
        {
            My_SQL mysql = new My_SQL();
            OdbcDataReader res;
            res = mysql.hazConsulta("select * from usuarios " + cond);
            int i;
            String[] row = new String[colums];
            if (res.HasRows)
            {
                while (res.Read())
                {
                    for (i = 0; i < colums; i++)
                        row[i] = res.GetString(i);
                    dgv.Rows.Add(row);
                }
            }
        }
        private void frmGrafica_Load(object sender, EventArgs e)
        {
            inicializaVentana();
            SetupDataGridView();
            PopulateDataGridView("");
            actualiza();
        }
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >=0)
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
            String[] c = new String[6];
            for(int i = 0; i < 6; i++)
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
            if (Sval[4] != "")
            {
                if (c[1] != "" || c[2] != "" || c[3] != "" || c[4] != "")
                    c[5] = " and ";
                else
                    c[0] = " where";
                c[5] += " " + cam[4] + " = '" + Sval[4] + "'";
            }
            dgv.Hide();
            SetupDataGridView();
            PopulateDataGridView(c[0] + c[1] + c[2] + c[3] + c[4]);
            actualiza();
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmOperacion ca = new frmOperacion(roll, colums, "Agregar");
            ca.ShowDialog();
            dgv.Hide();
            SetupDataGridView();
            PopulateDataGridView("");
            actualiza();
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (sel >= 0)
            {
                frmOperacion cm = new frmOperacion(roll, colums, val, "Modificar");
                cm.ShowDialog();
                dgv.Hide();
                SetupDataGridView();
                PopulateDataGridView("");
                actualiza();
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (sel >= 0)
            {
                My_SQL mysql;
                int nq;
                mysql = new My_SQL();
                nq = mysql.hazNoConsulta("delete from " + roll + " where " + cam[0] + " = '" + val[0] + "'");
                dgv.Hide();
                SetupDataGridView();
                PopulateDataGridView("");
                actualiza();
                MessageBox.Show("Registro Borrado");
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
    }
}
