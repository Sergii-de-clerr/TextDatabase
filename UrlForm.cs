using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using HtmlAgilityPack;

namespace TextDatabase
{
    public partial class UrlForm : Form
    {
        public List<string> parsedColumnName;
        public UrlForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parsedColumnName = new List<string>();
            addRowBySiteAsync(textBox1.Text);
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
                parsedColumnName.Add(title);

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
                parsedColumnName.Add(tags);

                var timeNode = document.DocumentNode.SelectSingleNode("//time[@class='published']");
                string publishedTime = timeNode != null ? timeNode.GetAttributeValue("datetime", "Час не знайдено") : "Час не знайдено";
                publishedTime = DateTime.Now.ToString();
                parsedColumnName.Add(publishedTime);

                parsedColumnName.Add(publishedTime);

                var contentNode = document.DocumentNode.SelectSingleNode("//div[@id='bodyContent']");
                string content = contentNode != null ? contentNode.InnerText.Trim() : "Текст не знайдено";
                string cleanedText = content.Replace("\r", "");
                cleanedText = cleanedText.Replace("\t", "");
                parsedColumnName.Add(cleanedText);


                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
