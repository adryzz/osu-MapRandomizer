using System.Collections.Generic;
using System.IO;

namespace osu_MapRandomizer
{
    public class Beatmap
    {
        public List<BeatmapDifficulty> Difficulties = new List<BeatmapDifficulty>();

        public void Export()
        {
            Directory.CreateDirectory("Map");
            BeatmapDifficulty d = Difficulties[0];
            File.Copy(d.Song.AudioFile.FullName, "Map\\audio.mp3", true);
            File.Copy(d.Background.FullName, Path.Combine("Map", d.Background.Name), true);
            string contents = $"osu file format v14\n\n[General]\nAudioFilename: audio.mp3\n";
            contents += $"AudioLeadIn: {d.Song.AudioLeadIn}\n";
            contents += $"PreviewTime: {d.Song.PreviewTime}\n";
            contents += $"Countdown: {d.Song.Countdown}\n";
            contents += "SampleSet: Normal\nStackLeniency: 0.3\n\nMode: 0\nLetterboxInBreaks: 0WidescreenStoryboard: 1\n\n";

            contents += "[Editor]\nBookmarks: 191448,375140\nDistanceSpacing: 1\nBeatDivisor: 4\nGridSize: 4\nTimelineZoom: 1.7\n\n";

            contents += $"[Metadata]\nTitle:{d.Title}\n";
            contents += $"TitleUnicode:{d.TitleUnicode}\n";
            contents += $"Artist:{d.Artist}\n";
            contents += $"ArtistUnicode:{d.ArtistUnicode}\n";
            contents += $"Creator:{d.Creator}\n";
            contents += $"Version:{d.Version}\n";
            contents += $"Source:{d.Source}\n";
            contents += $"Tags:{d.Tags}\n";
            contents += $"BeatmapID:{d.BeatmapID}\n";
            contents += $"BeatmapSetID:{d.BeatmapSetID}\n\n";

            contents += $"[Difficulty]\nHPDrainRate::{d.HP}\n";
            contents += $"CircleSize:{d.CS}\n";
            contents += $"OverallDifficulty:{d.OD}\n";
            contents += $"ApproachRate{d.AR}\n";
            contents += $"SliderMultiplier:{d.SM}\n";
            contents += $"SliderTickRate:{d.TR}\n\n";

            contents += $"[Events]\n//Background and Video events\n0,0,\"{d.Background.Name}\",0,0\n//Break Periods\n//Storyboard Layer 0 (Background)\n//Storyboard Layer 1 (Fail)\n//Storyboard Layer 2 (Pass)\n//Storyboard Layer 3 (Foreground)\n\n//Storyboard Sound Samples\n\n";

            contents += d.Song.TimingPoints + "\n\n\n";
            contents += d.HitObjects;
            File.WriteAllText(Path.Combine("Map", d.Artist + " - " + d.Title + ".osu"), contents);
        }
    }
}