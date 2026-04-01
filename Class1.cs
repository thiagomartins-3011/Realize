using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace projetofinalUC620
{
    public class PostIt : Panel
    {
        private bool dragging = false;
        private Point dragStartCursor;
        private Point dragStartControl;
        private TextBox txtConteudo;
        private DateTimePicker dtp;
        private Panel colorBox;
        private ContextMenuStrip colorMenu;
        private ContextMenuStrip menuPostIt = new ContextMenuStrip();

        public PostIt()
        {
            this.Size = new Size(180, 180);
            this.BackColor = Color.LightYellow;
            this.BorderStyle = BorderStyle.FixedSingle;

            // Data
            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Short;
            dtp.Location = new Point(10, 10);
            dtp.Size = new Size(160, 25);
            this.Controls.Add(dtp);

            // Conteúdo
            txtConteudo = new TextBox();
            txtConteudo.Multiline = true;
            txtConteudo.Location = new Point(10, 45);
            txtConteudo.Size = new Size(160, 95);
            txtConteudo.BorderStyle = BorderStyle.FixedSingle;
            txtConteudo.BackColor = Color.LightYellow;
            txtConteudo.Font = new Font("High Tower Text", 12);
            this.Controls.Add(txtConteudo);

            // Quadrado de cor / prioridade
            colorBox = new Panel();
            colorBox.Size = new Size(20, 20);
            colorBox.BackColor = this.BackColor;
            colorBox.BorderStyle = BorderStyle.FixedSingle;
            colorBox.Location = new Point(this.Width - 25, this.Height - 25);
            colorBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            colorBox.Cursor = Cursors.Hand;
            colorBox.Click += ColorBox_Click;
            this.Controls.Add(colorBox);

            // Menu de cores / prioridade
            colorMenu = new ContextMenuStrip();
            AdicionarCor("Pouco importante", Color.FromArgb(255, 255, 180));
            AdicionarCor("Importante", Color.FromArgb(173, 216, 230));
            AdicionarCor("Muito importante", Color.FromArgb(190, 230, 190));
            AdicionarCor("Importantíssimo", Color.FromArgb(255, 182, 193));

            // Eventos de arrastar
            this.MouseDown += PostIt_MouseDown;
            this.MouseMove += PostIt_MouseMove;
            this.MouseUp += PostIt_MouseUp;
            txtConteudo.MouseDown += PostIt_MouseDown;
            txtConteudo.MouseMove += PostIt_MouseMove;
            txtConteudo.MouseUp += PostIt_MouseUp;
            dtp.MouseDown += PostIt_MouseDown;
            dtp.MouseMove += PostIt_MouseMove;
            dtp.MouseUp += PostIt_MouseUp;

            // --- Menu do PostIt ---
            menuPostIt = new ContextMenuStrip();

            // Item "Concluir"
            var concluirItem = new ToolStripMenuItem("Concluir");
            concluirItem.Click += (s, e) =>
            {
                Form parentForm = this.FindForm();
                if (parentForm is Form1 form)
                {
                    form.ConcluirPostIt(this);
                }
            };
            menuPostIt.Items.Add(concluirItem);

            // Item "Apagar"
            var apagarItem = new ToolStripMenuItem("Apagar");
            apagarItem.Click += (s, e) =>
            {
                Form parentForm = this.FindForm();
                if (parentForm is Form1 form)
                {
                    form.RemoverPostIt(this);
                }
            };
            menuPostIt.Items.Add(apagarItem);

            this.ContextMenuStrip = menuPostIt;
        }

        private void ColorBox_Click(object sender, EventArgs e)
        {
            colorMenu.Show(colorBox, new Point(0, -colorMenu.Height));
        }

        private void AdicionarCor(string nome, Color cor)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(nome);
            Bitmap bmp = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp)) g.Clear(cor);
            item.Image = bmp;
            item.Tag = cor;
            item.Click += (s, e) =>
            {
                Color novaCor = (Color)((ToolStripMenuItem)s).Tag;
                this.BackColor = novaCor;
                txtConteudo.BackColor = novaCor;
                colorBox.BackColor = novaCor;
                this.Urgencia = ((ToolStripMenuItem)s).Text;
            };
            colorMenu.Items.Add(item);
        }

        public void AtualizarCor(Color cor)
        {
            this.BackColor = cor;
            txtConteudo.BackColor = cor;
            colorBox.BackColor = cor;
        }

        private void PostIt_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragStartCursor = Cursor.Position;
                dragStartControl = this.Location;
                this.BringToFront();
            }
        }

        private void PostIt_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = new Point(Cursor.Position.X - dragStartCursor.X, Cursor.Position.Y - dragStartCursor.Y);
                Point novaPos = new Point(dragStartControl.X + diff.X, dragStartControl.Y + diff.Y);

                if (this.Parent != null)
                {
                    int maxX = this.Parent.ClientSize.Width - this.Width;
                    int maxY = this.Parent.ClientSize.Height - this.Height;
                    novaPos.X = Math.Max(0, Math.Min(novaPos.X, maxX));
                    novaPos.Y = Math.Max(0, Math.Min(novaPos.Y, maxY));
                }

                this.Location = novaPos;
            }
        }
        private void PostIt_MouseUp(object sender, MouseEventArgs e) => dragging = false;

        // Propriedades
        public string Conteudo { get => txtConteudo.Text; set => txtConteudo.Text = value; }
        public DateTime Data { get => dtp.Value.Date; set => dtp.Value = value; }
        public string Urgencia { get; private set; } = "Pouco importante";
    }
}
