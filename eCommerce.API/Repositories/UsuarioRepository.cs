using eCommerce.API.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace eCommerce.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        /*
         * Connection - Estabelecer a conexão
         * Command - Executar comandos SQL (INSERT, UPDATE, DELETE...)
         * DataReader - Ler dados (SELECT) conectado ao database
         * DataAdapter - Ler dados (SELECT) estando desconectado ao database e armezando os dados na memória
         */

        private SqlConnection _connection = new SqlConnection(@"Server=GUI-DESKTOP;Database=eCommerce;Integrated Security=True;TrustServerCertificate=True");

        public List<Usuario> GetAll()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT TOP 100 * FROM [Usuarios]";
                cmd.Connection = _connection;

                _connection.Open();

                SqlDataReader dataReader = cmd.ExecuteReader();
                //Dapper, EF, NHibernate (ORM - Object-POO Relational-MER Mapper)
                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32("Id");
                    usuario.Nome = dataReader.GetString("Nome");
                    usuario.Email = dataReader.GetString("Email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.CPF = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                    usuarios.Add(usuario);
                }
            }
            finally
            {
                _connection.Close();
            }
            return usuarios;
        }

        public Usuario Get(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = $"SELECT TOP 100 * FROM [Usuarios] AS U LEFT JOIN [Contatos] AS C ON U.Id = C.UsuarioId LEFT JOIN [EnderecosEntrega] AS EE ON U.Id = EE.UsuarioId LEFT JOIN [UsuariosDepartamentos] AS UD ON U.Id = UD.UsuarioId LEFT JOIN [Departamentos] AS D ON UD.DepartamentoId = D.Id WHERE U.Id = @Id";
                cmd.Connection = _connection;
                cmd.Parameters.AddWithValue("@Id", id);

                _connection.Open();

                SqlDataReader dataReader = cmd.ExecuteReader();

                Dictionary<int, Usuario> dUsuario = new Dictionary<int, Usuario>();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    if (!(dUsuario.ContainsKey(dataReader.GetInt32(0))))
                    {
                        usuario.Id = dataReader.GetInt32(0);
                        usuario.Nome = dataReader.GetString("Nome");
                        usuario.Email = dataReader.GetString("Email");
                        usuario.Sexo = dataReader.GetString("Sexo");
                        usuario.RG = dataReader.GetString("RG");
                        usuario.CPF = dataReader.GetString("CPF");
                        usuario.NomeMae = dataReader.GetString("NomeMae");
                        usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                        usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                        Contato contato = new Contato();
                        contato.Id = dataReader.GetInt32(9);
                        contato.UsuarioId = usuario.Id;
                        contato.Telefone = dataReader.GetString("Telefone");
                        contato.Celular = dataReader.GetString("Celular");

                        usuario.Contato = contato;

                        dUsuario.Add(usuario.Id, usuario);
                    }
                    else
                    {
                        usuario = dUsuario[dataReader.GetInt32(0)];
                    }

                    EnderecoEntrega enderecoEntrega = new EnderecoEntrega();
                    enderecoEntrega.Id = dataReader.GetInt32(13);
                    enderecoEntrega.UsuarioId = usuario.Id;
                    enderecoEntrega.NomeEndereco = dataReader.GetString("NomeEndereco");
                    enderecoEntrega.CEP = dataReader.GetString("CEP");
                    enderecoEntrega.Estado = dataReader.GetString("Estado");
                    enderecoEntrega.Cidade = dataReader.GetString("Cidade");
                    enderecoEntrega.Bairro = dataReader.GetString("Bairro");
                    enderecoEntrega.Endereco = dataReader.GetString("Endereco");
                    enderecoEntrega.Numero = dataReader.GetString("Numero");
                    enderecoEntrega.Complemento = dataReader.GetString("Complemento");

                    if (usuario.EnderecosEntrega == null)
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                    else
                        usuario.EnderecosEntrega = usuario.EnderecosEntrega;

                    if (usuario.EnderecosEntrega.FirstOrDefault(x => x.Id == enderecoEntrega.Id) == null)
                        usuario.EnderecosEntrega.Add(enderecoEntrega);

                    Departamento departamento = new Departamento();
                    departamento.Id = dataReader.GetInt32(26);
                    departamento.Nome = dataReader.GetString(27);

                    if (usuario.Departamentos == null)
                        usuario.Departamentos = new List<Departamento>();
                    else
                        usuario.Departamentos = usuario.Departamentos;

                    if (usuario.Departamentos.FirstOrDefault(x => x.Id == departamento.Id) == null)
                        usuario.Departamentos.Add(departamento);
                }
                return dUsuario[dUsuario.Keys.First()];
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand($"INSERT INTO [Usuarios](Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES(@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); SELECT CAST(scope_identity() AS int)");
                cmd.Transaction = transaction;
                cmd.Connection = _connection;

                cmd.Parameters.AddWithValue("@Nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@Email", usuario.Email);
                cmd.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                cmd.Parameters.AddWithValue("@RG", usuario.RG);
                cmd.Parameters.AddWithValue("@CPF", usuario.CPF);
                cmd.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                cmd.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                cmd.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);

                usuario.Id = (int)cmd.ExecuteScalar();

                cmd.CommandText = "INSERT INTO [Contatos](UsuarioId, Telefone, Celular) VALUES(@UsuarioId, @Telefone, @Celular); SELECT CAST(scope_identity() AS int)";
                cmd.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                cmd.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                cmd.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);

                usuario.Contato.UsuarioId = usuario.Id;
                usuario.Contato.Id = (int)cmd.ExecuteScalar();


                foreach (var endereco in usuario.EnderecosEntrega)
                {
                    cmd = new SqlCommand();
                    cmd.Transaction = transaction;
                    cmd.Connection = _connection;

                    cmd.CommandText = "INSERT INTO [EnderecosEntrega](UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES(@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento);SELECT CAST(scope_identity() AS int)";
                    cmd.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    cmd.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    cmd.Parameters.AddWithValue("@CEP", endereco.CEP);
                    cmd.Parameters.AddWithValue("@Estado", endereco.Estado);
                    cmd.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    cmd.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    cmd.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    cmd.Parameters.AddWithValue("@Numero", endereco.Numero);
                    cmd.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)cmd.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }

                foreach (var departamento in usuario.Departamentos)
                {
                    cmd = new SqlCommand();
                    cmd.Transaction = transaction;
                    cmd.Connection = _connection;
                    cmd.CommandText = "INSERT INTO [UsuariosDepartamentos](UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId);";
                    cmd.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    cmd.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                throw new Exception("Erro ao tentar inserir os dados!");
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            SqlTransaction transaction = (SqlTransaction)_connection.BeginTransaction();
            try
            {
                #region Usuario
                SqlCommand command = new SqlCommand();
                command.CommandText = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro=@DataCadastro WHERE Id = @Id";
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;

                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Sexo", usuario.Sexo);
                command.Parameters.AddWithValue("@RG", usuario.RG);
                command.Parameters.AddWithValue("@CPF", usuario.CPF);
                command.Parameters.AddWithValue("@NomeMae", usuario.NomeMae);
                command.Parameters.AddWithValue("@SituacaoCadastro", usuario.SituacaoCadastro);
                command.Parameters.AddWithValue("@DataCadastro", usuario.DataCadastro);

                command.Parameters.AddWithValue("@Id", usuario.Id);

                command.ExecuteNonQuery();
                #endregion
                #region Contato
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;

                command.CommandText = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";

                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                command.Parameters.AddWithValue("@Telefone", usuario.Contato.Telefone);
                command.Parameters.AddWithValue("@Celular", usuario.Contato.Celular);

                command.Parameters.AddWithValue("@Id", usuario.Contato.Id);

                command.ExecuteNonQuery();
                #endregion
                #region Endereço de Entrega
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @UsuarioId";
                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);

                command.ExecuteNonQuery();

                foreach (var endereco in usuario.EnderecosEntrega)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(scope_identity() AS int)";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@NomeEndereco", endereco.NomeEndereco);
                    command.Parameters.AddWithValue("@CEP", endereco.CEP);
                    command.Parameters.AddWithValue("@Estado", endereco.Estado);
                    command.Parameters.AddWithValue("@Cidade", endereco.Cidade);
                    command.Parameters.AddWithValue("@Bairro", endereco.Bairro);
                    command.Parameters.AddWithValue("@Endereco", endereco.Endereco);
                    command.Parameters.AddWithValue("@Numero", endereco.Numero);
                    command.Parameters.AddWithValue("@Complemento", endereco.Complemento);

                    endereco.Id = (int)command.ExecuteScalar();
                    endereco.UsuarioId = usuario.Id;
                }

                #endregion
                #region Departamentos
                command = new SqlCommand();
                command.Connection = (SqlConnection)_connection;
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @UsuarioId";
                command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                command.ExecuteNonQuery();

                foreach (var departamento in usuario.Departamentos)
                {
                    command = new SqlCommand();
                    command.Connection = (SqlConnection)_connection;
                    command.Transaction = transaction;

                    command.CommandText = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId);";
                    command.Parameters.AddWithValue("@UsuarioId", usuario.Id);
                    command.Parameters.AddWithValue("@DepartamentoId", departamento.Id);

                    command.ExecuteNonQuery();
                }
                #endregion
                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception)
                {
                    //Registrar no Log
                }
                throw new Exception("Erro não conseguimos atualizar os dados!");
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM [Usuarios] WHERE Id = @Id";
                cmd.Connection = _connection;
                cmd.Parameters.AddWithValue("@Id", id);

                _connection.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
