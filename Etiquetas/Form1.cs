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

namespace Etiquetas
{
    public partial class Form1 : Form
    {
        String error;

        int status;
        DadosPaciente detiq;
        HospubDados dados = new HospubDados();
        string conStr = "DSN=hospub-server;Uid=;Pwd=;";//string de conexão com o banco de dados


        public Form1()
        {
            InitializeComponent();
            status = 0;
            error = "";
            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
            printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
            rb6.Checked = true;
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
                
                int be = Convert.ToInt32(txbRh.Text);
                detiq = dados.getDados(be);
              
                if (!(detiq.Rh == ""))
                {
                    if (TesteObito(detiq.Rh))
                    {
                        MessageBox.Show("Este RH é de um paciente com ÓBITO!");
                    }
                        PrintDialog printDialog1 = new PrintDialog();
                        printDialog1.Document = printDocument1;
                        DialogResult result = printDialog1.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            if (rb6.Checked == true)
                            {
                                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1000);
                            }
                            else if (rb8.Checked == true)
                            {
                                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);
                                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 400, 1200);

                            }
                            printDocument1.Print();

                        }
                    }
                  
                
                else
                {
                    MessageBox.Show("Número de RH inexistente!");
                }
                status = 0;
                
          
            }
            catch (Exception ex)
            {
                MessageBox.Show("Número de RH inexistente! " + ex.Message);
                status = 1;
                
            }
        }
        public bool TesteObito(string rh)
        {
            string status = "";

            bool bstatus = false;
            using (OdbcConnection cnn7 = new OdbcConnection(conStr))
            {
                OdbcCommand cmm7 = cnn7.CreateCommand();
                cmm7.CommandText = "select c15motivo from cen15 where  i15pront = " + rh;
                cnn7.Open();
                OdbcDataReader dr7 = cmm7.ExecuteReader();
                if (dr7.Read())
                {
                    status = dr7.GetString(0);
                    if (status == "3")
                        bstatus = true;
                    else if (status == "4")
                        bstatus = true;
                    else
                        bstatus = false;
                }
                dr7.Close();
            }
            using (OdbcConnection cnn8 = new OdbcConnection(conStr))
            {

                OdbcCommand cmm8 = cnn8.CreateCommand();
                cmm8.CommandText = "select * from intb6 where ((ib6compos like '%OBITO%') or (ib6dtobito != '' and ib6dtobito != '00000000')) and ib6regist =" + rh;
                cnn8.Open();
                OdbcDataReader dr8 = cmm8.ExecuteReader();

                if (dr8.Read())
                {
                    bstatus = true;

                }
                dr8.Close();
            }

            if (bstatus)
            {
                Console.WriteLine("Este RH é de um paciente com ÓBITO!");


            }
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

            if (rb6.Checked == true)
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
                        
                        if (detiq.Nome.Length > 26)
                        {
                            string nomep1 = detiq.Nome;
                            int contN = nomep1.Length;
                            string nomep = detiq.Nome.Substring(0, 26);
                            string nomeCompos = nomep1.Substring(26);
                            

                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr , new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                            g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if ( detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else 
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " +detiq.Leito , new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                        }
                        else
                        {
                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            }
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR" )
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                        }
                    }
                }
            }
            else if (rb8.Checked == true)
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
                        

                        if (detiq.Nome.Length > 26)
                        {
                            string nomep1 = detiq.Nome;
                            int contN = nomep1.Length;
                            string nomep = detiq.Nome.Substring(0, 26);
                            string nomeCompos = nomep1.Substring(26);
                            

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     "+ bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + nomep, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("            " + nomeCompos, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 104);


                        }
                        else
                        {

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            g.DrawString("", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 88);

                            starty += pulaEtiq;

                            //g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            if (bmr == "MDR")
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf + "     " + bmr, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 6);
                            }
                            else
                            {
                                g.DrawString("RH: " + txbRh.Text + "       RF: " + detiq.Rf, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 7);

                            } 
                            g.DrawString("Nome: " + detiq.Nome, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 24);
                            g.DrawString("Nasc: " + detiq.Data + " Idade: " + detiq.Idade + " Sexo: " + detiq.Sexo, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 40);
                            g.DrawString("Mãe: " + detiq.Mae, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 56);
                            if (detiq.Andar == "")
                                g.DrawString("Andar:____ Quarto:____ Leito:____ ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else if (detiq.Andar == "Leito Extra")
                                g.DrawString("Leito Extra ", new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
                            else
                                g.DrawString("Andar: " + detiq.Andar + " Quarto: " + detiq.Quarto + " Leito: " + detiq.Leito, new Font("Arial", 10, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 72);
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