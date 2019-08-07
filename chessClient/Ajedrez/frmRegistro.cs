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
    public partial class frmRegistro : frmConecciones
    {
        Thread hiloReg;
        public frmRegistro()
        {
            InitializeComponent();
        }
        private void btnRegistrarse_Click(object sender, EventArgs e)
        {
            if (txbUsuario.Text != "" || txbPassWord.Text != "" || txbNombre.Text != "")
                hcoms.accion = "REGISTRO";
            else
                MessageBox.Show("Hay datos sin llenar...");
        }
        private void frmRegistro_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(hcs != null)
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

        private void frmRegistro_Load(object sender, EventArgs e)
        {
            try
            {
                cxnServidor = new TcpClient("pcgera", 5432);
                hcoms = new HiloComs(txbUsuario, txbPassWord, txbNombre, cxnServidor);
                hcs = new Thread(new ThreadStart(hcoms.cnnRegistro));
                hcs.Start();
                hiloReg = new Thread(new ThreadStart(hilo_Regenera));
                hiloReg.Start();
            }
            catch (SocketException se)
            {
                this.Close();
                MessageBox.Show(se.ToString());
            }
        }
    }
}
