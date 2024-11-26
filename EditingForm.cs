using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextDatabase
{
    public partial class EditingForm : Form
    {
        public List<string> parsedcolumnname;
        public List<string> currentparsedline;
        private List<TextBox> textBoxes;
        public EditingForm()
        {
            InitializeComponent();
        }

        private void EditingForm_Load(object sender, EventArgs e)
        {
            textBoxes = new List<TextBox>();
            InitializeTextBoxes();
        }

        private void InitializeTextBoxes()
        {
            int yOffset = 10;
            int index = 0;

            for (int i = 0; i < Math.Min(currentparsedline.Count, parsedcolumnname.Count); i++)
            {
                Label labelbox = new Label
                {
                    Text = parsedcolumnname[i],
                    Width = 400,
                    Location = new System.Drawing.Point(10, yOffset)
                };

                Font currentFont = labelbox.Font;
                Font newFont = new Font(currentFont.FontFamily, 12, currentFont.Style);
                labelbox.Font = newFont;

                TextBox textBox = new TextBox
                {
                    Text = currentparsedline[i],
                    Width = 400,
                    Height = 100,
                    Location = new System.Drawing.Point(10, yOffset + 20),
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };

                currentFont = textBox.Font;
                newFont = new Font(currentFont.FontFamily, 12, currentFont.Style);
                textBox.Font = newFont;

                this.Controls.Add(labelbox);
                this.Controls.Add(textBox);
                textBoxes.Add(textBox);

                yOffset += 120;
                index++;
            }

            Button saveButton = new Button
            {
                Text = "Зберегти",
                Location = new System.Drawing.Point(10, yOffset),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);

            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(450, yOffset + 50);

            Button deleteButton = new Button
            {
                Text = "Видалити",
                Location = new System.Drawing.Point(150, yOffset),
                Width = 100
            };
            deleteButton.Click += DeleteButton_Click;
            this.Controls.Add(deleteButton);

            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(450, yOffset + 50);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                currentparsedline[i] = textBoxes[i].Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                currentparsedline[i] = textBoxes[i].Text;
            }

            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
