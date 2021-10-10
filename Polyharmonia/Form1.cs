using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANPSupport;
using System.IO;
using System.Windows.Input;

namespace Polyharmonia
{
    public partial class Form1 : Form
    {
        phMidi currentDevice;
        List<phMidi> availableDevices = new List<phMidi>();
        instrumentDatabase cInstrumentDatabase;
        Polyharmonia_engine music_engine;
        IDatabaseEditor Editor;
        KeymapEditor k_editor;
        bool isUpdating = false;
        Polyharmonia_Keys keymap { get => music_engine.key_handler.keymap; set
            {
                music_engine.key_handler.keymap = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void midiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            devicesToolStripMenuItem.DropDownItems.Clear();
            availableDevices.Clear();
            for(int i=0; i<phMidi.getNumDevices(); i++)
            {
                phMidi newDevice = new phMidi(i);
                phMidi.MidiOutCaps devStat = newDevice.getDeviceStats();
                availableDevices.Add(newDevice);
                ToolStripMenuItem newItem = new ToolStripMenuItem(i.ToString() + ": " + devStat.szPname);
                newItem.Click += new System.EventHandler(deviceSelectToolStripMenuItem_Click);
                newItem.Tag = i;
                newItem.ToolTipText = devStat.ToString();//temporary placeholder
                newItem.Name = "Devices" + devStat.szPname + "ToolStripMenuItem";
                if (i == currentDevice.deviceID)
                    newItem.Checked = true;
                else
                    newItem.Checked = false;
                devicesToolStripMenuItem.DropDownItems.Add(newItem);
            }

        }
        private void deviceSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentDevice = new phMidi((int)((Control)sender).Tag);
            music_engine = new Polyharmonia_engine(currentDevice, keymap);
            availableDevices.Clear();
        }
        private void voicesMenuItemSelect(object sender, EventArgs e)
        {
            int r = -1;
            foreach (ToolStripMenuItem t in ((ToolStripDropDownItem)sender).DropDownItems)
            {
                t.Tag = ++r;
                t.DropDownItems.Clear();
                foreach (instrumentDatabase.bank cBank in cInstrumentDatabase.banks)
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem();
                    newItem.Text = cBank.name;
                    newItem.Tag = cBank;
                    if (music_engine.rank_instruments[r].bankInformation == cBank)
                        newItem.Checked = true;
                    t.DropDownItems.Add(newItem);
                }
            }
        }
        struct instSelect
        {
            public int rank;
            public instrumentDatabase.instrument instrument;
        }
        private void rankMenuItemSelect(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem t in ((ToolStripMenuItem)sender).DropDownItems)
            {
                foreach (instrumentDatabase.instrument i in cInstrumentDatabase.getInstrumentsByBank((instrumentDatabase.bank)t.Tag))
                {
                    instSelect selectionTag;
                    ToolStripMenuItem newItem = new ToolStripMenuItem();
                    newItem.Text = i.name;
                    selectionTag.rank = (int)(((ToolStripMenuItem)sender).Tag);
                    selectionTag.instrument = i;
                    newItem.Tag = selectionTag;
                    newItem.Click += instrumentSelect;
                    if (music_engine.rank_instruments[selectionTag.rank] == i)
                        newItem.Checked = true;
                    t.DropDownItems.Add(newItem);
                }
            }
        }
        private void instrumentSelect(object sender, EventArgs e)
        {
            instSelect selection = (instSelect)((ToolStripMenuItem)sender).Tag;
            music_engine.ranks[selection.rank].engine.instrument = selection.instrument;
        }

        private void instrumentDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editor = new IDatabaseEditor(cInstrumentDatabase);
            Editor.FormClosed += instrumentDatabaseEditSuccessful;
            Editor.ShowDialog();
        }

        private void instrumentDatabaseEditSuccessful(object sender, EventArgs e)
        {
            cInstrumentDatabase = Editor.iData;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currentDevice = new phMidi();
            Polyharmonia_Keys keymap_t;
            try//attempt to read default keymap - create a blank one if not found.
            {
                FileStream default_keys = new FileStream("Polyharmonia-Default.akp", FileMode.Open, FileAccess.Read);
                byte[] vs = new byte[default_keys.Length];
                default_keys.Read(vs, 0, vs.Length);
                json_dto<Polyharmonia_Keys> json_Dto = new json_dto<Polyharmonia_Keys>(UnicodeEncoding.Default.GetString(vs));
                keymap_t = json_Dto.Data;
                default_keys.Close();
            }
            catch
            {
                keymap_t = new Polyharmonia_Keys();
            }
            music_engine = new Polyharmonia_engine(currentDevice, keymap_t);
            try
            {
                FileStream default_instruments = new FileStream("GeneralMIDI.json", FileMode.Open, FileAccess.Read);
                byte[] vs = new byte[default_instruments.Length];
                default_instruments.Read(vs, 0, vs.Length);
                json_dto<instrumentDatabase> json_Dto = new json_dto<instrumentDatabase>(UnicodeEncoding.Default.GetString(vs));
                cInstrumentDatabase = json_Dto.Data;
                default_instruments.Close();
            }
            catch
            {
                cInstrumentDatabase = new instrumentDatabase();
            }
        }

        private void keysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            k_editor = new KeymapEditor(keymap);
            k_editor.FormClosed += keysEditorOperationComplete;
            k_editor.ShowDialog();
        }

        private void keysEditorOperationComplete(object sender, EventArgs e)
        {
            keymap = (Polyharmonia_Keys)k_editor.keys;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Form1.ActiveForm.KeyDown += music_engine.keyDownEvent;
            Form1.ActiveForm.KeyUp += music_engine.keyUpEvent;
            music_engine.display_callback = updateDisplay;
        }
        private void updateDisplay()
        {
            isUpdating = true;
            ChkR0K0.Checked = music_engine.ranks[0].keys[0];
            ChkR0K1.Checked = music_engine.ranks[0].keys[1];
            ChkR0K2.Checked = music_engine.ranks[0].keys[2];
            ChkR0K3.Checked = music_engine.ranks[0].keys[3];
            ChkR1K0.Checked = music_engine.ranks[1].keys[0];
            ChkR1K1.Checked = music_engine.ranks[1].keys[1];
            ChkR1K2.Checked = music_engine.ranks[1].keys[2];
            ChkR1K3.Checked = music_engine.ranks[1].keys[3];
            ChkR2K0.Checked = music_engine.ranks[2].keys[0];
            ChkR2K1.Checked = music_engine.ranks[2].keys[1];
            ChkR2K2.Checked = music_engine.ranks[2].keys[2];
            ChkR2K3.Checked = music_engine.ranks[2].keys[3];
            isUpdating = false;
            txtDecision0.Text = music_engine.ranks[0].lastDecision.ToString();
            txtDecision1.Text = music_engine.ranks[1].lastDecision.ToString();
            txtDecision2.Text = music_engine.ranks[2].lastDecision.ToString();
        }
        private void changedCheckBox(object sender, EventArgs e)
        {
            if (!isUpdating)
            {
                music_engine.ranks[0].keys[0] = ChkR0K0.Checked;
                music_engine.ranks[0].keys[1] = ChkR0K0.Checked;
                music_engine.ranks[0].keys[2] = ChkR0K0.Checked;
                music_engine.ranks[0].keys[3] = ChkR0K0.Checked;
                music_engine.ranks[1].keys[0] = ChkR0K0.Checked;
                music_engine.ranks[1].keys[1] = ChkR0K0.Checked;
                music_engine.ranks[1].keys[2] = ChkR0K0.Checked;
                music_engine.ranks[1].keys[3] = ChkR0K0.Checked;
                music_engine.ranks[2].keys[0] = ChkR0K0.Checked;
                music_engine.ranks[2].keys[1] = ChkR0K0.Checked;
                music_engine.ranks[2].keys[2] = ChkR0K0.Checked;
                music_engine.ranks[2].keys[3] = ChkR0K0.Checked;
                music_engine.ranks[0].changeKey();
                music_engine.ranks[1].changeKey();
                music_engine.ranks[2].changeKey();
            }

        }
    }
}
