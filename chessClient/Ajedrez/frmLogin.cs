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

namespace Ajedrez
{
    public partial class frmLogin : frmConecciones
    {
        Thread hiloReg;
        public frmLogin()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txbUsuario.Text != "" || txbPassWord.Text != "")
                hcoms.accion = "LOGIN";
            else
                MessageBox.Show("Ingrese un Usuario y una Contraseña");
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                //cxnServidor = new TcpClient("127.0.0.1", 5432);
                cxnServidor = new TcpClient("pcgera", 5432);
                hcoms = new HiloComs(txbUsuario, txbPassWord, cxnServidor);
                hcs = new Thread(new ThreadStart(hcoms.cnnLogin));
                hcs.Start();
                hiloReg = new Thread(new ThreadStart(hilo_Regenera));
                hiloReg.Start();
            }
            catch (SocketException se)
            {
                this.Close();
                MessageBox.Show(se.ToString());
            }
            /*
            DialogResult j = MessageBox.Show("En espera de cliente", "Situación", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (j == DialogResult.OK)
                MessageBox.Show("Aceptado");
            else
                MessageBox.Show("Cancelado");
            */
        }
        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hcs != null)
                hcs.Abort();
            if (hiloReg != null)
                hiloReg.Abort();
            if (hcoms != null)
            {
                hcoms.cierraCxnS();
                hcoms = null;
            }
        }
        private void hilo_Regenera()
        {
            while (!hcoms.unload)
                Thread.Sleep(10);
            cierraLogin();
        }
        delegate void delCierraLogin();
        void cierraLogin()
        {
            if (this.InvokeRequired)
                this.Invoke(new delCierraLogin(cierraLogin));
            else
                this.Close();
        }
    }
}
