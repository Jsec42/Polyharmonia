using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polyharmonia
{
    public class MIDIEngine
    {
        private List<phMidi.notes.note> _notesPlaying;
        private phMidi midiInterface;
        private int _channel;
        private int _velocity;
        private instrumentDatabase.instrument _instrument;

        public instrumentDatabase.instrument instrument
        {
            get => _instrument; set
            {
                _instrument = value;
                midiInterface.changeVoice(value, _channel);
            }
        }
        public int velocity
        {
            get => _velocity; set
            {
                _velocity = value;
                foreach (phMidi.notes.note n in _notesPlaying)
                {
                    playNote(n, true);
                }
            }
        }
        public int channel
        {
            get => _channel; set
            {
                int tmp_channel;
                foreach (phMidi.notes.note n in _notesPlaying)
                {
                    tmp_channel = _channel;
                    stopNote(n, true);
                    _channel = value;
                    playNote(n, true);
                    _channel = tmp_channel;
                }
                _channel = channel;
            }
        }
        public MIDIEngine(phMidi midi_interface, int velocity_t, int channel_t, instrumentDatabase.instrument instrument_t)
        {
            midiInterface = new phMidi(midi_interface);
            _velocity = velocity_t;
            _channel = channel_t;
            instrument = instrument_t;
            _notesPlaying = new List<phMidi.notes.note>();
        }

        public bool playNote(phMidi.notes.note note, bool force = false)
        {
            if (!_notesPlaying.Contains(note) || force)
            {
                midiInterface.playNote(note, _velocity, _channel);
                if (!force)
                    _notesPlaying.Add(note);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool stopNote(phMidi.notes.note note, bool force = false)
        {
            if (_notesPlaying.Contains(note) || force)
            {
                midiInterface.stopNote(note, _velocity, _channel);
                if (!force)
                    _notesPlaying.Remove(note);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void clear()
        {
             while(_notesPlaying.Count > 0)  
                stopNote(_notesPlaying[0]);
        }
    }
    public class rankPresetInfo
    {
        public instrumentDatabase.instrument instrument;
        public bool locked_tone;
        public bool locked_interval;
        public bool negative_interval;
        public phMidi.notes.note note;
        public phMidi.intervals.interval[] intervals;
    }

    public class rank
    {
        public bool[] keys = { false, false, false, false };
        public bool[] interval_keys = { false, false, false, false, false, false, false, false, false, false, false, false };
        public rankPresetInfo[] presets = new rankPresetInfo[10];
        private phMidi.notes.note _note;
        private List<phMidi.intervals.interval> current_intervals;
        private phMidi.intervals.interval _base_interval;
        public MIDIEngine engine;
        private bool _isLockedPitch;
        private bool _isLockedInterval;
        private bool _isNegativeInterval;
        private bool _manual_command = false;
        private bool _auto_command = false;
        private bool _isSustain;
        public int lastDecision = 0;
        public phMidi.notes.note note { get => _note; set
            {
                _note = value;
                changeKey();
            } }
        public bool doSustain
        {
            get => _isSustain; set
            {
                if (!value && _isSustain)
                    engine.clear();
                if (command)
                    _playAll();
                _isSustain = value;
            }
        }
        public bool command
        {
            get => _manual_command || _auto_command || _isSustain; set
            {
                if(!_isSustain && !_auto_command)
                {
                    if (!value)
                    {
                        engine.clear();
                    }
                    else
                    {
                        _playAll();
                    }
                }
                _manual_command = value;
            }
        }
        public bool negative_interval
        {
            get => _isNegativeInterval; set
            {
                _isNegativeInterval = value;
                if (!_isLockedInterval)
                    changeInterval();
            }
        }
        public bool lockedPitch
        {
            get => _isLockedPitch;
            set
            {
                _isLockedPitch = value;
                if (!value)
                    changeKey();
            }
        }
        public bool lockedInterval
        {
            get => _isLockedInterval;
            set
            {
                _isLockedInterval = value;
                if (!value)
                    changeInterval();
            }
        }
        public rank(phMidi midi_interface, int velocity_t, int channel_t, instrumentDatabase.instrument instrument_t, phMidi.notes.note note_start, bool locked_t, bool locked_interval_t)
        {
            engine = new MIDIEngine(midi_interface, velocity_t, channel_t, instrument_t);
            _note = note_start;
            _isLockedPitch = locked_t;
            _isLockedInterval = locked_interval_t;
            _base_interval = phMidi.intervals.i1;
            current_intervals = new List<phMidi.intervals.interval>();
        }
        private void _playAll()
        {
            phMidi.notes.note baseNote = _note.interval(_base_interval);
            if (_auto_command || _manual_command)
            {
                engine.playNote(baseNote);
                foreach (phMidi.intervals.interval interval in current_intervals)
                {
                    engine.playNote(baseNote.interval(interval));
                }

            }
        }
        public void changePreset(int p)
        {
            if (p < 0 || p > 9)
                throw new IndexOutOfRangeException();
            rankPresetInfo selected = presets[p];
            if (selected != null)
            {
                _note = selected.note;
                current_intervals = new List<phMidi.intervals.interval>(selected.intervals);
                lockedInterval = selected.locked_interval;
                lockedPitch = selected.locked_tone;
                negative_interval = selected.negative_interval;
                engine.clear();
                engine.instrument = selected.instrument;
                changeKey();
                changeInterval();
            }
        }
        public void savePreset(int p)
        {
            if (p < 0 || p > 9)
                throw new IndexOutOfRangeException();
            rankPresetInfo presetInfo = new rankPresetInfo();
            presetInfo.instrument = engine.instrument;
            presetInfo.note = _note;
            presetInfo.intervals = current_intervals.ToArray();
            presetInfo.locked_interval = lockedInterval;
            presetInfo.locked_tone = lockedPitch;
            presetInfo.negative_interval = negative_interval;
            presets[p] = presetInfo;
        }
        public void changeKey()
        {
            int decision = 0;
            int tmp = 0;
            if (!_isLockedPitch)
            {

                decision = (keys[0] ? 1 : 0) + (keys[1] ? 2 : 0) + (keys[2] ? 4 : 0) + (keys[3] ? 8 : 0);
                /*
                switch (decision)
                {
                    case 1:
                        _base_interval = phMidi.intervals.i1;
                        break;
                    case 3:
                        _base_interval = phMidi.intervals.i2.minor;
                        break;
                    case 2:
                        _base_interval = phMidi.intervals.i2;
                        break;
                    case 6:
                        _base_interval = phMidi.intervals.i3.minor;
                        break;
                    case 7:
                        _base_interval = phMidi.intervals.i3;
                        break;
                    case 5:
                        _base_interval = phMidi.intervals.i4;
                        break;
                    case 4:
                        _base_interval = phMidi.intervals.i5.minor;
                        break;
                    case 12:
                        _base_interval = phMidi.intervals.i5;
                        break;
                    case 13:
                        _base_interval = phMidi.intervals.i6.minor;
                        break;
                    case 15:
                        _base_interval = phMidi.intervals.i6;
                        break;
                    case 14:
                        _base_interval = phMidi.intervals.i7.minor;
                        break;
                    case 9:
                    case 10:
                        _base_interval = phMidi.intervals.i7;
                        break;
                    case 8:
                        _base_interval = phMidi.intervals.i1.octave(1);
                        break;
                }
                */
                switch (decision)
                {
                    case 1:
                        _base_interval = phMidi.intervals.i1;
                        break;
                    case 2:
                        _base_interval = phMidi.intervals.i2.minor;
                        break;
                    case 3:
                        _base_interval = phMidi.intervals.i2;
                        break;
                    case 6:
                        _base_interval = phMidi.intervals.i3.minor;
                        break;
                    case 7:
                        _base_interval = phMidi.intervals.i3;
                        break;
                    case 5:
                        _base_interval = phMidi.intervals.i4;
                        break;
                    case 4:
                        _base_interval = phMidi.intervals.i5.minor;
                        break;
                    case 13:
                        _base_interval = phMidi.intervals.i5;
                        break;
                    case 14:
                        _base_interval = phMidi.intervals.i6.minor;
                        break;
                    case 15:
                        _base_interval = phMidi.intervals.i6;
                        break;
                    case 10:
                        _base_interval = phMidi.intervals.i7.minor;
                        break;
                    case 11:
                        _base_interval = phMidi.intervals.i7;
                        break;
                    case 9:
                        _base_interval = phMidi.intervals.i1.octave(1);
                        break;
                }
                lastDecision = decision;
                if (decision != 0)
                {
                    _auto_command = true;
                    changeInterval();
                }
                else
                {
                    _auto_command = false;
                    if (!_manual_command && !_isSustain)
                        engine.clear();
                }
            }
        }
        public void changeInterval()
        {
            phMidi.notes.note this_note = _note.interval(_base_interval);
            phMidi.intervals.interval tmpInterval = phMidi.intervals.i2.minor;
            if (!_isSustain)
                engine.clear();
            if (!_isLockedInterval)
            {

                current_intervals.Clear();
                if (tmpInterval == phMidi.intervals.i2.minor && _isNegativeInterval)
                    tmpInterval = tmpInterval.inverted;
                for(int i=0;i<interval_keys.Length;i++)
                {
                    bool playInterval = interval_keys[i];
                    if (playInterval)
                    {
                        current_intervals.Add(tmpInterval);
                    }
                    tmpInterval++;
                }
            }
            _playAll();
        }
    }
    public struct Polyharmonia_Keys
    {
        public struct rank
        {
            public Keys key_0;
            public Keys key_1;
            public Keys key_2;
            public Keys key_3;
            public Keys Lock;
            public Keys Interval_Lock;
        }
        public struct intervalKey
        {
            public Keys minor_2nd;
            public Keys major_2nd;
            public Keys minor_3rd;
            public Keys major_3rd;
            public Keys major_4th;
            public Keys minor_5th;
            public Keys major_5th;
            public Keys minor_6th;
            public Keys major_6th;
            public Keys minor_7th;
            public Keys major_7th;
            public Keys Octave;
            public Keys Direction_up;
            public Keys Direction_down;
        }
        public struct presetKey
        {
            public Keys store;
            public Keys preset_0;
            public Keys preset_1;
            public Keys preset_2;
            public Keys preset_3;
            public Keys preset_4;
            public Keys preset_5;
            public Keys preset_6;
            public Keys preset_7;
            public Keys preset_8;
            public Keys preset_9;
        }
        public struct controls
        {
            public Keys hold;
            public Keys manual_command;
            public Keys set_transpose;
            public Keys Octave_increment;
            public Keys Octave_decrement;
            public Keys toggle_hold_trigger_mode;
            public Keys trigger_held_rank;
        }

        public rank rank_0;
        public rank rank_1;
        public rank rank_2;
        public intervalKey interval;
        public presetKey preset;
        public presetKey preset_alt;
        public controls control;
    }
    public struct controlEventArgs
    {
        public bool key_status;
        public bool hold;
        public bool manual_command;
        public bool set_transpose;
        public bool Octave_increment;
        public bool Octave_decrement;
        public bool toggle_hold_trigger_mode;
        public bool trigger_held_rank;
    }
    public class packedPresetInfoDTO
    {
        rankPresetInfo[] rank_1 = new rankPresetInfo[10];
        rankPresetInfo[] rank_2 = new rankPresetInfo[10];
        rankPresetInfo[] rank_3 = new rankPresetInfo[10];
    }
    public class Polyhamronia_keyboard_event_handler
    {
        //define delegate types
        public delegate void type_rank_event(int rank_index, int key_index, bool value);
        public delegate void type_interval_change_event(int interval_key_index, bool value);
        public delegate void type_preset_event(int preset, bool is_store);
        public delegate void type_control_event(controlEventArgs e);
        //declare delegate instances
        public type_rank_event rank_event;
        public type_interval_change_event interval_Event ;
        public type_preset_event preset_Event;
        public type_control_event control_event;
        //other necessary delegations
        public Polyharmonia_Keys keymap;
        bool isStore = false;
        public Polyhamronia_keyboard_event_handler(Polyharmonia_Keys polyharmonia_Keys)
        {
            keymap = polyharmonia_Keys;
        }
        public void keyDispatcher(KeyEventArgs keyEvent, bool isKeyDown)
        {
            Keys keycode = keyEvent.KeyCode;
            //rank event dispatcher
            if (keycode == keymap.rank_0.key_0)
                rank_event(0, 0, isKeyDown);
            if (keycode == keymap.rank_0.key_1)
                rank_event(0, 1, isKeyDown);
            if (keycode == keymap.rank_0.key_2)
                rank_event(0, 2, isKeyDown);
            if (keycode == keymap.rank_0.key_3)
                rank_event(0, 3, isKeyDown);
            if (keycode == keymap.rank_0.Lock && isKeyDown)
                rank_event(0, -1, true);
            if (keycode == keymap.rank_0.Interval_Lock && isKeyDown)
                rank_event(0, -2, true);
            if (keycode == keymap.rank_1.key_0)
                rank_event(1, 0, isKeyDown);
            if (keycode == keymap.rank_1.key_1)
                rank_event(1, 1, isKeyDown);
            if (keycode == keymap.rank_1.key_2)
                rank_event(1, 2, isKeyDown);
            if (keycode == keymap.rank_1.key_3)
                rank_event(1, 3, isKeyDown);
            if (keycode == keymap.rank_1.Lock && isKeyDown)
                rank_event(1, -1, true);
            if (keycode == keymap.rank_1.Interval_Lock)
                rank_event(1, -2, true);
            if (keycode == keymap.rank_2.key_0)
                rank_event(2, 0, isKeyDown);
            if (keycode == keymap.rank_2.key_1)
                rank_event(2, 1, isKeyDown);
            if (keycode == keymap.rank_2.key_2)
                rank_event(2, 2, isKeyDown);
            if (keycode == keymap.rank_2.key_3)
                rank_event(2, 3, isKeyDown);
            if (keycode == keymap.rank_2.Lock && isKeyDown)
                rank_event(2, -1, true);
            if (keycode == keymap.rank_2.Interval_Lock && isKeyDown)
                rank_event(2, -2, true);
            //interval change event dispatcher
            if (keycode == keymap.interval.Direction_down && isKeyDown)
                interval_Event(-1, false);
            if (keycode == keymap.interval.Direction_up && isKeyDown)
                interval_Event(-1, true);
            if (keycode == keymap.interval.major_2nd)
                interval_Event(2, isKeyDown);
            if (keycode == keymap.interval.major_3rd)
                interval_Event(4, isKeyDown);
            if (keycode == keymap.interval.major_4th)
                interval_Event(5, isKeyDown);
            if (keycode == keymap.interval.major_5th)
                interval_Event(7, isKeyDown);
            if (keycode == keymap.interval.major_6th)
                interval_Event(9, isKeyDown);
            if (keycode == keymap.interval.major_7th)
                interval_Event(11, isKeyDown);
            if (keycode == keymap.interval.minor_2nd)
                interval_Event(1, isKeyDown);
            if (keycode == keymap.interval.minor_3rd)
                interval_Event(3, isKeyDown);
            if (keycode == keymap.interval.minor_5th)
                interval_Event(6, isKeyDown);
            if (keycode == keymap.interval.minor_6th)
                interval_Event(8, isKeyDown);
            if (keycode == keymap.interval.minor_7th)
                interval_Event(10, isKeyDown);
            //preset event dispatcher
            if(keycode == keymap.preset.store || keycode == keymap.preset_alt.store)
            {
                isStore = isKeyDown;
                return;
            }
            else
            {
                if (!isKeyDown)
                {
                    if (keycode == keymap.preset.preset_0 || keycode == keymap.preset_alt.preset_0)
                        preset_Event(0, isStore);
                    if (keycode == keymap.preset.preset_1 || keycode == keymap.preset_alt.preset_1)
                        preset_Event(1, isStore);
                    if (keycode == keymap.preset.preset_2 || keycode == keymap.preset_alt.preset_2)
                        preset_Event(2, isStore);
                    if (keycode == keymap.preset.preset_3 || keycode == keymap.preset_alt.preset_3)
                        preset_Event(3, isStore);
                    if (keycode == keymap.preset.preset_4 || keycode == keymap.preset_alt.preset_4)
                        preset_Event(4, isStore);
                    if (keycode == keymap.preset.preset_5 || keycode == keymap.preset_alt.preset_5)
                        preset_Event(5, isStore);
                    if (keycode == keymap.preset.preset_6 || keycode == keymap.preset_alt.preset_6)
                        preset_Event(6, isStore);
                    if (keycode == keymap.preset.preset_7 || keycode == keymap.preset_alt.preset_7)
                        preset_Event(7, isStore);
                    if (keycode == keymap.preset.preset_8 || keycode == keymap.preset_alt.preset_8)
                        preset_Event(8, isStore);
                    if (keycode == keymap.preset.preset_9 || keycode == keymap.preset_alt.preset_9)
                        preset_Event(9, isStore);
                }
            }
            //control event dispatcher
            controlEventArgs args = new controlEventArgs();
            args.key_status = isKeyDown;
            args.hold = keycode == keymap.control.hold;
            args.Octave_decrement = keycode == keymap.control.Octave_decrement;
            args.Octave_increment = keycode == keymap.control.Octave_increment;
            args.set_transpose = keycode == keymap.control.set_transpose;
            args.toggle_hold_trigger_mode = keycode == keymap.control.toggle_hold_trigger_mode;
            args.manual_command = keycode == keymap.control.manual_command;
            args.trigger_held_rank = keycode == keymap.control.trigger_held_rank;
            control_event(args);
        }
    }
    public class Polyharmonia_engine
    {
        public rank[] ranks = new rank[3];
        private bool[] transpose_keys = new bool[4];
        private int transpose_octave;
        phMidi root_interface;
        private instrumentDatabase.instrument[] _rank_instruments;
        public Polyhamronia_keyboard_event_handler key_handler;
        bool transpose_ready;
        Keys lastKeyCode = 0;
        public delegate void type_display_callback();
        public type_display_callback display_callback;
        public Timer keyChange_timer;
        private delegate void updateEvent();
        private List<updateEvent> currentEvents;
        public ref instrumentDatabase.instrument[] rank_instruments { get
            {
                _rank_instruments = new instrumentDatabase.instrument[3];
                int i = 0;
                foreach (rank a in ranks)
                {
                    _rank_instruments[i++] = a.engine.instrument;
                }
                return ref _rank_instruments;
            }
        }
        public Polyharmonia_engine(phMidi root_interface, Polyharmonia_Keys new_keymap, int initial_interval = 10)
        {
            for(int i = 0; i<3; i++)
            {
                ranks[i] = new rank(root_interface, 64, i, new instrumentDatabase.instrument(), phMidi.notes.C.octave(i + 2), false, false);
            }
            transpose_ready = false;
            key_handler = new Polyhamronia_keyboard_event_handler(new_keymap);
            key_handler.control_event = control_event;
            key_handler.interval_Event = interval_change_event;
            key_handler.preset_Event = preset_event;
            key_handler.rank_event = rank_event;
            keyChange_timer = new Timer();
            keyChange_timer.Interval = initial_interval;
            keyChange_timer.Tick += runCurrentEvents;
            currentEvents = new List<updateEvent>();
        }
        public void updateRankInstruments()
        {
            for(int i=0; i<ranks.Length; i++)
            {
                ranks[i].engine.instrument = _rank_instruments[i];
            }
        }

        private phMidi.notes.note getTransposeNote()
        {
            int decision = 0;
            int tmp = 0;
            phMidi.intervals.interval _base_interval;
            foreach (bool key in transpose_keys)
            {
                if (key)
                    decision += 1 << tmp;
                tmp++;
            }
            switch (decision)
            {
                case 0:
                case 1:
                    _base_interval = phMidi.intervals.i1;
                    break;
                case 3:
                    _base_interval = phMidi.intervals.i2.minor;
                    break;
                case 2:
                    _base_interval = phMidi.intervals.i2;
                    break;
                case 6:
                    _base_interval = phMidi.intervals.i3.minor;
                    break;
                case 7:
                    _base_interval = phMidi.intervals.i3;
                    break;
                case 5:
                    _base_interval = phMidi.intervals.i4;
                    break;
                case 4:
                    _base_interval = phMidi.intervals.i5.minor;
                    break;
                case 12:
                    _base_interval = phMidi.intervals.i5;
                    break;
                case 13:
                    _base_interval = phMidi.intervals.i6.minor;
                    break;
                case 9:
                case 14:
                    _base_interval = phMidi.intervals.i6;
                    break;
                case 11:
                    _base_interval = phMidi.intervals.i7.minor;
                    break;
                case 10:
                case 15:
                    _base_interval = phMidi.intervals.i7;
                    break;
                case 8:
                default:
                    _base_interval = phMidi.intervals.i1.octave(1);
                    break;
            }
            return phMidi.notes.C.octave(3 + transpose_octave).interval(_base_interval);
        }
        public void keyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (lastKeyCode != e.KeyCode)
                key_handler.keyDispatcher(e, true);
            lastKeyCode = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        public void keyUpEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            key_handler.keyDispatcher(e, false);
            lastKeyCode = 0;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        public void rank_event(int rank_index, int key_index, bool value)
        {
            ref rank cRank = ref ranks[rank_index];
            if (key_index == -1)
                cRank.lockedPitch = !cRank.lockedPitch;
            else if (key_index == -2)
                cRank.lockedInterval = !cRank.lockedInterval;
            else
            {
                if (!transpose_ready)
                {
                    keyChange_timer.Stop();
                    cRank.keys[key_index] = value;
                    if (!currentEvents.Contains(cRank.changeKey))
                        currentEvents.Add(cRank.changeKey);
                    keyChange_timer.Start();
                }
                else
                {
                    transpose_keys[key_index] = value;
                    transpose_octave = rank_index;
                }
            }
            if (display_callback != null)
                display_callback();
        }
        public void interval_change_event(int interval_key_index, bool value)
        {
            keyChange_timer.Stop();
            for (int i=0; i<ranks.Length; i++)
            {
                ref rank cRank = ref ranks[i];
                if (interval_key_index == -1)
                {
                    cRank.negative_interval = value;
                }
                else
                {
                    cRank.interval_keys[interval_key_index] = value;
                }
                if (!currentEvents.Contains(cRank.changeInterval))
                    currentEvents.Add(cRank.changeInterval);
            }
            keyChange_timer.Start();
            if (display_callback != null)
                display_callback();
        }
        public void preset_event(int preset, bool is_store)
        {
            for(int i = 0; i < ranks.Length; i++)
            {
                ref rank cRank = ref ranks[i];
                if (is_store)
                    cRank.savePreset(preset);
                else
                    cRank.changePreset(preset);
            }
            if (display_callback != null)
                display_callback();
        }
        public void control_event(controlEventArgs e)
        {
            for(int i=0; i<ranks.Length; i++)
            {
                ref rank cRank = ref ranks[i];
                if (e.hold)
                    cRank.doSustain = e.key_status;
                if (e.manual_command)
                    cRank.command = e.key_status;
                if (cRank.command)
                {
                    phMidi.notes.note cNote = cRank.note;
                    if (e.Octave_decrement && e.key_status)
                        cNote = cNote.octave(-1);
                    if (e.Octave_increment && e.key_status)
                        cNote = cNote.octave(1);
                }
                if (e.set_transpose)
                {
                    if (e.key_status)
                    {
                        transpose_ready = true;
                        transpose_keys = new bool[4];
                    }
                    else if (!cRank.lockedPitch)
                        cRank.note = getTransposeNote();
                }
            }
            if (display_callback != null)
                display_callback();
        }
        public void loadTotalPresetInfo(object sender, CancelEventArgs e)
        {
            OpenFileDialog source = (OpenFileDialog)sender;
        }
        public void saveTotalPresetInfo(object sender, CancelEventArgs e)
        {
            SaveFileDialog source = (SaveFileDialog)sender;

        }
        private void runCurrentEvents(object sender, EventArgs e)
        {
            ((Timer)sender).Stop();
            foreach (updateEvent u in currentEvents)
                u();
        }
    }
}
