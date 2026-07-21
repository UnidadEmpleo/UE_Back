using API.Seguridad.Application.Core;
using System.ComponentModel.DataAnnotations;

namespace API.Seguridad.Domain.Seguridad
{
    public class BaseDatos
    {
        public short Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        [MaxLength(250)]
        public string Descripcion { get; set; }
        public string? ServerName { get; set; }
        public string? DatabaseName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Port { get; set; }
        public Boolean TrustServerCertificate { get; set; }

        public string? ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(ServerName) || string.IsNullOrEmpty(DatabaseName))
                {
                    return null; // Devuelve null si ServerName o DatabaseName no están definidos
                }

                var puerto = !Port.IsNullOrWhiteSpace() ? $",{Port}" : "";
                var connectionString = $"Server={ServerName}{puerto};Initial Catalog={DatabaseName};";

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    connectionString += $"User Id={UserName};Password={Password};TrustServerCertificate={TrustServerCertificate};";
                }
                else
                {
                    connectionString += "Trusted_Connection=True;TrustServerCertificate=True;";
                }

                return connectionString;
            }
        }
    }
}
