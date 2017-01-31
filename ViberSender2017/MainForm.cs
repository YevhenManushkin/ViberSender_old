namespace ViberSender2017
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    public class MainForm : Form
    {
        private Thread _th = null;
        private Button button_browse_numbers;
        private Button button_load_numbers;
        private Button button_new_acc;
        private Button button_picture;
        private Button button_start;
        private Button button_stop;
        private Button button1;
        private CheckBox checkBox_data;
        private CheckBox checkBox_link;
        private CheckBox checkBox_swap_acc;
        private CheckBox checkBox_toFirst;
        private CheckBox checkBox_unvalid_accs;
        private IContainer components = null;
        private ContextMenuStrip contextMenu;
        private DataGridView dataGridView_all_accs;
        private DataGridView dataGridView_all_numbers;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label label1;
        private Label label2;
        private DataGridViewTextBoxColumn Login;
        private DataGridViewTextBoxColumn Number;
        private DataGridViewTextBoxColumn Numbers;
        private DataGridViewTextBoxColumn NumberSerial;
        private DataGridViewTextBoxColumn NumberSerial1;
        private NumericUpDown numericUpDown_kol_swap;
        private NumericUpDown numericUpDown_kol_unvalid;
        private NumericUpDown numericUpDown_pause;
        private bool pause = false;
        private ProgressBar progressBar1;
        private Random r = new Random();
        private RichTextBox richTextBox_text;
        private DataGridViewTextBoxColumn Sended;
        private TabControl tabControl_general;
        private TabPage tabPage_numbers;
        private TabPage tabPage_settings;
        private TabPage tabPage_subjects;
        private TextBox textBox_file;
        private TextBox textBox_link;
        private TextBox textBox_path_numbers;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private Button button_change_acc;
        private DataGridViewCheckBoxColumn UseSend;

        public MainForm()
        {
            this.InitializeComponent();
            this.toolStripMenuItem1.Click += delegate (object a, EventArgs b) {
                for (int j = 0; j < this.dataGridView_all_accs.Rows.Count; j++)
                {
                    if ((this.dataGridView_all_accs.SelectedRows.Count > 0) && (this.dataGridView_all_accs.SelectedRows[0].Index != j))
                    {
                        this.dataGridView_all_accs.Rows[j].Cells[1].Value = "0";
                    }
                    if ((this.dataGridView_all_accs.SelectedRows.Count > 0) && (this.dataGridView_all_accs.SelectedRows[0].Index == j))
                    {
                        this.dataGridView_all_accs.Rows[j].Cells[1].Value = "1";
                    }
                    this.dataGridView_all_accs.Update();
                }
            };
            this.toolStripMenuItem2.Click += delegate (object a, EventArgs b) {
                for (int k = 0; k < this.dataGridView_all_accs.Rows.Count; k++)
                {
                    if (k < this.dataGridView_all_accs.SelectedRows[0].Index)
                    {
                        this.dataGridView_all_accs.Rows[k].Cells[1].Value = "0";
                    }
                    else
                    {
                        this.dataGridView_all_accs.Rows[k].Cells[1].Value = "1";
                    }
                }
            };
            try
            {
                StartSettings settings = new StartSettings();
                if (File.Exists("settings.xml"))
                {
                    using (Stream stream = new FileStream("settings.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(StartSettings));
                        settings = (StartSettings) serializer.Deserialize(stream);
                        this.checkBox_data.Checked = settings.add_date;
                        this.checkBox_link.Checked = settings.link;
                        this.checkBox_swap_acc.Checked = settings.swap_accs;
                        this.checkBox_toFirst.Checked = settings.first_accs;
                        this.checkBox_unvalid_accs.Checked = settings.unvalid;
                        this.numericUpDown_kol_swap.Value = settings.swap_accs_kol;
                        this.numericUpDown_kol_unvalid.Value = settings.unvalid_kol;
                        this.textBox_file.Text = settings.folder_files;
                        this.textBox_link.Text = settings.link_str;
                        this.richTextBox_text.Text = settings.richtextbox;
                        this.textBox_path_numbers.Text = settings.file_numbers;
                        if (!string.IsNullOrEmpty(settings.file_numbers))
                        {
                            this.LoadNumbers();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button_browse_numbers_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "TXT|*.txt"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_path_numbers.Text = dialog.FileName;
            }
        }

        private void button_load_numbers_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox_path_numbers.Text))
            {
                MessageBox.Show("Укажите файл с номерами!");
            }
            else
            {
                this.LoadNumbers();
            }
        }

        private void button_new_acc_Click(object sender, EventArgs e)
        {
            IEnumerable<Process> source = from f in Process.GetProcesses()
                where f.ProcessName == "Viber"
                select f;
            if (source.Any<Process>())
            {
                source.First<Process>().Kill();
            }
            this.button_new_acc.Enabled = false;
            WorkBD.OffAccs();
            Process.Start(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + @"\Users\" + Environment.UserName + @"\AppData\Local\Viber\Viber.exe");
            this.dataGridView_all_accs.DataSource = WorkBD.WaitNewAcc();
            for (int i = 0; i < this.dataGridView_all_accs.Rows.Count; i++)
            {
                this.dataGridView_all_accs.Rows[i].Cells[0].Value = i + 1;
            }
            this.button_new_acc.Enabled = true;
        }

        private void button_change_acc_Click(object sender, EventArgs e)
        {
            IEnumerable<Process> source = from f in Process.GetProcesses()
                                          where f.ProcessName == "Viber"
                                          select f;
            if (source.Any<Process>())
            {
                source.First<Process>().Kill();
            }
            this.button_change_acc.Enabled = false;
            dataGridView_all_accs.Rows[WorkBD.NextAcc()].Selected = true;
            Process.Start(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + @"\Users\" + Environment.UserName + @"\AppData\Local\Viber\Viber.exe");
            this.button_change_acc.Enabled = true;
        }

        private void button_picture_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_file.Text = dialog.SelectedPath;
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (this.dataGridView_all_numbers.Rows.Count < 2)
            {
                MessageBox.Show("Загрузите номера!");
            }
            else
            {
                StartSettings o = new StartSettings {
                    add_date = this.checkBox_data.Checked,
                    file_numbers = this.textBox_path_numbers.Text,
                    first_accs = this.checkBox_toFirst.Checked,
                    folder_files = this.textBox_file.Text,
                    link = this.checkBox_link.Checked,
                    link_str = this.textBox_link.Text,
                    richtextbox = this.richTextBox_text.Text,
                    swap_accs = this.checkBox_swap_acc.Checked,
                    swap_accs_kol = this.numericUpDown_kol_swap.Value,
                    unvalid = this.checkBox_unvalid_accs.Checked,
                    unvalid_kol = this.numericUpDown_kol_unvalid.Value
                };
                using (Stream stream = new FileStream("settings.xml", FileMode.Create))
                {
                    new XmlSerializer(typeof(StartSettings)).Serialize(stream, o);
                }
                this.button_start.Enabled = false;
                this._th = new Thread(new ThreadStart(this.Solution));
                this._th.SetApartmentState(ApartmentState.STA);
                this._th.IsBackground = true;
                this._th.Start();
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            try
            {
                this._th.Resume();
            }
            catch
            {
            }
            this._th.Abort();
            this.button1.Text = "Пауза";
            this.button_start.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.pause = !this.pause;
                if (this.pause)
                {
                    this._th.Suspend();
                    this.button1.Text = "Продолжить";
                }
                else
                {
                    this.button1.Text = "Пауза";
                    this.button1.Enabled = true;
                    this._th.Resume();
                }
            }
            catch
            {
            }
        }

        private void checkBox_link_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox_link.Enabled = this.checkBox_link.Checked;
        }

        private void checkBox_swap_acc_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_kol_swap.Enabled = this.checkBox_swap_acc.Checked;
        }

        private void checkBox_unvalid_accs_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_kol_unvalid.Enabled = this.checkBox_unvalid_accs.Checked;
        }

        private void dataGridView_all_accs_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    DataGridView.HitTestInfo info = this.dataGridView_all_accs.HitTest(e.X, e.Y);
                    this.dataGridView_all_accs.ClearSelection();
                    this.dataGridView_all_accs.Rows[info.RowIndex].Selected = true;
                }
                catch
                {
                }
                this.contextMenu.Show(this.dataGridView_all_accs, e.X, e.Y);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //Activate activate = new Activate();
            //if (!File.Exists("key.it") || (File.ReadAllText("key.it") != activate.GetKey()))
            //{
            //    activate.ShowDialog();
            //    if (!activate.flag)
            //    {
            //        base.Close();
            //    }
            //    else
            //    {
            //        File.WriteAllText("key.it", activate.GetKey());
            //    }
            //}
            //if (DateTime.Now >= DateTime.Parse("28.01.2017"))
            //{
            //    MessageBox.Show("Истек демо период!");
            //    Application.Exit();
            //}
            this.dataGridView_all_accs.AutoGenerateColumns = false;
            this.dataGridView_all_accs.AllowUserToAddRows = false;
            this.dataGridView_all_accs.DataSource = WorkBD.DownloadBD();
            dataGridView_all_accs.Rows[0].Selected = false;
            int current_index = 0;
            string current_phone = WorkBD.CurAcc();
            for (int n = 0; n < dataGridView_all_accs.RowCount; n++)
            {
                if (dataGridView_all_accs.Rows[n].Cells[3].Value.ToString() == current_phone)
                {
                    current_index = n;
                    break;
                }
            }
            dataGridView_all_accs.Rows[current_index].Selected = true;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl_general = new System.Windows.Forms.TabControl();
            this.tabPage_settings = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_pause = new System.Windows.Forms.NumericUpDown();
            this.checkBox_data = new System.Windows.Forms.CheckBox();
            this.checkBox_toFirst = new System.Windows.Forms.CheckBox();
            this.numericUpDown_kol_unvalid = new System.Windows.Forms.NumericUpDown();
            this.checkBox_unvalid_accs = new System.Windows.Forms.CheckBox();
            this.numericUpDown_kol_swap = new System.Windows.Forms.NumericUpDown();
            this.checkBox_swap_acc = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_change_acc = new System.Windows.Forms.Button();
            this.button_new_acc = new System.Windows.Forms.Button();
            this.dataGridView_all_accs = new System.Windows.Forms.DataGridView();
            this.NumberSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UseSend = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Login = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_numbers = new System.Windows.Forms.TabPage();
            this.dataGridView_all_numbers = new System.Windows.Forms.DataGridView();
            this.NumberSerial1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Numbers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sended = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_load_numbers = new System.Windows.Forms.Button();
            this.textBox_path_numbers = new System.Windows.Forms.TextBox();
            this.button_browse_numbers = new System.Windows.Forms.Button();
            this.tabPage_subjects = new System.Windows.Forms.TabPage();
            this.textBox_link = new System.Windows.Forms.TextBox();
            this.checkBox_link = new System.Windows.Forms.CheckBox();
            this.button_picture = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_file = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.richTextBox_text = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl_general.SuspendLayout();
            this.tabPage_settings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_pause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_kol_unvalid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_kol_swap)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_all_accs)).BeginInit();
            this.tabPage_numbers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_all_numbers)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabPage_subjects.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl_general
            // 
            this.tabControl_general.Controls.Add(this.tabPage_settings);
            this.tabControl_general.Controls.Add(this.tabPage_numbers);
            this.tabControl_general.Controls.Add(this.tabPage_subjects);
            this.tabControl_general.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_general.Location = new System.Drawing.Point(0, 0);
            this.tabControl_general.Name = "tabControl_general";
            this.tabControl_general.SelectedIndex = 0;
            this.tabControl_general.Size = new System.Drawing.Size(652, 382);
            this.tabControl_general.TabIndex = 0;
            // 
            // tabPage_settings
            // 
            this.tabPage_settings.Controls.Add(this.groupBox2);
            this.tabPage_settings.Controls.Add(this.groupBox1);
            this.tabPage_settings.Location = new System.Drawing.Point(4, 22);
            this.tabPage_settings.Name = "tabPage_settings";
            this.tabPage_settings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_settings.Size = new System.Drawing.Size(644, 356);
            this.tabPage_settings.TabIndex = 0;
            this.tabPage_settings.Text = "Настройки рассылки";
            this.tabPage_settings.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numericUpDown_pause);
            this.groupBox2.Controls.Add(this.checkBox_data);
            this.groupBox2.Controls.Add(this.checkBox_toFirst);
            this.groupBox2.Controls.Add(this.numericUpDown_kol_unvalid);
            this.groupBox2.Controls.Add(this.checkBox_unvalid_accs);
            this.groupBox2.Controls.Add(this.numericUpDown_kol_swap);
            this.groupBox2.Controls.Add(this.checkBox_swap_acc);
            this.groupBox2.Location = new System.Drawing.Point(9, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(627, 119);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Настройки анти- бан системы";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(369, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Пауза между сменой аккаунта";
            // 
            // numericUpDown_pause
            // 
            this.numericUpDown_pause.Location = new System.Drawing.Point(539, 17);
            this.numericUpDown_pause.Name = "numericUpDown_pause";
            this.numericUpDown_pause.Size = new System.Drawing.Size(73, 20);
            this.numericUpDown_pause.TabIndex = 6;
            // 
            // checkBox_data
            // 
            this.checkBox_data.AutoSize = true;
            this.checkBox_data.Location = new System.Drawing.Point(7, 73);
            this.checkBox_data.Name = "checkBox_data";
            this.checkBox_data.Size = new System.Drawing.Size(178, 17);
            this.checkBox_data.TabIndex = 5;
            this.checkBox_data.Text = "Добавлять дату к сообщению";
            this.checkBox_data.UseVisualStyleBackColor = true;
            // 
            // checkBox_toFirst
            // 
            this.checkBox_toFirst.AutoSize = true;
            this.checkBox_toFirst.Location = new System.Drawing.Point(7, 96);
            this.checkBox_toFirst.Name = "checkBox_toFirst";
            this.checkBox_toFirst.Size = new System.Drawing.Size(285, 17);
            this.checkBox_toFirst.TabIndex = 4;
            this.checkBox_toFirst.Text = "После последнего аккаунта переходить к первому";
            this.checkBox_toFirst.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_kol_unvalid
            // 
            this.numericUpDown_kol_unvalid.Enabled = false;
            this.numericUpDown_kol_unvalid.Location = new System.Drawing.Point(277, 44);
            this.numericUpDown_kol_unvalid.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDown_kol_unvalid.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_kol_unvalid.Name = "numericUpDown_kol_unvalid";
            this.numericUpDown_kol_unvalid.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown_kol_unvalid.TabIndex = 3;
            this.numericUpDown_kol_unvalid.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBox_unvalid_accs
            // 
            this.checkBox_unvalid_accs.AutoSize = true;
            this.checkBox_unvalid_accs.Location = new System.Drawing.Point(7, 45);
            this.checkBox_unvalid_accs.Name = "checkBox_unvalid_accs";
            this.checkBox_unvalid_accs.Size = new System.Drawing.Size(264, 17);
            this.checkBox_unvalid_accs.TabIndex = 2;
            this.checkBox_unvalid_accs.Text = "Число не валидных отправок для перезапуска";
            this.checkBox_unvalid_accs.UseVisualStyleBackColor = true;
            this.checkBox_unvalid_accs.CheckedChanged += new System.EventHandler(this.checkBox_unvalid_accs_CheckedChanged);
            // 
            // numericUpDown_kol_swap
            // 
            this.numericUpDown_kol_swap.Enabled = false;
            this.numericUpDown_kol_swap.Location = new System.Drawing.Point(153, 19);
            this.numericUpDown_kol_swap.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numericUpDown_kol_swap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_kol_swap.Name = "numericUpDown_kol_swap";
            this.numericUpDown_kol_swap.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown_kol_swap.TabIndex = 1;
            this.numericUpDown_kol_swap.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBox_swap_acc
            // 
            this.checkBox_swap_acc.AutoSize = true;
            this.checkBox_swap_acc.Location = new System.Drawing.Point(7, 20);
            this.checkBox_swap_acc.Name = "checkBox_swap_acc";
            this.checkBox_swap_acc.Size = new System.Drawing.Size(140, 17);
            this.checkBox_swap_acc.TabIndex = 0;
            this.checkBox_swap_acc.Text = "Менять аккаунт после";
            this.checkBox_swap_acc.UseVisualStyleBackColor = true;
            this.checkBox_swap_acc.CheckedChanged += new System.EventHandler(this.checkBox_swap_acc_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_change_acc);
            this.groupBox1.Controls.Add(this.button_new_acc);
            this.groupBox1.Controls.Add(this.dataGridView_all_accs);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(3, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(638, 221);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настроки аккаунтов Viber";
            // 
            // button_change_acc
            // 
            this.button_change_acc.Location = new System.Drawing.Point(227, 20);
            this.button_change_acc.Name = "button_change_acc";
            this.button_change_acc.Size = new System.Drawing.Size(195, 37);
            this.button_change_acc.TabIndex = 2;
            this.button_change_acc.Text = "Сменить пользователя";
            this.button_change_acc.UseVisualStyleBackColor = true;
            this.button_change_acc.Click += new System.EventHandler(this.button_change_acc_Click);
            // 
            // button_new_acc
            // 
            this.button_new_acc.Location = new System.Drawing.Point(6, 20);
            this.button_new_acc.Name = "button_new_acc";
            this.button_new_acc.Size = new System.Drawing.Size(195, 37);
            this.button_new_acc.TabIndex = 1;
            this.button_new_acc.Text = "Зарегистрировать новый Viber";
            this.button_new_acc.UseVisualStyleBackColor = true;
            this.button_new_acc.Click += new System.EventHandler(this.button_new_acc_Click);
            // 
            // dataGridView_all_accs
            // 
            this.dataGridView_all_accs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_all_accs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumberSerial,
            this.UseSend,
            this.Login,
            this.Number});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LimeGreen;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_all_accs.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_all_accs.Location = new System.Drawing.Point(3, 63);
            this.dataGridView_all_accs.MultiSelect = false;
            this.dataGridView_all_accs.Name = "dataGridView_all_accs";
            this.dataGridView_all_accs.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_all_accs.RowHeadersVisible = false;
            this.dataGridView_all_accs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_all_accs.Size = new System.Drawing.Size(630, 143);
            this.dataGridView_all_accs.TabIndex = 0;
            this.dataGridView_all_accs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView_all_accs_MouseClick);
            // 
            // NumberSerial
            // 
            this.NumberSerial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.NumberSerial.DataPropertyName = "№";
            this.NumberSerial.HeaderText = "№";
            this.NumberSerial.Name = "NumberSerial";
            this.NumberSerial.ReadOnly = true;
            this.NumberSerial.Width = 43;
            // 
            // UseSend
            // 
            this.UseSend.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UseSend.DataPropertyName = "IsValid";
            this.UseSend.FalseValue = "0";
            this.UseSend.HeaderText = "Использовать для рассылки";
            this.UseSend.Name = "UseSend";
            this.UseSend.TrueValue = "1";
            // 
            // Login
            // 
            this.Login.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Login.DataPropertyName = "NickName";
            this.Login.HeaderText = "Логин";
            this.Login.Name = "Login";
            this.Login.ReadOnly = true;
            // 
            // Number
            // 
            this.Number.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Number.DataPropertyName = "ID";
            this.Number.HeaderText = "Номер";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            // 
            // tabPage_numbers
            // 
            this.tabPage_numbers.Controls.Add(this.dataGridView_all_numbers);
            this.tabPage_numbers.Controls.Add(this.groupBox3);
            this.tabPage_numbers.Location = new System.Drawing.Point(4, 22);
            this.tabPage_numbers.Name = "tabPage_numbers";
            this.tabPage_numbers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_numbers.Size = new System.Drawing.Size(644, 356);
            this.tabPage_numbers.TabIndex = 1;
            this.tabPage_numbers.Text = "Номера";
            this.tabPage_numbers.UseVisualStyleBackColor = true;
            // 
            // dataGridView_all_numbers
            // 
            this.dataGridView_all_numbers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_all_numbers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NumberSerial1,
            this.Numbers,
            this.Sended});
            this.dataGridView_all_numbers.Location = new System.Drawing.Point(317, 7);
            this.dataGridView_all_numbers.Name = "dataGridView_all_numbers";
            this.dataGridView_all_numbers.RowHeadersVisible = false;
            this.dataGridView_all_numbers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_all_numbers.Size = new System.Drawing.Size(319, 323);
            this.dataGridView_all_numbers.TabIndex = 1;
            // 
            // NumberSerial1
            // 
            this.NumberSerial1.DataPropertyName = "№";
            this.NumberSerial1.HeaderText = "№";
            this.NumberSerial1.Name = "NumberSerial1";
            this.NumberSerial1.ReadOnly = true;
            this.NumberSerial1.Width = 43;
            // 
            // Numbers
            // 
            this.Numbers.DataPropertyName = "Номера";
            this.Numbers.HeaderText = "Номера";
            this.Numbers.Name = "Numbers";
            this.Numbers.ReadOnly = true;
            this.Numbers.Width = 137;
            // 
            // Sended
            // 
            this.Sended.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Sended.DataPropertyName = "Отправлено";
            this.Sended.HeaderText = "Отправлено";
            this.Sended.Name = "Sended";
            this.Sended.ReadOnly = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_load_numbers);
            this.groupBox3.Controls.Add(this.textBox_path_numbers);
            this.groupBox3.Controls.Add(this.button_browse_numbers);
            this.groupBox3.Location = new System.Drawing.Point(9, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(293, 141);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Файл с номерами";
            // 
            // button_load_numbers
            // 
            this.button_load_numbers.Location = new System.Drawing.Point(7, 68);
            this.button_load_numbers.Name = "button_load_numbers";
            this.button_load_numbers.Size = new System.Drawing.Size(85, 28);
            this.button_load_numbers.TabIndex = 2;
            this.button_load_numbers.Text = "Загрузить";
            this.button_load_numbers.UseVisualStyleBackColor = true;
            this.button_load_numbers.Click += new System.EventHandler(this.button_load_numbers_Click);
            // 
            // textBox_path_numbers
            // 
            this.textBox_path_numbers.Location = new System.Drawing.Point(47, 23);
            this.textBox_path_numbers.Name = "textBox_path_numbers";
            this.textBox_path_numbers.Size = new System.Drawing.Size(240, 20);
            this.textBox_path_numbers.TabIndex = 1;
            // 
            // button_browse_numbers
            // 
            this.button_browse_numbers.Location = new System.Drawing.Point(7, 20);
            this.button_browse_numbers.Name = "button_browse_numbers";
            this.button_browse_numbers.Size = new System.Drawing.Size(34, 24);
            this.button_browse_numbers.TabIndex = 0;
            this.button_browse_numbers.Text = "...";
            this.button_browse_numbers.UseVisualStyleBackColor = true;
            this.button_browse_numbers.Click += new System.EventHandler(this.button_browse_numbers_Click);
            // 
            // tabPage_subjects
            // 
            this.tabPage_subjects.Controls.Add(this.textBox_link);
            this.tabPage_subjects.Controls.Add(this.checkBox_link);
            this.tabPage_subjects.Controls.Add(this.button_picture);
            this.tabPage_subjects.Controls.Add(this.label1);
            this.tabPage_subjects.Controls.Add(this.textBox_file);
            this.tabPage_subjects.Controls.Add(this.groupBox4);
            this.tabPage_subjects.Location = new System.Drawing.Point(4, 22);
            this.tabPage_subjects.Name = "tabPage_subjects";
            this.tabPage_subjects.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_subjects.Size = new System.Drawing.Size(644, 356);
            this.tabPage_subjects.TabIndex = 2;
            this.tabPage_subjects.Text = "Сообщения";
            this.tabPage_subjects.UseVisualStyleBackColor = true;
            // 
            // textBox_link
            // 
            this.textBox_link.Enabled = false;
            this.textBox_link.Location = new System.Drawing.Point(357, 289);
            this.textBox_link.Name = "textBox_link";
            this.textBox_link.Size = new System.Drawing.Size(276, 20);
            this.textBox_link.TabIndex = 5;
            // 
            // checkBox_link
            // 
            this.checkBox_link.AutoSize = true;
            this.checkBox_link.Location = new System.Drawing.Point(357, 266);
            this.checkBox_link.Name = "checkBox_link";
            this.checkBox_link.Size = new System.Drawing.Size(231, 17);
            this.checkBox_link.TabIndex = 4;
            this.checkBox_link.Text = "Прикреплять ссылку (доп. сообщением)";
            this.checkBox_link.UseVisualStyleBackColor = true;
            this.checkBox_link.CheckedChanged += new System.EventHandler(this.checkBox_link_CheckedChanged);
            // 
            // button_picture
            // 
            this.button_picture.Location = new System.Drawing.Point(4, 289);
            this.button_picture.Name = "button_picture";
            this.button_picture.Size = new System.Drawing.Size(32, 20);
            this.button_picture.TabIndex = 3;
            this.button_picture.Text = "...";
            this.button_picture.UseVisualStyleBackColor = true;
            this.button_picture.Click += new System.EventHandler(this.button_picture_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Папка с файлами (отправляется случайная)";
            // 
            // textBox_file
            // 
            this.textBox_file.Location = new System.Drawing.Point(42, 289);
            this.textBox_file.Name = "textBox_file";
            this.textBox_file.Size = new System.Drawing.Size(266, 20);
            this.textBox_file.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.richTextBox_text);
            this.groupBox4.Location = new System.Drawing.Point(9, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(627, 249);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Текст для рассылки (рандомизация: {var1|var2|var3})";
            // 
            // richTextBox_text
            // 
            this.richTextBox_text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_text.Location = new System.Drawing.Point(3, 16);
            this.richTextBox_text.Name = "richTextBox_text";
            this.richTextBox_text.Size = new System.Drawing.Size(621, 230);
            this.richTextBox_text.TabIndex = 0;
            this.richTextBox_text.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(7, 389);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(641, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // button_start
            // 
            this.button_start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_start.Location = new System.Drawing.Point(7, 419);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(98, 32);
            this.button_start.TabIndex = 2;
            this.button_start.Text = "Старт";
            this.button_start.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(234, 419);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(98, 32);
            this.button_stop.TabIndex = 3;
            this.button_stop.Text = "Стоп";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem1});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(264, 48);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItem2.Text = "Начать рассылку с этого аккаунта";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(263, 22);
            this.toolStripMenuItem1.Text = "Использовать только этот аккаунт";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(120, 419);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 32);
            this.button1.TabIndex = 5;
            this.button1.Text = "Пауза";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 454);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tabControl_general);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "ViberSender2017 by ITLabs";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl_general.ResumeLayout(false);
            this.tabPage_settings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_pause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_kol_unvalid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_kol_swap)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_all_accs)).EndInit();
            this.tabPage_numbers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_all_numbers)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage_subjects.ResumeLayout(false);
            this.tabPage_subjects.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void LoadNumbers()
        {
            string[] a = File.ReadAllLines(this.textBox_path_numbers.Text);
            this.button_load_numbers.Enabled = false;
            new Thread((ThreadStart) delegate {
                DataTable dt = new DataTable();
                DataColumn[] columns = new DataColumn[3];
                DataColumn column1 = new DataColumn {
                    ColumnName = "Номера"
                };
                columns[0] = column1;
                DataColumn column2 = new DataColumn {
                    ColumnName = "Отправлено"
                };
                columns[1] = column2;
                DataColumn column3 = new DataColumn {
                    ColumnName = "№",
                    AutoIncrement = true,
                    AutoIncrementSeed = 1L
                };
                columns[2] = column3;
                dt.Columns.AddRange(columns);
                for (int j = 0; j < a.Length; j++)
                {
                    object[] values = new object[] { a[j], false };
                    dt.Rows.Add(values);
                }
                this.dataGridView_all_numbers.Invoke(new Action(() => this.dataGridView_all_numbers.DataSource = dt));
                this.button_load_numbers.Invoke(new Action(() => this.button_load_numbers.Enabled = true));
                Thread.CurrentThread.Abort();
            }) { IsBackground = true }.Start();
        }

        private void Solution()
        {
            int num5;
            this.progressBar1.Invoke(new Action (delegate{
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = this.dataGridView_all_numbers.Rows.Count - 1;
            }));
            MakeSends sends = new MakeSends();
            int num = 0;
            int num2 = 0;

            int current_index = 0;
            string current_phone = WorkBD.CurAcc();

            for (int n = 0; n < dataGridView_all_accs.RowCount; n++)
            {
                if (dataGridView_all_accs.Rows[n].Cells[3].Value.ToString() == current_phone)
                {
                    current_index = n;
                    break;
                }
            }

            for (int i = current_index; i < this.dataGridView_all_accs.Rows.Count; i = num5 + 1)
            {
                Thread.Sleep((int) this.numericUpDown_pause.Value);
                bool first = true;
                for (int k = 0; k < this.dataGridView_all_accs.SelectedRows.Count; k++)
                {
                    this.dataGridView_all_accs.SelectedRows[k].Selected = false;
                }
                this.dataGridView_all_accs.Rows[i].Selected = true;
                this.dataGridView_all_accs.Invoke(new Action(() => this.dataGridView_all_accs.FirstDisplayedScrollingRowIndex = i));
                if (this.dataGridView_all_accs.Rows[i].Cells[1].Value.ToString() != "0")
                {
                    IEnumerable<Process> source = from f in Process.GetProcesses()
                        where f.ProcessName == "Viber"
                        select f;
                    if (source.Any<Process>())
                    {
                        source.First<Process>().Kill();
                        source.First<Process>().WaitForExit(0x1388);
                    }
                    string phone = this.dataGridView_all_accs.Rows[i].Cells[3].Value.ToString();
                    WorkBD.SetAcc(phone);
                    WorkBD.SetLanguage(phone);
                    WorkBD.ClearHistory(phone);
                    Process.Start(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + @"\Users\" + Environment.UserName + @"\AppData\Local\Viber\Viber.exe");
                    while (WinApi.FindWindow("Qt5QWindowOwnDCIcon", null) == IntPtr.Zero)
                    {
                        Thread.Sleep(0x3e8);
                    }
                    Thread.Sleep(0xbb8);
                    if ((WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero) || (WinApi.FindWindow("Qt5QWindowOwnDCIcon", "Viber") != IntPtr.Zero))
                    {
                        WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        Thread.Sleep(0x7d0);
                    }
                    else
                    {
                        for (int j = num; j < (this.dataGridView_all_numbers.Rows.Count - 1); j = num5 + 1)
                        {
                            for (int m = 0; m < this.dataGridView_all_numbers.SelectedRows.Count; m++)
                            {
                                this.dataGridView_all_numbers.SelectedRows[m].Selected = false;
                            }
                            this.dataGridView_all_numbers.Rows[j].Selected = true;
                            this.dataGridView_all_numbers.Invoke(new Action(() => this.dataGridView_all_numbers.FirstDisplayedScrollingRowIndex = j));
                            num = j;
                            string input = this.richTextBox_text.Text; // (string) this.richTextBox_text.Invoke(new Action(() => this.richTextBox_text.Text = this.richTextBox_text.Text));
                            MatchCollection matchs = Regex.Matches(input, "{.*?}");
                            foreach (Match match in matchs)
                            {
                                char[] separator = new char[] { '|' };
                                string[] strArray = match.Value.Split(separator);
                                input = input.Replace(match.Value, strArray[this.r.Next(0, strArray.Length)].Replace("{", "").Replace("}", ""));
                            }
                            if (this.checkBox_data.Checked)
                            {
                                input = input + "\n" + DateTime.Now;
                            }
                            bool flag7 = false;
                            if (!string.IsNullOrEmpty(this.textBox_file.Text))
                            {
                                flag7 = sends.SendSms(this.dataGridView_all_numbers.Rows[j].Cells[0].Value.ToString(), input, first, this.textBox_file.Text);
                            }
                            else
                            {
                                flag7 = sends.SendSms(this.dataGridView_all_numbers.Rows[j].Cells[0].Value.ToString(), input, first, null);
                            }
                            first = false;
                            if (!flag7)
                            {
                                num2++;
                                this.dataGridView_all_numbers.Rows[j].Cells[1].Value = false;
                            }
                            else
                            {
                                this.dataGridView_all_numbers.Rows[j].Cells[1].Value = true;
                            }
                            this.progressBar1.Invoke(new Action(delegate {
                                this.progressBar1.Value++;
                                if (this.progressBar1.Value == this.progressBar1.Maximum)
                                {
                                    this.button_start.Enabled = true;
                                    this._th.Abort();
                                }
                            }));
                            if (this.checkBox_link.Checked && !string.IsNullOrEmpty(this.textBox_link.Text))
                            {
                                Thread.Sleep(500);
                                WinApi.SendMsg(this.textBox_link.Text, null, false);
                            }
                            if (this.checkBox_unvalid_accs.Checked && (num2 >= this.numericUpDown_kol_unvalid.Value))
                            {
                                num2 = 0;
                                break;
                            }
                            if (this.checkBox_swap_acc.Checked && (((j + 1) % this.numericUpDown_kol_swap.Value) == decimal.Zero))
                            {
                                num++;
                                break;
                            }
                            num5 = j;
                        }
                        if (this.checkBox_toFirst.Checked && ((i + 2) > this.dataGridView_all_accs.Rows.Count))
                        {
                            i = -1;
                        }
                        Thread.Sleep(0xbb8);
                    }
                }
                num5 = i;
            }
            this.button_start.Invoke(new Action(() => this.button_start.Enabled = true));
        }

        [Serializable, CompilerGenerated]
        private sealed class c
        {
            public static readonly MainForm.c r9 = new MainForm.c();
            public static Func<Process, bool> r9__12_2;
            public static Func<Process, bool> r9__6_0;

            internal bool button_new_acc_Clickb__6_0(Process f) =>
                (f.ProcessName == "Viber");

            internal bool Solutionb__12_2(Process f) =>
                (f.ProcessName == "Viber");
        }


    }
}

