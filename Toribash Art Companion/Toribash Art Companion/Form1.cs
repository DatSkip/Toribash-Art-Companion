using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Toribash_Art_Companion
{
    public partial class Form1 : Form
    {
        struct tbColour
        {
            public string name, hex, index;
        }

        bool saved = true;

        List<int> colourTheme = new List<int>();

        tbColour[] colours;

        public Form1()
        {
            InitializeComponent();
            getColoursAndUpdate();
        }

        void getColoursAndUpdate()
        {
            TextReader tr = new StreamReader(@"colours.txt");
            string line;
            string[] elements;
            int i = 0;
            colours = new tbColour[int.Parse(tr.ReadLine())];
            while ((line = tr.ReadLine()) != null)
            {
                elements = line.Split('|');
                colours[i].name = elements[0];
                colours[i].index = elements[1];
                colours[i].hex = elements[2];

                colour_listBox.Items.Add(elements[0]);
                i++;
            }

            tr.Close();
        }

        private void colour_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index, r, g, b;
            double luminance;
            string hex;

            index = colour_listBox.SelectedIndex;

            hex = colours[index].hex;

            hexToRGB(hex, out r, out g, out b);

            luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;

            colour_label.BackColor = ColorTranslator.FromHtml("#" + hex);

            if (luminance > 127.5) colour_label.ForeColor = Color.Black;
            else colour_label.ForeColor = Color.White;

            if (showColourName.Checked) colour_label.Text = colours[index].name;
            else colour_label.Text = "";

            rgbOutput_textBox.Text = r + " , " + g + " , " + b;
            hexOutput_textBox.Text = hex;
            id_textBox.Text = colours[index].index;
        }



        private void showColourName_CheckedChanged(object sender, EventArgs e)
        {
            if (showColourName.Checked) colour_label.Text = colours[colour_listBox.SelectedIndex].name;
            else colour_label.Text = "";
        }

        private void nameSearch_button_Click(object sender, EventArgs e)
        {
            string searchString;
            bool found = false;

            searchString = search_textBox.Text;
            for (int i = 0; i < colours.Length; i++)
            {
                if (colours[i].name.ToLower() == searchString.ToLower())
                {
                    colour_listBox.SelectedIndex = i;
                    found = true;
                    break;
                }
                if (colours[i].index == searchString)
                {
                    colour_listBox.SelectedIndex = i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                MessageBox.Show("Colour not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void colourSearch_button_Click(object sender, EventArgs e)
        {

            DialogResult result = colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                int r = 0, g = 0, b = 0, bestIndex = -1;
                double h, s, l;
                double closeness, closestValue = double.MaxValue;

                r = colorDialog1.Color.R;
                g = colorDialog1.Color.G;
                b = colorDialog1.Color.B;

                rgbToHsl(r, g, b, out h, out s, out l);

                for (int i = 0; i < colours.Length; i++)
                {
                    int r1, b1, g1;
                    double h1, s1, l1;
                    string hex = colours[i].hex.TrimStart('#');

                    hexToRGB(hex, out r1, out g1, out b1);

                    rgbToHsl(r1, g1, b1, out h1, out s1, out l1);
                    closeness = compareColours(h, s, l, h1, s1, l1);

                    if (closeness < closestValue)
                    {
                        closestValue = closeness;
                        bestIndex = i;
                    }

                }

                colour_listBox.SelectedIndex = bestIndex;
            }

        }

        void hexToRGB(string hex, out int r, out int g, out int b)
        {
            r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }

        private void addTheme_button_Click(object sender, EventArgs e)
        {
            int i = colour_listBox.SelectedIndex;
            if (i != -1 && !colourTheme.Contains(i))
            {
                colourTheme.Add(i);
                force_comboBox.Items.Add(colours[i].name);
                relax_comboBox.Items.Add(colours[i].name);
                priGrad_comboBox.Items.Add(colours[i].name);
                secGrad_comboBox.Items.Add(colours[i].name);
                blood_comboBox.Items.Add(colours[i].name);
                grip_comboBox.Items.Add(colours[i].name);
                torso_comboBox.Items.Add(colours[i].name);
                dq_comboBox.Items.Add(colours[i].name);
                updateDrawing(theme_pictureBox);
                saved = false;
            }
        }
        void updateDrawing(PictureBox picture)
        {
            if (colourTheme.Count > 0)
            {
                for (int k = 0; k < colourTheme.Count; k++)
                {
                    int r, g, b;
                    hexToRGB(colours[colourTheme[k]].hex, out r, out g, out b);
                    SolidBrush myBrush = new SolidBrush(Color.FromArgb(255, r, g, b));
                    Graphics formGraphics;
                    formGraphics = picture.CreateGraphics();
                    formGraphics.FillRectangle(myBrush, new Rectangle(0, k * picture.Height / colourTheme.Count, picture.Width, picture.Height / colourTheme.Count));
                    myBrush.Dispose();
                    formGraphics.Dispose();
                }
            }
            else
            {
                picture.Invalidate();
            }

        }

        private void removeTheme_button_Click(object sender, EventArgs e)
        {
            if (colourTheme.Count > 0)
            {
                int lastIndex = colourTheme.Count - 1;
                colourTheme.RemoveAt(lastIndex);
                force_comboBox.Items.RemoveAt(lastIndex);
                relax_comboBox.Items.RemoveAt(lastIndex);
                priGrad_comboBox.Items.RemoveAt(lastIndex);
                secGrad_comboBox.Items.RemoveAt(lastIndex);
                blood_comboBox.Items.RemoveAt(lastIndex);
                grip_comboBox.Items.RemoveAt(lastIndex);
                torso_comboBox.Items.RemoveAt(lastIndex);
                dq_comboBox.Items.RemoveAt(lastIndex);
                updateDrawing(theme_pictureBox);
                saved = false;
            }
        }

        void rgbToHsl(int r, int g, int b, out double h, out double s, out double l)
        {
            double temp1, temp2, temp3, max, min;

            temp1 = r / 255.0;
            temp2 = g / 255.0;
            temp3 = b / 255.0;

            max = min = temp1;

            if (temp2 > temp1) max = temp2;
            else min = temp2;
            if (temp3 > max) max = temp3;
            else if (temp3 < min) min = temp3;

            l = (max + min) * 100 / 2.0;

            if ((max - min) == 0) s = 0;
            else if (l < 0.5) s = (max - min) / (max + min);
            else s = (max - min) / (2.0 - max - min);

            s *= 100;

            if ((max - min) == 0) h = 0;
            else if (temp1 == max) h = (temp2 - temp3) / (max - min);
            else if (temp2 == max) h = 2.0 + (temp3 - temp1) / (max - min);
            else h = 4.0 + (temp1 - temp2) / (max - min);

            h *= 60;
            if (h < 0) h += 360;
        }

        double compareColours(double h, double s, double l, double h1, double s1, double l1)
        {
            double closeness = 0, h_error, s_error, l_error;

            l_error = Math.Abs(l - l1);
            s_error = Math.Abs(s - s1);

            h_error = Math.Abs(h - h1);

            if (h_error > 180)
            {
                h_error = 360 - h_error;
            }

            l_error /= 2.4;
            s_error /= 2.4;
            h_error /= 1.8;

            if (s < 15 && s1 < 15) closeness = l_error;
            closeness = (l_error + s_error + h_error) / 3;

            return closeness;
        }

        private void create_button_Click(object sender, EventArgs e)
        {

            saveFileDialog1.FileName = "item.dat";
            saveFileDialog1.Filter = "Data File (*.dat)|*.dat|All files(*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string force, relax, priGrad, secGrad, blood, grip, torso, dq, output;

                force = relax = priGrad = secGrad = blood = grip = torso = dq = "0";

                if (force_comboBox.SelectedIndex != -1) force = colours[colourTheme[force_comboBox.SelectedIndex]].index;
                if (relax_comboBox.SelectedIndex != -1) relax = colours[colourTheme[relax_comboBox.SelectedIndex]].index;
                if (priGrad_comboBox.SelectedIndex != -1) priGrad = colours[colourTheme[priGrad_comboBox.SelectedIndex]].index;
                if (secGrad_comboBox.SelectedIndex != -1) secGrad = colours[colourTheme[secGrad_comboBox.SelectedIndex]].index;
                if (blood_comboBox.SelectedIndex != -1) blood = colours[colourTheme[blood_comboBox.SelectedIndex]].index;
                if (grip_comboBox.SelectedIndex != -1) grip = colours[colourTheme[grip_comboBox.SelectedIndex]].index;
                if (torso_comboBox.SelectedIndex != -1) torso = colours[colourTheme[torso_comboBox.SelectedIndex]].index;
                if (dq_comboBox.SelectedIndex != -1) dq = colours[colourTheme[dq_comboBox.SelectedIndex]].index;

                output = "TEXT 0;0 0 0 0 0 0\nITEM 0;" + blood + " " + force + " " + relax + " " + force + " " + dq + " 0 0 0 0 0 0 0 0 0 " + grip + " 0 0 0 0 0 0\nBODCOL 0;0 0 1 " + torso + " 2 " + torso + " 3 0 4 0 5 " + torso + " 6 0 7 0 8 " + torso + " 9 0 10 0 11 0 12 0 13 0 14 0 15 0 16 0 17 0 18 0 19 0 20 0\n" +
                         repeat("GRADCOL1 0;0", priGrad) +
                         repeat("GRADCOL2 0;0", secGrad) +
                         repeat("FORCOL 0;0", force) +
                         repeat("RELCOL 0;0", relax) +
                         repeat("REPCOL 0;0", force) +
                         "BELTNAME 0;\nTRAILCOL 0;0 0 1 0 2 0 3 0\nTEXBODY 0; 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0\nBMAPBODY 0; 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 0 0\n" +
                         repeat("TEXJOINT 0; ", "0") +
                         repeat("BMAPJOINT 0; ", "0") +
                         "TEXTRAIL 0; 0 0 0 0\nTEXGROUND 0; 0";

                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                sw.Write(output);
                sw.Close();
            }

        }

        string repeat(string start, string filler)
        {
            string final = start;

            final += " " + filler;
            for (int i = 1; i <= 20; i++)
            {
                final += " " + i + " " + filler;
            }
            final += "\n";
            return final;
        }

        private void clearTheme_button_Click(object sender, EventArgs e)
        {
            colourTheme.Clear();
            force_comboBox.Items.Clear();
            relax_comboBox.Items.Clear();
            priGrad_comboBox.Items.Clear();
            secGrad_comboBox.Items.Clear();
            blood_comboBox.Items.Clear();
            grip_comboBox.Items.Clear();
            torso_comboBox.Items.Clear();
            dq_comboBox.Items.Clear();
            updateDrawing(theme_pictureBox);
            saved = true;
            clear_button.PerformClick();
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            force_comboBox.SelectedIndex = -1;
            relax_comboBox.SelectedIndex = -1;
            priGrad_comboBox.SelectedIndex = -1;
            secGrad_comboBox.SelectedIndex = -1;
            blood_comboBox.SelectedIndex = -1;
            grip_comboBox.SelectedIndex = -1;
            torso_comboBox.SelectedIndex = -1;
            dq_comboBox.SelectedIndex = -1;
            force_comboBox.Text = "";
            relax_comboBox.Text = "";
            priGrad_comboBox.Text = "";
            secGrad_comboBox.Text = "";
            blood_comboBox.Text = "";
            grip_comboBox.Text = "";
            torso_comboBox.Text = "";
            dq_comboBox.Text = "";
            saved = false;
        }


        private void exit_button_Click(object sender, EventArgs e)
        {
            if (saved) Close();
            else
            {
                DialogResult result;

                result = MessageBox.Show("Changes have been made. Do you want to save?", "Notice", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    save_button.PerformClick();
                    Close();
                }
                else if (result == DialogResult.No)
                {
                    Close();
                }
            }
        }

        private void reset_button_Click(object sender, EventArgs e)
        {
            clearTheme_button.PerformClick();
            clear_button.PerformClick();
            saved = true;
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            string output = "";

            foreach (int index in colourTheme)
            {
                output += index + " ";
            }

            output += "|" + force_comboBox.SelectedIndex + " " + relax_comboBox.SelectedIndex + " " + priGrad_comboBox.SelectedIndex + " " + secGrad_comboBox.SelectedIndex + " " + blood_comboBox.SelectedIndex + " " + grip_comboBox.SelectedIndex + " " + torso_comboBox.SelectedIndex + " " + dq_comboBox.SelectedIndex;

            saveFileDialog1.FileName = "save.txt";
            saveFileDialog1.Filter = "Text File (*.txt)|*.txt|All files(*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TextWriter sw = new StreamWriter(saveFileDialog1.FileName);
                sw.Write(output);
                sw.Close();
                saved = true;
            }
        }

        private void load_button_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "save.txt";
            openFileDialog1.Filter = "Text File (*.txt)|*.txt|All files(*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    string input = sr.ReadLine();
                    string[] elements = input.Split('|');
                    string[] theme = elements[0].Split(' ');
                    string[] custom = elements[1].Split(' ');

                    foreach (string index in theme)
                    {
                        if (index != "")
                        {
                            colour_listBox.SelectedIndex = int.Parse(index);
                            addTheme_button.PerformClick();
                        }
                    }

                    force_comboBox.SelectedIndex = int.Parse(custom[0]);
                    relax_comboBox.SelectedIndex = int.Parse(custom[1]);
                    priGrad_comboBox.SelectedIndex = int.Parse(custom[2]);
                    secGrad_comboBox.SelectedIndex = int.Parse(custom[3]);
                    blood_comboBox.SelectedIndex = int.Parse(custom[4]);
                    grip_comboBox.SelectedIndex = int.Parse(custom[5]);
                    torso_comboBox.SelectedIndex = int.Parse(custom[6]);
                    dq_comboBox.SelectedIndex = int.Parse(custom[7]);
                    sr.Close();
                    saved = true;
                }
                catch
                {
                    MessageBox.Show("Error occured while loading file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void theme_pictureBox_Click(object sender, EventArgs e)
        {
            if (colourTheme.Count > 0)
            {
                int y;

                y = theme_pictureBox.PointToClient(Cursor.Position).Y;

                for (int i = 0; i < colourTheme.Count; i++)
                {
                    if (y <= theme_pictureBox.Height / colourTheme.Count)
                    {
                        colour_listBox.SelectedIndex = colourTheme[i];
                        break;
                    }
                    y -= theme_pictureBox.Height / colourTheme.Count;
                }
            }
        }

        private void dq_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            saved = false;
        }
    }
}
