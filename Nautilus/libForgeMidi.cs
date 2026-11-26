using MidiCS.Events;
using MidiCS;
using Nautilus.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibForge.Midi
{
    public class MidiFileResource
    {
        public class TEMPO
        {
            public float StartMillis;
            public uint StartTick;
            public int Tempo;
        }
        public struct TIMESIG
        {
            public int Measure;
            public uint Tick;
            public short Numerator;
            public short Denominator;
        }
        public struct BEAT
        {
            public uint Tick;
            public bool Downbeat;
        }
        public struct FUSER_DATA
        {
            public byte[] data;
        }

        public int MidiSongResourceMagic;
        public uint LastTrackFinalTick;
        public MidiCS.MidiTrack[] MidiTracks;
        public int? FuserRevision;
        public uint FinalTick;
        public uint Measures;
        public uint[] Unknown;
        public uint FinalTickMinusOne;
        public float[] UnknownFloats;
        public TEMPO[] Tempos;
        public TIMESIG[] TimeSigs;
        public BEAT[] Beats;
        public int UnknownZero;

        public int? FuserRevision2;
        public FUSER_DATA[] FuserData;
        public string[] MidiTrackNames;
    }
}

namespace LibForge.Midi
{
    public class MidiFileResourceReader : ReaderBase<MidiFileResource>
    {
        public MidiFileResourceReader(Stream s) : base(s)
        {
        }

        public override MidiFileResource Read()
        {
            var r = new MidiFileResource();
            Read(r);
            return r;
        }

        public void Read(MidiFileResource r)
        {
            r.MidiSongResourceMagic = Check(Int(), 2);
            r.LastTrackFinalTick = UInt();
            r.MidiTracks = Arr(ReadMidiTrack);
            var finalTickOrRev = UInt();
            if (finalTickOrRev == 0x56455223) // '#REV'
            {
                r.FuserRevision = Int();
                r.FinalTick = UInt();
            }
            else
            {
                r.FinalTick = finalTickOrRev;
            }
            r.Measures = UInt();
            r.Unknown = FixedArr(UInt, 6);
            r.FinalTickMinusOne = Check(UInt(), r.FinalTick - 1);
            r.UnknownFloats = FixedArr(Float, 4);
            r.Tempos = Arr(ReadTempo);
            r.TimeSigs = Arr(ReadTimesig);
            r.Beats = Arr(ReadBeat);
            r.UnknownZero = Check(Int(), 0);
            if (r.FuserRevision == 2)
            {
                r.FuserRevision2 = Int();
                r.FuserData = Arr(ReadFuserData);
            }
            r.MidiTrackNames = CheckedArr(String, (uint)r.MidiTracks.Length);
        }

        private uint midiTick;
        private string trackName;
        private string[] trackStrings;
        private MidiTrack ReadMidiTrack()
        {
            var unk = Byte();
            var unk2 = Int();
            var num_events = UInt();
            midiTick = 0;
            trackName = "";
            var start = s.Position;
            s.Position += num_events * 8;
            trackStrings = Arr(String);
            var end = s.Position;
            s.Position = start;
            var msgs = new List<IMidiMessage>(FixedArr(ReadMessage, num_events));
            msgs.Add(new EndOfTrackEvent(0));
            s.Position = end;
            return new MidiTrack(msgs, midiTick, trackName);
        }
        private IMidiMessage ReadMessage()
        {
            var tick = UInt();
            var deltaTime = tick - midiTick;
            midiTick = tick;
            var kind = Byte();
            switch (kind)
            {
                // Midi Messages
                case 1:
                    var tc = Byte();
                    var channel = (byte)(tc & 0xF);
                    var type = tc >> 4;
                    var note = Byte();
                    var velocity = Byte();
                    switch (type)
                    {
                        case 8:
                            return new NoteOffEvent(deltaTime, channel, note, velocity);
                        case 9:
                            return new NoteOnEvent(deltaTime, channel, note, velocity);
                        case 11: // seen in touchofgrey and others, assuming ctrl chg
                            return new ControllerEvent(deltaTime, channel, note, velocity);
                        case 12: // seen in foreplaylongtime, assuming prgmchg
                            return new ProgramChgEvent(deltaTime, channel, note);
                        case 13: // seen in huckleberrycrumble, assuming channel pressure
                            return new ChannelPressureEvent(deltaTime, channel, note);
                        case 14: // seen in theballadofirahayes, assuming pitch bend
                            return new PitchBendEvent(deltaTime, channel, (ushort)(note | (velocity << 8)));
                        default:
                            throw new NotImplementedException($"Message type {type}");
                    }
                // Tempo
                case 2:
                    var tempo_msb = (uint)Byte();
                    var tempo_lsb = UShort();
                    return new TempoEvent(deltaTime, tempo_msb << 16 | tempo_lsb);
                // Time Signature
                case 4:
                    var num = Byte();
                    var denom = Byte();
                    var denom_pow2 = (byte)Math.Log(denom, 2);
                    Skip(1)();
                    return new TimeSignature(deltaTime, num, denom_pow2, 24, 8);
                // Text
                case 8:
                    var ttype = Byte();
                    var txt = trackStrings[Short()];
                    switch (ttype)
                    {
                        case 1:
                            return new TextEvent(deltaTime, txt);
                        case 2:
                            return new CopyrightNotice(deltaTime, txt);
                        case 3:
                            trackName = txt;
                            return new TrackName(deltaTime, txt);
                        case 5:
                            return new Lyric(deltaTime, txt);
                        default:
                            throw new NotImplementedException($"Text event {ttype} not implemented");
                    }
                default:
                    throw new NotImplementedException($"Message kind {kind} not yet known");
            }
        }
        private MidiFileResource.TEMPO ReadTempo() => new RBMid.TEMPO
        {
            StartMillis = Float(),
            StartTick = UInt(),
            Tempo = Int()
        };
        private MidiFileResource.TIMESIG ReadTimesig() => new RBMid.TIMESIG
        {
            Measure = Int(),
            Tick = UInt(),
            Numerator = Short(),
            Denominator = Short()
        };
        private MidiFileResource.BEAT ReadBeat() => new RBMid.BEAT
        {
            Tick = UInt(),
            Downbeat = Int() != 0
        };
        private MidiFileResource.FUSER_DATA ReadFuserData()
        {
            var unk_count = UInt();
            return new MidiFileResource.FUSER_DATA()
            {
                data = FixedArr(Byte, unk_count + 8)
            };
        }
    }
}

namespace LibForge.Midi
{
    public class MidiFileResourceWriter : WriterBase<MidiFileResource>
    {
        public static void WriteStream(MidiFileResource r, Stream s)
        {
            new MidiFileResourceWriter(s).WriteStream(r);
        }
        private MidiFileResourceWriter(Stream s) : base(s) { }
        public override void WriteStream(MidiFileResource r)
        {
            Write(r.MidiSongResourceMagic);
            Write(r.LastTrackFinalTick);
            Write(r.MidiTracks, WriteMidiTrack);
            if (r.FuserRevision != null)
            {
                Write(r.FuserRevision.Value);
            }
            Write(r.FinalTick);
            Write(r.Measures);
            Array.ForEach(r.Unknown, Write);
            Write(r.FinalTickMinusOne);
            Array.ForEach(r.UnknownFloats, Write);
            Write(r.Tempos, WriteTempo);
            Write(r.TimeSigs, WriteTimesig);
            Write(r.Beats, WriteBeat);
            Write(r.UnknownZero);
            if (r.FuserRevision != null)
            {
                Write(r.FuserRevision2.Value);
                Write(r.FuserData, WriteFuserData);
            }
            Write(r.MidiTrackNames, Write);
        }
        private bool first_track = true;
        private List<string> track_strings;
        private void WriteMidiTrack(MidiTrack obj)
        {
            track_strings = new List<string>();

            Write((byte)1);
            if (first_track || obj.Name == "EVENTS")
                Write(-1);
            else
                Write(0);
            first_track = false;
            // Subtract 1 for the end-of-track event
            Write(obj.Messages.Count - 1);
            uint ticks = 0;
            foreach (var m in obj.Messages)
            {
                byte kind, d1, d2, d3;
                switch (m)
                {
                    case NoteOffEvent e:
                        kind = 1;
                        d1 = (byte)(0x80 | e.Channel);
                        d2 = e.Key;
                        d3 = e.Velocity;
                        break;
                    case NoteOnEvent e:
                        kind = 1;
                        d1 = (byte)(0x90 | e.Channel);
                        d2 = e.Key;
                        d3 = e.Velocity;
                        break;
                    case ControllerEvent e:
                        kind = 1;
                        d1 = (byte)(0xB0 | e.Channel);
                        d2 = e.Controller;
                        d3 = e.Value;
                        break;
                    case ProgramChgEvent e:
                        kind = 1;
                        d1 = (byte)(0xC0 | e.Channel);
                        d2 = e.Program;
                        d3 = 0;
                        break;
                    case ChannelPressureEvent e:
                        kind = 1;
                        d1 = (byte)(0xD0 | e.Channel);
                        d2 = e.Pressure;
                        d3 = 0;
                        break;
                    case PitchBendEvent e:
                        kind = 1;
                        d1 = (byte)(0xE0 | e.Channel);
                        d2 = (byte)(e.Bend & 0xFF);
                        d3 = (byte)(e.Bend >> 8);
                        break;
                    case TempoEvent e:
                        kind = 2;
                        d1 = (byte)(e.MicrosPerQn >> 16);
                        d2 = (byte)(e.MicrosPerQn & 0xFFU);
                        d3 = (byte)((e.MicrosPerQn >> 8) & 0xFFU);
                        break;
                    case TimeSignature e:
                        kind = 4;
                        d1 = e.Numerator;
                        d2 = (byte)(1 << e.Denominator);
                        d3 = 0;
                        break;
                    case MetaTextEvent e:
                        kind = 8;
                        var idx = GetString(e.Text);
                        d2 = (byte)(idx & 0xFF);
                        d3 = (byte)(idx >> 8);
                        switch (e)
                        {
                            case TextEvent x:
                                d1 = 1;
                                break;
                            case TrackName x:
                                d1 = 3;
                                break;
                            case Lyric x:
                                d1 = 5;
                                break;
                            default:
                                d1 = 1;
                                break;
                        }
                        break;
                    case EndOfTrackEvent e:
                        continue;
                    default:
                        throw new Exception("Unknown Midi Message type");
                }
                ticks += m.DeltaTime;
                Write(ticks);
                Write(kind);
                Write(d1);
                Write(d2);
                Write(d3);
            }
            Write(track_strings.Count);
            track_strings.ForEach(Write);
        }
        private int GetString(string s)
        {
            var idx = track_strings.Count;
            track_strings.Add(s);
            return idx;
        }
        private void WriteTempo(MidiFileResource.TEMPO obj)
        {
            Write(obj.StartMillis);
            Write(obj.StartTick);
            Write(obj.Tempo);
        }
        private void WriteTimesig(MidiFileResource.TIMESIG obj)
        {
            Write(obj.Measure);
            Write(obj.Tick);
            Write(obj.Numerator);
            Write(obj.Denominator);
        }
        private void WriteBeat(MidiFileResource.BEAT obj)
        {
            Write(obj.Tick);
            Write(obj.Downbeat ? 1 : 0);
        }

        private void WriteFuserData(MidiFileResource.FUSER_DATA obj)
        {
            Write(obj.data.Length - 8);
            s.Write(obj.data, 0, obj.data.Length);
        }
    }
}

namespace LibForge.Midi
{
    public class MidiHelper
    {
        private MidiCS.MidiFile file;

        public List<MidiTrackProcessed> ProcessTracks(MidiCS.MidiFile file)
        {
            this.file = file;
            return file.Tracks.Select(processTrack).ToList();
        }

        private MidiCS.TimeSigTempoEvent GetTempo(uint tick)
        {
            var idx = 0;
            for (; idx < file.TempoTimeSigMap.Count; idx++)
            {
                if (file.TempoTimeSigMap[idx].Tick > tick)
                {
                    break;
                }
            }
            idx--;
            return file.TempoTimeSigMap[idx];
        }
        private MidiTrackProcessed processTrack(MidiCS.MidiTrack track)
        {
            var items = new List<MidiItem>();
            var notesOn = new Dictionary<int, MidiNote>();
            var ticks = 0u;
            var finalTick = 0u;
            var finalTime = 0d;
            foreach (var msg in track.Messages)
            {
                ticks += msg.DeltaTime;
                var tempo = GetTempo(ticks);
                var time = tempo.Time + ((ticks - tempo.Tick) / 480.0) * (60 / tempo.BPM);
                switch (msg)
                {
                    case NoteOnEvent e when e.Velocity != 0:
                        {
                            var note = new MidiNote
                            {
                                StartTicks = ticks,
                                StartTime = time,
                                Channel = e.Channel,
                                Key = e.Key,
                                Velocity = e.Velocity,
                                CurrentTimeSig = tempo
                            };
                            notesOn[e.Channel << 8 | e.Key] = note;
                            items.Add(note);
                        }
                        break;
                    case NoteOnEvent e when e.Velocity == 0:
                        {
                            var note = notesOn[e.Channel << 8 | e.Key];
                            note.Length = time - note.StartTime;
                            note.LengthTicks = ticks - note.StartTicks;
                        }
                        break;
                    case NoteOffEvent e:
                        {
                            var note = notesOn[e.Channel << 8 | e.Key];
                            note.Length = time - note.StartTime;
                            note.LengthTicks = ticks - note.StartTicks;
                        }
                        break;
                    case TrackName e:
                        // We already know the track name and we don't want this
                        // grouped in with other text events
                        break;
                    case MetaTextEvent e:
                        items.Add(new MidiText
                        {
                            StartTicks = ticks,
                            StartTime = time,
                            Text = e.Text,
                            CurrentTimeSig = tempo
                        });
                        break;
                    default:
                        continue;
                }

                if (ticks > finalTick)
                {
                    finalTick = ticks;
                    finalTime = time;
                }
            }

            return new MidiTrackProcessed
            {
                Name = track.Name,
                LastTick = finalTick,
                LastTime = finalTime,
                Items = items
            };
        }

        public static List<IMidiMessage> ToAbsolute(List<IMidiMessage> messages)
        {
            var msgs = new List<IMidiMessage>();
            var abstime = 0u;
            foreach (var msg in messages)
            {
                abstime += msg.DeltaTime;
                switch (msg)
                {
                    case NoteOnEvent e:
                        if (e.Velocity == 0)
                        {
                            msgs.Add(new NoteOffEvent(abstime, e.Channel, e.Key, e.Velocity));
                        }
                        else
                        {
                            msgs.Add(new NoteOnEvent(abstime, e.Channel, e.Key, e.Velocity));
                        }
                        break;
                    case NoteOffEvent e:
                        msgs.Add(new NoteOffEvent(abstime, e.Channel, e.Key, e.Velocity));
                        break;
                    case NotePressureEvent e:
                        msgs.Add(new NotePressureEvent(abstime, e.Channel, e.Key, e.Pressure));
                        break;
                    case ControllerEvent e:
                        msgs.Add(new ControllerEvent(abstime, e.Channel, e.Controller, e.Value));
                        break;
                    case ProgramChgEvent e:
                        msgs.Add(new ProgramChgEvent(abstime, e.Channel, e.Program));
                        break;
                    case ChannelPressureEvent e:
                        msgs.Add(new ChannelPressureEvent(abstime, e.Channel, e.Pressure));
                        break;
                    case PitchBendEvent e:
                        msgs.Add(new PitchBendEvent(abstime, e.Channel, e.Bend));
                        break;
                    case SysexEvent e:
                        msgs.Add(new SysexEvent(abstime, e.Data));
                        break;
                    case SequenceNumber e:
                        msgs.Add(new SequenceNumber(abstime, e.Number));
                        break;
                    case TextEvent e:
                        msgs.Add(new TextEvent(abstime, e.Text));
                        break;
                    case CopyrightNotice e:
                        msgs.Add(new CopyrightNotice(abstime, e.Text));
                        break;
                    case TrackName e:
                        msgs.Add(new TrackName(abstime, e.Text));
                        break;
                    case InstrumentName e:
                        msgs.Add(new InstrumentName(abstime, e.Text));
                        break;
                    case Lyric e:
                        msgs.Add(new Lyric(abstime, e.Text));
                        break;
                    case Marker e:
                        msgs.Add(new Marker(abstime, e.Text));
                        break;
                    case CuePoint e:
                        msgs.Add(new CuePoint(abstime, e.Text));
                        break;
                    case ChannelPrefix e:
                        msgs.Add(new ChannelPrefix(abstime, e.Channel));
                        break;
                    case EndOfTrackEvent e:
                        msgs.Add(new EndOfTrackEvent(abstime));
                        break;
                    case TempoEvent e:
                        msgs.Add(new TempoEvent(abstime, e.MicrosPerQn));
                        break;
                    case SmtpeOffset e:
                        msgs.Add(new SmtpeOffset(abstime, e.Hours, e.Minutes, e.Seconds, e.Frames, e.FrameHundredths));
                        break;
                    case TimeSignature e:
                        msgs.Add(new TimeSignature(abstime, e.Numerator, e.Denominator, e.ClocksPerTick, e.ThirtySecondNotesPer24Clocks));
                        break;
                    case KeySignature e:
                        msgs.Add(new KeySignature(abstime, e.Sharps, e.Tonality));
                        break;
                    case SequencerSpecificEvent e:
                        msgs.Add(new SequencerSpecificEvent(abstime, e.Data));
                        break;
                }
            }
            return msgs;
        }

