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

namespace Ajedrez
{
    public class HiloComsUDP : frmConecciones
    {
        public bool serverActive = true, loseOP = false, tablero = false, movimiento = false, enroque = false;
        public UdpClient client;
        public int port, nOp = -1, nCte1 = -1, x1, y1, x2, y2;
        public String server = "pcgera", from, usr1 = "", oponent = "", color = "";
        IPEndPoint recvpt;

        public HiloComsUDP(int p)
        {
            client = new UdpClient(p);
            port = p;
        }
        public void enviaMensaje(String mssg)
        {
            byte[] senddata;
            String answer;
            answer = mssg + "@" + Dns.GetHostName() ;
            senddata = Encoding.ASCII.GetBytes ( answer ) ;
            try
            {
                client.Send ( senddata, senddata.Length, server, 200 ) ;
            }
            catch
            {
                MessageBox.Show ( "Error sending data" ) ;
            }
        }
        public void closeClient()
        {
            client.Close();
            recvpt = null;
            client = null;
        }
        public void startplayer()
        {
            recvpt = new IPEndPoint(IPAddress.Any, 200);
            byte[] data;
            String str;
            String[] cds;
            while (serverActive)
            {
                try
                {
                    if (client != null)
                    {
                        data = client.Receive(ref recvpt);
                        str = Encoding.ASCII.GetString(data);
                        cds = str.Split('@');
                        // 3-Check the answer
                        if (cds[0] == "JUEGOINI")
                        {
                            usr = cds[1];
                            usr1 = usr;
                            oponent = cds[2];
                            nCte = int.Parse(cds[3]);
                            nCte1 = nCte;
                            nOp = int.Parse(cds[4]);
                            color = cds[5];
                            from = cds[6];
                        }
                        if (cds[0] == "SERVERCLOSED")
                        {
                            serverActive = false;
                        }
                        if (cds[0] == "GRANTEDCLOSE")
                        {
                            serverActive = false;
                        }
                        if (cds[0] == "LOSEOPONENT")
                        {
                            loseOP = true;
                            oponent = "";
                            color = "";
                            nOp = -1;
                        }
                        if (cds[0] == "partida")
                        {
                            if (cds[1] == "GRANTEDCLOSEGAME")
                            {
                                loseOP = true;
                                oponent = "";
                                color = "";
                                nOp = -1;
                                if (cds[2] == "CLOSEBOARD")
                                    tablero = false;
                            }
                            if (cds[1] == "MOVIMIENTO")
                            {
                                x1 = int.Parse(cds[2]);
                                y1 = int.Parse(cds[3]);
                                x2 = int.Parse(cds[4]);
                                y2 = int.Parse(cds[5]);
                                movimiento = true;
                                if (cds[6] == "R")
                                    enroque = true;
                            }
                        }
                        if (cds[0] == "cierraserver")
                        {
                            serverActive = false;
                        }
                    }
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
        }
    }
}
