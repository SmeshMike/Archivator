using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp
{
    public partial class MainForm : Form
    {
        string choosenFile;
        bool isArch = false;

        public MainForm()
        {
            InitializeComponent();
            this.button1.Enabled = false;
            this.panel1.AllowDrop = true;
            this.panel1.DragEnter += Panel1_DragEnter;
            this.panel1.DragLeave += Panel1_DragLeave;
            this.panel1.DragDrop += Panel1_DragDrop;

            this.progressBar1.Maximum = 100;

            this.button1.Click += Button1_Click;
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.button1.Text = "loading";

            Archivating.Archivator arch = new Archivating.Archivator(choosenFile);
            arch.LoadChanged += Arch_LoadChanged;

            await arch.Upload();

            if (!isArch)
            {
                await arch.Archive();
                await arch.SaveFile();
            }
            else
            {
                await arch.DeArchive();
                await arch.SaveFileWithoutArch();
            }

            this.progressBar1.Value = 100;
            MessageBox.Show(this, "Done", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!isArch)
            {
                var res = MessageBox.Show(this, "Show log?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                    Process.Start(Path.Combine(arch.appDataPath, "last.log"));
            }

            GoStart();
        }

        private void Arch_LoadChanged(double proc)
        {
            this.BeginInvoke(new Action(() => { this.progressBar1.Value = Convert.ToInt32(proc * 100); }));
        }

        private void Panel1_DragDrop(object sender, DragEventArgs e)
        {

            setFile(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
            setUnDrop();
        }

        private void Panel1_DragLeave(object sender, EventArgs e)
        {
            setUnDrop();
        }

        private void Panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;

            label1.Text = "DROP HERE";
            this.panel1.BackColor = Color.FloralWhite;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            var res = ofd.ShowDialog();
            if(res == DialogResult.OK && ofd.FileNames.Length > 0)
            {
                setFile(ofd.FileNames[0]);
            }
        }

        private void setUnDrop()
        {
            this.panel1.BackColor = SystemColors.ControlLight;
            label1.Text = "CHOOSE FILE";
        }

        private void setFile(string text)
        {
            choosenFile = text;
            if (Directory.Exists(choosenFile))
            {
                MessageBox.Show(this, "No folders, sorry", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] extensions = { ".lnk"};

            if (extensions.Any( o => o == Path.GetExtension(choosenFile)))
            {
                MessageBox.Show(this, "Wrong extension", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(Path.GetExtension(choosenFile) == ".arch")
            {
                isArch = true;
            }
            else
            {
                isArch = false;
            }


            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(choosenFile);
            pictureBox1.Image = icon.ToBitmap();

            label2.Text = Path.GetFileName(choosenFile);

            if (isArch)
            {
                button1.Text = "Restore this file";
            }
            else
            {
                button1.Text = "Archivate this file";
            }
            button1.Enabled = true;

        }

        void GoStart()
        {
            label2.Text = "NoFile";
            pictureBox1.Image = new Bitmap(32,32);

            this.button1.Text = "choose one";
            this.button1.Enabled = false;
            this.progressBar1.Value = 0;
        }
    }
}
