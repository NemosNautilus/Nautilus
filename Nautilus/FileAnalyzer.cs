using LibForge.Midi;
using Microsoft.VisualBasic;
using MidiCS;
using NAudio.Midi;
using Nautilus.Properties;
using Nautilus.Texture;
using Nautilus.x360;
using NautilusFREE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Un4seen.Bass;
using Application = System.Windows.Forms.Application;

namespace Nautilus
{
    public partial class SongAnalyzer : Form
    {
        public bool ExitonClose;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private readonly string config;
        private string InputFile;
        private bool HaveFile;
        private readonly string argument;
        NAudio.Midi.MidiFile MIDIFile;
        private List<TempoEvent> TempoEvents;
        private List<TimeSignature> TimeSignatures;
        private List<LyricPhrase> LeadVocals;
        private List<LyricPhrase> Harmonies1;
        private List<LyricPhrase> Harmonies2;
        private List<LyricPhrase> Harmonies3;
        private List<string> MoggFiles;
        private List<string> filesToProcess;
        private string proDrumsFolder;
        private List<string> SongsToSearch;
        private string SearchTerm;
        private string LyricSearchFolder;

        private int TotalMIDINotes;
        private int TotalPlayableNotes;
        private int TicksPerQuarter;

        private int DrumsXPlus;
        private int DrumsX;
        private int DrumsXKick;
        private int DrumsXSnare;
        private int DrumsXBlueTom;
        private int DrumsXBlueCymbal;
        private int DrumsXYellowTom;
        private int DrumsXYellowCymbal;
        private int DrumsXGreenTom;
        private int DrumsXGreenCymbal;
        private int DrumsH;
        private int DrumsHKick;
        private int DrumsHSnare;
        private int DrumsHBlueTom;
        private int DrumsHBlueCymbal;
        private int DrumsHYellowTom;
        private int DrumsHYellowCymbal;
        private int DrumsHGreenTom;
        private int DrumsHGreenCymbal;
        private int DrumsM;
        private int DrumsMKick;
        private int DrumsMSnare;
        private int DrumsMBlueTom;
        private int DrumsMBlueCymbal;
        private int DrumsMYellowTom;
        private int DrumsMYellowCymbal;
        private int DrumsMGreenTom;
        private int DrumsMGreenCymbal;
        private int DrumsE;
        private int DrumsEKick;
        private int DrumsESnare;
        private int DrumsEBlueTom;
        private int DrumsEBlueCymbal;
        private int DrumsEYellowTom;
        private int DrumsEYellowCymbal;
        private int DrumsEGreenTom;
        private int DrumsEGreenCymbal;
        private int DrumsAnim;
        private int ProDrumsAnim;
        private int DrumsSolo;
        private double DrumsSoloLength;
        private double DrumsSoloNPS;
        private int DrumsSwells;
        private int DrumsRolls;
        private int DrumsOD;
        private int DrumsToms;
        private int DrumsFills;
        private List<int> DrumsDensity;
        private bool HasDiscoFlip;
        
        private int BassX;
        private int BassXO;
        private int BassXB;
        private int BassXY;
        private int BassXR;
        private int BassXG;
        private int BassH;
        private int BassHO;
        private int BassHB;
        private int BassHY;
        private int BassHR;
        private int BassHG;
        private int BassM;
        private int BassMO;
        private int BassMB;
        private int BassMY;
        private int BassMR;
        private int BassMG;
        private int BassE;
        private int BassEO;
        private int BassEB;
        private int BassEY;
        private int BassER;
        private int BassEG;
        private int BassTrills;
        private int BassTremolos;
        private int BassSolo;
        private double BassSoloLength;
        private double BassSoloNPS;
        private int BassOD;
        private int BassHOPOoff;
        private int BassHOPOon;
        private int BassLHAnim;
        private List<int> BassDensity;

        private int ProBass17X;
        private int ProBass17H;
        private int ProBass17M;
        private int ProBass17E;
        private int ProBass17Misc;
        private int ProBass22X;
        private int ProBass22H;
        private int ProBass22M;
        private int ProBass22E;
        private int ProBass22Misc;
        
        private int GuitarX;
        private int GuitarXO;
        private int GuitarXB;
        private int GuitarXY;
        private int GuitarXR;
        private int GuitarXG;
        private int GuitarH;
        private int GuitarHO;
        private int GuitarHB;
        private int GuitarHY;
        private int GuitarHR;
        private int GuitarHG;
        private int GuitarM;
        private int GuitarMO;
        private int GuitarMB;
        private int GuitarMY;
        private int GuitarMR;
        private int GuitarMG;
        private int GuitarE;
        private int GuitarEO;
        private int GuitarEB;
        private int GuitarEY;
        private int GuitarER;
        private int GuitarEG;
        private int GuitarTrills;
        private int GuitarTremolos;
        private int GuitarSolo;
        private double GuitarSoloLength;
        private double GuitarSoloNPS;
        private int GuitarOD;
        private int GuitarHOPOoff;
        private int GuitarHOPOon;
        private int GuitarLHAnim;
        private List<int> GuitarDensity;

        private int ProGuitar17X;
        private int ProGuitar17H;
        private int ProGuitar17M;
        private int ProGuitar17E;
        private int ProGuitar17Misc;
        private int ProGuitar22X;
        private int ProGuitar22H;
        private int ProGuitar22M;
        private int ProGuitar22E;
        private int ProGuitar22Misc;

        private int KeysX;
        private int KeysXO;
        private int KeysXB;
        private int KeysXY;
        private int KeysXR;
        private int KeysXG;
        private int KeysH;
        private int KeysHO;
        private int KeysHB;
        private int KeysHY;
        private int KeysHR;
        private int KeysHG;
        private int KeysM;
        private int KeysMO;
        private int KeysMB;
        private int KeysMY;
        private int KeysMR;
        private int KeysMG;
        private int KeysE;
        private int KeysEO;
        private int KeysEB;
        private int KeysEY;
        private int KeysER;
        private int KeysEG;
        private int KeysTrills;
        private int KeysSolo;
        private double KeysSoloLength;
        private double KeysSoloNPS;
        private int KeysOD;
        private List<int> KeysDensity;
        
        private int ProKeysX;
        private int ProKeysXBlack;
        private int ProKeysXTrills;
        private int ProKeysXGliss;
        private int ProKeysXOD;
        private int ProKeysXSolo;
        private int ProKeysXRanges;
        private int ProKeysH;
        private int ProKeysHBlack;
        private int ProKeysM;
        private int ProKeysMBlack;
        private int ProKeysE;
        private int ProKeysEBlack;
        private double ProKeysSoloLength;
        private double ProKeysSoloNPS;
        private List<int> ProKeysDensity;
        
        private int KeysAnimRH;
        private int KeysAnimLH;
        
        private int Vocals;
        private int VocalsOD;
        private int VocalsPhrases;
        private int VocalsHidPerc;
        private int VocalsDispPerc;
        private int VocalsLyricShift;
        private int VocalsRangeShift;
        private int VocalsTalkies;
        private string PercType;
        private List<int> VocalsDensity;

        private int Harm1;
        private int Harm1OD;
        private int Harm1Phrases;
        private int Harm1LyricShift;
        private int Harm1RangeShift;
        private int Harm1Talkies;
        private List<int> Harm1Density;

        private int Harm2;
        private int Harm2OD;
        private int Harm2Phrases;
        private int Harm2LyricShift;
        private int Harm2RangeShift;
        private int Harm2Talkies;
        private List<int> Harm2Density;

        private int Harm3;
        private int Harm3OD;
        private int Harm3Phrases;
        private int Harm3LyricShift;
        private int Harm3RangeShift;
        private int Harm3Talkies;
        private List<int> Harm3Density;
        
        private double LengthSeconds;
        private long LengthLong;
        private long MIDISize;

        private bool HasBeat;
        private bool HasVenue;
        private bool HasEvents;
        private bool HasBRE;
        private bool HasEndEvent;

        private int SongsWithLyric;
        private int LyricsFound;
        private int TotalCONs;
        private int TotalMIDIs;
        private List<string[,]> NeedProDrums;
        private List<string[,]> NeedTomMarkers;
        private bool CancelWorkers;
        private static Color mMenuBackground;
        private string vocalsFolder;
        private bool isSearchingForUnpitchedVocals;
        private List<string[,]> HasUnpitchedVocals;
        private List<string[,]> HasNoVocals;
        private List<string[,]> MissingFills;
        private List<string[,]> MissingOverdrive;
        private nTools nautilus3;
        private bool doMoggBatch;
        private bool isSearchingForToms;

        public SongAnalyzer(string arg)
        {
            InitializeComponent();
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();
            Tools = new NemoTools();
            Parser = new DTAParser();
            NeedProDrums = new List<string[,]>();
            NeedTomMarkers = new List<string[,]>();
            HasUnpitchedVocals = new List<string[,]>();
            HasNoVocals = new List<string[,]>();
            MissingFills = new List<string[,]>();
            MissingOverdrive = new List<string[,]>();
            SongsToSearch = new List<string>();
            MoggFiles = new List<string>();
            filesToProcess = new List<string>();
            argument = arg;
            config = Application.StartupPath + "\\bin\\config\\analyzer.config";
            nautilus3 = new nTools();
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(config)) return;

            var sr = new StreamReader(config);
            try
            {
                while (sr.Peek() >= 0)
                {
                    calculateNPS.Checked = sr.ReadLine().Contains("True");
                    calculateDensity.Checked = sr.ReadLine().Contains("True");
                    breakDownInstruments.Checked = sr.ReadLine().Contains("True");
                    chkMisc.Checked = sr.ReadLine().Contains("True");
                    chkDrums.Checked = sr.ReadLine().Contains("True");
                    chkBass.Checked = sr.ReadLine().Contains("True");
                    chkGuitar.Checked = sr.ReadLine().Contains("True");
                    chkVocals.Checked = sr.ReadLine().Contains("True");
                    chkHarms.Checked = sr.ReadLine().Contains("True");
                    chkKeys.Checked = sr.ReadLine().Contains("True");
                    chkProKeys.Checked = sr.ReadLine().Contains("True");
                    try
                    {
                        analyzeMoggFileInCONs.Checked = sr.ReadLine().Contains("True");
                        separateFilesThatAreMissingProDrums.Checked = sr.ReadLine().Contains("True");
                        onlyListSongs.Checked = sr.ReadLine().Contains("True");
                        showLyricsToolStrip.Checked = sr.ReadLine().Contains("True");
                        ignoreHarmonies.Checked = sr.ReadLine().Contains("True");
                        analyzePngxboxFile.Checked = sr.ReadLine().Contains("True");
                        displayPhraseTiming.Checked = sr.ReadLine().Contains("True");
                        separateUnpitchedSongs.Checked = sr.ReadLine().Contains("True");
                        chkProBass.Checked = sr.ReadLine().Contains("True");
                        chkProGuitar.Checked = sr.ReadLine().Contains("True");
                    }
                    catch (Exception)
                    {} // old config missing these values;
                }
            }
            catch (Exception ex)
            {
                Log("Error loading configuration:");
                Log(ex.Message);
            }
            sr.Dispose();
        }

        private void SaveConfig()
        {
            var sw = new StreamWriter(config, false);
            sw.WriteLine("CalculateNPS=" + calculateNPS.Checked);
            sw.WriteLine("CalculateDensity=" + calculateDensity.Checked);
            sw.WriteLine("BreakdownInstruments=" + breakDownInstruments.Checked);
            sw.WriteLine("DoMisc=" + chkMisc.Checked);
            sw.WriteLine("DoDrums=" + chkDrums.Checked);
            sw.WriteLine("DoBass=" + chkBass.Checked);
            sw.WriteLine("DoGuitar=" + chkGuitar.Checked);
            sw.WriteLine("DoVocals=" + chkVocals.Checked);
            sw.WriteLine("DoHarms=" + chkHarms.Checked);
            sw.WriteLine("DoKeys=" + chkKeys.Checked);
            sw.WriteLine("DoProKeys=" + chkProKeys.Checked);
            sw.WriteLine("AnalyzeMoggFiles=" + analyzeMoggFileInCONs.Checked);
            sw.WriteLine("SeparateIfNeedProDrums=" + separateFilesThatAreMissingProDrums.Checked);
            sw.WriteLine("OnlyListSongs=" + onlyListSongs.Checked);
            sw.WriteLine("DisplayLyrics=" + showLyricsToolStrip.Checked);
            sw.WriteLine("IgnoreHarmonies=" + ignoreHarmonies.Checked);
            sw.WriteLine("AnalyzeAlbumArt=" + analyzePngxboxFile.Checked);
            sw.WriteLine("DisplayPhraseTiming=" + displayPhraseTiming.Checked);
            sw.WriteLine("SeparateUnpitchedVocals=" + separateUnpitchedSongs.Checked);
            sw.WriteLine("DoProBass=" + chkProBass.Checked);
            sw.WriteLine("DoProGuitar=" + chkProGuitar.Checked);
            sw.Dispose();
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picloading.Visible) return;

            ResetAll();
            ResetFormText();
            if (Parser != null && Parser.Songs != null)
            {
                Parser.Songs[0] = null;
            }
            runAnalysisAgain.Enabled = false;
            lstStats.Items.Clear();
            filesToProcess.Clear();

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            List<string> moggToProcess = new List<string>();
            List<string> pngToProcess = new List<string>();
            
            for (var i = 0; i < files.Count(); i++)
            {
                if (VariousFunctions.ReadFileType(files[i]) == XboxFileType.STFS)
                {
                    Log("Received file " + Path.GetFileName(files[i]));
                    filesToProcess.Add(files[i]);                    
                }
                else switch (Path.GetExtension(files[i]).ToLowerInvariant())
                {
                    case ".mid":
                        Log("Received MIDI file " + Path.GetFileName(files[i]));
                        filesToProcess.Add(files[i]);
                        break;
                    case ".mogg":
                        Log("Received MOGG file " + Path.GetFileName(files[i]));
                        moggToProcess.Add(files[i]);
                        break;
                    case ".yarg_mogg":
                        Log("Received YARG MOGG file " + Path.GetFileName(files[i]));
                        moggToProcess.Add(files[i]);
                        break;
                    case ".png_ps3":
                        Log("Received PNG_PS3 file " + Path.GetFileName(files[i]));
                        pngToProcess.Add(files[i]);
                        break;
                    case ".png_ps4":
                        Log("Received PNG_PS4 file " + Path.GetFileName(files[i]));
                        pngToProcess.Add(files[i]);
                        break;
                    case ".png_xbox":
                        Log("Received PNG_XBOX file " + Path.GetFileName(files[i]));
                        pngToProcess.Add(files[i]);
                        break;
                    case ".pkg":
                        Log("Received PS3 PKG file " + Path.GetFileName(files[i]));
                        filesToProcess.Add(files[i]);
                        break;
                        case ".rbmid_ps4":
                            Log("Received PS4 RBMID file " + Path.GetFileName(files[i]));
                            filesToProcess.Add(files[i]);
                            break;
                     default:
                        Log("Received invalid file " + Path.GetFileName(files[i]));
                        break;
                }
            }
            if (filesToProcess.Count() + moggToProcess.Count() + pngToProcess.Count() == 0)
            {
                Log("No valid files to process");
                return;
            }

