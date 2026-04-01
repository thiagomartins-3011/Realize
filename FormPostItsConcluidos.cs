using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace projetofinalUC620
{
    public partial class FormPostItsConcluidos : Form
    {
        private ListView lvConcluidos;

        public FormPostItsConcluidos()
        {
            this.Text = "Post-its Concluídos";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            lvConcluidos = new ListView();
            lvConcluidos.Dock = DockStyle.Fill;
            lvConcluidos.View = View.Details;
            lvConcluidos.FullRowSelect = true;
            lvConcluidos.GridLines = true;

            lvConcluidos.Columns.Add("Conteúdo", 250);
            lvConcluidos.Columns.Add("Data", 100);
            lvConcluidos.Columns.Add("Urgência", 120);

            this.Controls.Add(lvConcluidos);

            CarregarPostItsConcluidos();
        }

        private void CarregarPostItsConcluidos()
        {
            string caminho = "postits_concluidos.xml";
            if (!File.Exists(caminho)) return;

            XmlDocument doc = new XmlDocument();
            doc.Load(caminho);

            foreach (XmlNode node in doc.SelectNodes("//PostIt"))
            {
                string conteudo = node["Conteudo"]?.InnerText ?? "(sem texto)";
                string data = node["Data"]?.InnerText ?? "(sem data)";
                string urgencia = node["Urgencia"]?.InnerText ?? "(sem urgência)";

                ListViewItem item = new ListViewItem(conteudo);
                item.SubItems.Add(data);
                item.SubItems.Add(urgencia);
                lvConcluidos.Items.Add(item);
            }
        }
    }
}
