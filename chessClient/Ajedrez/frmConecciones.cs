using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ajedrez
{
    public class frmConecciones : Form
    {
        public HiloComs hcoms;
        public Thread hcs;
        public TcpClient cxnServidor;
        public static bool conectado = false;
        public static String Susr = "";
        public String usr = "";
        public int nCte = -1;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmConecciones
            // 
            this.ClientSize = new System.Drawing.Size(292, 269);
            this.Name = "frmConecciones";
            this.Load += new System.EventHandler(this.frmConecciones_Load);
            this.ResumeLayout(false);

        }
        private void frmConecciones_Load(object sender, EventArgs e)
        {

        }
    }
}
