using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using SimpleTCP;

namespace WF1 
{
    public partial class O : Form
    {

        private bool WhoTurn = false;

        private Controller _controller;
        public O(Controller controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        SimpleTcpServer server;

        private void Form1_Load(object sender, EventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.StringEncoder = Encoding.Unicode;
            server.DataReceived += Server_DataReceived;

        }

        private void Server_DataReceived(object sender, SimpleTCP.Message e)
        {   
            
            string[] s = e.MessageString.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            int x = Int32.Parse(s[0]);
            int y = Int32.Parse(s[1]);
            _controller.setX(Field, x, y);

            _controller.Win(Field);

            WhoTurn = true;

            Turn.Invoke((MethodInvoker)delegate ()
            {

                Turn.Text = "O";

            });


        }

        public void __Click(object sender, EventArgs e) 
        {
            Label label = (Label)sender;
            if (label.Text == "" && WhoTurn)
            {
                WhoTurn = false;

                Turn.Text = "X";

                _controller.setO(label);

                int x = Field.GetColumn((Label)sender);
                int y = Field.GetRow((Label)sender);

                server.Broadcast(string.Format("{0}:{1}", x, y));

                _controller.Win(Field);
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Start.Enabled = false;

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            server.Start(ip, 8910);

        }

        private void Stop_Click(object sender, EventArgs e)
        {

            if (server.IsStarted)
            {

                server.Stop();

            }

        }

        
    }
}
