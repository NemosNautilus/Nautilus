using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Nautilus.Properties;
using Nautilus.x360;
using NAudio.Midi;
using System.Drawing;

namespace Nautilus
{
    public partial class MIDICleaner : Form
    {
        public bool ExitonClose;
        private readonly List<string> inputMIDIs;
        private readonly List<string> inputCONs;
        private List<string> problem_midis;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private List<MidiEvent> toRemove;
        private List<MidiEvent> toAdd;
        private readonly List<string> detailed_message;
        private DateTime startTime;
        private DateTime endTime;
        private readonly string argument;
        private int TicksPerQuarter;
        private MidiFile dirtyMIDI;
        private long endevent;
        private long songfixes;
        private long totalfixes;
        private List<TempoEvent> TempoEvents;
        private List<TimeSignature> TimeSignatures;
        private int drumfixes;
        private int bassfixes;
        private int guitarfixes;
        private int vocalsfixes;
        private int harm1fixes;
        private int harm2fixes;
        private int harm3fixes;
        private int venuefixes;
        private int eventsfixes;
        private int beatfixes;
        private int keysfixes;
        private int prokeysfixes;
        private bool loading;
        private bool saving;
        private string fixedmidi;
        private int workingcounter;
        private STFSPackage xPackage;
        private RSAParams xsignature;
        private bool isCON;
        private bool isRBN2;
        private static Color mMenuBackground;
        private string activeFile;

        public MIDICleaner(string args, Color ButtonBackColor, Color ButtonTextColor)
        {
            InitializeComponent();
            loading = true;
            mMenuBackground = menuStrip1.BackColor;
            menuStrip1.Renderer = new DarkRenderer();
            Tools = new NemoTools();
            Parser = new DTAParser();
            argument = args;
            inputMIDIs = new List<string>();
            inputCONs = new List<string>();
            detailed_message = new List<string>();
            cboLength.SelectedIndex = 1;

            var formButtons = new List<Button> { btnBegin, btnOpen};
            foreach (var button in formButtons)
            {
                button.BackColor = ButtonBackColor;
                button.ForeColor = ButtonTextColor;
                button.FlatAppearance.MouseOverBackColor = button.BackColor == Color.Transparent ? Color.FromArgb(127, Color.AliceBlue.R, Color.AliceBlue.G, Color.AliceBlue.B) : Tools.LightenColor(button.BackColor);
            }
        }