        public static List<IMidiMessage> ToRelative(List<IMidiMessage> messages)
        {
            var msgs = new List<IMidiMessage>();
            var lasttime = 0u;
            foreach (var msg in messages)
            {
                var deltatime = msg.DeltaTime - lasttime;
                lasttime = msg.DeltaTime;
                switch (msg)
                {
                    case NoteOnEvent e:
                        msgs.Add(new NoteOnEvent(deltatime, e.Channel, e.Key, e.Velocity));
                        break;
                    case NoteOffEvent e:
                        msgs.Add(new NoteOffEvent(deltatime, e.Channel, e.Key, e.Velocity));
                        break;
                    case NotePressureEvent e:
                        msgs.Add(new NotePressureEvent(deltatime, e.Channel, e.Key, e.Pressure));
                        break;
                    case ControllerEvent e:
                        msgs.Add(new ControllerEvent(deltatime, e.Channel, e.Controller, e.Value));
                        break;
                    case ProgramChgEvent e:
                        msgs.Add(new ProgramChgEvent(deltatime, e.Channel, e.Program));
                        break;
                    case ChannelPressureEvent e:
                        msgs.Add(new ChannelPressureEvent(deltatime, e.Channel, e.Pressure));
                        break;
                    case PitchBendEvent e:
                        msgs.Add(new PitchBendEvent(deltatime, e.Channel, e.Bend));
                        break;
                    case SysexEvent e:
                        msgs.Add(new SysexEvent(deltatime, e.Data));
                        break;
                    case SequenceNumber e:
                        msgs.Add(new SequenceNumber(deltatime, e.Number));
                        break;
                    case TextEvent e:
                        msgs.Add(new TextEvent(deltatime, e.Text));
                        break;
                    case CopyrightNotice e:
                        msgs.Add(new CopyrightNotice(deltatime, e.Text));
                        break;
                    case TrackName e:
                        msgs.Add(new TrackName(deltatime, e.Text));
                        break;
                    case InstrumentName e:
                        msgs.Add(new InstrumentName(deltatime, e.Text));
                        break;
                    case Lyric e:
                        msgs.Add(new Lyric(deltatime, e.Text));
                        break;
                    case Marker e:
                        msgs.Add(new Marker(deltatime, e.Text));
                        break;
                    case CuePoint e:
                        msgs.Add(new CuePoint(deltatime, e.Text));
                        break;
                    case ChannelPrefix e:
                        msgs.Add(new ChannelPrefix(deltatime, e.Channel));
                        break;
                    case EndOfTrackEvent e:
                        msgs.Add(new EndOfTrackEvent(deltatime));
                        break;
                    case TempoEvent e:
                        msgs.Add(new TempoEvent(deltatime, e.MicrosPerQn));
                        break;
                    case SmtpeOffset e:
                        msgs.Add(new SmtpeOffset(deltatime, e.Hours, e.Minutes, e.Seconds, e.Frames, e.FrameHundredths));
                        break;
                    case TimeSignature e:
                        msgs.Add(new TimeSignature(deltatime, e.Numerator, e.Denominator, e.ClocksPerTick, e.ThirtySecondNotesPer24Clocks));
                        break;
                    case KeySignature e:
                        msgs.Add(new KeySignature(deltatime, e.Sharps, e.Tonality));
                        break;
                    case SequencerSpecificEvent e:
                        msgs.Add(new SequencerSpecificEvent(deltatime, e.Data));
                        break;
                }
            }
            return msgs;
        }

    }

    public class MidiTrackProcessed
    {
        public string Name;
        public uint LastTick;
        public double LastTime;
        public List<MidiItem> Items;
    }

    public abstract class MidiItem
    {
        public double StartTime;
        public uint StartTicks;
        public uint Measure;
        public MidiCS.TimeSigTempoEvent CurrentTimeSig;
    }

    public class MidiNote : MidiItem
    {
        public double Length;
        public uint LengthTicks;
        public byte Channel;
        public byte Key;
        public byte Velocity;
    }

    public class MidiText : MidiItem
    {
        public string Text;
    }
}

namespace LibForge.Midi
{
    public class RBMid : MidiFileResource
    {
        public struct TICKTEXT
        {
            public uint Tick;
            public string Text;
        }
        public struct LYRICS
        {
            public string TrackName;
            public TICKTEXT[] Lyrics;
            public int Unknown1;
            public int Unknown2;
            public byte Unknown3;
        }

        public struct DRUMFILLS
        {
            public struct FILL_LANES
            {
                public uint Tick;
                public uint Lanes;
            }
            public struct FILL
            {
                public uint StartTick;
                public uint EndTick;
                public byte IsBRE;
            }
            public FILL_LANES[] Lanes;
            public FILL[] Fills;
        }
        public struct ANIM
        {
            public struct EVENT
            {
                public float StartMillis;
                public uint StartTick;
                public ushort LengthMillis;
                public ushort LengthTicks;
                public int KeyBitfield;
                public int Unknown2;
                public short Unknown3;
            }
            public string TrackName;
            public int Unknown1;
            public int Unknown2;
            public EVENT[] Events;
            public int Unknown3;
        }

        public struct TOMMARKER
        {
            public class MARKER
            {
                public uint Tick;
                [Flags]
                public enum FLAGS : int
                {
                    Unk = 1,
                    Unk2 = 2,
                    ProYellow = 4,
                    ProBlue = 8,
                    ProGreen = 16
                }
                public FLAGS Flags;
            }
            public MARKER[] Markers;
            public int Unknown1;
            public int Unknown2;
        }

        public struct LANEMARKER
        {
            public struct MARKER
            {
                public uint StartTick;
                public uint EndTick;
                public int Lanes;
            }
            // First dimension: difficulty
            public MARKER[][] Markers;
        }

        public struct GTRTRILLS
        {
            public class TRILL
            {
                public uint StartTick;
                public uint EndTick;
                public int FirstFret;
                public int SecondFret;
            }
            // First dimension: difficulty
            public TRILL[][] Trills;
        }

        public struct DRUMMIXES
        {
            // 1st dimension: difficulties
            public TICKTEXT[][] Mixes;
        }

        public struct GEMTRACK
        {
            public class GEM
            {
                public float StartMillis;
                public uint StartTicks;
                public ushort LengthMillis;
                public ushort LengthTicks;
                public int Lanes;
                public bool IsHopo;
                public bool NoTail;
                public int ProCymbal;
            }
            public GEM[][] Gems;
            public int HopoThreshold;
        }
        public struct SECTIONS
        {
            public struct SECTION
            {
                public uint StartTicks;
                public uint LengthTicks;
            }
            // Only seen 0 and 1 used although there are always 6 entries...
            public enum SectionType : int
            {
                Overdrive = 0,
                Solo = 1
            }
            // 1st dimension: difficulty, 2nd: section type, 3rd: list of sections
            public SECTION[][][] Sections;
        }
        public struct VOCALTRACK
        {
            public class PHRASE_MARKER
            {
                public const byte FLAG_NORMAL = 1;
                public const byte FLAG_TUG_OF_WAR = 2;

                public float StartMillis;
                public float LengthMillis;
                public uint StartTicks;
                public uint LengthTicks;
                public int StartNoteIdx;
                public int EndNoteIdx;
                public bool HasPitchedVox;
                public bool HasUnpitchedVox;
                public float LowNote;
                public float HighNote;
                /// <summary>
                /// Bitmask for regular phrase (105), alternate phrase (106), other?
                /// </summary>
                public byte PhraseFlags; // seen: 0, 1, 2, and 3
                /// <summary>
                /// Set to true on fake phrase markers during percussion
                /// </summary>
                public bool PercussionSection;
            }
            public class VOCAL_NOTE
            {
                public int PhraseIndex;
                public int MidiNote;
                public int MidiNote2;
                public float StartMillis;
                public uint StartTick;
                public float LengthMillis;
                public ushort LengthTicks;
                public string Lyric;
                // 9 Bytes are flags
                // 0 0 0 0 0 1 0 0 1 for normal notes
                // 0 0 0 0 0 1 1 0 1 for portamento
                // 0 0 1 0 0 1 0 0 1 for unpitched
                /// <summary>
                /// Set to true on the last note of any phrase
                /// </summary>
                public bool LastNoteInPhrase;
                /// <summary>
                /// Always false
                /// </summary>
                public bool False1;
                /// <summary>
                /// Set to true on unpitched notes
                /// </summary>
                public bool Unpitched;
                /// <summary>
                /// Set to true on unpitched notes with the generous detection (^) character
                /// </summary>
                public bool UnpitchedGenerous;
                /// <summary>
                /// Set to true when a vocal range divider (%) is attached to this note
                /// </summary>
                public bool RangeDivider;
                /// <summary>
                /// Set the first bit if a regular phrase (105), second if alternate phrase (106), both if both
                /// </summary>
                public byte PhraseFlags;
                /// <summary>
                /// Set to true if this note is a slide between two notes
                /// </summary>
                public bool Portamento;
                /// <summary>
                /// Set to true when a lyric shift marker (note 1) follows this note
                /// </summary>
                public bool LyricShift;
                /// <summary>
                /// Set to false when there is a $ character on a harmony lyric.
                /// </summary>
                public bool ShowLyric;
            }
            public struct OD_REGION
            {
                public float StartMillis;
                public float EndMillis;
            }
            public PHRASE_MARKER[] FakePhraseMarkers;
            public PHRASE_MARKER[] AuthoredPhraseMarkers;
            public VOCAL_NOTE[] Notes;
            public uint[] Percussion;
            public OD_REGION[] FreestyleRegions;
        }
        public struct UNKSTRUCT1
        {
            public uint Tick;
            public float FloatData;
        }
        public struct VocalTrackRange
        {
            public float StartMillis;
            public int StartTicks;
            public float LowNote;
            public float HighNote;
        }
        public struct MAP
        {
            public float StartTime;
            public int Map;
        }
        public struct HANDPOS
        {
            public float StartTime;
            public float Length;
            public int Position;
            public byte Unknown;
        }
        public struct MARKUP_SOLO_NOTES
        {
            public uint StartTick;
            public uint EndTick;
            public int NoteOffset;
        }
        public struct TWOTICKS
        {
            public uint StartTick;
            public uint EndTick;
        }
        public class MARKUPCHORD
        {
            public uint StartTick;
            public uint EndTick;
            public int[] Pitches;
        }
        public class RBVREVENTS
        {
            public struct BEATMATCH_SECTION
            {
                public int unk_zero;
                public string beatmatch_section;
                public uint StartTick;
                public uint EndTick;
            }
            public struct UNKSTRUCT1
            {
                public int Unk1;
                public float StartPercentage;
                public float EndPercentage;
                public uint StartTick;
                public uint EndTick;
                public int Unk2;
            }
            public struct UNKSTRUCT2
            {
                public int Unk;
                public string Name;
                public uint Tick;
            }
            public struct UNKSTRUCT3
            {
                public int Unk1;
                public string exsandohs;
                public uint StartTick;
                public uint EndTick;
                public byte[] Flags;
                public int Unk2;
            }
            public struct UNKSTRUCT4
            {
                public int Unk;
                public string Name;
                public uint StartTick;
                public uint EndTick;
            }
            public struct UNKSTRUCT5
            {
                public int Unk1;
                public string Name;
                public string[] ExsOhs;
                public uint StartTick;
                public uint EndTick;
                public byte Unk2;
            }
            public struct UNKSTRUCT6
            {
                public uint Tick;
                public int Unk;
            }
            public BEATMATCH_SECTION[] BeatmatchSections;
            public UNKSTRUCT1[] UnkStruct1;
            public UNKSTRUCT2[] UnkStruct2;
            public UNKSTRUCT3[] UnkStruct3;
            public UNKSTRUCT4[] UnkStruct4;
            public UNKSTRUCT5[] UnkStruct5;
            public uint[] UnknownTicks;
            public int UnkZero2;
            public UNKSTRUCT6[] UnkStruct6;
        }

        public const int FORMAT_RB4 = 0x10;
        public const int FORMAT_RBVR = 0x2F;
        public int Format;
        public LYRICS[] Lyrics;
        public DRUMFILLS[] DrumFills;
        public ANIM[] Anims;
        public TOMMARKER[] ProMarkers;
        public LANEMARKER[] LaneMarkers;
        public GTRTRILLS[] TrillMarkers;
        public DRUMMIXES[] DrumMixes;
        public GEMTRACK[] GemTracks;
        public SECTIONS[] OverdriveSoloSections;
        public VOCALTRACK[] VocalTracks;
        public int UnknownOne;
        public int UnknownNegOne;
        public float UnknownHundred;
        public UNKSTRUCT1[] Unknown4;
        public VocalTrackRange[] VocalRange;
        // Takes values 90, 92, 125, 130, 170, 250
        public int HopoThreshold;
        public uint NumPlayableTracks;
        public uint FinalEventTick;
        public uint UnkVrTick;
        public byte UnknownZeroByte;
        public float PreviewStartMillis;
        public float PreviewEndMillis;
        public MAP[][] HandMaps;
        public HANDPOS[][] GuitarLeftHandPos;
        public MAP[][] StrumMaps;

        public MARKUP_SOLO_NOTES[] MarkupSoloNotes1;
        public TWOTICKS[] MarkupLoop1;
        public MARKUPCHORD[] MarkupChords1;
        public MARKUP_SOLO_NOTES[] MarkupSoloNotes2;
        public MARKUP_SOLO_NOTES[] MarkupSoloNotes3;
        public TWOTICKS[] MarkupLoop2;

        public RBVREVENTS VREvents;

