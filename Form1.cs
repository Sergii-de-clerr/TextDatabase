using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextDatabase
{
    public partial class Form1 : Form
    {
        List<List<string>> parsed_Data;
        List<string> parsedColumnName;
        List<int> currentparsednumbers;
        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
        }

        private void InitializeListBox(string main_text)
        {
            string[] lines = main_text.Split(new[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);

            parsed_Data = new List<List<string>>();
            parsedColumnName = new List<string>();
            currentparsednumbers = new List<int>();
            listBox1.Items.Clear();

            bool is_first = false;

            foreach (string line in lines)
            {
                if (is_first == false)
                {
                    string[] titletheses = line.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries);

                    List<string> titlethesesList = new List<string>();
                    foreach (string thesis in titletheses)
                    {
                        parsedColumnName.Add(thesis.Trim());
                    }
                    is_first = true;
                }
                else
                {
                    string[] theses = line.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Trim() == "")
                    {
                        continue;
                    }
                    List<string> thesesList = new List<string>();
                    foreach (string thesis in theses)
                    {
                        thesesList.Add(thesis.Trim());
                    }
                    parsed_Data.Add(thesesList);
                }
            }
            for (int i = 0; i < parsed_Data.Count; i++)
            {
                currentparsednumbers.Add(i);
                if (parsed_Data[currentparsednumbers[i]].Count > 0)
                {
                    listBox1.Items.Add(parsed_Data[currentparsednumbers[i]][0]);
                }
                else
                {
                    listBox1.Items.Add("NAN");
                }
            }
            listBox1.SelectedIndexChanged += ListBoxItems_SelectedIndexChanged;
            listBox1.MouseDoubleClick += ListBoxItems_MouseDoubleClick;
        }

        private void ListBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            //panel1.Location = new System.Drawing.Point(10, 10);
            //panel1.Size = new System.Drawing.Size(300, 200);
            //panel1.AutoScroll = true;
            int selectedIndex = listBox1.SelectedIndex;
            label1.Text = "";

            if (selectedIndex != -1)
            {
                for (int i = 0; i < Math.Min(parsedColumnName.Count, parsed_Data[currentparsednumbers[selectedIndex]].Count); i++)
                {
                    label1.Text += $"{parsedColumnName[i]}:\n{parsed_Data[currentparsednumbers[selectedIndex]][i]}\n\n";
                }
            }
        }

        private void ListBoxItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (index != -1)
            {
                EditingForm editForm = new EditingForm
                {
                    parsedcolumnname = parsedColumnName,
                    currentparsedline = parsed_Data[currentparsednumbers[index]]
                };

                var resdialog = editForm.ShowDialog();

                if (resdialog == DialogResult.OK)
                {
                    parsed_Data[currentparsednumbers[index]] = editForm.currentparsedline;
                    listBox1.Items[index] = editForm.currentparsedline[0];
                }

                if (resdialog == DialogResult.No)
                {
                    parsed_Data.RemoveAt(currentparsednumbers[index]);
                    currentparsednumbers.RemoveAt(index);
                    listBox1.Items.RemoveAt(index);
                }
            }
        }

        private void uploadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Виберіть файл",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    InitializeListBox(fileContent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталася помилка під час відкриття файлу:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV файли (*.csv)|*.csv";
                saveFileDialog.Title = "Зберегти таблицю як CSV";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveDataToCSV(saveFileDialog.FileName);
                }
            }
        }

        private void SaveDataToCSV(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    for (int i = 0; i < parsedColumnName.Count; i++)
                    {
                        writer.Write(parsedColumnName[i]);
                        if (i < parsedColumnName.Count - 1)
                            writer.Write("^");
                    }
                    writer.WriteLine();

                    foreach (int rowindex in currentparsednumbers)
                    {
                        for (int i = 0; i < parsed_Data[rowindex].Count; i++)
                        {
                            var value = parsed_Data[rowindex][i]?.ToString() ?? "";
                            writer.Write(value.Replace("^", "\\^"));
                            if (i < parsed_Data[rowindex].Count - 1)
                                writer.Write("^");
                        }
                        writer.WriteLine();
                    }
                }

                MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні файлу:\n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (index != -1)
            {
                List<string> fakedata = new List<string>();
                foreach (var item in parsedColumnName)
                {
                    fakedata.Add("");
                }
                EditingForm editForm = new EditingForm
                {
                    parsedcolumnname = parsedColumnName,
                    currentparsedline = fakedata
                };

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    currentparsednumbers.Add(parsed_Data.Count);
                    parsed_Data.Add(editForm.currentparsedline);
                    if (editForm.currentparsedline.Count > 0)
                    {
                        listBox1.Items.Add(editForm.currentparsedline[0]);
                    }
                    else
                    {
                        listBox1.Items.Add("NAN");
                    }
                }
            }
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (index != -1)
            {
                List<string> fakedata = new List<string>();
                foreach (var item in parsedColumnName)
                {
                    fakedata.Add("");
                }
                EditingForm editForm = new EditingForm
                {
                    parsedcolumnname = parsedColumnName,
                    currentparsedline = fakedata
                };


                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    List<string> filter = editForm.currentparsedline;
                    currentparsednumbers.Clear();
                    listBox1.Items.Clear();
                    for (int i = 0; i < parsed_Data.Count; i++)
                    {
                        bool is_same = true;
                        for (int j = 0; j < Math.Min(filter.Count, parsed_Data[i].Count); j++)
                        {
                            if (!parsed_Data[i][j].Contains(filter[j]))
                            {
                                is_same = false;
                                break;
                            }
                        }
                        if (is_same == true)
                        {
                            currentparsednumbers.Add(i);
                        }
                    }
                    foreach (int num in currentparsednumbers)
                    {
                        if (parsed_Data[num].Count > 0)
                        {
                            listBox1.Items.Add(parsed_Data[num][0]);
                        }
                        else
                        {
                            listBox1.Items.Add("NAN");
                        }
                    }
                }
            }
        }

        private void deleteFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentparsednumbers.Clear();
            listBox1.Items.Clear();
            for (int i = 0; i < parsed_Data.Count; i++)
            {
                currentparsednumbers.Add(i);
                if (parsed_Data[currentparsednumbers[i]].Count > 0)
                {
                    listBox1.Items.Add(parsed_Data[currentparsednumbers[i]][0]);
                }
                else
                {
                    listBox1.Items.Add("NAN");
                }
            }
        }

        private void addRowBySiteToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            UrlForm urlForm = new UrlForm();

            if (urlForm.ShowDialog() == DialogResult.OK)
            {
                currentparsednumbers.Add(parsed_Data.Count);
                parsed_Data.Add(urlForm.parsedColumnName);
                if (urlForm.parsedColumnName.Count > 0)
                {
                    listBox1.Items.Add(urlForm.parsedColumnName[0]);
                }
                else
                {
                    listBox1.Items.Add("NAN");
                }
            }
            //addRowBySiteAsync("https://en.wikipedia.org/wiki/William_Shakespeare");
        }
        private async Task addRowBySiteAsync(string siteurl)
        {
            string url = siteurl;

            using (HttpClient client = new HttpClient())
            {
                string html = await client.GetStringAsync(url);

                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);

                var titleNode = document.DocumentNode.SelectSingleNode("//h1[@id='firstHeading']");
                string title = titleNode != null ? titleNode.InnerText : "Заголовок не знайдено";

                var categories = document.DocumentNode.SelectNodes("//div[@id='mw-normal-catlinks']//ul//li");
                string tags = "";
                if (categories != null)
                {
                    foreach (var category in categories)
                    {
                        string categoryName = category.InnerText;
                        string categoryLink = category.SelectSingleNode(".//a").Attributes["href"].Value;
                        tags += categoryName;
                        tags += ", ";
                        Console.WriteLine($"- {categoryName} ({categoryLink})");
                    }
                }

                var timeNode = document.DocumentNode.SelectSingleNode("//time[@class='published']");
                string publishedTime = timeNode != null ? timeNode.GetAttributeValue("datetime", "Час не знайдено") : "Час не знайдено";
                publishedTime = DateTime.Now.ToString();

                var contentNode = document.DocumentNode.SelectSingleNode("//div[@id='bodyContent']");
                string content = contentNode != null ? contentNode.InnerText.Trim() : "Текст не знайдено";
                content = DateTime.Now.ToString();
            }
        }
    }
}
