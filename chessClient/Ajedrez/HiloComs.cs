using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;


namespace Ajedrez
{
    public class HiloComs : frmConecciones
    {
        TcpClient cxnServ;
        NetworkStream flujo;
        TextBox[] txb;
        DataGridView dgv;
        internal bool unload = false;
        internal String accion = "";
        
        internal HiloComs(TextBox u, TextBox p, TcpClient cxnServ)
        {
            this.cxnServ = cxnServ;
            flujo = cxnServ.GetStream();
            txb = new TextBox[2];
            txb[0] = u;
            txb[1] = p;
        }
        internal HiloComs(TextBox u, TextBox p,TextBox n, TcpClient cxnServ)
        {
            this.cxnServ = cxnServ;
            flujo = cxnServ.GetStream();
            txb = new TextBox[3];
            txb[0] = u;
            txb[1] = n;
            txb[2] = p;
        }
        internal HiloComs(String u, DataGridView d, TcpClient cxnServ)
        {
            this.cxnServ = cxnServ;
            flujo = cxnServ.GetStream();
            dgv = d;
            usr = u;
        }
        delegate string delLeeTBM(TextBox m);
        string leeTBM(TextBox m)
        {
            if (m.InvokeRequired)
                return (string)m.Invoke(new delLeeTBM(leeTBM),m);
            else 
                return m.Text;
        }
        delegate void delLlenaGrid(DataGridView d, String[] row);
        void LlenaGrid(DataGridView d, String[] row)
        {
            if (d.InvokeRequired)
                d.Invoke(new delLlenaGrid(LlenaGrid),d, row);
            else
                d.Rows.Add(row);
        }
        internal void cierraCxnS()
        {
            ASCIIEncoding cod = new ASCIIEncoding();
            byte[] datos = cod.GetBytes("@X");
            flujo.Write(datos, 0, datos.Length);
            cxnServ.Close();
        }
        internal void cnnLogin()
        {
            byte[] datos = null;
            ASCIIEncoding cod = new ASCIIEncoding();
            int nBytes = 0;
            String cad;
            while (true)
            {
                if (accion == "LOGIN")
                {
                    try
                    {
                        datos = cod.GetBytes("login@" + leeTBM(txb[0]) + "@" + leeTBM(txb[1]));
                        flujo.Write(datos, 0, datos.Length);
                        datos = new byte[256];
                        nBytes = flujo.Read(datos, 0, datos.Length);
                        flujo.Flush();
                        cad = cod.GetString(datos, 0, nBytes);
                        if (cad == "login@GRANTED")
                        {
                            //MessageBox.Show("Bien Venido");
                            unload = true;
                            conectado = true;
                            Susr = leeTBM(txb[0]);
                        }
                        if (cad == "login@DENIED")
                            MessageBox.Show("Usted no se encuentra Registrado");
                        if (cad == "login@ALREADYCONNECTED")
                            MessageBox.Show("Ese usuario ya esta conectado");
                    }
                    catch(SocketException se)
                    {
                        MessageBox.Show("Problemas con el servidor " + se.ToString());
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    accion = "";
                }
                Thread.Sleep(10);
            }
        }
        internal void cnnRegistro()
        {
            byte[] datos = null;
            ASCIIEncoding cod = new ASCIIEncoding();
            int nBytes = 0;
            String cad;
            while (true)
            {
                if (accion == "REGISTRO")
                {
                    try
                    {
                        datos = cod.GetBytes("registro@" + leeTBM(txb[0]) + "@" + leeTBM(txb[1]) + "@" + leeTBM(txb[2]));
                        flujo.Write(datos, 0, datos.Length);
                        datos = new byte[256];
                        nBytes = flujo.Read(datos, 0, datos.Length);
                        flujo.Flush();
                        cad = cod.GetString(datos, 0, nBytes);
                        if (cad == "registro@GRANTED")
                        {
                            //MessageBox.Show("Bien Venido");
                            unload = true;
                            conectado = true;
                            Susr = leeTBM(txb[0]);
                        }
                        if (cad == "registro@EXIST")
                            MessageBox.Show("Ya existe un usuario con ese nombre");
                        accion = "";
                    }
                    catch(SocketException se)
                    {
                        MessageBox.Show("Problemas con el servidor " + se.ToString());
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                Thread.Sleep(10);
            }
        }
        internal void cnnRetadores()
        {
            byte[] datos = null;
            ASCIIEncoding cod = new ASCIIEncoding();
            int nBytes = 0;
            String cad;
            String[] cds, cads;
            while (true)
            {
                if (accion != "")
                {
                    cds = accion.Split('@');
                    if (cds[0] == "LISTO")
                    {
                        try
                        {
                            datos = cod.GetBytes("retador@LISTO@" + usr);
                            flujo.Write(datos, 0, datos.Length);
                            datos = new byte[256];
                            nBytes = flujo.Read(datos, 0, datos.Length);
                            flujo.Flush();
                            cad = cod.GetString(datos, 0, nBytes);
                            cads = cad.Split('@');
                            if (cads[0] == "retador" && cads[1] == "OK")
                            {
                                nCte = int.Parse(cads[2]);
                                //MessageBox.Show("Comienza a Retar");
                            }
                            accion = "";
                        }
                        catch (SocketException se1)
                        {
                            MessageBox.Show("Problemas con el servidor " + se1.ToString());
                        }
                        catch (Exception e1)
                        {
                            MessageBox.Show(e1.ToString());
                        }
                    }
                    if (cds[0] == "REGISTRO")
                    {
                        try
                        {
                            datos = cod.GetBytes("retador@LISTO@" + usr);
                            flujo.Write(datos, 0, datos.Length);
                            datos = new byte[256];
                            nBytes = flujo.Read(datos, 0, datos.Length);
                            flujo.Flush();
                            cad = cod.GetString(datos, 0, nBytes);
                            cads = cad.Split('@');
                            if (cads[0] == "retador" && cads[1] == "OK")
                            {
                                nCte = int.Parse(cads[2]);
                                MessageBox.Show("Registro Exitoso");
                            }
                            accion = "";
                        }
                        catch (SocketException se1)
                        {
                            MessageBox.Show("Problemas con el servidor " + se1.ToString());
                        }
                        catch (Exception e1)
                        {
                            MessageBox.Show(e1.ToString());
                        }
                    }
                    if (cds[0] == "RETAR")
                    {
                        try
                        {
                            datos = cod.GetBytes("retador@RETAR@" + cds[1]);
                            flujo.Write(datos, 0, datos.Length);
                            datos = new byte[256];
                            nBytes = flujo.Read(datos, 0, datos.Length);
                            flujo.Flush();
                            cad = cod.GetString(datos, 0, nBytes);
                            if (cad == "retador@ESPERANOTIFICAR")
                            { }
                            //   MessageBox.Show("Espera a Notificar al Oponente");
                        }
                        catch (SocketException se2)
                        {
                            MessageBox.Show("Problemas con el servidor " + se2.ToString());
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show(e2.ToString());
                        }
                        accion = "";
                    }
                    if (cds[0] == "RENEW")
                    {
                        try
                        {
                            datos = cod.GetBytes("retador@RENEW@" + cds[2] + "@" + cds[3]);
                            flujo.Write(datos, 0, datos.Length);
                            datos = new byte[256];
                            nBytes = flujo.Read(datos, 0, datos.Length);
                            flujo.Flush();
                            cad = cod.GetString(datos, 0, nBytes);
                            if (cad == "retador@RENEW_OK")
                            { }
                        }
                        catch (SocketException se3)
                        {
                            MessageBox.Show("Problemas con el servidor " + se3.ToString());
                        }
                        catch (Exception e3)
                        {
                            MessageBox.Show(e3.ToString());
                        }
                        accion = "";
                    }
                    if (cds[0] == "AQUIRE_OP")
                    {
                        try
                        {
                            datos = cod.GetBytes("retador@AQUIRE_OP@" + cds[1] + "@" + cds[2]);
                            flujo.Write(datos, 0, datos.Length);
                            datos = new byte[256];
                            nBytes = flujo.Read(datos, 0, datos.Length);
                            flujo.Flush();
                            cad = cod.GetString(datos, 0, nBytes);
                            if (cad == "retador@AQUIRE_OK")
                            { }
                        }
                        catch (SocketException se3)
                        {
                            MessageBox.Show("Problemas con el servidor " + se3.ToString());
                        }
                        catch (Exception e3)
                        {
                            MessageBox.Show(e3.ToString());
                        }
                        accion = "";
                    }
                }
                Thread.Sleep(10);
            }
        }
        internal void notificaEdo(String datos)
        {
            byte[] mensaje = null;
            mensaje = Encoding.ASCII.GetBytes(datos);
            flujo.Write(mensaje, 0, mensaje.Length);
            flujo.Flush();
            Thread.Sleep(1);
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HiloComs
            // 
            this.ClientSize = new System.Drawing.Size(292, 269);
            this.Name = "HiloComs";
            this.Load += new System.EventHandler(this.HiloComs_Load);
            this.ResumeLayout(false);

        }
        private void HiloComs_Load(object sender, EventArgs e)
        {

        }
    }
}
