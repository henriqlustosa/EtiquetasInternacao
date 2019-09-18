using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace Etiquetas
{
    public class HospubDados
    {

        string conStr2 = "Data Source=10.48.16.14;Initial Catalog=Isolamento;User ID=h010994;Password=soundgarden";//string de conexão com o banco de dados
        string conStr = "DSN=hospub-server;Uid=;Pwd=;";//string de conexão com o banco de dados

        public DadosPaciente getDados(int rh)
        {
            DadosPaciente detiq = new DadosPaciente();

            detiq.Bmr = "false";
            string andarCompleto = "";


            detiq.Rh = "";



            string strSql = "Select ib6pnome, ib6compos, ib6mae, ib6sexo, ib6dtnasc, ib6prontuar, ib6codcs, ib6regist from intb6 where ib6regist = " + rh;
            string strSql3 = "Select c02codleito from cen02 where i02pront = " + rh;







            string nasc = "";
            string dtnasc = "";
            int idade = 0;
            try
            {

                OdbcConnection com = new OdbcConnection(conStr);//Define a conexão
                OdbcCommand commd = new OdbcCommand(strSql, com);//obtem acesso e executa o comando SQL
                com.Open();

                OdbcDataReader dr = commd.ExecuteReader();

                if (dr.Read())
                {

                    detiq.Rh = dr.GetDecimal(7).ToString();
                    detiq.Nome = dr.GetString(0) + dr.GetString(1);
                    detiq.Mae = dr.GetString(2);
                    detiq.Sexo = dr.GetString(3);

                    if (detiq.Sexo == "1")
                    {
                        detiq.Sexo = "M";
                    }
                    else if (detiq.Sexo == "3")
                    {
                        detiq.Sexo = "F";
                    }
                    dtnasc = dr.GetString(4);
                    detiq.Rf = dr.GetDecimal(5).ToString();
                    if (detiq.Rf == "0")
                        detiq.Rf = "";
                    detiq.Us = dr.GetString(6);

                    nasc = dtnasc.Substring(6, 2) + "/" + dtnasc.Substring(4, 2) + "/" + dtnasc.Substring(0, 4);

                    idade = DateTime.Now.Year - Convert.ToInt32(dtnasc.Substring(0, 4));
                    int mes = Convert.ToInt32(dtnasc.Substring(4, 2));
                    if ((DateTime.Now.Month <= mes) && (DateTime.Now.Day < Convert.ToInt32(dtnasc.Substring(6, 2))))
                        idade--;
                    detiq.Idade = idade.ToString();
                    detiq.Data = nasc;
                    ExisteExameSI(rh, detiq);
                    //detiq.NumbBe = Convert.ToString(rh);
                }
                dr.Close();

                commd.Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                OdbcConnection com3 = new OdbcConnection(conStr);//Define a conexão
                OdbcCommand commd3 = new OdbcCommand(strSql3, com3);//obtem acesso e executa o comando SQL
                com3.Open();

                OdbcDataReader dr3 = commd3.ExecuteReader();
                string andar = "";

                detiq.Andar = "";
                detiq.Quarto = "";
                detiq.Leito = "";
                if (dr3.Read())
                {
                    andarCompleto = dr3.GetString(0);

                    andar = andarCompleto.Substring(0, 2);
                    if (andar == "99")
                        detiq.Andar = "Leito Extra";
                    else
                    {
                        detiq.Andar = andar;
                        detiq.Quarto = andarCompleto.Substring(3, 2);
                        detiq.Leito = andarCompleto.Substring(5, 2);
                    }


                }
            }
            catch (Exception ex2)
            {
                Console.WriteLine(ex2.Message);
            }




            return detiq;


        }


        public bool isSistemaIsolado(int rh, DadosPaciente detiq)
        {
            bool isSI = false;

            string strSql2 = "SELECT * FROM [Isolamento].[dbo].[Exame] where rh = " + rh;

            try
            {

                SqlConnection com2 = new SqlConnection(conStr2);//Define a conexão
                SqlCommand commd2 = new SqlCommand(strSql2, com2);//obtem acesso e executa o comando SQL
                com2.Open();

                SqlDataReader dr2 = commd2.ExecuteReader();

                if (dr2.Read())
                {
                    isSI = true;
                }
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
            }
            return isSI;
        }

        public void ExisteExameSI(int rh, DadosPaciente detiq)
        {



            using (SqlConnection cnn = new SqlConnection(conStr2))
            {
                SqlCommand cmm = cnn.CreateCommand();
                cmm.CommandText = "SELECT p.rh as RH ,p.nome  as Nome ,m.descricao as Microorganismo ,ma.descricao as Sitio , convert(varchar, e.dt_resultado, 103) as 'Data' FROM [Isolamento].[dbo].[Exame] as e "
                + " INNER JOIN [Isolamento].[dbo].[Paciente] as p ON e.rh = p.rh "
                + " INNER JOIN [Isolamento].[dbo].[tipos_microorganismos] as  m ON e.microorganismo = m.cod_microorg "
                + " INNER JOIN [Isolamento].[dbo].[tipos_materiais] as  ma ON e.material = ma.cod_material where p.rh = " + rh + "  order by dt_resultado ";
                cnn.Open();
                SqlDataReader dr = cmm.ExecuteReader();

                while (dr.Read())
                {
                    bool imprime = false;
                    DateTime dtSaidaAnterior = new DateTime(0001, 01, 01, 0, 0, 0);
                    DateTime dtSaida = new DateTime();
                    DateTime dtEntrada = new DateTime();
                    int count2 = 0;
                    string dataExame = dr.GetString(4);
                    DateTime dtAtual = DateTime.Now;


                    DateTime dtExame = Convert.ToDateTime(dataExame);
                    using (OdbcConnection cnn10 = new OdbcConnection(conStr))
                    {
                        OdbcCommand cmm10 = cnn10.CreateCommand();
                        cmm10.CommandText = "select d15apres,d15compos1,d15inter from cen15 where i15pront = " + rh;
                        cnn10.Open();
                        OdbcDataReader dr10 = cmm10.ExecuteReader();

                        if (dr10.Read())
                        {
                            string dtEntrada10 = Convert.ToString(dr10.GetDecimal(2));
                            dtEntrada10 = dtEntrada10.Substring(6, 2) + "/" + dtEntrada10.Substring(4, 2) + "/" + dtEntrada10.Substring(0, 4);
                            DateTime dtEntrada2 = Convert.ToDateTime(dtEntrada10);
                            DateTime dtComparacao = dtExame.AddDays(180);
                            if (dtComparacao > dtEntrada2)
                            {
                                using (OdbcConnection cnn2 = new OdbcConnection(conStr))
                                {

                                    OdbcCommand cmm2 = cnn2.CreateCommand();
                                    cmm2.CommandText = "select d15apres,d15compos1,d15inter from cen15 where i15pront = " + rh;
                                    cnn2.Open();
                                    OdbcDataReader dr2 = cmm2.ExecuteReader();

                                    while (dr2.Read())
                                    {
                                        string dtSaida1 = Convert.ToString(dr2.GetDecimal(0));
                                        string dtSaida2 = Convert.ToString(dr2.GetDecimal(1));
                                        string dtEntrada1 = Convert.ToString(dr2.GetDecimal(2));
                                        dtEntrada1 = dtEntrada1.Substring(6, 2) + "/" + dtEntrada1.Substring(4, 2) + "/" + dtEntrada1.Substring(0, 4);
                                        dtEntrada = Convert.ToDateTime(dtEntrada1);

                                        dtSaida2 = dtSaida2.PadLeft(2, '0');
                                        string data = dtSaida2 + "/" + dtSaida1.Substring(4, 2) + "/" + dtSaida1.Substring(0, 4);
                                        dtSaida = Convert.ToDateTime(data);


                                        dtSaida = dtSaida.AddDays(15);



                                        if (dtExame <= dtSaida)
                                        {
                                            dtSaida = dtSaida.AddDays(-15);
                                            count2 = count2 + 1;
                                            imprime = true;


                                            DateTime comparar = new DateTime(0001, 01, 01, 0, 0, 0);
                                            if (dtSaidaAnterior.CompareTo(comparar) != 0)
                                            {
                                                if ((dtEntrada - dtSaidaAnterior).Days <= 180)
                                                {


                                                    imprime = true;

                                                }
                                                else
                                                {
                                                    imprime = false;
                                                    break;
                                                }
                                            }

                                            dtSaidaAnterior = dtSaida;

                                        }
                                        //count2 = count2 + 1;
                                        //break;





                                    }//while existir Data de Saida

                                }

                            }
                        }
                    }


                    string dtInternacao = "";
                    using (OdbcConnection cnn3 = new OdbcConnection(conStr))
                    {
                        DateTime dataInternacao = new DateTime(0001, 01, 01, 0, 0, 0);

                        OdbcCommand cmm3 = cnn3.CreateCommand();
                        cmm3.CommandText = "select d02inter from cen02 where i02pront = " + rh;
                        cnn3.Open();
                        OdbcDataReader dr3 = cmm3.ExecuteReader();

                        if (dr3.Read())
                        {
                            dtInternacao = Convert.ToString(dr3.GetDecimal(0));
                            dtInternacao = dtInternacao.Substring(6, 2) + "/" + dtInternacao.Substring(4, 2) + "/" + dtInternacao.Substring(0, 4);
                            dataInternacao = Convert.ToDateTime(dtInternacao);
                        }
                        DateTime comparar = new DateTime(0001, 01, 01, 0, 0, 0);
                        if (imprime)
                        {

                            if (((dtAtual - dtSaida).Days <= 180) || ((dataInternacao.CompareTo(comparar) != 0) && ((dataInternacao - dtSaida).Days <= 180)))
                            {


                                detiq.Bmr = "MDR";
                            }


                        }
                        if (dataInternacao.CompareTo(comparar) != 0)
                        {

                            if (count2 == 0)
                            {
                                if (dataInternacao <= dtExame)
                                {
                                    count2 = count2 + 1;

                                    detiq.Bmr = "MDR";

                                }

                            }
                        }
                    }
                    if (count2 == 0)
                    {
                        if ((dtAtual - dtExame).Days <= 180)
                        {
                            count2 = count2 + 1;

                            detiq.Bmr = "MDR";

                        }

                    }
                }


            }

        }


        private string dataFormatada(string data)
        {
            return data.Substring(0, 4) + "-" + data.Substring(4, 2) + "-" + data.Substring(6, 2);

        }
    }

}
