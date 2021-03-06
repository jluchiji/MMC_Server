﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Lidgren.Network;
using SamplesCommon;

namespace MMC_Client
{
    public partial class Form1 : Form
    {
        private static NetPeer s_server;

        public Form1()
        {
            InitializeComponent();

        }

        private void Output(string text)
        {
            NativeMethods.AppendText(richTextBox1, text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("MMC");
            config.MaximumConnections = 100;
            config.Port = 39393;
            // Enable DiscoveryResponse messages
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            s_server = new NetPeer(config);
            Output("listening on " + config.Port.ToString());
            s_server.RegisterReceivedCallback(new SendOrPostCallback(GotMessage));
            s_server.Start();
        }

        public void GotMessage(object peer)
        {
            NetIncomingMessage im;
            while ((im = s_server.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        Output("Recieved Discovery Signal");
                        NetOutgoingMessage response = s_server.CreateMessage();
                        response.Write("character@MMC_Server01");
 
                        // Send the response to the sender of the request
                        s_server.SendDiscoveryResponse(response, im.SenderEndPoint);
                        break;
                    default:
                        Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            s_server.Shutdown("bye");
        }
    }
}
