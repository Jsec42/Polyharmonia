using ANPSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polyharmonia
{
    public partial class IDatabaseEditor : Form
    {
        public instrumentDatabase iData;
        public IDatabaseEditor(instrumentDatabase cIData)
        {
            iData = cIData;
            InitializeComponent();
        }

        private void lstInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstInstruments.SelectedIndex == -1)
                lstInstruments.SelectedIndex = lstInstruments.Items.Count - 1;
            if(lstInstruments.SelectedItem.GetType() == typeof(string))
            {
                txtIName.Text = "";
                if (lstInstruments.Items.Count <= 128 && lstInstruments.Items.Count>1)
                    numIIndex.Value = lstInstruments.Items.Count-1;
                btnAddInst.Text = "Add Instrument";
            }
            else
            {
                instrumentDatabase.instrument current = (instrumentDatabase.instrument)lstInstruments.SelectedItem;

                txtIName.Text = current.name;
                txtBName.Text = current.bankInformation.name;
                numBIndex.Value = current.bankInformation.id;
                if (lstBankSelect.Items.Contains(current.bankInformation))
                {
                    btnAddBank.Text = "Edit Bank";
                    lstBankSelect.SelectedIndex = lstBankSelect.Items.IndexOf(current.bankInformation);
                }
                else
                {
                    lstBankSelect.SelectedIndex = -1;
                    txtBName.Text = current.bankInformation.name;
                    numBIndex.Value = current.bankInformation.id;
                }
                numIIndex.Value = current.id;
                btnAddInst.Text = "Edit Instrument";
            }
        }

        private void lstBankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBankSelect.SelectedIndex == -1)
                lstBankSelect.SelectedIndex = lstBankSelect.Items.Count - 1;
            if (lstBankSelect.SelectedItem.GetType() == typeof(string))
            {
                txtBName.Text = "";
                if(lstBankSelect.Items.Count - 1 < numBIndex.Maximum && lstBankSelect.Items.Count > 1)
                    numBIndex.Value = lstBankSelect.Items.Count-1;
                btnAddBank.Text = "Add Bank";
            }
            else
            {
                instrumentDatabase.bank current = (instrumentDatabase.bank)lstBankSelect.SelectedItem;
                txtBName.Text = current.name;
                numBIndex.Value = current.id;
                btnAddBank.Text = "Edit Bank";
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            int index = lstInstruments.SelectedIndex - 1;
            if (index >= 0 && index < lstInstruments.Items.Count-1)
            {
                instrument_swap(index);
            }
        }
        private void instrument_swap(int index, int b_index = -1)
        {
            instrumentDatabase.instrument a, b;
            int i_original = index;
            if (b_index == -1)
                b_index = index + 1;
            a = (instrumentDatabase.instrument)lstInstruments.Items[index];
            b = (instrumentDatabase.instrument)lstInstruments.Items[b_index];
            (a.id, b.id) = (b.id, a.id);
            if (index > b_index)
            {
                (a, b) = (b, a);
                (index, b_index) = (b_index, index);
            }
            lstInstruments.Items.RemoveAt(b_index);
            lstInstruments.Items.RemoveAt(index);
            lstInstruments.Items.Insert(index, b);
            lstInstruments.Items.Insert(b_index, a);
            lstInstruments.SelectedIndex = i_original;
        }
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            int index = lstInstruments.SelectedIndex;
            if (index >= 0 && index < lstInstruments.Items.Count-2)
            {
                instrument_swap(index);
                lstInstruments.SelectedIndex++;
            }
        }

        private void btnAddBank_Click(object sender, EventArgs e)
        {
            instrumentDatabase.bank newBank = new instrumentDatabase.bank((int)numBIndex.Value, txtBName.Text);
            int i = lstBankSelect.SelectedIndex;
            if (lstBankSelect.SelectedItem.GetType() == typeof(instrumentDatabase.instrument))
            {
                instrumentDatabase.bank oldBank = (instrumentDatabase.bank)lstBankSelect.SelectedItem;
                bankModified(oldBank, newBank);
                lstBankSelect.Items.RemoveAt(i);
            }
            lstBankSelect.Items.Insert(i, newBank);
            lstBankSelect.SelectedIndex = i+1;
            if(numBIndex.Value!=numBIndex.Maximum)
                numBIndex.Value += 1;
            txtBName.SelectAll();
        }

        private void btnAddInst_Click(object sender, EventArgs e)
        {
            instrumentDatabase.instrument newInstrument = new instrumentDatabase.instrument((int)numIIndex.Value, txtIName.Text);
            int i = lstInstruments.SelectedIndex;
            newInstrument.bankInformation = new instrumentDatabase.bank((int)numBIndex.Value, txtBName.Text);
            lstInstruments.Items.Insert(i, newInstrument);
            if (lstInstruments.SelectedItem.GetType() == newInstrument.GetType())
            {
                lstInstruments.Items.RemoveAt(i+1);
            }
            if (!lstBankSelect.Items.Contains(newInstrument.bankInformation))
            {
                lstBankSelect.SelectedIndex = -1;
                btnAddBank_Click(sender, e);
                lstBankSelect.SelectedIndex--;
            }
            if(numIIndex.Value!= numIIndex.Maximum)
                numIIndex.Value += 1;
            txtIName.SelectAll();
            lstInstruments.SelectedIndex = i+1;
            if (lstBankSelect.Focused)
            {
                txtIName.Focus();
            }
        }

        private void btnBMoveDown_Click(object sender, EventArgs e)
        {
            int index = lstBankSelect.SelectedIndex;
            if (index >= 0 && index < lstBankSelect.Items.Count - 2)
            {
                bank_swap(index);
                lstBankSelect.SelectedIndex++;
            }
        }
        private void bank_swap(int index, int b_index = -1)
        {
            instrumentDatabase.bank a, b, aa, bb;
            int i_original = index;
            if (b_index == -1)
                b_index = index + 1;
            a = (instrumentDatabase.bank)lstBankSelect.Items[index];
            b = (instrumentDatabase.bank)lstBankSelect.Items[b_index];
            aa = new instrumentDatabase.bank(a);
            bb = new instrumentDatabase.bank(b);
            (a.id, b.id) = (b.id, a.id);
            if (index > b_index)
            {
                (a, b) = (b, a);
                (index, b_index) = (b_index, index);
            }
            lstBankSelect.Items.RemoveAt(b_index);
            lstBankSelect.Items.RemoveAt(index);
            lstBankSelect.Items.Insert(index, b);
            lstBankSelect.Items.Insert(b_index, a);
            bankModified(aa, a);
            bankModified(bb, b);
            lstBankSelect.SelectedIndex = i_original;
        }
        private void bankModified(instrumentDatabase.bank oldBank, instrumentDatabase.bank newBank)
        {
            int selected = lstInstruments.SelectedIndex;
            for (int i= 0; i < lstInstruments.Items.Count; i++)
            {
                object target = lstInstruments.Items[i];
                if (target.GetType() == typeof(instrumentDatabase.instrument))
                {
                    instrumentDatabase.instrument this_instrument = (instrumentDatabase.instrument)target;
                    if(this_instrument.bankInformation == oldBank)
                    {
                        this_instrument.bankInformation = newBank;
                        lstInstruments.Items.Insert(i, this_instrument);
                        lstInstruments.Items.RemoveAt(i);
                    }
                }
            }
            lstInstruments.SelectedIndex = selected;
        }

        private void btnBMoveUp_Click(object sender, EventArgs e)
        {
            int index = lstBankSelect.SelectedIndex - 1;
            if (index >= 0 && index < lstBankSelect.Items.Count - 1)
            {
                bank_swap(index);
            }
        }
        //Clears iData and re-initializes blank list boxes for new database
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iData = new instrumentDatabase();
            uiInit();
        }
        //show open file dialog
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        //function which initializes all list boxes
        private void uiInit(bool isNew = true)
        {
            lstBankSelect.Items.Clear();
            lstInstruments.Items.Clear();
            if (!isNew)//only updates list boxes with existing information if a new database is actually desired(i.e. file or host import)
            {
                lstBankSelect.Items.AddRange(iData.banks.ToArray());
                lstInstruments.Items.AddRange(iData.instruments.ToArray());
            }
            lstBankSelect.Items.Add("(new)");
            lstInstruments.Items.Add("(new)");
            lstBankSelect.SelectedIndex = 0;
            lstInstruments.SelectedIndex = 0;
        }
        //loads file selected in open file dialog into iData and updates list boxes accordingly
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Stream stream = openFileDialog1.OpenFile();
            byte[] inputData = new byte[stream.Length];
            stream.Read(inputData, 0, (int)stream.Length);
            json_dto<instrumentDatabase> input_processor = new json_dto<instrumentDatabase>(UnicodeEncoding.Default.GetString(inputData));
            iData = input_processor.Data;
            uiInit(false);
            stream.Close();
            saveFileDialog1.FileName = openFileDialog1.FileName;
        }
        //loads save file dialog
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }
        //Updates iData and dumps current configuration to file
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Stream stream = saveFileDialog1.OpenFile();
            saveInstData();
            json_dto<instrumentDatabase> json_Dto = new json_dto<instrumentDatabase>(iData);
            byte []data = UnicodeEncoding.Default.GetBytes(json_Dto.Json_string);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }
        //Saves data currently stored in list boxes to iData
        private void saveInstData()
        {
            iData.banks.Clear();
            iData.instruments.Clear();
            foreach (object instrument in lstInstruments.Items)
            {
                if(instrument.GetType() == typeof(instrumentDatabase.instrument))
                    iData.instruments.Add((instrumentDatabase.instrument)instrument);
            }
            foreach (object bank in lstBankSelect.Items)
            {
                if(bank.GetType() == typeof(instrumentDatabase.bank))
                    iData.banks.Add((instrumentDatabase.bank)bank);
            }
        }
        //loads initial values into list boxes from existing iData from calling object
        private void IDatabaseEditor_Load(object sender, EventArgs e)
        {
            uiInit(false);
        }
        //Originally on-click functions - executes upon text boxes gaining focus - adjusts default button target to appropriate add/edit button
        private void txtBName_Click(object sender, EventArgs e)
        {
            if (!txtBName.Focused)
            {
                IDatabaseEditor.ActiveForm.AcceptButton = btnAddBank;
                txtBName.SelectAll();

            }
        }

        private void txtIName_Click(object sender, EventArgs e)
        {
            if (!txtIName.Focused)
            {
                IDatabaseEditor.ActiveForm.AcceptButton = btnAddInst;
                txtIName.SelectAll();
            }
        }
        //Updates global Instrument Database
        private void IDatabaseEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveInstData();
        }
        //Functionality for delete buttons
        private void btnDeleteInstrument_Click(object sender, EventArgs e)
        {
            int i = lstInstruments.SelectedIndex;
            lstInstruments.Items.RemoveAt(i);
            lstInstruments.SelectedIndex = i;
        }

        private void btnDeleteBank_Click(object sender, EventArgs e)
        {
            int i = lstBankSelect.SelectedIndex;
            lstBankSelect.Items.RemoveAt(i);
            lstBankSelect.SelectedIndex = i;
        }
        //Allows for deleting from list box using keyboard controls only
        private void ILstDeleteDispatch(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnDeleteInstrument_Click(sender, e);
        }
        private void BLstDeleteDispatch(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnDeleteBank_Click(sender, e);
        }
        //Dictates list box item swap behavior(keyboard only!)
        private void lstInstruments_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Shift
                && lstInstruments.SelectedItem.GetType() == typeof(instrumentDatabase.instrument))
            {
                lstInstruments.Tag = lstInstruments.SelectedIndex;
            }
        }
        private void lstInstruments_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift
                && lstInstruments.Tag != null
                && lstInstruments.SelectedIndex != lstInstruments.Items.Count - 1
                && !lstInstruments.Tag.Equals(lstInstruments.SelectedIndex))
            {
                bank_swap(lstInstruments.SelectedIndex, (int)lstInstruments.Tag);
                lstInstruments_SelectedIndexChanged(sender, e);
                lstInstruments_Leave(sender, e);
            }
        }

        private void lstInstruments_Leave(object sender, EventArgs e)
        {
            lstInstruments.Tag = null;
            lstInstruments.SelectedIndexChanged += lstInstruments_SelectedIndexChanged;
        }

        private void lstBankSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift
                && lstBankSelect.SelectedItem.GetType() == typeof(instrumentDatabase.bank))
            {
                lstBankSelect.Tag = lstBankSelect.SelectedIndex;
            }
        }
        private void lstBankSelect_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift
                && lstBankSelect.Tag != null
                && lstBankSelect.SelectedIndex != lstBankSelect.Items.Count - 1
                && !lstBankSelect.Tag.Equals(lstBankSelect.SelectedIndex))
            {
                bank_swap(lstBankSelect.SelectedIndex, (int)lstBankSelect.Tag);
                lstBankSelect_SelectedIndexChanged(sender, e);
                lstBankSelect_Leave(sender, e);
            }

        }
        private void lstBankSelect_Leave(object sender, EventArgs e)
        {
            lstBankSelect.Tag = null;
            lstBankSelect.SelectedIndexChanged += lstBankSelect_SelectedIndexChanged;
        }
    }
}
