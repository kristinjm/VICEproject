using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace VICE_Final
{
    public partial class Form2 : Form
    {
        public static OSIPI myPI;
        public Form2()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            String[] moon = new String[2];
            int i = AsynchTCP.moon(moon);

            Thread formUpdater = new Thread(formPointUpdate); //thread to recieve Rasp PI/ALPACA data
            formUpdater.Start(); //start updating form
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 newPiform = new Form1();                //new pi update test form
            newPiform.Show();
        }

        public void formPointUpdate()
        {
            while(true)
            {
            label4.Text = myPI.searchPiPoints("SP14VICE_Flow", DateTime.Now.ToString(), DateTime.Now.ToString()).ToString();
            label5.Text =  myPI.searchPiPoints("SP14VICE_Freq", DateTime.Now.ToString(), DateTime.Now.ToString()).ToString();
            label6.Text = myPI.searchPiPoints("SP14VICE_RPM", DateTime.Now.ToString(), DateTime.Now.ToString()).ToString();
            }
        }
    }
}
