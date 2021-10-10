using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace ANPSupport
{
    public partial class KeymapEditor : Form
    {
        private object keys_internal;
        public object keys { get => keys_internal; }

        private List<Keys> keyBuffer;
        private bool isMultiKey;
        public KeymapEditor(object keyeditor_obj)
        {
            InitializeComponent();
            keys_internal = keyeditor_obj;
        }

        private void KeymapEditor_Load(object sender, EventArgs e)
        {
            catagory_reload();
        }
        struct fieldInfoShortName
        {
            public FieldInfo value;

            public string displayName;
            public override string ToString()
            {
                return displayName;
            }
        }
        void catagory_reload()
        {
            lstCatagories.Items.Clear();
            foreach(FieldInfo catagory in keys_internal.GetType().GetFields())
            {
                fieldInfoShortName list_item;
                list_item.value = catagory;
                list_item.displayName = catagory.Name;
                lstCatagories.Items.Add(list_item);
            }
            if (lstCatagories.Items.Count > 0)
            {
                lstCatagories.SelectedIndex = 0;
            }
        }
        private void lstKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldInfo catagoryField = ((fieldInfoShortName)lstCatagories.SelectedItem).value;
            FieldInfo keyField = ((fieldInfoShortName)lstKeys.SelectedItem).value;
            txtKey.Text = getKeyName(catagoryField, keyField);
            if(keyField.FieldType == typeof(Keys))
            {
                isMultiKey = false;
            }
            else
            {
                keyBuffer = new List<Keys>();
                isMultiKey = true;
            }
        }

        string getKeyName(FieldInfo requested_catagory, FieldInfo requested_field)
        {
            string rtn_string = "";
            if (requested_field.FieldType == typeof(Keys))
            {
                rtn_string = ((Keys)requested_field.GetValue(requested_catagory.GetValue(keys_internal))).ToString();
            }
            else
            {
                Keys[] keyArray = (Keys[])requested_field.GetValue(requested_catagory.GetValue(keys_internal));
                foreach (Keys key in keyArray)
                {
                    rtn_string += "+" + key.ToString();
                }
                if (rtn_string == "")
                {
                    rtn_string = "none";
                }
                else
                {
                    rtn_string = rtn_string.Substring(1);
                }
            }
            return rtn_string;
        }
        private void lstCatagories_SelectedIndexChanged(object sender, EventArgs e)
        {
            FieldInfo field = ((fieldInfoShortName)((ListBox)sender).SelectedItem).value;
            lstKeys.Items.Clear();
            foreach (FieldInfo member_key in field.FieldType.GetFields())
            {
                fieldInfoShortName list_item;
                if (member_key.FieldType == typeof(Keys) || member_key.FieldType == typeof(Keys[]))
                {
                    list_item.value = member_key;
                    list_item.displayName = member_key.Name + "=" + getKeyName(field, member_key);
                    lstKeys.Items.Add(list_item);
                }
            }
            if (lstKeys.Items.Count > 0)
                lstKeys.SelectedIndex = 0;
        }

        private void txtKey_Click(object sender, EventArgs e)
        {
            txtKey.Text = "Ready...";
            KeymapEditor.ActiveForm.KeyPreview = true;
        }

        private void KeymapEditor_KeyUp(object sender, KeyEventArgs e)
        {
            FieldInfo currentCatagoryMember = ((fieldInfoShortName)lstCatagories.SelectedItem).value;
            object currentCatagory = currentCatagoryMember.GetValue(keys_internal);
            FieldInfo currentFieldMember = ((fieldInfoShortName)lstKeys.SelectedItem).value;
            int current = lstKeys.SelectedIndex;
            if (currentFieldMember.FieldType == typeof(Keys))
                currentFieldMember.SetValue(currentCatagory, e.KeyCode);
            else
                currentFieldMember.SetValue(currentCatagory, keyBuffer.ToArray());
            currentCatagoryMember.SetValue(keys_internal, currentCatagory);
            KeymapEditor.ActiveForm.KeyPreview = false;
            if (current == lstKeys.Items.Count - 1)
            {
                if (lstCatagories.SelectedIndex == lstCatagories.Items.Count - 1)
                    lstCatagories.SelectedIndex = 0;
                else
                    lstCatagories.SelectedIndex++;
                current = 0;
            }
            else
            {
                lstCatagories_SelectedIndexChanged(lstCatagories, e);
                current++;
            }
            lstKeys.SelectedIndex = current;
        }

        private void KeymapEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (isMultiKey)
                keyBuffer.Add(e.KeyCode);
        }

        private void lstKeys_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                KeymapEditor.ActiveForm.KeyPreview = true;
                KeymapEditor.ActiveForm.KeyUp += KeymapEditor_keyWait;
            }

        }

        private void KeymapEditor_keyWait(object sender, KeyEventArgs e)
        {
            txtKey_Click(sender, e);
            KeymapEditor.ActiveForm.KeyDown -= KeymapEditor_keyWait;
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            keys_internal = Activator.CreateInstance(keys_internal.GetType());
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            object temporary_input_keys;
            Type target_type = keys_internal.GetType();
            Stream raw_file = openFileDialog1.OpenFile();
            byte[] raw_input = new byte[raw_file.Length];
            raw_file.Read(raw_input, 0, (int)raw_file.Length);
            string input_data = UnicodeEncoding.Default.GetString(raw_input);
            try
            {
                temporary_input_keys = JsonConvert.DeserializeObject(input_data, target_type);
                keys_internal = temporary_input_keys;
            }
            catch
            {
            }
            raw_file.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Stream raw_file = saveFileDialog1.OpenFile();
            byte[] raw_data = UnicodeEncoding.Default.GetBytes(JsonConvert.SerializeObject(keys_internal));
            raw_file.Write(raw_data, 0, raw_data.Length);
            raw_file.Close();
        }
    }
}
