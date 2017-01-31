namespace ViberSender2017
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;

    public class Activate : Form
    {
        private Button button_ok;
        private IContainer components = null;
        public bool flag = false;
        private Label label1;
        private LinkLabel linkLabel;
        private TextBox textBox_key;

        public Activate()
        {
            this.InitializeComponent();
        }

        private void Activate_Load(object sender, EventArgs e)
        {
            this.label1.Text = this.label1.Text + CreateMD5(Workstation.GenerateWorkstationId());
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            string text = this.textBox_key.Text;
            char[] separator = new char[] { ':' };
            string input = this.label1.Text.Split(separator)[1].Trim();
            for (int i = 0; i < 0x3e8; i++)
            {
                input = CreateMD5(input);
            }
            if (text == input)
            {
                this.flag = true;
                base.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Неверный ключ!");
            }
        }

        public static string CreateMD5(string input)
        {
            using (MD5 md = MD5.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                byte[] buffer2 = md.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer2.Length; i++)
                {
                    builder.Append(buffer2[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetKey()
        {
            string input = CreateMD5(Workstation.GenerateWorkstationId());
            for (int i = 0; i < 0x3e8; i++)
            {
                input = CreateMD5(input);
            }
            return input;
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.textBox_key = new TextBox();
            this.button_ok = new Button();
            this.linkLabel = new LinkLabel();
            base.SuspendLayout();
            this.label1.AutoSize = true;
            this.label1.Location = new Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x8d, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ИД вашего оборудования:";
            this.label1.Click += new EventHandler(this.label1_Click);
            this.textBox_key.Location = new Point(0x31, 0x44);
            this.textBox_key.Name = "textBox_key";
            this.textBox_key.Size = new Size(0x131, 20);
            this.textBox_key.TabIndex = 1;
            this.button_ok.Location = new Point(0xa6, 0x5e);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new Size(0x4b, 0x17);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "Ок!";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new EventHandler(this.button_ok_Click);
            this.linkLabel.AutoSize = true;
            this.linkLabel.Location = new Point(13, 0x22);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new Size(0x5d, 13);
            this.linkLabel.TabIndex = 3;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "Скопировать ИД";
            this.linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x192, 0x7d);
            base.Controls.Add(this.linkLabel);
            base.Controls.Add(this.button_ok);
            base.Controls.Add(this.textBox_key);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "Activate";
            this.Text = "Требуется активация!";
            base.Load += new EventHandler(this.Activate_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            char[] separator = new char[] { ':' };
            Clipboard.SetText(this.label1.Text.Split(separator)[1].Trim());
        }
    }
}

