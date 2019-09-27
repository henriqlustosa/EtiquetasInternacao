using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using GenCode128;
using System.Globalization;
//using System.ServiceModel.Web;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Etiquetas
{
    public partial class Form1 : Form
    {
        String error;

        int status;
        Paciente detiq;
        Paciente detiq2;
        //HospubDados dados = new HospubDados();
        //string conStr = "DSN=hospub-server;Uid=;Pwd=;";//string de conexão com o banco de dados


        public Form1()
        {
            InitializeComponent();
            status = 0;
            error = "";
            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
            printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
            rbEtiqueta_8.Checked = true;
        }
        public class Paciente
        {
            public int cd_prontuario { get; set; }
            public string nm_nome { get; set; }
            public int cd_rf_matricula { get; set; }
            public string in_sexo { get; set; }
            public string nm_mae { get; set; }
            public string dt_data_nascimento { get; set; }
            public int nr_idade { get; set; }
            public string Bmr { get; set; }
            
        }
        private void btImprimir_Click(object sender, EventArgs e)
        {
            btImprimir.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            //btImprimir.Enabled = true;

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           btImprimir.Enabled = true;
            if (status == 1)
                lblError.Text = error;
            else
                lblError.ResetText();
            this.txbRh.ResetText();
            this.txbRh.Enabled = true;
            this.txbRh.Focus();
            this.txbRh.Text = "";
         
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                int rh = Convert.ToInt32(txbRh.Text);
                //detiq = dados.getDados(be);
                string url = "http://10.48.21.64:5000/hspmsgh-api/pacientes/paciente/" + rh;
                WebRequest request = WebRequest.Create(url);
                try
                {
                    using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                        {
                            JsonSerializer json = new JsonSerializer();
                            var objText = reader.ReadToEnd();
                            detiq = JsonConvert.DeserializeObject<Paciente>(objText);

                        }
                    }
                  
                  
                        if (TesteObito(detiq.cd_prontuario.ToString()))
                        {
                            MessageBox.Show("Este RH é de um paciente com ÓBITO!");
                        }
                        PrintDialog printDialog1 = new PrintDialog();
                        printDialog1.Document = printDocument1;
                        DialogResult result = printDialog1.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            if (rbEtiqueta_6.Checked == true)
                            {
                                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                            }
                            else if (rbEtiqueta_8.Checked == true)
                            {
                                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);
                                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);

                            }
                            printDocument1.Print();
                            txbAndar.Text = "";
                            txbQuarto.Text = "";
                            txbLeito.Text = "";


                        }
                   


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Número de RH inexistente! " + ex.Message);
                    status = 1;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Número de RH inexistente! " + ex.Message);
                status = 1;

            }
        }
        public bool TesteObito(string rh)
        { 
          
            bool bstatus = false;

            // int be = Convert.ToInt32(txbRh.Text);
                //detiq = dados.getDados(be);
                string url = "http://10.48.21.64:5000/hspmsgh-api/pacientes/paciente/" + rh;
                WebRequest request = WebRequest.Create(url);

                using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                    {
                        JsonSerializer json = new JsonSerializer();
                        var objText = reader.ReadToEnd();
                        detiq2 = JsonConvert.DeserializeObject<Paciente>(objText);

                    }
                }
                    if (detiq2.nm_nome.Contains("OBITO"))
                        bstatus = true;
           
        
            return bstatus;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            DateTime data = DateTime.Now;
            string bmr = detiq.Bmr;
            if (bmr == "MDR")
            {
                MessageBox.Show("Atenção! Paciente com RH: " + txbRh.Text + " identificado com MDR.");
                

            }

            if (rbEtiqueta_6.Checked == true)
            {

                e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);//900 é a largura da página
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                using (Graphics g = e.Graphics)
                {
                    using (Font fnt = new Font("Arial", 12))
                    {

                        int startXEsquerda = 50;
                        int starty = 10;//distancia das linhas
                        int pulaEtiq = 167;
                        
                        if (detiq.nm_nome.Length > 26)
                        {
                            string nomep1 = detiq.nm_nome;
                            int contN = nomep1.Length;
                            string nomep = detiq.nm_nome.Substring(0, 26);
                            string nomeCompos = nomep1.Substring(26);
                            

                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr , new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                            g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if ( txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else 
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " +txbLeito.Text , new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                        }
                        else
                        {
                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            }
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR" )
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                        }
                    }
                }
            }
            else if (rbEtiqueta_8.Checked == true)
            {

                e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);//900 é a largura da página
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);
                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);
                using (Graphics g = e.Graphics)
                {
                    using (Font fnt = new Font("Arial", 12))
                    {

                        int startXEsquerda = 40;
                        int starty = 10;//distancia das linhas
                        int pulaEtiq = 150;
                        

                        if (detiq.nm_nome.Length > 26)
                        {
                            string nomep1 = detiq.nm_nome;
                            int contN = nomep1.Length;
                            string nomep = detiq.nm_nome.Substring(0, 26);
                            string nomeCompos = nomep1.Substring(26);
                            

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     "+ bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);


                        }
                        else
                        {

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.cd_rf_matricula, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.nm_nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.dt_data_nascimento + " Idade: " + detiq.nr_idade + " Sexo: " + detiq.in_sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.nm_mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (txbAndar.Text == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (txbAndar.Text == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + txbAndar.Text + " Quarto: " + txbQuarto.Text + " Leito: " + txbLeito.Text, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                        }
                    }
                }
            }
            
        }

        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {
             
            if (e.KeyChar == (char)Keys.Enter)
            {

                btImprimir_Click( sender,  e);

            }
        }

     
    }
}