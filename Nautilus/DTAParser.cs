using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Nautilus.x360;

namespace Nautilus
{
    public class DTAParser
    {
        private NemoTools Tools;
        public List<SongData> Songs;

        private int Diff1;
        private int Diff2;
        private int Diff3;
        private int Diff4;
        private int Diff5;
        private int Diff6;
        public List<List<string>> DTAEntries;
        public byte[] DTA;

        /// <summary>
        /// Extracts DTA file to specified path
        /// </summary>
        /// <param name="xPackage">STFS Package (CON or LIVE file)</param>
        /// <param name="closeIO">Whether to close the STFS Package IO stream after extracting</param>
        /// <param name="doUpg">Extract upgrades.dta instead of songs.dta</param>
        /// <returns>Boolean True/False</returns>
        public bool ExtractDTA(STFSPackage xPackage, bool closeIO, bool doUpg = false)
        {
            try
            {
                Tools = new NemoTools();
                DTA = null;
                var xDTA = xPackage.GetFile(doUpg ? "songs_upgrades/upgrades.dta" : "songs/songs.dta");
                if (xDTA == null)
                {
                    if (closeIO) xPackage.CloseIO();
                    return false;
                }
                DTA = xDTA.Extract();
                if (closeIO) xPackage.CloseIO();
                if (DTA != null && DTA.Length > 0)
                {
                    ReadDTA(DTA);
                    return true;
                }
                else
                { return false; }
            }
            catch (Exception)
            {
                if (closeIO) xPackage.CloseIO();
                return false;
            }
        }

        public bool WriteDTAToFile(string dtaOUT)
        {
            if (DTA == null || DTA.Length == 0) return false;
            Tools.DeleteFile(dtaOUT);
            try
            {
                var encoding = DetectEncoding(DTA);
                var sw = new StreamWriter(dtaOUT, false, encoding);
                var sr = new StreamReader(new MemoryStream(DTA), encoding);
                sw.Write(sr.ReadToEnd());
                sr.Dispose();
                sw.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return File.Exists(dtaOUT);
        }

        /// <summary>
        /// Extracts DTA file to specified path
        /// </summary>
        /// <param name="xPackage">STFS Package (CON or LIVE file)</param>
        /// <returns>Boolean True/False</returns>
        public bool ExtractDTA(STFSPackage xPackage)
        {
            return ExtractDTA(xPackage, false);
        }

        /// <summary>
        /// Extracts DTA file to specified path
        /// </summary>
        /// <param name="xPackage">Full path to STFS package (CON or LIVE file)</param>
        /// <returns>Boolean True/False</returns>
        public bool ExtractDTA(string xPackage)
        {
            try
            {
                var xCON = new STFSPackage(xPackage);
                return xCON.ParseSuccess && ExtractDTA(xCON, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
                return false;
            }
        }

        private string CleanHSANLine(string line, string value)
        {
            return line.Replace(value, "").Replace(":", "").Replace(",", "").Replace("\"", "").Trim();
        }
        
        private enum Instrument
        {
            Lead, Rhythm, Bass
        }

        public bool ReadXMLFile(string xmlFile)
        {
            try
            {
                Tools = new NemoTools();
                Songs = new List<SongData>();
                var song = new SongData();
                song.Initialize();
                song.ChartAuthor = "Power Gig";
                var sr = new StreamReader(xmlFile);
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("<title>") && line.Contains("</title>"))
                        {
                            song.Name = line.Replace("/", "").Replace("<title>", "").Trim();
                        }
                        else if (line.Contains("<artist>") && line.Contains("</artist>"))
                        {
                            song.Artist = line.Replace("/", "").Replace("<artist>", "").Trim();
                        }
                        else if (line.Contains("<album>") && line.Contains("</album>"))
                        {
                            song.Album = line.Replace("/", "").Replace("<album>", "").Trim();
                        }
                        else if (line.Contains("<genre>") && line.Contains("</genre>"))
                        {
                            song.Genre = line.Replace("/", "").Replace("<genre>", "").Trim();
                        }
                        else if (line.Contains("<year>") && line.Contains("</year>"))
                        {
                            song.YearReleased = Convert.ToInt16(line.Replace("/", "").Replace("<year>", "").Trim());
                        }
                        else if (line.Contains("<singer_gender>") && line.Contains("</singer_gender>"))
                        {
                            song.Gender = line.Replace("/", "").Replace("<singer_gender>", "").Trim();
                        }
                        else if (line.Contains("<guitar_intensity>") && line.Contains("</guitar_intensity>"))
                        {
                            song.GuitarDiff = Convert.ToInt16(line.Replace("/", "").Replace("<guitar_intensity>", "").Trim());
                        }
                        else if (line.Contains("<drums_intensity>") && line.Contains("</drums_intensity>"))
                        {
                            song.DrumsDiff = Convert.ToInt16(line.Replace("/", "").Replace("<drums_intensity>", "").Trim());
                        }
                        else if (line.Contains("<vocals_intensity>") && line.Contains("</vocals_intensity>"))
                        {
                            song.VocalsDiff = Convert.ToInt16(line.Replace("/", "").Replace("<vocals_intensity>", "").Trim());
                        }
                        else if (line.Contains("<preview"))
                        {
                            try
                            {
                                line = line.Replace("<preview start_position=\"", "").TrimStart();
                                var preview = Convert.ToDouble(line.Substring(0, line.IndexOf("\"")));
                                song.PreviewStart = (int)(preview * 1000);
                            }
                            catch { }
                        }
                        else if (line.Contains("<combined_audio"))
                        {
                            try
                            {
                                var index = line.IndexOf("num_channels=\"");
                                line = line.Substring(index, line.Length - index).Trim();
                                var channels = Convert.ToInt16(line.Replace("num_channels=\"", "").Replace("\"", "").Replace("/>", "").Trim());
                                song.ChannelsTotal = channels;
                            }
                            catch { }
                        }
                    }
                    catch (Exception) { }                        
                }
                sr.Dispose();
                Songs.Add(song);
            }
            catch (Exception)
            {
                return false;
            }
            return Songs.Count == 1;
        }