        private string Check<T>(IList<T> a, IList<T> b, string n, Func<T, T, string> f)
        {
            if ((b == null || b.Count == 0) && (a == null || a.Count == 0))
                return null;
            else if (a == null)
                return $"{n} was null in a";
            else if (b == null)
                return $"{n} was null in b";

            if (a.Count != b.Count)
                return $"{n}.Length: a={a.Count}, b={b.Count}";
            for (var i = 0; i < a.Count; i++)
            {
                var r = f(a[i], b[i]);
                if (r != null)
                    return $"{n}[{i}].{r}";
            }
            return null;
        }
        private string Check<T>(T[][] a, T[][] b, string n, Func<T, T, string> f)
        {
            if ((b == null || b.Length == 0) && (a == null || a.Length == 0))
                return null;
            else if (a == null)
                return $"{n} was null in a";
            else if (b == null)
                return $"{n} was null in b";

            if (a.Length != b.Length)
                return $"{n}.Length: a={a.Length}, b={b.Length}";
            for (var i = 0; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i].Length != b[i].Length)
                    return $"{n}[{i}].Length: a={a[i].Length}, b={b[i].Length}";
                for (var j = 0; j < Math.Min(a[i].Length, b[i].Length); j++)
                {
                    var r = f(a[i][j], b[i][j]);
                    if (r != null)
                        return $"{n}[{i}][{j}].{r}";
                }
            }
            return null;
        }
        private string Check<T>(T[][][] a, T[][][] b, string n, Func<T, T, string> f)
        {
            if ((b == null || b.Length == 0) && (a == null || a.Length == 0))
                return null;
            else if (a == null)
                return $"{n} was null in a";
            else if (b == null)
                return $"{n} was null in b";

            if (a.Length != b.Length)
                return $"{n}.Length: a={a.Length}, b={b.Length}";
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i].Length != b[i].Length)
                    return $"{n}[{i}].Length: a={a[i].Length}, b={b[i].Length}";
                for (var j = 0; j < a[i].Length; j++)
                {
                    if (a[i][j].Length != b[i][j].Length)
                        return $"{n}[{i}][{j}].Length: a={a[i][j].Length}, b={b[i][j].Length}";
                    for (var k = 0; k < a[i][j].Length; k++)
                    {
                        var r = f(a[i][j][k], b[i][j][k]);
                        if (r != null)
                            return $"{n}[{i}][{j}][{k}].{r}";
                    }
                }
            }
            return null;
        }
        private string Check<T>(T a, T b, string n)
          => a.Equals(b) ? null : $"{n}: a={a}, b={b}";
        private string Check<T>(T a, T b)
          => a.Equals(b) ? null : $": a={a}, b={b}";
        private string CheckFloats(float a, float b, string n, float tolerance = 0.1f)
          => Math.Abs(a - b) < tolerance ? null : $"{n}: a={a}, b={b}";
        private string CheckTickText(TICKTEXT a, TICKTEXT b)
          => Check(a.Tick, b.Tick, nameof(TICKTEXT.Tick))
          ?? Check(a.Text, b.Text, nameof(TICKTEXT.Text));
        private string CheckTwoTick(TWOTICKS a, TWOTICKS b)
          => Check(a.StartTick, b.StartTick, nameof(TWOTICKS.StartTick))
          ?? Check(a.EndTick, b.EndTick, nameof(TWOTICKS.EndTick));
        private string CheckSoloNotes(MARKUP_SOLO_NOTES their, MARKUP_SOLO_NOTES my)
          => Check(their.StartTick, my.StartTick, nameof(my.StartTick))
          ?? Check(their.EndTick, my.EndTick, nameof(my.EndTick))
          ?? Check(their.NoteOffset, my.NoteOffset, nameof(my.NoteOffset));
        /// <summary>
        /// Compares this RBMid with another RBMid.
        /// 
        /// Returns null if they are equivalent.
        /// If they are not equivalent, this returns the first field name in which they differ.
        /// For multi-dimensional fields, the return value will look like this:
        /// "Lyrics[0].Lyrics[0].Text"
        /// </summary>
        /// <param name="other">The RBMid to compare to</param>
        /// <returns>null if the files are equivalent, or else a string describing the first differing field</returns>
        public string Compare(RBMid other)
          => Check(other.Format, Format, nameof(Format))
          ?? Check(other.Lyrics, Lyrics, nameof(Lyrics), (their, my)
               => Check(their.TrackName, my.TrackName, nameof(my.TrackName))
               ?? Check(their.Lyrics, my.Lyrics, nameof(my.Lyrics), CheckTickText)
               ?? Check(their.Unknown1, my.Unknown1, nameof(my.Unknown1))
               ?? Check(their.Unknown2, my.Unknown2, nameof(my.Unknown2))
               ?? Check(their.Unknown3, my.Unknown3, nameof(my.Unknown3)))
          ?? Check(other.DrumFills, DrumFills, nameof(DrumFills), (their, my)
               => Check(their.Lanes, my.Lanes, nameof(my.Lanes), (their2, my2)
                    => Check(their2.Tick, my2.Tick, nameof(my2.Tick))
                    ?? Check(their2.Lanes, my2.Lanes, nameof(my2.Lanes)))
               ?? Check(their.Fills, my.Fills, nameof(my.Fills), (their2, my2)
                    => Check(their2.StartTick, my2.StartTick, nameof(my2.StartTick))
                    // TODO: Figure out why this is not always the same after conversion
                    ?? CheckFloats(their2.EndTick, my2.EndTick, nameof(my2.EndTick), 11)
                    ?? Check(their2.IsBRE, my2.IsBRE, nameof(my2.IsBRE))))
          ?? Check(other.Anims, Anims, nameof(Anims), (their, my)
               => Check(their.TrackName, my.TrackName, nameof(my.TrackName))
               ?? Check(their.Unknown1, my.Unknown1, nameof(my.Unknown1))
               ?? Check(their.Unknown2, my.Unknown2, nameof(my.Unknown2))
               // TODO: We are generating more events somehow. Probably to do with the broken chords
               //?? Check(their.Events, my.Events, nameof(my.Events), (their2, my2) => null)
               ?? Check(their.Unknown3, my.Unknown3, nameof(my.Unknown3)))
          ?? Check(other.ProMarkers, ProMarkers, nameof(ProMarkers), (their, my)
               => Check(their.Markers, my.Markers, nameof(my.Markers), (their2, my2)
                    => Check(their2.Tick, my2.Tick, nameof(my2.Tick))
                    ?? Check(their2.Flags, my2.Flags, nameof(my2.Flags)))
               ?? Check(their.Unknown1, my.Unknown1, nameof(my.Unknown1))
               ?? Check(their.Unknown2, my.Unknown2, nameof(my.Unknown2)))
          ?? Check(other.LaneMarkers, LaneMarkers, nameof(LaneMarkers), (their, my)
               => Check(their.Markers, my.Markers, nameof(my.Markers), (their2, my2)
                    => Check(their2.StartTick, my2.StartTick, nameof(my2.StartTick))
                    ?? Check(their2.EndTick, my2.EndTick, nameof(my2.EndTick))
                    ?? Check(their2.Lanes, my2.Lanes, nameof(my2.Lanes))))
          ?? Check(other.TrillMarkers, TrillMarkers, nameof(TrillMarkers), (their, my)
               => Check(their.Trills, my.Trills, nameof(my.Trills), (their2, my2)
                    => Check(their2.StartTick, my2.StartTick, nameof(my2.StartTick))
                    ?? Check(their2.EndTick, my2.EndTick, nameof(my2.EndTick))
                    ?? Check(their2.FirstFret, my2.FirstFret, nameof(my2.FirstFret))
                    ?? Check(their2.SecondFret, my2.SecondFret, nameof(my2.SecondFret))))
          ?? Check(other.DrumMixes, DrumMixes, nameof(DrumMixes), (their, my)
               => Check(their.Mixes, my.Mixes, nameof(my.Mixes), (t, m) => Check(t, m, "", CheckTickText)))
          ?? Check(other.GemTracks, GemTracks, nameof(GemTracks), (their, my)
               => Check(their.Gems, my.Gems, nameof(my.Gems), (their2, my2)
                    => CheckFloats(their2.StartMillis, my2.StartMillis, nameof(my2.StartMillis), 0.2f)
                    ?? Check(their2.StartTicks, my2.StartTicks, nameof(my2.StartTicks))
                    ?? CheckFloats(their2.LengthMillis, my2.LengthMillis, nameof(my2.LengthMillis), 1.5f) // who ever cared about a couple ms
                    ?? Check(their2.LengthTicks, my2.LengthTicks, nameof(my2.LengthTicks))
                    ?? Check(their2.Lanes, my2.Lanes, nameof(my2.Lanes))
                    ?? Check(their2.IsHopo, my2.IsHopo, nameof(my2.IsHopo))
                    ?? Check(their2.NoTail, my2.NoTail, nameof(my2.NoTail))
                    ?? Check(their2.ProCymbal, my2.ProCymbal, nameof(my2.ProCymbal))
                    )
                ?? Check(their.HopoThreshold, my.HopoThreshold, nameof(my.HopoThreshold)))
          ?? Check(other.OverdriveSoloSections, OverdriveSoloSections, nameof(OverdriveSoloSections), (their, my)
               => Check(their.Sections, my.Sections, nameof(my.Sections), (their2, my2)
                    => Check(their2.StartTicks, my2.StartTicks, nameof(my2.StartTicks))
                    ?? Check(their2.LengthTicks, my2.LengthTicks, nameof(my2.LengthTicks))))
          ?? Check(other.VocalTracks, VocalTracks, nameof(VocalTracks), (their, my)
               => Check(their.Percussion, my.Percussion, nameof(my.Percussion), Check)
               // TODO: Fix tacets on HARM tracks
               //?? Check(their.Tacets, my.Tacets, nameof(my.Tacets), (their2, my2)
               //=> CheckFloats(their2.StartMillis, my2.StartMillis, nameof(my2.StartMillis), 1f)
               //?? CheckFloats(their2.EndMillis, my2.EndMillis, nameof(my2.EndMillis), 2f)
               //)
               ?? Check(their.FakePhraseMarkers, my.FakePhraseMarkers, nameof(my.FakePhraseMarkers), (their2, my2)
                     => CheckFloats(their2.StartMillis, my2.StartMillis, nameof(my2.StartMillis), 1f)
                     ?? CheckFloats(their2.LengthMillis, my2.LengthMillis, nameof(my2.LengthMillis), 1f)
                     ?? Check(their2.StartTicks, my2.StartTicks, nameof(my2.StartTicks))
                     ?? Check(their2.LengthTicks, my2.LengthTicks, nameof(my2.LengthTicks))
                     ?? Check(their2.StartNoteIdx, my2.StartNoteIdx, nameof(my2.StartNoteIdx))
                     ?? Check(their2.EndNoteIdx, my2.EndNoteIdx, nameof(my2.EndNoteIdx))
                     ?? Check(their2.HasPitchedVox, my2.HasPitchedVox, nameof(my2.HasPitchedVox))
                     ?? Check(their2.HasUnpitchedVox, my2.HasUnpitchedVox, nameof(my2.HasUnpitchedVox))
                     //?? Check(their2.LowNote, my2.LowNote, nameof(my2.LowNote))
                     //?? Check(their2.HighNote, my2.HighNote, nameof(my2.HighNote))
                     ?? Check(their2.PhraseFlags, my2.PhraseFlags, nameof(my2.PhraseFlags))
                     ?? Check(their2.PercussionSection, my2.PercussionSection, nameof(my2.PercussionSection)))
               ?? Check(their.Notes, my.Notes, nameof(my.Notes), (their2, my2)
                    =>
                       // TODO: enable after fixing phrases
                       null//Check(their2.PhraseIndex, my2.PhraseIndex, nameof(my2.PhraseIndex))
                    ?? Check(their2.MidiNote, my2.MidiNote, nameof(my2.MidiNote))
                    ?? Check(their2.MidiNote2, my2.MidiNote2, nameof(my2.MidiNote2))
                    ?? CheckFloats(their2.StartMillis, my2.StartMillis, nameof(my2.StartMillis), 0.4f)
                    ?? Check(their2.StartTick, my2.StartTick, nameof(my2.StartTick))
                    ?? CheckFloats(their2.LengthMillis, my2.LengthMillis, nameof(my2.LengthMillis), 0.4f)
                    ?? Check(their2.LengthTicks, my2.LengthTicks, nameof(my2.LengthTicks))
                    ?? Check(their2.Lyric, my2.Lyric, nameof(my2.Lyric))
                    ?? Check(their2.LastNoteInPhrase, my2.LastNoteInPhrase, nameof(my2.LastNoteInPhrase))
                    ?? Check(their2.False1, my2.False1, nameof(my2.False1))
                    ?? Check(their2.Unpitched, my2.Unpitched, nameof(my2.Unpitched))
                    ?? Check(their2.UnpitchedGenerous, my2.UnpitchedGenerous, nameof(my2.UnpitchedGenerous))
                    ?? Check(their2.RangeDivider, my2.RangeDivider, nameof(my2.RangeDivider))
                    ?? Check(their2.PhraseFlags, my2.PhraseFlags, nameof(my2.PhraseFlags))
                    ?? Check(their2.Portamento, my2.Portamento, nameof(my2.Portamento))
                    ?? Check(their2.LyricShift, my2.LyricShift, nameof(my2.LyricShift))
                    ?? Check(their2.ShowLyric, my2.ShowLyric, nameof(my2.ShowLyric))
                    )
               ?? Check(their.AuthoredPhraseMarkers, my.AuthoredPhraseMarkers, nameof(my.AuthoredPhraseMarkers), (their2, my2)
                     => CheckFloats(their2.StartMillis, my2.StartMillis, nameof(my2.StartMillis), 1f)
                     ?? CheckFloats(their2.LengthMillis, my2.LengthMillis, nameof(my2.LengthMillis), 1f)
                     ?? Check(their2.StartTicks, my2.StartTicks, nameof(my2.StartTicks))
                     ?? Check(their2.LengthTicks, my2.LengthTicks, nameof(my2.LengthTicks))
                     ?? Check(their2.StartNoteIdx, my2.StartNoteIdx, nameof(my2.StartNoteIdx))
                     ?? Check(their2.EndNoteIdx, my2.EndNoteIdx, nameof(my2.EndNoteIdx))
                     ?? Check(their2.HasPitchedVox, my2.HasPitchedVox, nameof(my2.HasPitchedVox))
                     ?? Check(their2.HasUnpitchedVox, my2.HasUnpitchedVox, nameof(my2.HasUnpitchedVox))
                     ?? Check(their2.LowNote, my2.LowNote, nameof(my2.LowNote))
                     ?? Check(their2.HighNote, my2.HighNote, nameof(my2.HighNote))
                     ?? Check(their2.PhraseFlags, my2.PhraseFlags, nameof(my2.PhraseFlags))
                     ?? Check(their2.PercussionSection, my2.PercussionSection, nameof(my2.PercussionSection))))
          ?? Check(other.UnknownOne, UnknownOne, nameof(UnknownOne))
          ?? Check(other.UnknownNegOne, UnknownNegOne, nameof(UnknownNegOne))
          ?? Check(other.UnknownHundred, UnknownHundred, nameof(UnknownHundred))
          ?? Check(other.Unknown4, Unknown4, nameof(Unknown4), (their, my)
               // TODO: What is this?
               => // Check(their.Tick, my.Tick, nameof(my.Tick))
                  // ?? Check(their.FloatData, my.FloatData, nameof(my.FloatData))
               null
               )
          //?? Check(other.VocalRange, VocalRange, nameof(VocalRange), (their, my)
          //     => Check(their.StartMillis, my.StartMillis, nameof(my.StartMillis))
          //     ?? Check(their.StartTicks, my.StartTicks, nameof(my.StartTicks))
          //     ?? Check(their.LowNote, my.LowNote, nameof(my.LowNote))
          //     ?? Check(their.HighNote, my.HighNote, nameof(my.HighNote)))
          ?? Check(other.HopoThreshold, HopoThreshold, nameof(HopoThreshold))
          ?? Check(other.NumPlayableTracks, NumPlayableTracks, nameof(NumPlayableTracks))
          ?? Check(other.FinalEventTick, FinalEventTick, nameof(FinalEventTick))
          ?? Check(other.UnknownZeroByte, UnknownZeroByte, nameof(UnknownZeroByte))
          ?? CheckFloats(other.PreviewStartMillis, PreviewStartMillis, nameof(PreviewStartMillis))
          ?? CheckFloats(other.PreviewEndMillis, PreviewEndMillis, nameof(PreviewEndMillis))
          ?? Check(other.HandMaps, HandMaps, nameof(HandMaps), (their, my)
               => CheckFloats(their.StartTime, my.StartTime, nameof(my.StartTime), 0.1f)
               ?? Check(their.Map, my.Map, nameof(my.Map)))
          ?? Check(other.GuitarLeftHandPos, GuitarLeftHandPos, nameof(GuitarLeftHandPos), (their, my)
               => CheckFloats(their.StartTime, my.StartTime, nameof(my.StartTime), 0.0002f)
               ?? CheckFloats(their.Length, my.Length, nameof(my.Length), 0.2f)
               ?? Check(their.Position, my.Position, nameof(my.Position))
               //?? Check(their.Unknown, my.Unknown, nameof(my.Unknown))
               )
          ?? Check(other.StrumMaps, StrumMaps, nameof(StrumMaps), (their, my)
                    => CheckFloats(their.StartTime, my.StartTime, nameof(my.StartTime))
                    ?? Check(their.Map, my.Map, nameof(my.Map)))
          ?? Check(other.MarkupSoloNotes1, MarkupSoloNotes1, nameof(MarkupSoloNotes1), CheckSoloNotes)
          ?? Check(other.MarkupLoop1, MarkupLoop1, nameof(MarkupLoop1), CheckTwoTick)
          ?? Check(other.MarkupChords1, MarkupChords1, nameof(MarkupChords1), (their, my)
               => Check(their.StartTick, my.StartTick, nameof(my.StartTick))
               ?? Check(their.EndTick, my.EndTick, nameof(my.EndTick))
               ?? Check(their.Pitches, my.Pitches, nameof(my.Pitches), Check))
          ?? Check(other.MarkupSoloNotes2, MarkupSoloNotes2, nameof(MarkupSoloNotes2), CheckSoloNotes)
          ?? Check(other.MarkupSoloNotes3, MarkupSoloNotes3, nameof(MarkupSoloNotes3), CheckSoloNotes)
          ?? Check(other.MarkupLoop2, MarkupLoop2, nameof(MarkupLoop2), CheckTwoTick)
          ?? Check(other.MidiSongResourceMagic, MidiSongResourceMagic, nameof(MidiSongResourceMagic))
          ?? Check(other.LastTrackFinalTick, LastTrackFinalTick, nameof(LastTrackFinalTick))
          ?? Check(other.MidiTracks, MidiTracks, nameof(MidiTracks), (their, my)
               => Check(their.Name, my.Name, nameof(my.Name))
               ?? Check(their.TotalTicks, my.TotalTicks, nameof(my.TotalTicks))
               ?? Check(their.Messages, my.Messages, nameof(my.Messages), (IMidiMessage their2, IMidiMessage my2)
                    => Check(their2.DeltaTime, my2.DeltaTime, nameof(my2.DeltaTime))
                    ?? Check(their2.PrettyString, my2.PrettyString, "<pretty_string>")))
          ?? Check(other.FinalTick, FinalTick, nameof(FinalTick))
          //?? Check(other.Measures, Measures, nameof(Measures))
          ?? Check(other.Unknown, Unknown, nameof(Unknown), Check)
          ?? Check(other.FinalTickMinusOne, FinalTickMinusOne, nameof(FinalTickMinusOne))
          // TODO: Floats are sometimes 0xABCDABCD ???
          //?? Check(other.UnknownFloats, UnknownFloats, nameof(UnknownFloats), Check)
          ?? Check(other.Tempos, Tempos, nameof(Tempos), (their, my)
               => CheckFloats(their.StartMillis, my.StartMillis, nameof(my.StartMillis), 0.3f)
               ?? Check(their.StartTick, my.StartTick, nameof(my.StartTick))
               // TODO: Fix precision of tempo conversions...
               ?? CheckFloats(their.Tempo, my.Tempo, nameof(my.Tempo), 2))
          ?? Check(other.TimeSigs, TimeSigs, nameof(TimeSigs), (their, my)
               => Check(their.Tick, my.Tick, nameof(my.Tick))
               ?? Check(their.Measure, my.Measure, nameof(my.Measure))
               ?? Check(their.Numerator, my.Numerator, nameof(my.Numerator))
               ?? Check(their.Denominator, my.Denominator, nameof(my.Denominator)))
          ?? Check(other.Beats, Beats, nameof(Beats), (their, my)
               => Check(their.Tick, my.Tick, nameof(my.Tick))
               ?? Check(their.Downbeat, my.Downbeat, nameof(my.Downbeat)))
          ?? Check(other.UnknownZero, UnknownZero, nameof(UnknownZero))
          ?? Check(other.MidiTrackNames, MidiTrackNames, nameof(MidiTrackNames), Check);
    }
}

namespace LibForge.Midi
{
    public class RBMidConverter
    {
        public static RBMid ToRBMid(MidiFile mf, int hopoThreshold = 170, Action<string> warner = null)
        {
            return new MidiConverter(mf, hopoThreshold, warner).ToRBMid();
        }
        public static MidiFile ToMid(RBMid m)
        {
            return new MidiFile(MidiFormat.MultiTrack, new List<MidiTrack>(m.MidiTracks), 480);
        }
        public static MidiFileResource ToMidiFileResource(MidiFile mf)
        {
            var mfr = new MidiFileResource();
            MidiConverter.ReadMidiFileResourceFromMidiFile(mfr, mf, mf.Tracks, new List<uint>() { 0U });
            return mfr;
        }

        public class MidiConverter
        {
            private MidiFile mf;
            private Action<string> warnAction;
            private void Warn(string msg)
            {
                warnAction?.Invoke(msg);
            }
            private RBMid rb;
            private List<MidiTrackProcessed> processedTracks;

            private List<RBMid.LYRICS> Lyrics;
            private List<RBMid.DRUMFILLS> DrumFills;
            private List<RBMid.ANIM> Anims;
            private List<RBMid.TOMMARKER> ProMarkers;
            private List<RBMid.LANEMARKER> LaneMarkers;
            private List<RBMid.GTRTRILLS> TrillMarkers;
            private List<RBMid.DRUMMIXES> DrumMixes;
            private List<RBMid.GEMTRACK> GemTracks;
            private List<RBMid.SECTIONS> OverdriveSoloSections;
            private List<RBMid.VOCALTRACK> VocalTracks;
            private List<RBMid.UNKSTRUCT1> Unknown4;
            private List<RBMid.VocalTrackRange> VocalRanges;
            // TODO: multiple ranges? that's probably a thing?
            private RBMid.VocalTrackRange theVocalRange = new RBMid.VocalTrackRange { LowNote = 100, HighNote = 0 };
            private List<RBMid.MAP[]> HandMap;
            private List<RBMid.HANDPOS[]> HandPos;
            private List<RBMid.MAP[]> strumMaps;
            private List<RBMid.MARKUP_SOLO_NOTES> MarkupSoloNotes1, MarkupSoloNotes2, MarkupSoloNotes3;
            private List<RBMid.TWOTICKS> SoloLoops1, SoloLoops2;
            private List<RBMid.MARKUPCHORD> MarkupChords1;            
            private float PreviewStart;
            private float PreviewEnd;
            private uint LastMarkupTick;
            private int hopoThreshold;

