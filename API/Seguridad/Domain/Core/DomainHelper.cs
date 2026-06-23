using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Seguridad.Domain.Core
{
    public class DomainHelper
    {
        /// <summary>
        /// Genera el RFC conforme a las últimas regulaciones del SAT en México.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente</param>
        /// <param name="primerApellido">Primer apellido</param>
        /// <param name="segundoApellido">Segundo apellido (puede ser null)</param>
        /// <param name="fechaNacimiento">Fecha de nacimiento</param>
        /// <returns>RFC generado</returns>
        public static string GenerarRFC(
            string nombre,
            string primerApellido,
            string? segundoApellido,
            DateTime fechaNacimiento)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(primerApellido))
                throw new ArgumentException("El nombre y el primer apellido son obligatorios.");

            // Eliminar acentos y caracteres especiales
            string nombreNorm = RemoverAcentos(nombre.Trim().ToUpper());
            string apellidoPaternoNorm = RemoverAcentos(primerApellido.Trim().ToUpper());
            string apellidoMaternoNorm = string.IsNullOrWhiteSpace(segundoApellido) ? "" : RemoverAcentos(segundoApellido.Trim().ToUpper());

            // Reglas para nombres compuestos y palabras prohibidas
            nombreNorm = AjustarNombre(nombreNorm);

            // Primer letra y primera vocal interna del apellido paterno
            var rfc = new StringBuilder();
            rfc.Append(ObtenerLetraApellidoPaterno(apellidoPaternoNorm));

            // Primera letra del apellido materno (o X si no hay)
            rfc.Append(string.IsNullOrEmpty(apellidoMaternoNorm) ? "X" : apellidoMaternoNorm.Substring(0, 1));

            // Primera letra del nombre
            rfc.Append(nombreNorm.Substring(0, 1));

            // Fecha de nacimiento en formato: aaMMDD
            rfc.Append(fechaNacimiento.ToString("yyMMdd", CultureInfo.InvariantCulture));

            // Homoclave simplificada
            rfc.Append("XXX");

            return rfc.ToString().ToUpper();
        }

        /// <summary>
        /// Genera el CURP conforme a las últimas regulaciones del RENAPO/SAT en México.
        /// </summary>
        /// <param name="nombre">Nombre(s) del paciente</param>
        /// <param name="primerApellido">Primer apellido</param>
        /// <param name="segundoApellido">Segundo apellido (puede ser null)</param>
        /// <param name="fechaNacimiento">Fecha de nacimiento</param>
        /// <param name="sexo">Sexo: 'H' (hombre) o 'M' (mujer)</param>
        /// <param name="entidadNacimiento">Clave de la entidad federativa de nacimiento (2 letras, ej. 'DF', 'NL')</param>
        /// <returns>CURP generado</returns>
        public static string GenerarCURP(
            string nombre,
            string primerApellido,
            string? segundoApellido,
            DateTime fechaNacimiento,
            char sexo,
            string entidadNacimiento)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(primerApellido) ||
                string.IsNullOrWhiteSpace(entidadNacimiento) ||
                (sexo != 'H' && sexo != 'M'))
                throw new ArgumentException("Nombre, primer apellido, sexo y entidad de nacimiento son obligatorios.");

            // Normalización
            string nombreNorm = RemoverAcentos(nombre.Trim().ToUpper());
            string apellidoPaternoNorm = RemoverAcentos(primerApellido.Trim().ToUpper());
            string apellidoMaternoNorm = string.IsNullOrWhiteSpace(segundoApellido) ? "" : RemoverAcentos(segundoApellido.Trim().ToUpper());
            string entidadNorm = entidadNacimiento.Trim().ToUpper();

            // Reglas para nombres compuestos
            nombreNorm = AjustarNombre(nombreNorm);

            // Usar StringBuilder para curp
            var curp = new StringBuilder();

            // 1. Primera letra y primera vocal interna del primer apellido
            curp.Append(ObtenerLetraApellidoPaternoCURP(apellidoPaternoNorm));

            // 2. Primera letra del segundo apellido (o X si no hay)
            curp.Append(string.IsNullOrEmpty(apellidoMaternoNorm) ? "X" : apellidoMaternoNorm.Substring(0, 1));

            // 3. Primera letra del nombre
            curp.Append(nombreNorm.Substring(0, 1));

            // 4. Fecha de nacimiento: aaMMDD
            curp.Append(fechaNacimiento.ToString("yyMMdd", CultureInfo.InvariantCulture));

            // 5. Sexo
            curp.Append(sexo);

            // 6. Entidad federativa
            curp.Append(entidadNorm);

            // 7. Primera consonante interna del primer apellido
            curp.Append(ObtenerPrimeraConsonanteInterna(apellidoPaternoNorm));

            // 8. Primera consonante interna del segundo apellido
            curp.Append(string.IsNullOrEmpty(apellidoMaternoNorm) ? "X" : ObtenerPrimeraConsonanteInterna(apellidoMaternoNorm));

            // 9. Primera consonante interna del nombre
            curp.Append(ObtenerPrimeraConsonanteInterna(nombreNorm));

            // 10. Diferenciador de siglo y homoclave (simplificado: '00')
            curp.Append(fechaNacimiento.Year >= 2000 ? "A0" : "00");

            return curp.ToString().ToUpper();
        }

        private static string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace("Ñ", "X")
                .Replace("Ü", "U");
        }

        private static string AjustarNombre(string nombre)
        {
            var nombres = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nombres.Length > 1)
            {
                var compuestos = new[] { "JOSE", "MARIA", "MA", "J", "M" };
                if (compuestos.Contains(nombres[0]))
                    return nombres[1];
            }
            return nombres[0];
        }

        private static string ObtenerLetraApellidoPaterno(string apellidoPaterno)
        {
            string rfc = apellidoPaterno.Substring(0, 1);
            var match = Regex.Match(apellidoPaterno.Substring(1), "[AEIOU]");
            rfc += match.Success ? match.Value : "X";
            return rfc;
        }

        private static string ObtenerLetraApellidoPaternoCURP(string apellidoPaterno)
        {
            string curp = apellidoPaterno.Substring(0, 1);
            var match = Regex.Match(apellidoPaterno.Substring(1), "[AEIOU]");
            curp += match.Success ? match.Value : "X";
            return curp;
        }

        private static string ObtenerPrimeraConsonanteInterna(string texto)
        {
            var match = Regex.Match(texto.Substring(1), "[BCDFGHJKLMNPQRSTVWXYZ]");
            return match.Success ? match.Value : "X";
        }
    }
}
