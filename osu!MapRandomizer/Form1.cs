using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;

namespace osu_MapRandomizer
{
    public partial class Form1 : Form
    {
        Dictionary<DirectoryInfo, Beatmap> LoadedMaps = new Dictionary<DirectoryInfo, Beatmap>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = Directory.Exists(textBox1.Text);//allow to load beatmaps only if the songs directory exists
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadedMaps.Clear();
            button2.Enabled = false;
            button1.Enabled = false;
            List<string> dirs = Directory.EnumerateDirectories(Path.GetFullPath(textBox1.Text)).ToList();
            List<DirectoryInfo> bmaps = new List<DirectoryInfo>();
            foreach (string s in dirs)
            {
                DirectoryInfo beatmapInfo = new DirectoryInfo(s);
                if (char.IsDigit(beatmapInfo.Name[0])) //beatmaps start with the beatmap ID first, that is a number
                {
                    bmaps.Add(beatmapInfo);
                }
            }
            List<FileInfo> fmaps = new List<FileInfo>();
            foreach(DirectoryInfo info in bmaps)
            {
                foreach(FileInfo i in info.EnumerateFiles("*.osu", SearchOption.TopDirectoryOnly))
                {
                    fmaps.Add(i);
                }
            }
            Log($"Found {dirs.Count} directories and {fmaps.Count} beatmaps.");
            progressBar1.Maximum = fmaps.Count;
            label2.Text = $"0/{fmaps.Count} maps loaded.";
            new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < fmaps.Count; i++)
                {

                    Beatmap map = BeatmapDecoder.Decode(fmaps[i].FullName);
                    if (map != null)
                    {
                        LoadedMaps.Add(new DirectoryInfo(Path.GetDirectoryName(fmaps[i].FullName)), map);
                    }
                    else
                    {
                        Log($"{fmaps[i].Name} couldn't be loaded.");
                    }
                    Invoke(new UIUpdateDelegate(() =>
                    {
                        label2.Text = $"{i + 1}/{fmaps.Count} maps loaded.";
                        progressBar1.Value = i + 1;
                        if (i + 1 == fmaps.Count)
                        {
                            button1.Enabled = true;
                            button2.Enabled = true;
                        }
                    }));
                }
            })).Start();
        }
        delegate void UIUpdateDelegate();

        private void button2_Click(object sender, EventArgs e)
        {
            RandomizeBeatmap();
        }

        void Log(string text)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new UIUpdateDelegate(() => Log(text)));
            }
            else
            {
                string[] messages = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (messages.Length > 0 && messages.Length < 2)
                {
                    if (text.StartsWith(" ")) text = text.Remove(0);
                    listBox1.Items.Add(text);
                    listBox1.TopIndex = listBox1.Items.Count - 1;
                }
                else if (messages.Length > 1)
                {
                    foreach (string s in messages)
                    {
                        Log(s);
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/adryzz");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/realTobby/OsuSkinRandomizer");
        }

        void RandomizeBeatmap()
        {
            Random r = new Random();
            var kmap = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
            var ksong = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));

            while (kmap.Value.GeneralSection.Mode != ksong.Value.GeneralSection.Mode)
            {
                kmap = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                ksong = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
            }

            kmap.Value.TimingPoints = ksong.Value.TimingPoints;
            kmap.Value.GeneralSection.AudioFilename = ksong.Value.GeneralSection.AudioFilename;
            kmap.Value.GeneralSection.Length = ksong.Value.GeneralSection.Length;
            kmap.Value.MetadataSection.Artist =  "osu!map randomizer";
            kmap.Value.MetadataSection.Title = "Randomized map";
            kmap.Value.MetadataSection.Creator = "osu!map randomizer";
            Directory.CreateDirectory("RandomizedMap");
            File.Copy(Path.Combine(ksong.Key.FullName, ksong.Value.GeneralSection.AudioFilename), Path.Combine("RandomizedMap", ksong.Value.GeneralSection.AudioFilename));
            kmap.Value.Save(Path.Combine("RandomizedMap", "map.osu"));
        }
    }
}