            private List<uint> MeasureTicks = new List<uint>() { 0U };

            public static void ReadMidiFileResourceFromMidiFile(MidiFileResource mfr, MidiFile mf, List<MidiTrack> tracks, List<uint> MeasureTicks)
            {
                var Tempos = new List<MidiFileResource.TEMPO>();
                var TimeSigs = new List<MidiFileResource.TIMESIG>();
                var Beats = new List<MidiFileResource.BEAT>();
                var TempoMap = mf.TempoTimeSigMap;
                var lastTimeSig = mf.TempoTimeSigMap[0];
                var measure = 0;
                foreach (var tempo in TempoMap)
                {
                    if (tempo.NewTempo)
                        Tempos.Add(new MidiFileResource.TEMPO
                        {
                            StartTick = (uint)tempo.Tick,
                            StartMillis = (float)(tempo.Time * 1000.0),
                            Tempo = (int)(60_000_000 / (float)tempo.BPM)
                        });
                    if (tempo.NewTimeSig)
                    {
                        if (tempo.Tick > 0)
                        {
                            var elapsed = tempo.Tick - lastTimeSig.Tick;
                            var ticksPerBeat = (480 * 4) / lastTimeSig.Denominator;
                            measure += (int)(elapsed / ticksPerBeat / lastTimeSig.Numerator);
                            var lastMeasureTick = MeasureTicks.LastOrDefault();
                            for (var i = MeasureTicks.Count; i < measure; i++)
                            {
                                lastMeasureTick += 480U * lastTimeSig.Numerator * 4 / lastTimeSig.Denominator;
                                MeasureTicks.Add(lastMeasureTick);
                            }
                        }
                        TimeSigs.Add(new MidiFileResource.TIMESIG
                        {
                            Numerator = tempo.Numerator,
                            Denominator = tempo.Denominator,
                            Tick = (uint)tempo.Tick,
                            Measure = measure
                        });
                        lastTimeSig = tempo;
                    }
                };
                var FinalTick = tracks.Select(t => t.TotalTicks).Max();
                uint lastTimeSigTicksPerMeasure = 480U * lastTimeSig.Numerator * 4 / lastTimeSig.Denominator;
                for (uint lastMeasureTick2 = MeasureTicks.LastOrDefault() + lastTimeSigTicksPerMeasure; lastMeasureTick2 < FinalTick; lastMeasureTick2 += lastTimeSigTicksPerMeasure)
                {
                    MeasureTicks.Add(lastMeasureTick2);
                }

                foreach (var item in MidiHelper.ToAbsolute(tracks.Where(t => t.Name == "BEAT").First().Messages))
                {
                    switch (item)
                    {
                        case NoteOnEvent e:
                            switch (e.Key)
                            {
                                case 12:
                                case 13:
                                    Beats.Add(new RBMid.BEAT
                                    {
                                        Tick = e.DeltaTime,
                                        Downbeat = e.Key == 12
                                    });
                                    break;
                            }
                            break;
                    }
                }

                mfr.MidiSongResourceMagic = 2;
                mfr.LastTrackFinalTick = (uint)tracks.Select(t => t.TotalTicks).LastOrDefault();
                mfr.MidiTracks = new MidiTrack[tracks.Count];
                for (int i = 0; i < mfr.MidiTracks.Length; i++)
                {
                    mfr.MidiTracks[i] = new MidiTrack(tracks[i].Messages.Select(x =>
                      x is NoteOnEvent e && e.Velocity == 0 ? new NoteOffEvent(e.DeltaTime, e.Channel, e.Key, e.Velocity) : x
                    ).ToList(), tracks[i].TotalTicks, tracks[i].Name);
                }
                mfr.FinalTick = (uint)tracks.Select(t => t.TotalTicks).Max();
                mfr.Measures = (uint)MeasureTicks.Count();
                mfr.Unknown = new uint[6];
                mfr.FinalTickMinusOne = mfr.FinalTick - 1;
                mfr.UnknownFloats = new float[4] { -1, -1, -1, -1 };
                mfr.Tempos = Tempos.ToArray();
                mfr.TimeSigs = TimeSigs.ToArray();
                mfr.Beats = Beats.ToArray();
                mfr.UnknownZero = 0;
                mfr.MidiTrackNames = tracks.Select(t => t.Name).ToArray();
            }

            public MidiConverter(MidiFile mf, int hopoThreshold = 170, Action<string> warnAction = null)
            {
                this.mf = mf;
                this.warnAction = warnAction;
                this.hopoThreshold = hopoThreshold;
                processedTracks = new MidiHelper().ProcessTracks(mf);
                trackHandlers = new Dictionary<string, Action<MidiTrackProcessed>>
        {
          {"PART DRUMS", HandleDrumTrk },
          {"PART BASS", HandleGuitarBass },
          {"PART GUITAR", HandleGuitarBass },
          {"PART REAL_KEYS_X", HandleRealKeysXTrk },
          {"PART KEYS_ANIM_RH", HandleKeysAnimTrk },
          {"PART KEYS_ANIM_LH", HandleKeysAnimTrk },
          {"PART VOCALS", HandleVocalsTrk },
          {"HARM1", HandleVocalsTrk },
          {"HARM2", HandleVocalsTrk },
          {"HARM3", HandleVocalsTrk },
          {"EVENTS", HandleEventsTrk },
          {"MARKUP", HandleMarkupTrk },
          {"VENUE", HandleVenueTrk }
        };
            }

            public RBMid ToRBMid()
            {
                rb = new RBMid();
                var processedMidiTracks = ConvertVenueTrack(mf.Tracks);
                ReadMidiFileResourceFromMidiFile(rb, mf, processedMidiTracks, MeasureTicks);
                rb.Format = 0x10;

                Lyrics = new List<RBMid.LYRICS>();
                DrumFills = new List<RBMid.DRUMFILLS>();
                Anims = new List<RBMid.ANIM>();
                ProMarkers = new List<RBMid.TOMMARKER>();
                LaneMarkers = new List<RBMid.LANEMARKER>();
                TrillMarkers = new List<RBMid.GTRTRILLS>();
                DrumMixes = new List<RBMid.DRUMMIXES>();
                GemTracks = new List<RBMid.GEMTRACK>();
                OverdriveSoloSections = new List<RBMid.SECTIONS>();
                VocalTracks = new List<RBMid.VOCALTRACK>();
                Unknown4 = new List<RBMid.UNKSTRUCT1>();
                VocalRanges = new List<RBMid.VocalTrackRange>();
                HandMap = new List<RBMid.MAP[]>();
                HandPos = new List<RBMid.HANDPOS[]>();
                strumMaps = new List<RBMid.MAP[]>();
                MarkupSoloNotes1 = new List<RBMid.MARKUP_SOLO_NOTES>();
                SoloLoops1 = new List<RBMid.TWOTICKS>();
                MarkupChords1 = new List<RBMid.MARKUPCHORD>();
                MarkupSoloNotes2 = new List<RBMid.MARKUP_SOLO_NOTES>();
                MarkupSoloNotes3 = new List<RBMid.MARKUP_SOLO_NOTES>();
                SoloLoops2 = new List<RBMid.TWOTICKS>();
                var trackNames = new[] {
          "PART DRUMS",
          "PART BASS",
          "PART REAL_BASS",
          "PART GUITAR",
          "PART REAL_GUITAR",
          // TODO: Allow these in release builds when shit's no longer borked
#if DEBUG
          "PART KEYS",
          "PART REAL_KEYS_X",
          "PART REAL_KEYS_H",
          "PART REAL_KEYS_M",
          "PART REAL_KEYS_E",
          "PART KEYS_ANIM_RH",
          "PART KEYS_ANIM_LH",
#endif
          "PART VOCALS",
          "HARM1",
          "HARM2",
          "HARM3",
          "EVENTS",
          "BEAT",
          "MARKUP",
        };
                foreach (var trackname in trackNames)
                {
                    var track = processedTracks.Where(x => x.Name == trackname).FirstOrDefault();
                    if (track != null && trackHandlers.ContainsKey(track.Name))
                    {
                        trackHandlers[track.Name](track);
                    }
                }
                VocalRanges.Add(theVocalRange);
                rb.Lyrics = Lyrics.ToArray();
                rb.DrumFills = DrumFills.ToArray();
                rb.Anims = Anims.ToArray();
                rb.ProMarkers = ProMarkers.ToArray();
                rb.LaneMarkers = LaneMarkers.ToArray();
                rb.TrillMarkers = TrillMarkers.ToArray();
                rb.DrumMixes = DrumMixes.ToArray();
                rb.GemTracks = GemTracks.ToArray();
                rb.OverdriveSoloSections = OverdriveSoloSections.ToArray();
                rb.VocalTracks = VocalTracks.ToArray();
                rb.Unknown4 = Unknown4.ToArray();
                rb.VocalRange = VocalRanges.ToArray();
                rb.HandMaps = HandMap.ToArray();
                rb.GuitarLeftHandPos = HandPos.ToArray();
                rb.StrumMaps = strumMaps.ToArray();
                rb.MarkupSoloNotes1 = MarkupSoloNotes1.ToArray();
                rb.MarkupLoop1 = SoloLoops1.ToArray();
                rb.MarkupChords1 = MarkupChords1.ToArray();
                rb.MarkupSoloNotes2 = MarkupSoloNotes2.ToArray();
                rb.MarkupSoloNotes3 = MarkupSoloNotes3.ToArray();
                rb.MarkupLoop2 = SoloLoops2.ToArray();
                rb.PreviewStartMillis = PreviewStart;
                rb.PreviewEndMillis = PreviewEnd;
                rb.NumPlayableTracks = (uint)Lyrics.Count;
                rb.FinalEventTick = processedTracks.Where(t => t.Name == "EVENTS").Select(t => t.LastTick).First();
                rb.UnknownHundred = 100f;
                rb.UnknownNegOne = -1;
                rb.UnknownOne = 1;
                rb.UnknownZeroByte = 0;
                rb.UnknownZero = 0;
                rb.HopoThreshold = hopoThreshold;
                return rb;
            }
            private Dictionary<string, Action<MidiTrackProcessed>> trackHandlers;

            private float GetMillis(uint tick)
            {
                var tempo = mf.TempoTimeSigMap.Last(e => e.Tick <= tick);
                return (float)(tempo.Time + ((tick - tempo.Tick) / 480.0) * (60 / tempo.BPM)) * 1000f;
            }

            const byte Roll2 = 127;
            const byte Roll1 = 126;
            const byte DrumFillMarkerEnd = 124;
            const byte DrumFillMarkerStart = 120;
            const byte OverdriveMarker = 116;
            const byte ProGreen = 112;
            const byte ProBlue = 111;
            const byte ProYellow = 110;
            const byte SoloMarker = 103;
            const byte ExpertHopoOff = 102;
            const byte ExpertHopoOn = 101;
            const byte ExpertEnd = 100;
            const byte ExpertStart = 96;
            const byte HardHopoOff = 90;
            const byte HardHopoOn = 89;
            const byte HardEnd = 88;
            const byte HardStart = 84;
            const byte MediumEnd = 76;
            const byte MediumStart = 72;
            const byte EasyEnd = 64;
            const byte EasyStart = 60;
            const byte DrumAnimEnd = 51;
            const byte DrumAnimStart = 24;
            private void HandleDrumTrk(MidiTrackProcessed track)
            {
                var drumfills = new List<RBMid.DRUMFILLS.FILL>();
                var fills_unk = new List<RBMid.DRUMFILLS.FILL_LANES>();
                var tom_markers = new SortedDictionary<uint, RBMid.TOMMARKER.MARKER>();
                var overdrive_markers = new List<RBMid.SECTIONS.SECTION>();
                var solo_markers = new List<RBMid.SECTIONS.SECTION>();
                var gem_tracks = new List<RBMid.GEMTRACK.GEM>[4];
                var rolls = new List<RBMid.LANEMARKER.MARKER>();

                tom_markers[0] = new RBMid.TOMMARKER.MARKER
                {
                    Tick = 0,
                    Flags = 0
                };
                var marker_ends = new uint[3];
                var mixes = new List<RBMid.TICKTEXT>[4];
                for (var i = 0; i < 4; i++)
                {
                    mixes[i] = new List<RBMid.TICKTEXT>();
                }
                void SetMarkerOn(uint tick, RBMid.TOMMARKER.MARKER.FLAGS flag)
                {
                    if (tom_markers.ContainsKey(tick))
                    {
                        tom_markers[tick].Flags |= flag;
                    }
                    else
                    {
                        var active_flag = 4;
                        foreach (var end in marker_ends)
                        {
                            if (end > tick)
                            {
                                flag |= (RBMid.TOMMARKER.MARKER.FLAGS)active_flag;
                            }
                            active_flag <<= 1;
                        }
                        tom_markers[tick] = new RBMid.TOMMARKER.MARKER
                        {
                            Tick = tick,
                            Flags = flag
                        };
                    }
                    // HACK for superunknownrb4 which has a badly quantized PRO marker
                    for (var diff = 2; diff < 4; diff++)
                    {
                        var count = gem_tracks[diff]?.Count ?? 0;
                        if (count > 0
                          && tick - gem_tracks[diff][count - 1].StartTicks < 5
                          && (gem_tracks[diff][count - 1].Lanes & (int)flag) != 0)
                        {
                            gem_tracks[diff][count - 1].ProCymbal = 0;
                        }
                    }
                }
                void SetMarkerOff(uint tick, RBMid.TOMMARKER.MARKER.FLAGS flag)
                {
                    if (tom_markers.ContainsKey(tick))
                    {
                        tom_markers[tick].Flags &= ~flag;
                    }
                    else
                    {
                        RBMid.TOMMARKER.MARKER.FLAGS new_flag = 0;
                        var active_flag = 4;
                        foreach (var end in marker_ends)
                        {
                            if (end > tick)
                            {
                                new_flag |= (RBMid.TOMMARKER.MARKER.FLAGS)active_flag;
                            }
                            active_flag <<= 1;
                        }
                        tom_markers[tick] = new RBMid.TOMMARKER.MARKER
                        {
                            Tick = tick,
                            Flags = new_flag
                        };
                    }
                }
                RBMid.TOMMARKER.MARKER.FLAGS GetFlag(byte key)
                {
                    switch (key)
                    {
                        case ProYellow:
                            return RBMid.TOMMARKER.MARKER.FLAGS.ProYellow;
                        case ProBlue:
                            return RBMid.TOMMARKER.MARKER.FLAGS.ProBlue;
                        case ProGreen:
                            return RBMid.TOMMARKER.MARKER.FLAGS.ProGreen;
                    }
                    return 0;
                }
                bool AddGem(MidiNote e)
                {
                    var key = e.Key;
                    var lane = 0;
                    var diff = 0;
                    if (key >= EasyStart && key <= EasyEnd)
                    {
                        lane = key - EasyStart;
                        diff = 0;
                    }
                    else if (key >= MediumStart && key <= MediumEnd)
                    {
                        lane = key - MediumStart;
                        diff = 1;
                    }
                    else if (key >= HardStart && key <= HardEnd)
                    {
                        lane = key - HardStart;
                        diff = 2;
                    }
                    else if (key >= ExpertStart && key <= ExpertEnd)
                    {
                        lane = key - ExpertStart;
                        diff = 3;
                    }
                    else
                    {
                        return false;
                    }

                    if (diff == 3 && rolls.Count > 0 && rolls[rolls.Count - 1].EndTick > e.StartTicks)
                    {
                        var tmp = rolls[rolls.Count - 1];
                        tmp.Lanes |= 1 << lane;
                        rolls[rolls.Count - 1] = tmp;
                    }
                    if (gem_tracks[diff] == null) gem_tracks[diff] = new List<RBMid.GEMTRACK.GEM>();
                    var lastOverdrive = overdrive_markers.LastOrDefault();
                    var proCymbal = (lane > 1 && marker_ends[lane - 2] <= e.StartTicks) ? 1 : 0;
                    gem_tracks[diff].Add(new RBMid.GEMTRACK.GEM
                    {
                        StartMillis = (float)e.StartTime * 1000,
                        StartTicks = e.StartTicks,
                        LengthMillis = (ushort)(e.Length * 1000),
                        LengthTicks = (ushort)e.LengthTicks,
                        Lanes = 1 << lane,
                        IsHopo = false,
                        NoTail = true,
                        // TODO: Sometimes this is not zero
                        ProCymbal = proCymbal
                    });
                    return true;
                }
                // 
                var itemsOrdered = track.Items
                  // If shorter notes come first we get better output for arabella (seems to be breaking things)
                  .OrderBy(x => (x as MidiNote)?.LengthTicks ?? 0)
                  .OrderBy(x => {
                      // Sort modifiers to come before gems
                      var key = (x as MidiNote)?.Key ?? 0;
                      if (key <= ExpertEnd) key = 127;
                      return key;
                  })
                  .OrderBy(x => x.StartTicks);
                foreach (var item in itemsOrdered)
                {
                    var ticks = item.StartTicks;
                    var time = item.StartTime;
                    switch (item)
                    {
                        case MidiNote e:
                            if (e.Key == DrumFillMarkerStart)
                            {
                                fills_unk.Add(new RBMid.DRUMFILLS.FILL_LANES
                                {
                                    Tick = ticks,
                                    Lanes = 0b11111 // TODO: parse each lane
                                });
                                drumfills.Add(new RBMid.DRUMFILLS.FILL
                                {
                                    StartTick = ticks,
                                    EndTick = ticks + e.LengthTicks, // TODO: this seems to be rounded up to the next note
                                    IsBRE = 0
                                });
                            }
                            else if (e.Key >= DrumFillMarkerStart && e.Key <= DrumFillMarkerEnd) { }
                            else if (e.Key == OverdriveMarker)
                            {
                                overdrive_markers.Add(new RBMid.SECTIONS.SECTION
                                {
                                    StartTicks = ticks,
                                    LengthTicks = e.LengthTicks
                                });
                            }
                            else if (e.Key == SoloMarker)
                            {
                                solo_markers.Add(new RBMid.SECTIONS.SECTION
                                {
                                    StartTicks = ticks,
                                    LengthTicks = e.LengthTicks
                                });
                            }
                            else if (e.Key == ProYellow || e.Key == ProBlue || e.Key == ProGreen)
                            {
                                // Pro Tom Markers
                                var flag = GetFlag(e.Key);
                                SetMarkerOn(e.StartTicks, flag);
                                SetMarkerOff(e.StartTicks + e.LengthTicks, flag);
                                marker_ends[e.Key - ProYellow] = e.StartTicks + e.LengthTicks;
                                foreach (var x in tom_markers.Values.Where(k => k.Tick >= e.StartTicks && k.Tick < e.StartTicks + e.LengthTicks))
                                {
                                    x.Flags |= flag;
                                }
                            }
                            else if (AddGem(e)) { }  // everything is handled in AddGem
                            else if (e.Key >= DrumAnimStart && e.Key <= DrumAnimEnd)
                            {
                                // Animations are handled by the game engine: it parses the MIDI track.
                            }
                            else if (e.Key == Roll1 || e.Key == Roll2)
                            {
                                rolls.Add(new RBMid.LANEMARKER.MARKER
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    Lanes = 0
                                });
                            }
                            else if (e.Key == 105 || e.Key == 106 || e.Key == 12 || e.Key == 13 || e.Key == 14 || e.Key == 15)
                            {
                                // TODO: What are these note?
                            }
                            else
                            {
                                Warn($"Unhandled midi note {e.Key} in drum track at time {e.StartTime}");
                            }
                            break;
                        case MidiText e:
                            switch (e.Text)
                            {
                                default:
                                    var regex = new System.Text.RegularExpressions.Regex("\\[mix ([0-9]) (\\S+)\\]");
                                    var match = regex.Match(e.Text);
                                    if (match.Success)
                                    {
                                        var difficulty = Int32.Parse(match.Groups[1].Value);
                                        var mix = match.Groups[2].Value;
                                        mixes[difficulty].Add(new RBMid.TICKTEXT
                                        {
                                            Text = mix,
                                            Tick = e.StartTicks
                                        });
                                    }
                                    break;
                            }
                            break;
                    }
                }
                Lyrics.Add(new RBMid.LYRICS
                {
                    TrackName = track.Name,
                    Lyrics = new RBMid.TICKTEXT[0],
                    Unknown1 = 0,
                    Unknown2 = 0,
                    Unknown3 = 1
                });
                DrumFills.Add(new RBMid.DRUMFILLS
                {
                    Fills = drumfills.ToArray(),
                    Lanes = fills_unk.ToArray()
                });
                ProMarkers.Add(new RBMid.TOMMARKER
                {
                    Markers = tom_markers.Values.ToArray()
                });
                LaneMarkers.Add(new RBMid.LANEMARKER
                {
                    Markers = rolls.Count == 0 ? new RBMid.LANEMARKER.MARKER[0][] : new RBMid.LANEMARKER.MARKER[4][]
                  {
            new RBMid.LANEMARKER.MARKER[0],
            new RBMid.LANEMARKER.MARKER[0],
            new RBMid.LANEMARKER.MARKER[0],
            rolls.ToArray()
                  }
                });
                TrillMarkers.Add(new RBMid.GTRTRILLS { Trills = new RBMid.GTRTRILLS.TRILL[0][] });
                DrumMixes.Add(new RBMid.DRUMMIXES
                {
                    Mixes = mixes.Select(m => m.ToArray()).ToArray()
                });
                GemTracks.Add(new RBMid.GEMTRACK
                {
                    Gems = gem_tracks.Select(g => g.ToArray()).ToArray(),
                    HopoThreshold = hopoThreshold
                });
                var sections = new RBMid.SECTIONS.SECTION[6][] {
          overdrive_markers.ToArray(),
          solo_markers.ToArray(),
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0]
        };
                OverdriveSoloSections.Add(new RBMid.SECTIONS
                {
                    Sections = new RBMid.SECTIONS.SECTION[4][][]
                  {
            sections, sections, sections, sections
                  }
                });
                HandMap.Add(new RBMid.MAP[0]);
                HandPos.Add(new RBMid.HANDPOS[0]);
                strumMaps.Add(new RBMid.MAP[0]);
            }

