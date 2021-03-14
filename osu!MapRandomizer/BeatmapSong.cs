using System.IO;

namespace osu_MapRandomizer
{
    public struct BeatmapSong
    {
        public FileInfo AudioFile;
        public long AudioLeadIn;
        public long PreviewTime;
        public int Countdown;
        public string TimingPoints;
    }
}