        private void Log(string message)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new MethodInvoker(() => lstLog.Items.Add(message)));
                lstLog.Invoke(new MethodInvoker(() => lstLog.SelectedIndex = lstLog.Items.Count - 1));
            }
            else
            {
                lstLog.Items.Add(message);
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
            }
        }
        
        private void LogDetails(string message,bool force_show = false,bool show_details = false)
        {
            if (message.Contains("PART DRUMS:"))
            {
                drumfixes++;
                songfixes++;
            }
            else if (message.Contains("PART BASS:"))
            {
                bassfixes++;
                songfixes++;
            }
            else if (message.Contains("PART GUITAR:"))
            {
                guitarfixes++;
                songfixes++;
            }
            else if (message.Contains("PART VOCALS:"))
            {
                vocalsfixes++;
                songfixes++;
            }
            else if (message.Contains("HARM1:"))
            {
                harm1fixes++;
                songfixes++;
            }
            else if (message.Contains("HARM2:"))
            {
                harm2fixes++;
                songfixes++;
            }
            else if (message.Contains("HARM3:"))
            {
                harm3fixes++;
                songfixes++;
            }
            else if (message.Contains("PART KEYS:"))
            {
                keysfixes++;
                songfixes++;
            }
            else if (message.Contains("PART REAL_KEYS_") && message.Contains(":")) //accounts for XMHE tracks
            {
                prokeysfixes++;
                songfixes++;
            }
            else if (message.Contains("EVENTS:"))
            {
                eventsfixes++;
                songfixes++;
            }
            else if (message.Contains("VENUE:"))
            {
                venuefixes++;
                songfixes++;
            }
            else if (message.Contains("BEAT:"))
            {
                beatfixes++;
                songfixes++;
            }
            else if (message.Contains(":") && !message.Contains("Magma: C3")) //all errors are preceding with trackname:
            {
                songfixes++;
            }

            //for some details we always want to be shown even if "detailed logging" is disabled
            if (force_show)
            {
                Log(message);
                return;
            }

            if (!detailedLoggingToolStripMenuItem.Checked)
            {
                return;
            }

            //show_details = throw the whole log at the log display
            //otherwise just keep accumulating the details, this speeds up display of the log
            if (show_details && detailed_message.Count != 0)
            {
                if (lstLog.InvokeRequired)
                {
                    lstLog.Invoke(new MethodInvoker(() => lstLog.BeginUpdate()));
                    foreach (var line in detailed_message)
                    {
                        var newline = line;
                        lstLog.Invoke(new MethodInvoker(() => lstLog.Items.Add(newline)));
                    }
                    lstLog.Invoke(new MethodInvoker(() => lstLog.EndUpdate()));
                    lstLog.Invoke(new MethodInvoker(() => lstLog.SelectedIndex = lstLog.Items.Count - 1));
                }
                else
                {
                    lstLog.BeginUpdate();
                    foreach (var line in detailed_message)
                    {
                        lstLog.Items.Add(line);
                    }
                    lstLog.EndUpdate();
                    lstLog.SelectedIndex = lstLog.Items.Count - 1;
                }
            }
            else
            {
                detailed_message.Add(message);
            }
        }
        
        private int GetVelocity()
        {
            var velocity = 96; //default in case something goes wrong

            if (numVelocity.InvokeRequired)
            {
                numVelocity.Invoke(new MethodInvoker(() => velocity = (int)numVelocity.Value));
            }
            else
            {
                velocity = (int) numVelocity.Value;
            }
            return velocity;
        }

        private void HandleDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void HandleDragDrop(object sender, DragEventArgs e)
        {
            if (picWorking.Visible) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Tools.CurrentFolder = Path.GetDirectoryName(files[0]);
            RefreshInputFiles(files);
        }
        
        private void exportLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.ExportLog(Text, lstLog.Items);
        }
        
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("mc");
            var help = new HelpForm(Text + " - Help", message, true);
            help.ShowDialog();
        }

        private void BuildTempoList()
        {
            //code provided by raynebc
            //Build tempo list
            var currentbpm = 120.00;
            var realtime = 0.0;
            var reldelta = 0;   //The number of delta ticks since the last tempo change
            TempoEvents = new List<TempoEvent>();
            foreach (var ev in dirtyMIDI.Events[0])
            {
                //Log(ev.ToString() + " " + ev.CommandCode);
                reldelta += ev.DeltaTime;
                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;
                var tempo = (MetaEvent)ev;
                if (tempo.MetaEventType != MetaEventType.SetTempo) continue;
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

        private void BuildTimeSignatureList()
        {
            TimeSignatures = new List<TimeSignature>();
            foreach (var ev in dirtyMIDI.Events[0])
            {
                if (ev.CommandCode != MidiCommandCode.MetaEvent) continue;
                var signature = (MetaEvent)ev;
                if (signature.MetaEventType != MetaEventType.TimeSignature) continue;
                //Track the time signature change
                var index1 = signature.ToString().IndexOf(" ", signature.ToString().IndexOf("TimeSignature", StringComparison.Ordinal), StringComparison.Ordinal) + 1;
                var index2 = signature.ToString().IndexOf("/", StringComparison.Ordinal);
                var numerator = Convert.ToInt16(signature.ToString().Substring(index1, index2 - index1));
                //Track the time signature change
                index1 = signature.ToString().IndexOf("/", StringComparison.Ordinal) + 1;
                index2 = signature.ToString().IndexOf(" ", signature.ToString().IndexOf("/", StringComparison.Ordinal), StringComparison.Ordinal);
                var denominator = Convert.ToInt16(signature.ToString().Substring(index1, index2 - index1));
                var time_sig = new TimeSignature
                {
                    AbsoluteTime = ev.AbsoluteTime,
                    Numerator = numerator,
                    Denominator = denominator
                };
                TimeSignatures.Add(time_sig);
            }
            foreach (var signature in TimeSignatures.Where(signature => signature.Numerator == 1))
            {
                Log("WARNING: Found 1/" + signature.Denominator + " time signature" + FormattedTime(signature.AbsoluteTime));
                Log("These are extremely rare, please check the tempo map and BEAT tracks for possible mistakes");
            }
        }

        private void ClearAll()
        {
            detailed_message.Clear();
            songfixes = 0;
            drumfixes = 0;
            bassfixes = 0;
            guitarfixes = 0;
            vocalsfixes = 0;
            harm1fixes = 0;
            harm2fixes = 0;
            harm3fixes = 0;
            eventsfixes = 0;
            venuefixes = 0;
            beatfixes = 0;
            endevent = 0;
        }

        private void CleanMIDI(string file)
        {
            ClearAll();
            Log("Trying to clean MIDI file " + Path.GetFileName(file));

            try
            {
                dirtyMIDI = Tools.NemoLoadMIDI(file);
                if (dirtyMIDI == null)
                {
                    Log("Unable to load MIDI file '" + Path.GetFileName(file) + "' to clean it");
                    problem_midis.Add(Path.GetFileName(file));
                    return;
                }

                TicksPerQuarter = dirtyMIDI.DeltaTicksPerQuarterNote;
                BuildTempoList();
                BuildTimeSignatureList();
                var HasBEAT = false;
                var cleanMIDI = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "_clean.mid";                              

                //let's clean all the sequencer specific and other misc shit that was causing issues
                //this is when they occur before the trackname, which throws off the code below
                for (var i = 0; i < dirtyMIDI.Events.Tracks; i++)
                {
                    toRemove = new List<MidiEvent>();
                    foreach (var ev in dirtyMIDI.Events[i])
                    {
                        var crap = ev.ToString().ToLowerInvariant();
                        if (crap.Contains("sequencerspecific") || crap.Contains("patchchange") || crap.Contains("controlchange") || crap.Contains("pitchwheelchange") || crap.Contains("channelaftertouch") || crap.Contains("keyaftertouch"))
                        {
                            toRemove.Add(ev);
                        }
                        else if (crap.Contains("keysignature"))
                        {
                            toRemove.Add(ev);
                            songfixes++;
                            Log("");
                            Log("Removed unsupported KeySignature event (MagmaCompiler error 0x50)");
                            Log("");

                        }
                    }
                    if (!toRemove.Any()) continue;
                    foreach (var remove in toRemove)
                    {
                        dirtyMIDI.Events[i].Remove(remove);
                    }
                }

                LogDetails("=== " + Path.GetFileNameWithoutExtension(file).ToUpper() + " ===");
                var venue = -1;
                var newDrums = new List<MidiEvent>();

                for (var i = 0; i < dirtyMIDI.Events.Tracks; i++)
                {
                    if (dirtyMIDI.Events[i][0].ToString().Contains("PART DRUMS"))
                    {                        
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_drums = new List<int> { 24, 25, 26, 27, 28, 29, 30, 31, 32, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 43, 44, 45, 46, 47, 48, 49, 50, 51, 60, 61, 62, 63, 64, 72, 73, 74, 75, 76, 84, 85, 86, 87, 88, 96, 97, 98, 99, 100, 103, 110, 111, 112, 116, 120, 121, 122, 123, 124, 126, 127 };
                        
                        long odLength = 0;
                        var doublePedal = new List<MidiEvent>();
                        long greenCymbal = 0;
                        long lastODFill = 0;

                        List<(long start, long end)> overdriveRanges = new List<(long, long)>();
                        long firstOverdriveTime = -1; // Track first Overdrive marker

                        foreach (var midiEvent in dirtyMIDI.Events[i])
                        {
                            if (midiEvent is NoteOnEvent note && note.NoteNumber == 116 && note.Velocity > 0)
                            {
                                long odStart = note.AbsoluteTime;
                                long odEnd = odStart + note.NoteLength; // Assuming odLength defines duration

                                overdriveRanges.Add((odStart, odEnd)); // Store Overdrive range
                                if (odStart < firstOverdriveTime || firstOverdriveTime == -1) // Track first overdrive appearance
                                {
                                    firstOverdriveTime = odStart;
                                }
                            }
                        }

                        if (doNotDelete2xBassPedalNotes.Checked)
                        {
                            allowed_drums.Add(95); //will make these notes be skipped during track processing
                        }

                        for (var z = 0; z < dirtyMIDI.Events[i].Count; z++)
                        {       
                            var notes = dirtyMIDI.Events[i][z];

                            switch (notes.CommandCode)
                            {
                                case MidiCommandCode.MetaEvent:
                                    {
                                        var mixevent = (MetaEvent)notes;

                                        if (!mixevent.ToString().Contains("mix") || !mixevent.ToString().Contains("drums"))
                                            continue;
                                        var index = mixevent.ToString().IndexOf("[", StringComparison.Ordinal);
                                        var old_mix = mixevent.ToString()
                                                              .Substring(index, mixevent.ToString().Length - index);
                                        var mix = old_mix.Replace("easy", ""); //old drum mix no longer used
                                        mix = mix.Replace("nokick", ""); //old drum mix no longer used
                                        mix = mix.Replace("noflip", ""); //old drum mix no longer used

                                        if (old_mix == mix) continue;
                                        dirtyMIDI.Events[i][z] = new TextEvent(mix, MetaEventType.TextEvent, mixevent.AbsoluteTime);
                                        LogDetails("PART DRUMS: drums mix " + old_mix + FormattedTime(notes.AbsoluteTime) + " is not supported");
                                        LogDetails("Changed mix to " + mix);
                                    }
                                    break;
                                case MidiCommandCode.NoteOn:
                                    {
                                        var note = (NoteOnEvent)notes;

                                        if (note.Velocity > 0) //avoid running events causing null exceptions
                                        {
                                            if (note.NoteNumber > 59 && note.NoteNumber < 101 &&
                                                note.NoteLength > (TicksPerQuarter > 480 ? 120 : TicksPerQuarter / 4))
                                            //drum notes can't be longer than 1/16th
                                            {
                                                note.NoteLength = TicksPerQuarter > 480 ? 120 : TicksPerQuarter / 4;
                                                LogDetails("PART DRUMS: note " + note.NoteNumber +
                                                           FormattedTime(notes.AbsoluteTime) +
                                                           " had a duration that was too long and was resized");
                                            }
                                        }
                                        switch (note.NoteNumber)
                                        {
                                            case 95:
                                                if (separate2xBassPedalNotes.Checked)
                                                {
                                                    doublePedal.Add(note);
                                                }
                                                break;
                                            case 100:
                                                if (note.Velocity > 0) //avoid running events causing null exceptions
                                                {
                                                    greenCymbal = note.AbsoluteTime + note.NoteLength;
                                                }
                                                break;
                                            case 101:
                                                if (moveGreenTomMarkers.Checked)
                                                {
                                                    //move the note to tom markers
                                                    note.NoteNumber = 112;

                                                    if (note.Velocity > 0)
                                                    //avoid running events causing null exceptions
                                                    {
                                                        if (note.AbsoluteTime > greenCymbal)
                                                        //meaning there isn't a cymbal note already there
                                                        {
                                                            //now add the green cymbals to go with the markers - would cause collision if chart has cymbals on green (happens)
                                                            var cymbal = new NoteOnEvent(note.AbsoluteTime,note.Channel, 100,chkVelocity.Checked ? GetVelocity() : note.Velocity, (note.NoteLength < TicksPerQuarter / 16 &&
                                                                                          chkLength.Checked) ? MinNoteLength() : note.NoteLength);
                                                            toAdd.Add(cymbal);
                                                            toAdd.Add(cymbal.OffEvent);
                                                            LogDetails("PART DRUMS: note 101" +
                                                                       FormattedTime(note.AbsoluteTime) +
                                                                       " is a GH tom marker, moved to correct place and corresponding cymbal note added");
                                                        }
                                                        else
                                                        {
                                                            LogDetails("PART DRUMS: note 101" +
                                                                       FormattedTime(note.AbsoluteTime) +
                                                                       " is a GH tom marker, moved to correct place for RB3 use");
                                                        }
                                                    }
                                                }
                                                break;
                                            case 116:
                                                if (note.Velocity > 0) //avoid running events causing null exceptions
                                                {
                                                    odLength = note.AbsoluteTime + note.NoteLength;
                                                }
                                                break;
                                            case 124:
                                            case 123:
                                            case 122:
                                            case 121:
                                            case 120:
                                                bool beforeFirstOverdrive = note.AbsoluteTime < firstOverdriveTime;
                                                bool overlapsOverdrive = false;
                                                if (note.Velocity > 0) //to avoid null exceptions due to running events
                                                {
                                                    overlapsOverdrive = overdriveRanges.Any(od =>
                                                         // Fill starts inside the overdrive range
                                                         (note.AbsoluteTime >= od.start && note.AbsoluteTime <= od.end) ||
                                                         // Fill starts before an overdrive but extends into it
                                                         (note.AbsoluteTime < od.start && note.AbsoluteTime + note.NoteLength > od.start));
                                                }

                                                if (beforeFirstOverdrive || overlapsOverdrive) // If before first OD OR overlapping OD, remove it
                                                {
                                                    toRemove.Add(note);

                                                    if (note.AbsoluteTime > lastODFill && note.Velocity > 0) // Only notify once per set of OD notes
                                                    {
                                                        string reason = beforeFirstOverdrive ? "before any overdrive marker" : "overlapping overdrive";
                                                        LogDetails($"PART DRUMS: Drum fill at {FormattedTime(note.AbsoluteTime)} {reason} is not allowed and was removed.");
                                                    }
                                                    if (note.Velocity > 0) // Avoid null exceptions
                                                    {
                                                        toRemove.Add(note.OffEvent);
                                                    }
                                                }
                                                if (note.Velocity > 0) //avoid running events causing null exceptions
                                                {
                                                    lastODFill = note.AbsoluteTime;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }

                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_drums);
                        
                        if (!doublePedal.Any() || !separate2xBassPedalNotes.Checked) continue;
                        //duplicate part drums, change track name, change expert kick notes
                        newDrums = new List<MidiEvent>(dirtyMIDI.Events[i]);

                        //remove old track name
                        newDrums.Remove(newDrums[0]);

                        //add new track name
                        newDrums.Add(new TextEvent("PART DRUMS_2X", MetaEventType.SequenceTrackName, 0));

                        //move extra kick notes from 95 (GH) to 96 (RB)
                        foreach (var note in doublePedal)
                        {
                            switch (note.CommandCode)
                            {
                                case MidiCommandCode.NoteOn:
                                {
                                    var newNote = (NoteOnEvent)note;
                                    if (newNote.Velocity == 0) continue; //avoid running events causing null exceptions

                                    newNote.NoteNumber = 96;
                                    if (chkVelocity.Checked)
                                    {
                                        newNote.Velocity = GetVelocity();
                                    }
                                    if (newNote.NoteLength < TicksPerQuarter / 16 && chkLength.Checked)
                                    {
                                        newNote.NoteLength = MinNoteLength();
                                        LogDetails("PART DRUMS_2X: note " + newNote.NoteNumber + FormattedTime(newNote.AbsoluteTime) + " had a duration that was too short and was resized");
                                    }
                                    newDrums.Add(newNote);
                                    newDrums.Add(newNote.OffEvent);
                                }
                                    break;
                            }
                        }
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("PART GUITAR") || dirtyMIDI.Events[i][0].ToString().Contains("PART BASS") || dirtyMIDI.Events[i][0].ToString().Contains("PART RHYTHM"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_guitar_bass = new List<int> { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 72, 73, 74, 75, 76, 84, 85, 86, 87, 88, 89, 90, 96, 97, 98, 99, 100, 101, 102, 103, 116, 120, 121, 122, 123, 124, 126, 127 };
                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_guitar_bass);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("PART KEYS") && !dirtyMIDI.Events[i][0].ToString().Contains("ANIM"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_keys = new List<int> { 60, 61, 62, 63, 64, 72, 73, 74, 75, 76, 84, 85, 86, 87, 88, 89, 90, 96, 97, 98, 99, 100, 101, 102, 103, 116, 120, 121, 122, 123, 124, 127 };
                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_keys);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("REAL_KEYS_"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_prokeys = new List<int> { 0, 2, 4, 5, 7, 9, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 115, 116, 126, 127 };
                        var allowed_prokeysx = new List<int> { 0, 2, 4, 5, 7, 9, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 115, 116, 120, 126, 127 };
                        ProcessTrack(dirtyMIDI.Events[i], i, dirtyMIDI.Events[i][0].ToString().Contains("X") ? allowed_prokeysx : allowed_prokeys);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("KEYS_ANIM_"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_keysanim = new List<int> { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72 };
                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_keysanim);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("BEAT"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_beat = new List<int> { 12, 13 };
                        HasBEAT = true;

                        if (endevent > 0) //this means we found [end] in EVENTS track, otherwise don't even bother
                        {
                            foreach (var note in dirtyMIDI.Events[i].Where(notes => notes.CommandCode == MidiCommandCode.NoteOn).Select(notes => (NoteOnEvent)notes).Where(note => (note.NoteNumber == 12 || note.NoteNumber == 13) && note.AbsoluteTime >= endevent))
                            {
                                toRemove.Add(note);
                                if (note.Velocity > 0) //ignore running events
                                {
                                    toRemove.Add(note.OffEvent);
                                }
                                LogDetails("BEAT: note " + note.NoteNumber + FormattedTime(note.AbsoluteTime) + " after [end] event" + FormattedTime(endevent) + " was removed");
                            }
                        }

                        var dbeat = false;
                        for (var beat = 0; beat < dirtyMIDI.Events[i].Count(); beat++)
                        {
                            if (dirtyMIDI.Events[i][beat].CommandCode != MidiCommandCode.NoteOn &&
                                dirtyMIDI.Events[i][beat].CommandCode != MidiCommandCode.NoteOff) continue;
                            var note = (NoteEvent)dirtyMIDI.Events[i][beat];

                            if (note.NoteNumber == 12 && dbeat)
                            {
                                note.NoteNumber = 13;
                                dirtyMIDI.Events[i][beat] = note;

                                if (note.Velocity > 0) //don't notify for running events (i.e. double notify)
                                {
                                    LogDetails("BEAT: Found back-to-back downbeat" + FormattedTime(note.AbsoluteTime) + " and was changed to upbeat");
                                }
                            }
                            dbeat = note.NoteNumber == 12 && note.Velocity == 0;
                        }
                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_beat);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("VENUE") && deleteVENUETracks.Checked)
                    {
                        venue = i;
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("VENUE") && !deleteVENUETracks.Checked && (!isCON || (isCON && isRBN2)))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        //all notes after 87 in this list are "not" allowed, but are removed in this code rather than later in the processtrack code to avoid double reporting
                        var allowed_venue = new List<int> { 37, 38, 39, 40, 41, 85, 86, 87, 48, 60, 61, 62, 63, 64, 70, 71, 72, 73, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110 };
                        var cameracuts = new List<CameraCuts>();
                        var camera_number = -1;
                        var old_postproc = "";
                        var new_postproc = "";
                        var directedCuts = new List<string>() { "[directed_all_lt]", "[directed_drums_lt]", "[directed_bass_cls]",
                            "[directed_guitar_cls]", "[directed_drums_kd]", "[directed_crowd]", "[directed_bass_cls]", "[directed_guitar_cls]",
                            "[directed_drums_kd]", "[directed_crowd]" };
                        var directedCutString = "";
                        long directedCutAbsTime = 0;
                        bool addedDirectedCut = true;

                        for (var z = 0; z < dirtyMIDI.Events[i].Count; z++)
                        {
                            var notes = dirtyMIDI.Events[i][z];

                            switch (notes.CommandCode)
                            {
                                case MidiCommandCode.MetaEvent:
                                    {
                                        var venue_event = (MetaEvent)notes;

                                        if (!venue_event.ToString().Contains("["))
                                        {
                                            continue;
                                        }
                                        if (venue_event.ToString().Contains("[lighting ()]") && removeLighting.Checked)
                                        {
                                            LogDetails("VENUE: Found disallowed [lighting ()] event and removed it");
                                            var log_path = Path.Combine(Path.GetDirectoryName(activeFile), "jphn.log");
                                            using (var sw = new StreamWriter(log_path, true))
                                            {
                                                sw.WriteLine($"{Path.GetFileName(activeFile)} - found disallowed [lighting ()] event and cleaned it");
                                            }
                                            toRemove.Add(notes);
                                        }
                                        var index = venue_event.ToString().IndexOf("[", StringComparison.Ordinal);
                                        var old_event = venue_event.ToString().Substring(index, venue_event.ToString().Length - index);
                                        var new_event = old_event.ToLowerInvariant().Trim();

                                        if (new_event.Contains("fog"))
                                        {
                                            LogDetails("VENUE: Stage Kit instruction" + FormattedTime(notes.AbsoluteTime) + " is not allowed and was removed");
                                            toRemove.Add(notes);
                                        }
                                        else if (directedCuts.Contains(new_event))
                                        {
                                            directedCutString = new_event;
                                            directedCutAbsTime = venue_event.AbsoluteTime;
                                            addedDirectedCut = false;
                                        }
                                        else if (isCameraEvent(new_event) && !addedDirectedCut)
                                        {
                                            toAdd.Add(new TextEvent(directedCutString, MetaEventType.TextEvent, directedCutAbsTime - 50));
                                            addedDirectedCut = true;
                                            LogDetails($"VENUE: Added directed cut '{directedCutString}'");
                                        }
                                        else if (new_event.Contains("[do_directed_cut "))
                                        {
                                            new_event = new_event.Replace("do_directed_cut ", "");
                                            new_event = new_event.Replace("[directed_vocals_cam]",
                                                                          "[directed_vocals_cam_pt]");
                                            new_event = new_event.Replace("[directed_guitar_cam]",
                                                                          "[directed_guitar_cam_pt]");
                                            new_event = new_event.Replace("[directed_drums_cam]",
                                                                          "[directed_drums_pnt]");
                                            dirtyMIDI.Events[i][z] = new TextEvent(new_event, MetaEventType.TextEvent, venue_event.AbsoluteTime);
                                            LogDetails("VENUE: cut " + old_event + FormattedTime(venue_event.AbsoluteTime) +
                                                       " is not supported and was changed to " + new_event);
                                            if (directedCuts.Contains(new_event) && addedDirectedCut)
                                            {
                                                directedCutString = new_event;
                                                directedCutAbsTime = venue_event.AbsoluteTime;
                                                addedDirectedCut = false;
                                            }
                                            else if (isCameraEvent(new_event) && !addedDirectedCut)
                                            {
                                                toAdd.Add(new TextEvent(directedCutString, MetaEventType.TextEvent, directedCutAbsTime - 50));
                                                addedDirectedCut = true;
                                                if (directedCuts.Contains(new_event))
                                                {
                                                    directedCutString = new_event;
                                                    directedCutAbsTime = venue_event.AbsoluteTime;
                                                    addedDirectedCut = false;
                                                }
                                            }
                                        }
                                        else if (new_event.Contains("[verse]"))
                                        {
                                            new_event = "[lighting (verse)]";
                                            dirtyMIDI.Events[i][z] = new TextEvent(new_event, MetaEventType.TextEvent, venue_event.AbsoluteTime);
                                            LogDetails("VENUE: lighting call " + old_event + FormattedTime(venue_event.AbsoluteTime) +
                                                       " is not supported and was changed to " + new_event);
                                        }
                                        else if (new_event.Contains("[chorus]"))
                                        {
                                            new_event = "[lighting (chorus)]";
                                            dirtyMIDI.Events[i][z] = new TextEvent(new_event, MetaEventType.TextEvent, venue_event.AbsoluteTime);
                                            LogDetails("VENUE: lighting call " + old_event + FormattedTime(venue_event.AbsoluteTime) +
                                                       " is not supported and was changed to " + new_event);
                                        }
                                        /*else if (new_event.Contains("[lighting ()]") && !removeLighting.Checked)
                                        {
                                            new_event = "[lighting (harmony)]";
                                            dirtyMIDI.Events[i][z] = new TextEvent(new_event, MetaEventType.TextEvent, venue_event.AbsoluteTime);
                                            LogDetails("VENUE: lighting call " + old_event + FormattedTime(venue_event.AbsoluteTime) +
                                                       " is not supported and was changed to " + new_event);
                                        }*/
                                        else if (new_event.Contains(".pp"))
                                        {
                                            old_postproc = new_event;
                                        }
                                    }
                                    break;
                                case MidiCommandCode.NoteOff:
                                    var noteoff = (NoteEvent)notes;
                                    switch (noteoff.NoteNumber)
                                    {
                                        case 48:
                                        case 60:
                                        case 61:
                                        case 62:
                                        case 63:
                                        case 64:
                                        case 70:
                                        case 71:
                                        case 72:
                                        case 73:
                                        case 96:
                                        case 97:
                                        case 98:
                                        case 99:
                                        case 100:
                                        case 101:
                                        case 102:
                                        case 103:
                                        case 104:
                                        case 105:
                                        case 106:
                                        case 107:
                                        case 108:
                                        case 109:
                                        case 110:
                                            toRemove.Add(noteoff);
                                            break;
                                    }
                                    break;
                                case MidiCommandCode.NoteOn:
                                    var note = (NoteOnEvent)notes;
                                    var doDirectedCut = false;

                                    if (note.NoteNumber == 48)
                                    {
                                        toRemove.Add(note);
                                        if (note.Velocity > 0) //ignore running events
                                        {
                                            toAdd.Add(new TextEvent("[next]", MetaEventType.TextEvent, note.AbsoluteTime));
                                            LogDetails("VENUE: MIDI note " + note.NoteNumber + FormattedTime(note.AbsoluteTime) + " was changed to [next] Text Event");
                                        }
                                    }
                                    else if (note.NoteNumber > 59 && note.NoteNumber < 74 && note.Velocity == 0) //running events
                                    {
                                        toRemove.Add(note);
                                    }
                                    else if (note.NoteNumber > 59 && note.NoteNumber < 74 && note.Velocity > 0)
                                    {
                                        toRemove.Add(note);
                                        switch (note.NoteNumber)
                                        {
                                            case 61:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].Bass = true;
                                                doDirectedCut = true;
                                                break;
                                            case 62:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].Drummer = true;
                                                doDirectedCut = true;
                                                break;
                                            case 63:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].Guitar = true;
                                                doDirectedCut = true;
                                                break;
                                            case 64:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].Singer = true;
                                                doDirectedCut = true;
                                                break;
                                            case 70:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].NoBehind = true;
                                                doDirectedCut = true;
                                                break;
                                            case 71:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].OnlyFar = true;
                                                doDirectedCut = true;
                                                break;
                                            case 72:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].OnlyClose = true;
                                                doDirectedCut = true;
                                                break;
                                            case 73:
                                                if (camera_number == -1) //can't use value -1 in the list
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                else if (cameracuts[camera_number].TimeStamp != note.AbsoluteTime)
                                                {
                                                    camera_number++;
                                                    cameracuts.Add(new CameraCuts());
                                                }
                                                cameracuts[camera_number].TimeStamp = note.AbsoluteTime;
                                                cameracuts[camera_number].NoClose = true;
                                                doDirectedCut = true;
                                                break;
                                        }
                                    }
                                    else if (note.NoteNumber > 95 && note.NoteNumber < 111 && note.Velocity == 0) //running events
                                    {
                                        toRemove.Add(note);
                                    }
                                    else if (note.NoteNumber > 95 && note.NoteNumber < 111 && note.Velocity > 0)
                                    {
                                        toRemove.Add(note);
                                        switch (note.NoteNumber)
                                        {
                                            case 96:
                                                new_postproc = "[ProFilm_a.pp]";
                                                break;
                                            case 97:
                                                new_postproc = "[contrast_a.pp]";
                                                break;
                                            case 98:
                                                new_postproc = "[film_16mm.pp]";
                                                break;
                                            case 99:
                                                new_postproc = "[film_sepia_ink.pp]";
                                                break;
                                            case 100:
                                                new_postproc = "[film_silvertone.pp]";
                                                break;
                                            case 101:
                                                new_postproc = "[photo_negative.pp]";
                                                break;
                                            case 102:
                                                new_postproc = "[photocopy.pp]";
                                                break;
                                            case 103:
                                                new_postproc = "[bloom.pp]";
                                                break;
                                            case 104:
                                                new_postproc = "[bright.pp]";
                                                break;
                                            case 105:
                                                new_postproc = "[ProFilm_mirror_a.pp]";
                                                break;
                                            case 106:
                                                new_postproc = "[film_blue_filter.pp]";
                                                break;
                                            case 107:
                                                new_postproc = "[video_a.pp]";
                                                break;
                                            case 108:
                                                new_postproc = "[film_b+w.pp]";
                                                break;
                                            case 109:
                                                new_postproc = "[video_security.pp]";
                                                break;
                                            case 110:
                                                new_postproc = "[clean_trails.pp]";
                                                break;
                                        }

                                        if (string.IsNullOrWhiteSpace(old_postproc) && note.AbsoluteTime > 0)
                                        {
                                            //if there is no post proc specified at 0, need to add one
                                            old_postproc = "[ProFilm_a.pp]";
                                            toAdd.Add(new TextEvent(old_postproc, MetaEventType.TextEvent, 0));
                                        }

                                        toAdd.Add(new TextEvent(string.IsNullOrWhiteSpace(old_postproc) ? new_postproc : old_postproc, MetaEventType.TextEvent, note.AbsoluteTime));
                                        toAdd.Add(new TextEvent(new_postproc, MetaEventType.TextEvent,note.AbsoluteTime + note.NoteLength));
                                        LogDetails("VENUE: post-processing note " + note.NoteNumber + FormattedTime(note.AbsoluteTime) + " was changed to " +
                                                   new_postproc + " Text Event");
                                        old_postproc = new_postproc;                                        
                                    }
                                    if (!string.IsNullOrEmpty(directedCutString) && doDirectedCut && !addedDirectedCut && note.AbsoluteTime > directedCutAbsTime)
                                    {
                                        toAdd.Add(new TextEvent(directedCutString, MetaEventType.TextEvent, note.AbsoluteTime - 50));
                                        addedDirectedCut = true;
                                        directedCutString = "";
                                        directedCutAbsTime = 0;
                                        LogDetails($"VENUE: Added directed cut '{directedCutString}'");
                                    }
                                    break;
                            }
                        }

                        foreach (var cut in cameracuts)
                        {
                            var camera = "";
                            if (cut.Singer && cut.Bass && cut.Guitar)
                            {
                                if (cut.OnlyFar)
                                {
                                    camera = "[coop_all_far]";
                                }
                                else if (cut.NoBehind || cut.OnlyClose)
                                {
                                    camera = "[coop_" + (cut.Drummer ? "all" : "front") + "_near]";
                                }
                                else if (cut.NoClose)
                                {
                                    camera = "[coop_" + (cut.Drummer ? "all" : "front") + "_behind]";
                                }
                                else
                                {
                                    camera = "[coop_" + (cut.Drummer ? "all" : "front") + "_near]";
                                    var camera2 = "[coop_" + (cut.Drummer ? "all" : "front") + "_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");

                                    if (cut.Drummer)
                                    {
                                        toAdd.Add(new TextEvent("[coop_all_far]", MetaEventType.TextEvent, cut.TimeStamp));
                                        LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to [coop_all_far] Text Event");
                                    }
                                }
                            }
                            else if (cut.Singer && cut.Bass)
                            {
                                if (cut.OnlyClose || cut.NoBehind)
                                {
                                    camera = "[coop_bv_near]";
                                }
                                else if (cut.NoClose)
                                {
                                    camera = "[coop_bv_behind]";
                                }
                                else
                                {
                                    camera = "[coop_bv_near]";
                                    const string camera2 = "[coop_bv_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Singer && cut.Guitar)
                            {
                                if (cut.OnlyClose || cut.NoBehind)
                                {
                                    camera = "[coop_gv_near]";
                                }
                                else if (cut.NoClose)
                                {
                                    camera = "[coop_gv_behind]";
                                }
                                else
                                {
                                    camera = "[coop_gv_near]";
                                    const string camera2 = "[coop_gv_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Singer && cut.Drummer)
                            {
                                camera = "[coop_dv_near]";
                            }
                            else if (cut.Bass && cut.Drummer)
                            {
                                camera = "[coop_bd_near]";
                            }
                            else if (cut.Guitar && cut.Drummer)
                            {
                                camera = "[coop_dg_near]";
                            }
                            else if (cut.Bass && cut.Guitar)
                            {
                                if (cut.OnlyClose || cut.NoBehind)
                                {
                                    camera = "[coop_bg_near]";
                                }
                                else if (cut.NoClose)
                                {
                                    camera = "[coop_bg_behind]";
                                }
                                else
                                {
                                    camera = "[coop_bg_near]";
                                    const string camera2 = "[coop_bg_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Singer)
                            {
                                if (cut.OnlyClose)
                                {
                                    camera = "[coop_v_closeup]";
                                }
                                else if (cut.NoBehind)
                                {
                                    camera = "[coop_v_near]";
                                }
                                else
                                {
                                    camera = "[coop_v_near]";
                                    const string camera2 = "[coop_v_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Guitar)
                            {
                                if (cut.OnlyClose)
                                {
                                    camera = "[coop_g_closeup_hand]";
                                }
                                else if (cut.NoBehind)
                                {
                                    camera = "[coop_g_near]";
                                }
                                else
                                {
                                    camera = "[coop_g_near]";
                                    const string camera2 = "[coop_g_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Bass)
                            {
                                if (cut.OnlyClose)
                                {
                                    camera = "[coop_b_closeup_hand]";
                                }
                                else if (cut.NoBehind)
                                {
                                    camera = "[coop_b_near]";
                                }
                                else
                                {
                                    camera = "[coop_b_near]";
                                    const string camera2 = "[coop_b_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.Drummer)
                            {
                                if (cut.OnlyClose)
                                {
                                    camera = "[coop_d_closeup_hand]";
                                }
                                else if (cut.NoBehind)
                                {
                                    camera = "[coop_d_near]";
                                }
                                else
                                {
                                    camera = "[coop_d_near]";
                                    const string camera2 = "[coop_d_behind]";
                                    toAdd.Add(new TextEvent(camera2, MetaEventType.TextEvent, cut.TimeStamp));
                                    LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera2 + " Text Event");
                                }
                            }
                            else if (cut.OnlyFar)
                            {
                                camera = "[coop_all_far]";
                            }
                            if (string.IsNullOrWhiteSpace(camera)) continue;
                            toAdd.Add(new TextEvent(camera, MetaEventType.TextEvent, cut.TimeStamp));
                            LogDetails("VENUE: converted camera cut notes" + FormattedTime(cut.TimeStamp) + " to " + camera + " Text Event");
                        }
                        ProcessTrack(dirtyMIDI.Events[i], i, allowed_venue);
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("EVENTS"))
                    {
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var midinote26 = 0;

                        for (var z = 0; z < dirtyMIDI.Events[i].Count; z++)
                        {
                            var notes = dirtyMIDI.Events[i][z];

                            switch (notes.CommandCode)
                            {
                                case MidiCommandCode.MetaEvent:
                                    {
                                        var sectionevent = (MetaEvent)notes;

                                        if (sectionevent.ToString().Contains("[section "))
                                        {
                                            var index = sectionevent.ToString().IndexOf("[", StringComparison.Ordinal);
                                            var old_section = sectionevent.ToString().Substring(index,sectionevent.ToString().Length - index);
                                            var new_section = old_section.ToLowerInvariant().Trim();
                                            new_section = new_section.Replace("section ", "prc_");
                                            new_section = new_section.Replace("guitar", "gtr");
                                            new_section = new_section.Replace("practice_outro", "outro");
                                            new_section = new_section.Replace("big_rock_ending", "bre");
                                            new_section = new_section.Replace(" ", "_");
                                            new_section = new_section.Replace("-", "");
                                            new_section = new_section.Replace("!", "");
                                            new_section = new_section.Replace("?", "");
                                            dirtyMIDI.Events[i][z] = new TextEvent(new_section, MetaEventType.TextEvent, sectionevent.AbsoluteTime);
                                            LogDetails("EVENTS: practice section " + old_section + FormattedTime(sectionevent.AbsoluteTime) +
                                                       " is not supported and was changed to " + new_section);
                                        }
                                        else if (sectionevent.ToString().Contains("[end]"))
                                        {
                                            endevent = sectionevent.AbsoluteTime;
                                        }
                                        else if (sectionevent.ToString().Contains("music_start") && sectionevent.AbsoluteTime < TicksPerQuarter * 4)
                                        {
                                            var division = 4;
                                            long[] last_sig = {0};
                                            foreach (var signature in TimeSignatures.Where(signature => signature.AbsoluteTime <= sectionevent.AbsoluteTime && signature.AbsoluteTime >= last_sig[0]))
                                            {
                                                division = signature.Denominator;
                                                last_sig[0] = signature.AbsoluteTime;
                                            }
                                            var beat_length = TicksPerQuarter / (division / 4);
                                            if (sectionevent.AbsoluteTime >= beat_length * 2) continue;

                                            dirtyMIDI.Events[i][z] = new TextEvent("[music_start]", MetaEventType.TextEvent, beat_length * 2);
                                            LogDetails("EVENTS: [music_start] event found earlier than two beats from the start of the song and was moved");
                                        }
                                        else if (sectionevent.MetaEventType == MetaEventType.Lyric && sectionevent.ToString().Contains("["))
                                        {
                                            var index = sectionevent.ToString().IndexOf("[", StringComparison.Ordinal);
                                            var oldevent = sectionevent.ToString().Substring(index, sectionevent.ToString().Length - index);
                                            toAdd.Add(new TextEvent(oldevent, MetaEventType.TextEvent, sectionevent.AbsoluteTime));
                                            toRemove.Add(notes);
                                            LogDetails("EVENTS: event " + oldevent + " was set as Lyric Event, changed to Text Event");
                                        }
                                    }
                                    break;
                                case MidiCommandCode.NoteOn:
                                    var note = (NoteOnEvent)notes;

                                    if (note.NoteNumber == 26 && note.Velocity > 0)
                                    {
                                        midinote26++;
                                    }
                                    else if (note.Velocity > 0) //don't count note-offs
                                    {
                                        LogDetails("EVENTS: MIDI note " + ((NoteOnEvent)notes).NoteNumber + FormattedTime(notes.AbsoluteTime) + " is not allowed and was removed");
                                    }
                                    toRemove.Add(notes); //can't have midi notes in EVENTS track
                                    break;
                                case MidiCommandCode.NoteOff:
                                    toRemove.Add(notes); //can't have midi notes in EVENTS track
                                    break;
                            }
                        }

                        if (midinote26 > 0)
                        {
                            LogDetails("EVENTS: MIDI note 26 is not allowed, found and removed " + midinote26 + " instances");
                            eventsfixes = eventsfixes + midinote26 - 1; //the logdetail above would be adding an unnecessary 1
                            songfixes = songfixes + midinote26 - 1;
                        }
                        if (toRemove.Any())
                        {
                            foreach (var notes in toRemove)
                            {
                                dirtyMIDI.Events[i].Remove(notes);
                            }
                        }
                        if (!toAdd.Any()) continue;
                        foreach (var notes in toAdd)
                        {
                            dirtyMIDI.Events[i].Add(notes);
                        }
                    }
                    else if (dirtyMIDI.Events[i][0].ToString().Contains("HARM") || dirtyMIDI.Events[i][0].ToString().Contains("VOCALS"))
                    {
                        var track = Tools.GetMidiTrackName(dirtyMIDI.Events[i][0].ToString());
                        toRemove = new List<MidiEvent>();
                        toAdd = new List<MidiEvent>();
                        var allowed_vocals = new List<int> { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 96, 97, 105, 116 };
                        var allowed_harm1 = new List<int> { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 96, 97, 105, 116 };
                        var allowed_harm2 = new List<int> { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 105 };
                        var allowed_harm3 = new List<int> { 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84 };
                        var phrasemarkers = new List<PhraseMarkers>();
                        var markers = -1;

                        for (var z = 0; z < dirtyMIDI.Events[i].Count; z++)
                        {
                            var note = dirtyMIDI.Events[i][z];
                            switch (note.CommandCode)
                            {
                                case MidiCommandCode.MetaEvent:
                                    {
                                        var vocal_event = (MetaEvent)note;
                                        if (vocal_event.ToString().Contains("range") && vocal_event.ToString().Contains("shift"))
                                        {
                                            var RangeMarker = new NoteOnEvent(vocal_event.AbsoluteTime, 1, 0, chkVelocity.Checked ? GetVelocity() : 96, TicksPerQuarter / 2);
                                            toAdd.Add(RangeMarker);
                                            toAdd.Add(RangeMarker.OffEvent);
                                            toRemove.Add(note);
                                            LogDetails(track + ": Found old format [range_shift] event" + FormattedTime(vocal_event.AbsoluteTime) + " and changed it to new range shift note");
                                        }
                                        else if (vocal_event.ToString().Contains("lyric") && vocal_event.ToString().Contains("shift"))
                                        {
                                            var RangeMarker = new NoteOnEvent(vocal_event.AbsoluteTime, 1, 1, chkVelocity.Checked ? GetVelocity() : 96, TicksPerQuarter / 2);
                                            toAdd.Add(RangeMarker);
                                            toAdd.Add(RangeMarker.OffEvent);
                                            toRemove.Add(note);
                                            LogDetails(track + ": Found old format [lyric_shift] event" + FormattedTime(vocal_event.AbsoluteTime) + " and changed it to new lyric shift note");
                                        }
                                        else if (!vocal_event.ToString().Contains("[") && vocal_event.MetaEventType == MetaEventType.TextEvent)
                                        {
                                            var lyric = GetCleanMIDILyric(vocal_event.ToString(), track, vocal_event.AbsoluteTime);
                                            if (lyric != "")
                                            {
                                                dirtyMIDI.Events[i][z] = new TextEvent(lyric, MetaEventType.Lyric,
                                                                                       vocal_event.AbsoluteTime);
                                                LogDetails(track + ": Found lyric '" + lyric + "' in a Text Event" + FormattedTime(vocal_event.AbsoluteTime) + " and changed it to Lyric Event");
                                            }
                                        }
                                        else switch (vocal_event.MetaEventType)
                                            {
                                                case MetaEventType.Lyric:
                                                    {
                                                        var lyric = GetCleanMIDILyric(vocal_event.ToString(), track, vocal_event.AbsoluteTime);
                                                        if (lyric != "" && lyric != "+") //no need to do anything here
                                                        {
                                                            dirtyMIDI.Events[i][z] = new TextEvent(lyric, MetaEventType.Lyric, vocal_event.AbsoluteTime);
                                                        }
                                                    }
                                                    break;
                                                case MetaEventType.SequenceTrackName:
                                                    if (vocal_event.ToString().Contains("PART HARM")) //old format
                                                    {
                                                        var oldname = Tools.GetMidiTrackName(vocal_event.ToString());
                                                        var newname = oldname.Replace("PART ", "");
                                                        dirtyMIDI.Events[i][z] = new TextEvent(newname, MetaEventType.SequenceTrackName, 0);
                                                        LogDetails("Renamed track name from old format '" + oldname + "' to '" + newname + "'");
                                                    }
                                                    break;
                                            }
                                    }
                                    break;
                                case MidiCommandCode.NoteOn:
                                    {
                                        var notes = (NoteOnEvent)note;
                                        if (notes.Velocity == 0) continue;  //avoid running events causing null exceptions

                                        switch (notes.NoteNumber)
                                        {
                                            case 0:
                                            case 1:
                                                if (!ignoreMIDINotes0And1.Checked)
                                                {
                                                    LogDetails(track + ": note " + notes.NoteNumber + FormattedTime(note.AbsoluteTime) + " is a tonic note indicator");
                                                    LogDetails("This method of specifying tonic note value is not supported in RBN 2.0, so it will be removed");
                                                    LogDetails("Use Magma: Rok On Edition and enter tonic note value " + notes.NoteNumber);
                                                }
                                                break;
                                            case 2:
                                            case 3:
                                            case 4:
                                            case 5:
                                            case 6:
                                            case 7:
                                            case 8:
                                            case 9:
                                            case 10:
                                            case 11:
                                                LogDetails(track + ": note " + notes.NoteNumber + FormattedTime(note.AbsoluteTime) + " is a tonic note indicator");
                                                LogDetails("This method of specifying tonic note value is not supported in RBN 2.0, so it will be removed");
                                                LogDetails("Use Magma: Rok On Edition and enter tonic note value " + notes.NoteNumber);
                                                break;
                                            case 105:
                                                markers++;
                                                phrasemarkers.Add(new PhraseMarkers());
                                                phrasemarkers[markers].StartTime = notes.AbsoluteTime;
                                                phrasemarkers[markers].EndTime = notes.AbsoluteTime +
                                                                                 notes.NoteLength;
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }

                        //check which of the player sections at note 106 can be moved to phrase markers
                        //checks if there is overlap, in which case we don't move, if no overlap, move down to phrase marker
                        foreach (var notes in dirtyMIDI.Events[i])
                        {
                            var overlap = false;
                            switch (notes.CommandCode)
                            {
                                case MidiCommandCode.NoteOn:
                                    var note = (NoteOnEvent)notes;
                                    if (note.Velocity == 0) continue;  //avoid running events causing null exceptions

                                    if (note.NoteNumber == 106)
                                    {
                                        foreach (var phrasemarker in phrasemarkers)
                                        {
                                            if (phrasemarker.StartTime > note.AbsoluteTime &&
                                                phrasemarker.StartTime < note.AbsoluteTime + note.NoteLength)
                                            {
                                                overlap = true;
                                                break;
                                            }
                                            if (phrasemarker.EndTime > note.AbsoluteTime &&
                                                phrasemarker.EndTime < note.AbsoluteTime + note.NoteLength)
                                            {
                                                overlap = true;
                                                break;
                                            }
                                            if (phrasemarker.StartTime != note.AbsoluteTime &&
                                                phrasemarker.EndTime != note.AbsoluteTime + note.NoteLength)
                                                continue;
                                            overlap = true;
                                            break;
                                        }
                                        if (!overlap)
                                        {
                                            note.NoteNumber = 105;
                                            LogDetails(track + ": player section marker" + FormattedTime(note.AbsoluteTime) + " was changed to phrase marker");
                                        }
                                    }
                                    break;
                            }
                        }

                        if (((track.Contains("HARM1") || track.Contains("VOCALS")) && !dONOTFixCapitalizationErrorsHarm1.Checked) || (track.Contains("HARM2") && !dONOTFixCapitalizationErrorsHarm2.Checked)) //not harm3 because of weird phrases
                        {
                            //now that the HARM/VOCALS charts are otherwise cleaned up, let's run through again for one more thing
                            var newphrase = false;
                            long phrase = 0;
                            var notefirst = false; //for when the note and the phrase marker start at the same exact time

                            for (var z = 0; z < dirtyMIDI.Events[i].Count; z++)
                            {
                                var notes = dirtyMIDI.Events[i][z];
                                switch (notes.CommandCode)
                                {
                                    case MidiCommandCode.NoteOn:
                                        var note = (NoteOnEvent)notes;
                                        if (note.Velocity == 0) continue;  //avoid running events causing null exceptions

                                        if (note.NoteNumber == 105)
                                        {
                                            if (!notefirst)
                                            {
                                                newphrase = true; //mark the start of a new vocal phrase
                                            }
                                            notefirst = false;
                                            phrase = note.OffEvent.AbsoluteTime;
                                        }
                                        break;
                                    case MidiCommandCode.MetaEvent:
                                        var vocal_event = (MetaEvent)notes;
                                        if (vocal_event.MetaEventType != MetaEventType.Lyric) continue;
                                        if (vocal_event.AbsoluteTime > phrase || newphrase) //this is the first lyric after the end of last phrase marker
                                        {
                                            notefirst = vocal_event.AbsoluteTime > phrase;
                                            newphrase = false;
                                            var lyric = GetCleanMIDILyric(vocal_event.ToString(), track, vocal_event.AbsoluteTime);

                                            if (!string.IsNullOrWhiteSpace(lyric.Trim()) && lyric != "+")
                                            {
                                                var newlyric = lyric.Length > 1 ? lyric.Substring(0, 1).ToUpper() + lyric.Substring(1, lyric.Length - 1) : lyric.ToUpper();
                                                if (lyric != newlyric)
                                                {
                                                    LogDetails(track + ": lyric '" + lyric + "'" + FormattedTime(notes.AbsoluteTime) + " at start of phrase was not capitalized, changed to '" + newlyric + "'");
                                                    dirtyMIDI.Events[i][z] = new TextEvent(newlyric, MetaEventType.Lyric, vocal_event.AbsoluteTime);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }

                        //by adding 0 and 1 to the arrays, they won't be deleted when processing the tracks
                        if (ignoreMIDINotes0And1.Checked)
                        {
                            allowed_vocals.Add(0);
                            allowed_vocals.Add(1);
                            allowed_harm1.Add(0);
                            allowed_harm2.Add(0);
                            allowed_harm3.Add(0);
                            allowed_harm1.Add(1);
                            allowed_harm2.Add(1);
                            allowed_harm3.Add(1);
                        }

                        if (track.Contains("VOCALS"))
                        {
                            ProcessTrack(dirtyMIDI.Events[i], i, allowed_vocals, true);
                        }
                        else if (track.Contains("HARM1"))
                        {
                            ProcessTrack(dirtyMIDI.Events[i], i, allowed_harm1, true);
                        }
                        else if (track.Contains("HARM2"))
                        {
                            ProcessTrack(dirtyMIDI.Events[i], i, allowed_harm2, true);
                        }
                        else if (track.Contains("HARM3"))
                        {
                            ProcessTrack(dirtyMIDI.Events[i], i, allowed_harm3, true);
                        }
                    }
                }

                if (newDrums.Any() && separate2xBassPedalNotes.Checked && !isCON)
                {
                    dirtyMIDI.Events.AddTrack(newDrums);
                    LogDetails("PART DRUMS_2X: track created based on found Expert+ GH drum notes");
                }

                if (venue > -1 && deleteVENUETracks.Checked && !isCON)
                {
                    dirtyMIDI.Events.RemoveTrack(venue);
                    LogDetails("Found and removed VENUE track as instructed");
                }

                if (createBEATTracks.Checked && !HasBEAT)
                {
                    var beattrack = new List<MidiEvent> { new TextEvent("BEAT", MetaEventType.SequenceTrackName, 0) };
                    var time_sig = 4; //default
                    var division = 4;

                    //if no [end] event is found in the EVENTS track, find the very last event in the MIDI track
                    //and run the BEAT track until that point
                    if (endevent < TicksPerQuarter)
                    {
                        for (var t = 1; t < dirtyMIDI.Events.Tracks; t++) //start at 1 to skip tempo map
                        {
                            var lastevent = dirtyMIDI.Events[t][dirtyMIDI.Events[t].Count - 1].AbsoluteTime;
                            if (lastevent > endevent)
                            {
                                endevent = lastevent;
                            }
                        }
                    }

                    long i = 0;
                    while (i < endevent)
                    {
                        long[] last_sig = {0};
                        var i1 = i;
                        foreach (var signature in TimeSignatures.Where(signature => signature.AbsoluteTime <= i1 && signature.AbsoluteTime >= last_sig[0]))
                        {
                            time_sig = signature.Numerator;
                            division = signature.Denominator;
                            last_sig[0] = signature.AbsoluteTime;
                        }

                        //if 1/x time signature, make it an upbeat, rather than a downbeat
                        var dbeat = new NoteOnEvent(i, 1, time_sig > 1 ? 12 : 13, chkVelocity.Checked ? GetVelocity() : 96, TicksPerQuarter / division);
                        beattrack.Add(dbeat);
                        beattrack.Add(dbeat.OffEvent);

                        for (var c = 2; c <= time_sig; c++)
                        {
                            var ubeat = new NoteOnEvent(i + ((TicksPerQuarter / (division / 4)) * (c - 1)), 1, 13, chkVelocity.Checked ? GetVelocity() : 96, TicksPerQuarter / division);
                            beattrack.Add(ubeat);
                            beattrack.Add(ubeat.OffEvent);
                        }

                        i += ((TicksPerQuarter / (division / 4)) * time_sig);
                    }

                    dirtyMIDI.Events.AddTrack(beattrack);
                    LogDetails("Created BEAT track as instructed");
                }

                if (chkVelocity.Checked && !isCON)
                {
                    if (songfixes > 0)
                    {
                        LogDetails("All note velocities were changed to " + GetVelocity() + " as instructed");
                    }
                    else
                    {
                        Log("All note velocities were changed to " + GetVelocity() + " as instructed");
                    }
                }

                if (songfixes > 0 || (chkVelocity.Checked && !isCON))
                {
                    //MagmaCompiler won't accept beyond 480, so let's change it,
                    //Have to change the location of every event too to keep the same overall length
                    if (TicksPerQuarter > 480)
                    {
                        //create new midi collection with acceptable ticks per quarter value
                        var goodMIDI = new MidiEventCollection(dirtyMIDI.FileFormat, 480);

                        //change the location of all events in old midi
                        var multiplier = (Double)TicksPerQuarter / 480;
                        for (var i = 0; i < dirtyMIDI.Tracks; i++)
                        {
                            foreach (var note in dirtyMIDI.Events[i])
                            {
                                note.AbsoluteTime = (long)(note.AbsoluteTime / multiplier);
                            }
                        }

                        //now add all those to the new (good) midi
                        foreach (var track in dirtyMIDI.Events)
                        {
                            goodMIDI.AddTrack(track);
                        }
                        LogDetails("MIDI: " + Path.GetFileName(file) + " had an unsupported ticks per quarter note value of " + TicksPerQuarter 
                            + " and was changed to 480");
                        MidiFile.Export(cleanMIDI, goodMIDI);
                    }
                    else
                    {
                        MidiFile.Export(cleanMIDI, dirtyMIDI.Events);
                    }
                }

                //delete temporary midi file created by midishrink
                Tools.DeleteFile(fixedmidi);

                if (songfixes > 0)
                {
                    LogDetails("", false, true);
                    Log("Found and cleaned " + songfixes + (songfixes > 1 ? " issues..." : " issue..."));
                    DoEspherReport();
                    Log("Cleaned MIDI file '" + Path.GetFileName(file) + "' successfully");
                    Log("Cleaned MIDI file saved to '" + cleanMIDI + "'");
                    totalfixes += songfixes;
                }
                else
                {
                    Log("Done processing MIDI file '" + Path.GetFileName(file) + "' but I found nothing to clean!");
                }
                workingcounter++;
            }
            catch (Exception ex)
            {
                Log("Error cleaning MIDI file '" + Path.GetFileName(file) + "'");
                Log("The error says: " + ex.Message);
                MessageBox.Show("I can't work with that MIDI file!\n\nError: '" + ex.Message + "'\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                problem_midis.Add(Path.GetFileName(file));
            }
        }

        private string GetMIDIFromCON(string file)
        {
            isRBN2 = false; //reset from previous use, only enable if found in this DTA

            if (!Parser.ExtractDTA(xPackage, false))
            {
                Log("Something went wrong extracting the songs.dta file from '" + Path.GetFileName(file) + "' ... aborting");
                return "";
            }
            if (!Parser.ReadDTA(Parser.DTA) || !Parser.Songs.Any())
            {
                Log("Something went wrong reading the songs.dta file from '" + Path.GetFileName(file) + "' ... aborting");
                return "";
            }
            if (Parser.Songs.Count > 1)
            {
                Log("It looks like file '" + Path.GetFileName(file) + "' is a pack ... try a single song, please");
                return "";
            }

            var songname = Parser.Songs[0].InternalName;
            isRBN2 = Parser.Songs[0].Source == "ugc_plus";

            if (string.IsNullOrWhiteSpace(songname))
            {
                Log("Could not find MIDI file inside STFS file '" + Path.GetFileName(file) + "' ... aborting");
                return "";
            }

            var xFile = xPackage.GetFile("songs/" + songname + "/" + songname + ".mid");
            if (xFile == null) return "";
            var midi = Path.GetTempPath() + songname + ".mid";
            Tools.DeleteFile(midi);
            if (xFile.ExtractToFile(midi)) return midi;
            Log("Could not extract MIDI file from STFS file '" + Path.GetFileName(file) + "' ... aborting");
            midi = "";
            return midi;
        }

        private void CleanCON(string file)
        {
            Log("Processing STFS file '" + Path.GetFileName(file) + "'");
            xPackage = new STFSPackage(file);

            if (!xPackage.ParseSuccess)
            {
                Log("Error parsing CON file '" + Path.GetFileName(file) + "' - MIDI not cleaned");
                xPackage.CloseIO();
                return;
            }

            var midi = GetMIDIFromCON(file);
            if (string.IsNullOrWhiteSpace(midi) || !File.Exists(midi))
            {
                xPackage.CloseIO();
                return;
            }

            Log("Extracted MIDI file " + Path.GetFileName(midi) + " successfully");
            CleanMIDI(midi);
            
            var newmidi = midi.Replace(".mid", "_clean.mid");

            if (lstLog.Items[lstLog.Items.Count - 1].ToString().Contains("nothing to clean"))
            {
                xPackage.CloseIO();
                Log("Nothing to clean ... leaving STFS file '" + Path.GetFileName(file) + "' unmodified");
                Tools.DeleteFile(midi);
                Tools.DeleteFile(newmidi);
                workingcounter--;
                return;
            }
            
            if (!File.Exists(newmidi))
            {
                Log("Cleaning of MIDI file from CON file '" + Path.GetFileName(file) + "' failed");
                xPackage.CloseIO();
                Tools.DeleteFile(midi);
                return;
            }

            if (backUpFileWhenCleaning.Checked)
            {
                xPackage.CloseIO();
                var newcon = file + "_clean";
                Tools.DeleteFile(newcon);
                File.Copy(file, newcon);
                xPackage = new STFSPackage(newcon);
                if (!xPackage.ParseSuccess)
                {
                    Log("Error parsing backup CON file '" + Path.GetFileName(file) + "' - MIDI not cleaned");
                    xPackage.CloseIO();
                    return;
                }
                file = newcon;
            }

            var internalname = Path.GetFileNameWithoutExtension(midi);
            var xent = xPackage.GetFile("/songs/" + internalname + "/" + internalname + ".mid");
            if (xent == null)
            {
                xPackage.CloseIO();
                Log("Error replacing old MIDI file with cleaned file");
                Tools.DeleteFile(midi);
                Tools.DeleteFile(newmidi);
                return;
            }

            if (!xent.Replace(newmidi))
            {
                xPackage.CloseIO();
                Log("Error replacing old MIDI file with cleaned file");
                Tools.DeleteFile(midi);
                Tools.DeleteFile(newmidi);
                return;
            }

            if (FinalizeCON(file))
            {
                Log("Cleaned MIDI in CON file '" + Path.GetFileName(file) + "' successfully");
            }
            else
            {
                Log("Something went wrong when finalizing changes to CON file '" + Path.GetFileName(file) + "'");
                Log("Check the file with Song Explorer for any possible problems");
            }

            //clean up midis
            Tools.DeleteFile(midi);
            Tools.DeleteFile(newmidi);
        }

        private bool FinalizeCON(string file)
        {
            xPackage.Header.ThisType = PackageType.SavedGame;
            xPackage.Header.MakeAnonymous();
            
            try
            {
                Log("Saving changes to CON file '" + Path.GetFileName(file) + "' ... sit tight");
                xsignature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                xPackage.RebuildPackage(xsignature);
                xPackage.FlushPackage(xsignature);
                xPackage.CloseIO();
            }
            catch (Exception ex)
            {
                Log("There was an error: " + ex.Message);
                xPackage.CloseIO();
                return false;
            }

            var success = Tools.UnlockCON(file);
            if (success)
            {
                success = Tools.SignCON(file);
            }
            return success;
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            workingcounter = 0;
            startTime = DateTime.Now;
            
            problem_midis = new List<String>();
            fixedmidi = "";
            
            EnableDisable(false);
            backgroundWorker1.RunWorkerAsync();
        }

        private void EnableDisable(bool enabled)
        {
            btnBegin.Enabled = enabled;
            btnOpen.Enabled = enabled;
            menuStrip1.Enabled = enabled;
            chkLength.Enabled = enabled;
            cboLength.Enabled = enabled;
            chkVelocity.Enabled = enabled;
            numVelocity.Enabled = enabled;
            picWorking.Visible = !enabled;
            lstLog.Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
            Cursor = lstLog.Cursor;
        }

        private void DoEspherReport()
        {
            if (!breakDownIssueCountByTrackToolStripMenuItem.Checked || !detailedLoggingToolStripMenuItem.Checked) return;

            //Log("Here is the Espher Breakdown of Issues (patent-pending):");
            Log("DRUMS - " + (drumfixes > 0 ? drumfixes + " " + (drumfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("BASS - " + (bassfixes > 0 ? bassfixes + " " + (bassfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("GUITAR - " + (guitarfixes > 0 ? guitarfixes + " " + (guitarfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("VOCALS - " + (vocalsfixes > 0 ? vocalsfixes + " " + (vocalsfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("HARM1 - " + (harm1fixes > 0 ? harm1fixes + " " + (harm1fixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("HARM2 - " + (harm2fixes > 0 ? harm2fixes + " " + (harm2fixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("HARM3 - " + (harm3fixes > 0 ? harm3fixes + " " + (harm3fixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("KEYS - " + (keysfixes > 0 ? keysfixes + " " + (keysfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("PRO KEYS - " + (prokeysfixes > 0 ? prokeysfixes + " " + (prokeysfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("EVENTS - " + (eventsfixes > 0 ? eventsfixes + " " + (eventsfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("VENUE - " + (venuefixes > 0 ? venuefixes + " " + (venuefixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));
            Log("BEAT - " + (beatfixes > 0 ? beatfixes + " " + (beatfixes > 1 ? "issues" : "issue") + " cleaned" : "no issues found"));

            var other = songfixes - drumfixes - bassfixes - guitarfixes - vocalsfixes - harm1fixes - harm2fixes -
                        harm3fixes - eventsfixes - venuefixes - beatfixes - keysfixes - prokeysfixes;
            if (other > 0)
            {
                Log("OTHER / MISC - " + other + " " + (other > 1 ? "issues" : "issue") + " cleaned");
            }
        }

        private void ProcessTrack(IList<MidiEvent> track, int midiTrackNumber, ICollection<int> allowed_notes, bool allow_lyrics = false)
        {
            var trackName = Tools.GetMidiTrackName(track[0].ToString()) + ": ";
            if (trackName.Contains("-")) 
            {
                //if track name is moved, track name is added automatically but includes " - filename", let's remove that
                var index = trackName.IndexOf("-", StringComparison.Ordinal);
                var newname = trackName.Substring(0, index);
                trackName = newname.Trim() + ": ";
            }
            
            foreach (var notes in track)
            {
                switch (notes.CommandCode)
                {
                    case MidiCommandCode.MetaEvent:
                        {
                            var mixEvent = (MetaEvent)notes;

                            if (!mixEvent.ToString().Contains("[") && mixEvent.MetaEventType != MetaEventType.SequenceTrackName 
                                && mixEvent.MetaEventType != MetaEventType.EndTrack)
                            {
                                if (!allow_lyrics || (mixEvent.MetaEventType != MetaEventType.Lyric))
                                {
                                    if (mixEvent.MetaEventType != MetaEventType.TextEvent ||
                                        (!trackName.Contains("VOCALS") && !trackName.Contains("HARM1") 
                                        && !trackName.Contains("HARM2") && !trackName.Contains("HARM3")))
                                    {
                                        toRemove.Add(notes);
                                            //only track name and animation events with [ ] are supported
                                        if (!mixEvent.ToString().Contains("TimeSignature")) //this will just confuse people
                                        {
                                            LogDetails(trackName + "event " + mixEvent + FormattedTime(mixEvent.AbsoluteTime) + " is not allowed and was removed");
                                        }
                                    }
                                }
                            }
                            else if (mixEvent.MetaEventType == MetaEventType.Lyric && !allow_lyrics && mixEvent.ToString().Contains("["))
                            {
                                var index = mixEvent.ToString().IndexOf("[", StringComparison.Ordinal);
                                var oldEvent = mixEvent.ToString().Substring(index, mixEvent.ToString().Length -index);
                                toAdd.Add(new TextEvent(oldEvent, MetaEventType.TextEvent, mixEvent.AbsoluteTime));
                                toRemove.Add(notes);

                                LogDetails(trackName + "event " + oldEvent + FormattedTime(mixEvent.AbsoluteTime) + " was set as Lyric Event, changed to Text Event");
                            }
                            else if (mixEvent.MetaEventType == MetaEventType.TextEvent && mixEvent.AbsoluteTime < TicksPerQuarter * 4 &&
                                     (mixEvent.ToString().Contains("idle") || mixEvent.ToString().Contains("play") ||
                                      mixEvent.ToString().Contains("intense") || mixEvent.ToString().Contains("mellow")))
                            {
                                var division = 4;
                                long[] last_sig = {0};
                                foreach (var signature in TimeSignatures.Where(signature => signature.AbsoluteTime <= mixEvent.AbsoluteTime && signature.AbsoluteTime >= last_sig[0]))
                                {
                                    division = signature.Denominator;
                                    last_sig[0] = signature.AbsoluteTime;
                                }
                                var beat_length = TicksPerQuarter/(division/4);
                                if (mixEvent.AbsoluteTime >= beat_length*2) continue;
                                
                                var index = mixEvent.ToString().IndexOf("[", StringComparison.Ordinal);
                                var oldevent = mixEvent.ToString().Substring(index, mixEvent.ToString().Length - index);
                                toAdd.Add(new TextEvent(oldevent, MetaEventType.TextEvent, beat_length * 2));
                                toRemove.Add(notes);
                                LogDetails(trackName + "event " + mixEvent + FormattedTime(mixEvent.AbsoluteTime) + " occurred too early in the track and was moved");
                            }
                            else if (mixEvent.MetaEventType == MetaEventType.SequenceTrackName &&
                                     mixEvent.AbsoluteTime != 0) //move to 1.1.00 / 0ms
                            {
                                LogDetails(trackName + "track name found" + FormattedTime(mixEvent.AbsoluteTime) + " was moved to [1.1.00] / 0 ms");
                                if (track[0].CommandCode == MidiCommandCode.MetaEvent)
                                {
                                    //if user put track name later than 1.1.00/0ms, another track name is added automatically, let's remove it
                                    var name = (MetaEvent) track[0];
                                    if (name.MetaEventType == MetaEventType.SequenceTrackName)
                                    {
                                        toRemove.Add(track[0]);
                                    }
                                }
                                mixEvent.AbsoluteTime = 0;
                            }
                        }
                        break;
                    case MidiCommandCode.NoteOn:
                        {
                            var note = (NoteOnEvent)notes;
                            if (!allowed_notes.Contains(note.NoteNumber))
                            {
                                toRemove.Add(notes);
                                if (note.Velocity > 0) //otherwise you get double-reporting for NoteOffs using running status
                                {
                                    LogDetails(trackName + "note " + note.NoteNumber + FormattedTime(notes.AbsoluteTime) + " is not allowed in this track and was removed");
                                }
                            }
                            else if (note.Velocity > 0)
                            {
                                //velocity check is to circumvent problematic MIDIs with NoteOffs reporting as NoteOns (vel = 0)
                                //this is a known bug with NAudio and running events, markheath is working on it

                                //if note length is less than 1/64th
                                try
                                {
                                    if (note.NoteLength < (TicksPerQuarter / 16) && !allow_lyrics && chkLength.Checked)
                                    {
                                        note.NoteLength = MinNoteLength();
                                        LogDetails(trackName + "note " + note.NoteNumber + FormattedTime(note.AbsoluteTime) + " had a duration that was too short and was resized");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log("Error fixing note: " + note);
                                    Log("Error: " + ex.Message);
                                    Log("Stack trace: " + ex.StackTrace);
                                    Log("Skipping that note...check the final MIDI for possible issues");
                                }
                            }
                            if (chkVelocity.Checked && note.Velocity != GetVelocity() && note.Velocity > 0) //avoid running event problems, see above
                            {
                                note.Velocity = GetVelocity();
                            }
                        }
                        break;
                    case MidiCommandCode.NoteOff:
                        {
                            var note = (NoteEvent)notes;

                            if (!allowed_notes.Contains(note.NoteNumber))
                            {
                                toRemove.Add(notes);
                            }
                        }
                        break;
                    case MidiCommandCode.Sysex:
                        if (!leaveSysEx.Checked)
                        {
                            toRemove.Add(notes); //events from Phase Shift, etc, not allowed
                            LogDetails(trackName + notes.CommandCode + " event" + FormattedTime(notes.AbsoluteTime) + " is not supported and was removed");
                        }
                        break;
                    case MidiCommandCode.PatchChange:
                    case MidiCommandCode.ChannelAfterTouch:
                    case MidiCommandCode.ControlChange:
                    case MidiCommandCode.KeyAfterTouch:
                    case MidiCommandCode.PitchWheelChange:
                        toRemove.Add(notes); //events from Phase Shift, etc, not allowed
                        LogDetails(trackName + notes.CommandCode + " event" + FormattedTime(notes.AbsoluteTime) + " is not supported and was removed");
                        break;
                }
            }

            if (toRemove.Any() && midiTrackNumber > -1)
            {
                foreach (var notes in toRemove)
                {
                    dirtyMIDI.Events[midiTrackNumber].Remove(notes);
                }
            }
            if (!toAdd.Any() || midiTrackNumber < 0) return;
            foreach (var notes in toAdd)
            {
                dirtyMIDI.Events[midiTrackNumber].Add(notes);
            }
        }

        private int MinNoteLength()
        {
            //we don't want FoF style 0-length notes
            //only when it's not vocals/harmonies

            var index = 0;
            if (cboLength.InvokeRequired)
            {
                cboLength.Invoke(new MethodInvoker(() => index = cboLength.SelectedIndex));
            }
            else
            {
                index = cboLength.SelectedIndex;
            }

            switch (index)
            {
                case 0:
                    return TicksPerQuarter / 4; // 1/16
                case 1:
                    return TicksPerQuarter / 8; // 1/32
                case 2:
                    return TicksPerQuarter / 16; // 1/64
                default:
                    return TicksPerQuarter/8;
            }
        }

        private string GetRealtime(long absdelta)
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
            return (Math.Round(time / 1000, 3) + " seconds");
        }

        private string FormattedTime(long absoluteTime)
        {
            if (reportUsingMeasureBeatTicksToolStripMenuItem.Checked)
            {
                return " at " + GetMBT(absoluteTime);
            }
            return " at " + GetRealtime(absoluteTime);
        }

        private string GetCleanMIDILyric(string raw_event,string trackname,long absolutetime)
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
                string[] lyric = {old_lyric.Replace(",", "")};
                lyric[0] = lyric[0].Replace("\"", "");
                lyric[0] = lyric[0].Replace(".", "").Trim();
                
                var spec_chars = new List<string> {"#", "^", "$"};
                foreach (var special in spec_chars.Where(special => lyric[0].Contains(special)))
                {
                    lyric[0] = lyric[0].Replace(special, "").Trim() + special;
                }
                
                if (lyric[0] != old_lyric)
                {
                    LogDetails(trackname + ": lyric" + FormattedTime(absolutetime) + " was cleaned from '" + old_lyric +
                               "' to '" + lyric[0] + "'");
                }
                return lyric[0];
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void chkVelocity_CheckedChanged(object sender, EventArgs e)
        {
            numVelocity.Enabled = chkVelocity.Checked;

            SaveOptions(sender,e);
        }

        private void chkLength_CheckedChanged(object sender, EventArgs e)
        {
            cboLength.Enabled = chkLength.Checked;

            SaveOptions(sender, e);
        }

        private void MIDICleaner_Shown(object sender, EventArgs e)
        {
            LogDefaults();
            if (argument != "" && (argument.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal) || 
                VariousFunctions.ReadFileType(argument) == XboxFileType.STFS))
            {
                RefreshInputFiles(new List<string>{ argument });
            }
            
            var config = Application.StartupPath + "\\bin\\config\\midicleaner.config";
            if (!File.Exists(config))
            {
                loading = false;
                return;
            }

            var sr = new StreamReader(config, Encoding.Default);
            try
            {
                while (sr.Peek() > 0)
                {
                    detailedLoggingToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                    reportUsingLocationInSecondsToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;
                    reportUsingMeasureBeatTicksToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;
                    breakDownIssueCountByTrackToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;
                    reportUsingMeasureBeatTicksToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                    reportUsingLocationInSecondsToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                    breakDownIssueCountByTrackToolStripMenuItem.Checked = sr.ReadLine().Contains("True");
                    dONOTFixCapitalizationErrorsHarm1.Checked = sr.ReadLine().Contains("True");
                    dONOTFixCapitalizationErrorsHarm2.Checked = sr.ReadLine().Contains("True");
                    ignoreMIDINotes0And1.Checked = sr.ReadLine().Contains("True");
                    createBEATTracks.Checked = sr.ReadLine().Contains("True");
                    deleteVENUETracks.Checked = sr.ReadLine().Contains("True");
                    moveGreenTomMarkers.Checked = sr.ReadLine().Contains("True");
                    doNotDelete2xBassPedalNotes.Checked = sr.ReadLine().Contains("True");
                    separate2xBassPedalNotes.Checked = sr.ReadLine().Contains("True");
                    chkVelocity.Checked = sr.ReadLine().Contains("True");
                    numVelocity.Enabled = chkVelocity.Checked;
                    try
                    {
                        numVelocity.Value = Convert.ToDecimal(Tools.GetConfigString(sr.ReadLine()));
                    }
                    catch (Exception)
                    {
                        numVelocity.Value = 96;
                    }
                    chkLength.Checked = sr.ReadLine().Contains("True");
                    cboLength.Enabled = chkLength.Checked;
                    try
                    {
                        cboLength.SelectedIndex = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                    }
                    catch (Exception)
                    {
                        cboLength.SelectedIndex = 1;
                    }
                    try
                    {
                        backUpFileWhenCleaning.Checked = sr.ReadLine().Contains("True");
                    }
                    catch (Exception)
                    {
                        //old config file without it, leave it
                    }
                }
                sr.Dispose();
            }
            catch (Exception ex)
            {
                sr.Dispose();
                Tools.DeleteFile(config);
                Log("Error loading configuration file");
                Log("The error says: " + ex.Message);
            }
            loading = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                InitialDirectory = Tools.CurrentFolder,
                Title = "Select MIDI and/or CON file(s) to clean",
                Multiselect = true,
            };
            
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Tools.CurrentFolder = Path.GetDirectoryName(ofd.FileNames[0]);
            RefreshInputFiles(ofd.FileNames);
        }

        private void RefreshInputFiles(IEnumerable<string> files)
        {
            inputMIDIs.Clear();
            inputCONs.Clear();
            LogDefaults();
            Log("");
            btnBegin.Visible = false;
            totalfixes = 0;

            foreach (var file in files)
            {
                if (Path.GetExtension(file).ToLowerInvariant() == ".mid")
                {
                    inputMIDIs.Add(file);
                }
                else
                {
                    if (VariousFunctions.ReadFileType(file) == XboxFileType.STFS)
                    {
                        inputCONs.Add(file);
                    }
                    else
                    {
                        Log("File " + Path.GetFileName(file) + " is not allowed, only MIDI and CON files allowed");
                    }
                }
            }

            if (!inputCONs.Any() && !inputMIDIs.Any()) return;
            if (inputMIDIs.Any())
            {
                Log("Received " + (inputMIDIs.Count > 1 ? inputMIDIs.Count + " MIDI files" : "MIDI file " + Path.GetFileName(inputMIDIs[0])));
            }
            if (inputCONs.Any())
            {
                Log("Received " + (inputCONs.Count > 1 ? inputCONs.Count + " STFS files" : "STFS file '" + Path.GetFileName(inputCONs[0]) + "'"));
            }
            Log("Ready to begin cleaning process");
            btnBegin.Visible = true;
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogDefaults();
        }

        private void LogDefaults()
        {
            lstLog.Items.Clear();
            Log("Welcome to " + Text);
            Log("You can drop one or multiple MIDI or CON/LIVE files here");
            Log("You can also click on 'Browse...' to find your MIDI or CON/LIVE file(s)");
            Log("Ready to begin");
        }

        private void doNotDelete2xBassPedalNotes_Click(object sender, EventArgs e)
        {
            if (doNotDelete2xBassPedalNotes.Checked)
            {
                separate2xBassPedalNotes.Checked = false;
            }
            SaveOptions(sender, e);
        }

        private void separate2xBassPedalNotes_Click(object sender, EventArgs e)
        {
            if (separate2xBassPedalNotes.Checked)
            {
                doNotDelete2xBassPedalNotes.Checked = false;
            }
            SaveOptions(sender, e);
        }

        private string GetMBT(long absdelta)
        {
            var time_sig = 4;
            var time_division = 4;
            var totalmeasures = 0;
            long beats_counter = 0;
            var beat_length = TicksPerQuarter;
            long i = 0;

            if (absdelta == 0)
            {
                return "[1:0:00]";
            }

            while (i <= absdelta)
            {
                beats_counter = absdelta - i;
                var i1 = i;
                foreach (var signature in TimeSignatures.TakeWhile(signature => signature.AbsoluteTime <= i1))
                {
                    time_sig = signature.Numerator;
                    time_division = signature.Denominator;
                }
                totalmeasures++;
                beat_length = (int)(TicksPerQuarter / ((decimal)time_division / 4));
                i += beat_length * time_sig;
            }

            var totalbeats = beats_counter == 0 ? 1 : 1 + ((int)(beats_counter / beat_length)); //beat count starts at 1
            var totalticks = beats_counter - ((totalbeats - 1) * beat_length);

            //let's format the ticks values for a nice string
            var tick = "00";
            if (totalticks <= 0) return ("[" + totalmeasures + ":" + totalbeats + ":" + tick + "]");
            var ticker = Math.Round((Double)totalticks / beat_length, 2);
            tick = ticker.ToString(CultureInfo.InvariantCulture);
            tick = tick.Substring(tick.IndexOf(".", StringComparison.Ordinal) + 1); //we only want the decimal portion
            tick = (tick + "0").Substring(0, 2); //this always returns a 2 digit number
            return ("[" + totalmeasures + ":" + totalbeats + ":" + tick + "]");
        }

        private void reportUsingMeasureBeatTicksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (reportUsingMeasureBeatTicksToolStripMenuItem.Checked)
            {
                reportUsingLocationInSecondsToolStripMenuItem.Checked = false;
            }
            SaveOptions(sender, e);
        }

        private void reportUsingLocationInSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (reportUsingLocationInSecondsToolStripMenuItem.Checked)
            {
                reportUsingMeasureBeatTicksToolStripMenuItem.Checked = false;
            }
            SaveOptions(sender, e);
        }

        private void detailedLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reportUsingLocationInSecondsToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;
            reportUsingMeasureBeatTicksToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;
            breakDownIssueCountByTrackToolStripMenuItem.Enabled = detailedLoggingToolStripMenuItem.Checked;

            SaveOptions(sender,e);
        }

        private void SaveOptions(object sender, EventArgs e)
        {
            if (loading || saving) return;

            saving = true;
            try
            {
                var sw = new StreamWriter(Application.StartupPath + "\\bin\\config\\midicleaner.config", false, Encoding.Default);
                sw.WriteLine("DetailedLogging=" + detailedLoggingToolStripMenuItem.Checked);
                sw.WriteLine("ReportInMBT=" + reportUsingMeasureBeatTicksToolStripMenuItem.Checked);
                sw.WriteLine("ReportInSeconds=" + reportUsingLocationInSecondsToolStripMenuItem.Checked);
                sw.WriteLine("BreakDownReport=" + breakDownIssueCountByTrackToolStripMenuItem.Checked);
                sw.WriteLine("DoNotFixCapErrors=" + dONOTFixCapitalizationErrorsHarm1.Checked);
                sw.WriteLine("DoNotFixCapErrors2=" + dONOTFixCapitalizationErrorsHarm2.Checked);
                sw.WriteLine("IgnoreTonicNotes=" + ignoreMIDINotes0And1.Checked);
                sw.WriteLine("CreateBeatTrack=" + createBEATTracks.Checked);
                sw.WriteLine("DeleteVenueTrack=" + deleteVENUETracks.Checked);
                sw.WriteLine("MoveTomMarkers=" + moveGreenTomMarkers.Checked);
                sw.WriteLine("LeaveExpert+Notes=" + doNotDelete2xBassPedalNotes.Checked);
                sw.WriteLine("Separate2xTrack=" + separate2xBassPedalNotes.Checked);
                sw.WriteLine("OverrideVelocity=" + chkVelocity.Checked);
                sw.WriteLine("VelocityValue=" + numVelocity.Value);
                sw.WriteLine("FixZeroLength=" + chkLength.Checked);
                sw.WriteLine("ZeroLengthIndex=" + cboLength.SelectedIndex);
                sw.WriteLine("MakeBackUpFirst=" + backUpFileWhenCleaning.Checked);
                sw.Dispose();
            }
            catch (Exception ex)
            {
                Log("Error saving configuration file");
                Log("The error says: " + ex.Message);
            }
            saving = false;
        }

        private bool isCameraEvent(string ev)
        {
            var cameraEvents = new List<string>() { 
                "[coop_all_behind]", 
                "[coop_all_far]", 
                "[coop_all_near]", 
                "[coop_front_behind]", 
                "[coop_front_near]", 
                "[coop_d_behind]", 
                "[coop_d_near]", 
                "[coop_v_behind]", 
                "[coop_v_near]", 
                "[coop_b_behind]",
                "[coop_b_near]", 
                "[coop_g_behind]", 
                "[coop_g_near]", 
                "[coop_k_behind]", 
                "[coop_k_near]", 
                "[coop_dv_near]", 
                "[coop_bd_near]",
                "[coop_dg_near]",
                "[coop_bv_behind]",
                "[coop_bv_near]",
                "[coop_gv_behind]",
                "[coop_gv_near]",
                "[coop_kv_behind]",
                "[coop_kv_near]",
                "[coop_bg_behind]",
                "[coop_bg_near]",
                "[coop_bk_behind]",
                "[coop_bk_near]",
                "[coop_gk_behind]",
                "[coop_gk_near]",    
                "[coop_dv_near]",
                "[coop_bd_near]",
                "[coop_dg_near]",
                "[coop_bv_behind]",
                "[coop_bv_near]",
                "[coop_gv_behind]",
                "[coop_gv_near]",
                "[coop_kv_behind]",
                "[coop_kv_near]",
                "[coop_bg_behind]",
                "[coop_bg_near]",
                "[coop_bk_behind]",
                "[coop_bk_near]",
                "[coop_gk_behind]",
                "[coop_gk_near]", 
                "[directed_all]",
                "[directed_all_cam]",
                "[directed_all_yeah]",
                "[directed_all_lt]",
                "[directed_bre]",
                "[directed_brej]",
                "[directed_crowd]",
                "[directed_drums]",
                "[directed_drums_pnt]",
                "[directed_drums_np]",
                "[directed_drums_lt]",
                "[directed_drums_kd]",
                "[directed_vocals]",
                "[directed_vocals_np]",
                "[directed_vocals_cls]",
                "[directed_vocals_cam_pr]",
                "[directed_vocals_cam_pt]",
                "[directed_stagedive]",
                "[directed_crowdsurf]",
                "[directed_bass]",
                "[directed_crowd_b]",
                "[directed_bass_np]",
                "[directed_bass_cam]",
                "[directed_bass_cls]",
                "[directed_guitar]",
                "[directed_crowd_g]",
                "[directed_guitar_np]",
                "[directed_guitar_cls]",
                "[directed_guitar_cam_pr]",
                "[directed_guitar_cam_pt]",
                "[directed_keys]",
                "[directed_keys_cam]",
                "[directed_keys_np]",
                "[directed_duo_drums]",
                "[directed_duo_guitar]",
                "[directed_duo_bass]",
                "[directed_duo_kv]",
                "[directed_duo_gb]",
                "[directed_duo_kb]",
                "[directed_duo_kg]",    
                "[directed_all]",
                "[directed_all_cam]",
                "[directed_all_yeah]",
                "[directed_all_lt]",
                "[directed_bre]",
                "[directed_brej]",
                "[directed_crowd]",
                "[directed_drums]",
                "[directed_drums_pnt]",
                "[directed_drums_np]",
                "[directed_drums_lt]",
                "[directed_drums_kd]",
                "[directed_vocals]",
                "[directed_vocals_np]",
                "[directed_vocals_cls]",
                "[directed_vocals_cam_pr]",
                "[directed_vocals_cam_pt]",
                "[directed_stagedive]",
                "[directed_crowdsurf]",
                "[directed_bass]",
                "[directed_crowd_b]",
                "[directed_bass_np]",
                "[directed_bass_cam]",
                "[directed_bass_cls]",
                "[directed_guitar]",
                "[directed_crowd_g]",
                "[directed_guitar_np]",
                "[directed_guitar_cls]",
                "[directed_guitar_cam_pr]",
                "[directed_guitar_cam_pt]",
                "[directed_keys]",
                "[directed_keys_cam]",
                "[directed_keys_np]",
                "[directed_duo_drums]",
                "[directed_duo_guitar]",
                "[directed_duo_bass]",
                "[directed_duo_kv]",
                "[directed_duo_gb]",
                "[directed_duo_kb]",
                "[directed_duo_kg]",
                "[directed_all]",
                "[directed_all_cam]",
                "[directed_all_lt]",
                "[directed_all_yeah]",
                "[directed_bre]",
                "[directed_brej]",
                "[directed_drums_np]",
                "[directed_bass_np]",
                "[directed_guitar_np]",
                "[directed_vocals_np]",
                "[directed_keys_np]",
                "[directed_drums]",
                "[directed_drums_lt]",
                "[directed_vocals]",
                "[directed_bass]",
                "[directed_guitar]",
                "[directed_keys]",
                "[directed_vocals_cam_pr]",
                "[directed_vocals_cam_pt]",
                "[directed_guitar_cam_pr]",
                "[directed_guitar_cam_pt]",
                "[directed_keys_cam]",
                "[directed_bass_cam]",
                "[directed_stagedive]",
                "[directed_crowdsurf]",
                "[directed_vocals_cls]",
                "[directed_bass_cls]",
                "[directed_guitar_cls]",
                "[directed_drums_kd]",
                "[directed_drums_pnt]",
                "[directed_crowd_g]",
                "[directed_crowd_b]",
                "[directed_duo_drums]",
                "[directed_duo_guitar]",
                "[directed_duo_bass]",
                "[directed_duo_kv]",
                "[directed_duo_gb]",
                "[directed_duo_kb]",
                "[directed_duo_kg]",
                "[directed_crowd]",
            };

            return cameraEvents.Contains(ev);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (var file in inputMIDIs.Where(file => file.ToLowerInvariant().EndsWith(".mid", StringComparison.Ordinal)))
            {
                Log("");
                isCON = false;
                activeFile = file;
                CleanMIDI(file);
            }

            foreach (var file in inputCONs.Where(file => VariousFunctions.ReadFileType(file) == XboxFileType.STFS))
            {
                Log("");
                isCON = true;
                activeFile = file;
                CleanCON(file);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lstLog.Cursor = Cursors.Default;
            Log("");
            if (workingcounter == 0)
            {
                Log("No files were cleaned");
                Log("Ready");
                EnableDisable(true);
                return;
            }

            Log("Cleaned " + workingcounter + " of " + (inputMIDIs.Count + inputCONs.Count) + " MIDI files successfully for a total of " + totalfixes + " fixes");
            if (problem_midis.Any())
            {
                Log("");
                Log("The following " + problem_midis.Count + " MIDI " + (problem_midis.Count > 1 ? "files were" : "file was") + " not cleaned:");
                foreach (var midi in problem_midis)
                {
                    Log(" - " + midi);
                }
                Log("Sorry, there are certain MIDIs that this library just can't process");
                Log("");
            }
            endTime = DateTime.Now;
            var timeDiff = endTime - startTime;
            Log("Process took " + timeDiff.Minutes + (timeDiff.Minutes == 1 ? " minute" : " minutes") + " and " + (timeDiff.Minutes == 0 && timeDiff.Seconds == 0 ? "1 second" : timeDiff.Seconds + " seconds"));
            Log("Ready");
            EnableDisable(true);
        }

        private void MIDICleaner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWorking.Visible)
            {
                SaveOptions(null,null);
                if (ExitonClose)
                {
                    Environment.Exit(0);
                }
                return;
            }
            MessageBox.Show("Please wait until the current process finishes", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            e.Cancel = true;
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
    }          

    public class CameraCuts
    {
        public long TimeStamp { get; set; }
        public bool Guitar { get; set; }
        public bool Bass { get; set; }
        public bool Drummer { get; set; }
        public bool Singer { get; set; }
        public bool NoBehind { get; set; }
        public bool OnlyFar { get; set; }
        public bool OnlyClose { get; set; }
        public bool NoClose { get; set; }
    }

    public class PhraseMarkers
    {
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }
}