            private static Dictionary<string, int> HandMaps = new Dictionary<string, int>
      {
        {"HandMap_Default", 0 },
        {"HandMap_AllBend", 1 },
        {"HandMap_AllChords", 2 },
        {"HandMap_Chord_A", 3 },
        {"HandMap_Chord_C", 4 },
        {"HandMap_Chord_D", 5 },
        {"HandMap_DropD", 6 },
        {"HandMap_DropD2", 7 },
        {"HandMap_NoChords", 8 },
        {"HandMap_Solo", 9 },
      };

            private static Dictionary<string, int> StrumMaps = new Dictionary<string, int>
      {
        {"StrumMap_Default", 0 },
        {"StrumMap_Pick", 1 },
        {"StrumMap_SlapBass", 2 },
      };

            const byte TrillMarker = 127;
            const byte TremoloMarker = 126;
            const byte LeftHandEnd = 59;
            const byte LeftHandStart = 40;
            struct Hopo
            {
                public uint EndTick;
                public enum State { NormalOff, NormalOn, ForcedOn, ForcedOff }
                public State state;
            };
            private void HandleGuitarBass(MidiTrackProcessed track)
            {
                var drumfills = new List<RBMid.DRUMFILLS.FILL>();
                var fills_unk = new List<RBMid.DRUMFILLS.FILL_LANES>();
                var gem_tracks = new List<RBMid.GEMTRACK.GEM>[4];
                RBMid.GEMTRACK.GEM[] chords = new RBMid.GEMTRACK.GEM[4];
                var trills = new List<RBMid.GTRTRILLS.TRILL>();
                var trill = new RBMid.GTRTRILLS.TRILL();
                var maps = new List<RBMid.MAP>();
                var left_hand = new List<RBMid.HANDPOS>();
                var overdrive_markers = new List<RBMid.SECTIONS.SECTION>();
                var solo_markers = new List<RBMid.SECTIONS.SECTION>();
                var tremolos = new List<RBMid.LANEMARKER.MARKER>();
                var strummaps = new List<RBMid.MAP>();
                var hopoState = new Hopo[]{
          new Hopo { EndTick = uint.MaxValue, state = Hopo.State.NormalOff },
          new Hopo { EndTick = uint.MaxValue, state = Hopo.State.NormalOff },
          new Hopo { EndTick = uint.MaxValue, state = Hopo.State.NormalOff },
          new Hopo { EndTick = uint.MaxValue, state = Hopo.State.NormalOff },
        };

                bool AddGem(MidiNote e)
                {
                    var key = e.Key;
                    var lane = 0;
                    var diff = 0;
                    if (key >= EasyStart && key <= EasyEnd)
                    {
                        lane = key - EasyStart;
                        diff = 0;
                    }
                    else if (key >= MediumStart && key <= MediumEnd)
                    {
                        lane = key - MediumStart;
                        diff = 1;
                    }
                    else if (key >= HardStart && key <= HardEnd)
                    {
                        lane = key - HardStart;
                        diff = 2;
                    }
                    else if (key >= ExpertStart && key <= ExpertEnd)
                    {
                        lane = key - ExpertStart;
                        diff = 3;
                        if (trill.EndTick > e.StartTicks)
                        {
                            if (trill.FirstFret == -1)
                            {
                                trill.FirstFret = lane;
                            }
                            else if (trill.SecondFret == -1)
                            {
                                trill.SecondFret = lane;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    if (diff == 3 && tremolos.Count > 0 && tremolos[tremolos.Count - 1].EndTick > e.StartTicks)
                    {
                        var tmp = tremolos[tremolos.Count - 1];
                        tmp.Lanes |= 1 << lane;
                        tremolos[tremolos.Count - 1] = tmp;
                    }
                    if (gem_tracks[diff] == null) gem_tracks[diff] = new List<RBMid.GEMTRACK.GEM>();
                    if (chords[diff] != null && e.StartTicks - chords[diff].StartTicks < 5)
                    { // additional gem in a chord
                        if (chords[diff].Lanes != 0 && hopoState[diff].state != Hopo.State.ForcedOn || hopoState[diff].EndTick <= e.StartTicks)
                        {
                            // chords are not automatically HOPO'd
                            chords[diff].IsHopo = false;
                        }
                        chords[diff].Lanes |= (1 << lane);

                        if (gem_tracks[diff].Count > 0
                          && 0 != (gem_tracks[diff].Last().Lanes & chords[diff].Lanes)
                          && (Hopo.State.ForcedOn != hopoState[diff].state || hopoState[diff].EndTick < e.StartTicks))
                        {
                            chords[diff].IsHopo = false;
                        }
                        chords[diff].ProCymbal = (chords[diff].Lanes & 3) != 0 ? 0 : 1;
                    }
                    else
                    { // new chord

                        bool hopo = false;
                        if (chords[diff] != null)
                        {
                            if (e.StartTicks - chords[diff].StartTicks <= hopoThreshold && ((1 << lane) & chords[diff].Lanes) == 0)
                            {
                                if (hopoState[diff].state != Hopo.State.ForcedOff || hopoState[diff].EndTick <= e.StartTicks)
                                    hopo = true;
                            }
                        }
                        if (hopoState[diff].state == Hopo.State.ForcedOn && hopoState[diff].EndTick > e.StartTicks)
                            hopo = true;
                        var chord = new RBMid.GEMTRACK.GEM
                        {
                            StartMillis = (float)e.StartTime * 1000,
                            StartTicks = e.StartTicks,
                            LengthMillis = (ushort)(e.Length * 1000),
                            LengthTicks = (ushort)e.LengthTicks,
                            Lanes = 1 << lane,
                            IsHopo = diff > 1 ? hopo : false,
                            NoTail = e.LengthTicks <= 120 || (hopo && e.LengthTicks <= 160) || (diff <= 2 && e.LengthTicks <= 160),
                            ProCymbal = lane > 1 ? 1 : 0
                        };
                        chords[diff] = chord;
                        gem_tracks[diff].Add(chord);
                    }
                    return true;
                }
                bool AddHopo(MidiNote e)
                {
                    var key = e.Key;
                    var diff = 0;
                    bool force;
                    switch (e.Key)
                    {
                        case ExpertHopoOff:
                            diff = 3;
                            hopoState[diff].state = Hopo.State.ForcedOff;
                            force = false;
                            break;
                        case ExpertHopoOn:
                            diff = 3;
                            force = true;
                            hopoState[diff].state = Hopo.State.ForcedOn;
                            break;
                        case HardHopoOff:
                            diff = 2;
                            force = false;
                            hopoState[diff].state = Hopo.State.ForcedOff;
                            break;
                        case HardHopoOn:
                            diff = 2;
                            force = true;
                            hopoState[diff].state = Hopo.State.ForcedOn;
                            break;
                        default:
                            return false;
                    }
                    hopoState[diff].EndTick = e.StartTicks + e.LengthTicks;
                    if (chords[diff] != null && chords[diff].StartTicks == e.StartTicks)
                    {
                        chords[diff].IsHopo = force;
                    }
                    return true;
                }
                foreach (var item in track.Items
                  .OrderBy(e => 127 - (e as MidiNote)?.Key ?? 0)
                  .OrderBy(e => e.StartTicks))
                {
                    switch (item)
                    {
                        case MidiNote e:
                            if (e.Key == DrumFillMarkerStart)
                            {
                                fills_unk.Add(new RBMid.DRUMFILLS.FILL_LANES
                                {
                                    Tick = e.StartTicks,
                                    Lanes = 31
                                });
                                drumfills.Add(new RBMid.DRUMFILLS.FILL
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    IsBRE = 1
                                });
                            }
                            else if (AddGem(e)) { }
                            else if (AddHopo(e)) { }
                            else if (e.Key == TrillMarker)
                            {
                                // Remove invalid trill (one with no notes)
                                if (trills.Count > 0)
                                {
                                    var lastTrill = trills.Last();
                                    if (lastTrill.FirstFret == -1)
                                        trills.RemoveAt(trills.Count - 1);
                                    else if (lastTrill.SecondFret == -1)
                                        lastTrill.SecondFret = lastTrill.FirstFret;
                                }
                                trill = new RBMid.GTRTRILLS.TRILL
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    FirstFret = -1,
                                    SecondFret = -1
                                };
                                trills.Add(trill);
                            }
                            else if (e.Key == TremoloMarker)
                            {
                                tremolos.Add(new RBMid.LANEMARKER.MARKER
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    Lanes = 0
                                });
                            }
                            else if (e.Key >= LeftHandStart && e.Key <= LeftHandEnd)
                            {
                                left_hand.Add(new RBMid.HANDPOS
                                {
                                    StartTime = (float)e.StartTime,
                                    Length = (float)e.Length,
                                    Position = e.Key - LeftHandStart,
                                    // TODO
                                    Unknown = 0
                                });
                            }
                            else if (e.Key == OverdriveMarker)
                            {
                                overdrive_markers.Add(new RBMid.SECTIONS.SECTION
                                {
                                    StartTicks = e.StartTicks,
                                    LengthTicks = e.LengthTicks
                                });
                            }
                            else if (e.Key == SoloMarker)
                            {
                                solo_markers.Add(new RBMid.SECTIONS.SECTION
                                {
                                    StartTicks = e.StartTicks,
                                    LengthTicks = e.LengthTicks
                                });
                            }
                            break;
                        case MidiText e:
                            var regex = new System.Text.RegularExpressions.Regex("\\[map (HandMap_[A-Za-z_2]+)\\]");
                            var match = regex.Match(e.Text);
                            if (match.Success && HandMaps.ContainsKey(match.Groups[1].Value))
                            {
                                var mapType = HandMaps[match.Groups[1].Value];
                                maps.Add(new RBMid.MAP
                                {
                                    Map = mapType,
                                    StartTime = (float)e.StartTime
                                });
                                break;
                            }
                            regex = new System.Text.RegularExpressions.Regex("\\[map (StrumMap_[A-Za-z]+)\\]");
                            match = regex.Match(e.Text);
                            if (match.Success && StrumMaps.ContainsKey(match.Groups[1].Value))
                            {
                                var mapType = StrumMaps[match.Groups[1].Value];
                                strummaps.Add(new RBMid.MAP
                                {
                                    StartTime = (float)e.StartTime,
                                    Map = mapType
                                });
                                break;
                            }
                            break;
                    }
                }

                int Unk = track.Name == "PART BASS" ? 2 : 1;
                Lyrics.Add(new RBMid.LYRICS
                {
                    TrackName = track.Name,
                    Lyrics = new RBMid.TICKTEXT[0],
                    Unknown1 = Unk,
                    Unknown2 = Unk,
                    Unknown3 = 0
                });
                DrumFills.Add(new RBMid.DRUMFILLS
                {
                    Fills = drumfills.ToArray(),
                    Lanes = fills_unk.ToArray()
                });
                ProMarkers.Add(new RBMid.TOMMARKER
                {
                    Markers = new RBMid.TOMMARKER.MARKER[]
                  {
            new RBMid.TOMMARKER.MARKER
            {
              Tick = 0,
              Flags = 0
            }
                  }
                });
                LaneMarkers.Add(new RBMid.LANEMARKER
                {
                    Markers = tremolos.Count == 0 ? new RBMid.LANEMARKER.MARKER[0][] : new RBMid.LANEMARKER.MARKER[4][]
                  {
            new RBMid.LANEMARKER.MARKER[0],
            new RBMid.LANEMARKER.MARKER[0],
            new RBMid.LANEMARKER.MARKER[0],
            tremolos.ToArray()
                  }
                });

                TrillMarkers.Add(new RBMid.GTRTRILLS
                {
                    Trills = trills.Count == 0 ? new RBMid.GTRTRILLS.TRILL[0][] : new RBMid.GTRTRILLS.TRILL[4][]
                  {
            new RBMid.GTRTRILLS.TRILL[0],
            new RBMid.GTRTRILLS.TRILL[0],
            new RBMid.GTRTRILLS.TRILL[0],
            trills.ToArray()
                  }
                });
                DrumMixes.Add(new RBMid.DRUMMIXES
                {
                    Mixes = new RBMid.TICKTEXT[4][]
                  {
            new RBMid.TICKTEXT[0], new RBMid.TICKTEXT[0], new RBMid.TICKTEXT[0], new RBMid.TICKTEXT[0]
                  }
                });
                GemTracks.Add(new RBMid.GEMTRACK
                {
                    Gems = gem_tracks.Select(g => g.ToArray()).ToArray(),
                    HopoThreshold = hopoThreshold
                });
                var sections = new RBMid.SECTIONS.SECTION[6][] {
          overdrive_markers.ToArray(),
          solo_markers.ToArray(),
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0]
        };
                OverdriveSoloSections.Add(new RBMid.SECTIONS
                {
                    Sections = new RBMid.SECTIONS.SECTION[4][][]
                  {
            sections, sections, sections, sections
                  }
                });
                HandMap.Add(maps.ToArray());
                HandPos.Add(left_hand.ToArray());
                strumMaps.Add(strummaps.ToArray());
            }

            private void HandleRealKeysXTrk(MidiTrackProcessed track)
            {
                Lyrics.Add(new RBMid.LYRICS
                {
                    TrackName = track.Name,
                    Lyrics = new RBMid.TICKTEXT[0],
                    Unknown1 = 4,
                    Unknown2 = 5,
                    Unknown3 = 0
                });
                DrumFills.Add(new RBMid.DRUMFILLS
                {
                    Fills = new RBMid.DRUMFILLS.FILL[0],
                    Lanes = new RBMid.DRUMFILLS.FILL_LANES[0]
                });
                ProMarkers.Add(new RBMid.TOMMARKER
                {
                    Markers = new RBMid.TOMMARKER.MARKER[]
                  {
            new RBMid.TOMMARKER.MARKER
            {
              Tick = 0,
              Flags = 0
            }
                  }
                });
                LaneMarkers.Add(new RBMid.LANEMARKER
                {
                    Markers = new RBMid.LANEMARKER.MARKER[0][]
                });
                TrillMarkers.Add(new RBMid.GTRTRILLS
                {
                    Trills = new RBMid.GTRTRILLS.TRILL[0][]
                });
                DrumMixes.Add(new RBMid.DRUMMIXES
                {
                    Mixes = new RBMid.TICKTEXT[4][]
                });
                GemTracks.Add(new RBMid.GEMTRACK
                {
                    Gems = new RBMid.GEMTRACK.GEM[0][],
                    HopoThreshold = hopoThreshold
                });
                OverdriveSoloSections.Add(new RBMid.SECTIONS
                {
                    Sections = new RBMid.SECTIONS.SECTION[0][][]
                });
                HandMap.Add(new RBMid.MAP[0]);
                HandPos.Add(new RBMid.HANDPOS[0]);
                strumMaps.Add(new RBMid.MAP[0]);
            }

            private void HandleKeysAnimTrk(MidiTrackProcessed track)
            {
                var anims = new List<RBMid.ANIM.EVENT>();
                foreach (var item in track.Items)
                {
                    switch (item)
                    {
                        case MidiNote e:
                            if (e.Key >= 48 && e.Key <= 72)
                            {
                                anims.Add(new RBMid.ANIM.EVENT
                                {
                                    StartMillis = (float)e.StartTime * 1000,
                                    StartTick = e.StartTicks,
                                    KeyBitfield = 1 << (e.Key - 48),
                                    LengthTicks = (ushort)(e.LengthTicks),
                                    LengthMillis = (ushort)(e.Length * 1000),
                                    // TODO: Usually this is 256, or 0, or 65536 (so maybe it is actually 4 bools?)
                                    Unknown2 = 256,
                                    // TODO
                                    Unknown3 = 0
                                });
                            }
                            break;
                    }
                }
                Anims.Add(new RBMid.ANIM
                {
                    TrackName = track.Name,
                    Unknown1 = 1,
                    Unknown2 = 120,
                    Events = anims.ToArray(),
                    Unknown3 = 120
                });
            }

            const byte AltPhraseMarker = 106;
            const byte PhraseMarker = 105;
            const byte AutoPercussion = 97;
            const byte Percussion = 96;
            const byte VocalsEnd = 84;
            const byte VocalsStart = 36;
            private void HandleVocalsTrk(MidiTrackProcessed track)
            {
                var lyrics = new List<RBMid.TICKTEXT>();
                var overdrive_markers = new List<RBMid.SECTIONS.SECTION>();
                var percussions = new List<uint>();
                var notes = new List<RBMid.VOCALTRACK.VOCAL_NOTE>();
                var gen_phrases = new List<RBMid.VOCALTRACK.PHRASE_MARKER>();
                var authored_phrases = new List<RBMid.VOCALTRACK.PHRASE_MARKER>();
                var freestyle = new List<RBMid.VOCALTRACK.OD_REGION>();                
                bool pitched = false;
                var trackVocalRange = new RBMid.VocalTrackRange
                {
                    LowNote = float.MaxValue,
                    HighNote = float.MinValue
                };
                RBMid.VOCALTRACK.PHRASE_MARKER gen_phrase = null;
                RBMid.VOCALTRACK.PHRASE_MARKER authored_phrase = null;

                int harm;
                switch (track.Name)
                {
                    case "HARM1": harm = 1; break;
                    case "HARM2": harm = 2; break;
                    case "HARM3": harm = 3; break;
                    default: harm = 0; break;
                }

                // Initialization
                bool copyPreviousPhrases = harm >= 2;
                RBMid.VOCALTRACK.PHRASE_MARKER[] last_track_markers = null;
                if (copyPreviousPhrases)
                {
                    last_track_markers = VocalTracks.Last().FakePhraseMarkers;
                    foreach (var marker in last_track_markers)
                    {
                        gen_phrases.Add(new RBMid.VOCALTRACK.PHRASE_MARKER
                        {
                            StartMillis = marker.StartMillis,
                            LengthMillis = marker.LengthMillis,
                            StartTicks = marker.StartTicks,
                            LengthTicks = marker.LengthTicks,
                            HighNote = float.MinValue,
                            LowNote = float.MaxValue,
                            StartNoteIdx = marker.StartNoteIdx == -1 ? -1 : 0,
                            EndNoteIdx = marker.EndNoteIdx == -1 ? -1 : 0,
                            PhraseFlags = marker.PhraseFlags
                        });
                        gen_phrase = gen_phrases[0];
                    }
                }
                else
                {
                    gen_phrase = new RBMid.VOCALTRACK.PHRASE_MARKER
                    {
                        StartMillis = 0f,
                        StartTicks = 0,
                        StartNoteIdx = -1,
                        EndNoteIdx = -1,
                        LowNote = float.MaxValue,
                        HighNote = float.MinValue,
                    };
                    gen_phrases.Add(gen_phrase);
                }

                // Event handlers
                bool AddLyric(MidiText e)
                {
                    if (e.Text[0] != '[')
                    {
                        lyrics.Add(new RBMid.TICKTEXT
                        {
                            Text = e.Text.Trim(' '),
                            Tick = e.StartTicks,
                        });
                        return true;
                    }
                    return false;
                }
                bool AddVocalNote(MidiNote e)
                {
                    if (e.Key < VocalsStart || e.Key > VocalsEnd)
                        return false;
                    if (lyrics.Count == 0) throw new Exception("Note without accompanying lyric");
                    if (copyPreviousPhrases)
                    {
                        // Get the phrase that contains this note
                        var phrase = gen_phrases.First(x => x.StartTicks + x.LengthTicks > e.StartTicks);
                        if (phrase != gen_phrase)
                        {
                            gen_phrase = phrase;
                            if (gen_phrase.StartNoteIdx == 0)
                                gen_phrase.StartNoteIdx = notes.Count;
                            if (notes.Count > 0)
                                notes.Last().LastNoteInPhrase = true;
                        }
                    }
                    var lyric = lyrics.Last().Text;
                    var lyricCleaned = lyric.Replace("$", "").Replace("#", "").Replace("^", "");
                    if (lyricCleaned == "+")
                    {
                        var previous = notes.LastOrDefault();
                        if (previous == null)
                            throw new Exception("Vocal track " + track.Name + " started with a '+' lyric");
                        notes.Add(new RBMid.VOCALTRACK.VOCAL_NOTE
                        {
                            PhraseIndex = previous.PhraseIndex,
                            MidiNote = previous.MidiNote2,
                            MidiNote2 = e.Key,
                            StartMillis = previous.StartMillis + previous.LengthMillis,
                            StartTick = previous.StartTick + previous.LengthTicks,
                            LengthMillis = ((float)e.StartTime * 1000) - (previous.StartMillis + previous.LengthMillis),
                            LengthTicks = (ushort)(e.StartTicks - (previous.StartTick + previous.LengthTicks)),
                            Lyric = "",
                            LastNoteInPhrase = false,
                            PhraseFlags = previous.PhraseFlags,
                            Portamento = true,
                            ShowLyric = previous.ShowLyric,
                        });
                    }
                    var last = notes.LastOrDefault();
                    var lastNoteEnd = last == null ? 0 : (last.StartMillis + last.LengthMillis);
                    var tacet = ((float)e.StartTime * 1000) - lastNoteEnd;
                    if (tacet > 600f)
                    {
                        float transition = (tacet > 800f ? 100f : 50f);
                        freestyle.Add(new RBMid.VOCALTRACK.OD_REGION
                        {
                            StartMillis = lastNoteEnd + transition,
                            EndMillis = (float)e.StartTime * 1000 - transition
                        });
                    }
                    RBMid.VOCALTRACK.VOCAL_NOTE note;
                    notes.Add(note = new RBMid.VOCALTRACK.VOCAL_NOTE
                    {
                        PhraseIndex = gen_phrases.IndexOf(gen_phrase),
                        MidiNote = e.Key,
                        MidiNote2 = e.Key,
                        StartMillis = (float)e.StartTime * 1000,
                        StartTick = e.StartTicks,
                        LengthMillis = (float)e.Length * 1000,
                        LengthTicks = (ushort)e.LengthTicks,
                        Lyric = lyricCleaned == "+" ? "" : lyricCleaned,
                        LastNoteInPhrase = false,
                        False1 = false,
                        Unpitched = lyric.Contains('#') || lyric.Contains('^'),
                        UnpitchedGenerous = lyric.Contains('^'),
                        RangeDivider = lyric.Contains('%'),
                        PhraseFlags = gen_phrase.PhraseFlags,
                        Portamento = lyricCleaned == "+",
                        LyricShift = false,
                        ShowLyric = !lyric.Contains('$'),
                    });
                    if (note.Unpitched)
                    {
                        gen_phrase.HasUnpitchedVox = true;
                    }
                    else
                    {
                        pitched = true;
                        gen_phrase.HasPitchedVox = true;
                        gen_phrase.LowNote = Math.Min(e.Key, gen_phrase.LowNote);
                        gen_phrase.HighNote = Math.Max(e.Key, gen_phrase.HighNote);
                    }
                    gen_phrase.EndNoteIdx = notes.Count;

                    if (authored_phrase != null)
                    {
                        if (harm < 2)
                        {
                            authored_phrase.EndNoteIdx = gen_phrase.EndNoteIdx;
                            authored_phrase.HasUnpitchedVox = gen_phrase.HasUnpitchedVox;
                        }
                    }

                    theVocalRange.HighNote = Math.Max(e.Key, theVocalRange.HighNote);
                    theVocalRange.LowNote = Math.Min(e.Key, theVocalRange.LowNote);
                    return true;
                }
                bool AddPhrase(MidiNote e)
                {
                    if (e.Key != PhraseMarker && e.Key != AltPhraseMarker)
                        return false;
                    byte phraseFlags = (byte)((e.Key == PhraseMarker ? RBMid.VOCALTRACK.PHRASE_MARKER.FLAG_NORMAL : 0)
                               + (e.Key == AltPhraseMarker ? RBMid.VOCALTRACK.PHRASE_MARKER.FLAG_TUG_OF_WAR : 0));
                    if (authored_phrase?.StartTicks == e.StartTicks)
                    {
                        // Add tug-of-war bit
                        authored_phrase.PhraseFlags += phraseFlags;
                        gen_phrase.PhraseFlags = authored_phrase.PhraseFlags;
                        return true;
                    }

                    if (!copyPreviousPhrases)
                    {
                        if (notes.Count > 0)
                        {
                            notes.Last().LastNoteInPhrase = true;
                        }
                        if (gen_phrase.StartTicks == 0)
                        {
                            var tick = e.StartTicks - 640;
                            gen_phrase.LengthMillis = GetMillis(tick);
                            gen_phrase.LengthTicks = tick;
                        }
                        var start = gen_phrase.StartTicks + gen_phrase.LengthTicks;
                        var end = e.StartTicks + e.LengthTicks;
                        gen_phrase = new RBMid.VOCALTRACK.PHRASE_MARKER
                        {
                            StartTicks = start,
                            LengthTicks = end - start,
                            StartMillis = GetMillis(start),
                            LengthMillis = GetMillis(end) - GetMillis(start),
                            StartNoteIdx = notes.Count,
                            EndNoteIdx = notes.Count,
                            HasPitchedVox = false,
                            HasUnpitchedVox = false,
                            LowNote = float.MaxValue,
                            HighNote = float.MinValue,
                            PercussionSection = false,
                            PhraseFlags = phraseFlags,
                        };
                        gen_phrases.Add(gen_phrase);
                    }

                    authored_phrase = new RBMid.VOCALTRACK.PHRASE_MARKER
                    {
                        StartMillis = 0f,
                        LengthMillis = 0f,
                        StartTicks = e.StartTicks,
                        LengthTicks = e.LengthTicks,
                        StartNoteIdx = harm == 2 ? -1 : notes.Count,
                        EndNoteIdx = harm == 2 ? -1 : notes.Count,
                        HasPitchedVox = false,
                        HasUnpitchedVox = false,
                        LowNote = float.MaxValue,
                        HighNote = float.MinValue,
                        PhraseFlags = phraseFlags,
                    };
                    authored_phrases.Add(authored_phrase);
                    return true;
                }
                bool AddOverdrive(MidiNote e)
                {
                    if (e.Key != OverdriveMarker)
                        return false;
                    overdrive_markers.Add(new RBMid.SECTIONS.SECTION
                    {
                        StartTicks = e.StartTicks,
                        LengthTicks = e.LengthTicks
                    });
                    return true;
                }
                bool AddPercussion(MidiNote e)
                {
                    // TODO: Test autopercussion
                    if (e.Key != Percussion && e.Key != AutoPercussion)
                        return false;
                    gen_phrase.PercussionSection = true;
                    if (e.Key == Percussion)
                        percussions.Add(e.StartTicks);
                    return true;
                }

                // Order the notes by descending key so that bad phrase markers (that start at the same time as a note) are counted
                foreach (var item in track.Items.OrderBy(x => 128 - (x as MidiNote)?.Key ?? 0).OrderBy(x => x.StartTicks))
                {
                    switch (item)
                    {
                        case MidiNote e:
                            if (AddVocalNote(e)) { }
                            else if (AddPercussion(e)) { }
                            else if (AddPhrase(e)) { }
                            else if (AddOverdrive(e)) { }
                            break;
                        case MidiText e:
                            if (AddLyric(e)) { }
                            break;
                    }
                }

                var lastNote = notes.Last();
                var lastTempo = mf.TempoTimeSigMap.Last();
                var lastMeasure = MeasureTicks.Last() + (480U * lastTempo.Numerator * 4 / lastTempo.Denominator);
                var lastTime = lastTempo.Time + ((lastMeasure - lastTempo.Tick) / 480.0) * (60 / lastTempo.BPM);
                freestyle.Add(new RBMid.VOCALTRACK.OD_REGION
                {
                    StartMillis = lastNote.StartMillis + lastNote.LengthMillis + 100f,
                    EndMillis = (float)lastTime * 1000,
                });
                lastNote.LastNoteInPhrase = true;
                notes[notes.Count - 1] = lastNote;

                // fix for cases like deadblack
                if (pitched)
                    foreach (var phrase in gen_phrases)
                    {
                        phrase.LowNote = theVocalRange.LowNote;
                        phrase.HighNote = theVocalRange.HighNote;
                    }

                Lyrics.Add(new RBMid.LYRICS
                {
                    TrackName = track.Name,
                    Lyrics = lyrics.ToArray(),
                    Unknown1 = 3,
                    Unknown2 = 3,
                    Unknown3 = 0
                });
                DrumFills.Add(new RBMid.DRUMFILLS
                {
                    Fills = new RBMid.DRUMFILLS.FILL[0],
                    Lanes = new RBMid.DRUMFILLS.FILL_LANES[0],
                });
                ProMarkers.Add(new RBMid.TOMMARKER
                {
                    Markers = new RBMid.TOMMARKER.MARKER[]
                  {
            new RBMid.TOMMARKER.MARKER
            {
              Tick = 0,
              Flags = 0
            }
                  }
                });
                LaneMarkers.Add(new RBMid.LANEMARKER
                {
                    Markers = new RBMid.LANEMARKER.MARKER[0][],
                });
                TrillMarkers.Add(new RBMid.GTRTRILLS
                {
                    Trills = new RBMid.GTRTRILLS.TRILL[0][],
                });
                var emptyMixes = new RBMid.TICKTEXT[0];
                DrumMixes.Add(new RBMid.DRUMMIXES
                {
                    Mixes = new RBMid.TICKTEXT[4][] { emptyMixes, emptyMixes, emptyMixes, emptyMixes }
                });
                var emptyGems = new RBMid.GEMTRACK.GEM[0];
                GemTracks.Add(new RBMid.GEMTRACK
                {
                    Gems = new RBMid.GEMTRACK.GEM[4][] { emptyGems, emptyGems, emptyGems, emptyGems },
                    HopoThreshold = hopoThreshold
                });
                var overdriveSections = new RBMid.SECTIONS.SECTION[6][]
                {
          overdrive_markers.ToArray(),
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0],
          new RBMid.SECTIONS.SECTION[0]
                };
                OverdriveSoloSections.Add(new RBMid.SECTIONS
                {
                    Sections = new RBMid.SECTIONS.SECTION[4][][]
                  {
            overdriveSections, overdriveSections, overdriveSections, overdriveSections
                  }
                });
                // hack: copy data from HARM2 into HARM3
                if (copyPreviousPhrases && harm == 3)
                {
                    VocalTracks.Add(new RBMid.VOCALTRACK
                    {
                        FakePhraseMarkers = gen_phrases.ToArray(),
                        AuthoredPhraseMarkers = new RBMid.VOCALTRACK.PHRASE_MARKER[0],
                        Notes = notes.ToArray(),
                        Percussion = percussions.ToArray(),
                        FreestyleRegions = VocalTracks.Last().FreestyleRegions
                    });
                }
                else
                {
                    VocalTracks.Add(new RBMid.VOCALTRACK
                    {
                        FakePhraseMarkers = gen_phrases.ToArray(),
                        AuthoredPhraseMarkers = authored_phrases.ToArray(),
                        Notes = notes.ToArray(),
                        Percussion = percussions.ToArray(),
                        FreestyleRegions = freestyle.ToArray()
                    });
                }
                HandMap.Add(new RBMid.MAP[0]);
                HandPos.Add(new RBMid.HANDPOS[0]);
                strumMaps.Add(new RBMid.MAP[0]);
            }

            private void HandleEventsTrk(MidiTrackProcessed track)
            {
                foreach (var item in track.Items)
                {
                    var timeMillis = (float)(item.StartTime * 1000);
                    switch (item)
                    {
                        case MidiText e:
                            switch (e.Text)
                            {
                                case "[preview_start]":
                                    PreviewStart = timeMillis;
                                    break;
                                case "[preview_end]":
                                    PreviewEnd = timeMillis;
                                    break;
                                case "[preview]":
                                    PreviewStart = timeMillis;
                                    PreviewEnd = PreviewStart + 30_000;
                                    break;
                                case "[coda]":
                                    // TODO: This would be better in the Drum track code,
                                    // but we don't know if it's a BRE there because the lanes are the same for normal fills
                                    var idx = DrumFills[0].Fills.Length - 1;
                                    var lastDrumFill = DrumFills[0].Fills[idx];
                                    lastDrumFill.IsBRE = 1;
                                    DrumFills[0].Fills[idx] = lastDrumFill;
                                    break;
                            }
                            break;
                    }
                }
            }

            const byte MarkupNotes2 = 127;
            const byte MarkupChordsEnd = 64;
            const byte MarkupChordsStart = 36;
            const byte MarkupNotes3End = 23;
            const byte MarkupNotes3Start = 12;
            const byte MarkupNotes1End = 11;
            const byte MarkupNotes1Start = 0;
            private void HandleMarkupTrk(MidiTrackProcessed track)
            {
                LastMarkupTick = track.LastTick;
                RBMid.MARKUPCHORD last_chord = null;
                var pitches = new SortedSet<int>();
                foreach (var item in track.Items)
                {
                    switch (item)
                    {
                        case MidiNote e:
                            if (e.Key >= MarkupChordsStart && e.Key <= MarkupChordsEnd)
                            {
                                if (last_chord?.StartTick == e.StartTicks)
                                {
                                    pitches.Add(e.Key % 12);
                                    last_chord.Pitches = pitches.ToArray();
                                }
                                else
                                {
                                    if (last_chord != null)
                                    {
                                        last_chord.EndTick = e.StartTicks;
                                    }
                                    last_chord = new RBMid.MARKUPCHORD
                                    {
                                        StartTick = e.StartTicks,
                                        EndTick = 2147483647,
                                        Pitches = new[] { e.Key % 12 }
                                    };
                                    MarkupChords1.Add(last_chord);
                                    pitches.Clear();
                                    pitches.Add(e.Key % 12);
                                }
                            }
                            else if (e.Key >= MarkupNotes1Start && e.Key <= MarkupNotes1End)
                            {
                                MarkupSoloNotes1.Add(new RBMid.MARKUP_SOLO_NOTES
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    NoteOffset = e.Key - MarkupNotes1Start
                                });
                            }
                            else if (e.Key >= MarkupNotes3Start && e.Key <= MarkupNotes3End)
                            {
                                MarkupSoloNotes3.Add(new RBMid.MARKUP_SOLO_NOTES
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    NoteOffset = e.Key - MarkupNotes3Start
                                });
                            }
                            else if (e.Key == MarkupNotes2)
                            {
                                MarkupSoloNotes2.Add(new RBMid.MARKUP_SOLO_NOTES
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = e.StartTicks + e.LengthTicks,
                                    NoteOffset = e.Velocity
                                });
                            }
                            break;
                        case MidiText e:
                            var regex = new System.Text.RegularExpressions.Regex("\\[sololoop ([0-9]+)\\]");
                            var match = regex.Match(e.Text);
                            if (match.Success)
                            {
                                var loop_measures = int.Parse(match.Groups[1].Value);
                                var loop_end = MeasureTicks[loop_measures - 1];
                                SoloLoops1.Add(new RBMid.TWOTICKS
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = loop_end
                                });
                                SoloLoops2.Add(new RBMid.TWOTICKS
                                {
                                    StartTick = e.StartTicks,
                                    EndTick = loop_end
                                });
                            }
                            break;
                    }
                }
            }

            private void HandleVenueTrk(MidiTrackProcessed track)
            {
                // Venue track processing is not implemented.
            }

            // Converts the venue track from RBN2 to RBN1 so that RB4 can autogen animations
            private static List<MidiTrack> ConvertVenueTrack(List<MidiTrack> tracks)
            {
                const int tpqn = 480;
                const int note_length = tpqn / 4; //16th note
                var to_remove = new List<IMidiMessage>();
                var to_add = new List<IMidiMessage>();
                long lastevent = (note_length * -1) - 1; //this ensures (lastevent + note_length) starts at -1
                uint final_event = 0;

                var venueTrack = tracks.Where(x => x.Name == "VENUE").FirstOrDefault();
                if (venueTrack == null)
                {
                    return tracks;
                }
                if (!venueTrack.Messages.Any(m => m is TextEvent t
                    && (t.Text.Contains(".pp]") || t.Text.Contains("[coop") || t.Text.Contains("[directed"))))
                {
                    // If this is already a RBN1 VENUE, skip it.
                    return tracks;
                }
                long last_first = 0;
                long last_next = 0;
                long last_proc_time = 0;
                var last_proc_note = 0;

                to_add.Add(new TextEvent(0, "[verse]"));
                var absMessages = MidiHelper.ToAbsolute(venueTrack.Messages);
                foreach (var message in absMessages)
                {
                    final_event = message.DeltaTime;
                    if (message is MetaTextEvent mt && mt.Text.Contains("["))
                    {
                        var index = mt.Text.IndexOf("[", StringComparison.Ordinal);
                        var new_event = mt.Text.Substring(index, mt.Text.Length - index).Trim();

                        if (new_event.Contains("[directed"))
                        {
                            new_event = new_event
                              .Replace("[directed_vocals_cam_pt]", "[directed_vocals_cam]")
                              .Replace("[directed_vocals_cam_pr]", "[directed_vocals_cam]")
                              .Replace("[directed_guitar_cam_pt]", "[directed_guitar_cam]")
                              .Replace("[directed_guitar_cam_pr]", "[directed_guitar_cam]")
                              .Replace("[directed_crowd]", "[directed_crowd_g]")
                              .Replace("[directed_duo_drums]", "[directed_drums]")

                              // Replace all keys cuts with arbitrary choices
                              .Replace("[directed_duo_kv]", "[directed_duo_guitar]")
                              .Replace("[directed_duo_kb]", "[directed_duo_gb]")
                              .Replace("[directed_duo_kg]", "[directed_duo_gb]")
                              .Replace("[directed_keys]", "[directed_crowd_b]")
                              .Replace("[directed_keys_cam]", "[directed_crowd_b]")
                              .Replace("[directed_keys_np]", "[directed_crowd_b]")

                              // RBN1 format the directed cut
                              .Replace("[directed", "[do_directed_cut directed");
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[lighting") && message.DeltaTime == 0)
                        {
                            new_event = "[lighting ()]";
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[lighting (manual") || new_event.Contains("[lighting (dischord)]"))
                        {
                            if (message.DeltaTime <= last_next)
                            {
                                to_remove.Add(message);
                                continue;
                            }

                            //add First Frame note as found in most RBN1 MIDIs
                            var note = new NoteOnEvent(message.DeltaTime, 0, 50, 96);
                            to_add.Add(note);
                            to_add.Add(new NoteOffEvent(note.DeltaTime + note_length, note.Channel, note.Key, 0));
                            last_first = note.DeltaTime + note_length; //to prevent having both Next and First events in the same spot
                            continue;
                        }
                        else if (new_event.Contains("[lighting (verse)]"))
                        {
                            new_event = "[verse]";
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[lighting (chorus)]"))
                        {
                            new_event = "[chorus]";
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[lighting (intro)]"))
                        {
                            new_event = "[lighting ()]";
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[lighting (blackout_spot)]"))
                        {
                            new_event = "[lighting (silhouettes_spot)]";
                            to_add.Add(new TextEvent(message.DeltaTime, new_event));
                        }
                        else if (new_event.Contains("[next]"))
                        {
                            if (message.DeltaTime <= last_first)
                            {
                                to_remove.Add(message);
                                continue;
                            }

                            var note = new NoteOnEvent(message.DeltaTime, 0, 48, 96);
                            to_add.Add(note);
                            to_add.Add(new NoteOffEvent(note.DeltaTime + note_length, note.Channel, note.Key, 0));
                            last_next = note.DeltaTime + note_length; //to prevent having both Next and First events in the same spot
                        }
                        else if (new_event.Contains(".pp]"))
                        {
                            byte NoteNumber = 0;
                            switch (new_event)
                            {
                                case "[ProFilm_a.pp]":
                                case "[ProFilm_b.pp]":
                                    NoteNumber = 96;
                                    break;
                                case "[film_contrast.pp]":
                                case "[film_contrast_green.pp]":
                                case "[film_contrast_red.pp]":
                                case "[contrast_a.pp]":
                                    NoteNumber = 97;
                                    break;
                                case "[desat_posterize_trails.pp]":
                                case "[film_16mm.pp]":
                                    NoteNumber = 98;
                                    break;
                                case "[film_sepia_ink.pp]":
                                    NoteNumber = 99;
                                    break;
                                case "[film_silvertone.pp]":
                                    NoteNumber = 100;
                                    break;
                                case "[horror_movie_special.pp]":
                                case "[ProFilm_psychedelic_blue_red.pp]":
                                case "[photo_negative.pp]":
                                    NoteNumber = 101;
                                    break;
                                case "[photocopy.pp]":
                                    NoteNumber = 102;
                                    break;
                                case "[posterize.pp]":
                                case "[bloom.pp]":
                                    NoteNumber = 103;
                                    break;
                                case "[bright.pp]":
                                    NoteNumber = 104;
                                    break;
                                case "[ProFilm_mirror_a.pp]":
                                    NoteNumber = 105;
                                    break;
                                case "[desat_blue.pp]":
                                case "[film_contrast_blue.pp]":
                                case "[film_blue_filter.pp]":
                                    NoteNumber = 106;
                                    break;
                                case "[video_a.pp]":
                                    NoteNumber = 107;
                                    break;
                                case "[video_bw.pp]":
                                case "[film_b+w.pp]":
                                    NoteNumber = 108;
                                    break;
                                case "[shitty_tv.pp]":
                                case "[video_security.pp]":
                                    NoteNumber = 109;
                                    break;
                                case "[video_trails.pp]":
                                case "[flicker_trails.pp]":
                                case "[space_woosh.pp]":
                                case "[clean_trails.pp]":
                                    NoteNumber = 110;
                                    break;
                            }

                            //reduces instances of pp notes to bare minimum
                            if (NoteNumber > 0 && NoteNumber != last_proc_note && message.DeltaTime >= last_proc_time)
                            {
                                to_add.Add(new NoteOnEvent(message.DeltaTime, 0, NoteNumber, 96));
                                to_add.Add(new NoteOffEvent(message.DeltaTime + note_length, 0, NoteNumber, 0));
                            }

                            //we want at least 1 measure between pp effects
                            last_proc_time = message.DeltaTime + (tpqn * 4);
                            //we don't want to put multiple PP notes for the same effect
                            last_proc_note = NoteNumber;
                        }
                        else if (new_event.Contains("[coop"))
                        {
                            if (message.DeltaTime <= (lastevent + note_length)) //to avoid double notes)
                            {
                                to_remove.Add(message);
                                continue;
                            }

                            var cameraNoteOn = new NoteOnEvent[9];
                            var enabled = new bool[9];
                            lastevent = message.DeltaTime;

                            const int cameracut = 0; //60
                            const int bass = 1; //61
                            const int drummer = 2; //62
                            const int guitar = 3; //63
                            const int vocals = 4; //64
                            const int nobehind = 5; //70
                            const int onlyfar = 6; //71
                            const int onlyclose = 7; //72
                            const int noclose = 8; //73

                            cameraNoteOn[cameracut] = new NoteOnEvent(message.DeltaTime, 0, 60, 96);
                            cameraNoteOn[bass] = new NoteOnEvent(message.DeltaTime, 0, 61, 96);
                            cameraNoteOn[drummer] = new NoteOnEvent(message.DeltaTime, 0, 62, 96);
                            cameraNoteOn[guitar] = new NoteOnEvent(message.DeltaTime, 0, 63, 96);
                            cameraNoteOn[vocals] = new NoteOnEvent(message.DeltaTime, 0, 64, 96);
                            cameraNoteOn[nobehind] = new NoteOnEvent(message.DeltaTime, 0, 70, 96);
                            cameraNoteOn[onlyfar] = new NoteOnEvent(message.DeltaTime, 0, 71, 96);
                            cameraNoteOn[onlyclose] = new NoteOnEvent(message.DeltaTime, 0, 72, 96);
                            cameraNoteOn[noclose] = new NoteOnEvent(message.DeltaTime, 0, 73, 96);

                            enabled[cameracut] = true; //always enabled for [coop shots

                            //players
                            if (new_event.Contains("all_"))
                            {
                                enabled[drummer] = true;
                                enabled[bass] = true;
                                enabled[guitar] = true;
                                enabled[vocals] = true;
                            }
                            else if (new_event.Contains("front_"))
                            {
                                enabled[bass] = true;
                                enabled[guitar] = true;
                                enabled[vocals] = true;
                            }
                            else if (new_event.Contains("v_") || new_event.Contains("_v"))
                            //this allows for single or duo shots
                            {
                                enabled[vocals] = true;
                            }
                            else if (new_event.Contains("g_") || new_event.Contains("_g"))
                            {
                                enabled[guitar] = true;
                            }
                            else if ((new_event.Contains("b_") || new_event.Contains("_b")) &&
                                      !new_event.Contains("behind"))
                            {
                                enabled[bass] = true;
                            }
                            else if (new_event.Contains("d_") || new_event.Contains("_d"))
                            {
                                enabled[drummer] = true;
                            }
                            else if (new_event.Contains("k_") || new_event.Contains("_k"))
                            {
                                enabled[guitar] = true; //keys not supported
                            }

                            //camera placement
                            if (new_event.Contains("behind"))
                            {
                                enabled[noclose] = true;
                            }
                            else if (new_event.Contains("near") || new_event.Contains("closeup"))
                            {
                                enabled[nobehind] = true;
                                enabled[onlyclose] = true;
                            }
                            else if (new_event.Contains("far"))
                            {
                                enabled[nobehind] = true;
                                enabled[noclose] = true;
                                enabled[onlyfar] = true;
                            }

                            //add the notes that are enabled
                            for (var c = 0; c < 9; c++)
                            {
                                if (!enabled[c]) continue;
                                to_add.Add(cameraNoteOn[c]);
                                to_add.Add(new NoteOffEvent(cameraNoteOn[c].DeltaTime + note_length, cameraNoteOn[c].Channel, cameraNoteOn[c].Key, 0));
                            }
                        }
                        else
                        {
                            continue;
                        }
                        to_remove.Add(message);
                    }
                    else if (message is EndOfTrackEvent eot)
                    {
                        to_remove.Add(message);
                    }
                    else switch (message)
                        {
                            case NoteOnEvent e:
                                {
                                    if (e.Key == 41) //can't have keys spotlight
                                    {
                                        to_remove.Add(message);
                                    }
                                }
                                break;
                            case NoteOffEvent e:
                                {
                                    if (e.Key == 41) //can't have keys spotlight
                                    {
                                        to_remove.Add(message);
                                    }
                                }
                                break;
                        }
                }

                foreach (var remove in to_remove)
                {
                    absMessages.Remove(remove);
                }
                absMessages.AddRange(to_add);
                absMessages.Add(new EndOfTrackEvent(final_event + (note_length * 2)));
                var tracks2 = new List<MidiTrack>(tracks);
                var venueIndex = tracks2.IndexOf(venueTrack);
                tracks2[venueIndex] = new MidiTrack(MidiHelper.ToRelative(absMessages.OrderBy(x => x.DeltaTime).ToList()), final_event + (note_length * 2), "VENUE");
                return tracks2;
            }
        }
    }
}

namespace LibForge.Midi
{
    public class RBMidReader : ReaderBase<RBMid>
    {
        public static RBMid ReadStream(Stream s)
        {
            return new RBMidReader(s).Read();
        }
        public RBMidReader(Stream s) : base(s) { }

        const uint MaxInstTracks = 10;
        const uint MaxKeysAnimTracks = 2;
        const uint MaxVocalTracks = 4;
        public override RBMid Read()
        {
            var r = new RBMid();
            r.Format = Int();
            if (r.Format != RBMid.FORMAT_RB4 && r.Format != RBMid.FORMAT_RBVR)
            {
                //throw new Exception($"Invalid magic number (expected 10 or 2f, got {r.Format:X}");
                return null; //skip the MIDI for now
            }
            r.Lyrics = Arr(ReadLyrics, MaxInstTracks);
            var numTracks = (uint)r.Lyrics.Length;
            r.DrumFills = Arr(ReadDrumFills, numTracks);
            r.Anims = Arr(ReadAnims, numTracks);
            r.ProMarkers = Arr(ReadMarkers, numTracks);
            r.LaneMarkers = Arr(ReadUnktrack, numTracks);
            r.TrillMarkers = Arr(ReadUnktrack2, numTracks);
            r.DrumMixes = Arr(ReadDrumMixes, numTracks);
            r.GemTracks = Arr(ReadGemTracks, numTracks);
            r.OverdriveSoloSections = Arr(ReadOverdrives, numTracks);
            r.VocalTracks = Arr(ReadVocalTrack, MaxVocalTracks);
            r.UnknownOne = Check(Int(), 1);
            r.UnknownNegOne = Check(Int(), -1);
            r.UnknownHundred = Check(Float(), 100f);
            r.Unknown4 = Arr(ReadUnkstruct1);
            r.VocalRange = Arr(ReadVocalTrackRange);
            r.HopoThreshold = Int();
            r.NumPlayableTracks = Check(UInt(), numTracks);
            r.FinalEventTick = UInt();
            if (r.Format == RBMid.FORMAT_RBVR)
            {
                r.UnkVrTick = UInt();
            }
            r.UnknownZeroByte = Check(Byte(), (byte)0);
            r.PreviewStartMillis = Float();
            r.PreviewEndMillis = Float();
            r.HandMaps = Arr(() => Arr(ReadMap), numTracks);
            r.GuitarLeftHandPos = Arr(() => Arr(ReadHandPos), numTracks);
            r.StrumMaps = Arr(() => Arr(ReadMap), numTracks);

            r.MarkupSoloNotes1 = Arr(ReadMarkupSoloNotes);
            r.MarkupLoop1 = Arr(ReadTwoTicks);
            r.MarkupChords1 = Arr(ReadMarkupChord);
            r.MarkupSoloNotes2 = Arr(ReadMarkupSoloNotes);
            r.MarkupSoloNotes3 = Arr(ReadMarkupSoloNotes);
            r.MarkupLoop2 = Arr(ReadTwoTicks);

            if (r.Format == RBMid.FORMAT_RBVR)
            {
                r.VREvents = ReadVREvents();
            }
            new MidiFileResourceReader(s).Read(r);
            return r;
        }
        private RBMid.TICKTEXT ReadTickText() => new RBMid.TICKTEXT
        {
            Tick = UInt(),
            Text = String()
        };
        private RBMid.LYRICS ReadLyrics() => new RBMid.LYRICS
        {
            TrackName = String(),
            Lyrics = Arr(ReadTickText),
            Unknown1 = Int(),
            Unknown2 = Int(),
            Unknown3 = Byte()
        };
        private RBMid.DRUMFILLS ReadDrumFills() => new RBMid.DRUMFILLS
        {
            Lanes = Arr(() => new RBMid.DRUMFILLS.FILL_LANES
            {
                Tick = UInt(),
                Lanes = UInt()
            }),
            Fills = Arr(() => new RBMid.DRUMFILLS.FILL
            {
                StartTick = UInt(),
                EndTick = UInt(),
                IsBRE = Byte()
            })
        };
        private RBMid.ANIM ReadAnims() => new RBMid.ANIM
        {
            TrackName = String(),
            Unknown1 = Int(),
            Unknown2 = Int(),
            Events = Arr(() => new RBMid.ANIM.EVENT
            {
                StartMillis = Float(),
                StartTick = UInt(),
                LengthMillis = UShort(),
                LengthTicks = UShort(),
                KeyBitfield = Int(),
                Unknown2 = Int(),
                Unknown3 = Short()
            }),
            Unknown3 = Int()
        };
        private RBMid.TOMMARKER ReadMarkers() => new RBMid.TOMMARKER
        {
            Markers = Arr(() => new RBMid.TOMMARKER.MARKER
            {
                Tick = UInt(),
                Flags = (RBMid.TOMMARKER.MARKER.FLAGS)Int()
            }),
            Unknown1 = Int(),
            Unknown2 = Int()
        };
        private RBMid.LANEMARKER ReadUnktrack() => new RBMid.LANEMARKER
        {
            Markers = Arr(() => Arr(() => new RBMid.LANEMARKER.MARKER
            {
                StartTick = UInt(),
                EndTick = UInt(),
                Lanes = Int()
            }))
        };
        private RBMid.GTRTRILLS ReadUnktrack2() => new RBMid.GTRTRILLS
        {
            Trills = Arr(() => Arr(() => new RBMid.GTRTRILLS.TRILL
            {
                StartTick = UInt(),
                EndTick = UInt(),
                FirstFret = Int(),
                SecondFret = Int()
            }))
        };
        private RBMid.DRUMMIXES ReadDrumMixes() => new RBMid.DRUMMIXES
        {
            Mixes = Arr(() => Arr(ReadTickText))
        };
        private RBMid.GEMTRACK ReadGemTracks() => new RBMid.GEMTRACK
        {
            Gems = Arr(Seq(Skip(4), () => Arr(() => new RBMid.GEMTRACK.GEM
            {
                StartMillis = Float(),
                StartTicks = UInt(),
                LengthMillis = UShort(),
                LengthTicks = UShort(),
                Lanes = Int(),
                IsHopo = Bool(),
                NoTail = Bool(),
                ProCymbal = Int()
            }))),
            HopoThreshold = Int()
        };
        private RBMid.SECTIONS ReadOverdrives() => new RBMid.SECTIONS
        {
            Sections = Arr(() => Arr(() => Arr(() => new RBMid.SECTIONS.SECTION
            {
                StartTicks = UInt(),
                LengthTicks = UInt()
            })))
        };
        private RBMid.VOCALTRACK ReadVocalTrack() => new RBMid.VOCALTRACK
        {
            FakePhraseMarkers = Arr(ReadPhraseMarker),
            AuthoredPhraseMarkers = Arr(ReadPhraseMarker),
            Notes = Arr(() => {
                var note = new RBMid.VOCALTRACK.VOCAL_NOTE
                {
                    PhraseIndex = Int(),
                    MidiNote = CheckRange(Int(), 0, 127),
                    MidiNote2 = CheckRange(Int(), 0, 127),
                    StartMillis = Float(),
                    StartTick = UInt(),
                    LengthMillis = Float(),
                    LengthTicks = UShort(),
                    Lyric = String(),
                    LastNoteInPhrase = Bool(),
                    False1 = Check(Bool(), false, nameof(RBMid.VOCALTRACK.VOCAL_NOTE.False1)),
                    Unpitched = Bool(),
                    UnpitchedGenerous = Bool(),
                    RangeDivider = Bool(),
                    PhraseFlags = Byte(),
                    Portamento = Bool(),
                    LyricShift = Bool(),
                    ShowLyric = Bool(),
                }; return note;
            }),
            Percussion = Arr(UInt),
            FreestyleRegions = Arr(() => new RBMid.VOCALTRACK.OD_REGION
            {
                StartMillis = Float(),
                EndMillis = Float()
            })
        };
        private RBMid.VOCALTRACK.PHRASE_MARKER ReadPhraseMarker() =>
          new RBMid.VOCALTRACK.PHRASE_MARKER
          {
              StartMillis = Float(),
              LengthMillis = Float(),
              StartTicks = UInt(),
              LengthTicks = UInt(),
              StartNoteIdx = Int(),
              EndNoteIdx = Int(),
              HasPitchedVox = Bool(),
              HasUnpitchedVox = Bool().Then(Skip(9)),
              // 9 bytes here are zero in every single rbmid I have found. So we are skipping them.
              LowNote = Float(),
              HighNote = Float(),
              PhraseFlags = Byte(),
              PercussionSection = Bool().Then(() => { Check(Int(), 0); Check(Int(), 0); }),
              // 8 bytes here are zero in every single rbmid I have found. So we are skipping them.
          };
        private RBMid.UNKSTRUCT1 ReadUnkstruct1() => new RBMid.UNKSTRUCT1
        {
            Tick = UInt(),
            FloatData = Float()
        };
        private RBMid.VocalTrackRange ReadVocalTrackRange() => new RBMid.VocalTrackRange
        {
            StartMillis = Float(),
            StartTicks = Int(),
            LowNote = Float(),
            HighNote = Float(),
        };
        private RBMid.MAP ReadMap() => new RBMid.MAP
        {
            StartTime = Float(),
            Map = Int()
        };
        private RBMid.HANDPOS ReadHandPos() => new RBMid.HANDPOS
        {
            StartTime = Float(),
            Length = Float(),
            Position = Int(),
            Unknown = Byte()
        };
        private RBMid.MARKUP_SOLO_NOTES ReadMarkupSoloNotes() => new RBMid.MARKUP_SOLO_NOTES
        {
            StartTick = UInt(),
            EndTick = UInt(),
            NoteOffset = Int()
        };
        private RBMid.TWOTICKS ReadTwoTicks() => new RBMid.TWOTICKS
        {
            StartTick = UInt(),
            EndTick = UInt()
        };
        private RBMid.MARKUPCHORD ReadMarkupChord() => new RBMid.MARKUPCHORD
        {
            StartTick = UInt(),
            EndTick = UInt(),
            Pitches = Arr(Int)
        };

        private RBMid.RBVREVENTS ReadVREvents() => new RBMid.RBVREVENTS
        {
            BeatmatchSections = Arr(() => new RBMid.RBVREVENTS.BEATMATCH_SECTION
            {
                unk_zero = Check(Int(), 0),
                beatmatch_section = String(),
                StartTick = UInt(),
                EndTick = UInt()
            }),
            UnkStruct1 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT1
            {
                Unk1 = Int(),
                StartPercentage = Float(),
                EndPercentage = Float(),
                StartTick = UInt(),
                EndTick = UInt(),
                Unk2 = Int()
            }),
            UnkStruct2 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT2
            {
                Unk = Int(),
                Name = String(),
                Tick = UInt()
            }),
            UnkStruct3 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT3
            {
                Unk1 = Int(),
                exsandohs = String(),
                StartTick = UInt(),
                EndTick = UInt(),
                Flags = FixedArr(Byte, 7),
                Unk2 = Int()
            }),
            UnkStruct4 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT4
            {
                Unk = Int(),
                Name = String(),
                StartTick = UInt(),
                EndTick = UInt()
            }),
            UnkStruct5 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT5
            {
                Unk1 = Int(),
                Name = String(),
                ExsOhs = Arr(String),
                StartTick = UInt(),
                EndTick = UInt(),
                Unk2 = Byte()
            }),
            UnknownTicks = Arr(UInt),
            UnkZero2 = Check(Int(), 0),
            UnkStruct6 = Arr(() => new RBMid.RBVREVENTS.UNKSTRUCT6
            {
                Tick = UInt(),
                Unk = Int()
            })
        };
    }
}

