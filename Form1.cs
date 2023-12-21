using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WinFormsControls;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string version = "";
        public string serverVersion = "";
        public string path = @"Game\";
        public string exename = "Lethal Company.exe";
        public string url = "https://octoland.ru/games/lc/";
        public bool downloading = false;

        PrivateFontCollection pfc = new PrivateFontCollection();
        public void InitFont()
        {
            pfc.AddFontFile("Chava.ttf");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            InitFont();
            //pictureBox1.BackgroundImage = new Bitmap("background.jpg");
            pictureBox2.BackgroundImage = new Bitmap("loadbar.png");
            label1.Font = new Font(pfc.Families[0], 20, FontStyle.Regular);
            transparentLabel1.Font = new Font(pfc.Families[0], 20, FontStyle.Regular);
            label1.BringToFront();
            button1.Font = new Font(pfc.Families[0], label1.Font.Size);



            Directory.CreateDirectory(Path.GetFullPath(path));
            if (!File.Exists(Path.GetFullPath(path + exename)))
            {
                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey("lethalLauncher");
                key.SetValue("version", "none");
                key.Close();
                version = "none";
            }
            else
            {
                version = (string)Registry.CurrentUser.OpenSubKey("lethalLauncher").GetValue("version");
            }


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "v.txt");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream recivestream = response.GetResponseStream();
                StreamReader readstream = null;
                if (response.CharacterSet == null)
                {
                    readstream = new StreamReader(recivestream);
                }
                else
                {
                    readstream = new StreamReader(recivestream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readstream.ReadToEnd();
                serverVersion = data;
                response.Close();
                readstream.Close();
            }
            else
            {
                MessageBox.Show("Error");
            }

            label1.Text = version + "/" + serverVersion;
            if (!downloading)
            {
                if (version != serverVersion)
                {
                    if (File.Exists(Path.GetFullPath(path + exename)))
                    {
                        button1.Text = "Update to " + serverVersion;
                        button1.Click += UpdateGame;
                    }
                    else
                    {
                        button1.Text = "Download";
                        button1.Click += UpdateGame;
                    }
                }
                else
                {
                    button1.Text = "Play";
                    button1.Click += Play;
                }
            }
        }
        
        private void UpdateGame(object sender, EventArgs e)
        {
            downloading = false;
            button1.ForeColor = Color.Gray;
            button1.Enabled = false;
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(path));
            foreach (FileInfo item in dir.GetFiles())
            {
                item.Delete();
            }
            foreach (DirectoryInfo item in dir.GetDirectories())
            {
                item.Delete(true);
            }


            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += (s, g) => { 
                    if (g.ProgressPercentage == 100)
                    {
                        if (!File.Exists(Path.GetFullPath(path + exename)))
                        {
                            ZipFile.ExtractToDirectory("game.zip", path);
                            RegistryKey key;
                            key = Registry.CurrentUser.CreateSubKey("lethalLauncher");
                            key.SetValue("version", serverVersion);
                            key.Close();
                            version = serverVersion;
                            label1.Text = version + "/" + serverVersion;

                            button1.Text = "Play";
                            button1.Click -= UpdateGame;
                            button1.Click += Play; ;

                            button1.ForeColor = Color.White;
                        }
                        button1.Enabled = true;
                        downloading = false;
                    }
                    transparentLabel1.Text = (g.BytesReceived / 1024) + "/" + (g.TotalBytesToReceive / 1024);
                    pictureBox2.Width = (int)(591f * (g.ProgressPercentage / 100f));
                };
                wc.DownloadFileAsync(new Uri(url + "current.zip"), "game.zip");
            }
        }

        private void Play(object sender, EventArgs e)
        {
            Process.Start(path + exename);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
