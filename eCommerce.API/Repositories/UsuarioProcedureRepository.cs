using eCommerce.API.Models;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace eCommerce.API.Repositories
{
    public class UsuarioProcedureRepository : IUsuarioRepository
    {
        private SqlConnection _connection;

        public UsuarioProcedureRepository()
        {
            _connection = new SqlConnection(@"Server=GUI-DESKTOP;Database=eCommerce;Integrated Security=True;TrustServerCertificate=True");
        }
        public List<Usuario> GetAll()
        {
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SelecionarUsuarios";

                _connection.Open();
                SqlDataReader dataReader = cmd.ExecuteReader();

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
                return usuarios;
            }
            finally
            {
                _connection.Close();
            }
        }

        public Usuario Get(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SelecionarUsuario";
                cmd.Connection = _connection;
                cmd.Parameters.AddWithValue("@id", id);

                _connection.Open();

                SqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.Id = dataReader.GetInt32(0);
                    usuario.Nome = dataReader.GetString("Nome");
                    usuario.Email = dataReader.GetString("Email");
                    usuario.Sexo = dataReader.GetString("Sexo");
                    usuario.RG = dataReader.GetString("RG");
                    usuario.CPF = dataReader.GetString("CPF");
                    usuario.NomeMae = dataReader.GetString("NomeMae");
                    usuario.SituacaoCadastro = dataReader.GetString("SituacaoCadastro");
                    usuario.DataCadastro = dataReader.GetDateTimeOffset(8);

                    return usuario;
                }
            }
            finally
            {
                _connection.Close();
            }
            return null;
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CadastrarUsuario";

                cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@sexo", usuario.Sexo);
                cmd.Parameters.AddWithValue("@rg", usuario.RG);
                cmd.Parameters.AddWithValue("@cpf", usuario.CPF);
                cmd.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                cmd.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                cmd.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);

                usuario.Id = (int)cmd.ExecuteScalar();
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AtualizarUsuario";
                cmd.Connection = _connection;

                cmd.Parameters.AddWithValue("@id", usuario.Id);
                cmd.Parameters.AddWithValue("@nome", usuario.Nome);
                cmd.Parameters.AddWithValue("@email", usuario.Email);
                cmd.Parameters.AddWithValue("@sexo", usuario.Sexo);
                cmd.Parameters.AddWithValue("@rg", usuario.RG);
                cmd.Parameters.AddWithValue("@cpf", usuario.CPF);
                cmd.Parameters.AddWithValue("@nomeMae", usuario.NomeMae);
                cmd.Parameters.AddWithValue("@situacaoCadastro", usuario.SituacaoCadastro);
                cmd.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);

                cmd.ExecuteNonQuery();
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
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeletarUsuario";
                cmd.Connection = _connection;

                cmd.Parameters.AddWithValue("@id", id);

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
