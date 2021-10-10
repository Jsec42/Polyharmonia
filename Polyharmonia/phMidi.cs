/*
 * MIDI interface class used by Polyharmonia.
 * 
 * Special thanks to Peter Shaw and CodeGuru for providing much of the backend code to make this work! You can find the article used here.
 * https://www.codeguru.com/dotnet/making-music-with-midi-and-c/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using ANPSupport;
using System.Windows.Forms;
using System.CodeDom;

namespace Polyharmonia
{
    public class phMidi
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr winHandle);

        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(Int32 uDeviceID, ref MidiOutCaps lpMidiOutCaps, UInt32 cbMidiOutCaps);

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref int handle, int deviceID, MidiCallBack proc, int instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(int handle, int message);

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(int handle);

        [StructLayout(LayoutKind.Sequential)]
        public struct MidiOutCaps
        {
            public UInt16 wMid;
            public UInt16 wPid;
            public UInt32 vDriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public String szPname;

            public UInt16 wTechnology;
            public UInt16 wVoices;
            public UInt16 wNotes;
            public UInt16 wChannelMask;
            public UInt32 dwSupport;
        }
        public class notes
        {
            public class note
            {
                private int _baseValue;
                public int baseValue { get => _baseValue; }
                public note sharp { get => new note(baseValue + 1); }
                public note flat { get => new note(baseValue - 1); }

                public note(int n)
                {
                    _baseValue = n;
                }

                public static bool operator ==(note a, note b) => a.baseValue == b.baseValue;
                public static bool operator !=(note a, note b) => a.baseValue != b.baseValue;
                public override bool Equals(object obj)
                {
                    return this == (note)obj;
                }
                public override int GetHashCode()
                {
                    return _baseValue;
                }
                public static note operator ++(note a) => a.sharp;
                public static note operator --(note a) => a.flat;
                public note octave(int oValue) => new note(baseValue + oValue * 12);
                public note interval(intervals.interval new_interval) => new note(baseValue + new_interval.magnitude);
                /*public override string ToString()
                {
                    int raw_value = _baseValue - 21;
                    int octave = raw_value / 12;
                    int rem = 
                }*/
            }
            public static note A = new note(21);
            public static note B = new note(23);
            public static note C = new note(24);
            public static note D = new note(26);
            public static note E = new note(28);
            public static note F = new note(29);
            public static note G = new note(31);
        }
        public class intervals
        {
            public class interval
            {
                private int _magnitude;
                public int magnitude { get => _magnitude; }
                public interval minor { get => new interval(_magnitude - (_magnitude < 0 ? -1 : 1)); }
                public interval diminished { get => minor; }
                public interval augmented { get => new interval(_magnitude + (_magnitude < 0 ? -1 : 1)); }
                public interval inverted { get => new interval(0 - _magnitude); }
                public interval(int n_magnitude)
                {
                    _magnitude = n_magnitude;
                }
                public override bool Equals(object obj)
                {
                    return this == (interval)obj;
                }
                public override int GetHashCode()
                {
                    return _magnitude;
                }
                public static bool operator ==(interval a, interval b) => a.magnitude == b.magnitude;
                public static bool operator !=(interval a, interval b) => !(a == b);
                public static interval operator ++(interval a) => a.augmented;
                public static interval operator--(interval a) => a.minor;
                public interval octave(int o_magnitude) => new interval(_magnitude + 12 * o_magnitude);
                public override string ToString()
                {
                    int octave = _magnitude / 12;
                    int frac = _magnitude % 12;
                    bool high = frac > 4;
                    int true_interval = ((_magnitude + (high ? 2 : 1)) / 2)+1;
                    char type = (((_magnitude + (high ? 1 : 0)) % 2)==1)?'-':'+';
                    return octave.ToString() + "(" + true_interval.ToString() + type + ")";
                }
            }
            public static interval i1 = new interval(0);
            public static interval i2 = new interval(2);
            public static interval i3 = new interval(4);
            public static interval i4 = new interval(5);
            public static interval i5 = new interval(7);
            public static interval i6 = new interval(9);
            public static interval i7 = new interval(11);
        }
        public delegate void MidiCallBack(int handle, int msg, int instance, int param1, int param2);
        int handle;
        private int _deviceID;
        bool isInitializedCorrectly = false;
        bool isOriginal;

        public int deviceID { get => _deviceID; }

        public phMidi()
        {
            if (midiOutGetNumDevs() > 0)
            {
                init(0, null);
            }
        }
        public phMidi(int device) => init(device, null);
        public phMidi(int device, MidiCallBack callback) => init(device, callback);//notice - this functionality is UNTESTED!

        public phMidi(phMidi original)
        {
            if (!original.isInitializedCorrectly)
                throw new System.ArgumentException();
            isOriginal = false;
            isInitializedCorrectly = true;
            handle = original.handle;
            _deviceID = original._deviceID;
        }
        private void init(int device, MidiCallBack callBack)
        {
            midiOutOpen(ref handle, device, callBack, 0, 0);
            _deviceID = device;
            isInitializedCorrectly = true;
            isOriginal = true;
        }

        public static int getNumDevices() => midiOutGetNumDevs();
        public MidiOutCaps getDeviceStats()
        {
            MidiOutCaps tmp = new MidiOutCaps();
            if (!isInitializedCorrectly)
            {
                throw new System.NullReferenceException();
            }
            midiOutGetDevCaps(_deviceID, ref tmp, (UInt32)Marshal.SizeOf(tmp));
            return tmp;
        }
        public static string doMci(string command)
        {
            const int returnLength = 256;
            StringBuilder reply = new StringBuilder(returnLength);
            mciSendString(command, reply, returnLength, IntPtr.Zero);
            return reply.ToString();
        }

        private void sendRaw(int message)
        {
            midiOutShortMsg(handle, message);
        }
        private void sendRaw(int[] messageList) { 
            foreach(int msg in messageList)
            {
                sendRaw(msg);
            }
        }
        public void sendCommand(int command, notes.note note, int velocity = 127) => sendRaw((velocity << 16) + (note.baseValue << 8) + command);
        public void sendCommand(int command, int argument, int velocity = 127) => sendRaw((velocity << 16) + (argument << 8) + command);
        public void playNote(notes.note note, int velocity=127, int channel=0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0x90 + channel;
            sendCommand(command, note, velocity);
        }
        public void stopNote(notes.note note, int velocity = 127, int channel = 0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0x80 + channel;
            sendCommand(command, note, velocity);
        }
        public void polyAftertouch(notes.note note, int velocity, int channel = 0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0x90 + channel;
            sendCommand(command, note, velocity);
        }
        public void aftertouch(int velocity, int channel = 0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0xD0 + channel;
            sendCommand(command, velocity);
        }
        public void changeVoice(instrumentDatabase.instrument voice, int channel = 0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0xA0 + channel;
            sendCommand(command, voice.id, 0);
        }
        public void pitchBend(int bend, int channel = 0)
        {
            if (channel > 15)
                throw new System.IndexOutOfRangeException();
            int command = 0xE0 + channel;
            int bendlow = bend & 0x007F;
            int bendhigh = bend & 0x3F80;
            bendhigh <<= 1;
            sendCommand(command, bendhigh, bendlow);
        }

        ~phMidi()
        {
            if(isOriginal)
                midiOutClose(handle);
        }
    }
    public class instrumentDatabase
    {
        public class bank
        {
            public string name = "";
            public int id = 0;

            public bank(int _id, string _name)
            {
                id = _id;
                name = _name;
            }

            public bank()
            {
                id = 0;
                name = "";
            }

            public bank(bank old_bank)
            {
                id = old_bank.id;
                name = old_bank.name;
            }
            public override string ToString()
            {
                return id.ToString() + ": " + name;
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                    return false;
                return this == (bank)obj;
            }
            public override int GetHashCode()
            {
                return (id+name.GetHashCode())/2;
            }
            public static bool operator ==(bank a, bank b) => a.id == b.id && a.name == b.name;
            public static bool operator !=(bank a, bank b) => !(a == b);
        }
        public class instrument
        {
            public string name;
            public int id;
            public bank bankInformation = new bank();

            public instrument(int t_id, string t_name)
            {
                name = t_name;
                id = t_id;
            }
            public instrument()
            {
                name = "";
                id = 0;
            }
            public instrument(instrument old_instrument)
            {
                name = old_instrument.name;
                id = old_instrument.id;
            }
            public override string ToString()
            {
                return "[" + id.ToString() + "](" + bankInformation.ToString() + "):" + name;
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                    return false;
                return this == (instrument)obj;
            }
            public override int GetHashCode()
            {
                return (id + name.GetHashCode())/2;
            }
            public static bool operator ==(instrument a, instrument b) => a.id == b.id && a.name == b.name;
            public static bool operator !=(instrument a, instrument b) => !(a == b);
        }

        public List<instrument> instruments = new List<instrument>();
        public List<bank> banks = new List<bank>();

        public instrumentDatabase(instrumentDatabase old_obj)
        {
            init(old_obj);
        }
        public instrumentDatabase()
        {
            instruments = new List<instrument>();
            banks = new List<bank>();
        }
        private void init(instrumentDatabase old_obj)
        {
            this.instruments = old_obj.instruments;
            this.banks = old_obj.banks;

        }

        public bank getBankByName(string bankName)
        {
            return banks.Find(p => p.name == bankName);
        }
        public bank getBankByID(int id)
        {
            return banks.Find(p => p.id == id);
        }
        public instrument GetInstrumentByName(string instName)
        {
            return instruments.Find(p => p.name == instName);
        }
        public instrument GetInstrumentById(int id)
        {
            return instruments.Find(p => p.id == id);
        }
        public List<instrument> getInstrumentsByBank(bank targetBank)
        {
            return instruments.FindAll(p => p.bankInformation == targetBank);
        }
    }
}
