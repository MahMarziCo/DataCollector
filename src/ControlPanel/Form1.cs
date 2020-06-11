
using Mah.Common.Encrypt;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;

namespace ControlPanel
{
    public partial class Form1 : Form
    {
        private string encryptionKey = "M@H&M@RZ!K3Y";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            Cryptor cryptor = new Cryptor(encryptionKey);
            txtOutputText.Text = cryptor.Encrypt(txtInputText.Text) ;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            Cryptor cryptor = new Cryptor(encryptionKey);
            txtOutputText.Text = cryptor.Decrypt(txtInputText.Text);
        }
    }
}
