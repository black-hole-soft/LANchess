using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace chessServer
{
    public partial class frmChessServer : Form
    {
        Label[] etiqs;
        EscuchaCte[] clientes;
        HiloComsUDP ctesUDP;
        Thread[] hilosCte;
        Thread hiloEscucha, hiloRegenera, hiloUDP;
        int ctes, puerto;
        IPAddress dirLocal;
        TcpListener servidor;
        TcpClient cteTmp = null;

        public frmChessServer()
        {
            InitializeComponent();
            ctes = 0;
            etiqs = new Label[10];
            clientes = new EscuchaCte[10];
            hilosCte = new Thread[10];
            puerto = 5432;
            //dirLocal = IPAddress.Parse("127.0.0.1");
            dirLocal = IPAddress.Any;
            servidor = new TcpListener(dirLocal, puerto);
        }
        private void frmChessServer_Load(object sender, EventArgs e)
        {
            btnDetener.Enabled = false;
        }
        private void cierraServer()
        {
            My_SQL mysql = new My_SQL();
            int nq = mysql.hazNoConsulta("update usuarios set connected='0', playing='0'");
            if (hiloEscucha != null)
            {
                hiloEscucha.Abort();
                hiloRegenera.Abort();
                servidor.Stop();
                for (int i = 0; i < ctes; i++)
                {
                    if (clientes[i] != null)
                    {
                        clientes[i].cierraCte("");
                        hilosCte[i].Abort();
                    }
                }
            }
            if (ctesUDP != null)
            {
                ctesUDP.cierraServidor();
                while(!ctesUDP.cerrado)
                {
                    Thread.Sleep(10);
                }
                ctesUDP.closeServer();
                ctesUDP = null;
            }
            if (hiloUDP != null)
                hiloUDP.Abort();
        }
        private void frmChessServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            cierraServer();
        }
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            My_SQL mysql = new My_SQL();
            int nq = mysql.hazNoConsulta("update usuarios set connected='0', playing='0'");
            btnIniciar.Enabled = false;
            btnDetener.Enabled = true;
            ctesUDP = new HiloComsUDP();
            hiloUDP = new Thread(new ThreadStart(ctesUDP.startserver));
            hiloUDP.Start();
            hiloEscucha = new Thread(new ThreadStart(this.hilo_Escucha));
            hiloEscucha.Start();
            hiloRegenera = new Thread(new ThreadStart(this.hilo_Regenera));
            hiloRegenera.Start();
        }
        private void btnDetener_Click(object sender, EventArgs e)
        {
            cierraServer();
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
        }
        delegate void delAgregaEtiqueta(Label l);
        void AgregaEtiqueta(Label l)
        {
            if (this.InvokeRequired)
                this.Invoke(new delAgregaEtiqueta(AgregaEtiqueta), l);
            else
                this.Controls.Add(l);
        }
        delegate void delQuitaEtiqueta(Label l);
        void QuitaEtiqueta(Label l)
        {
            if (l.InvokeRequired)
                l.Invoke(new delQuitaEtiqueta(QuitaEtiqueta), l);
            else
                l.Hide();
        }
        private void hilo_Escucha()
        {
            servidor.Start();
            int i, j;
            while (true)
            {
                cteTmp = servidor.AcceptTcpClient();
                i = -1;
                j = 0;
                ctes = 0;
                while (j < 10)
                {
                    if (clientes[j] == null)
                    {
                        if (i == -1)
                            i = j;
                    }
                    else
                    {
                        if (clientes[j].unload)
                        {
                            clientes[j] = null;
                            j--;
                        }
                    }
                    j++;
                }
                if (i != -1)
                {
                    etiqs[i] = new Label();
                    clientes[i] = new EscuchaCte(cteTmp, etiqs, i, ctesUDP);
                    hilosCte[i] = new Thread(new ThreadStart(clientes[i].atiende));
                    hilosCte[i].Start();
                    etiqs[i].Location = new System.Drawing.Point(6, i * 28 + 10);
                    etiqs[i].Size = new System.Drawing.Size(241, 36);
                    etiqs[i].Text = "Usuario:PassWord";
                    etiqs[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    AgregaEtiqueta(etiqs[i]);
                    Thread.Sleep(10);
                }
            }
        }
        private void hilo_Regenera() 
        {
            int j;
            while (true)
            {
                j = 0;
                ctes = 0;
                while (j < 10)
                {
                    if (clientes[j] != null)
                    {
                        if (clientes[j].unload)
                        {
                            clientes[j] = null;
                            QuitaEtiqueta(etiqs[j]);
                            etiqs[j] = null;
                        }
                        else
                            ctes++;
                    }
                    j++;
                }
                Thread.Sleep(10);
            }
        }
        private void btnUsers_Click(object sender, EventArgs e)
        {
            frmGrafica usrs = new frmGrafica("usuarios", 5);
            usrs.Show();
        }
    }
}
