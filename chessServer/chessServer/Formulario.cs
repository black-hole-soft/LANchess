using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace chessServer
{
    public class Formulario : Form
    {
        public static String[] Sval = new String[6];
        public static String Scurp;
        public String[] cam, val;
        public String roll, nombre;
        public int colums;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Formulario
            // 
            this.ClientSize = new System.Drawing.Size(292, 269);
            this.Name = "Formulario";
            this.Load += new System.EventHandler(this.Formulario_Load);
            this.ResumeLayout(false);

        }

        private void Formulario_Load(object sender, EventArgs e)
        {

        }
    }
}
