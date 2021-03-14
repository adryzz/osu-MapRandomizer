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

namespace osu_MapRandomizer
{
    public partial class Form1 : Form
    {
        List<Beatmap> LoadedMaps = new List<Beatmap>();

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
            List<DirectoryInfo> maps = new List<DirectoryInfo>();
            foreach (string s in dirs)
            {
                DirectoryInfo beatmapInfo = new DirectoryInfo(s);
                if (char.IsDigit(beatmapInfo.Name[0])) //beatmaps start with the beatmap ID first, that is a number
                {
                    maps.Add(beatmapInfo);
                }
            }
            Log($"Found {dirs.Count} directories and {maps.Count} beatmaps.");
            progressBar1.Maximum = maps.Count;
            label2.Text = $"0/{maps.Count} maps loaded.";
            new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < maps.Count; i++)
                {
                    DirectoryInfo info = maps[i];
                    Beatmap map = BeatmapLoader.LoadBeatmap(info);
                    if (map != null)
                    {
                        LoadedMaps.Add(map);
                    }
                    else
                    {
                        Log($"{info.Name} couldn't be loaded.");
                    }
                    Invoke(new UIUpdateDelegate(() =>
                    {
                        label2.Text = $"{i + 1}/{maps.Count} maps loaded.";
                        progressBar1.Value = i + 1;
                        if (i + 1 == maps.Count)
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
            Beatmap map = RandomizeBeatmap();
            map.Export();
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

        Beatmap RandomizeBeatmap()
        {
            Random r = new Random();
            BeatmapDifficulty map = LoadedMaps[r.Next(0, LoadedMaps.Count)].Difficulties[0];
            Console.WriteLine(map.Artist + " - " + map.TitleUnicode);
            Beatmap song = LoadedMaps[r.Next(0, LoadedMaps.Count)];
            Console.WriteLine(song.Difficulties[0].Artist + " - " + song.Difficulties[0].TitleUnicode);
            map.Song = song.Difficulties[0].Song;
            map.Title = "Randomized song";
            map.TitleUnicode = "Randomized song";
            map.Artist = "osu! map randomizer";
            map.ArtistUnicode = "osu! map randomizer";
            map.Creator = "Adryzz";
            map.Version = "Random";
            map.Source = "osu! map randomizer";
            map.Tags = "Random";
            map.BeatmapID = 9999999;
            map.BeatmapSetID = 7777777;
            Beatmap end = new Beatmap();
            end.Difficulties.Add(map);
            return end;
        }
    }
}
