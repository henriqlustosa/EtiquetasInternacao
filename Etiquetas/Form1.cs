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
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Printing;

namespace Etiquetas
{
    public partial class Etiqueta : Form
    {
        // Track potential errors or status
        private string _errorMessage = string.Empty;
        private int _status = 0;

        // Models
        private Paciente _paciente;
        private List<Internacao> _internacoes;

        // Internação details
        private string _andar = "";
        private string _quarto = "";
        private string _leito = "";
        private string _clinica = "";

        public Etiqueta()
        {
            InitializeComponent();
            // ...
            cbQtdEtiquetas.SelectedIndex = 0; // Seleciona o 1º item da lista
            // Exemplo: se quiser selecionar o terceiro item
            // comboBox1.SelectedIndex = 2;

            // Initialize printing settings
            printDocument1.DefaultPageSettings.PaperSize = new PaperSize("Custom2", 400, 1000);
            //printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new PaperSize("Custom2", 400, 1000);

            // Configure printer and print
            //printDocument1.PrinterSettings.PrinterName = "HP TI";
            printDocument1.PrinterSettings.PrinterName = "PrinterEtiqueta";
            //printDocument1.PrinterSettings.PrinterName = "ImpressoraPS_";
            // Default radio button
           
        }
      
        #region Models

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
            public string nm_pai { get; set; }
            public string nm_nome_social { get; set; }
        }

        public class Internacao
        {
            public string cd_prontuario { get; set; }
            public string nr_leito { get; set; }
            public string dt_alta_medica { get; set; }
            public string nm_especialidade { get; set; }
        }

        #endregion

        #region Event Handlers

