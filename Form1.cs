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
using System.Xml;

namespace projetofinalUC620
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CarregarPostIts();
            CarregarNomeUsuario();
        }

        List<PostIt> postIts = new List<PostIt>();

        int posicao = 0;
        private void btn_adicionar_Click(object sender, EventArgs e)
        {
            PostIt novo = new PostIt();

            int centroX = ((panel_postits.ClientSize.Width - novo.Width) / 2 + posicao);
            int centroY = ((panel_postits.ClientSize.Height - novo.Height) / 2 + posicao);
            novo.Location = new Point(centroX, centroY);

            panel_postits.Controls.Add(novo);
            postIts.Add(novo);

            novo.Disposed += (s, ev) => postIts.Remove(novo);
            posicao += 10;
        }

        public void SalvarPostIts()
        {
            using (XmlWriter writer = XmlWriter.Create("postits.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("PostIts");

                foreach (PostIt p in postIts)
                {
                    writer.WriteStartElement("PostIt");

                    writer.WriteElementString("Conteudo", p.Conteudo);
                    writer.WriteElementString("Data", p.Data.ToString("yyyy-MM-dd"));
                    writer.WriteElementString("Cor", p.BackColor.ToArgb().ToString());
                    writer.WriteElementString("X", p.Location.X.ToString());
                    writer.WriteElementString("Y", p.Location.Y.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        private void CarregarPostIts()
        {
            string caminho = "postits.xml";

            // Se o ficheiro não existe ou está vazio, sai da função
            if (!File.Exists(caminho) || new FileInfo(caminho).Length == 0)
                return;

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(caminho);
            }
            catch
            {
                // Se o ficheiro estiver malformado, apaga e ignora
                File.Delete(caminho);
                return;
            }

            foreach (XmlNode node in doc.SelectNodes("//PostIt"))
            {
                PostIt novo = new PostIt();
                novo.Conteudo = node["Conteudo"].InnerText;
                novo.Data = DateTime.Parse(node["Data"].InnerText);
                novo.AtualizarCor(Color.FromArgb(int.Parse(node["Cor"].InnerText)));
                novo.Location = new Point(
                    int.Parse(node["X"].InnerText),
                    int.Parse(node["Y"].InnerText)
                );

                panel_postits.Controls.Add(novo);
                postIts.Add(novo);
            }
        }

        // Salva um post-it concluído no XML "postits_concluidos.xml"
        public void SalvarPostItConcluido(PostIt p)
        {
            string caminho = "postits_concluidos.xml";
            XmlDocument doc = new XmlDocument();

            if (File.Exists(caminho) && new FileInfo(caminho).Length > 0)
            {
                try { doc.Load(caminho); }
                catch
                {
                    doc = new XmlDocument();
                    XmlElement root = doc.CreateElement("PostItsConcluidos");
                    doc.AppendChild(root);
                }
            }
            else
            {
                XmlElement root = doc.CreateElement("PostItsConcluidos");
                doc.AppendChild(root);
            }

            XmlElement postItNode = doc.CreateElement("PostIt");
            postItNode.AppendChild(CriarElemento(doc, "Conteudo", p.Conteudo));
            postItNode.AppendChild(CriarElemento(doc, "Data", p.Data.ToString("yyyy-MM-dd")));
            postItNode.AppendChild(CriarElemento(doc, "Urgencia", p.Urgencia));

            doc.DocumentElement.AppendChild(postItNode);
            doc.Save(caminho);
        }

        // Remove do painel e da lista, atualiza XML
        public void RemoverPostIt(PostIt p)
        {
            if (panel_postits.Controls.Contains(p))
                panel_postits.Controls.Remove(p);

            if (postIts.Contains(p))
                postIts.Remove(p);

            SalvarPostIts();
        }

        // Concluir um post-it
        public void ConcluirPostIt(PostIt p)
        {
            SalvarPostItConcluido(p);
            RemoverPostIt(p);
        }

        // Função auxiliar para criar elementos XML
        private XmlElement CriarElemento(XmlDocument doc, string nome, string valor)
        {
            XmlElement el = doc.CreateElement(nome);
            el.InnerText = valor;
            return el;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            SalvarPostIts();
            SalvarNomeUsuario();
        }

        private void btn_mostrar_Click(object sender, EventArgs e)
        {
            FormPostItsConcluidos frm = new FormPostItsConcluidos();
            frm.ShowDialog();
        }
        private void SalvarNomeUsuario()
        {
            string caminho = "usuario.txt";
            File.WriteAllText(caminho, tb_usuario.Text);
        }
        private void CarregarNomeUsuario()
        {
            string caminho = "usuario.txt";
            if (File.Exists(caminho))
            {
                tb_usuario.Text = File.ReadAllText(caminho);
            }
        }
    }
}