namespace LibForge.Midi
{
    public class RBMidWriter : WriterBase<RBMid>
    {
        public static void WriteStream(RBMid r, Stream s)
        {
            new RBMidWriter(s).WriteStream(r);
        }
        private RBMidWriter(Stream s) : base(s) { }
        public override void WriteStream(RBMid r)
        {
            Write(r.Format);
            Write(r.Lyrics, WriteLyrics);
            Write(r.DrumFills, WriteDrumFills);
            Write(r.Anims, WriteAnims);
            Write(r.ProMarkers, WriteProCymbalMarkers);
            Write(r.LaneMarkers, WriteLaneMarkers);
            Write(r.TrillMarkers, WriteTrillMarkers);
            Write(r.DrumMixes, WriteDrumMixes);
            Write(r.GemTracks, WriteGemTracks);
            Write(r.OverdriveSoloSections, WriteSectionMarkers);
            Write(r.VocalTracks, WriteReadVocalTrack);
            Write(r.UnknownOne);
            Write(r.UnknownNegOne);
            Write(r.UnknownHundred);
            Write(r.Unknown4, WriteUnkstruct1);
            Write(r.VocalRange, WriteVocalRange);
            Write(r.HopoThreshold);
            Write(r.NumPlayableTracks);
            Write(r.FinalEventTick);
            if (r.Format == RBMid.FORMAT_RBVR)
            {
                Write(r.UnkVrTick);
            }
            Write(r.UnknownZeroByte);
            Write(r.PreviewStartMillis);
            Write(r.PreviewEndMillis);
            Write(r.HandMaps, x => Write(x, WriteMap));
            Write(r.GuitarLeftHandPos, x => Write(x, WriteHandPos));
            Write(r.StrumMaps, x => Write(x, WriteMap));
            // begin weirdness
            Write(r.MarkupSoloNotes1, WriteSoloNotes);
            Write(r.MarkupLoop1, WriteTwoTicks);
            Write(r.MarkupChords1, WriteMarkupChord);
            Write(r.MarkupSoloNotes2, WriteSoloNotes);
            Write(r.MarkupSoloNotes3, WriteSoloNotes);
            Write(r.MarkupLoop2, WriteTwoTicks);
            // end weirdness
            if (r.Format == RBMid.FORMAT_RBVR)
            {
                WriteVREvents(r.VREvents);
            }
            MidiFileResourceWriter.WriteStream(r, s);
        }

