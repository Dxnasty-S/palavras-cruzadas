using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace palavras_cruzadas
{
    public partial class Form1 : Form
    {
        Dicas dicas = new Dicas();

        public string arquivoDeDicas = Application.StartupPath + "\\Palavras\\Palavras_1.Dxt";

        List<Palavra> palavras = new List<Palavra>();

        private void construirListaDePalavras()
        {
            string linha = "";

            using (StreamReader leitorDeLinhas = new StreamReader(arquivoDeDicas))
            {
                linha = leitorDeLinhas.ReadLine();
                while ((linha = leitorDeLinhas.ReadLine()) != null)
                {
                    string[] colunasSeparadas = linha.Split('|');

                    palavras.Add(new Palavra(int.Parse(colunasSeparadas[0]),
                        int.Parse(colunasSeparadas[1]),
                                  colunasSeparadas[2],
                                  colunasSeparadas[3],
                                  colunasSeparadas[4],
                                  colunasSeparadas[5]));

                    dicas.tabelaDicas.Rows.Add(new string[] { colunasSeparadas[3],
                                                              colunasSeparadas[2],
                                                              colunasSeparadas[5] });




                }
            }
        }
        public Form1()
        {
            construirListaDePalavras();
            InitializeComponent();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Palavras cruzadas", "Created By Marco Anthony");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inicializaTabuleiro();
            dicas.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
            dicas.StartPosition = FormStartPosition.Manual;
            dicas.Show();
            dicas.tabelaDicas.AutoResizeColumns();
        }

        private void inicializaTabuleiro()
        {
            Tabuleiro.BackgroundColor = Color.Black;
            Tabuleiro.DefaultCellStyle.BackColor = Color.Black;

            for (int i = 0; i < 22; i++)
            {
                Tabuleiro.Rows.Add();
            }

            foreach(DataGridViewColumn coluna in Tabuleiro.Columns)
            {
                coluna.Width = Tabuleiro.Width / Tabuleiro.Columns.Count;
            }

            foreach(DataGridViewRow linha in Tabuleiro.Rows)
            {
                linha.Height = Tabuleiro.Height / Tabuleiro.Rows.Count;
            }

            for(int linha = 0; linha < Tabuleiro.Rows.Count; linha++)
            {
                for (int coluna = 0; coluna < Tabuleiro.Columns.Count; coluna++)
                {
                    Tabuleiro[coluna, linha].ReadOnly = true;
                }
            }

            foreach(Palavra palavra in palavras)
            {
                int colunaInicial = palavra.x;
                int linhaInicial = palavra.y;

                char[] letrasSeparadas = palavra.words.ToCharArray();

                for (int i = 0; i < letrasSeparadas.Length; i++)
                {
                    if(palavra.direcao.ToUpper() == "HORIZONTAL")
                    {
                        FormatarCelula(linhaInicial, colunaInicial + i, letrasSeparadas[i].ToString());
                    }

                    if(palavra.direcao.ToUpper() == "VERTICAL")
                    {
                        FormatarCelula(linhaInicial + i, colunaInicial, letrasSeparadas[i].ToString());
                    }
                }
            }
        }

        private void FormatarCelula(int linha, int coluna, string letra)
        {
            DataGridViewCell celula = Tabuleiro[coluna, linha];

            celula.Style.BackColor = Color.White;
            celula.ReadOnly = false;
            celula.Style.SelectionBackColor = Color.Cyan;
            celula.Tag = letra;

        }

        private void Tabuleiro_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Tabuleiro_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Tabuleiro[e.ColumnIndex, e.RowIndex].Value = Tabuleiro[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();
            }
            catch
            {

            }

            try
            {
                if (Tabuleiro[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1)
                {
                    Tabuleiro[e.ColumnIndex, e.RowIndex].Value = Tabuleiro[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0,1);
                }
            }
            catch
            {

            }

            try
            {
                if(Tabuleiro[e.ColumnIndex, e.RowIndex].Value.ToString().Equals(Tabuleiro[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                {
                    Tabuleiro[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkGreen;
                }
                else
                {
                    Tabuleiro[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
                }
            }
            catch
            {

            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog janelaDeAbrirArquivo = new OpenFileDialog();
            janelaDeAbrirArquivo.Filter = "Palavras cruzadas |*.Dxt";

            if (janelaDeAbrirArquivo.ShowDialog().Equals(DialogResult.OK))
            {
                arquivoDeDicas = janelaDeAbrirArquivo.FileName;
                Tabuleiro.Rows.Clear();
                dicas.tabelaDicas.Rows.Clear();
                palavras.Clear();
            }
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            dicas.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
        }

        private void Tabuleiro_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string numero = "";
            
            if(palavras.Any(c => (numero = c.numero) != "" && c.x == e.ColumnIndex && c.y == e.RowIndex))
            {
                Rectangle r = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, r);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.DrawString(numero, f, Brushes.Black, r);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
        }
    }
    public class Palavra
    {
        public int x;
        public int y;
        public string direcao;
        public string numero;
        public string words;
        public string dica;

        public Palavra(int x, int y, string dir, string n, string p, string dic)
        {
            this.x = x;
            this.y = y;
            this.direcao = dir;
            this.numero = n;
            this.words = p;
            this.dica = dic;
        }
    }
}
