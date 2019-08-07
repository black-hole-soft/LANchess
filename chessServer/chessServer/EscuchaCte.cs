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
using System.Data.Odbc;

namespace chessServer
{
    class EscuchaCte
    {
        //String[] data;
        Label[] lCte;
        TcpClient cte = null;
        NetworkStream flujo = null;
        int nCte = -1, op = -1;
        public bool unload = false, me = false;
        public static bool disponible = true;
        String user = "", oponent = "", uso = "NOTHING";
        HiloComsUDP ctesUDP;

        internal EscuchaCte(TcpClient c, Label[] l, int n, HiloComsUDP u)
        {
            cte = c;
            flujo = c.GetStream();
            nCte = n;
            lCte = l;
            ctesUDP = u;
        }
        delegate string delLeeEtiq(Label etiq);
        string leeEtiq(Label etiq)
        {
            if (etiq.InvokeRequired)
                return (string)etiq.Invoke(new delLeeEtiq(leeEtiq), etiq);
            else return etiq.Text;
        }
        delegate void delCambiaTxt(Label l, String t);
        void cambiaTxt(Label l, String t)
        {
            if (l.InvokeRequired)
                l.Invoke(new delCambiaTxt(cambiaTxt), l, t);
            else l.Text = t;
        }
        internal void atiende()
        {
            byte[] bytes = new byte[256];
            String datos = null, edo;
            String[] cds, row;
            int i = 0, nq, x;
            My_SQL mysql = null;
            OdbcDataReader res = null;
            while (cte != null)
            {
                try
                {
                    if ((me && !disponible) || (!me && disponible))
                    {
                        if ((i = flujo.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            datos = Encoding.ASCII.GetString(bytes, 0, i);
                            if (datos.Equals("@X"))
                                cierraCte("CLOSED");
                            else
                            {
                                cds = datos.Split('@');
                                if (cds[0] == "login")
                                {
                                    uso = "LOGIN";
                                    mysql = new My_SQL();
                                    res = mysql.hazConsulta("select * from usuarios where user='" + cds[1] + "' and pssw='" + cds[2] + "'");
                                    if (res.HasRows)
                                    {
                                        if (res.Read())
                                        {
                                            edo = res.GetString(3);
                                            if (edo == "1")
                                                notificaEdo("login@ALREADYCONNECTED");
                                            else
                                            {
                                                nq = mysql.hazNoConsulta("update usuarios set connected='1' where user='" + cds[1] + "'");
                                                cambiaTxt(lCte[nCte], cds[1] + ":" + cds[2]);
                                                notificaEdo("login@GRANTED");
                                                user = cds[1];
                                            }
                                        }
                                    }
                                    else
                                        notificaEdo("login@DENIED");
                                }
                                if (cds[0] == "registro")
                                {
                                    uso = "REGISTRO";
                                    mysql = new My_SQL();
                                    res = mysql.hazConsulta("select * from usuarios where user='" + cds[1] + "'");
                                    row = new String[4];
                                    if (res.HasRows)
                                        notificaEdo("registro@EXIST");
                                    else
                                    {
                                        mysql = new My_SQL();
                                        nq = mysql.hazNoConsulta("insert into usuarios values('" + cds[1] + "','" + cds[2] + "','" + cds[3] + "','1','0')");
                                        cambiaTxt(lCte[nCte], cds[1] + ":" + cds[2]);
                                        notificaEdo("registro@GRANTED");
                                        user = cds[1];
                                    }
                                }
                                if (cds[0] == "retador")
                                {
                                    uso = "RETAR";
                                    if (cds[1] == "LISTO")
                                    {
                                        mysql = new My_SQL();
                                        user = cds[2];
                                        cambiaTxt(lCte[nCte], cds[1] + ":" + cds[2]);
                                        nq = mysql.hazNoConsulta("update usuarios set connected='1' where user='" + cds[2] + "'");
                                        notificaEdo("retador@OK@" + nCte);
                                    }
                                    if (cds[1] == "RETAR")
                                    {
                                        oponent = cds[2];
                                        for (x = 0; x < 10; x++)
                                            if (lCte[x] != null)
                                            {
                                                row = lCte[x].Text.Split(':');
                                                if (row[0] == "LISTO")
                                                    if (row[1] == oponent)
                                                    {
                                                        op = x;
                                                        //cambiaTxt(lCte[x], oponent + ":" + user);
                                                    }
                                            }
                                        //cambiaTxt(lCte[nCte], user + ":" + oponent);
                                        mysql = new My_SQL();
                                        nq = mysql.hazNoConsulta("update usuarios set playing='1' where user='" + user + "'");
                                        mysql = new My_SQL();
                                        nq = mysql.hazNoConsulta("update usuarios set playing='1' where user='" + cds[2] + "'");
                                        notificaEdo("retador@ESPERANOTIFICAR");
                                        ctesUDP.send("JUEGOINI@" + user + "@" + oponent + "@" + nCte.ToString() + "@" + op.ToString() + "@blancas", nCte);
                                        ctesUDP.send("JUEGOINI@" + oponent + "@" + user + "@" + op.ToString() + "@" + nCte.ToString() + "@doradas", op);
                                    }
                                    if (cds[1] == "RENEW")
                                    {
                                        cambiaTxt(lCte[nCte], "LISTO:" + user);
                                        oponent = "";
                                        notificaEdo("retador@RENEW_OK");
                                    }
                                    if (cds[1] == "AQUIRE_OP")
                                    {
                                        cambiaTxt(lCte[nCte], user + ":" + cds[2]);
                                        oponent = cds[2];
                                        op = int.Parse(cds[3]);
                                        notificaEdo("retador@RENEW_OK");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (SocketException se)
                {
                    cierraCte("");
                    MessageBox.Show("Problemas de conexion " + se.ToString());
                }
                catch (Exception e)
                {
                    cierraCte("");
                    MessageBox.Show(e.ToString());
                }
                Thread.Sleep(10);
            }
        }
        internal void cierraCte(String cad)
        {
            My_SQL mysql;
            int nq;
            if(cad == "CLOSED" && uso == "RETAR")
                ctesUDP.send("GRANTEDCLOSE@", nCte);
            else
                if (uso == "RETAR" || uso == "NOTHIGN")
                    ctesUDP.send("SERVERCLOSED@", nCte);
            if (cte != null)
            {
                cte.Close();
                cte = null;
            }
            unload = true;
            if (user != "")
            {
                mysql = new My_SQL();
                nq = mysql.hazNoConsulta("update usuarios set connected='0', playing='0' where user='" + user + "'");
                if (oponent != "")
                {
                    try
                    {
                        ctesUDP.send("LOSEOPONENT@", op);
                        if(lCte[op] != null)
                            cambiaTxt(lCte[op], "LISTO:" + oponent);
                    }
                    catch (Exception e1)
                    {
                        MessageBox.Show("Erro al Cerrar al Oponente " + e1.ToString());
                    }
                    mysql = new My_SQL();
                    nq = mysql.hazNoConsulta("update usuarios set playing='0' where user='" + oponent + "'");
                }
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
    }
}
