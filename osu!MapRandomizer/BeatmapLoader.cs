using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_MapRandomizer
{
    static class BeatmapLoader
    {
        public static Beatmap LoadBeatmap(DirectoryInfo dir)
        {
            Beatmap map = new Beatmap();
            List<FileInfo> diffs = dir.EnumerateFiles("*.osu").ToList();
            if (diffs.Count < 1)
            {
                return null;
            }
            foreach(FileInfo i in diffs)
            {
                BeatmapDifficulty diff = new BeatmapDifficulty(i);
                if (diff.IsValid)
                {
                    map.Difficulties.Add(diff);
                }
            }
            return map;
        }
    }
}