        public bool ReadHSANFile(string hsanFile)
        {
            try
            {
                Tools = new NemoTools();
                Songs = new List<SongData>();
                var song = new SongData();
                song.Initialize();
                song.ChartAuthor = "Rocksmith 2014";
                var whichInstrument = new Instrument();
                var sr = new StreamReader(hsanFile);
                while (sr.Peek() >= 0)
                {                    
                    try
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("\"AlbumName\""))
                        {
                            song.Album = CleanHSANLine(line, "AlbumName");
                        }
                        else if (line.Contains("\"ArtistName\""))
                        {
                            song.Artist = CleanHSANLine(line, "ArtistName");
                        }
                        else if (line.Contains("\"SongLength\""))
                        {
                            song.Length = Convert.ToInt32(Convert.ToDouble(CleanHSANLine(line, "SongLength")) * 1000.00);
                        }
                        else if (line.Contains("\"SongName\""))
                        {
                            song.Name = CleanHSANLine(line, "SongName");
                        }
                        else if (line.Contains("\"SongYear\""))
                        {
                            song.YearReleased = Convert.ToInt32(CleanHSANLine(line, "SongYear"));
                        }
                        else if (line.Contains("\"SongKey\""))
                        {
                            song.InternalName = CleanHSANLine(line, "SongKey");
                        }        
                        else if (line.Contains("\"ArrangementName\": \"Rhythm\","))
                        {
                            whichInstrument = Instrument.Rhythm;                            
                        }
                        else if (line.Contains("\"ArrangementName\": \"Lead\","))
                        {
                            whichInstrument = Instrument.Lead;                            
                        }
                        else if (line.Contains("\"ArrangementName\": \"Bass\","))
                        {
                            whichInstrument = Instrument.Bass;                            
                        }
                        else if (line.Contains("\"MediumMastery\""))
                        {
                            var diff = Convert.ToDouble(CleanHSANLine(line, "MediumMastery"));
                            switch (whichInstrument)
                            {
                                case Instrument.Lead:
                                    song.GuitarDiff = Convert.ToInt16(Math.Floor(7 * diff));
                                    break;
                                case Instrument.Rhythm:
                                    song.RhythmDiff = Convert.ToInt16(Math.Floor(7 * diff));
                                    break;
                                case Instrument.Bass:
                                    song.BassDiff = Convert.ToInt16(Math.Floor(7 * diff));
                                    break;
                            }                            
                        }
                    }
                    catch (Exception)
                    { }
                }
                sr.Dispose();
                Songs.Add(song);
            }
            catch (Exception)
            {
                return false;
            }
            return Songs.Count == 1;
        }

        public bool ReadGHWTDEFile(string iniFile)
        {
            try
            {
                Tools = new NemoTools();
                Songs = new List<SongData>();
                var song = new SongData();
                song.Initialize();
                var sr = new StreamReader(iniFile);
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        var line = sr.ReadLine();
                        if ((line.ToLowerInvariant().Contains("artist=") || line.ToLowerInvariant().Contains("artist =")) && !line.ToLowerInvariant().Contains("originalartist"))
                        {
                            song.Artist = Tools.GetConfigString(line);
                        }
                        else if (line.ToLowerInvariant().Contains("title=") || line.ToLowerInvariant().Contains("title ="))
                        {
                            song.Name = Tools.GetConfigString(line);
                        }
                        else if (line.ToLowerInvariant().Contains("checksum=") || line.ToLowerInvariant().Contains("checksum ="))
                        {
                            song.InternalName = Tools.GetConfigString(line);
                        }
                        else if (line.ToLowerInvariant().Contains("album=") || line.ToLowerInvariant().Contains("album ="))
                        {
                            song.Album = Tools.GetConfigString(line);
                        }
                        else if (line.ToLowerInvariant().Contains("year=") || line.ToLowerInvariant().Contains("year ="))
                        {
                            song.YearReleased = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("author=") || line.ToLowerInvariant().Contains("author ="))
                        {
                            song.ChartAuthor = Tools.GetConfigString(line);
                        }
                        else if (line.ToLowerInvariant().Contains("genre=") || line.ToLowerInvariant().Contains("genre ="))
                        {
                            song.Genre = Tools.GetConfigString(line);
                            song.RawGenre = string.IsNullOrEmpty(song.Genre) ? "" : song.Genre;
                        }
                        else if (line.ToLowerInvariant().Contains("diff_keys_real") && !line.ToLowerInvariant().Contains("diff_keys_real_ps"))
                        {
                            song.ProKeysDiff = Convert.ToInt16(Tools.GetConfigString(line)) + 1;
                            song.DisableProKeys = song.ProKeysDiff <= 0;
                        }                       
                        else if (line.ToLowerInvariant().Contains("diffguitar=") || line.ToLowerInvariant().Contains("diffguitar ="))
                        {
                            song.GuitarDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("diffbass=") || line.ToLowerInvariant().Contains("diffbass ="))
                        {
                            song.BassDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("diffdrums=") || line.ToLowerInvariant().Contains("diffdrums ="))
                        {
                            song.DrumsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("diffkeys=") || line.ToLowerInvariant().Contains("diffkeys ="))
                        {
                            song.KeysDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("diffvocals=") || line.ToLowerInvariant().Contains("diffvocals ="))
                        {
                            song.VocalsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.ToLowerInvariant().Contains("diffband=") || line.ToLowerInvariant().Contains("diffband ="))
                        {
                            song.BandDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                    }
                    catch (Exception)
                    { }
                }
                sr.Dispose();
                Songs.Add(song);
            }
            catch (Exception)
            {
                return false;
            }
            return Songs.Count == 1;
        }

        public bool ReadFNFFile(string iniFile)
        {
            try
            {
                Tools = new NemoTools();
                Songs = new List<SongData>();
                var song = new SongData();
                song.Initialize();                               
                var sr = new StreamReader(iniFile);
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("artist=") || line.Contains("artist ="))
                        {
                            song.Artist = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("name=") || line.Contains("name ="))
                        {
                            song.Name = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("album=") || line.Contains("album ="))
                        {
                            song.Album = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("year=") || line.Contains("year ="))
                        {
                            song.YearReleased = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("genre=") || line.Contains("genre ="))
                        {
                            song.Genre = Tools.GetConfigString(line);
                            song.RawGenre = string.IsNullOrEmpty(song.Genre) ? "" : song.Genre;
                        }                        
                        else if (line.Contains("charter=") || line.Contains("charter ="))
                        {
                            song.ChartAuthor = Tools.GetConfigString(line);
                        }                        
                        else if (line.Contains("diff_guitar=") || line.Contains("diff_guitar ="))
                        {
                            song.GuitarDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_bass=") || line.Contains("diff_bass ="))
                        {
                            song.BassDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_drums=") || line.Contains("diff_drums ="))
                        {
                            song.DrumsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_keys=") || line.Contains("diff_keys ="))
                        {
                            song.KeysDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_vocals=") || line.Contains("diff_vocals ="))
                        {
                            song.VocalsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }                        
                    }
                    catch (Exception)
                    { }
                }
                sr.Dispose();             
                Songs.Add(song);
            }
            catch (Exception)
            {
                return false;
            }
            return Songs.Count == 1;
        }

        public bool ReadINIFile(string iniFile)
        {
            try
            {
                Tools = new NemoTools();
                Songs = new List<SongData>();
                var song = new SongData();
                song.Initialize();
                var isFretSmasher = false;
                var sr = new StreamReader(iniFile);
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        var line = sr.ReadLine();
                        if (line.Contains("diff") && line.Contains("-1"))
                        {
                            isFretSmasher |= true;
                        }
                        if (line.Contains("artist=") || line.Contains("artist ="))
                        {
                            song.Artist = Tools.GetConfigString(line);
                        }
                        else if ((line.Contains("name=") || line.Contains("name =")) && !line.Contains("shortname"))
                        {
                            song.Name = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("album=") || line.Contains("album ="))
                        {
                            song.Album = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("year=") || line.Contains("year ="))
                        {
                            song.YearReleased = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("genre=") || line.Contains("genre ="))
                        {
                            song.Genre = Tools.GetConfigString(line);
                            song.RawGenre = string.IsNullOrEmpty(song.Genre) ? "" : song.Genre;
                        }
                        else if (line.Contains("song_length=") || line.Contains("song_length ="))
                        {
                            song.Length = Convert.ToInt32(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("preview_start_time=") || line.Contains("preview_start_time ="))
                        {
                            song.PreviewStart = Convert.ToInt32(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("charter=") || line.Contains("charter ="))
                        {
                            song.ChartAuthor = Tools.GetConfigString(line);
                        }
                        else if (line.Contains("diff_keys_real") && !line.Contains("diff_keys_real_ps"))
                        {
                            song.ProKeysDiff = Convert.ToInt16(Tools.GetConfigString(line)) + 1;
                            song.DisableProKeys = song.ProKeysDiff <= 0;
                        }
                        else if (line.Contains("track=") || line.Contains("track ="))
                        {
                            song.TrackNumber = Convert.ToInt16(Tools.GetConfigString(line));
                            if (song.TrackNumber > 100)
                            {
                                song.TrackNumber = 1; //for stupid values like 16,000
                            }
                        }
                        else if (line.Contains("delay =") || line.Contains("delay="))
                        {
                            song.PSDelay = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("video_start_time =") || line.Contains("video_start_time="))
                        {
                            song.VideoStartTime = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_guitar=") || line.Contains("diff_guitar ="))
                        {
                            song.GuitarDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_bass=") || line.Contains("diff_bass ="))
                        {
                            song.BassDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_drums=") || line.Contains("diff_drums ="))
                        {
                            song.DrumsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_keys=") || line.Contains("diff_keys ="))
                        {
                            song.KeysDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_vocals=") || line.Contains("diff_vocals ="))
                        {
                            song.VocalsDiff = Convert.ToInt16(Tools.GetConfigString(line));
                        }
                        else if (line.Contains("diff_vocals_harm"))
                        {
                            //song.VocalParts = Convert.ToInt16(Tools.GetConfigString(line)); THIS IS WRONG
                        }
                    }
                    catch (Exception)
                    { }
                }
                sr.Dispose();

                if (isFretSmasher)//it uses a different difficulty tier than other games so we need to add 1
                {
                    song.GuitarDiff += 1;
                    song.BassDiff += 1;
                    song.DrumsDiff += 1;
                    song.VocalsDiff += 1;
                    song.KeysDiff += 1;
                }

                Songs.Add(song);
            }
            catch (Exception)
            {
                return false;
            }
            return Songs.Count == 1;
        }

        /// <summary>
        /// Extracts and then reads the DTA file for song entries
        /// </summary>
        /// <param name="xPackage">Full path to STFS package (CON or LIVE file) that holds the DTA file</param>
        /// <returns>Boolean True/False</returns>
        public bool ReadDTA(STFSPackage xPackage)
        {
            return ExtractDTA(xPackage, false) && ReadDTA(DTA);
        }
        
        /// <summary>
        /// Reads DTA file for song entries and populates Songs list
        /// </summary>
        /// <param name="xDTA">Full path to DTA file to read</param>
        /// <returns>Boolean True/False</returns>
        public bool ReadDTA(byte[] xDTA)
        {
            try
            {
                Songs = new List<SongData>();
                Tools = new NemoTools();

                if (!GetDTAEntries(xDTA)) return false;
                for (var i = 0; i < DTAEntries.Count; i++)
                {
                    var entry = DTAEntries[i];
                    Songs.Add(new SongData());
                    var index = Songs.Count - 1;
                    var song = Songs[index];
                    song.Initialize();
                    song.DTALines = entry;
                    song.DTAIndex = i;
                    
                    //RB1 short versions have double entries in the DTA and we have to ignore them
                    var didDrums = false;
                    var didBass = false;
                    var didGuitar = false;
                    var didKeys = false;
                    var didVocals = false;
                    var didCrowd = false;
                    var didPans = false;
                    var didCores = false;
                    var didVols = false;

                    var open = 0;
                    var line = "";
                    var isDLC = false;
                    var isRB1 = false;

                    //this is organized more or less how most HMX DTA files are formatted, but there are differences, hence the code variations
                    //do not mess with this unless you're willing to debug and troubleshoot. this is finely tuned to work with 99.99% of existing DTA files
                    for (var z = 0; z < entry.Count; z++)
                    {
                        try
                        {
                            line = entry[z];
                            if (line.Trim().StartsWith(";;", StringComparison.Ordinal)) continue; //skip commented out lines
                            if (string.IsNullOrWhiteSpace(line)) continue; //don't want empty lines

                            var inQuotes = line.TrimStart().StartsWith("\"");                            
                            if (!inQuotes && line.Contains("("))
                            {
                                open++;
                            }
                            if (open == 1 && line.Trim().StartsWith("(", StringComparison.Ordinal) && !line.Contains(")"))
                            {
                                song.ShortName = line.Trim() == "(" ? entry[z + 1].Replace("'", "").Trim() : line.Replace("(", "").Replace("'", "").Trim();
                            }
                            if (line.Contains("(name") && !line.Contains("songs/"))
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];                                    
                                }
                                if (line.Contains("songs/") && !line.Contains("_short"))
                                {
                                    song.FilePath = GetSongPath(line);
                                    song.InternalName = GetInternalName(line);
                                }
                                else
                                {
                                    song.Name = GetSongName(line);
                                }
                            }
                            else if (line.Contains("'name'") && !line.Contains("songs/"))
                            {
                                z++;
                                line = entry[z];
                                if (!(line.Contains("songs/")))
                                {
                                    song.Name = GetSongName(line);
                                }
                                else if (!line.Contains("_short"))
                                {
                                    song.FilePath = GetSongPath(line);
                                    song.InternalName = GetInternalName(line);
                                }
                            }
                            else if (line.Contains("songs/") && !line.Contains("_short"))
                            {
                                song.FilePath = GetSongPath(line);
                                song.InternalName = GetInternalName(line);
                            }
                            else if (line.Contains("midi_file") && !line.Contains("_short"))
                            {
                                song.MIDIFile = line.Replace("midi_file", "").Replace("(", "").Replace(")", "").Replace("\"", "").Trim();
                            }
                            else if (line.Contains("(artist"))
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];
                                }
                                song.Artist = GetArtistName(line);                                
                            }
                            else if (line.Contains("'artist'"))
                            {
                                z++;
                                line = entry[z];
                                song.Artist = GetArtistName(line);
                            }
                            else if (line.Contains("(master") | line.Contains("('master"))
                            {
                                song.Master = line.Contains("1") || line.ToLowerInvariant().Contains("true");
                            }
                            else if (line.Contains("(tracks"))
                            {
                                while (line != null && line.Trim() != ")")// && !line.Trim().Contains(")))"))
                                {
                                    if (line.ToLowerInvariant().Contains("bass") && !didBass)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        }
                                        song.ChannelsBass = getChannels(line, "bass");
                                        song.ChannelsBassStart = getChannelsStart(line, "bass");
                                        didBass = true;
                                        if (line.Contains(")))")) break;
                                    }
                                    else if (line.ToLowerInvariant().Contains("guitar") && !didGuitar)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        }
                                        song.ChannelsGuitar = getChannels(line, "guitar");
                                        song.ChannelsGuitarStart = getChannelsStart(line, "guitar");
                                        didGuitar = true;
                                        if (line.Contains(")))")) break;
                                    }
                                    else if (line.ToLowerInvariant().Contains("keys") && !didKeys)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        }
                                        song.ChannelsKeys = getChannels(line, "keys");
                                        song.ChannelsKeysStart = getChannelsStart(line, "keys");
                                        didKeys = true;
                                        if (line.Contains(")))")) break;
                                    }
                                    else if (line.ToLowerInvariant().Contains("vocals") && !didVocals)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        }
                                        song.ChannelsVocals = getChannels(line, "vocals");
                                        song.ChannelsVocalsStart = getChannelsStart(line, "vocals");
                                        didVocals = true;
                                        if (line.Contains(")))")) break;
                                    }
                                    else if (line.ToLowerInvariant().Contains("drum") && !didDrums)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        }
                                        song.ChannelsDrums = getChannels(line, "drum");
                                        song.ChannelsDrumsStart = getChannelsStart(line, "drum");
                                        didDrums = true;
                                        if (line.Contains(")))")) break;
                                    }
                                    else if (line.Contains("crowd_channels") && !didCrowd)
                                    {
                                        song.ChannelsCrowd = getChannels(line, "crowd_channels");
                                        song.ChannelsCrowdStart = getChannelsStart(line, "crowd_channels");
                                        didCrowd = true;
                                    }
                                    z++;
                                    line = entry[z];
                                }
                            }
                            else if (line.Contains("'tracks'"))
                            {
                                z++;
                                line = entry[z];
                                while (line != null && !line.ToLowerInvariant().Contains("pans"))
                                {
                                    if (line.ToLowerInvariant().Contains("bass") && !didBass)
                                    {
                                        if (!line.Contains(")")) //RB1 songs with one channel have the channel right in this line
                                        {
                                            z++;
                                            line = entry[z];
                                        }                                        
                                        song.ChannelsBass = getChannels(line, "bass");
                                        song.ChannelsBassStart = getChannelsStart(line, "bass");
                                        didBass = true;
                                    }
                                    else if (line.ToLowerInvariant().Contains("guitar") && !didGuitar)
                                    {
                                        if (!line.Contains(")")) //RB1 songs with one channel have the channel right in this line
                                        {
                                            z++;
                                            line = entry[z];
                                        }  
                                        song.ChannelsGuitar = getChannels(line, "guitar");
                                        song.ChannelsGuitarStart = getChannelsStart(line, "guitar");
                                        didGuitar = true;
                                    }
                                    else if (line.ToLowerInvariant().Contains("keys") && !didKeys)
                                    {
                                        if (!line.Contains(")"))
                                        {
                                            z++;
                                            line = entry[z];
                                        } 
                                        song.ChannelsKeys = getChannels(line, "keys");
                                        song.ChannelsKeysStart = getChannelsStart(line, "keys");
                                        didKeys = true;
                                    }
                                    else if (line.ToLowerInvariant().Contains("vocals") && !didVocals)
                                    {
                                        if (!line.Contains(")")) //RB1 songs with one channel have the channel right in this line
                                        {
                                            z++;
                                            line = entry[z];
                                        }  
                                        song.ChannelsVocals = getChannels(line, "vocals");
                                        song.ChannelsVocalsStart = getChannelsStart(line, "vocals");
                                        didVocals = true;
                                    }
                                    else if (line.ToLowerInvariant().Contains("drum") && !didDrums)
                                    {
                                        z++;
                                        line = entry[z];
                                        song.ChannelsDrums = getChannels(line, "drum");
                                        song.ChannelsDrumsStart = getChannelsStart(line, "drum");
                                        didDrums = true;
                                    }
                                    else if (line.Contains("crowd_channels") && !didCrowd)
                                    {
                                        song.ChannelsCrowd = getChannels(line, "crowd_channels");
                                        song.ChannelsCrowdStart = getChannelsStart(line, "crowd_channels");
                                        didCrowd = true;
                                    }                                    
                                    z++;
                                    line = entry[z];
                                }
                                if (line.Contains("'pans'") && !didPans)
                                {
                                    z++;
                                    line = entry[z];
                                    song.PanningValues = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("pans", "").Replace("\t"," ").Trim();
                                    didPans = true;
                                }
                            }
                            else if (line.Contains("song_id") && !line.Trim().StartsWith(";")) //ignore commented out original IDs
                            {
                                try
                                {
                                    song.SongIdString = GetSongID(line);
                                    song.HasSongIDError = string.IsNullOrWhiteSpace(song.SongIdString);
                                    song.SongId = Convert.ToInt32(GetSongID(line));
                                }
                                catch (Exception)
                                {
                                    //fails because it's not numeric, make it into a custom
                                    song.SongId = 99999999;
                                }
                            }
                            else if (line.Contains("vocal_parts"))
                            {
                                song.VocalParts = GetVocalParts(line);
                            }
                            else if (line.Contains("'vols'") && !didVols)
                            {
                                z++;
                                line = entry[z];

                                line = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("vols", "").Replace("\t", " ").Trim();
                                line = RemoveDTAComments(line);

                                song.AttenuationValues = line;
                                song.OriginalAttenuationValues = line;

                                didVols = true;
                            }
                            /*else if (line.Contains("(vols") && !didVols)
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];
                                }

                                line = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("vols", "").Replace("\t", " ").Trim();
                                line = RemoveDTAComments(line);

                                song.AttenuationValues = line;
                                song.OriginalAttenuationValues = line;

                                didVols = true;
                            }*/
                            else if (line.Contains("(vols") && !didVols)
                            {
                                var vols = new List<string>();
                                int parenDepth = 0;
                                bool started = false;

                                while (z < entry.Count)
                                {
                                    line = entry[z];

                                    foreach (char c in line)
                                    {
                                        if (c == '(')
                                        {
                                            parenDepth++;
                                            started = true;
                                        }
                                        else if (c == ')')
                                        {
                                            parenDepth--;
                                        }
                                    }

                                    string cleaned = line
                                        .Replace("(vols", "")
                                        .Replace("vols", "")
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Replace("'", "")
                                        .Replace("\t", " ")
                                        .Trim();

                                    if (!string.IsNullOrWhiteSpace(cleaned))
                                    {
                                        vols.Add(cleaned);
                                    }

                                    if (started && parenDepth <= 0)
                                        break;

                                    z++;
                                }

                                var attenuation = RemoveDTAComments(string.Join(" ", vols));
                                song.AttenuationValues = attenuation;
                                song.OriginalAttenuationValues = attenuation;
                                didVols = true;
                            }
                            else if (line.Contains("'pans'") && !didPans)
                            {
                                z++;
                                line = entry[z];
                                song.PanningValues = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("pans", "").Replace("\t", " ").Trim();
                                didPans = true;
                            }
                            /*else if (line.Contains("(pans") && !didPans)
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];
                                }
                                song.PanningValues = line.Replace("(", "").Replace(")", "").Replace("'", "").Replace("pans", "").Replace("\t", " ").Trim();
                                didPans = true;
                            }*/
                            else if (line.Contains("(pans") && !didPans)
                            {
                                var pans = new List<string>();
                                int parenDepth = 0;
                                bool started = false;

                                while (z < entry.Count)
                                {
                                    line = entry[z];

                                    foreach (char c in line)
                                    {
                                        if (c == '(')
                                        {
                                            parenDepth++;
                                            started = true;
                                        }
                                        else if (c == ')')
                                        {
                                            parenDepth--;
                                        }
                                    }

                                    string cleaned = line
                                        .Replace("(pans", "")
                                        .Replace("pans", "")
                                        .Replace("(", "")
                                        .Replace(")", "")
                                        .Replace("'", "")
                                        .Replace("\t", " ")
                                        .Trim();

                                    if (!string.IsNullOrWhiteSpace(cleaned))
                                    {
                                        pans.Add(cleaned);
                                    }

                                    if (started && parenDepth <= 0)
                                        break;

                                    z++;
                                }

                                song.PanningValues = string.Join(" ", pans);
                                didPans = true;
                            }
                            else if (line.Contains("(cores") && !didCores)
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];
                                }
                                song.ChannelsTotal = GetAudioChannels(line);
                                didCores = true;
                            }
                            else if (line.Contains("'cores'") && !didCores)
                            {
                                z++;
                                line = entry[z];
                                song.ChannelsTotal = GetAudioChannels(line);
                                didCores = true;
                            }
                            else if (line.Contains("hopo_threshold"))
                            {
                                try
                                {
                                    song.HopoThreshold = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.HopoThreshold = 0;
                                }
                            }
                            else if (line.Contains("crowd_channels") && !didCrowd)
                            {
                                song.ChannelsCrowd = getChannels(line, "crowd_channels");
                                song.ChannelsCrowdStart = getChannelsStart(line, "crowd_channels");
                                didCrowd = true;
                            }
                            else if (line.Contains("bank sfx") && !line.Contains("drum_bank"))
                            {
                                song.PercussionBank = line.Replace("(bank", "").Replace("(", "").Replace(")", "").Trim();
                            }
                            else if (line.Contains("'bank'"))
                            {
                                z++;
                                line = entry[z];
                                song.PercussionBank = line.Replace("\"", "").Trim();
                            }
                            else if (line.Contains("drum_bank"))
                            {
                                song.DrumBank = getDTAStringValue(line, "drum_bank");
                            }
                            else if (line.Contains("song_scroll_speed"))
                            {
                                try
                                {
                                    song.ScrollSpeed = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.ScrollSpeed = 2300;
                                }
                            }
                            else if ((line.Contains("(preview") || line.Contains("('preview")) && !line.Trim().StartsWith(";", StringComparison.Ordinal))
                            {
                                if (line.Contains("(") && line.Contains(")")) //most songs
                                {
                                    song.PreviewStart = (int)GetPreviewTimes(line, 0);
                                    song.PreviewEnd = (int)GetPreviewTimes(line, 1);
                                }
                                else
                                {
                                    try //this is mostly for Beatles Rock Band songs that have the preview times on the next two lines
                                    {
                                        z++;
                                        line = entry[z];
                                        song.PreviewStart = Convert.ToInt32(line.Replace("\t", ""));
                                        z++;
                                        line = entry[z];
                                        song.PreviewEnd = Convert.ToInt32(line.Replace("\t", "").Replace(")", ""));
                                    }
                                    catch
                                    {
                                        song.PreviewStart = 0;
                                        song.PreviewEnd = 30000;
                                    }
                                }
                            }
                            else if (line.Contains("song_length"))
                            {
                                try
                                {
                                    song.Length = Convert.ToInt32(GetSongDurationLong(line));
                                }
                                catch (Exception)
                                {
                                    song.Length = 0;
                                }
                            }
                            else if ((line.Contains("('drum'") || line.Contains("(drum ")) && !line.Contains("solo"))
                            {
                                song.DrumsDiff = DrumDiff(line);
                            }
                            else if ((line.Contains("('guitar'") || line.Contains("(guitar ")) && !line.Contains("solo"))
                            {
                                song.GuitarDiff = GuitarDiff(line);
                            }
                            else if ((line.Contains("('bass'") || line.Contains("(bass ")) && !line.Contains("solo"))
                            {
                                song.BassDiff = BassDiff(line);
                            }
                            else if ((line.Contains("('vocals'") || line.Contains("(vocals ")) && !line.Contains("solo"))
                            {
                                song.VocalsDiff = VocalsDiff(line);
                            }
                            else if ((line.Contains("('keys'") || line.Contains("(keys ")) && !line.Contains("solo"))
                            {
                                song.KeysDiff = KeysDiff(line);
                            }
                            else if (line.Contains("real_keys"))
                            {
                                song.ProKeysDiff = ProKeysDiff(line);
                            }
                            else if (line.Contains("real_guitar") && !(line.Contains("tuning")))
                            {
                                song.ProGuitarDiff = ProGuitarDiff(line);
                            }
                            else if (line.Contains("real_bass") && !(line.Contains("tuning")))
                            {
                                song.ProBassDiff = ProBassDiff(line);
                            }
                            else if (line.Contains("('band'") || (line.Contains("(band ")))
                            {
                                song.BandDiff = BandDiff(line);
                            }
                            else if (line.Contains("version") && !line.Contains("short") && !line.Contains("fake"))
                            {
                                song.GameVersion = GetGameVersion(line);
                            }
                            else if (line.ToLowerInvariant().Contains("game_origin"))
                            {
                                song.Source = GetSourceGame(line);
                            }
                            else if (line.ToLowerInvariant().Contains("(solo ("))
                            {
                                song.InstrumentSolos = line.Replace("solo", "").Replace("(", "").Replace(")", "");
                            }
                            else if (line.ToLowerInvariant().Contains("encoding"))
                            {
                                song.Encoding = line.ToLowerInvariant().Replace("encoding", "").Replace("(", "").Replace(")", "");
                            }
                            else if (line.ToLowerInvariant().Contains("rating"))
                            {
                                song.Rating = GetRating(line);
                            }
                            else if (line.Contains("genre") && !line.Contains("subgenre"))
                            {
                                song.Genre = doGenre(line, true);
                                song.RawGenre = GetRawGenre(line);
                            }
                            else if (line.ToLowerInvariant().Contains("subgenre_"))
                            {
                                song.SubGenre = doSubGenre(line, true);
                                song.RawSubGenre = GetRawSubGenre(line);
                            }
                            else if (line.ToLowerInvariant().Contains("gender"))
                            {
                                song.Gender = line.ToLowerInvariant().Contains("female") ? "Female" : "Male";
                            }
                            else if (line.Contains("(year_released") || line.Contains("('year_released") || line.Contains("date_released"))
                            {
                                song.YearReleased = GetYear(line);
                            }
                            else if (line.Contains("(year_recorded") || line.Contains("('year_recorded") || line.Contains("date_recorded"))
                            {
                                song.YearRecorded = GetYear(line);                            
                            }
                            else if (line.Contains("(author") || line.Contains("'author"))
                            {
                                song.ChartAuthor = line.Replace("(author", "").Replace("'author", "").Replace("\"", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim();
                            }
                            else if (line.Contains("(album_name"))
                            {
                                if (!line.Contains(")"))
                                {
                                    z++;
                                    line = entry[z];
                                }
                                song.Album = GetAlbumName(line);
                            }
                            else if (line.Contains("'album_name'"))
                            {
                                z++;
                                line = entry[z];
                                song.Album = GetAlbumName(line);
                            }
                            else if (line.ToLowerInvariant().Contains("track_number"))
                            {
                                try
                                {
                                    song.TrackNumber = Convert.ToInt32(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.TrackNumber = 1;
                                }
                            }
                            else if (line.Trim().Contains("(exported TRUE)"))
                            {
                                isRB1 = true;
                                song.Source = "rb1";
                            }
                            else if (line.Contains("vocal_tonic_note"))
                            {
                                try
                                {
                                    song.TonicNote = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.TonicNote = 0;
                                }
                            }
                            else if (line.Contains("song_tonality"))
                            {
                                try
                                {
                                    song.Tonality = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.Tonality = 0;
                                }
                            }
                            else if (line.Contains("guide_pitch"))
                            {
                                song.GuidePitch = GetGuidePitch(line);
                            }
                            else if (line.Contains("mute_volume_vocals"))
                            {
                                song.MuteVolumeVocals = GetVolumeLevel(line);
                            }
                            else if (line.Contains("mute_volume") && !line.Contains("vocals"))
                            {
                                song.MuteVolume = GetVolumeLevel(line);
                            }
                            if (line.Contains("tuning_offset"))
                            {
                                try
                                {
                                    song.TuningCents = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                                }
                                catch (Exception)
                                {
                                    song.TuningCents = 0;
                                }
                            }
                            else if (line.Contains("guitar_tuning"))
                            {
                                song.ProGuitarTuning = GetTuning(line);
                            }
                            else if (line.Contains("bass_tuning"))
                            {
                                song.ProBassTuning = GetTuning(line);
                            }
                            else if (line.ToLowerInvariant().Contains("downloaded") && line.ToLowerInvariant().Contains("true"))
                            {
                                isDLC = true;
                            }
                            //read extra Magma C3 info
                            else if (line.Contains(";Song authored by "))
                            {
                                song.ChartAuthor = line.Replace(";Song authored by ", "").Trim();
                            }
                            else if (line.Contains(";Song=") || line.Contains(";SongTitle="))
                            {
                                song.OverrideName = Tools.GetConfigString(line);
                            }
                            else if (line.Contains(";Language(s)"))
                            {
                                song.Languages = line.Replace(";Language(s)", "").Replace("=", "");
                            }
                            else if (line.Contains(";DisableProKeys=1"))
                            {
                                song.DisableProKeys = true;
                            }
                            else if (line.Contains(";RhythmBass=1"))
                            {
                                song.RhythmBass = true;
                            }
                            else if (line.Contains(";RhythmKeys=1") && !song.RhythmBass)
                            {
                                song.RhythmKeys = true;
                            }
                            else if (line.Contains(";2xBass=1"))
                            {
                                song.DoubleBass = true;
                            }
                            else if (line.Contains(";Karaoke=1"))
                            {
                                song.Karaoke = true;
                            }
                            else if (line.Contains(";Multitrack=1"))
                            {
                                song.Multitrack = true;
                            }
                            else if (line.Contains(";DIYStems=1"))
                            {
                                song.DIYStems = true;
                            }
                            else if (line.Contains(";PartialMultitrack=1"))
                            {
                                song.PartialMultitrack = true;
                            }
                            else if (line.Contains(";UnpitchedVocals=1"))
                            {
                                song.UnpitchedVocals = true;
                            }
                            else if (line.Contains(";Convert=1"))
                            {
                                song.Convert = true;
                            }
                            else if (line.Contains(";RB3Version=1"))
                            {
                                song.RB3Version = true;
                            }
                            else if (line.Contains(";CATemh=1"))
                            {
                                song.CATemh = true;
                            }
                            else if (line.Contains(";ExpertOnly=1"))
                            {
                                song.ExpertOnly = true;
                            }
                            else if (line.Contains(";OriginalAttenuationValues="))
                            {
                                song.OriginalAttenuationValues = line.Replace(";OriginalAttenuationValues=", "");
                            }
                            else if (line.Contains(";VolumeNormalizerAudioHash="))
                            {
                                song.VNAudioHash = line.Replace(";VolumeNormalizerAudioHash=", "").Trim();
                            }
                            else if (line.Contains(";NormalizedAsAlbum="))
                            {
                                song.NormalizedAsAlbum = (line.Replace(";NormalizedAsAlbum=", "").Trim() == "0") ? false : true;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!string.IsNullOrWhiteSpace(song.Artist) && !string.IsNullOrWhiteSpace(song.Name))
                            {
                                MessageBox.Show("There was an error loading song\n'" + song.Artist + " - " + song.Name +
                                    "'\nThe error says:\'n" + ex.Message + "'\nLine: " + line + "\nLine #: " + z + "\n\nI will attempt to recover the rest " +
                                                "of the song information but you might want to verify or update it afterwards", "DTA Parser",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    //old songs didn't have the vocal_parts line when it was one part
                    if (song.VocalsDiff > 0 && song.VocalParts == 0)
                    {
                        song.VocalParts = 1;
                    }
                    if (string.IsNullOrWhiteSpace(song.Source))
                    {
                        song.Source = "rb1_dlc"; //default to old DLC
                    }

                    if (string.IsNullOrWhiteSpace(song.ChartAuthor.Trim()) && song.SongIdString.Length == 7)
                    {
                        switch (song.SongIdString.Substring(0, 2))
                        {
                            case "10":
                                song.ChartAuthor = "Harmonix";
                                break;
                            case "50":
                                song.ChartAuthor = "Rock Band Network";
                                break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(song.SongIdString))
                    {
                        song.SongIdString = song.SongId.ToString(CultureInfo.InvariantCulture);
                    }

                    if (!isDLC || isRB1) continue;
                    switch (song.Source)
                    {
                        case "":
                        case "rb1":
                            song.Source = "rb1_dlc";
                            break;
                        case "rb2":
                            song.Source = "rb2_dlc";
                            break;
                    }

                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetVolumeLevel(string line)
        {
            var volume = line.Replace("mute_volume_vocals", "");
            volume = volume.Replace("(", "");
            volume = volume.Replace(")", "");
            volume = volume.Replace("'", "");
            volume = volume.Replace("\"", "");
            volume = volume.Trim();

            try
            {
                return Convert.ToInt16(volume);
            }
            catch (Exception)
            {
                return -96;
            }
        }

        public bool IsNumericID(string line)
        {
            var s_id = GetSongID(line);
            return int.TryParse(s_id, out int _id);
            //only worry if it's not a numeric value, no longer need to check for all the below crazyness
            /*//C3 unique numeric IDs are 10 digits
            //Xbox DLC/RBN IDs are 7 digits
            //Game IDs start at 1 and go to around 3000 (Green Day)
            //if (!PS3 && s_id.Length != 10 && s_id.Length != 7) return false;

            //PS3 DLC and on-disc songs have all kinds of weird IDs, be more lenient
            if (s_id == "0") return true; //this is being replaced by on-disc upgrade, do not modify
            if (s_id.Length > 4 && s_id.Length != 10 && s_id.Length != 7) return false; //valid ranges from 0 to 4 digit values

            //when the game converts a string ID to numeric, they always start with either of these values
            if (s_id.Length == 10)
            {
                if (s_id.Substring(0, 6) == "11844" || s_id.Substring(0, 6) == "10746") return false;
            }

            try
            {
                var n_id = Convert.ToInt32(s_id);
                //valid C3 unique numeric ID range is 1000100001 to 2147399999
                if (s_id.Length == 10)
                {
                    return (n_id >= 1000100001 && n_id <= 2147399999);
                }
                //valid Xbox DLC range starts at 100xxxx, old PS3 custom IDs start with 140xxxx, RBN range starts at 501xxxx
                if (s_id.Length == 7)
                {
                    var prefix = s_id.Substring(0, 2);
                    return prefix == "10" || prefix == "14" || prefix == "50";
                }
                //Game IDs start at 1 and go to around 3000 (Green Day)
                if (s_id.Length < 5)
                {
                    return n_id != 0;
                }
                return false;
            }
            catch
            {
                return false;
            }*/
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Must set difficulty boundaries prior to calling!
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        private int doDifficulty(int diff)
        {

            if (diff > 0 && diff < Diff1)
            {
                return 1;
            }
            if (diff >= Diff1 && diff < Diff2)
            {
                return 2;
            }
            if (diff >= Diff2 && diff < Diff3)
            {
                return 3;
            }
            if (diff >= Diff3 && diff < Diff4)
            {
                return 4;
            }
            if (diff >= Diff4 && diff < Diff5)
            {
                return 5;
            }
            if (diff >= Diff5 && diff < Diff6)
            {
                return 6;
            }
            return diff >= Diff6 ? 7 : 0;
        }

        /// <summary>
        /// Returns clean SubGenre
        /// </summary>
        /// <param name="raw_subgenre">Raw subgenre as found in the songs.dta file</param>
        /// <returns></returns>
        public string doSubGenre(string raw_subgenre)
        {
            switch (raw_subgenre)
            {
                case "alternative": { return "Alternative"; }
                case "college": { return "College"; }
                case "other": { return "Other"; }
                case "acoustic": { return "Acoustic"; }
                case "chicago": { return "Chicago"; }
                case "classic": { return "Classic"; }
                case "contemporary": { return "Contemporary"; }
                case "country": { return "Country"; }
                case "delta": { return "Delta"; }
                case "electric": { return "Electric"; }
                case "classicrock": { return "Classic Rock"; }
                case "bluegrass": { return "Bluegrass"; }
                case "honkytonk": { return "Honky Tonk"; }
                case "outlaw": { return "Outlaw"; }
                case "traditionalfolk": { return "Traditional Folk"; }
                case "emo": { return "Emo"; }
                case "fusion": { return "Fusion"; }
                case "glam": { return "Glam"; }
                case "goth": { return "Goth"; }
                case "acidjazz": { return "Acid Jazz"; }
                case "experimental": { return "Experimental"; }
                case "ragtime": { return "Ragtime"; }
                case "smooth": { return "Smooth"; }
                case "metal": { return "Metal"; }
                case "black": { return "Black"; }
                case "core": { return "Core"; }
                case "death": { return "Death"; }
                case "hair": { return "Hair"; }
                case "industrial": { return "Industrial"; }
                case "power": { return "Power"; }
                case "prog": { return "Prog"; }
                case "speed": { return "Speed"; }
                case "thrash": { return "Thrash"; }
                case "novelty": { return "Novelty"; }
                case "numetal": { return "Nu-Metal"; }
                case "disco": { return "Disco"; }
                case "motown": { return "Motown"; }
                case "pop": { return "Pop"; }
                case "rhythmandblues": { return "Rhythm and Blues"; }
                case "softrock": { return "Soft Rock"; }
                case "soul": { return "Soul"; }
                case "teen": { return "Teen"; }
                case "progrock": { return "Prog Rock"; }
                case "garage": { return "Garage"; }
                case "hardcore": { return "Hardcore"; }
                case "dancepunk": { return "Dance Punk"; }
                case "arena": { return "Arena"; }
                case "blues": { return "Blues"; }
                case "funk": { return "Funk"; }
                case "hardrock": { return "Hard Rock"; }
                case "psychadelic": { return "Psychedelic"; }
                case "rock": { return "Rock"; }
                case "rockandroll": { return "Rock and Roll"; }
                case "rockabilly": { return "Rockabilly"; }
                case "ska": { return "Ska"; }
                case "surf": { return "Surf"; }
                case "folkrock": { return "Folk Rock"; }
                case "reggae": { return "Reggae"; }
                case "southernrock": { return "Southern Rock"; }
                case "alternativerap": { return "Alternative Rap"; }
                case "dub": { return "Dub"; }
                case "downtempo": { return "Downtempo"; }
                case "electronica": { return "Electronica"; }
                case "gangsta": { return "Gangsta"; }
                case "hardcoredance": { return "Hardcore Dance"; }
                case "hardcorerap": { return "Hardcore Rap"; }
                case "hiphop": { return "Hip Hop"; }
                case "drumandbass": { return "Drum and Bass"; }
                case "oldschoolhiphop": { return "Old School Hip Hop"; }
                case "rap": { return "Rap"; }
                case "triphop": { return "Trip Hop"; }
                case "undergroundrap": { return "Underground Rap"; }
                case "acapella": { return "A capella"; }
                case "classical": { return "Classical"; }
                case "contemporaryfolk": { return "Contemporary Folk"; }
                case "oldies": { return "Oldies"; }
                case "house": { return "House"; }
                case "techno": { return "Techno"; }
                case "breakbeat": { return "Breakbeat"; }
                case "ambient": { return "Ambient"; }
                case "trance": { return "Trance"; }
                case "chiptune": { return "Chiptune"; }
                case "dance": { return "Dance"; }
                case "new_wave": { return "New Wave"; }
                case "electroclash": { return "Electroclash"; }
                case "darkwave": { return "Dark Wave"; }
                case "synth": { return "Synthpop"; }
                case "indierock": { return "Indie Rock"; }
                case "mathrock": { return "Math Rock"; }
                case "lofi": { return "Lo-fi"; }
                case "shoegazing": { return "Shoegazing"; }
                case "postrock": { return "Post Rock"; }
                case "noise": { return "Noise"; }
                case "grunge": { return "Grunge"; }
                case "jrock": { return "J-Rock"; }
                case "latin": { return "Latin"; }
                case "inspirational": { return "Inspirational"; }
                case "world": { return "World"; }
                default: { return ""; }
            }
        }

        /// <summary>
        /// Returns clean SubGenre
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <param name="is_dta_line">Set to true if it's a raw line, false if raw subgenre</param>
        /// <returns></returns>
        public string doSubGenre(string raw_line, bool is_dta_line)
        {
            return doSubGenre(is_dta_line ? GetRawSubGenre(raw_line).Replace("subgenre_", "").Trim() : raw_line);
        }

        public string GetRawGenre(string raw_line)
        {
            //old style
            string genre;
            if (raw_line.Contains("(genre") && raw_line.Length > 11)
            {
                genre = RemoveDTAComments(raw_line);
                genre = genre.Replace("(genre", "").Replace(")", "").Trim();
                return genre;
            }

            //new style
            if (!raw_line.Contains("'genre'") || raw_line.Length <= 15) return "";
            //genre = raw_line.Substring(13, raw_line.Length - 15);
            genre = RemoveDTAComments(raw_line);
            genre = genre.Replace("'genre'", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim();
            return genre;
        }

        public string GetRawSubGenre(string raw_line)
        {
            //remove "sub_genre" if it's in the same line
            var subgenre = RemoveDTAComments(raw_line);
            subgenre = subgenre.Replace("sub_genre", "");

            subgenre = subgenre.Replace("'", "");
            subgenre = subgenre.Replace("\"", "");
            subgenre = subgenre.Replace("(", "");
            subgenre = subgenre.Replace(")", "");

            return subgenre.Trim();
        }

        /// <summary>
        /// Returns clean Genre
        /// </summary>
        /// <param name="raw_genre">Raw genre as found in the songs.dta file</param>
        /// <returns></returns>
        public string doGenre(string raw_genre)
        {
            switch (raw_genre)
            {
                case "alternative": { return "Alternative"; }
                case "blues": { return "Blues"; }
                case "classical": { return "Classical"; }
                case "classicrock": { return "Classic Rock"; }
                case "country": { return "Country"; }
                case "emo": { return "Emo"; }
                case "fusion": { return "Fusion"; }
                case "glam": { return "Glam"; }
                case "grunge": { return "Grunge"; }
                case "hiphoprap": { return "Hip-Hop/Rap"; }
                case "indierock": { return "Indie Rock"; }
                case "jazz": { return "Jazz"; }
                case "jrock": { return "J-Rock"; }
                case "latin": { return "Latin"; }
                case "metal": { return "Metal"; }
                case "new_wave": { return "New Wave"; }
                case "novelty": { return "Novelty"; }
                case "numetal": { return "Nu-Metal"; }
                case "other": { return "Other"; }
                case "poprock": { return "Pop-Rock"; }
                case "popdanceelectronic": { return "Pop/Dance/Electronic"; }
                case "prog": { return "Prog"; }
                case "punk": { return "Punk"; }
                case "rbsoulfunk": { return "R&B/Soul/Funk"; }
                case "reggaeska": { return "Reggae/Ska"; }
                case "inspirational": { return "Inspirational"; }
                case "rock": { return "Rock"; }
                case "southernrock": { return "Southern Rock"; }
                case "urban": { return "Urban"; }
                case "world": { return "World"; }
                default: { return raw_genre; }
            }
        }

        /// <summary>
        /// Returns clean Genre
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <param name="is_dta_line">Set to true if it's a raw line, false if raw genre</param>
        /// <returns></returns>
        public string doGenre(string raw_line, bool is_dta_line)
        {
            return doGenre(is_dta_line ? GetRawGenre(raw_line) : raw_line);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int VocalsDiff(int diff)
        {
            Diff1 = 132;
            Diff2 = 175;
            Diff3 = 218;
            Diff4 = 279;
            Diff5 = 353;
            Diff6 = 427;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int VocalsDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return VocalsDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int ProKeysDiff(int diff)
        {
            Diff1 = 153;
            Diff2 = 211;
            Diff3 = 269;
            Diff4 = 327;
            Diff5 = 385;
            Diff6 = 443;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int ProKeysDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return ProKeysDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int KeysDiff(int diff)
        {
            Diff1 = 153;
            Diff2 = 211;
            Diff3 = 269;
            Diff4 = 327;
            Diff5 = 385;
            Diff6 = 443;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int KeysDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return KeysDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int DrumDiff(int diff)
        {
            Diff1 = 124;
            Diff2 = 151;
            Diff3 = 178;
            Diff4 = 242;
            Diff5 = 345;
            Diff6 = 448;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int DrumDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return DrumDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int BassDiff(int diff)
        {
            Diff1 = 135;
            Diff2 = 181;
            Diff3 = 228;
            Diff4 = 293;
            Diff5 = 364;
            Diff6 = 436;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int BassDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return BassDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int ProBassDiff(int diff)
        {
            Diff1 = 150;
            Diff2 = 208;
            Diff3 = 267;
            Diff4 = 325;
            Diff5 = 384;
            Diff6 = 442;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int ProBassDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return ProBassDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int GuitarDiff(int diff)
        {
            Diff1 = 139;
            Diff2 = 176;
            Diff3 = 221;
            Diff4 = 267;
            Diff5 = 333;
            Diff6 = 409;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int GuitarDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return GuitarDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int ProGuitarDiff(int diff)
        {
            Diff1 = 150;
            Diff2 = 208;
            Diff3 = 267;
            Diff4 = 325;
            Diff5 = 384;
            Diff6 = 442;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int ProGuitarDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return ProGuitarDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="diff">Raw integer value from songs.dta</param>
        /// <returns></returns>
        public int BandDiff(int diff)
        {
            Diff1 = 165;
            Diff2 = 215;
            Diff3 = 243;
            Diff4 = 267;
            Diff5 = 292;
            Diff6 = 345;
            return doDifficulty(diff);
        }

        /// <summary>
        /// Returns integer value from 0 to 6 (easiest to hardest) |
        /// Returns 0 for Disabled or Error |
        /// Based on official instrument difficulty tier values
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int BandDiff(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return BandDiff(Convert.ToInt32(Regex.Match(line, @"\d+").Value));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns clean Album Name
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public string GetAlbumName(string raw_line)
        {
            var album = RemoveDTAComments(raw_line);
            album = album.Replace("(album_name", "");
            album = album.Replace("(\"", "").Replace("\")", "").Trim();
            if (album.StartsWith("\"", StringComparison.Ordinal))
            {
                album = album.Substring(1, album.Length - 1);
            }
            if (album.EndsWith("\"", StringComparison.Ordinal))
            {
                album = album.Substring(0, album.Length - 1);
            }
            if (album.Contains(";"))
            {
                album = album.Substring(0, album.IndexOf(";", StringComparison.Ordinal));
            }
            album = album.Trim().Replace("\\q", "\"");

            Tools = new NemoTools();
            return Tools.FixBadChars(album);
        }

        /// <summary>
        /// Returns year or 0 if there was an error
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public int GetYear(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                return Convert.ToInt16(Regex.Match(line, @"\d+").Value);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private static string RemoveDTAComments(string raw_line)
        {
            var line = raw_line;
            var index = line.IndexOf(";", StringComparison.Ordinal);
            if (index > -1)
            {
                line = line.Substring(0, index); //remove comments
            }
            return line.Trim();
        }

        /// <summary>
        /// Returns clean Artist Name
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public string GetArtistName(string raw_line)
        {
            var artist = RemoveDTAComments(raw_line);
            artist = artist.Replace("(artist", "");
            artist = artist.Replace("(\"", "").Replace("\")", "").Trim();
            if (artist.StartsWith("\"", StringComparison.Ordinal))
            {
                artist = artist.Substring(1, artist.Length - 1);
            }
            if (artist.EndsWith("\"", StringComparison.Ordinal))
            {
                artist = artist.Substring(0, artist.Length - 1);
            }
            if (artist.Contains(";"))
            {
                artist = artist.Substring(0, artist.IndexOf(";", StringComparison.Ordinal));
            }
            artist = artist.Trim().Replace("\\q", "\"");
            Tools = new NemoTools();
            artist = Tools.FixBadChars(artist);

            //fix typical screwups
            switch (artist)
            {
                case "Blue yster Cult":
                case "Blue Oyster Cult":
                    artist = "Blue Öyster Cult";
                    break;
                case "Judy Buenda y Los Impostores":
                case "Judy Buendia y Los Impostores":
                    artist = "Judy Buendía y Los Impostores";
                    break;
                case "Mtley Cre":
                case "Motley Crue":
                    artist = "Mötley Crüe";
                    break;
                case "Queensrche":
                case "Queensryche":
                    artist = "Queensrÿche";
                    break;
                case "Maxmo Park":
                    artist = "Maxïmo Park";
                    break;
                case "Motorhead":
                case "Motrhead":
                    artist = "Motörhead";
                    break;
                case "Nio Burbuja":
                case "Nino Burbuja":
                    artist = "Niño Burbuja";
                    break;
                case "Octavio Su":
                case "Octavio Sune":
                    artist = "Octavio Suñé";
                    break;
                case "Marafer":
                case "Mariafer":
                    artist = "Maríafer";
                    break;
                case "Sintona Retro":
                case "Sintonia Retro":
                    artist = "Sintonía Retro";
                    break;
                case "AC DC":
                case "AC-DC":
                    artist = "ACDC";
                    break;
            }

            if (artist.ToLowerInvariant().Contains("weird") && artist.ToLowerInvariant().Contains("al") &&
                artist.ToLowerInvariant().Contains("yank"))
            {
                artist = "\"Weird Al\" Yankovic";
            }
            else if (artist.ToLowerInvariant().Contains("tley cr")) //one way to fix espher's mistery character issue
            {
                artist = "Mötley Crüe";
            }

            return artist;
        }

        /// <summary>
        /// Returns clean Song Name
        /// </summary>
        /// <param name="raw_line">Raw text line from songs.dta file</param>
        /// <returns></returns>
        public string GetSongName(string raw_line)
        {
            var song = "";
            if (!raw_line.Contains("Pearls; The Frailty of Matter")) //make exception for this one song
            {
                song = RemoveDTAComments(raw_line);
            }          
            else
            {
                song = raw_line;
            }
            song = song.Replace("(name", "");
            song = song.Replace("(\"", "").Replace("\")", "").Trim();
            if (song.StartsWith("\"", StringComparison.Ordinal))
            {
                song = song.Substring(1, song.Length - 1);
            }
            if (song.EndsWith("\"", StringComparison.Ordinal))
            {
                song = song.Substring(0, song.Length - 1);
            }
            if (song.Contains(";") && !song.Contains("Pearls; The Frailty of Matter")) //make exception for this one song
            {
                song = song.Substring(0, song.IndexOf(";", StringComparison.Ordinal));
            }
            song = song.Trim().Replace("\\q", "\"");
            Tools = new NemoTools();
            song = Tools.FixBadChars(song);

            //fix typical screw ups
            if (song.StartsWith("Viva la Gloria!", StringComparison.Ordinal))
            {
                song = "¡" + song;
            }
            else if (song.StartsWith("Viva la Gloria?", StringComparison.Ordinal))
            {
                song = "¿" + song;
            }

            switch (song)
            {
                case "Fjate Bien":
                case "Fijate Bien":
                    song = "Fíjate Bien";
                    break;
                case "Caamos":
                case "Caiamos":
                    song = "Caíamos";
                    break;
                case "Esto ya lo Toqu Maana":
                case "Esto ya lo Toque Manana":
                    song = "Esto ya lo Toqué Mañana";
                    break;
                case "Por Analoga":
                case "Por Analogia":
                    song = "Por Analogía";
                    break;
                case "Pltanos Con Sangre":
                case "Platanos Con Sangre":
                    song = " Plátanos Con Sangre";
                    break;
                case "Mustrame un Poco":
                case "Muestrame un Poco":
                    song = "Muéstrame un Poco";
                    break;
                case "Adicto al Dolor (Lgrimas)":
                case "Adicto al Dolor (Lagrimas)":
                    song = "Adicto al Dolor (Lágrimas)";
                    break;
                case "Nave":
                case "Naive":
                    song = "Naïve";
                    break;
                case "La Frmula":
                case "La Formula":
                    song = "La Fórmula";
                    break;
            }

            return song;
        }

        public string GetSourceGame(string raw_line)
        {
            var origin = raw_line.Replace("game_origin", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim();
            origin = RemoveDTAComments(origin);
            return origin.Trim();
        }

        public string GetSongPath(string raw_line)
        {
            return raw_line.Replace("(name", "").Replace(")", "").Replace("\"", "").Replace(".mid", "").Replace("midi_file", "").Replace("(", "").Trim();
        }

        /// <summary>
        /// Returns internal "short id" name for the song
        /// </summary>
        /// <param name="raw_line">Raw line from the songs.dta file</param>
        /// <returns></returns>
        public string GetInternalName(string raw_line)
        {
            var index1 = raw_line.IndexOf("/", StringComparison.Ordinal) + 1;
            var index2 = raw_line.IndexOf("/", index1, StringComparison.Ordinal);

            try
            {
                return raw_line.Substring(index1, index2 - index1);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public int GetGameVersion(string raw_line)
        {
            var version = raw_line.Replace("version", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim();
            version = RemoveDTAComments(version);

            try
            {
                return Convert.ToInt16(version);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetVocalParts(string raw_line)
        {
            var parts = raw_line.Replace("vocal_parts", "").Replace("'", "").Replace("(", "").Replace(")", "").Trim();
            parts = RemoveDTAComments(parts);

            try
            {
                return Convert.ToInt16(parts);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Can be used to get the raw string value of a DTA entry containing a specified field.
        /// <code>getDTAStringValue("('name' \"Ain't Messin Round (RB3 version)\")", "name") => Ain't Messin Round (RB3 version)</code>
        /// <code>getDTAStringValue("\t(\n\tdrum_bank\n\t"sfx/kit01_bank.milo"\n)", "drum_bank") => sfx/kit01_bank.milo</code>
        /// </summary>
        private string getDTAStringValue(string line, string field)
        {
            string quote = "['\"]";
            string openCloseParen = "^\\(|\\)$";
            string openCloseQuote = $"^{quote}|{quote}$";

            string result = RemoveDTAComments(line).Trim();
            result = Regex.Replace(result, $"{quote}?{field}{quote}?", "").Trim();
            result = Regex.Replace(result, openCloseParen, "").Trim();
            result = Regex.Replace(result, openCloseQuote, "").Trim();
            return result;
        }

        /// <summary>
        /// Returns int total number of audio channels in the song
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file containing the "cores" information</param>
        /// <returns></returns>
        public int GetAudioChannels(string raw_line)
        {
            var channels = RemoveDTAComments(raw_line);
            channels = channels.Replace("cores", "");
            channels = channels.Replace("'", "");
            channels = channels.Replace("(", "");
            channels = channels.Replace(")", "");
            channels = channels.Replace("-", "");
            channels = channels.Replace(" ", "");
            channels = channels.Replace("\t", "").Trim();
            return channels.Length;
        }

        private string CleanChannelsLine(string line, string remove)
        {
            var channels = line.Replace("(", "").Replace("tracks", "");
            channels = channels.Replace(remove, "");
            channels = channels.Replace(")", "");
            channels = channels.Replace(";", "");
            channels = channels.Replace("'", "").Trim();
            return channels;
        }

        public int getChannels(string line, string remove)
        {
            if (line.Contains("()")) return 0; //old GHtoRB3 songs have empty entries
            var channels = CleanChannelsLine(line, remove);

            if (string.IsNullOrEmpty(channels)) return 0;

            var number = channels.Contains(" ") ? Regex.Split(channels, " ").Length : 1;

            return number;
        }

        public int getChannelsStart(string line, string remove)
        {
            if (line.Contains("()")) return 0; //old GHtoRB3 songs have empty entries
            if (line.Contains(";")) return 0; //problem files from the RB4 to RB2 songs
            var channels = Regex.Split(CleanChannelsLine(line, remove), " ");
            
            var start = channels.Length > 0 ? Convert.ToInt16(channels[0]) : 0;
            
            return start;
        }

        public Encoding DetectEncoding(byte[] bytes)
        {
            // 1. BOM detection
            if (bytes.Length >= 3 &&
                bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return new UTF8Encoding(true);

            if (bytes.Length >= 2 &&
                bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode;

            if (bytes.Length >= 2 &&
                bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            // 2. Test UTF-8 (no BOM)
            try
            {
                var utf8 = new UTF8Encoding(false, true);
                utf8.GetString(bytes);
                return utf8;
            }
            catch { }

            // 3. Fallback to ANSI / Windows-1252
            return Encoding.GetEncoding(1252);
        }

        public Encoding DetectEncoding(string path)
        {
            return DetectEncoding(File.ReadAllBytes(path));            
        }

        /// <summary>
        /// Parses DTA file and populates the song entries to DTAEntries
        /// </summary>
        /// <param name="xDTA">byte[] contents of the DTA file</param>
        /// <returns>Boolean True/False</returns>
        private bool GetDTAEntries(byte[] xDTA)
        {
            StreamReader sr = null;
            try
            {
                //DTAEntries = new List<SongEntry>();
                DTAEntries = new List<List<string>>();
                var encoding = DetectEncoding(xDTA);
                sr = new StreamReader(new MemoryStream(xDTA), encoding);
                var open = 0;
                var closed = 0;
                var counter = 0;
                DTAEntries.Add(new List<string>());

                //read and separate all the entries into individual songs
                while (sr.Peek() > 0)
                {
                    var line = sr.ReadLine();
                    if (line.Trim().StartsWith(";;", StringComparison.Ordinal) && !line.Contains(";;ORIG_ID=")) continue; //skip commented out lines
                    if (string.IsNullOrWhiteSpace(line)) continue; //don't want empty lines
                    if (line.Trim().StartsWith(";", StringComparison.Ordinal) && open == 0) continue; //skip hmx comments in between songs

                    if ((line.Replace(" ", "").Trim().StartsWith(")(", StringComparison.Ordinal) || line.Replace(" ", "").Trim().EndsWith(")(", StringComparison.Ordinal)) && (closed + 1 == open)) //back-to-back dta entries combine )(
                    {
                        line = line.Replace(" ", "").Trim();
                        var index = line.IndexOf("(", StringComparison.Ordinal);
                        DTAEntries[counter].Add(line.Substring(0, index));
                        counter++;
                        DTAEntries.Add(new List<string>());
                        DTAEntries[counter].Add(line.Substring(index, line.Length - index));
                        open = 1;
                        closed = 0;
                        continue;
                    }
                    var inQuotes = line.TrimStart().StartsWith("\"") && line.TrimEnd().EndsWith("\"");
                    if (!inQuotes && line.Contains(")"))
                    {
                        closed = closed + line.Count(x => x == ')'); //rather than add +1, count how many instances in the string
                    }                    
                    if (!inQuotes && line.Contains("("))
                    {
                        open = open + line.Count(x => x == '('); //rather than add +1, count how many instances in the string
                    }
                    if (line.Trim() == "(album_art TRUE")
                    {
                        line = line + ")"; //rock 'n roll star is missing one
                        closed++;
                    }
                    DTAEntries[counter].Add(line);

                    if (closed != open || closed <= 0) continue;
                    open = 0;
                    closed = 0;
                    counter++;
                    DTAEntries.Add(new List<string>());
                }
                sr.Dispose();

                DTAEntries.RemoveAt(DTAEntries.Count - 1); //remove blank last entry
                return DTAEntries.Any();
            }
            catch (Exception)
            {
                sr.Dispose();
                return false;
            }
        }

        /// <summary>
        /// Returns long value for either Preview Start or Preview End
        /// </summary>
        /// <param name="raw_line">Raw line from the songs.dta file</param>
        /// <param name="request_type">0: Preview Start | 1: Preview End</param>
        /// <returns></returns>
        public long GetPreviewTimes(string raw_line, int request_type)
        {
            var preview = RemoveDTAComments(raw_line);
            preview = preview.Replace("preview", "").Replace("(", "").Replace(")", "").Replace("'", "").Replace("\"", "").Trim();
            var previews = Regex.Split(preview, " ");
            try
            {
                switch (request_type)
                {
                    case 0:
                        return Convert.ToInt32(previews[0]);
                    case 1:
                        return Convert.ToInt32(previews[1]);
                    default:
                        return Convert.ToInt32(previews[0]);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns rating value of 1-4, or -1 if incorrect/error
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public int GetRating(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                var rating = Convert.ToInt32(Regex.Match(line, @"\d+").Value);

                if (rating > 0 && rating < 5) //RB3 uses 1,2,3,4
                {
                    return rating;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns int value for drum kits used by RB3
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns>1=Hard Rock | 2=Arena | 3=Vintage | 4=Trashy | 5=Electronic </returns>
        public int GetDrumKit(string raw_line)
        {
            if (raw_line.ToLowerInvariant().Contains("kit01"))
            {
                return 1;
            }
            if (raw_line.ToLowerInvariant().Contains("kit02"))
            {
                return 2;
            }
            if (raw_line.ToLowerInvariant().Contains("kit03"))
            {
                return 3;
            }
            if (raw_line.ToLowerInvariant().Contains("kit04"))
            {
                return 4;
            }
            return raw_line.ToLowerInvariant().Contains("kit05") ? 5 : 1;
        }

        /// <summary>
        /// Returns tuning value for real guitar or real bass as string
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public string GetTuning(string raw_line)
        {
            var tuning = RemoveDTAComments(raw_line);
            tuning = tuning.Replace("(real_guitar_tuning (", "");
            tuning = tuning.Replace("(real_bass_tuning (", "");
            tuning = tuning.Replace(")", "");
            tuning = tuning.Replace("(", "");
            tuning = tuning.Replace("'", "").Trim();

            return tuning;
        }

        /// <summary>
        /// Returns preview time related information based on request type
        /// </summary>
        /// <param name="raw_line">The raw line from the songs.dta file</param>
        /// <param name="request">0 - unformatted minute:seconds | 1 - formatted preview message |
        /// 2 - unformatted duration seconds | 3 - formatted duration message | 4 - formatted preview and duration message
        /// </param>
        /// <returns></returns>
        public string PreviewTimes(string raw_line, int request)
        {
            var preview = raw_line.Replace("'", "");
            preview = preview.Replace("(", "");
            preview = preview.Replace(")", "");
            preview = preview.Replace("\"", "");
            preview = preview.Replace("preview", "").Trim();

            try
            {
                var index = preview.IndexOf(" ", StringComparison.Ordinal);
                var preview_start = Convert.ToInt32(preview.Substring(0, index)) / 1000;
                var preview_end = Convert.ToInt32(preview.Substring(index + 1, preview.Length - index - 1)) / 1000;

                var minutes = preview_start / 60;
                var seconds = "0" + (preview_start - (minutes * 60));
                if (seconds.Length > 2)
                {
                    seconds = seconds.Substring(1);
                }
                var duration = preview_end - preview_start;

                switch (request)
                {
                    case 0:
                        return (minutes + ":" + seconds);
                    case 1:
                        return ("Preview start: " + minutes + ":" + seconds);
                    case 2:
                        return (duration + " seconds");
                    case 3:
                        return ("Duration: " + duration + " seconds");
                    case 4:
                        return ("Preview start: " + minutes + ":" + seconds + "\nDuration: " + duration + " seconds");
                    default:
                        return (minutes + ":" + seconds);
                }

            }
            catch (Exception)
            {
                return "Error calculating preview time";
            }
        }

        /// <summary>
        /// Returns song duration formatted as hours:minutes:seconds
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public string GetSongDuration(string raw_line)
        {
            if (string.IsNullOrWhiteSpace(raw_line) || raw_line == "0") return "0:00";
            var line = RemoveDTAComments(raw_line);

            try
            {
                var time = Regex.Match(line, @"\d+").Value;
                time = time.Substring(0, time.Length - 3);
                return GetSongDuration(Convert.ToDouble(time));
            }
            catch (Exception)
            {
                try
                {
                    return GetSongDuration(Convert.ToDouble(raw_line));
                }
                catch
                {
                    return "0:00";
                }
            }
        }

        public string GetSongDuration(double time)
        {
            var duration = Convert.ToInt32(time);
            var minute = duration / 60;
            var second = duration % 60;
            var hours = minute / 60;

            string seconds;

            if (second == 0)
            {
                seconds = "00";
            }
            else if (second < 10)
            {
                seconds = "0" + second;
            }
            else
            {
                seconds = second.ToString(CultureInfo.InvariantCulture);
            }

            if (hours < 1) return (minute + ":" + seconds);
            minute = minute - (hours * 60);

            string minutes;
            if (minute == 0)
            {
                minutes = "00";
            }
            else if (minute < 10)
            {
                minutes = "0" + minute;
            }
            else
            {
                minutes = minute.ToString(CultureInfo.InvariantCulture);
            }
            return (hours + ":" + minutes + ":" + seconds);
        }

        public decimal GetGuidePitch(string line)
        {
            var pitch = (decimal)-3.0;

            var volume = line.Replace("guide_pitch_volume", "");
            volume = volume.Replace("'", "");
            volume = volume.Replace("(", "");
            volume = volume.Replace("\"", "");
            volume = volume.Trim();

            try
            {
                if (volume.Contains(")"))
                {
                    volume = volume.Substring(0, volume.IndexOf(")", StringComparison.Ordinal));
                }
                pitch = Convert.ToDecimal(volume);
            }
            catch (Exception)
            {
                return pitch;
            }
            return pitch;
        }

        /// <summary>
        /// Returns long value of song duration, unformatted
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public long GetSongDurationLong(string raw_line)
        {
            var line = RemoveDTAComments(raw_line);
            try
            {
                var time = Regex.Match(line, @"\d+").Value;
                return Convert.ToInt64(time);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns cleaned song id
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public string GetSongID(string raw_line)
        {
            var songid = RemoveDTAComments(raw_line);
            songid = songid.Replace("song_id", "");
            songid = songid.Replace("(", "");
            songid = songid.Replace("'", "");
            songid = songid.Replace("\"", "").Trim();
            try
            {
                songid = songid.Substring(0, songid.IndexOf(")", StringComparison.Ordinal));
            }
            catch (Exception)
            { }
            return songid;
        }

        public int GetAnimTempo(string raw_line)
        {
            var line = raw_line.ToLowerInvariant().Trim();
            if (line.Contains("ktempo"))
            {
                if (line.Contains("ktemposlow"))
                {
                    return 16;
                }
                if (line.Contains("ktempomedium"))
                {
                    return 32;
                }
                if (line.Contains("ktempofast"))
                {
                    return 64;
                }
            }
            else
            {
                try
                {
                    var tempo = Convert.ToInt16(Regex.Match(line, @"\d+").Value);
                    return tempo;
                }
                catch (Exception)
                {
                    return 32;
                }
            }
            return 32;
        }
    }

    public class SongData
    {
        public Int32 SongId { get; set; }
        public string ShortName { get; set; }
        public string FilePath { get; set; }
        public string InternalName { get; set; }
        public string Name { get; set; }
        public string OverrideName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Source { get; set; }
        public string Gender { get; set; }
        public string Genre { get; set; }
        public string RawGenre { get; set; }
        public string SubGenre { get; set; }
        public string RawSubGenre { get; set; }
        public string PercussionBank { get; set; }
        public string DrumBank { get; set; }
        public string ChartAuthor { get; set; }
        public string AttenuationValues { get; set; }
        public string OriginalAttenuationValues { get; set; }
        public string PanningValues { get; set; }
        public string ProBassTuning { get; set; }
        public string ProGuitarTuning { get; set; }
        public string InstrumentSolos { get; set; }
        public string Encoding { get; set; }
        public string Languages { get; set; }
        public string SongIdString { get; set; }
        public List<string> DTALines { get; set; }
        public string MIDIFile { get; set; }
        public string SongLink { get; set; }
        public int YearReleased { get; set; }
        public int YearRecorded { get; set; }
        public int DrumsDiff { get; set; }
        public int ProBassDiff { get; set; }
        public int BassDiff { get; set; }
        public int ProGuitarDiff { get; set; }
        public int RhythmDiff { get; set; }
        public int GuitarDiff { get; set; }
        public int KeysDiff { get; set; }
        public int ProKeysDiff { get; set; }
        public int VocalsDiff { get; set; }
        public int BandDiff { get; set; }
        public int VocalParts { get; set; }
        public int TrackNumber { get; set; }
        public int Rating { get; set; }
        public int GameVersion { get; set; }
        public int TonicNote { get; set; }
        public int Tonality { get; set; }
        public int ScrollSpeed { get; set; }
        public int ChannelsBass { get; set; }
        public int ChannelsDrums { get; set; }
        public int ChannelsGuitar { get; set; }
        public int ChannelsKeys { get; set; }
        public int ChannelsVocals { get; set; }
        public int ChannelsCrowd { get; set; }
        public int ChannelsTotal { get; set; }
        public int ChannelsBassStart { get; set; }
        public int ChannelsDrumsStart { get; set; }
        public int ChannelsGuitarStart { get; set; }
        public int ChannelsKeysStart { get; set; }
        public int ChannelsVocalsStart { get; set; }
        public int ChannelsCrowdStart { get; set; }
        public int MuteVolume { get; set; }
        public int MuteVolumeVocals { get; set; }
        public int TuningCents { get; set; }
        public int AnimTempo { get; set; }
        public Decimal GuidePitch { get; set; }
        public Int16 HopoThreshold { get; set; }
        public Int32 Length { get; set; }
        public Int32 PreviewStart { get; set; }
        public Int32 PreviewEnd { get; set; }
        public int DTAIndex { get; set; }
        public int ListIndex { get; set; }
        public int PSDelay { get; set; }
        public int VideoStartTime { get; set; }

        public bool Master { get; set; }
        public bool DoNotExport { get; set; }
        public bool IsSpecial { get; set; }
        public bool RB3Version { get; set; }
        public bool Karaoke { get; set; }
        public bool Multitrack { get; set; }
        public bool DIYStems { get; set; }
        public bool PartialMultitrack { get; set; }
        public bool UnpitchedVocals { get; set; }
        public bool Convert { get; set; }
        public bool ExpertOnly { get; set; }
        public bool RhythmKeys { get; set; }
        public bool RhythmBass { get; set; }
        public bool DoubleBass { get; set; }
        public bool DisableProKeys { get; set; }
        public bool CATemh { get; set; }
        public bool HasSongIDError { get; set; }
        public string VNAudioHash { get; set; }
        public double BPM { get; set; }
        public bool NormalizedAsAlbum { get; set; }

        public DateTime DateAdded { get; set; }

        public void Initialize()
        {
            DTALines = new List<string>();
            SongId = 0;
            Name = "";
            BPM = 120.0;
            ShortName = "";
            Artist = "";
            Album = "";
            Source = "";
            Gender = "N/A";
            Genre = "";
            RawGenre = "";
            SubGenre = "";
            RawSubGenre = "";
            FilePath = "";
            InternalName = "";
            DrumBank = "";
            PercussionBank = "";
            OverrideName = "";
            ChartAuthor = "";
            AttenuationValues = "";
            OriginalAttenuationValues = "";
            PanningValues = "";
            ProBassTuning = "";
            ProGuitarTuning = "";
            InstrumentSolos = "";
            Encoding = "";
            Languages = "";
            SongIdString = "";
            MIDIFile = "";
            SongLink = "";
            YearRecorded = 0;
            YearReleased = 0;
            DrumsDiff = 0;
            ProBassDiff = 0;
            BassDiff = 0;
            ProGuitarDiff = 0;
            GuitarDiff = 0;
            VocalsDiff = 0;
            KeysDiff = 0;
            ProKeysDiff = 0;
            BandDiff = 0;
            TrackNumber = 0;
            Rating = 4;
            GameVersion = -1;
            ScrollSpeed = 2300;
            Tonality = 0;
            TonicNote = -1;
            Length = 0;
            PreviewEnd = 0;
            PreviewStart = 0;
            ChannelsBass = 0;
            ChannelsDrums = 0;
            ChannelsGuitar = 0;
            ChannelsKeys = 0;
            ChannelsVocals = 0;
            ChannelsCrowd = 0;
            ChannelsTotal = 0;
            HopoThreshold = 170;
            MuteVolume = -96;
            MuteVolumeVocals = -12;
            TuningCents = 0;
            AnimTempo = 0;
            GuidePitch = (decimal)-3.0;
            Master = false;
            DoNotExport = false;
            IsSpecial = false;
            RB3Version = false;
            Karaoke = false;
            Multitrack = false;
            PartialMultitrack = false;
            DIYStems = false;
            UnpitchedVocals = false;
            Convert = false;
            ExpertOnly = false;
            RhythmBass = false;
            RhythmKeys = false;
            DoubleBass = false;
            DisableProKeys = false;
            HasSongIDError = false;
            VNAudioHash = "";
            CATemh = false;
            DTAIndex = 0;
            ListIndex = 0;
            PSDelay = 0;
            VideoStartTime = 0;
            ChannelsBassStart = 0;
            ChannelsDrumsStart = 0;
            ChannelsGuitarStart = 0;
            ChannelsKeysStart = 0;
            ChannelsVocalsStart = 0;
            ChannelsCrowdStart = 0;
            DateAdded = DateTime.Now;
            NormalizedAsAlbum = false;
        }

        public string GetRating()
        {
            string rating;

            switch (Rating)
            {
                case 1:
                    rating = "FF";
                    break;
                case 2:
                    rating = "SR";
                    break;
                case 3:
                    rating = "M";
                    break;
                default:
                    rating = "NR";
                    break;
            }

            return rating;
        }

        public string IsMaster()
        {
            return Master ? "Yes" : "No";
        }

        public string GetGender(bool NonBinary = false)
        {            
            return VocalsDiff == 0 || Gender == null ? "N/A" : (NonBinary ? Gender.Replace("Male", "Masc.").Replace("Female","Fem.") : Gender);
        }

        public string GetAnimTempoText()
        {
            switch (AnimTempo)
            {
                case 16:
                    return "kTempoSlow";
                case 32:
                    return "kTempoMedium";
                case 64:
                    return "kTempoFast";
                default:
                    return "kTempoMedium";
            }
        }

        public string GetSource()
        {
            if (string.IsNullOrWhiteSpace(Source)) return "";
            string source;
            switch (Source.ToLowerInvariant())
            {
                case "rb1":
                    source = ShortName.ToLowerInvariant().Contains("ugc") && !ShortName.Contains("##") ? "RBN1" : "RB1";
                    break;
                case "acdc":
                    source = "AC/DC";
                    break;
                case "rb2":
                    source = ShortName.ToLowerInvariant().Contains("ugc") && !ShortName.Contains("##") ? "RBN1" : "RB2";
                    break;
                case "rb3":
                    source = "RB3";
                    break;
                case "rb4":
                    source = "RB4";
                    break;
                case "rb1_dlc":
                    source = ShortName.ToLowerInvariant().Contains("_live") && Artist == "AC/DC" ? "AC/DC" : "RB1 DLC";
                    break;
                case "rb2_dlc":
                    source = "RB2 DLC";
                    break;
                case "rb3_dlc":
                    source = "RB3 DLC";
                    break;
                case "rb4_dlc":
                    source = "RB4 DLC";
                    break;
                case "greenday":
                case "gdrb":
                    source = "GD:RB";
                    break;
                case "blitz":
                    source = "Blitz";
                    break;
                case "lego":
                    source = "Lego";
                    break;
                case "ugc":
                case "rbn1":
                    source = "RBN1";
                    break;
                case "custom":
                    source = "Custom";
                    break;
                case "beatles":
                case "tbrb":
                    source = "TB:RB";
                    break;
                case "rbn2":
                    source = "RBN2";
                    break;
                case "ugc_plus":
                    if (SongId > 9999999)
                    {
                        source = ShortName.ToLowerInvariant().Contains("tbrb_") ? "TB:RB" : "Custom";
                    }
                    else
                    {
                        source = "RBN2";
                    }
                    break;
                default:
                    source = "DLC";
                    break;
            }
            return source;
        }

        public string GetDifficulty(int diff)
        {
            var difficulty = "No Part";
            switch (diff)
            {
                case 0:
                    difficulty = "No Part";
                    break;
                case 1:
                    difficulty = "Warmup";
                    break;
                case 2:
                    difficulty = "Apprentice";
                    break;
                case 3:
                    difficulty = "Solid";
                    break;
                case 4:
                    difficulty = "Moderate";
                    break;
                case 5:
                    difficulty = "Challenging";
                    break;
                case 6:
                    difficulty = "Nightmare";
                    break;
                case 7:
                    difficulty = "Impossible";
                    break;
            }
            return difficulty;
        }                
    }
}