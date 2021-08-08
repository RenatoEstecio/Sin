using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sin
{
    public partial class Sin : Form
    {
        string legenda;
        string loc;
        public Sin()
        {
            InitializeComponent();
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            opfArq.Filter = "SRT files (*.srt)|*.srt|All files (*.*)|*.*";/*Restringe o usuario 
                                                                           * pra não escolher arquivos nao compativeis*/

            if (opfArq.ShowDialog() == DialogResult.OK)/*Se caso ele mandou abrir algum arquivo*/
            {
                try
                {
                    ligaSin();
                    ttbName.Text = opfArq.FileName; /*Copia o nome do arquivo p/ textbox*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro! :(");
                }              
            }
        }
        void desligaSin()
        {
            ttbCorrigir.Visible = false;
            ttbName.Visible = false;
            button1.Visible = false;
            label1.Visible = false;
        }
        void ligaSin()
        {
            ttbCorrigir.Visible = true;
            ttbName.Visible = true;
            button1.Visible = true;
            label1.Visible = true;
        }
        private void Sin_Load(object sender, EventArgs e)
        {
            desligaSin();
        }
        string SegToTime(string aux)/*Agora transforma os segundos devolta em tempo(Time)*/
        {
            decimal segundos = Convert.ToDecimal(aux);

            int hora = (int) (segundos / 60) / 60;
            segundos -= (hora * 60) * 60;
            int min = (int)(segundos / 60);
            segundos -= (min * 60) ;
            string h = hora+"";
            string m = min + "";
            if (h.Length < 2)
                h = "0" + h;
            if (m.Length < 2)
                m = "0" + m;
            return h + ":" + m + ":" + segundos;
        }
        string Ajusta(string aux, decimal ajuste)
        {
            string[] time = aux.Split(':');/*Convert tempo em segundos e soma o que o usuario pediu*/
            ajuste = Convert.ToDecimal(time[0]) * 60 * 60 +
                     Convert.ToDecimal(time[1]) * 60 +
                     Convert.ToDecimal(time[2]) + ajuste;

            return ajuste + "";
        }
        void gravar(string s, string legenda)
        {
            s = s.Replace("_CorrigidaBySin", "[Copy]").Replace(".srt", "_CorrigidaBySin.srt");
            System.IO.StreamWriter file = new System.IO.StreamWriter(s);
            /*Manda por na mesma pasta do arquivo original mas muda o nome do arquivo*/
            file.Write(legenda);
            
            file.Close();
            loc= s;
        }
        void executa()
        {
            string aux1, aux2;
            string lin;/**/

            decimal Ajuste = Convert.ToDecimal(ttbCorrigir.Text);/*Pega os segundos que o usuario digitou*/
            decimal tempo = 0;
            if ((opfArq.OpenFile()) != null)
            {
                System.IO.StreamReader sr = new
                System.IO.StreamReader(opfArq.FileName);

                while (!sr.EndOfStream)
                {
                    lin = sr.ReadLine();/*Começa ler linha por linha*/

                    if (lin.Contains("-->"))/*Vê se essa linha é sobre marcação de tempo*/
                    {
                        try {
                            aux1 = Ajusta(lin.Split(' ')[0], Ajuste);/*Aqui ele pega o tempo que começa a fala*/
                            aux2 = Ajusta(lin.Split('>')[1], Ajuste);/*Aqui ele pega o fim da fala(Quando some da tela)*/

                            legenda += SegToTime(aux1) + " --> " + SegToTime(aux2) + "\r\n";
                        }
                        catch(Exception si) { }
                    }
                    else
                        legenda += lin + "\r\n";
                }
                sr.Close();
                sr.Dispose();
                string name = opfArq.FileName;
                opfArq.Dispose();
                opfArq.Reset();
                gravar(name, legenda);
                MessageBox.Show("Concluido!\nArquivo criado em "+loc);/*Molezinha... viu? ^^ */
                desligaSin();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (ttbCorrigir.Text.Length > 0)
            {
                try
                {
                    executa();
                }
                catch (Exception si)
                { MessageBox.Show("Erro!"); }
            }
            else
                MessageBox.Show("Digite a quantidade de segundos!");

        }
    }
}