        private void WriteTickText(RBMid.TICKTEXT obj)
        {
            Write(obj.Tick);
            Write(obj.Text);
        }
        private void WriteLyrics(RBMid.LYRICS obj)
        {
            Write(obj.TrackName);
            Write(obj.Lyrics, WriteTickText);
            Write(obj.Unknown1);
            Write(obj.Unknown2);
            Write(obj.Unknown3);
        }
        private void WriteDrumFills(RBMid.DRUMFILLS obj)
        {
            Write(obj.Lanes, o =>
            {
                Write(o.Tick);
                Write(o.Lanes);
            });
            Write(obj.Fills, o =>
            {
                Write(o.StartTick);
                Write(o.EndTick);
                Write(o.IsBRE);
            });
        }
        private void WriteAnims(RBMid.ANIM obj)
        {
            Write(obj.TrackName);
            Write(obj.Unknown1);
            Write(obj.Unknown2);
            Write(obj.Events, o =>
            {
                Write(o.StartMillis);
                Write(o.StartTick);
                Write(o.LengthMillis);
                Write(o.LengthTicks);
                Write(o.KeyBitfield);
                Write(o.Unknown2);
                Write(o.Unknown3);
            });
            Write(obj.Unknown3);
        }
        private void WriteProCymbalMarkers(RBMid.TOMMARKER obj)
        {
            Write(obj.Markers, o =>
            {
                Write(o.Tick);
                Write((int)o.Flags);
            });
            Write(obj.Unknown1);
            Write(obj.Unknown2);
        }
        private void WriteLaneMarkers(RBMid.LANEMARKER obj)
        {
            Write(obj.Markers, diff => Write(diff, marker =>
            {
                Write(marker.StartTick);
                Write(marker.EndTick);
                Write((uint)marker.Lanes);
            }));
        }
        private void WriteTrillMarkers(RBMid.GTRTRILLS obj)
        {
            Write(obj.Trills, x => Write(x, o =>
            {
                Write(o.StartTick);
                Write(o.EndTick);
                Write(o.FirstFret);
                Write(o.SecondFret);
            }));
        }
        private void WriteDrumMixes(RBMid.DRUMMIXES obj)
        {
            Write(obj.Mixes, o => Write(o, WriteTickText));
        }
        private void WriteGemTracks(RBMid.GEMTRACK obj)
        {
            Write(obj.Gems, gems =>
            {
                Write(0xAA);
                Write(gems, x =>
                {
                    Write(x.StartMillis);
                    Write(x.StartTicks);
                    Write(x.LengthMillis);
                    Write(x.LengthTicks);
                    Write(x.Lanes);
                    Write(x.IsHopo);
                    Write(x.NoTail);
                    Write(x.ProCymbal);
                });
            });
            Write(obj.HopoThreshold);
        }
        private void WriteSectionMarkers(RBMid.SECTIONS obj)
        {
            Write(obj.Sections, a => Write(a, b => Write(b, x => {
                Write(x.StartTicks);
                Write(x.LengthTicks);
            })));
        }
        private void WriteReadVocalTrack(RBMid.VOCALTRACK obj)
        {
            Write(obj.FakePhraseMarkers, WritePhraseMarker);
            Write(obj.AuthoredPhraseMarkers, WritePhraseMarker);
            Write(obj.Notes, x =>
            {
                Write(x.PhraseIndex);
                Write(x.MidiNote);
                Write(x.MidiNote2);
                Write(x.StartMillis);
                Write(x.StartTick);
                Write(x.LengthMillis);
                Write(x.LengthTicks);
                Write(x.Lyric);
                Write(x.LastNoteInPhrase);
                Write(x.False1);
                Write(x.Unpitched);
                Write(x.UnpitchedGenerous);
                Write(x.RangeDivider);
                Write(x.PhraseFlags);
                Write(x.Portamento);
                Write(x.LyricShift);
                Write(x.ShowLyric);
            });
            Write(obj.Percussion, Write);
            Write(obj.FreestyleRegions, x =>
            {
                Write(x.StartMillis);
                Write(x.EndMillis);
            });
        }
        private void WritePhraseMarker(RBMid.VOCALTRACK.PHRASE_MARKER obj)
        {
            Write(obj.StartMillis);
            Write(obj.LengthMillis);
            Write(obj.StartTicks);
            Write(obj.LengthTicks);
            Write(obj.StartNoteIdx);
            Write(obj.EndNoteIdx);
            Write(obj.HasPitchedVox);
            Write(obj.HasUnpitchedVox);
            s.Position += 9;
            Write(obj.LowNote);
            Write(obj.HighNote);
            Write(obj.PhraseFlags);
            Write(obj.PercussionSection);
            s.Position += 8;
        }
        private void WriteUnkstruct1(RBMid.UNKSTRUCT1 obj)
        {
            Write(obj.Tick);
            Write(obj.FloatData);
        }
        private void WriteVocalRange(RBMid.VocalTrackRange obj)
        {
            Write(obj.StartMillis);
            Write(obj.StartTicks);
            Write(obj.LowNote);
            Write(obj.HighNote);
        }
        private void WriteMap(RBMid.MAP obj)
        {
            Write(obj.StartTime);
            Write(obj.Map);
        }
        private void WriteHandPos(RBMid.HANDPOS obj)
        {
            Write(obj.StartTime);
            Write(obj.Length);
            Write(obj.Position);
            Write(obj.Unknown);
        }
        private void WriteSoloNotes(RBMid.MARKUP_SOLO_NOTES obj)
        {
            Write(obj.StartTick);
            Write(obj.EndTick);
            Write(obj.NoteOffset);
        }
        private void WriteTwoTicks(RBMid.TWOTICKS obj)
        {
            Write(obj.StartTick);
            Write(obj.EndTick);
        }
        private void WriteMarkupChord(RBMid.MARKUPCHORD obj)
        {
            Write(obj.StartTick);
            Write(obj.EndTick);
            Write(obj.Pitches, Write);
        }
        private void WriteVREvents(RBMid.RBVREVENTS obj)
        {
            Write(obj.BeatmatchSections, e =>
            {
                Write(e.unk_zero);
                Write(e.beatmatch_section);
                Write(e.StartTick);
                Write(e.EndTick);
            });
            Write(obj.UnkStruct1, x =>
            {
                Write(x.Unk1);
                Write(x.StartPercentage);
                Write(x.EndPercentage);
                Write(x.StartTick);
                Write(x.EndTick);
                Write(x.Unk2);
            });
            Write(obj.UnkStruct2, x =>
            {
                Write(x.Unk);
                Write(x.Name);
                Write(x.Tick);
            });
            Write(obj.UnkStruct3, x =>
            {
                Write(x.Unk1);
                Write(x.exsandohs);
                Write(x.StartTick);
                Write(x.EndTick);
                Array.ForEach(x.Flags, Write);
                Write(x.Unk2);
            });
            Write(obj.UnkStruct4, x =>
            {
                Write(x.Unk);
                Write(x.Name);
                Write(x.StartTick);
                Write(x.EndTick);
            });
            Write(obj.UnkStruct5, x =>
            {
                Write(x.Unk1);
                Write(x.Name);
                Write(x.ExsOhs, Write);
                Write(x.StartTick);
                Write(x.EndTick);
                Write(x.Unk2);
            });
            Write(obj.UnknownTicks, Write);
            Write(obj.UnkZero2);
            Write(obj.UnkStruct6, x =>
            {
                Write(x.Tick);
                Write(x.Unk);
            });
        }
    }
}