            if (pngToProcess.Count > 0)
            {
                foreach (var png in pngToProcess)
                {
                    if (pngToProcess.Count() == 1)
                    {
                        lstStats.Items.Clear();
                    }
                    Log("");
                    Log("Beginning analysis of album art file");
                    AnalyzeAlbumArt(png);
                    HaveFile = true;
                    Log("");
                    Log("Analysis of album art file complete");
                }                
            }
            if (filesToProcess.Count > 0)
            {
                ShowWait(true);                
                exportHarmonies.Enabled = false;
                exportPartVocals.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
                return;
            }
            if (moggToProcess.Count > 0)
            {
                MoggFiles = moggToProcess;                
                ShowWait(true);
                doMoggBatch = moggToProcess.Count > 1;
                backgroundWorker2.RunWorkerAsync();
                return;
            }   
        }

        private enum FileType
        {
            MIDI, MOGG, PNG_XBOX
        }

        private string ExtractFile(string input, string output, FileType type)
        {
            var extract = "mid";
            var to_extract = "MIDI";
            switch (type)
            {
                case FileType.MIDI:
                    extract = "mid";
                    break;
                case FileType.MOGG:
                    extract = "mogg";
                    to_extract = "MOGG";
                    break;
                case FileType.PNG_XBOX:
                    extract = "png_xbox";
                    to_extract = "PNG_XBOX";
                    break;
            }
            if (!Parser.ExtractDTA(input))
            {
                MessageBox.Show("Can't find songs.dta inside this file\nI can't extract the " + to_extract + " without it, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return "";
            }
            if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
            {
                MessageBox.Show("Something went wrong in reading the songs.dta file\nI can't extract the " + to_extract + " without it, sorry", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            if (Parser.Songs.Count > 1)
            {
                MessageBox.Show("Packs are not valid here, try single songs only", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return "";
            }
            var xPackage = new STFSPackage(input);
            if (!xPackage.ParseSuccess)
            {
                MessageBox.Show("There was an error parsing that CON file\nTry again", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            try
            {
                var internalname = Parser.Songs[0].InternalName;
                var xFile = xPackage.GetFile("songs/" + internalname + (type == FileType.PNG_XBOX ? "/gen/" : "/") + internalname + (type == FileType.PNG_XBOX ? "_keep." : ".") + extract);
                if (xFile != null)
                {
                    Tools.DeleteFile(output);
                    if (!xFile.ExtractToFile(output))
                    {
                        MessageBox.Show("Couldn't extract the " + to_extract + " from that CON file\nTry extracting it manually", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        xPackage.CloseIO();
                        return "";
                    }
                }
                else
                {
                    MessageBox.Show("Couldn't find the " + to_extract + " in that CON file\nTry extracting it manually", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    xPackage.CloseIO();
                    return "";
                }
                xPackage.CloseIO();
                if (!File.Exists(output)) return "";
                switch (type)
                {
                    case FileType.MIDI:
                        AnalyzeMIDI(output);
                        Tools.DeleteFile(output);
                        break;
                    case FileType.MOGG:
                        nautilus3.WriteOutData(File.ReadAllBytes(output), output);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error processing that CON file\nError says: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                xPackage.CloseIO();
            }
            return output;
        }

        private double GetRealtime(long absdelta)
        {
            //code by raynebc
            var BPM = 120.0;   //As per the MIDI specification, until a tempo change is reached, 120BPM is assumed
            var reldelta = absdelta;   //The number of delta ticks between the delta time being converted and the tempo change immediately at or before it
            var time = 0.0;   //The real time position of the tempo change immediately at or before the delta time being converted
            foreach (var tempo in TempoEvents.Where(tempo => tempo.AbsoluteTime <= absdelta))
            {
                BPM = tempo.BPM;
                time = tempo.RealTime;
                reldelta = absdelta - tempo.AbsoluteTime;
            }
            time += (double)reldelta / TicksPerQuarter * (60000.0 / BPM);
            return Math.Round(time / 1000, 3);
        }
        
        private void BuildTimeSignatureList()
        {
            TimeSignatures = new List<TimeSignature>();
            foreach (var ev in MIDIFile.Events[0])
            {
                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;
                var signature = (MetaEvent)ev;
                if (signature.MetaEventType != NAudio.Midi.MetaEventType.TimeSignature) continue;
                try
                {
                    //Track the time signature change
                    var index1 = signature.ToString().IndexOf(" ", signature.ToString().IndexOf("TimeSignature", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
                    var index2 = signature.ToString().IndexOf("/", StringComparison.Ordinal);
                    var numerator = Convert.ToInt16(signature.ToString().Substring(index1, index2 - index1));
                    //Track the time signature change
                    index1 = signature.ToString().IndexOf("/", StringComparison.Ordinal) + 1;
                    index2 = signature.ToString().IndexOf(" ", signature.ToString().IndexOf("/", StringComparison.Ordinal), StringComparison.Ordinal);
                    int denominator;
                    var split = signature.ToString().Substring(index1, index2 - index1);
                    if (split == "Unknown") //for some Onyx x/1 time signatures
                    {
                        denominator = 1;
                    }
                    else
                    {
                        denominator = Convert.ToInt16(split);
                    }
                    var time_sig = new TimeSignature
                    {
                        AbsoluteTime = ev.AbsoluteTime,
                        Numerator = numerator,
                        Denominator = denominator
                    };
                    TimeSignatures.Add(time_sig);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading time signature event:\n" + ex.Message + "\n\nEvent:\n" + signature.ToString() + "\n\nWill skip this time signature event but this may cause other problems when processing the MIDI file", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BuildTempoList()
        {
            //code provided by raynebc
            //Build tempo list
            var currentbpm = 120.00;
            var realtime = 0.0;
            var reldelta = 0;   //The number of delta ticks since the last tempo change
            TempoEvents = new List<TempoEvent>();
            foreach (var ev in MIDIFile.Events[0])
            {
                reldelta += ev.DeltaTime;
                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;
                var tempo = (MetaEvent)ev;
                if (tempo.MetaEventType != NAudio.Midi.MetaEventType.SetTempo) continue;
                var relativetime = (double)reldelta / TicksPerQuarter * (60000.0 / currentbpm);
                var index1 = tempo.ToString().IndexOf("SetTempo", StringComparison.Ordinal) + 9;
                var index2 = tempo.ToString().IndexOf("bpm", StringComparison.Ordinal);
                var bpm = tempo.ToString().Substring(index1, index2 - index1);
                currentbpm = Convert.ToDouble(bpm);   //As per the MIDI specification, until a tempo change is reached, 120BPM is assumed
                realtime += relativetime;   //Add that to the ongoing current real time of the MIDI
                reldelta = 0;
                var tempo_event = new TempoEvent
                {
                    AbsoluteTime = tempo.AbsoluteTime,
                    RealTime = realtime,
                    BPM = currentbpm
                };
                TempoEvents.Add(tempo_event);
            }
        }

        private void AnalyzeDrums(IList<MidiEvent> track)
        {
            NoteOnEvent YellowTom = null;
            NoteOnEvent BlueTom = null;
            NoteOnEvent GreenTom = null;
            NoteOnEvent DrumsSoloNote = null;
            var DrumsNPS = 0;
            var DrumsLength = 0;

            for (var z = 0; z < track.Count(); z++)
            {
                var notes = track[z];

                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        if (HasDiscoFlip) continue;
                        var drum_event = (MetaEvent)notes;
                        if (drum_event.ToString().Contains("drums0d") || drum_event.ToString().Contains("drums1d") || 
                            drum_event.ToString().Contains("drums2d") || drum_event.ToString().Contains("drums3d"))
                        {
                            HasDiscoFlip = true;
                        }
                        break;
                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        TotalMIDINotes++;
                        switch (note.NoteNumber)
                        {
                            case 127:
                                DrumsSwells++;
                                break;
                            case 126:
                                DrumsRolls++;
                                break;
                            case 120:
                                DrumsFills++;
                                break;
                            case 116:
                                DrumsOD++;
                                break;
                            case 112:
                                GreenTom = note;
                                DrumsToms++;
                                break;
                            case 111:
                                BlueTom = note;
                                DrumsToms++;
                                break;
                            case 110:
                                YellowTom = note;
                                DrumsToms++;
                                break;
                            case 103:
                                DrumsSolo++;
                                DrumsSoloNote = note;
                                DrumsNPS = 0;
                                if (DrumsSoloNote.NoteLength > DrumsLength)
                                {
                                    DrumsLength = DrumsSoloNote.NoteLength;
                                    DrumsSoloLength = NoteLengthInSecs(DrumsSoloNote.AbsoluteTime, DrumsSoloNote.NoteLength);
                                }
                                break;
                            case 100:
                                if (GreenTom == null)
                                {
                                    DrumsXGreenCymbal++;
                                }
                                else if (note.AbsoluteTime >= GreenTom.AbsoluteTime &&
                                         note.AbsoluteTime <= GreenTom.AbsoluteTime + GreenTom.NoteLength)
                                {
                                    DrumsXGreenTom++;
                                }
                                else
                                {
                                    DrumsXGreenCymbal++;
                                }
                                break;
                            case 99:
                                if (BlueTom == null)
                                {
                                    DrumsXBlueCymbal++;
                                }
                                else if (note.AbsoluteTime >= BlueTom.AbsoluteTime &&
                                         note.AbsoluteTime <= BlueTom.AbsoluteTime + BlueTom.NoteLength)
                                {
                                    DrumsXBlueTom++;
                                }
                                else
                                {
                                    DrumsXBlueCymbal++;
                                }
                                break;
                            case 98:
                                if (YellowTom == null)
                                {
                                    DrumsXYellowCymbal++;
                                }
                                else if (note.AbsoluteTime >= YellowTom.AbsoluteTime &&
                                         note.AbsoluteTime <= YellowTom.AbsoluteTime + YellowTom.NoteLength)
                                {
                                    DrumsXYellowTom++;
                                }
                                else
                                {
                                    DrumsXYellowCymbal++;
                                }
                                break;
                            case 97:
                                DrumsXSnare++;
                                break;
                            case 96:
                                DrumsXKick++;
                                break;
                            case 95:
                                DrumsXPlus++;
                                break;
                            case 88:
                                if (GreenTom == null)
                                {
                                    DrumsHGreenCymbal++;
                                }
                                else if (note.AbsoluteTime >= GreenTom.AbsoluteTime &&
                                         note.AbsoluteTime <= GreenTom.AbsoluteTime + GreenTom.NoteLength)
                                {
                                    DrumsHGreenTom++;
                                }
                                else
                                {
                                    DrumsHGreenCymbal++;
                                }
                                break;
                            case 87:
                                if (BlueTom == null)
                                {
                                    DrumsHBlueCymbal++;
                                }
                                else if (note.AbsoluteTime >= BlueTom.AbsoluteTime &&
                                         note.AbsoluteTime <= BlueTom.AbsoluteTime + BlueTom.NoteLength)
                                {
                                    DrumsHBlueTom++;
                                }
                                else
                                {
                                    DrumsHBlueCymbal++;
                                }
                                break;
                            case 86:
                                if (YellowTom == null)
                                {
                                    DrumsHYellowCymbal++;
                                }
                                else if (note.AbsoluteTime >= YellowTom.AbsoluteTime &&
                                         note.AbsoluteTime <= YellowTom.AbsoluteTime + YellowTom.NoteLength)
                                {
                                    DrumsHYellowTom++;
                                }
                                else
                                {
                                    DrumsHYellowCymbal++;
                                }
                                break;
                            case 85:
                                DrumsHSnare++;
                                break;
                            case 84:
                                DrumsHKick++;
                                break;
                            case 76:
                                if (GreenTom == null)
                                {
                                    DrumsMGreenCymbal++;
                                }
                                else if (note.AbsoluteTime >= GreenTom.AbsoluteTime &&
                                         note.AbsoluteTime <= GreenTom.AbsoluteTime + GreenTom.NoteLength)
                                {
                                    DrumsMGreenTom++;
                                }
                                else
                                {
                                    DrumsMGreenCymbal++;
                                }
                                break;
                            case 75:
                                if (BlueTom == null)
                                {
                                    DrumsMBlueCymbal++;
                                }
                                else if (note.AbsoluteTime >= BlueTom.AbsoluteTime &&
                                         note.AbsoluteTime <= BlueTom.AbsoluteTime + BlueTom.NoteLength)
                                {
                                    DrumsMBlueTom++;
                                }
                                else
                                {
                                    DrumsMBlueCymbal++;
                                }
                                break;
                            case 74:
                                if (YellowTom == null)
                                {
                                    DrumsMYellowCymbal++;
                                }
                                else if (note.AbsoluteTime >= YellowTom.AbsoluteTime &&
                                         note.AbsoluteTime <= YellowTom.AbsoluteTime + YellowTom.NoteLength)
                                {
                                    DrumsMYellowTom++;
                                }
                                else
                                {
                                    DrumsMYellowCymbal++;
                                }
                                break;
                            case 73:
                                DrumsMSnare++;
                                break;
                            case 72:
                                DrumsMKick++;
                                break;
                            case 64:
                                if (GreenTom == null)
                                {
                                    DrumsEGreenCymbal++;
                                }
                                else if (note.AbsoluteTime >= GreenTom.AbsoluteTime &&
                                         note.AbsoluteTime <= GreenTom.AbsoluteTime + GreenTom.NoteLength)
                                {
                                    DrumsEGreenTom++;
                                }
                                else
                                {
                                    DrumsEGreenCymbal++;
                                }
                                break;
                            case 63:
                                if (BlueTom == null)
                                {
                                    DrumsEBlueCymbal++;
                                }
                                else if (note.AbsoluteTime >= BlueTom.AbsoluteTime &&
                                         note.AbsoluteTime <= BlueTom.AbsoluteTime + BlueTom.NoteLength)
                                {
                                    DrumsEBlueTom++;
                                }
                                else
                                {
                                    DrumsEBlueCymbal++;
                                }
                                break;
                            case 62:
                                if (YellowTom == null)
                                {
                                    DrumsEYellowCymbal++;
                                }
                                else if (note.AbsoluteTime >= YellowTom.AbsoluteTime &&
                                         note.AbsoluteTime <= YellowTom.AbsoluteTime + YellowTom.NoteLength)
                                {
                                    DrumsEYellowTom++;
                                }
                                else
                                {
                                    DrumsEYellowCymbal++;
                                }
                                break;
                            case 61:
                                DrumsESnare++;
                                break;
                            case 60:
                                DrumsEKick++;
                                break;
                            default:
                                if (note.NoteNumber <= 51 && note.NoteNumber >= 46)
                                {
                                    ProDrumsAnim++;
                                }
                                if (note.NoteNumber <= 45 && note.NoteNumber >= 24)
                                {
                                    DrumsAnim++;
                                }
                                break;
                        }
                        if (note.NoteNumber > 100 || note.NoteNumber < 96) continue;
                        if (calculateDensity.Checked)
                        {
                            var measure = GetMeasure(note.AbsoluteTime);
                            if (!DrumsDensity.Contains(measure))
                            {
                                DrumsDensity.Add(measure);
                            }
                        }
                        if (DrumsSoloNote == null) continue;
                        if (note.AbsoluteTime < DrumsSoloNote.AbsoluteTime || note.AbsoluteTime > DrumsSoloNote.AbsoluteTime + DrumsSoloNote.NoteLength)
                        {
                            if (DrumsNPS / NoteLengthInSecs(DrumsSoloNote.AbsoluteTime, DrumsSoloNote.NoteLength) > DrumsSoloNPS)
                            {
                                DrumsSoloNPS = Math.Round(DrumsNPS / NoteLengthInSecs(DrumsSoloNote.AbsoluteTime, DrumsSoloNote.NoteLength), 1);
                            }
                        }
                        else
                        {
                            DrumsNPS++;
                        }
                        break;
                    }
                }
            DrumsX = DrumsXPlus + DrumsXSnare + DrumsXKick + DrumsXBlueCymbal + DrumsXBlueTom + DrumsXGreenCymbal + DrumsXGreenTom + DrumsXYellowCymbal + DrumsXYellowTom;
            DrumsH = DrumsHSnare + DrumsHKick + DrumsHBlueCymbal + DrumsHBlueTom + DrumsHGreenCymbal + DrumsHGreenTom + DrumsHYellowCymbal + DrumsHYellowTom;
            DrumsM = DrumsMSnare + DrumsMKick + DrumsMBlueCymbal + DrumsMBlueTom + DrumsMGreenCymbal + DrumsMGreenTom + DrumsMYellowCymbal + DrumsMYellowTom;
            DrumsE = DrumsESnare + DrumsEKick + DrumsEBlueCymbal + DrumsEBlueTom + DrumsEGreenCymbal + DrumsEGreenTom + DrumsEYellowCymbal + DrumsEYellowTom;
            TotalPlayableNotes += DrumsX + DrumsH + DrumsM + DrumsE;
        }
                
        private void AnalyzeBass(IEnumerable<MidiEvent> track)
        {
            NoteOnEvent BassSoloNote = null;
            var BassNPS = 0;
            var BassLength = 0;
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                var note = (NoteOnEvent)notes;
                if (note.Velocity <= 0) continue;

                TotalMIDINotes++;
                switch (note.NoteNumber)
                {
                    case 127:
                        BassTrills++;
                        break;
                    case 126:
                        BassTremolos++;
                        break;
                    case 124:
                    case 123:
                    case 122:
                    case 121:
                    case 120:
                        HasBRE = true;
                        break;
                    case 116:
                        BassOD++;
                        break;
                    case 103:
                        BassSolo++;
                        BassSoloNote = note;
                        BassNPS = 0;
                        if (BassSoloNote.NoteLength > BassLength)
                        {
                            BassLength = BassSoloNote.NoteLength;
                            BassSoloLength = NoteLengthInSecs(BassSoloNote.AbsoluteTime, BassSoloNote.NoteLength);
                        }
                        break;
                    case 102:
                    case 90:
                        BassHOPOoff++;
                        break;
                    case 101:
                    case 89:
                        BassHOPOon++;
                        break;
                    case 100:
                        BassXO++;
                        break;
                    case 99:
                        BassXB++;
                        break;
                    case 98:
                        BassXY++;
                        break;
                    case 97:
                        BassXR++;
                        break;
                    case 96:
                        BassXG++;
                        break;
                    case 88:
                        BassHO++;
                        break;
                    case 87:
                        BassHB++;
                        break;
                    case 86:
                        BassHY++;
                        break;
                    case 85:
                        BassHR++;
                        break;
                    case 84:
                        BassHG++;
                        break;
                    case 76:
                        BassMO++;
                        break;
                    case 75:
                        BassMB++;
                        break;
                    case 74:
                        BassMY++;
                        break;
                    case 73:
                        BassMR++;
                        break;
                    case 72:
                        BassMG++;
                        break;
                    case 64:
                        BassEO++;
                        break;
                    case 63:
                        BassEB++;
                        break;
                    case 62:
                        BassEY++;
                        break;
                    case 61:
                        BassER++;
                        break;
                    case 60:
                        BassEG++;
                        break;
                    default:
                        if (note.NoteNumber <= 59 && note.NoteNumber >= 40)
                        {
                            BassLHAnim++;
                        }
                        break;
                }
                if (note.NoteNumber > 100 || note.NoteNumber < 96) continue;
                if (calculateDensity.Checked)
                {
                    var measure = GetMeasure(note.AbsoluteTime);
                    if (!BassDensity.Contains(measure))
                    {
                        BassDensity.Add(measure);
                    }
                }
                if (BassSoloNote == null) continue;
                if (note.AbsoluteTime < BassSoloNote.AbsoluteTime || note.AbsoluteTime > BassSoloNote.AbsoluteTime + BassSoloNote.NoteLength)
                {
                    if (BassNPS / NoteLengthInSecs(BassSoloNote.AbsoluteTime, BassSoloNote.NoteLength) > BassSoloNPS)
                    {
                        BassSoloNPS = Math.Round(BassNPS / NoteLengthInSecs(BassSoloNote.AbsoluteTime, BassSoloNote.NoteLength), 1);
                    }
                }
                else
                {
                    BassNPS++;
                }
            }
            BassX = BassXO + BassXB + BassXY + BassXR + BassXG;
            BassH = BassHO + BassHB + BassHY + BassHR + BassHG;
            BassM = BassMO + BassMB + BassMY + BassMR + BassMG;
            BassE = BassEO + BassEB + BassEY + BassER + BassEG;
            TotalPlayableNotes += BassX + BassH + BassM + BassE;
        }
          
        private void AnalyzeGuitar(IEnumerable<MidiEvent> track)
        {
            NoteOnEvent GuitarSoloNote = null;
            var GuitarNPS = 0;
            var GuitarLength = 0;
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                var note = (NoteOnEvent)notes;
                if (note.Velocity <= 0) continue;

                TotalMIDINotes++;
                switch (note.NoteNumber)
                {
                    case 127:
                        GuitarTrills++;
                        break;
                    case 126:
                        GuitarTremolos++;
                        break;
                    case 124:
                    case 123:
                    case 122:
                    case 121:
                    case 120:
                        HasBRE = true;
                        break;
                    case 116:
                        GuitarOD++;
                        break;
                    case 103:
                        GuitarSolo++;
                        GuitarSoloNote = note;
                        GuitarNPS = 0;
                        if (GuitarSoloNote.NoteLength > GuitarLength)
                        {
                            GuitarLength = GuitarSoloNote.NoteLength;
                            GuitarSoloLength = NoteLengthInSecs(GuitarSoloNote.AbsoluteTime, GuitarSoloNote.NoteLength);
                        }
                        break;
                    case 102:
                    case 90:
                        GuitarHOPOoff++;
                        break;
                    case 101:
                    case 89:
                        GuitarHOPOon++;
                        break;
                    case 100:
                        GuitarXO++;
                        break;
                    case 99:
                        GuitarXB++;
                        break;
                    case 98:
                        GuitarXY++;
                        break;
                    case 97:
                        GuitarXR++;
                        break;
                    case 96:
                        GuitarXG++;
                        break;
                    case 88:
                        GuitarHO++;
                        break;
                    case 87:
                        GuitarHB++;
                        break;
                    case 86:
                        GuitarHY++;
                        break;
                    case 85:
                        GuitarHR++;
                        break;
                    case 84:
                        GuitarHG++;
                        break;
                    case 76:
                        GuitarMO++;
                        break;
                    case 75:
                        GuitarMB++;
                        break;
                    case 74:
                        GuitarMY++;
                        break;
                    case 73:
                        GuitarMR++;
                        break;
                    case 72:
                        GuitarMG++;
                        break;
                    case 64:
                        GuitarEO++;
                        break;
                    case 63:
                        GuitarEB++;
                        break;
                    case 62:
                        GuitarEY++;
                        break;
                    case 61:
                        GuitarER++;
                        break;
                    case 60:
                        GuitarEG++;
                        break;
                    default:
                        if (note.NoteNumber <= 59 && note.NoteNumber >= 40)
                        {
                            GuitarLHAnim++;
                        }
                        break;
                }
                if (note.NoteNumber > 100 || note.NoteNumber < 96) continue;
                if (calculateDensity.Checked)
                {
                    var measure = GetMeasure(note.AbsoluteTime);
                    if (!GuitarDensity.Contains(measure))
                    {
                        GuitarDensity.Add(measure);
                    }
                }
                if (GuitarSoloNote == null) continue;
                if (note.AbsoluteTime < GuitarSoloNote.AbsoluteTime || note.AbsoluteTime > GuitarSoloNote.AbsoluteTime + GuitarSoloNote.NoteLength)
                {
                    if (GuitarNPS / NoteLengthInSecs(GuitarSoloNote.AbsoluteTime, GuitarSoloNote.NoteLength) > GuitarSoloNPS)
                    {
                        GuitarSoloNPS = Math.Round(GuitarNPS / NoteLengthInSecs(GuitarSoloNote.AbsoluteTime, GuitarSoloNote.NoteLength), 1);
                    }
                }
                else
                {
                    GuitarNPS++;
                }
            }
            GuitarX = GuitarXO + GuitarXB + GuitarXY + GuitarXR + GuitarXG;
            GuitarH = GuitarHO + GuitarHB + GuitarHY + GuitarHR + GuitarHG;
            GuitarM = GuitarMO + GuitarMB + GuitarMY + GuitarMR + GuitarMG;
            GuitarE = GuitarEO + GuitarEB + GuitarEY + GuitarER + GuitarEG;
            TotalPlayableNotes += GuitarX + GuitarH + GuitarM + GuitarE;
        }

        private void AnalyzeKeys(IEnumerable<MidiEvent> track)
        {
            NoteOnEvent KeysSoloNote = null;
            var KeysNPS = 0;
            var KeysLength = 0;

            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                var note = (NoteOnEvent)notes;
                if (note.Velocity <= 0) continue;

                TotalMIDINotes++;
                switch (note.NoteNumber)
                {
                    case 127:
                        KeysTrills++;
                        break;
                    case 124:
                    case 123:
                    case 122:
                    case 121:
                    case 120:
                        HasBRE = true;
                        break;
                    case 116:
                        KeysOD++;
                        break;
                    case 103:
                        KeysSolo++;
                        KeysSoloNote = note;
                        KeysNPS = 0;
                        if (KeysSoloNote.NoteLength > KeysLength)
                        {
                            KeysLength = KeysSoloNote.NoteLength;
                            KeysSoloLength = NoteLengthInSecs(KeysSoloNote.AbsoluteTime, KeysSoloNote.NoteLength);
                        }
                        break;
                    case 100:
                        KeysXO++;
                        break;
                    case 99:
                        KeysXB++;
                        break;
                    case 98:
                        KeysXY++;
                        break;
                    case 97:
                        KeysXR++;
                        break;
                    case 96:
                        KeysXG++;
                        break;
                    case 88:
                        KeysHO++;
                        break;
                    case 87:
                        KeysHB++;
                        break;
                    case 86:
                        KeysHY++;
                        break;
                    case 85:
                        KeysHR++;
                        break;
                    case 84:
                        KeysHG++;
                        break;
                    case 76:
                        KeysMO++;
                        break;
                    case 75:
                        KeysMB++;
                        break;
                    case 74:
                        KeysMY++;
                        break;
                    case 73:
                        KeysMR++;
                        break;
                    case 72:
                        KeysMG++;
                        break;
                    case 64:
                        KeysEO++;
                        break;
                    case 63:
                        KeysEB++;
                        break;
                    case 62:
                        KeysEY++;
                        break;
                    case 61:
                        KeysER++;
                        break;
                    case 60:
                        KeysEG++;
                        break;
                }
                if (note.NoteNumber > 100 || note.NoteNumber < 96) continue;
                if (calculateDensity.Checked)
                {
                    var measure = GetMeasure(note.AbsoluteTime);
                    if (!KeysDensity.Contains(measure))
                    {
                        KeysDensity.Add(measure);
                    }
                }
                if (KeysSoloNote == null) continue;
                if (note.AbsoluteTime < KeysSoloNote.AbsoluteTime || note.AbsoluteTime > KeysSoloNote.AbsoluteTime + KeysSoloNote.NoteLength)
                {
                    if (KeysNPS / NoteLengthInSecs(KeysSoloNote.AbsoluteTime, KeysSoloNote.NoteLength) > KeysSoloNPS)
                    {
                        KeysSoloNPS = Math.Round(KeysNPS / NoteLengthInSecs(KeysSoloNote.AbsoluteTime, KeysSoloNote.NoteLength), 1);
                    }
                }
                else
                {
                    KeysNPS++;
                }
            }
            KeysX = KeysXO + KeysXB + KeysXY + KeysXR + KeysXG;
            KeysH = KeysHO + KeysHB + KeysHY + KeysHR + KeysHG;
            KeysM = KeysMO + KeysMB + KeysMY + KeysMR + KeysMG;
            KeysE = KeysEO + KeysEB + KeysEY + KeysER + KeysEG;
            TotalPlayableNotes += KeysX + KeysH + KeysM + KeysE;
        }

        private static string GetCleanMIDILyric(string raw_event)
        {
            try
            {
                raw_event = raw_event.Trim();
                int index;
                if (raw_event.Contains("Lyric"))
                {
                    index = raw_event.IndexOf("Lyric", StringComparison.Ordinal) + 5;
                }
                else if (raw_event.Contains("TextEvent"))
                {
                    index = raw_event.IndexOf("TextEvent", StringComparison.Ordinal) + 9;
                }
                else
                {
                    index = raw_event.LastIndexOf(" ", StringComparison.Ordinal);
                }
                if (index == -1) return "";

                var old_lyric = raw_event.Substring(index + 1, raw_event.Length - index - 1);
                return old_lyric;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void AnalyzeVocals(IEnumerable<MidiEvent> track)
        {
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        var vocal_event = (MetaEvent)notes;
                        if (vocal_event.ToString().Contains("[clap"))
                        {
                            PercType = "Hand Clap";
                        }
                        else if (vocal_event.ToString().Contains("[cowbell"))
                        {
                            PercType = "Cowbell";
                        }
                        else if (vocal_event.ToString().Contains("[tambourine"))
                        {
                            PercType = "Tambourine";
                        }
                        else if (vocal_event.ToString().Contains("#") || vocal_event.ToString().Contains("^"))
                        {
                            VocalsTalkies++;
                        }
                        if ((vocal_event.MetaEventType == NAudio.Midi.MetaEventType.Lyric || vocal_event.MetaEventType == NAudio.Midi.MetaEventType.TextEvent) && 
                            !vocal_event.ToString().Contains("["))
                        {
                            var lyric = GetCleanMIDILyric(vocal_event.ToString());
                            if (string.IsNullOrWhiteSpace(lyric)) continue;
                            var time = GetRealtime(vocal_event.AbsoluteTime);
                            var index = 0;
                            for (var i = 0; i < LeadVocals.Count; i++)
                            {
                                if (LeadVocals[i].PhraseStart > time) break;
                                index = i;
                            }
                            LeadVocals[index].PhraseText = LeadVocals[index].PhraseText + " " + lyric;
                        }
                        break;
                    
                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        TotalMIDINotes++;
                        switch (note.NoteNumber)
                        {
                            case 116:
                                VocalsOD++;
                                break;
                            case 105:
                                VocalsPhrases++;
                                break;
                            case 97:
                                VocalsHidPerc++;
                                break;
                            case 96:
                                VocalsDispPerc++;
                                break;
                            case 1:
                                VocalsLyricShift++;
                                break;
                            case 0:
                                VocalsRangeShift++;
                                break;
                            default:
                                if (note.NoteNumber <= 84 && note.NoteNumber >= 36)
                                {
                                    Vocals++;
                                    TotalPlayableNotes++;
                                    if (calculateDensity.Checked)
                                    {
                                        var measure = GetMeasure(note.AbsoluteTime);
                                        if (!VocalsDensity.Contains(measure))
                                        {
                                            VocalsDensity.Add(measure);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void GetPhraseMarkers(IEnumerable<MidiEvent> track, out List<LyricPhrase> LyricPhrases)
        {
            var Lyrics = new List<LyricPhrase>();
            foreach (var notes in track)
            {
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;
                        switch (note.NoteNumber)
                        {
                            case 106: //older style part vocals had both 106 and 105
                            case 105: //typical phrase marker
                                var time = GetRealtime(note.AbsoluteTime);
                                var end = GetRealtime(note.AbsoluteTime + note.NoteLength);
                                if (Lyrics.Any())
                                {
                                    var index = Lyrics.Count - 1;
                                    if (Lyrics[index].PhraseStart == time)
                                    {
                                        continue; //for old double-stacked markers
                                    }
                                }
                                var phrase = new LyricPhrase
                                {
                                    PhraseStart = time,
                                    PhraseEnd = end
                                };
                                Lyrics.Add(phrase); //new line
                                break;
                        }
                        break;
                }
            }
            LyricPhrases = Lyrics;
        }

        private void AnalyzeHarm1(IEnumerable<MidiEvent> track)
        {
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        var vocal_event = (MetaEvent)notes;
                        if (vocal_event.ToString().Contains("#") || vocal_event.ToString().Contains("^"))
                        {
                            Harm1Talkies++;
                        }
                        if ((vocal_event.MetaEventType == NAudio.Midi.MetaEventType.Lyric || vocal_event.MetaEventType == NAudio.Midi.MetaEventType.TextEvent) &&
                            !vocal_event.ToString().Contains("["))
                        {
                            var lyric = GetCleanMIDILyric(vocal_event.ToString());
                            if (string.IsNullOrWhiteSpace(lyric)) continue;
                            var time = GetRealtime(vocal_event.AbsoluteTime);
                            var index = 0;
                            for (var i = 0; i < Harmonies1.Count; i++)
                            {
                                if (Harmonies1[i].PhraseStart > time) break;
                                index = i;
                            }
                            Harmonies1[index].PhraseText = Harmonies1[index].PhraseText + " " + lyric;
                        }
                        break;

                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        TotalMIDINotes++;
                        switch (note.NoteNumber)
                        {
                            case 116:
                                Harm1OD++;
                                break;
                            case 105:
                                Harm1Phrases++;
                                break;
                            case 1:
                                Harm1LyricShift++;
                                break;
                            case 0:
                                Harm1RangeShift++;
                                break;
                            default:
                                if (note.NoteNumber <= 84 && note.NoteNumber >= 36)
                                {
                                    Harm1++;
                                    TotalPlayableNotes++;
                                    if (calculateDensity.Checked)
                                    {
                                        var measure = GetMeasure(note.AbsoluteTime);
                                        if (!Harm1Density.Contains(measure))
                                        {
                                            Harm1Density.Add(measure);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void AnalyzeHarm2(IEnumerable<MidiEvent> track)
        {
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        var vocal_event = (MetaEvent)notes;
                        if (vocal_event.ToString().Contains("#") || vocal_event.ToString().Contains("^"))
                        {
                            Harm2Talkies++;
                        }
                        if ((vocal_event.MetaEventType == NAudio.Midi.MetaEventType.Lyric || vocal_event.MetaEventType == NAudio.Midi.MetaEventType.TextEvent) &&
                            !vocal_event.ToString().Contains("["))
                        {
                            var lyric = GetCleanMIDILyric(vocal_event.ToString());
                            if (string.IsNullOrWhiteSpace(lyric)) continue;
                            var time = GetRealtime(vocal_event.AbsoluteTime);
                            var index = 0;
                            for (var i = 0; i < Harmonies2.Count; i++)
                            {
                                if (Harmonies2[i].PhraseStart > time) break;
                                index = i;
                            }
                            Harmonies2[index].PhraseText = Harmonies2[index].PhraseText + " " + lyric;
                        }
                        break;

                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        TotalMIDINotes++;
                        switch (note.NoteNumber)
                        {
                            case 116:
                                Harm2OD++;
                                break;
                            case 105:
                                Harm2Phrases++;
                                break;
                            case 1:
                                Harm2LyricShift++;
                                break;
                            case 0:
                                Harm2RangeShift++;
                                break;
                            default:
                                if (note.NoteNumber <= 84 && note.NoteNumber >= 36)
                                {
                                    Harm2++;
                                    TotalPlayableNotes++;
                                    if (calculateDensity.Checked)
                                    {
                                        var measure = GetMeasure(note.AbsoluteTime);
                                        if (!Harm2Density.Contains(measure))
                                        {
                                            Harm2Density.Add(measure);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void AnalyzeHarm3(IEnumerable<MidiEvent> track)
        {
            foreach (var notes in track)
            {
                if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                {
                    LengthLong = notes.AbsoluteTime;
                }
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        var vocal_event = (MetaEvent)notes;
                        if (vocal_event.ToString().Contains("#") || vocal_event.ToString().Contains("^"))
                        {
                            Harm3Talkies++;
                        }
                        if ((vocal_event.MetaEventType == NAudio.Midi.MetaEventType.Lyric || vocal_event.MetaEventType == NAudio.Midi.MetaEventType.TextEvent) &&
                            !vocal_event.ToString().Contains("["))
                        {
                            var lyric = GetCleanMIDILyric(vocal_event.ToString());
                            if (string.IsNullOrWhiteSpace(lyric)) continue;
                            var time = GetRealtime(vocal_event.AbsoluteTime);
                            var index = 0;
                            for (var i = 0; i < Harmonies3.Count; i++)
                            {
                                if (Harmonies3[i].PhraseStart > time) break;
                                index = i;
                            }
                            Harmonies3[index].PhraseText = Harmonies3[index].PhraseText + " " + lyric;
                        }
                        break;

                    case MidiCommandCode.NoteOn:
                        var note = (NoteOnEvent)notes;
                        if (note.Velocity <= 0) continue;

                        TotalMIDINotes++;
                        switch (note.NoteNumber)
                        {
                            case 116:
                                Harm3OD++;
                                break;
                            case 105:
                                Harm3Phrases++;
                                break;
                            case 1:
                                Harm3LyricShift++;
                                break;
                            case 0:
                                Harm3RangeShift++;
                                break;
                            default:
                                if (note.NoteNumber <= 84 && note.NoteNumber >= 36)
                                {
                                    Harm3++;
                                    TotalPlayableNotes++;
                                    if (calculateDensity.Checked)
                                    {
                                        var measure = GetMeasure(note.AbsoluteTime);
                                        if (!Harm3Density.Contains(measure))
                                        {
                                            Harm3Density.Add(measure);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void AnalyzeMIDI(string midi, bool DoNotDisplay = false, bool clearItems = false)
        {
            ResetAll(!clearItems);
            HaveFile = true;
            lstStats.Invoke(new MethodInvoker(() => lstStats.BackgroundImage = null));
            if (clearItems)
            {
                lstStats.Invoke(new MethodInvoker(() => lstStats.Items.Clear()));
            }
            LeadVocals.Clear();
            Harmonies1.Clear();
            Harmonies2.Clear();
            Harmonies3.Clear();
            MoggFiles.Clear();

            Log("");
            Log("Beginning song analysis...");
            Log("");
            var type = "Unknown";
            if (!string.IsNullOrWhiteSpace(midi) && VariousFunctions.ReadFileType(InputFile) == XboxFileType.STFS)
            {
                type = "CON";
                try
                {
                    var xPackage = new STFSPackage(InputFile);
                    if (xPackage.ParseSuccess)
                    {
                        type = xPackage.Header.ThisType == PackageType.SavedGame ? "CON" : "LIVE";
                    }
                    xPackage.CloseIO();                    
                }
                catch (Exception)
                {}
            }
            else if (Path.GetExtension(InputFile).ToLowerInvariant() == ".pkg")
            {
                type = "PKG";
            }
            
            Log(type + " File Name", Path.GetFileName(InputFile));
            var fileSize = new FileInfo(InputFile);
            var size = fileSize.Length;
            var mb = Math.Round((double)size / 1048576, 2);
            Log(type + " File Size", size.ToString(CultureInfo.InvariantCulture) + " bytes" + " (" + mb + " MB)");
            var name = Parser.Songs != null ? Parser.Songs[0].InternalName + ".mid" + (type == "PKG" ? ".edat" : "") : Path.GetFileName(midi);
            
            Log("MIDI File Name", name);
            try
            {
                MIDIFile = Tools.NemoLoadMIDI(midi);
                if (MIDIFile == null)
                {
                    Log("Unable to load MIDI file '" + Path.GetFileName(midi) + "' to analyze it");
                    return;
                }
                
                fileSize = new FileInfo(midi);
                MIDISize = fileSize.Length;
                TicksPerQuarter = MIDIFile.DeltaTicksPerQuarterNote;
                BuildTempoList();
                BuildTimeSignatureList();
               
                for (var i = 0; i < MIDIFile.Events.Tracks; i++)
                {
                    var trackname = MIDIFile.Events[i][0].ToString();
                    if (trackname.Contains("PART DRUMS") && chkDrums.Checked)
                    {
                        AnalyzeDrums(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("PART BASS") && chkBass.Checked)
                    {
                        AnalyzeBass(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("PART REAL_BASS") && !trackname.Contains("22") && chkProBass.Checked)
                    {
                        foreach (var notes in MIDIFile.Events[i])
                        {
                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            switch (note.NoteName)
                            {
                                case "C8":
                                case "C#8":
                                case "D8":
                                case "D#8":
                                case "E8":
                                case "F8":
                                    ProBass17X++; //expert notes
                                    break;
                                case "C6":
                                case "C#6":
                                case "D6":
                                case "D#6":
                                case "E6":
                                case "F6":
                                    ProBass17H++; //hard notes
                                    break;
                                case "C4":
                                case "C#4":
                                case "D4":
                                case "D#4":
                                case "E4":
                                case "F4":
                                    ProBass17M++; //medium notes
                                    break;
                                case "C2":
                                case "C#2":
                                case "D2":
                                case "D#2":
                                case "E2":
                                case "F2":
                                    ProBass17E++; //easy notes
                                    break;
                                default:
                                    ProBass17Misc++; //misc notes
                                    break;
                            }
                            TotalMIDINotes++;
                        }
                        TotalPlayableNotes += ProBass17X + ProBass17H + ProBass17M + ProBass17E;
                    }
                    else if (trackname.Contains("PART REAL_BASS_22") && chkProBass.Checked)
                    {
                        foreach (var notes in MIDIFile.Events[i])
                        {
                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            switch (note.NoteName)
                            {
                                case "C8":
                                case "C#8":
                                case "D8":
                                case "D#8":
                                case "E8":
                                case "F8":
                                    ProBass22X++; //expert notes
                                    break;
                                case "C6":
                                case "C#6":
                                case "D6":
                                case "D#6":
                                case "E6":
                                case "F6":
                                    ProBass22H++; //hard notes
                                    break;
                                case "C4":
                                case "C#4":
                                case "D4":
                                case "D#4":
                                case "E4":
                                case "F4":
                                    ProBass22M++; //medium notes
                                    break;
                                case "C2":
                                case "C#2":
                                case "D2":
                                case "D#2":
                                case "E2":
                                case "F2":
                                    ProBass22E++; //easy notes
                                    break;
                                default:
                                    ProBass22Misc++; //misc notes
                                    break;
                            }
                            TotalMIDINotes++;
                        }
                        TotalPlayableNotes += ProBass22X + ProBass22H + ProBass22M + ProBass22E;
                    }
                    else if (trackname.Contains("PART REAL_GUITAR") && !trackname.Contains("22") && chkProGuitar.Checked)
                    {
                        foreach (var notes in MIDIFile.Events[i])
                        {
                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            switch (note.NoteName)
                            {
                                case "C8":
                                case "C#8":
                                case "D8":
                                case "D#8":
                                case "E8":
                                case "F8":
                                    ProGuitar17X++; //expert notes
                                    break;
                                case "C6":
                                case "C#6":
                                case "D6":
                                case "D#6":
                                case "E6":
                                case "F6":
                                    ProGuitar17H++; //hard notes
                                    break;
                                case "C4":
                                case "C#4":
                                case "D4":
                                case "D#4":
                                case "E4":
                                case "F4":
                                    ProGuitar17M++; //medium notes
                                    break;
                                case "C2":
                                case "C#2":
                                case "D2":
                                case "D#2":
                                case "E2":
                                case "F2":
                                    ProGuitar17E++; //easy notes
                                    break;
                                default:
                                    ProGuitar17Misc++; //misc notes
                                    break;
                            }
                            TotalMIDINotes++;
                        }
                        TotalPlayableNotes += ProGuitar17X + ProGuitar17H + ProGuitar17M + ProGuitar17E;
                    }
                    else if (trackname.Contains("PART REAL_GUITAR_22") && chkProGuitar.Checked)
                    {
                        foreach (var notes in MIDIFile.Events[i])
                        {
                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            switch (note.NoteName)
                            {
                                case "C8":
                                case "C#8":
                                case "D8":
                                case "D#8":
                                case "E8":
                                case "F8":
                                    ProGuitar22X++; //expert notes
                                    break;
                                case "C6":
                                case "C#6":
                                case "D6":
                                case "D#6":
                                case "E6":
                                case "F6":
                                    ProGuitar22H++; //hard notes
                                    break;
                                case "C4":
                                case "C#4":
                                case "D4":
                                case "D#4":
                                case "E4":
                                case "F4":
                                    ProGuitar22M++; //medium notes
                                    break;
                                case "C2":
                                case "C#2":
                                case "D2":
                                case "D#2":
                                case "E2":
                                case "F2":
                                    ProGuitar22E++; //easy notes
                                    break;
                                default:
                                    ProGuitar22Misc++; //misc notes
                                    break;
                            }
                            TotalMIDINotes++;
                        }
                        TotalPlayableNotes += ProGuitar22X + ProGuitar22H + ProGuitar22M + ProGuitar22E;
                    }
                    else if (trackname.Contains("PART GUITAR") && chkGuitar.Checked)
                    {
                        AnalyzeGuitar(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("PART KEYS") && !trackname.Contains("ANIM") && chkKeys.Checked)
                    {
                        AnalyzeKeys(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("KEYS_X") && chkProKeys.Checked)
                    {
                        NoteOnEvent ProKeysSoloNote = null;
                        var ProKeysNPS = 0;
                        var ProKeysLength = 0;

                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            switch (note.NoteNumber)
                            {
                                case 127:
                                    ProKeysXTrills++;
                                    break;
                                case 126:
                                    ProKeysXGliss++;
                                    break;
                                case 116:
                                    ProKeysXOD++;
                                    break;
                                case 115:
                                    ProKeysXSolo++;
                                    ProKeysSoloNote = note;
                                    ProKeysNPS = 0;
                                    if (ProKeysSoloNote.NoteLength > ProKeysLength)
                                    {
                                        ProKeysLength = ProKeysSoloNote.NoteLength;
                                        ProKeysSoloLength = NoteLengthInSecs(ProKeysSoloNote.AbsoluteTime, ProKeysSoloNote.NoteLength);
                                    }
                                    break;
                                default:
                                    if (note.NoteNumber <= 72 && note.NoteNumber >= 48)
                                    {
                                        ProKeysX++;
                                        TotalPlayableNotes++;
                                        if (note.NoteName.Contains("#"))
                                        {
                                            ProKeysXBlack++;
                                        }
                                        if (calculateDensity.Checked)
                                        {
                                            var measure = GetMeasure(note.AbsoluteTime);
                                            if (!ProKeysDensity.Contains(measure))
                                            {
                                                ProKeysDensity.Add(measure);
                                            }
                                        }
                                        if (ProKeysSoloNote == null) continue;
                                        if (note.AbsoluteTime < ProKeysSoloNote.AbsoluteTime || note.AbsoluteTime > ProKeysSoloNote.AbsoluteTime + ProKeysSoloNote.NoteLength)
                                        {
                                            if (ProKeysNPS / NoteLengthInSecs(ProKeysSoloNote.AbsoluteTime, ProKeysSoloNote.NoteLength) > ProKeysSoloNPS)
                                            {
                                                ProKeysSoloNPS = Math.Round(ProKeysNPS / NoteLengthInSecs(ProKeysSoloNote.AbsoluteTime, ProKeysSoloNote.NoteLength), 1);
                                            }
                                        }
                                        else
                                        {
                                            ProKeysNPS++;
                                        }
                                    }
                                    else if (note.NoteNumber <= 9 && note.NoteNumber >= 0)
                                    {
                                        ProKeysXRanges++;
                                    }
                                    break;
                            }
                        }
                    }
                    else if (trackname.Contains("KEYS_H") && chkProKeys.Checked)
                    {
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            if (note.NoteNumber > 72 || note.NoteNumber < 48) continue;
                            ProKeysH++;
                            TotalPlayableNotes++;
                            if (note.NoteName.Contains("#"))
                            {
                                ProKeysHBlack++;
                            }
                        }
                    }
                    else if (trackname.Contains("KEYS_M") && chkProKeys.Checked)
                    {
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            if (note.NoteNumber > 72 || note.NoteNumber < 48) continue;
                            ProKeysM++;
                            TotalPlayableNotes++;
                            if (note.NoteName.Contains("#"))
                            {
                                ProKeysMBlack++;
                            }
                        }
                    }
                    else if (trackname.Contains("KEYS_E") && chkProKeys.Checked)
                    {
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            if (note.NoteNumber > 72 || note.NoteNumber < 48) continue;
                            ProKeysE++;
                            TotalPlayableNotes++;
                            if (note.NoteName.Contains("#"))
                            {
                                ProKeysEBlack++;
                            }
                        }
                    }
                    else if (trackname.Contains("KEYS_ANIM_RH") && (chkProKeys.Checked || chkKeys.Checked))
                    {
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            if (note.NoteNumber <= 72 && note.NoteNumber >= 48)
                            {
                                KeysAnimRH++;
                            }
                        }
                    }
                    else if (trackname.Contains("KEYS_ANIM_LH") && (chkProKeys.Checked || chkKeys.Checked))
                    {
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;

                            TotalMIDINotes++;
                            if (note.NoteNumber <= 72 && note.NoteNumber >= 48)
                            {
                                KeysAnimLH++;
                            }
                        }
                    }
                    else if (trackname.Contains("PART VOCALS") && chkVocals.Checked)
                    {
                        GetPhraseMarkers(MIDIFile.Events[i], out LeadVocals);
                        AnalyzeVocals(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("HARM1") && chkHarms.Checked)
                    {
                        GetPhraseMarkers(MIDIFile.Events[i], out Harmonies1);
                        AnalyzeHarm1(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("HARM2") && chkHarms.Checked)
                    {
                        GetPhraseMarkers(MIDIFile.Events[i], out Harmonies2);
                        AnalyzeHarm2(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("HARM3") && chkHarms.Checked)
                    {
                        foreach (var harmony in Harmonies1.Select(harm => new LyricPhrase {PhraseEnd = harm.PhraseEnd, PhraseStart = harm.PhraseStart, PhraseText = ""}))
                        {
                            Harmonies3.Add(harmony);
                        }
                        AnalyzeHarm3(MIDIFile.Events[i]);
                    }
                    else if (trackname.Contains("BEAT") && chkMisc.Checked)
                    {
                        HasBeat = true;
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            TotalMIDINotes++;
                        }
                    }
                    else if (trackname.Contains("VENUE") && chkMisc.Checked)
                    {
                        HasVenue = true;
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            if (!HasEndEvent && notes.AbsoluteTime > LengthLong)
                            {
                                LengthLong = notes.AbsoluteTime;
                            }
                            if (notes.CommandCode != MidiCommandCode.NoteOn) continue;
                            var note = (NoteOnEvent)notes;
                            if (note.Velocity <= 0) continue;
                            TotalMIDINotes++;
                        }
                    }
                    else if (trackname.Contains("EVENTS") && chkMisc.Checked)
                    {
                        HasEvents = true;
                        for (var z = 0; z < MIDIFile.Events[i].Count; z++)
                        {
                            var notes = MIDIFile.Events[i][z];

                            switch (notes.CommandCode)
                            {
                                case MidiCommandCode.MetaEvent:
                                    var events = (MetaEvent) notes;
                                    if (!events.ToString().Contains("[end]")) continue;
                                    HasEndEvent = true;
                                    LengthLong = notes.AbsoluteTime;
                                    break;

                                case MidiCommandCode.NoteOn:
                                    var note = (NoteOnEvent)notes;
                                    if (note.Velocity <= 0) continue;
                                    TotalMIDINotes++;
                                    break;
                            }
                        }
                    }
                }
                WriteAnalysis(DoNotDisplay);
            }
            catch (Exception ex)
            {
                Log("Error analyzing MIDI file " + Path.GetFileName(midi));
                Log("The error says: " + ex.Message);
            }
        }

        private double AverageBPM()
        {
            var total_bpm = 0.0;
            var last = 0.0;
            var bpm = 120.0;
            double difference;
            var LengthSecs = GetRealtime(LengthLong);
            if (LengthSecs <= 0.0)
            {
                var count = TempoEvents.Sum(tempo => tempo.BPM);
                return Math.Round(count / TempoEvents.Count, 2);
            }
            foreach (var tempo in TempoEvents)
            {
                var current = GetRealtime(tempo.AbsoluteTime);
                difference = current - last;
                last = GetRealtime(tempo.AbsoluteTime);
                if (difference <= 0.0)
                {
                    bpm = tempo.BPM;
                    continue;
                }
                total_bpm += bpm * (difference / LengthSecs);
                bpm = tempo.BPM;
            }
            difference = LengthSecs - last;
            total_bpm += bpm * (difference / LengthSecs);
            if (total_bpm == 0)
            {
                total_bpm = bpm;
            }
            return Math.Round(total_bpm, 2);
        }

        private void WriteAnalysis(bool DoNotDisplay = false)
        {
            LengthSeconds = GetRealtime(LengthLong);
            var minutes = Parser.GetSongDuration((LengthSeconds*1000).ToString(CultureInfo.InvariantCulture));
            var kb = Math.Round((double) MIDISize/1024, 2);
            var measures = GetMeasure(LengthLong);

            if (DoNotDisplay) return;

            if (chkMisc.Checked)
            {
                Log("MIDI File Size", MIDISize.ToString(CultureInfo.InvariantCulture) + " bytes" + " (" + kb + " KB)");
                Log("Length", LengthSeconds + " seconds (" + minutes + ")");
                Log("Tracks", (MIDIFile.Events.Tracks - 1).ToString(CultureInfo.InvariantCulture));
                Log("Authored Notes", TotalMIDINotes.ToString(CultureInfo.InvariantCulture));
                Log("Playable Notes", TotalPlayableNotes.ToString(CultureInfo.InvariantCulture));
                Log("Tempo Markers", TempoEvents.Count.ToString(CultureInfo.InvariantCulture));
                Log("Time Signature Markers", TimeSignatures.Count.ToString(CultureInfo.InvariantCulture));
                Log("Total # of Measures", measures.ToString(CultureInfo.InvariantCulture));
                Log("Average BPM", AverageBPM().ToString(CultureInfo.InvariantCulture));
                Log("Has Big Rock Ending?", HasBRE ? "Yes" : "No");
                Log("Has Events Track?", HasEvents ? "Yes" : "No");
                Log("Has [end] Event?", HasEndEvent ? "Yes" : "No");
                Log("Has Venue Track?", HasVenue ? "Yes" : "No");
                Log("Has BEAT Track?", HasBeat ? "Yes" : "No");
            }

            if (chkDrums.Checked)
            {
                Log("");
                Log("Has Drums?", (DrumsX > 0) ? "Yes" : "No");
                if (DrumsX > 0)
                {
                    Log(" - Expert Notes", NoteCount(DrumsX));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Green Cymbals", NoteCount(DrumsXGreenCymbal));
                        Log("    - Blue Cymbals", NoteCount(DrumsXBlueCymbal));
                        Log("    - Yellow Cymbals", NoteCount(DrumsXYellowCymbal));
                        Log("    - Green Toms", NoteCount(DrumsXGreenTom));
                        Log("    - Blue Toms", NoteCount(DrumsXBlueTom));
                        Log("    - Yellow Toms", NoteCount(DrumsXYellowTom));
                        Log("    - Snares", NoteCount(DrumsXSnare));
                        Log("    - Kicks", NoteCount(DrumsXKick));
                        Log("    - Expert+ 2x Kicks", NoteCount(DrumsXPlus));
                    }
                    Log(" - Hard Notes", NoteCount(DrumsH));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Green Cymbals", NoteCount(DrumsHGreenCymbal));
                        Log("    - Blue Cymbals", NoteCount(DrumsHBlueCymbal));
                        Log("    - Yellow Cymbals", NoteCount(DrumsHYellowCymbal));
                        Log("    - Green Toms", NoteCount(DrumsHGreenTom));
                        Log("    - Blue Toms", NoteCount(DrumsHBlueTom));
                        Log("    - Yellow Toms", NoteCount(DrumsHYellowTom));
                        Log("    - Snares", NoteCount(DrumsHSnare));
                        Log("    - Kicks", NoteCount(DrumsHKick));
                    }
                    Log(" - Medium Notes", NoteCount(DrumsM));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Green Cymbals", NoteCount(DrumsMGreenCymbal));
                        Log("    - Blue Cymbals", NoteCount(DrumsMBlueCymbal));
                        Log("    - Yellow Cymbals", NoteCount(DrumsMYellowCymbal));
                        Log("    - Green Toms", NoteCount(DrumsMGreenTom));
                        Log("    - Blue Toms", NoteCount(DrumsMBlueTom));
                        Log("    - Yellow Toms", NoteCount(DrumsMYellowTom));
                        Log("    - Snares", NoteCount(DrumsMSnare));
                        Log("    - Kicks", NoteCount(DrumsMKick));
                    }
                    Log(" - Easy Notes", NoteCount(DrumsE));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Green Cymbals", NoteCount(DrumsEGreenCymbal));
                        Log("    - Blue Cymbals", NoteCount(DrumsEBlueCymbal));
                        Log("    - Yellow Cymbals", NoteCount(DrumsEYellowCymbal));
                        Log("    - Green Toms", NoteCount(DrumsEGreenTom));
                        Log("    - Blue Toms", NoteCount(DrumsEBlueTom));
                        Log("    - Yellow Toms", NoteCount(DrumsEYellowTom));
                        Log("    - Snares", NoteCount(DrumsESnare));
                        Log("    - Kicks", NoteCount(DrumsEKick));
                    }
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",
                            Math.Round((double) DrumsDensity.Count/measures, 3)*100 + "% (" + DrumsDensity.Count +
                            " of " + measures + ")");
                    }
                    Log(" - Overdrive", DrumsOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Fills", DrumsFills.ToString(CultureInfo.InvariantCulture));
                    Log(" - Tom Markers", DrumsToms.ToString(CultureInfo.InvariantCulture));
                    Log(" - Swells", DrumsSwells.ToString(CultureInfo.InvariantCulture));
                    Log(" - Rolls", DrumsRolls.ToString(CultureInfo.InvariantCulture));
                    Log(" - Solos", DrumsSolo.ToString(CultureInfo.InvariantCulture));
                    if (DrumsSolo > 0)
                    {
                        Log(" - Longest Solo", DrumsSoloLength + " seconds");
                        Log(" - Toughest Solo", DrumsSoloNPS + " NPS");
                    }
                    Log(" - Uses Disco Flip?", HasDiscoFlip ? "Yes" : "No");
                    Log("Has Drum Animations?", (DrumsAnim > 0) ? "Yes" : "No");
                    if (DrumsAnim > 0)
                    {
                        Log(" - Animation Notes", DrumsAnim.ToString(CultureInfo.InvariantCulture));
                    }
                    Log("Has Pro Drum Animations?", ProDrumsAnim > 0 ? "Yes" : "No");
                    if (ProDrumsAnim > 0)
                    {
                        Log(" - Animation Notes", ProDrumsAnim.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            if (chkBass.Checked)
            {
                Log("");
                Log("Has Bass?", (BassX > 0) ? "Yes" : "No");
                if (BassX > 0)
                {
                    Log(" - Expert Notes", NoteCount(BassX));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(BassXO));
                        Log("    - Blues", NoteCount(BassXB));
                        Log("    - Yellows", NoteCount(BassXY));
                        Log("    - Reds", NoteCount(BassXR));
                        Log("    - Greens", NoteCount(BassXG));
                    }
                    Log(" - Hard Notes", NoteCount(BassH));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(BassHO));
                        Log("    - Blues", NoteCount(BassHB));
                        Log("    - Yellows", NoteCount(BassHY));
                        Log("    - Reds", NoteCount(BassHR));
                        Log("    - Greens", NoteCount(BassHG));
                    }
                    Log(" - Medium Notes", NoteCount(BassM));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(BassMO));
                        Log("    - Blues", NoteCount(BassMB));
                        Log("    - Yellows", NoteCount(BassMY));
                        Log("    - Reds", NoteCount(BassMR));
                        Log("    - Greens", NoteCount(BassMG));
                    }
                    Log(" - Easy Notes", NoteCount(BassE));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(BassEO));
                        Log("    - Blues", NoteCount(BassEB));
                        Log("    - Yellows", NoteCount(BassEY));
                        Log("    - Reds", NoteCount(BassER));
                        Log("    - Greens", NoteCount(BassEG));
                    }
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",Math.Round((double) BassDensity.Count/measures, 3)*100 + "% (" + BassDensity.Count + " of " +
                            measures + ")");
                    }
                    Log(" - Overdrive Markers", BassOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Trills", BassTrills.ToString(CultureInfo.InvariantCulture));
                    Log(" - Tremolos", BassTremolos.ToString(CultureInfo.InvariantCulture));
                    Log(" - HOPO On", BassHOPOon.ToString(CultureInfo.InvariantCulture));
                    Log(" - HOPO Off", BassHOPOoff.ToString(CultureInfo.InvariantCulture));
                    Log(" - Solos", BassSolo.ToString(CultureInfo.InvariantCulture));
                    if (BassSolo > 0)
                    {
                        Log(" - Longest Solo", BassSoloLength + " seconds");
                        Log(" - Toughest Solo", BassSoloNPS + " NPS");
                    }
                    Log("Has Bass Left Hand Animations?", (BassLHAnim > 0) ? "Yes" : "No");
                    if (BassLHAnim > 0)
                    {
                        Log(" - Left Hand Animation Notes", BassLHAnim.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            if (chkProBass.Checked)
            {
                Log("");
                Log("Has Pro Bass (17 frets)?", (ProBass17X > 0) ? "Yes" : "No");
                if (ProBass17X > 0)
                {
                    Log(" - Expert Notes", NoteCount(ProBass17X));
                    Log(" - Hard Notes", NoteCount(ProBass17H));
                    Log(" - Medium Notes", NoteCount(ProBass17M));
                    Log(" - Easy Notes", NoteCount(ProBass17E));
                    Log(" - Misc. Notes", ProBass17Misc.ToString(CultureInfo.InvariantCulture));
                }                
                Log("");
                Log("Has Pro Bass (22 frets)?", (ProBass22X > 0) ? "Yes" : "No");
                if (ProBass22X > 0)
                {
                    Log(" - Expert Notes", NoteCount(ProBass22X));
                    Log(" - Hard Notes", NoteCount(ProBass22H));
                    Log(" - Medium Notes", NoteCount(ProBass22M));
                    Log(" - Easy Notes", NoteCount(ProBass22E));
                    Log(" - Misc. Notes", ProBass22Misc.ToString(CultureInfo.InvariantCulture));
                }                
            }

            if (chkGuitar.Checked)
            {
                Log("");
                Log("Has Guitar?", (GuitarX > 0) ? "Yes" : "No");
                if (GuitarX > 0)
                {
                    Log(" - Expert Notes", NoteCount(GuitarX));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(GuitarXO));
                        Log("    - Blues", NoteCount(GuitarXB));
                        Log("    - Yellows", NoteCount(GuitarXY));
                        Log("    - Reds", NoteCount(GuitarXR));
                        Log("    - Greens", NoteCount(GuitarXG));
                    }
                    Log(" - Hard Notes", NoteCount(GuitarH));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(GuitarHO));
                        Log("    - Blues", NoteCount(GuitarHB));
                        Log("    - Yellows", NoteCount(GuitarHY));
                        Log("    - Reds", NoteCount(GuitarHR));
                        Log("    - Greens", NoteCount(GuitarHG));
                    }
                    Log(" - Medium Notes", NoteCount(GuitarM));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(GuitarMO));
                        Log("    - Blues", NoteCount(GuitarMB));
                        Log("    - Yellows", NoteCount(GuitarMY));
                        Log("    - Reds", NoteCount(GuitarMR));
                        Log("    - Greens", NoteCount(GuitarMG));
                    }
                    Log(" - Easy Notes", NoteCount(GuitarE));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(GuitarEO));
                        Log("    - Blues", NoteCount(GuitarEB));
                        Log("    - Yellows", NoteCount(GuitarEY));
                        Log("    - Reds", NoteCount(GuitarER));
                        Log("    - Greens", NoteCount(GuitarEG));
                    }
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",
                            Math.Round((double) GuitarDensity.Count/measures, 3)*100 + "% (" + GuitarDensity.Count +
                            " of " + measures + ")");
                    }
                    Log(" - Overdrive Markers", GuitarOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Trills", GuitarTrills.ToString(CultureInfo.InvariantCulture));
                    Log(" - Tremolos", GuitarTremolos.ToString(CultureInfo.InvariantCulture));
                    Log(" - HOPO On", GuitarHOPOon.ToString(CultureInfo.InvariantCulture));
                    Log(" - HOPO Off", GuitarHOPOoff.ToString(CultureInfo.InvariantCulture));
                    Log(" - Solos", GuitarSolo.ToString(CultureInfo.InvariantCulture));
                    if (GuitarSolo > 0)
                    {
                        Log(" - Longest Solo", GuitarSoloLength + " seconds");
                        Log(" - Toughest Solo", GuitarSoloNPS + " NPS");
                    }
                    Log("Has Guitar Left Hand Animations?", (GuitarLHAnim > 0) ? "Yes" : "No");
                    if (GuitarLHAnim > 0)
                    {
                        Log(" - Left Hand Animation Notes", GuitarLHAnim.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            if (chkProGuitar.Checked)
            {
                Log("");
                Log("Has Pro Guitar (17 frets)?", (ProGuitar17X > 0) ? "Yes" : "No");
                if (ProGuitar17X > 0)
                {
                    Log(" - Expert Notes", NoteCount(ProGuitar17X));
                    Log(" - Hard Notes", NoteCount(ProGuitar17H));
                    Log(" - Medium Notes", NoteCount(ProGuitar17M));
                    Log(" - Easy Notes", NoteCount(ProGuitar17E));
                    Log(" - Misc. Notes", ProGuitar17Misc.ToString(CultureInfo.InvariantCulture));
                }
                Log("");
                Log("Has Pro Guitar (22 frets)?", (ProGuitar22X > 0) ? "Yes" : "No");
                if (ProGuitar22X > 0)
                {
                    Log(" - Expert Notes", NoteCount(ProGuitar22X));
                    Log(" - Hard Notes", NoteCount(ProGuitar22H));
                    Log(" - Medium Notes", NoteCount(ProGuitar22M));
                    Log(" - Easy Notes", NoteCount(ProGuitar22E));
                    Log(" - Misc. Notes", ProGuitar22Misc.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (chkVocals.Checked)
            {
                Log("");
                Log("Has Vocals?", (Vocals > 0) ? "Yes" : "No");
                if (Vocals > 0)
                {
                    Log(" - Note Count", Vocals.ToString(CultureInfo.InvariantCulture));
                    Log(" - Pitched Notes (Sung)", (Vocals - VocalsTalkies).ToString(CultureInfo.InvariantCulture));
                    Log(" - Nonpitched Notes (Talkies)", (VocalsTalkies.ToString(CultureInfo.InvariantCulture)));
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",
                            Math.Round((double) VocalsDensity.Count/measures, 3)*100 + "% (" + VocalsDensity.Count +
                            " of " + measures + ")");
                    }
                    Log(" - Phrase Markers", VocalsPhrases.ToString(CultureInfo.InvariantCulture));
                    Log(" - Overdrive Markers", VocalsOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Percussion Type", PercType);
                    Log(" - Playable Percussion Notes", VocalsDispPerc.ToString(CultureInfo.InvariantCulture));
                    Log(" - Animation Percussion Notes", VocalsHidPerc.ToString(CultureInfo.InvariantCulture));
                    Log(" - Range Shifts", VocalsRangeShift.ToString(CultureInfo.InvariantCulture));
                    Log(" - Lyric Shifts", VocalsLyricShift.ToString(CultureInfo.InvariantCulture));
                    
                    if (chkHarms.Checked && chkHarms.Enabled)
                    {
                        Log("");
                        Log("Has Harmonies?", (Harm1 + Harm2 + Harm3 > 0) ? "Yes" : "No");
                        if (Harm1 + Harm2 + Harm3 > 0)
                        {
                            Log("Harmony Parts", Harm3 > 0 ? "3" : "2");
                            Log("");
                            Log("Harmony 1");
                            Log(" - Note Count", Harm1.ToString(CultureInfo.InvariantCulture));
                            Log(" - Pitched Notes (Sung)", (Harm1 - Harm1Talkies).ToString(CultureInfo.InvariantCulture));
                            Log(" - Nonpitched Notes (Talkies)", (Harm1Talkies.ToString(CultureInfo.InvariantCulture)));
                            if (calculateDensity.Checked)
                            {
                                Log(" - Measure / Note Density",
                                    Math.Round((double) Harm1Density.Count/measures, 3)*100 + "% (" + Harm1Density.Count +
                                    " of " + measures + ")");
                            }
                            Log(" - Phrase Markers", Harm1Phrases.ToString(CultureInfo.InvariantCulture));
                            Log(" - Overdrive Markers", Harm1OD.ToString(CultureInfo.InvariantCulture));
                            Log(" - Range Shifts", Harm1RangeShift.ToString(CultureInfo.InvariantCulture));
                            Log(" - Lyric Shifts", Harm1LyricShift.ToString(CultureInfo.InvariantCulture));

                            Log("");
                            Log("Harmony 2");
                            Log(" - Note Count", Harm2.ToString(CultureInfo.InvariantCulture));
                            Log(" - Pitched Notes (Sung)", (Harm2 - Harm2Talkies).ToString(CultureInfo.InvariantCulture));
                            Log(" - Nonpitched Notes (Talkies)", (Harm2Talkies.ToString(CultureInfo.InvariantCulture)));
                            if (calculateDensity.Checked)
                            {
                                Log(" - Measure / Note Density",
                                    Math.Round((double) Harm2Density.Count/measures, 3)*100 + "% (" + Harm2Density.Count +
                                    " of " + measures + ")");
                            }
                            Log(" - Phrase Markers", Harm2Phrases.ToString(CultureInfo.InvariantCulture));
                            Log(" - Overdrive Markers", Harm2OD.ToString(CultureInfo.InvariantCulture));
                            Log(" - Range Shifts", Harm2RangeShift.ToString(CultureInfo.InvariantCulture));
                            Log(" - Lyric Shifts", Harm2LyricShift.ToString(CultureInfo.InvariantCulture));

                            if (Harm3 > 0)
                            {
                                Log("");
                                Log("Harmony 3");
                                Log(" - Note Count", Harm3.ToString(CultureInfo.InvariantCulture));
                                Log(" - Pitched Notes (Sung)",
                                    (Harm3 - Harm3Talkies).ToString(CultureInfo.InvariantCulture));
                                Log(" - Nonpitched Notes (Talkies)",
                                    (Harm3Talkies.ToString(CultureInfo.InvariantCulture)));
                                if (calculateDensity.Checked)
                                {
                                    Log(" - Measure / Note Density",
                                        Math.Round((double) Harm3Density.Count/measures, 3)*100 + "% (" +
                                        Harm3Density.Count +
                                        " of " + measures + ")");
                                }
                                Log(" - Phrase Markers", Harm3Phrases.ToString(CultureInfo.InvariantCulture));
                                Log(" - Overdrive Markers", Harm3OD.ToString(CultureInfo.InvariantCulture));
                                Log(" - Range Shifts", Harm3RangeShift.ToString(CultureInfo.InvariantCulture));
                                Log(" - Lyric Shifts", Harm3LyricShift.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            }

            if (chkKeys.Checked)
            {
                Log("");
                Log("Has Keys?", (KeysX > 0) ? "Yes" : "No");
                if (KeysX > 0)
                {
                    Log(" - Expert Notes", NoteCount(KeysX));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(KeysXO));
                        Log("    - Blues", NoteCount(KeysXB));
                        Log("    - Yellows", NoteCount(KeysXY));
                        Log("    - Reds", NoteCount(KeysXR));
                        Log("    - Greens", NoteCount(KeysXG));
                    }
                    Log(" - Hard Notes", NoteCount(KeysH));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(KeysHO));
                        Log("    - Blues", NoteCount(KeysHB));
                        Log("    - Yellows", NoteCount(KeysHY));
                        Log("    - Reds", NoteCount(KeysHR));
                        Log("    - Greens", NoteCount(KeysHG));
                    }
                    Log(" - Medium Notes", NoteCount(KeysM));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(KeysMO));
                        Log("    - Blues", NoteCount(KeysMB));
                        Log("    - Yellows", NoteCount(KeysMY));
                        Log("    - Reds", NoteCount(KeysMR));
                        Log("    - Greens", NoteCount(KeysMG));
                    }
                    Log(" - Easy Notes", NoteCount(KeysE));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - Oranges", NoteCount(KeysEO));
                        Log("    - Blues", NoteCount(KeysEB));
                        Log("    - Yellows", NoteCount(KeysEY));
                        Log("    - Reds", NoteCount(KeysER));
                        Log("    - Greens", NoteCount(KeysEG));
                    }
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",
                            Math.Round((double) KeysDensity.Count/measures, 3)*100 + "% (" + KeysDensity.Count + " of " +
                            measures + ")");
                    }
                    Log(" - Overdrive Markers", KeysOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Trills", KeysTrills.ToString(CultureInfo.InvariantCulture));
                    Log(" - Solos", KeysSolo.ToString(CultureInfo.InvariantCulture));
                    if (KeysSolo > 0)
                    {
                        Log(" - Longest Solo", KeysSoloLength + " seconds");
                        Log(" - Toughest Solo", KeysSoloNPS + " NPS");
                    }
                }
            }

            if (chkProKeys.Checked)
            {
                Log("");
                Log("Has Pro Keys?", (ProKeysX > 0) ? "Yes" : "No");
                if (ProKeysX > 0)
                {
                    Log(" - Expert Notes", NoteCount(ProKeysX));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - White Keys", (ProKeysX - ProKeysXBlack).ToString(CultureInfo.InvariantCulture));
                        Log("    - Black Keys", ProKeysXBlack.ToString(CultureInfo.InvariantCulture));
                    }
                    Log(" - Hard Notes", NoteCount(ProKeysH));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - White Keys", (ProKeysH - ProKeysHBlack).ToString(CultureInfo.InvariantCulture));
                        Log("    - Black Keys", ProKeysHBlack.ToString(CultureInfo.InvariantCulture));
                    }
                    Log(" - Medium Notes", NoteCount(ProKeysM));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - White Keys", (ProKeysM - ProKeysMBlack).ToString(CultureInfo.InvariantCulture));
                        Log("    - Black Keys", ProKeysMBlack.ToString(CultureInfo.InvariantCulture));
                    }
                    Log(" - Easy Notes", NoteCount(ProKeysE));
                    if (breakDownInstruments.Checked)
                    {
                        Log("    - White Keys", (ProKeysE - ProKeysEBlack).ToString(CultureInfo.InvariantCulture));
                        Log("    - Black Keys", ProKeysEBlack.ToString(CultureInfo.InvariantCulture));
                    }
                    if (calculateDensity.Checked)
                    {
                        Log(" - Measure / Note Density",
                            Math.Round((double) ProKeysDensity.Count/measures, 3)*100 + "% (" + ProKeysDensity.Count +
                            " of " + measures + ")");
                    }
                    Log(" - Overdrive Markers", ProKeysXOD.ToString(CultureInfo.InvariantCulture));
                    Log(" - Trills", ProKeysXTrills.ToString(CultureInfo.InvariantCulture));
                    Log(" - Glissandos", ProKeysXGliss.ToString(CultureInfo.InvariantCulture));
                    Log(" - Solos", ProKeysXSolo.ToString(CultureInfo.InvariantCulture));
                    if (ProKeysXSolo > 0)
                    {
                        Log(" - Longest Solo", ProKeysSoloLength + " seconds");
                        Log(" - Toughest Solo", ProKeysSoloNPS + " NPS");
                    }
                    Log(" - Range Markers", ProKeysXRanges.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (!chkKeys.Checked && !chkProKeys.Checked) return;
            Log("");
            Log("Has Keys Right Hand Animations?", (KeysAnimRH > 0) ? "Yes" : "No");
            if (KeysAnimRH > 0)
            {
                Log(" - Right Hand Animation Notes", KeysAnimRH.ToString(CultureInfo.InvariantCulture));
            }
            Log("Has Keys Left Hand Animations?", (KeysAnimLH > 0) ? "Yes" : "No");
            if (KeysAnimLH > 0)
            {
                Log(" - Left Hand Animation Notes", KeysAnimLH.ToString(CultureInfo.InvariantCulture));
            }
        }

        private string NoteCount(int count)
        {
            var amount = "NPS";
            var nps = count/LengthSeconds;

            if (!(nps < 1.0))
                return count + (count > 0 && calculateNPS.Checked ? " (" + Math.Round(nps, 1) + " " + amount + ")" : "");
            nps = count/(LengthSeconds/60);
            amount = "NPM";

            return count + (count > 0 && calculateNPS.Checked ? " (" + Math.Round(nps, 1) + " " + amount + ")" : "");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MIDIAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (picloading.Visible)
            {
                MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            SaveConfig();
            if (ExitonClose)
            {
                Environment.Exit(0);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("");
            Log("Analysis complete");
            ShowWait(false);
            runAnalysisAgain.Enabled = true;
            Tools.DeleteFile(Application.StartupPath + "\\bin\\c.exe");
            if (LeadVocals.Count > 0)
            {
                exportPartVocals.Enabled = true;
            }
            if (Harmonies1.Count > 0)
            {
                exportHarmonies.Enabled = true;
            }
        }

        private void ShowWait(bool wait)
        {
            picloading.Visible = wait;
            lstStats.Cursor = wait ? Cursors.WaitCursor : Cursors.Default;
            Cursor = lstStats.Cursor;
            menuStrip1.Enabled = !wait;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ResetAll();
            
            foreach (var file in filesToProcess)
            {
                if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    InputFile = file;
                    if (string.IsNullOrWhiteSpace(ExtractFile(file, Path.GetTempPath() + "temp.mid", FileType.MIDI))) return;
                    if (analyzeMoggFileInCONs.Checked)
                    {                        
                        doMoggBatch = false;
                        AnalyzeMoggFiles(new List<string> { file });
                    }
                    if (!analyzePngxboxFile.Checked) return;
                    var art = ExtractFile(file, Path.GetTempPath() + "temp.png_xbox", FileType.PNG_XBOX);
                    if (string.IsNullOrWhiteSpace(art) && !File.Exists(art))
                    {
                        Log("");
                        Log("No album art found inside CON file!");
                        return;
                    }
                    AnalyzeAlbumArt(art);
                    Tools.DeleteFile(art);
                }
                else switch (Path.GetExtension(file).ToLowerInvariant())
                {
                    case ".rbmid_ps4":
                        InputFile = file;
                            var tempMidi = Path.GetTempFileName() + ".mid";
                            if (File.Exists(file))
                            {
                                using (var inStream = File.OpenRead(file))
                                {
                                    var rb4mid = RBMidReader.ReadStream(inStream);
                                    var rb3mid = RBMidConverter.ToMid(rb4mid);

                                    using (var outStream = File.Create(tempMidi))
                                    {
                                        MidiFileWriter.WriteSMF(rb3mid, outStream);
                                    }
                                }
                            }
                            AnalyzeMIDI(tempMidi, false, filesToProcess.Count == 1);
                            break;
                    case ".mid":
                        InputFile = file;
                        AnalyzeMIDI(file, false, filesToProcess.Count == 1);
                        break;
                    case ".mogg":
                        AnalyzeMoggFiles(new List<string> { file });
                        break;
                     case ".pkg":
                        ExtractPKG(file);
                        break;
                }
            }            
        }

        private void AnalyzeAlbumArt(string art)
        {
            Log("");
            var name = Parser.Songs != null  && !art.Contains("ps3") ? Parser.Songs[0].InternalName + "_keep.png_xbox" : Path.GetFileName(art);
            Log("Album Art File Name", name);
            var fileSize = new FileInfo(art);
            string size;
            if (fileSize.Length >= 1048576)
            {
                size = Math.Round((double)fileSize.Length / 1048576, 2) + " MB";
            }
            else
            {
                size = Math.Round((double)fileSize.Length / 1024, 2) + " KB";
            }
            Log("Album Art File Size", fileSize.Length.ToString(CultureInfo.InvariantCulture) + " bytes (" + size + ")");
            var temp = Application.StartupPath + "\\bin\\temp.png";
            Tools.DeleteFile(temp);
            if (Path.GetExtension(art) == ".png_ps4")
            {
                Bitmap image = null;
                var converter = new TextureConverter();
                try
                {
                    using (var fileStream = new FileStream(art, FileMode.Open, FileAccess.Read))
                    {
                        image = converter.ToBitmap(TextureReader.ReadStream(fileStream), 0);
                    }
                }
                catch
                {
                    Log("No more information available");
                    return;
                }                
                Log("Album Art Dimensions", image.Width + "x" + image.Height);
                Log("Album Art Format", converter.TextureFormat);
                Log("Album Art Console", "PS4");
                return;
            }
            else if (!Tools.ConvertRBImage(art, temp, "png", false))
            {
                Log("No more information available");
                return;
            }
            var img = Image.FromFile(temp);
            Log("Album Art Dimensions", img.Width + "x" + img.Height);
            img.Dispose();
            Tools.DeleteFile(temp);
            Log("Album Art Format", Tools.DDS_Format);
            if (string.IsNullOrWhiteSpace(InputFile))
            {
                Log("Album Art Console", Path.GetExtension(art).ToLowerInvariant().Contains("xbox") ? "Xbox 360" : "PS3");
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip1.Enabled = !picloading.Visible;
            exportToTextFile.Visible = HaveFile && !picloading.Visible;
        }

        private void resetToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ResetAll();
            ResetFormText();
            runAnalysisAgain.Enabled = false;
        }

        private void exportToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string name;
            if (!string.IsNullOrWhiteSpace(proDrumsFolder))
            {
                name = "_prodrums";
            }
            else if (MoggFiles.Any())
            {
                name = "_mogg";
            }
            else if (!string.IsNullOrWhiteSpace(LyricSearchFolder))
            {
                name = "_lyricsearch";
            }
            else if (!string.IsNullOrWhiteSpace(InputFile))
            {
                name = Path.GetFileNameWithoutExtension(InputFile);
            }
            else
            {
                name = "_file";
            }
            name = Tools.CleanString(name, true).Replace("'", "").Replace(" ", "").Replace(",", "") + "_analysis.txt";
            
            var sfd = new SaveFileDialog
            {
                Title = "Select location to export to",
                FileName = name,
                OverwritePrompt = true,
                AddExtension = true,
                InitialDirectory = MoggFiles.Any() ? Path.GetDirectoryName(MoggFiles[0]) : desktop,
                Filter = "Text Files (*.txt)|*txt"
            };

            if (sfd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(sfd.FileName)) return;

            Tools.DeleteFile(sfd.FileName);

            var sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(false, false));
            try
            {
                for (var i = 0; i < lstStats.Items.Count; i++)
                {
                    sw.WriteLine(FormatText(lstStats.Items[i].SubItems[0].Text, lstStats.Items[i].SubItems[1].Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error exporting the MIDI Analysis\nThe error says: " + ex.Message, Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                sw.Dispose();
                return;
            }
            sw.Dispose();

            if (MessageBox.Show("Analysis file exported successfully\nDo you want to open the file now?", Text,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            Process.Start(sfd.FileName);
        }

        private string FormatText(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(value)) return "";
            if (string.IsNullOrWhiteSpace(value)) return name.Trim();

            var spacer = "...................................";
            spacer = name.Trim().Length > spacer.Length ? "" : spacer.Substring(0, spacer.Length - name.Trim().Length);

            return name.Trim() + (string.IsNullOrWhiteSpace(proDrumsFolder) ? ":   "  : "   ") + spacer + "   " + value.Trim();
        }

        private void resetToolStrip_Click(object sender, EventArgs e)
        {
            ResetAll();
            ResetFormText();
            runAnalysisAgain.Enabled = false;
            InputFile = "";
        }

        private double NoteLengthInSecs(long start, long length)
        {
            try
            {
                var startsec = GetRealtime(start);
                var endsec = GetRealtime(start + length);

                return Math.Round(endsec - startsec, 1);
            }
            catch (Exception)
            {
                return 0.0;
            }
        }
        
        private int GetMeasure(long absdelta)
        {
            var time_sig = 4;
            var time_division = 4;
            var totalmeasures = 0;
            long i = 0;

            if (absdelta == 0)
            {
                return 0;
            }

            while (i <= absdelta)
            {
                var i1 = i;
                foreach (var signature in TimeSignatures.TakeWhile(signature => signature.AbsoluteTime <= i1))
                {
                    time_sig = signature.Numerator;
                    time_division = signature.Denominator;
                }
                totalmeasures++;
                var beat_length = (int)(TicksPerQuarter / ((decimal)time_division / 4));
                i += beat_length * time_sig;
            }
            return totalmeasures;
        }

        private void ResetAll(bool batch = false)
        {
            if (!batch)
            {
                HaveFile = false;
            }            
            TotalMIDINotes = 0;
            TotalPlayableNotes = 0;
            TicksPerQuarter = 0;
            TempoEvents = new List<TempoEvent>();
            TimeSignatures = new List<TimeSignature>();
            LeadVocals = new List<LyricPhrase>();
            Harmonies1 = new List<LyricPhrase>();
            Harmonies2 = new List<LyricPhrase>();
            Harmonies3 = new List<LyricPhrase>();
            CancelWorkers = false;
            DrumsXPlus = 0;
            DrumsX = 0;
            DrumsXKick = 0;
            DrumsXSnare = 0;
            DrumsXBlueTom = 0;
            DrumsXBlueCymbal = 0;
            DrumsXYellowTom = 0;
            DrumsXYellowCymbal = 0;
            DrumsXGreenTom = 0;
            DrumsXGreenCymbal = 0;
            DrumsH = 0;
            DrumsHKick = 0;
            DrumsHSnare = 0;
            DrumsHBlueTom = 0;
            DrumsHBlueCymbal = 0;
            DrumsHYellowTom = 0;
            DrumsHYellowCymbal = 0;
            DrumsHGreenTom = 0;
            DrumsHGreenCymbal = 0;
            DrumsM = 0;
            DrumsMKick = 0;
            DrumsMSnare = 0;
            DrumsMBlueTom = 0;
            DrumsMBlueCymbal = 0;
            DrumsMYellowTom = 0;
            DrumsMYellowCymbal = 0;
            DrumsMGreenTom = 0;
            DrumsMGreenCymbal = 0;
            DrumsE = 0;
            DrumsEKick = 0;
            DrumsESnare = 0;
            DrumsEBlueTom = 0;
            DrumsEBlueCymbal = 0;
            DrumsEYellowTom = 0;
            DrumsEYellowCymbal = 0;
            DrumsEGreenTom = 0;
            DrumsEGreenCymbal = 0; 
            DrumsAnim = 0;
            ProDrumsAnim = 0;
            DrumsSolo = 0 ;
            DrumsSwells = 0 ;
            DrumsRolls = 0 ;
            DrumsOD = 0 ;
            DrumsToms = 0 ;
            DrumsFills = 0 ;
            DrumsSoloLength = 0.0;
            DrumsSoloNPS = 0.0;
            DrumsDensity = new List<int>();
            HasDiscoFlip = false;
            BassX = 0;
            BassXO = 0;
            BassXB = 0;
            BassXY = 0;
            BassXR = 0;
            BassXG = 0;
            BassH = 0;
            BassHO = 0;
            BassHB = 0;
            BassHY = 0;
            BassHR = 0;
            BassHG = 0;
            BassM = 0;
            BassMO = 0;
            BassMB = 0;
            BassMY = 0;
            BassMR = 0;
            BassMG = 0;
            BassE = 0;
            BassEO = 0;
            BassEB = 0;
            BassEY = 0;
            BassER = 0;
            BassEG = 0;
            BassTrills = 0;
            BassTremolos = 0;
            BassOD = 0;
            BassHOPOoff = 0;
            BassHOPOon = 0;
            BassLHAnim = 0;
            BassSolo = 0;
            BassSoloLength = 0.0;
            BassSoloNPS = 0.0;
            BassDensity = new List<int>();
            GuitarX = 0;
            GuitarXO = 0;
            GuitarXB = 0;
            GuitarXY = 0;
            GuitarXR = 0;
            GuitarXG = 0;
            GuitarH = 0;
            GuitarHO = 0;
            GuitarHB = 0;
            GuitarHY = 0;
            GuitarHR = 0;
            GuitarHG = 0;
            GuitarM = 0;
            GuitarMO = 0;
            GuitarMB = 0;
            GuitarMY = 0;
            GuitarMR = 0;
            GuitarMG = 0;
            GuitarE = 0;
            GuitarEO = 0;
            GuitarEB = 0;
            GuitarEY = 0;
            GuitarER = 0;
            GuitarEG = 0;
            GuitarTrills = 0;
            GuitarTremolos = 0;
            GuitarOD = 0;
            GuitarHOPOoff = 0;
            GuitarHOPOon = 0;
            GuitarLHAnim = 0;
            GuitarSolo = 0;
            GuitarSoloLength = 0.0;
            GuitarSoloNPS = 0.0;
            GuitarDensity = new List<int>();
            KeysX = 0;
            KeysXO = 0;
            KeysXB = 0;
            KeysXY = 0;
            KeysXR = 0;
            KeysXG = 0;
            KeysH = 0;
            KeysHO = 0;
            KeysHB = 0;
            KeysHY = 0;
            KeysHR = 0;
            KeysHG = 0;
            KeysM = 0;
            KeysMO = 0;
            KeysMB = 0;
            KeysMY = 0;
            KeysMR = 0;
            KeysMG = 0;
            KeysE = 0;
            KeysEO = 0;
            KeysEB = 0;
            KeysEY = 0;
            KeysER = 0;
            KeysEG = 0;
            KeysTrills = 0;
            KeysSolo = 0;
            KeysOD = 0;
            KeysSoloLength = 0.0;
            KeysSoloNPS = 0.0;
            KeysDensity = new List<int>();
            ProKeysX = 0;
            ProKeysXTrills = 0;
            ProKeysXGliss = 0;
            ProKeysXOD = 0;
            ProKeysXSolo = 0;
            ProKeysXRanges = 0;
            ProKeysH = 0;
            ProKeysM = 0;
            ProKeysE = 0;
            ProKeysXBlack = 0;
            ProKeysHBlack = 0;
            ProKeysMBlack = 0;
            ProKeysEBlack = 0;
            KeysSoloLength = 0.0;
            KeysSoloNPS = 0.0;
            ProKeysDensity = new List<int>();
            KeysAnimRH = 0;
            KeysAnimLH = 0;
            Vocals = 0;
            VocalsOD = 0;
            VocalsPhrases = 0;
            VocalsHidPerc = 0;
            VocalsDispPerc = 0;
            VocalsLyricShift = 0;
            VocalsRangeShift = 0;
            VocalsTalkies = 0;
            PercType = "N/A";
            VocalsDensity = new List<int>();
            Harm1 = 0;
            Harm1OD = 0;
            Harm1Phrases = 0;
            Harm1LyricShift = 0;
            Harm1RangeShift = 0;
            Harm1Talkies = 0;
            Harm1Density = new List<int>();
            Harm2 = 0;
            Harm2OD = 0;
            Harm2Phrases = 0;
            Harm2LyricShift = 0;
            Harm2RangeShift = 0;
            Harm2Talkies = 0;
            Harm2Density = new List<int>();
            Harm3 = 0;
            Harm3OD = 0;
            Harm3Phrases = 0;
            Harm3LyricShift = 0;
            Harm3RangeShift = 0;
            Harm3Talkies = 0;
            Harm3Density = new List<int>();
            LengthSeconds = 0.0;
            LengthLong = 0;
            MIDISize = 0;
            HasBeat = false;
            HasVenue = false;
            HasEvents = false;
            HasBRE = false;
            HasEndEvent = false;
            exportHarmonies.Enabled = false;
            exportPartVocals.Enabled = false;
            SongsToSearch.Clear();
            LyricsFound = 0;
            SongsWithLyric = 0;
            LyricSearchFolder = "";
            MoggFiles.Clear();
            proDrumsFolder = "";
            NeedProDrums = new List<string[,]>();
            NeedTomMarkers = new List<string[,]>();
            HasUnpitchedVocals = new List<string[,]>();
            HasNoVocals = new List<string[,]>();
            MissingFills = new List<string[,]>();
            MissingOverdrive = new List<string[,]>();
            TotalMIDIs = 0;
            TotalCONs = 0;
            ProBass17X = 0;
            ProBass17H = 0;
            ProBass17M = 0;
            ProBass17E = 0;
            ProBass17Misc = 0;
            ProBass22X = 0;
            ProBass22H = 0;
            ProBass22M = 0;
            ProBass22E = 0;
            ProBass22Misc = 0;
            ProGuitar17X = 0;
            ProGuitar17H = 0;
            ProGuitar17M = 0;
            ProGuitar17E = 0;
            ProGuitar17Misc = 0;
            ProGuitar22X = 0;
            ProGuitar22H = 0;
            ProGuitar22M = 0;
            ProGuitar22E = 0;
            ProGuitar22Misc = 0;            
        }

        private void ResetFormText()
        {
            lstStats.Items.Clear();
            Log("Welcome to the File Analyzer");
            Log("Drag and drop a CON, MIDI, MOGG or PNG_XBOX file here");
            Log("Or click on File -> Open song file");
            Log("Ready to begin");
        }

        private void MIDIAnalyzer_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (string.IsNullOrWhiteSpace(argument))
            {
                ResetFormText();
                return;
            }
            if (Path.GetExtension(argument).ToLowerInvariant() == ".mogg")
            {
                ResetFormText();
                doMoggBatch = false;
                AnalyzeMoggFiles(new List<string> { argument });
                return;
            }
            if (VariousFunctions.ReadFileType(argument) != XboxFileType.STFS && Path.GetExtension(argument).ToLowerInvariant() != ".mid")
            {
                ResetFormText();
                return;
            }
            ShowWait(true);
            //InputFile = argument;
            filesToProcess.Add(argument);
            backgroundWorker1.RunWorkerAsync();
        }

        private void Log(string field, string value = "")
        {
            var entry = new ListViewItem(field);
            entry.SubItems.Add(value);

            if (lstStats.InvokeRequired)
            {
                lstStats.Invoke(new MethodInvoker(() => lstStats.Items.Add(entry)));
            }
            else
            {
                lstStats.Items.Add(entry); 
            }
        }

        private void openMIDIOrCONFileToolStrip_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select song file to analyze",
                Multiselect = false
            };
            if (ofd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(ofd.FileName)) return;
            
            if (VariousFunctions.ReadFileType(ofd.FileName) == XboxFileType.STFS || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".mid"
                || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".mogg" || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".yarg_mogg"
                || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".png_xbox" || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".png_ps3"
                || Path.GetExtension(ofd.FileName).ToLowerInvariant() == ".pkg")
            {
                ShowWait(true);
                lstStats.Items.Clear();
                InputFile = ofd.FileName;
                filesToProcess.Clear();
                filesToProcess.Add(ofd.FileName);
                exportHarmonies.Enabled = false;
                exportPartVocals.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("That's not a valid file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        
        private void runAnalysisAgain_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputFile) || !File.Exists(InputFile)) return;
            ShowWait(true);
            lstStats.Items.Clear();
            exportHarmonies.Enabled = false;
            exportPartVocals.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runAnalysisAgain.Enabled = !string.IsNullOrWhiteSpace(InputFile) && File.Exists(InputFile) && HaveFile;
        }

        private void chkVocals_CheckedChanged(object sender, EventArgs e)
        {
            chkHarms.Enabled = chkVocals.Checked;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("ma");
            var help = new HelpForm(Text + " - Help", message);
            help.ShowDialog();
        }

        private void exportPartVocals_Click(object sender, EventArgs e)
        {
            ExportLyrics(false);
        }

        private string ProcessLine(string line, bool force = false)
        {
            if (line == null) return "";
            string newline;
            if (joinSyllables.Checked || force)
            {
                newline = line.Replace("$", "");
                newline = newline.Replace("#", "");
                newline = newline.Replace("^", "");
                newline = newline.Replace("- + ", "");
                newline = newline.Replace("+- ", "");
                newline = newline.Replace("- ", "");
                newline = newline.Replace(" + ", " ");
                newline = newline.Replace(" +", "");
                newline = newline.Replace("+ ", "");
                newline = newline.Replace("+-", "");
                newline = newline.Replace("=", "-");
            }
            else
            {
                newline = line;
            }
            return newline.Trim();
        }

        private void ExportLyrics(bool export_harm)
        {
            if (LeadVocals.Count > 0 || Harmonies1.Count > 0 || Harmonies2.Count > 0)
            {
                var folder = Path.GetDirectoryName(InputFile) + "\\";
                var vocals = Path.GetFileNameWithoutExtension(InputFile) + "_LeadVocals.txt";
                var harm1 = Path.GetFileNameWithoutExtension(InputFile) + "_Harmonies1.txt";
                var harm2 = Path.GetFileNameWithoutExtension(InputFile) + "_Harmonies2.txt";
                var harm3 = Path.GetFileNameWithoutExtension(InputFile) + "_Harmonies3.txt";
                var exported = "";

                if (LeadVocals.Count > 0)
                {
                    var sw = new StreamWriter(folder + vocals, false);
                    foreach (var line in LeadVocals.Where(line => !string.IsNullOrWhiteSpace(line.PhraseText)))
                    {
                        sw.WriteLine(ProcessLine(line.PhraseText));
                    }
                    sw.Dispose();
                    if (!export_harm)
                    {
                        MessageBox.Show("Lyrics for lead vocals exported successfully to " + vocals, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    exported = vocals;
                }

                if (Harmonies1.Count > 0)
                {
                    var sw = new StreamWriter(folder + harm1, false);
                    foreach (var line in Harmonies1.Where(line => !string.IsNullOrWhiteSpace(line.PhraseText)))
                    {
                        sw.WriteLine(ProcessLine(line.PhraseText));
                    }
                    sw.Dispose();
                    exported = exported + "\n" + harm1;
                }

                if (Harmonies2.Count > 0)
                {
                    var sw = new StreamWriter(folder + harm2, false);
                    foreach (var line in Harmonies2.Where(line => !string.IsNullOrWhiteSpace(line.PhraseText)))
                    {
                        sw.WriteLine(ProcessLine(line.PhraseText));
                    }
                    sw.Dispose();
                    exported = exported + "\n" + harm2;
                }

                if (Harmonies3.Count > 0)
                {
                    var sw = new StreamWriter(folder + harm3, false);
                    foreach (var line in Harmonies3.Where(line => !string.IsNullOrWhiteSpace(line.PhraseText)))
                    {
                        sw.WriteLine(ProcessLine(line.PhraseText));
                    }
                    sw.Dispose();
                    exported = exported + "\n" + harm3;
                }

                MessageBox.Show("The following lyric files were exported successfully:\n" + exported, Text,
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No lyrics to export???", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void exportHarmonies_Click(object sender, EventArgs e)
        {
            ExportLyrics(true);
        }

        private void leaveWordsSeparated_Click(object sender, EventArgs e)
        {
            leaveWordsSeparated.Checked = true;
            joinSyllables.Checked = false;
        }

        private void joinSyllables_Click(object sender, EventArgs e)
        {
            leaveWordsSeparated.Checked = false;
            joinSyllables.Checked = true;
        }

        private void batchAnalyzeMoggFilesToolStrip_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select CON or MOGG Files Folder",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;            
            Environment.CurrentDirectory = ofd.ResultPath;
            MoggFiles = Directory.GetFiles(ofd.ResultPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
            if (!MoggFiles.Any())
            {
                MessageBox.Show("No CON or MOGG files found in that directory, try again", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            ShowWait(true);
            btnCancel.Visible = true;
            doMoggBatch = true;
            backgroundWorker2.RunWorkerAsync();
        }

        private void AnalyzeMoggFiles(IEnumerable<string> files)
        {
            var analyzed = 0;
            var encrypted = 0;
            long file_size_average = 0;
            var average_channels = 0;
            var average_length = 0.00;
            var sample32 = 0;
            var sample44 = 0;
            var sample48 = 0;
            var moggs = new List<string>();
            foreach (var file in files.Where(file => !Path.GetExtension(file).ToLowerInvariant().Contains("ex")).Where(file => !Path.GetExtension(file).ToLowerInvariant().Contains("db")))//avoid EXEcutables
            {
                if (Path.GetExtension(file).ToLowerInvariant() == ".mogg")
                {
                    moggs.Add(file);
                }
                else if (Path.GetExtension(file).ToLowerInvariant() == ".yarg_mogg")
                {
                    moggs.Add(file);
                }
                else if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    moggs.Add(file);
                }
            }
            if (doMoggBatch)
            {
                Log("Beginning analysis of " + moggs.Count() + " mogg " + (moggs.Count() == 1 ? "file" : "files"));
            }
            var counter = 0;
            foreach (var file in moggs)
            {
                if (CancelWorkers)
                {
                    break;
                }
                if (string.IsNullOrEmpty(InputFile))
                {
                    InputFile = file;
                }
                var mogg = Path.GetTempPath() + "m";
                var isCON = VariousFunctions.ReadFileType(file) == XboxFileType.STFS;
                mogg = isCON ? ExtractFile(file, mogg, FileType.MOGG) : file;               
                if (string.IsNullOrWhiteSpace(mogg) || !File.Exists(mogg)) continue;
                var name = isCON || (Parser.Songs != null && Parser.Songs[0] != null) ? Parser.Songs[0].InternalName : Path.GetFileNameWithoutExtension(file);
                counter++;
                Log("");
                if (doMoggBatch)
                {
                    Log(counter + ". " + name + Path.GetExtension(file));
                }
                else
                {
                    Log("Audio File Name", name + Path.GetExtension(file));
                }
                var fileSize = new FileInfo(mogg);
                file_size_average += fileSize.Length;
                string size;
                if (fileSize.Length >= 1048576)
                {
                    size = Math.Round((double) fileSize.Length/1048576, 2) + " MB";
                }
                else
                {
                    size = Math.Round((double) fileSize.Length/1024, 2) + " KB";
                }
                Log((doMoggBatch ? "" : "Audio ") + "File Size", fileSize.Length.ToString(CultureInfo.InvariantCulture) + " bytes (" + size + ")");
                try
                {
                    if (Path.GetExtension(file).ToLowerInvariant() == ".yarg_mogg")
                    {
                        Log("Encryption Type", "YARG");
                        encrypted++;
                    }
                    else
                    {
                        using (var fs = File.OpenRead(mogg))
                        {
                            using (var br = new BinaryReader(fs))
                            {
                                var version = br.ReadByte();
                                Log("Encryption Type", version == 0x0A || version == 0xF0 ? "None" : "0x" + version.ToString("X2"));
                                if (version != 0x0A && version != 0xF0)
                                {
                                    encrypted++;
                                }
                            }
                        }
                    }
                    
                }
                catch
                {
                    if (Path.GetExtension(file).ToLowerInvariant() == ".yarg_mogg")
                    {
                        Log("Encryption Type", "YARG");
                    }
                    else
                    {
                        Log("Encryption Type", "Unknown");
                    }
                    encrypted++;
                }
                if (Path.GetExtension(file).ToLowerInvariant() == ".yarg_mogg")
                {
                    if (!nautilus3.DecY(file, DecryptMode.ToMemory))
                    {
                        Log("Encrypted, can't get more information...");
                        continue;
                    }
                }
                else
                {
                    if (isCON)
                    {
                        var Splitter = new MoggSplitter();
                        if (!Splitter.ExtractDecryptMogg(file, nautilus3, Parser))
                        {
                            Log("Encrypted, can't get more information...");
                            continue;
                        }
                    }
                    else
                    {
                        if (!nautilus3.DecM(File.ReadAllBytes(file), false, false, DecryptMode.ToMemory))
                        {
                            //try as TBRB mogg as last attempt
                            if (!Tools.BeatlesDecrypt(file, nautilus3, false))
                            {
                                Log("Encrypted, can't get more information...");
                                continue;
                            }
                        }
                    }
                }
                analyzed++;

                var length = 0.0;
                var channels = Parser.Songs == null || Parser.Songs[0] == null ? 0 : Parser.Songs[0].ChannelsTotal;
                var rate = 0;
                try
                {
                    if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                    {
                        var stream = Bass.BASS_StreamCreateFile(nautilus3.GetOggStreamIntPtr(), 0L, nautilus3.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE);
                        var len = Bass.BASS_ChannelGetLength(stream);
                        length = Math.Round(Bass.BASS_ChannelBytes2Seconds(stream, len), 2);
                        average_length += length;
                        var mogg_info = Bass.BASS_ChannelGetInfo(stream);
                        channels = mogg_info.chans;
                        average_channels += channels;
                        rate = mogg_info.freq;
                        switch (rate)
                        {
                            case 32000:
                                sample32++;
                                break;
                            case 44100:
                                sample44++;
                                break;
                            case 48000:
                                sample48++;
                                break;
                        }
                        Bass.BASS_StreamFree(stream);
                        Bass.BASS_Free();
                    }
                }
                catch (Exception ex)
                {
                    Log("Error processing that file:");
                    Log(ex.Message);
                    Bass.BASS_Free();
                    continue;
                }
                Bass.BASS_Free();

                Log("Total Channels", channels.ToString(CultureInfo.InvariantCulture));
                if (Parser.Songs != null && Parser.Songs[0] != null)
                {
                    Log("Drums Channels", Parser.Songs[0].ChannelsDrums.ToString(CultureInfo.InvariantCulture));
                    Log("Bass Channels", Parser.Songs[0].ChannelsBass.ToString(CultureInfo.InvariantCulture));
                    Log("Guitar Channels", Parser.Songs[0].ChannelsGuitar.ToString(CultureInfo.InvariantCulture));
                    Log("Vocals Channels", Parser.Songs[0].ChannelsVocals.ToString(CultureInfo.InvariantCulture));
                    Log("Keys Channels", Parser.Songs[0].ChannelsKeys.ToString(CultureInfo.InvariantCulture));
                    Log("Crowd Channels", Parser.Songs[0].ChannelsCrowd.ToString(CultureInfo.InvariantCulture));
                    Log("Backing Channels", (channels - Parser.Songs[0].ChannelsDrums - Parser.Songs[0].ChannelsBass - 
                        Parser.Songs[0].ChannelsGuitar - Parser.Songs[0].ChannelsVocals - Parser.Songs[0].ChannelsKeys - Parser.Songs[0].ChannelsCrowd).ToString(CultureInfo.InvariantCulture));
                }
                Log("Sample Rate", rate + " Hz");
                var minutes = Parser.GetSongDuration((length * 1000).ToString(CultureInfo.InvariantCulture));
                Log("Length", length + " seconds (" + minutes + ")");
                if (isCON)
                {
                    Tools.DeleteFile(mogg);
                }
            }
            if (doMoggBatch)
            {
                Log("");
                Log("Total mogg files loaded", moggs.Count().ToString(CultureInfo.InvariantCulture));
                Log("Total mogg files analyzed", analyzed.ToString(CultureInfo.InvariantCulture));
                Log("Total mogg files skipped", (moggs.Count() - analyzed).ToString(CultureInfo.InvariantCulture));
                Log("Total encrypted mogg files", encrypted.ToString(CultureInfo.InvariantCulture));
                Log("Total unencrypted mogg files", (moggs.Count() - encrypted).ToString(CultureInfo.InvariantCulture));
                Log("");
                file_size_average = file_size_average/moggs.Count();
                string average;
                if (file_size_average >= 1048576)
                {
                    average = Math.Round((double) file_size_average/1048576, 2) + " MB";
                }
                else
                {
                    average = Math.Round((double) file_size_average/1024, 2) + " KB";
                }
                if (analyzed > 0)
                {
                    average_channels = average_channels/analyzed;
                    average_length = average_length/analyzed;
                }
                Log("Mogg files using 32 kHz sample rate", sample32.ToString(CultureInfo.InvariantCulture));
                Log("Mogg files using 44.1 kHz sample rate", sample44.ToString(CultureInfo.InvariantCulture));
                Log("Mogg files using 48 kHz sample rate", sample48.ToString(CultureInfo.InvariantCulture));
                Log("Mogg files using other sample rates",(analyzed - sample48 - sample44 - sample32).ToString(CultureInfo.InvariantCulture));
                Log("Average number of channels per mogg", average_channels.ToString(CultureInfo.InvariantCulture));
                Log("Average length (in seconds) per mogg",Math.Round(average_length, 2).ToString(CultureInfo.InvariantCulture));
                Log("Average file size per mogg", average);
            }
            if (!doMoggBatch) return;
            Log("");
            Log("Batch mogg file analysis complete");
        }
        
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            HaveFile = true;
            AnalyzeMoggFiles(MoggFiles);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MoggFiles.Clear();
            ShowWait(false);
            btnCancel.Visible = false;
            CancelWorkers = false;
            Tools.DeleteFile(Application.StartupPath + "\\bin\\c.exe");
        }
        
        private void batchAnalyzeForMissingProDrums_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder containing CON/MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Environment.CurrentDirectory = ofd.ResultPath;

            ResetAll();
            ShowWait(true);
            HaveFile = true;
            proDrumsFolder = ofd.ResultPath;
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Beginning batch analysis...");
            btnCancel.Visible = true;
            isSearchingForUnpitchedVocals = false;
            isSearchingForToms = false;
            backgroundWorker4.RunWorkerAsync();
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isSearchingForUnpitchedVocals)
            {
                lstStats.Items.Clear();
            }
            Log("Analyzed " + TotalMIDIs + " MIDI " + (TotalMIDIs == 1 ? "file" : "files") + (TotalCONs == 0 ? "" : (" in " + TotalCONs + " CON " + (TotalCONs == 1 ? "file" : "files"))));
            if (isSearchingForUnpitchedVocals)
            {
                if (HasNoVocals.Any())
                {
                    Log(HasNoVocals.Count + (HasNoVocals.Count == 1 ? " song has" : " songs have") + " no vocal charts:");
                    Log("MIDI Files:", "CON Files:");
                    foreach (var need in HasNoVocals)
                    {
                        var con = Path.GetFileName(need[0, 1]);
                        Log(need[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                    }
                }
                else
                {
                    Log((TotalMIDIs == 1 ? "The file has" : "All of the files have") + " vocal charts!");
                }
                Log("");
                if (HasUnpitchedVocals.Any())
                {
                    Log(HasUnpitchedVocals.Count + (HasUnpitchedVocals.Count == 1 ? " song" : " songs") + " have unpitched vocals:");
                    Log("MIDI Files:", "CON Files:");
                    foreach (var need in HasUnpitchedVocals)
                    {
                        var con = Path.GetFileName(need[0, 1]);
                        Log(need[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                    }
                }
                else
                {
                    Log((TotalMIDIs == 1 ? "The file doesn't" : "None of the files") + " have unpitched vocals!");
                }
                Log("");
            }
            else
            {
                if (isSearchingForToms)
                {
                    if (NeedTomMarkers.Any())
                    {
                        Log(NeedTomMarkers.Count + (NeedTomMarkers.Count == 1 ? " song" : " songs") + " need tom markers:");
                        Log("MIDI Files:", "CON Files:");
                        foreach (var need in NeedTomMarkers)
                        {
                            var con = Path.GetFileName(need[0, 1]);
                            Log(need[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                        }
                    }
                    else
                    {
                        Log((TotalMIDIs == 1 ? "The file doesn't" : "None of the files") + " need tom markers!");
                    }
                    Log("");
                    isSearchingForToms = false;
                }
                else
                {
                    if (NeedProDrums.Any())
                    {
                        Log(NeedProDrums.Count + (NeedProDrums.Count == 1 ? " song" : " songs") + " need Pro Drums markers:");
                        Log("MIDI Files:", "CON Files:");
                        foreach (var need in NeedProDrums)
                        {
                            var con = Path.GetFileName(need[0, 1]);
                            Log(need[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                        }
                    }
                    else
                    {
                        Log((TotalMIDIs == 1 ? "The file doesn't" : "None of the files") + " need Pro Drums markers!");
                    }
                    Log("");
                }
            }
            
            Log("Right-click to export this information");
            ShowWait(false);
            btnCancel.Visible = false;
            CancelWorkers = false;
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            var files = Directory.GetFiles(isSearchingForUnpitchedVocals ? vocalsFolder : proDrumsFolder, "*.*").ToList();
            var CONs = new List<string>();
            foreach (var file in files.Where(file => !Path.GetExtension(file).ToLowerInvariant().Contains("ex")))//avoid EXEcutables
            {
                if (Path.GetExtension(file).ToLowerInvariant() == ".mid")
                {
                    CONs.Add(file);
                }
                else if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    CONs.Add(file);
                }
            }

            if (!CONs.Any())
            {
                Log("No CON or MIDI files found in that directory");
                return;
            }

            foreach (var con in CONs.TakeWhile(con => !CancelWorkers))
            {
                if (Path.GetExtension(con).ToLowerInvariant() == ".mid")
                {
                    if (isSearchingForUnpitchedVocals)
                    {
                        CheckMIDIForUnpitchedVocals(con, "", Path.GetFileName(con));
                    }
                    else
                    {
                        CheckMIDIForProDrums(con, "", Path.GetFileName(con));
                    }
                    TotalMIDIs++;
                }
                else if (VariousFunctions.ReadFileType(con) == XboxFileType.STFS)
                {
                    TotalCONs++;

                    var dta = Application.StartupPath + "\\bin\\temp.dta";
                    Tools.DeleteFile(dta);

                    var xPackage = new STFSPackage(con);
                    if (!xPackage.ParseSuccess) continue;

                    var xent = xPackage.GetFolder("songs");
                    if (xent == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }

                    var xfile = xPackage.GetFile("songs/songs.dta");
                    if (xfile == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }
                    
                    var xDTA = xfile.Extract();
                    if (xDTA == null || xDTA.Length == 0)
                    {
                        xPackage.CloseIO();
                        continue;
                    }
                    var sr = new StreamReader(new MemoryStream(xDTA));
                    var name = "";

                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();

                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.ToLowerInvariant().Contains("songs/") && !line.Contains("midi_file") && !line.Contains(".mid"))
                        {
                            name = Parser.GetInternalName(line);
                        }

                        if (string.IsNullOrWhiteSpace(name)) continue;
                        xfile = xPackage.GetFile("songs/" + name + "/" + name + ".mid");
                        if (xfile == null) continue;

                        var midi = Application.StartupPath + "\\bin\\analyze.mid";
                        Tools.DeleteFile(midi);
                        if (!xfile.ExtractToFile(midi)) continue;

                        TotalMIDIs++;
                        if (isSearchingForUnpitchedVocals)
                        {
                            CheckMIDIForUnpitchedVocals(midi, con, name);
                        }
                        else
                        {
                            CheckMIDIForProDrums(midi, con, name);
                        }
                        name = "";
                    }
                    sr.Dispose();
                    xPackage.CloseIO();
                }
            }

            if (isSearchingForUnpitchedVocals)
            {
                if (!HasUnpitchedVocals.Any() || !separateUnpitchedSongs.Checked) return;
                var unpitched = vocalsFolder + "\\_UNPITCHED_VOCALS\\";
                var novocals = vocalsFolder + "\\_NO_VOCALS\\";
                if (!Directory.Exists(unpitched))
                {
                    Directory.CreateDirectory(unpitched);
                }
                if (!Directory.Exists(novocals))
                {
                    Directory.CreateDirectory(novocals);
                }
                foreach (var need in HasNoVocals)
                {
                    if (!File.Exists(need[0, 1])) continue; //if already moved, ignore
                    var newfile = novocals + Path.GetFileName(need[0, 1]);
                    Tools.DeleteFile(newfile);
                    Tools.MoveFile(need[0, 1], newfile);
                }
                foreach (var need in HasUnpitchedVocals)
                {
                    if (!File.Exists(need[0, 1])) continue; //if already moved, ignore
                    var newfile = unpitched + Path.GetFileName(need[0, 1]);
                    Tools.DeleteFile(newfile);
                    Tools.MoveFile(need[0, 1], newfile);
                }
                return;
            }

            if ((!NeedProDrums.Any() || !separateFilesThatAreMissingProDrums.Checked) && (!NeedTomMarkers.Any() || !separateFilesThatAreMissingTomMarkers.Checked)) return;

            if (NeedProDrums.Any() && separateFilesThatAreMissingProDrums.Checked)
            {
                var folder = proDrumsFolder + "\\_NEED_PRO_DRUMS\\";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                foreach (var need in NeedProDrums)
                {
                    if (!File.Exists(need[0, 1])) continue; //if already moved, ignore
                    var newfile = folder + Path.GetFileName(need[0, 1]);
                    Tools.DeleteFile(newfile);
                    Tools.MoveFile(need[0, 1], newfile);
                }
            }
            if (NeedTomMarkers.Any() && separateFilesThatAreMissingTomMarkers.Checked)
            {
                var folder = proDrumsFolder + "\\_NEED_TOM_MARKERS\\";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                foreach (var need in NeedTomMarkers)
                {
                    if (!File.Exists(need[0, 1])) continue; //if already moved, ignore
                    var newfile = folder + Path.GetFileName(need[0, 1]);
                    Tools.DeleteFile(newfile);
                    Tools.MoveFile(need[0, 1], newfile);
                }
            }
        }

        private void CheckMIDIForUnpitchedVocals(string midi, string con, string name)
        {
            const int threshold = 75; //percentage to determine where to categorize song
            AnalyzeMIDI(midi, true);
            if (Vocals <= 0)
            {
                HasNoVocals.Add(new[,] { { name, string.IsNullOrWhiteSpace(con) ? midi : con } });
            }
            else if (VocalsTalkies/Vocals*100 > threshold)
            {
                HasUnpitchedVocals.Add(new[,] { { name, string.IsNullOrWhiteSpace(con) ? midi : con } });
            }
            Vocals = 0;
            VocalsTalkies = 0;
            if (string.IsNullOrWhiteSpace(con)) return;
            Tools.DeleteFile(midi);
        }

        private void CheckMIDIForProDrums(string midi, string con, string name)
        {
            MIDIFile = Tools.NemoLoadMIDI(midi);
            if (MIDIFile == null)
            {
                Log("Unable to load MIDI file '" + Path.GetFileName(midi) + "' to analyze it");
                return;
            }

            var hasdrums = false;
            for (var i = 0; i < MIDIFile.Events.Tracks; i++)
            {
                var trackname = MIDIFile.Events[i][0].ToString();
                if (!trackname.Contains("DRUMS")) continue;
                hasdrums = true;
                AnalyzeDrums(MIDIFile.Events[i]);
                break;
            }

            if (!hasdrums) return;
            if (DrumsToms > 0) 
            {
                //reset
                DrumsToms = 0;
                ProDrumsAnim = 0;
                return; //already has pro markers
            }
            else if (isSearchingForToms)
            {
                NeedTomMarkers.Add(new[,] { { name, string.IsNullOrWhiteSpace(con) ? midi : con } });
                return;
            }
            if (ProDrumsAnim > 0)
            {
                NeedProDrums.Add(new[,] {{name, string.IsNullOrWhiteSpace(con) ? midi : con}});
                //reset
                DrumsToms = 0;
                ProDrumsAnim = 0;
            }
            else
            {
                //reset
                DrumsToms = 0;
                ProDrumsAnim = 0;
                return; //doesn't have pro animations
            }

            if (string.IsNullOrWhiteSpace(con)) return;
            Tools.DeleteFile(midi);
        }

        private void searchForOccurrenceOfLyricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder of MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;            
            Environment.CurrentDirectory = ofd.ResultPath;

            var midis = Directory.GetFiles(ofd.ResultPath, "*.mid*");
            if (!midis.Any())
            {
                MessageBox.Show("No MIDI files found in that directory, try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            const string message = "Enter a word or phrase to search for:";
            var search = Interaction.InputBox(message, Text);
            if (string.IsNullOrWhiteSpace(search.Trim())) return;

            LyricSearchFolder = ofd.ResultPath;
            SongsWithLyric = 0;
            LyricsFound = 0;
            ShowWait(true);
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Searching " + midis.Count() + (midis.Count() == 1 ? " song" : " songs") + " for lyric phrase:");
            Log("'" + search + "'");
            SongsToSearch = midis.ToList();
            SearchTerm = search;
            HaveFile = true;
            btnCancel.Visible = true;
            backgroundWorker5.RunWorkerAsync();
        }

        private void backgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowWait(false);
            btnCancel.Visible = false;
            CancelWorkers = false;
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            var errors = new List<string>();
            var isFirst = true;
            foreach (var midi in SongsToSearch.TakeWhile(midi => !CancelWorkers))
            {
                LeadVocals = new List<LyricPhrase>();
                Harmonies1 = new List<LyricPhrase>();
                Harmonies2 = new List<LyricPhrase>();
                Harmonies3 = new List<LyricPhrase>();
                MIDIFile = Tools.NemoLoadMIDI(midi);
                if (MIDIFile == null)
                {
                    Log("Unable to load MIDI file '" + Path.GetFileName(midi) + "' to analyze it");
                    errors.Add("Song: " + Path.GetFileNameWithoutExtension(midi));
                    continue;
                }
                TicksPerQuarter = MIDIFile.DeltaTicksPerQuarterNote;
                BuildTempoList();
                BuildTimeSignatureList();
                try
                {
                    for (var i = 0; i < MIDIFile.Events.Tracks; i++)
                    {
                        var trackname = MIDIFile.Events[i][0].ToString();
                        if (trackname.Contains("VOCALS") && chkVocals.Checked)
                        {
                            GetPhraseMarkers(MIDIFile.Events[i], out LeadVocals);
                            AnalyzeVocals(MIDIFile.Events[i]);
                        }
                        else if (trackname.Contains("HARM1") && chkHarms.Checked)
                        {
                            GetPhraseMarkers(MIDIFile.Events[i], out Harmonies1);
                            AnalyzeHarm1(MIDIFile.Events[i]);
                        }
                        else if (trackname.Contains("HARM2") && chkHarms.Checked)
                        {
                            GetPhraseMarkers(MIDIFile.Events[i], out Harmonies2);
                            AnalyzeHarm2(MIDIFile.Events[i]);
                        }
                        else if (trackname.Contains("HARM3") && chkHarms.Checked)
                        {
                            foreach (var harmony in Harmonies1.Select(harm => new LyricPhrase { PhraseEnd = harm.PhraseEnd, PhraseStart = harm.PhraseStart, PhraseText = "" }))
                            {
                                Harmonies3.Add(harmony);
                            }
                            AnalyzeHarm3(MIDIFile.Events[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Song: " + Path.GetFileNameWithoutExtension(midi));
                    errors.Add("Error: " + ex.Message);
                    continue;
                }
                var FoundVocals = new List<LyricPhrase>();
                if (LeadVocals.Any())
                {
                    FoundVocals.AddRange(from line in LeadVocals where LineContains(ProcessLine(line.PhraseText, true), SearchTerm) select line);
                }
                var FoundHarm1 = new List<LyricPhrase>();
                if (Harmonies1.Any())
                {
                    FoundHarm1.AddRange(from line in Harmonies1 where LineContains(ProcessLine(line.PhraseText, true), SearchTerm) select line);
                }
                var FoundHarm2 = new List<LyricPhrase>();
                if (Harmonies2.Any())
                {
                    FoundHarm2.AddRange(from line in Harmonies2 where LineContains(ProcessLine(line.PhraseText, true), SearchTerm) select line);
                }
                var FoundHarm3 = new List<LyricPhrase>();
                if (Harmonies3.Any())
                {
                    FoundHarm3.AddRange(from line in Harmonies3 where LineContains(ProcessLine(line.PhraseText, true), SearchTerm) select line);
                }

                if (!FoundVocals.Any() && ((!FoundHarm1.Any() && !FoundHarm2.Any() && !FoundHarm3.Any()) || ignoreHarmonies.Checked)) continue;
                if (!ignoreHarmonies.Checked || isFirst)
                {
                    Log("");
                    isFirst = false;
                }
                Log("Song: " + Path.GetFileNameWithoutExtension(midi));
                if (showLyricsToolStrip.Checked)
                {
                    if (FoundVocals.Any())
                    {
                        Log("PART VOCALS");
                        foreach (var line in FoundVocals)
                        {
                            LogFoundLyric(line);
                        }
                    }
                    if (!ignoreHarmonies.Checked)
                    {
                        if (FoundHarm1.Any())
                        {
                            Log("HARM1");
                            foreach (var line in FoundHarm1)
                            {
                                LogFoundLyric(line);
                            }
                        }
                        if (FoundHarm2.Any())
                        {
                            Log("HARM2");
                            foreach (var line in FoundHarm2)
                            {
                                LogFoundLyric(line);
                            }
                        }
                        if (FoundHarm3.Any())
                        {
                            Log("HARM3");
                            foreach (var line in FoundHarm3)
                            {
                                LogFoundLyric(line);
                            }
                        }
                    }
                }
                LyricsFound = LyricsFound + FoundVocals.Count;
                if (!ignoreHarmonies.Checked)
                {
                    LyricsFound = LyricsFound + FoundHarm1.Count + FoundHarm2.Count + FoundHarm3.Count;
                }
                SongsWithLyric++;
            }
            Log("");
            if (LyricsFound == 0)
            {
                Log("No instances of that lyric phrase found");
            }
            else
            {
                Log("Found " + LyricsFound + (LyricsFound == 1 ? " instance" : " instances") + " of that lyric phrase in " + SongsWithLyric + (SongsWithLyric == 1 ? " song" : " songs"));
            }
            if (errors.Any())
            {
                Log("");
                Log("The following " + (errors.Count == 2 ? "song was" : "songs were") + " not read:");
                foreach (var error in errors)
                {
                    Log(error);
                }
                Log("");
            }
            Log("Right-click to export this information");
        }

        private void LogFoundLyric(LyricPhrase line)
        {
            if (displayPhraseTiming.Checked)
            {
                var start = Parser.GetSongDuration(Convert.ToInt64(line.PhraseStart * 1000).ToString(CultureInfo.InvariantCulture));
                var end = Parser.GetSongDuration(Convert.ToInt64(line.PhraseEnd * 1000).ToString(CultureInfo.InvariantCulture));
                Log(ProcessLine("[" + start + " to " + end + "]"));
            }
            Log(ProcessLine(line.PhraseText));
        }

        private static bool LineContains(string line, string search_term)
        {
            
            var clean_line = line.ToLowerInvariant();
            var term = search_term.ToLowerInvariant();

            try
            {
                if (clean_line.Contains(term))
                {
                    return true;
                }
                if (clean_line.Replace("ing ", "in' ").Contains(term.Replace("ing ", "in' ")))
                {
                    return true;
                }
                if (clean_line.EndsWith("ing", StringComparison.Ordinal))
                {
                    clean_line = clean_line.Substring(0, clean_line.Length - 3) + "in'";
                    return LineContains(clean_line, term);
                }
                if (term.EndsWith("ing", StringComparison.Ordinal))
                {
                    term = term.Substring(0, term.Length - 3) + "in'";
                    return LineContains(clean_line, term);
                }
            }
            catch (Exception)
            {}
            return false;
        }

        private void onlyListSongsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showLyricsToolStrip.Checked = !onlyListSongs.Checked;
        }

        private void showLyricsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onlyListSongs.Checked = !showLyricsToolStrip.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelWorkers = true;
            Log("");
            Log("User cancelled process...stopping...");
            Log("");
        }
        
        private void openAudioAnalyzer_Click(object sender, EventArgs e)
        {
            if (InputFile == null)
            {
                InputFile = "";
            }
            var analyzer = new AudioAnalyzer(InputFile);
            analyzer.ShowDialog();
        }

        private sealed class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base(new DarkColors()) { }
        }

        private sealed class DarkColors : ProfessionalColorTable
        {
            public override Color ImageMarginGradientBegin
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return mMenuBackground; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return mMenuBackground; }
            }
        }

        private void picPin_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (picPin.Tag.ToString())
            {
                case "pinned":
                    picPin.Image = Resources.unpinned;
                    picPin.Tag = "unpinned";
                    break;
                case "unpinned":
                    picPin.Image = Resources.pinned;
                    picPin.Tag = "pinned";
                    break;
            }
            TopMost = picPin.Tag.ToString() == "pinned";
        }

        private void batchAnalyzeForUnpitched_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder containing CON/MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Environment.CurrentDirectory = ofd.ResultPath;

            ResetAll();
            ShowWait(true);
            HaveFile = true;
            vocalsFolder = ofd.ResultPath;
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Beginning batch analysis...");
            btnCancel.Visible = true;
            isSearchingForUnpitchedVocals = true;
            backgroundWorker4.RunWorkerAsync();
        }

        private void ExtractPKG(string file)
        {
            var folder = Application.StartupPath + "\\file\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var shortPath = Path.GetFileNameWithoutExtension(file).Replace(" ", "").Replace("-", "").Replace("_", "").Trim();
            if (shortPath.Length > 10) //to avoid very lengthy PKG file names, which can cause issues with long paths on Windows
            {
                shortPath = shortPath.Substring(0, 10) + "_ex";
            }
            var outFolder = folder + shortPath;
            Tools.DeleteFolder(outFolder, true);            
            if (!Tools.ExtractPS3PKG(file, outFolder))
            {
                MessageBox.Show("Failed to process that PKG file, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var DTA = Directory.GetFiles(outFolder, "songs.dta", SearchOption.AllDirectories);
            if (DTA.Count() == 0)
            {
                MessageBox.Show("No songs.dta file found, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Parser.ReadDTA(File.ReadAllBytes(DTA[0]));
            var internalName = Parser.Songs[0].InternalName;
            var PNG_PS3 = Directory.GetFiles(outFolder, internalName + "_keep.png_ps3", SearchOption.AllDirectories);
            var EDAT = Directory.GetFiles(outFolder, internalName + ".mid.edat", SearchOption.AllDirectories);
            var MIDI = EDAT[0].Replace(".mid.edat", ".mid");
            //var decrypt = Tools.DecryptEdat(EDAT[0], MIDI, klic);
            var MOGG = Directory.GetFiles(outFolder, internalName + ".mogg", SearchOption.AllDirectories);            
            if (!nautilus3.DecM(File.ReadAllBytes(MOGG[0]), false, false, DecryptMode.ToMemory))
            {
                if (!Tools.BeatlesDecrypt(MOGG[0], nautilus3, false))
                {
                    MessageBox.Show("Failed to decrypt mogg file, can't analyze", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            InputFile = file;
            AnalyzeMIDI(MIDI);
            AnalyzeMoggFiles(new List<string> { MOGG[0] });
            AnalyzeAlbumArt(PNG_PS3[0]);
        }

        private void checkForNoDrumFills_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder containing CON/MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Environment.CurrentDirectory = ofd.ResultPath;

            ResetAll();
            ShowWait(true);
            HaveFile = true;
            proDrumsFolder = ofd.ResultPath;
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Beginning batch analysis...");
            btnCancel.Visible = true;
            backgroundWorker6.RunWorkerAsync();
        }

        private void backgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {
            var files = Directory.GetFiles(proDrumsFolder, "*.*").ToList();
            var CONs = new List<string>();
            foreach (var file in files.Where(file => !Path.GetExtension(file).ToLowerInvariant().Contains("ex")))//avoid EXEcutables
            {
                if (Path.GetExtension(file).ToLowerInvariant() == ".mid")
                {
                    CONs.Add(file);
                }
                else if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    CONs.Add(file);
                }
            }

            if (!CONs.Any())
            {
                Log("No CON or MIDI files found in that directory");
                return;
            }

            foreach (var con in CONs.TakeWhile(con => !CancelWorkers))
            {
                if (Path.GetExtension(con).ToLowerInvariant() == ".mid")
                {
                    CheckForMissingFills(con, "", Path.GetFileName(con));
                    TotalMIDIs++;
                }
                else if (VariousFunctions.ReadFileType(con) == XboxFileType.STFS)
                {
                    TotalCONs++;

                    var dta = Application.StartupPath + "\\bin\\temp.dta";
                    Tools.DeleteFile(dta);

                    var xPackage = new STFSPackage(con);
                    if (!xPackage.ParseSuccess) continue;

                    var xent = xPackage.GetFolder("songs");
                    if (xent == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }

                    var xfile = xPackage.GetFile("songs/songs.dta");
                    if (xfile == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }

                    var xDTA = xfile.Extract();
                    if (xDTA == null || xDTA.Length == 0)
                    {
                        xPackage.CloseIO();
                        continue;
                    }
                    var sr = new StreamReader(new MemoryStream(xDTA));
                    var name = "";

                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();

                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.ToLowerInvariant().Contains("songs/") && !line.Contains("midi_file") && !line.Contains(".mid"))
                        {
                            name = Parser.GetInternalName(line);
                        }

                        if (string.IsNullOrWhiteSpace(name)) continue;
                        xfile = xPackage.GetFile("songs/" + name + "/" + name + ".mid");
                        if (xfile == null) continue;

                        var midi = Application.StartupPath + "\\bin\\analyze.mid";
                        Tools.DeleteFile(midi);
                        if (!xfile.ExtractToFile(midi)) continue;

                        TotalMIDIs++;
                        CheckForMissingFills(midi, con, name);
                        name = "";
                    }
                    sr.Dispose();
                    xPackage.CloseIO();
                }
            }            

            if (!MissingFills.Any()) return;

            var folder = proDrumsFolder + "\\_MISSING_FILLS\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (var missing in MissingFills)
            {
                if (!File.Exists(missing[0, 1])) continue; //if already moved, ignore
                var newfile = folder + Path.GetFileName(missing[0, 1]);
                Tools.DeleteFile(newfile);
                Tools.MoveFile(missing[0, 1], newfile);
            }
        }

        private void CheckForMissingFills(string midi, string con, string name)
        {
            MIDIFile = Tools.NemoLoadMIDI(midi);
            if (MIDIFile == null)
            {
                Log("Unable to load MIDI file '" + Path.GetFileName(midi) + "' to analyze it");
                return;
            }

            var hasDrums = false;
            DrumsFills = 0;//reset for every song
            DrumsX = 0;//reset for every song
            for (var i = 0; i < MIDIFile.Events.Tracks; i++)
            {
                var trackname = MIDIFile.Events[i][0].ToString();
                if (!trackname.Contains("DRUMS")) continue;
                hasDrums = true;
                AnalyzeDrums(MIDIFile.Events[i]);
                break;
            }

            if (!hasDrums || DrumsX == 0) return; //avoid false positives like Song of the Century with a blank PART DRUMS chart
            var hasFills = DrumsFills > 0;

            if (hasFills) return;
            MissingFills.Add(new[,] { { name, string.IsNullOrWhiteSpace(con) ? midi : con } });
            
            if (string.IsNullOrWhiteSpace(con)) return;
            Tools.DeleteFile(midi);
        }

        private void backgroundWorker6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            Log("Analyzed " + TotalMIDIs + " MIDI " + (TotalMIDIs == 1 ? "file" : "files") + (TotalCONs == 0 ? "" : (" in " + TotalCONs + " CON " + (TotalCONs == 1 ? "file" : "files"))));

            if (MissingFills.Any())
            {
                Log(MissingFills.Count + (MissingFills.Count == 1 ? " song is" : " songs are") + " missing drums fills:");
                Log("MIDI Files:", "CON Files:");
                foreach (var missing in MissingFills)
                {
                    var con = Path.GetFileName(missing[0, 1]);
                    Log(missing[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                }
            }
            else
            {
                Log((TotalMIDIs == 1 ? "The file isn't" : "None of the files are") + " missing drum fills!");
            }
            Log("");

            Log("Right-click to export this information");
            ShowWait(false);
            btnCancel.Visible = false;
            CancelWorkers = false;
        }

        private void checkForNoOverdrive_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder containing CON/MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Environment.CurrentDirectory = ofd.ResultPath;

            ResetAll();
            ShowWait(true);
            HaveFile = true;
            proDrumsFolder = ofd.ResultPath;
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Beginning batch analysis...");
            btnCancel.Visible = true;
            backgroundWorker7.RunWorkerAsync();
        }

        private void backgroundWorker7_DoWork(object sender, DoWorkEventArgs e)
        {
            var files = Directory.GetFiles(proDrumsFolder, "*.*").ToList();
            var CONs = new List<string>();
            foreach (var file in files.Where(file => !Path.GetExtension(file).ToLowerInvariant().Contains("ex")))//avoid EXEcutables
            {
                if (Path.GetExtension(file).ToLowerInvariant() == ".mid")
                {
                    CONs.Add(file);
                }
                else if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                {
                    CONs.Add(file);
                }
            }

            if (!CONs.Any())
            {
                Log("No CON or MIDI files found in that directory");
                return;
            }

            foreach (var con in CONs.TakeWhile(con => !CancelWorkers))
            {
                if (Path.GetExtension(con).ToLowerInvariant() == ".mid")
                {
                    CheckForMissingOverdrive(con, "", Path.GetFileName(con));
                    TotalMIDIs++;
                }
                else if (VariousFunctions.ReadFileType(con) == XboxFileType.STFS)
                {
                    TotalCONs++;

                    var dta = Application.StartupPath + "\\bin\\temp.dta";
                    Tools.DeleteFile(dta);

                    var xPackage = new STFSPackage(con);
                    if (!xPackage.ParseSuccess) continue;

                    var xent = xPackage.GetFolder("songs");
                    if (xent == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }

                    var xfile = xPackage.GetFile("songs/songs.dta");
                    if (xfile == null)
                    {
                        xPackage.CloseIO();
                        continue;
                    }

                    var xDTA = xfile.Extract();
                    if (xDTA == null || xDTA.Length == 0)
                    {
                        xPackage.CloseIO();
                        continue;
                    }
                    var sr = new StreamReader(new MemoryStream(xDTA));
                    var name = "";

                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();

                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.ToLowerInvariant().Contains("songs/") && !line.Contains("midi_file") && !line.Contains(".mid"))
                        {
                            name = Parser.GetInternalName(line);
                        }

                        if (string.IsNullOrWhiteSpace(name)) continue;
                        xfile = xPackage.GetFile("songs/" + name + "/" + name + ".mid");
                        if (xfile == null) continue;

                        var midi = Application.StartupPath + "\\bin\\analyze.mid";
                        Tools.DeleteFile(midi);
                        if (!xfile.ExtractToFile(midi)) continue;

                        TotalMIDIs++;
                        CheckForMissingOverdrive(midi, con, name);
                        name = "";
                    }
                    sr.Dispose();
                    xPackage.CloseIO();
                }
            }

            if (!MissingOverdrive.Any()) return;

            var folder = proDrumsFolder + "\\_MISSING_OVERDRIVE\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (var missing in MissingOverdrive)
            {
                if (!File.Exists(missing[0, 1])) continue; //if already moved, ignore
                var newfile = folder + Path.GetFileName(missing[0, 1]);
                Tools.DeleteFile(newfile);
                Tools.MoveFile(missing[0, 1], newfile);
            }
        }

        private void backgroundWorker7_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Analyzed " + TotalMIDIs + " MIDI " + (TotalMIDIs == 1 ? "file" : "files") + (TotalCONs == 0 ? "" : (" in " + TotalCONs + " CON " + (TotalCONs == 1 ? "file" : "files"))));

            if (MissingOverdrive.Any())
            {
                Log(MissingOverdrive.Count + (MissingOverdrive.Count == 1 ? " song is" : " songs are") + " missing overdrive markers:");
                Log("MIDI Files:", "CON Files:");
                foreach (var missing in MissingOverdrive)
                {
                    var con = Path.GetFileName(missing[0, 1]);
                    Log(missing[0, 0] + ".mid", string.IsNullOrWhiteSpace(con) || con.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) ? "N/A" : con);
                }
            }
            else
            {
                Log((TotalMIDIs == 1 ? "The file isn't" : "None of the files are") + " missing overdrive markers!");
            }
            Log("");

            Log("Right-click to export this information");
            ShowWait(false);
            btnCancel.Visible = false;
            CancelWorkers = false;
        }

        private void CheckForMissingOverdrive(string midi, string con, string name)
        {
            MIDIFile = Tools.NemoLoadMIDI(midi);
            if (MIDIFile == null)
            {
                Log("Unable to load MIDI file '" + Path.GetFileName(midi) + "' to analyze it");
                return;
            }

            var hasDrums = false;
            var hasBass = false;
            var hasKeys = false;
            var hasGuitar = false;
            DrumsOD = 0;//reset for every song
            DrumsX = 0;
            BassOD = 0;
            BassX = 0;
            KeysOD = 0;
            KeysX = 0;
            GuitarOD = 0;
            GuitarX = 0;
            for (var i = 0; i < MIDIFile.Events.Tracks; i++)
            {
                var trackname = MIDIFile.Events[i][0].ToString();
                if (trackname.Contains("DRUMS"))
                {
                    hasDrums = true;
                    AnalyzeDrums(MIDIFile.Events[i]);
                }
                else if (trackname.Contains("BASS"))
                {
                    hasBass = true;
                    AnalyzeBass(MIDIFile.Events[i]);
                }
                else if (trackname.Contains("KEYS"))
                {
                    hasKeys = true;
                    AnalyzeKeys(MIDIFile.Events[i]);
                }
                else if (trackname.Contains("GUITAR"))
                {
                    hasGuitar = true;
                    AnalyzeGuitar(MIDIFile.Events[i]);
                }
            }

            var hasOverdrive = true; //let's default to having overdrive
            if (hasDrums && DrumsX > 0 && DrumsOD == 0) hasOverdrive = false;
            if (hasBass && BassX > 0 &&  BassOD == 0) hasOverdrive = false;
            if (hasKeys && KeysX > 0 && KeysOD == 0) hasOverdrive = false;
            if (hasGuitar && GuitarX > 0 && GuitarOD == 0) hasOverdrive = false;

            if (hasOverdrive) return;
            MissingOverdrive.Add(new[,] { { name, string.IsNullOrWhiteSpace(con) ? midi : con } });

            if (string.IsNullOrWhiteSpace(con)) return;
            Tools.DeleteFile(midi);
        }

        private void batchAnalyzeForMissingTomMarkers_Click(object sender, EventArgs e)
        {
            var ofd = new FolderPicker
            {
                Title = "Select folder containing CON/MIDI files",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true) return;
            Environment.CurrentDirectory = ofd.ResultPath;

            ResetAll();
            ShowWait(true);
            HaveFile = true;
            proDrumsFolder = ofd.ResultPath;
            lstStats.BackgroundImage = null;
            lstStats.Items.Clear();
            Log("Beginning batch analysis...");
            btnCancel.Visible = true;
            isSearchingForUnpitchedVocals = false;
            isSearchingForToms = true;
            backgroundWorker4.RunWorkerAsync();
        }
    }    
}
