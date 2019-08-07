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
    public partial class HiloComsUDP
    {
        UdpClient server = new UdpClient(200);
        UdpClient auxSrv = new UdpClient(199);
        ClientesUDP[] ctes = new ClientesUDP[10];
        public bool activo = true, cerrado = false;
        public HiloComsUDP()
        {
        }
        public void send(String mssg, int pc)
        {
            String juego;
            byte[] senddata;
            juego = mssg + "@" + Dns.GetHostName();
            senddata = Encoding.ASCII.GetBytes(juego);
            try
            {
                server.Send(senddata, senddata.Length, ctes[pc].pcName, pc + 201);
            }
            catch
            {
                MessageBox.Show ("Cannot send to pc" + pc);
            }
        }
        public void cierraServidor()
        {
            String juego;
            byte[] senddata;
            juego = "cierraserver@CERRAR" + "@" + Dns.GetHostName();
            senddata = Encoding.ASCII.GetBytes(juego);
            try
            {
                 auxSrv.Send(senddata, senddata.Length, "pcgera", 200);
            }
            catch
            {
                MessageBox.Show("Cannot send to Server");
            }
        }
        delegate void delEditaListBox(ListBox l, String t);
        void EditaListBox(ListBox l, String t)
        {
            if (l.InvokeRequired)
                l.Invoke(new delEditaListBox(EditaListBox), l, t);
            else
                l.Items.Add(t);
        }
        public void closeServer()
        {
            server.Close();
            server = null;
            auxSrv.Close();
            auxSrv = null;
        }
        public void startserver()
        {
            // 1-Create an IPEndPoint to receive messages
            IPEndPoint recvpt = new IPEndPoint(IPAddress.Any, 0);
            byte[] data;
            String[] cds;
            String str, aux;
            int i, nq;
            My_SQL mysql = null;

            while (activo)
            {
                try
                {
                    // 2-Receive data
                    data = server.Receive(ref recvpt);
                    str = Encoding.ASCII.GetString(data);
                    cds = str.Split('@');
                    // 3-Check the answer
                    if (cds[0] == "identifica")
                    {
                        i = int.Parse(cds[2]);
                        ctes[i] = new ClientesUDP();
                        ctes[i].usr = cds[1];
                        ctes[i].nCte = i;
                        ctes[i].pcName = cds[3];
                    }
                    if (cds[0] == "partida")
                    {
                        if (cds[1] == "CERRAR")
                        {
                            i = int.Parse(cds[2]);
                            send("partida@GRANTEDCLOSEGAME@NOCLOSEBOARD", i);
                            i = int.Parse(cds[3]);
                            send("partida@GRANTEDCLOSEGAME@CLOSEBOARD", i);
                            mysql = new My_SQL();
                            nq = mysql.hazNoConsulta("update usuarios set playing='0' where user='" + cds[4] + "' or user='" + cds[5] + "'");
                        }
                        if (cds[1] == "MOVIMIENTO")
                        {
                            i = int.Parse(cds[2]);
                            send("partida@MOVIMIENTO@" + cds[4] + "@" + cds[5] + "@" + cds[6] + "@" + cds[7] + "@" + cds[8], i);
                            i = int.Parse(cds[3]);
                            send("partida@MOVIMIENTO@" + cds[4] + "@" + cds[5] + "@" + cds[6] + "@" + cds[7] + "@" + cds[8], i);
                        }
                    }
                    if (cds[0] == "cierraserver")
                    {
                        activo = false;
                        for (i = 0; i < 10; i++)
                            if (ctes[i] != null)
                                if(ctes[i].usr != "")
                                    send("cierraserver@CERRAR", ctes[i].nCte);
                    }
                }
                catch(Exception e1)
                {
                    MessageBox.Show("Problemas con el Servidor " + e1.ToString());
                }
            }
            cerrado = true;
        }
    }
}
