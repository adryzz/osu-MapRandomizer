using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace osu_MapRandomizer
{
    public class BeatmapDifficulty
    {
        public FileInfo MapFile;

        public bool IsValid = true;

        public float BPM = 0;

        public float CS = 0;//circle size
        public float AR = 0;//approach rate
        public float OD = 0;//overall difficulty
        public float HP = 0;//HP
        public float SM = 0;//slider multiplier
        public float TR = 0;//slider tick rate

        public string Title = "Randomized song";
        public string TitleUnicode = "Randomized song";
        public string Artist = "osu! map randomizer";
        public string ArtistUnicode = "osu! map randomizer";
        public string Creator = "Adryzz";
        public string Version = "Random";
        public string Source = "osu! map randomizer";
        public string Tags = "Random";
        public int BeatmapID = 9999999;
        public int BeatmapSetID = 7777777;

        public BeatmapSong Song;

        public FileInfo Background;

        public string HitObjects = "";
        public BeatmapDifficulty()
        {

        }

        public BeatmapDifficulty(FileInfo file)
        {
            MapFile = file;
            Song = new BeatmapSong();
            string fileData = File.ReadAllText(file.FullName);
            List<string> lines = fileData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int generalIndex = 2;//generally it's 2

            int diffIndex = 33;//generally it's 33

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("[General]"))
                {
                    generalIndex = i;
                }
                else if (i == generalIndex + 1)
                {
                    //audio file is here
                    if (lines[i].StartsWith("AudioFilename: "))
                    {
                        string name = lines[i].Replace("AudioFilename: ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        Song.AudioFile = new FileInfo(Path.Combine(MapFile.Directory.FullName, name));
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == generalIndex + 2)
                {
                    //audio leadin
                    if (lines[i].StartsWith("AudioLeadIn: "))
                    {
                        string value = lines[i].Replace("AudioLeadIn: ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        Song.AudioLeadIn = long.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == generalIndex + 3)
                {
                    //preview time
                    if (lines[i].StartsWith("PreviewTime: "))
                    {
                        string value = lines[i].Replace("PreviewTime: ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        Song.PreviewTime = long.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == generalIndex + 4)
                {
                    //countdown
                    if (lines[i].StartsWith("Countdown: "))
                    {
                        string value = lines[i].Replace("Countdown: ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        Song.Countdown = int.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (lines[i].StartsWith("[Difficulty]"))
                {
                    diffIndex = i;
                }
                else if (i == diffIndex + 1)
                {
                    if (lines[i].StartsWith("HPDrainRate:"))
                    {
                        string value = lines[i].Replace("HPDrainRate:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        HP = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == diffIndex + 2)
                {
                    if (lines[i].StartsWith("CircleSize:"))
                    {
                        string value = lines[i].Replace("CircleSize:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        CS = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == diffIndex + 3)
                {
                    if (lines[i].StartsWith("OverallDifficulty:"))
                    {
                        string value = lines[i].Replace("OverallDifficulty:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        OD = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == diffIndex + 4)
                {
                    if (lines[i].StartsWith("ApproachRate:"))
                    {
                        string value = lines[i].Replace("ApproachRate:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        AR = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == diffIndex + 5)
                {
                    if (lines[i].StartsWith("SliderMultiplier:"))
                    {
                        string value = lines[i].Replace("SliderMultiplier:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        SM = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (i == diffIndex + 6)
                {
                    if (lines[i].StartsWith("SliderTickRate:"))
                    {
                        string value = lines[i].Replace("SliderTickRate:", "").Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        TR = float.Parse(value);
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                else if (lines[i].StartsWith("0,0,\""))
                {
                    try
                    {
                        string value = lines[i];
                        value = value.Remove(0, value.IndexOf('"') + 1);
                        value = value.Remove(value.IndexOf('"'));
                        Background = new FileInfo(Path.Combine(MapFile.Directory.FullName, value));
                    }
                    catch (Exception)
                    {
                        IsValid = false;
                    }
                }
                else if (lines[i].StartsWith("[Colours]") && fileData.Contains("[Colours]"))
                {
                    HitObjects = fileData.Substring(fileData.IndexOf("[Colours]"));
                }
                else if (fileData.StartsWith("[HitObjects]") && !fileData.Contains("[Colours]"))
                {
                    HitObjects = fileData.Substring(fileData.IndexOf("[HitObjects]"));
                }
            }
            if (fileData.Contains("[Colours]"))
            {
                string timing = fileData.Remove(0, fileData.IndexOf("[TimingPoints]"));
                timing = timing.Remove(timing.IndexOf("[Colours]") - 6);
                Song.TimingPoints = timing;
            }
            else
            {
                string timing = fileData.Remove(0, fileData.IndexOf("[TimingPoints]"));
                timing = timing.Remove(timing.IndexOf("[HitObjects]") - 6);
                Song.TimingPoints = timing;
            }
        }
    }
}