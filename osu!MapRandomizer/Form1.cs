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
        List<FileInfo> LoadedMaps = new List<FileInfo>();

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
            foreach(DirectoryInfo info in bmaps)
            {
                foreach(FileInfo i in info.EnumerateFiles("*.osu", SearchOption.TopDirectoryOnly))
                {
                    LoadedMaps.Add(i);
                }
            }
            Log($"Found {dirs.Count} directories and {LoadedMaps.Count} beatmaps.");
            button2.Enabled = true;
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
            Directory.Delete("RandomizedMap", true);
            Random r = new Random();
            var kmap = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
            var ksong = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
            Beatmap map = BeatmapDecoder.Decode(kmap.FullName);
            Beatmap song = BeatmapDecoder.Decode(kmap.FullName);

            while (map.GeneralSection.Mode != song.GeneralSection.Mode)
            {
                kmap = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                ksong = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
            }

            map.TimingPoints = song.TimingPoints;
            map.GeneralSection.AudioFilename = song.GeneralSection.AudioFilename;
            map.GeneralSection.Length = song.GeneralSection.Length;
            map.MetadataSection.Artist =  "osu!map randomizer";
            map.MetadataSection.Title = "Randomized map";
            map.MetadataSection.Creator = "osu!map randomizer";
            //create map directory
            Directory.CreateDirectory("RandomizedMap");
            //copy audio file
            File.Copy(Path.Combine(Path.GetDirectoryName(ksong.FullName), song.GeneralSection.AudioFilename), Path.Combine("RandomizedMap", song.GeneralSection.AudioFilename));
            //copy background
            File.Copy(Path.Combine(Path.GetDirectoryName(kmap.FullName), song.EventsSection.BackgroundImage), Path.Combine("RandomizedMap", song.EventsSection.BackgroundImage));
            map.Save(Path.Combine("RandomizedMap", "map.osu"));
        }
    }
}