        private void btImprimir_Click(object sender, EventArgs e)
        {
            btImprimir.Enabled = false;
            backgroundWorker1.RunWorkerAsync();

            // Re-enable the Print button after background worker completes
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btImprimir.Enabled = true;

            if (_status == 1)
                lblError.Text = _errorMessage;
            else
                lblError.ResetText();

            // Reset input field
            txbRh.ResetText();
            txbRh.Enabled = true;
            txbRh.Focus();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int rhNumber = Convert.ToInt32(txbRh.Text);
                FetchPacienteData(rhNumber);
                // (a) Invocar o printDocument1.Print() na thread de UI:
                this.Invoke((MethodInvoker)delegate
                {
                    int i = 0;
                    int size = Convert.ToInt32(cbQtdEtiquetas.SelectedItem);

                    while (i < size)
                    {
                        printDocument1.Print();
                        i++;
                    }
                });
            }
            catch (Exception ex)
            {
                _status = 1;
                _errorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Trigger printing when the user presses Enter in the txbRh textbox.
        /// </summary>
        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btImprimir_Click(sender, e);
            }
        }

        /// <summary>
        /// Main print logic: decides page size and how many labels to print, then draws them.
        /// </summary>
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // If patient data is missing, just return
            if (_paciente == null)
                return;

            // Quick check for MDR status
            if (_paciente.Bmr == "MDR")
            {
                MessageBox.Show(
                    string.Format("Atenção! Paciente com RH: {0} identificado com MDR.", txbRh.Text));
            }

          
            int height = 1000;
            e.PageSettings.PaperSize = new PaperSize("Custom2", 400, height);

            // Draw the specified number of labels
            int startX = 65;
            int startY = 2;

           
            int labelCount = Convert.ToInt32(cbQtdEtiquetas.SelectedItem);
            using (Graphics g = e.Graphics)
            {
                //for (int i = 0; i < labelCount; i++)
                //{
                    // Y offset for each label
                 
                    DrawSingleLabel(g, _paciente, txbRh.Text, startY, startX);
               // }
            }
        }

        #endregion

        #region Data Fetching

        /// <summary>
        /// Fetches Paciente and Internacao data from their respective APIs.
        /// Populates private fields for later use in printing.
        /// </summary>
        private void FetchPacienteData(int rhNumber)
        {
            // Reset error/status
            _errorMessage = "";
            _status = 0;

            // Example URIs (replace with your real endpoints)
            string pacienteUrl = string.Format("http://10.48.21.64:5000/hspmsgh-api/pacientes/paciente/{0}", rhNumber);
            string internacoesUrl = string.Format("http://10.48.17.99:5003/hspmsgh-api/internacoes/{0}", rhNumber);

            // Fetch Paciente
            _paciente = MakeWebRequest<Paciente>(pacienteUrl);

            // Fetch Internacoes
            _internacoes = MakeWebRequest<List<Internacao>>(internacoesUrl);

            // If there's at least one internacao and it has no alta (discharge) date
            if (_internacoes != null && _internacoes.Count > 0 && _internacoes[0].dt_alta_medica == null)
            {
                string leitoFull = _internacoes[0].nr_leito ?? "";
                _clinica = _internacoes[0].nm_especialidade ?? "";

                // Safety check on length
                if (leitoFull.Length >= 5)
                {
                    // e.g.: if nr_leito is "07 10 02" => "07" -> andar, "10" -> quarto, "02" -> leito
                    // You may need to adapt substring indexes to match your actual string format
                    _andar = leitoFull.Substring(0, 2).Trim();   // e.g., "07"
                    _quarto = leitoFull.Substring(2, 2).Trim();  // e.g., "10"
                    _leito = leitoFull.Substring(5, 2).Trim();   // e.g., "02"
                }
                else
                {
                    _andar = "Leito Extra";
                    _quarto = "";
                    _leito = "";
                }
            }
            else
            {
                _andar = "";
                _quarto = "";
                _leito = "";
                _clinica = "";
            }
        }

        /// <summary>
        /// Generic method to make a GET web request and deserialize JSON into T.
        /// </summary>
        private T MakeWebRequest<T>(string url) where T : class
        {
            WebRequest request = WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
        }

        #endregion

        #region Printing Helpers

        /// <summary>
        /// Draws a single label at the specified Y offset.
        /// </summary>
        private void DrawSingleLabel(Graphics g, Paciente paciente, string rhText, int offsetY, int offsetX)
        {
            if (paciente == null) return;

            // Fonts
            Font boldFont = new Font("Arial", 8, FontStyle.Bold);
            Font regularFont = new Font("Arial", 8, FontStyle.Regular);

            // Prepare text segments
            string nomeCompleto = (paciente.nm_nome ?? "").Trim();
            string nomeMae = (paciente.nm_mae ?? "").Trim();
            string nomeSocial = paciente.nm_nome_social ?? string.Empty;
            string nomePai = paciente.nm_pai ?? string.Empty;
            // If there's a nome social, concatenate it
            if (!string.IsNullOrEmpty(nomeSocial))
            {
                nomeCompleto = nomeSocial + " (" + nomeCompleto + ")";
            }

            // If the mother's name is "DESCONHECIDA", use the father's name
            if (nomeMae.Equals("DESCONHECIDA", StringComparison.OrdinalIgnoreCase))
            {
                nomeMae = nomePai;
            }
            // Split name strings (out parameters)
            string nomeLinha1, nomeLinha2;
            SplitString(nomeCompleto, 31, out nomeLinha1, out nomeLinha2);


            nomeMae = AbreviarSomenteNomesDoMeio(nomeMae, 38);
         
            // Start drawing
            g.DrawString(string.Format("Hospital do Servidor Publico Municipal"), boldFont, Brushes.Black, offsetX, offsetY + 7);
            // Build the line that contains RH, RF, and possibly BMR status
            string rhRfLine = string.Format("Prontuario: {0}       RF: {1}", rhText, paciente.cd_rf_matricula);
            if (paciente.Bmr == "MDR")
            {
                rhRfLine += "     MDR";
            }

            // Nome (split across two lines if needed)
            g.DrawString(
                string.Format(rhRfLine),
                regularFont,
                Brushes.Black,
                offsetX,
                offsetY + 19
            );

            // Nome (split across two lines if needed)
            g.DrawString(
                string.Format("Paciente: {0}", nomeLinha1),
                boldFont,
                Brushes.Black,
                offsetX,
                offsetY + 31
            );

            if (!string.IsNullOrEmpty(nomeLinha2))
            {
                g.DrawString(
                    string.Format("                 {0}", nomeLinha2),
                    boldFont,
                    Brushes.Black,
                    offsetX,
                    offsetY + 43
                );
            }

            // Decide vertical offset for the next line, depending on whether we used two lines
            int nascIdadeSexoY = !string.IsNullOrEmpty(nomeLinha2) ? (offsetY + 55) : (offsetY + 55 - 12);
            // Nascimento / Idade / Sexo
            g.DrawString(
                string.Format("Nasc: {0}      Idade: {1}      Sexo: {2}",
                    paciente.dt_data_nascimento,
                    paciente.nr_idade,
                    paciente.in_sexo),
                regularFont,
                Brushes.Black,
                offsetX,
                nascIdadeSexoY
            );

            // Mãe (split if needed)
            int maeBaseY = nascIdadeSexoY + 12;
            g.DrawString(
                string.Format("Mae: {0}", nomeMae),
                regularFont,
                Brushes.Black,
                offsetX,
                maeBaseY
            );

             int andarQuartoLeitoBaseY = maeBaseY + 12;
          
            // Andar/Quarto/Leito
            string leitoString;
            if (string.IsNullOrEmpty(_andar) &&
                string.IsNullOrEmpty(_quarto) &&
                string.IsNullOrEmpty(_leito))
            {
                leitoString = "Andar:____    Quarto:____    Leito:____";
            }
            else if (_andar == "Leito Extra")
            {
                leitoString = "Leito Extra";
            }
            else
            {
                leitoString = string.Format("Andar: {0}    Quarto: {1}    Leito: {2}", _andar, _quarto, _leito);
            }

            g.DrawString(leitoString, regularFont, Brushes.Black, offsetX, andarQuartoLeitoBaseY);
        }

        /// <summary>
        /// Utility method to split a string into two parts if it exceeds a given length.
        /// Compatible with .NET 3.5 (no tuples).
        /// </summary>
        private void SplitString(string original, int maxLength, out string part1, out string part2)
        {
            if (string.IsNullOrEmpty(original))
            {
                part1 = string.Empty;
                part2 = string.Empty;
                return;
            }

            if (original.Length <= maxLength)
            {
                part1 = original;
                part2 = string.Empty;
                return;
            }

            part1 = original.Substring(0, maxLength);
            part2 = original.Substring(maxLength).Trim();
        }
        /// <summary>
        /// Mantém o primeiro e o último nome por extenso,
        /// abrevia apenas os nomes do meio, caso o tamanho exceda 'tamanhoMaximo'.
        /// Nunca abrevia as palavras "DE", "DA", "DO", "DOS", "DAS" (ignora case).
        /// </summary>
        /// <param name="nomeMae">Nome completo (ex.: "MARIA DAS DORES DE OLIVEIRA DA SILVA").</param>
        /// <param name="tamanhoMaximo">Tamanho limite (ex.: 36).</param>
        /// <returns>Nome encurtado se necessário.</returns>
        private string AbreviarSomenteNomesDoMeio(string nomeMae, int tamanhoMaximo)
        {
            // 1) Se está vazio/nulo ou já é menor/igual ao limite, retorna como está
            if (string.IsNullOrEmpty(nomeMae) || nomeMae.Length <= tamanhoMaximo)
                return nomeMae;

            // 2) Dividir em partes (por espaços)
            //    Ex.: ["MARIA", "DAS", "DORES", "DE", "OLIVEIRA", "DA", "SILVA"]
            var partes = nomeMae.Trim().Split(' ');

            // Se só tiver uma palavra, não há "último" nome para manter, então corta forçado
            if (partes.Length == 1)
            {
                return nomeMae.Substring(0, tamanhoMaximo);
            }

            // Lista (ou conjunto) de termos que NÃO devem ser abreviados
            // Obs.: usando StringComparer.OrdinalIgnoreCase para ignorar maiúsc./minúsc.
            var naoAbreviar = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "DE", "DA", "DO", "DOS", "DAS"
    };

            // 3) Mantemos partes[0] e partes[1] sem mexer (conforme seu código, que inicia em i=2)
            //    e também o último (partes[partes.Length - 1]) sem mexer.
            //    Abreviamos apenas as posições de 2 até (Length - 2).
            //    Ajuste se desejar outra lógica.
            for (int i = 2; i < partes.Length - 1; i++)
            {
                // Se a palavra estiver na lista de não-abreviar, pule.
                if (naoAbreviar.Contains(partes[i]))
                    continue;

                // Se a palavra tiver mais de 1 letra, abrevia para "X."
                // Ex.: "Oliveira" → "O."
                if (partes[i].Length > 1)
                {
                    partes[i] = partes[i].Substring(0, 1) + ".";
                }

                // Recria a string e verifica o tamanho
                string nomeMontado = string.Join(" ", partes);

                if (nomeMontado.Length <= tamanhoMaximo)
                {
                    // Se já está dentro do limite, retornamos
                    return nomeMontado;
                }
            }

            // 4) Depois de abreviar tudo que foi possível, se ainda estiver grande,
            //    faz um corte forçado:
            string resultadoFinal = string.Join(" ", partes);
            if (resultadoFinal.Length > tamanhoMaximo)
            {
                resultadoFinal = resultadoFinal.Substring(0, tamanhoMaximo);
            }

            return resultadoFinal;
        }



        #endregion
    }
}
