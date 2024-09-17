using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;

namespace ProcessamentoArquivosDB
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void SalvarRegistro(object sender, EventArgs e)
        {
            {
                // Configurações do banco de dados
                string connectionString = @"Server=localhost\SQLEXPRESS;Database=db_teste;Trusted_connection=True;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Caminhos dos diretórios
                    string caminho = @"C:\Users\Daniela Chamon\Documents\Produção Temática";
                    string caminhoBackup = @"C:\Users\Daniela Chamon\Documents\Produção Temática\Backup";

                    if (Directory.Exists(caminho))
                    {
                        int vQtdArquivos = 0;
                        bool importei = false;

                        DirectoryInfo pasta = new DirectoryInfo(caminho);

                        foreach (FileInfo arquivo in pasta.GetFiles())
                        {
                            vQtdArquivos++;
                            string delim = ";";

                            string nomeArquivo = arquivo.Name;                  // Nome do arquivo
                            string enderecoCompleto = arquivo.FullName;          // Caminho completo do arquivo
                            long tamanhoArquivo = arquivo.Length;               // Tamanho do arquivo (em bytes)
                            DateTime dataDisponibilizacao = arquivo.CreationTime; // Data e hora da disponibilização


                            // Verifica se o arquivo já existe no banco de dados
                            string vSQLnomearq = $"SELECT no_arquivo FROM prTb001_Arquivo WHERE no_arquivo='{arquivo.Name}'";

                            SqlCommand cmdArquivo = new SqlCommand(vSQLnomearq, conn);
                            SqlDataReader rsArquivo = cmdArquivo.ExecuteReader();

                            bool arquivoExiste = rsArquivo.HasRows;
                            rsArquivo.Close();  // Fecha o DataReader antes de executar qualquer outra operação

                            // Executa o DELETE depois de fechar o DataReader
                            string deleteQuery = "DELETE FROM prTb002_Base_Mensal";
                            SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                            deleteCmd.ExecuteNonQuery();

                            // Fecha o leitor de dados antes de prosseguir


                            if (!arquivoExiste) // Se o arquivo não está na tabela
                            {
                                string vArquivoExiste = "0";

                                if (arquivo.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
                                {
                                    importei = true;

                                    string arquivoSICTD = Path.Combine(caminho, arquivo.Name);

                                    // Usar ExecuteNonQuery para a inserção em massa
                                    string bulkInsertQuery = $@"
                                BULK INSERT prTb002_Base_Mensal 
                                FROM '{arquivoSICTD}' 
                                WITH (CODEPAGE = '65001', FIRSTROW = 2, FIELDTERMINATOR = ';', ROWTERMINATOR = '0x0a')";
                                    SqlCommand bulkInsertCmd = new SqlCommand(bulkInsertQuery, conn);
                                    bulkInsertCmd.ExecuteNonQuery();


                                    string vSQL02 = $"INSERT INTO [dbo].[prTb001_Arquivo]([no_arquivo],[dh_arquivo],[qt_tamanho],[no_endereco_completo])VALUES('{arquivo.Name}.+ {dataDisponibilizacao}','{dataDisponibilizacao}',{tamanhoArquivo},'{enderecoCompleto}')  ";
                                    SqlCommand cmdSQL02 = new SqlCommand(vSQL02, conn);
                                    cmdSQL02.ExecuteNonQuery();

                                    // Mover o arquivo para o caminho de backup
                                    string arquivoDestino = Path.Combine(caminhoBackup, arquivo.Name);
                                    File.Copy(arquivo.FullName, arquivoDestino, true);
                                    File.Delete(arquivo.FullName);
                                }
                            }
                        }
                    }

                    conn.Close();
                }

            }
        }
    }
}
