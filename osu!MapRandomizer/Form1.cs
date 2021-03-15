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
using System.IO.Compression;
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
            try
            {
                
            }
            catch
            {

            }
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
            if (Directory.Exists("RandomizedMap"))
            {
                Directory.Delete("RandomizedMap", true);
            }

            if (File.Exists("RandomizedMap.osz"))
            {
                File.Delete("RandomizedMap.osz");
            }

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
            //create map directory
            Directory.CreateDirectory("RandomizedMap");

            map.TimingPoints = song.TimingPoints;
            map.GeneralSection.AudioFilename = song.GeneralSection.AudioFilename;
            map.GeneralSection.Length = song.GeneralSection.Length;
            map.EventsSection.Breaks = song.EventsSection.Breaks;

            //copy audio file
            File.Copy(Path.Combine(Path.GetDirectoryName(ksong.FullName), song.GeneralSection.AudioFilename), Path.Combine("RandomizedMap", song.GeneralSection.AudioFilename));

            //video randomizer
            if (checkedListBox1.GetItemChecked(2))
            {
                var kvid = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                Beatmap vid = BeatmapDecoder.Decode(kvid.FullName);
                while (string.IsNullOrEmpty(map.EventsSection.Video))
                {
                    kvid = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                    vid = BeatmapDecoder.Decode(kvid.FullName);
                }
                map.EventsSection.Video = vid.EventsSection.Video;
                map.EventsSection.VideoOffset = vid.EventsSection.VideoOffset;
                //copy video
                File.Copy(Path.Combine(Path.GetDirectoryName(kvid.FullName), vid.EventsSection.Video), Path.Combine("RandomizedMap", vid.EventsSection.Video));
            }

            //storyboard randomizer
            /*
            if (checkedListBox1.GetItemChecked(3))
            {
                var ksb = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                Beatmap sb = BeatmapDecoder.Decode(ksb.FullName);
                while (!sb.EventsSection.Storyboard.)
                {
                    ksb = LoadedMaps.ElementAt(r.Next(0, LoadedMaps.Count));
                    sb = BeatmapDecoder.Decode(ksb.FullName);
                }
                map.EventsSection.Video = sb.EventsSection.Video;
                map.EventsSection.VideoOffset = sb.EventsSection.VideoOffset;
                //copy video
                File.Copy(Path.Combine(Path.GetDirectoryName(ksb.FullName), sb.EventsSection.Video), Path.Combine("RandomizedMap", sb.EventsSection.Video));
            }
            */

            if(checkedListBox1.GetItemChecked(4))
            {
                map.DifficultySection.CircleSize = r.Next(0, 100) / 10f;
            }

            if (checkedListBox1.GetItemChecked(5))
            {
                map.DifficultySection.ApproachRate = r.Next(0, 100) / 10f;
            }

            if (checkedListBox1.GetItemChecked(6))
            {
                map.DifficultySection.OverallDifficulty = r.Next(0, 100) / 10f;
            }

            if (checkedListBox1.GetItemChecked(7))
            {
                map.DifficultySection.HPDrainRate = r.Next(0, 100) / 10f;
            }

            map.MetadataSection.Artist =  "osu!map randomizer";
            map.MetadataSection.Title = "Randomized map";
            map.MetadataSection.Creator = "osu!map randomizer";
     
            //copy background
            File.Copy(Path.Combine(Path.GetDirectoryName(kmap.FullName), song.EventsSection.BackgroundImage), Path.Combine("RandomizedMap", song.EventsSection.BackgroundImage));
            map.Save(Path.Combine("RandomizedMap", "map.osu"));
            ZipFile.CreateFromDirectory(Path.GetFullPath("RandomizedMap"), "RandomizedMap.osz");
            Process.Start(Path.GetFullPath("RandomizedMap.osz"));
        }
    }
}
