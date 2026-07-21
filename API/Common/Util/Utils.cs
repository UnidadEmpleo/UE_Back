using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Application.SolicitudSvs;
using API.UnidadEmpleo.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;

namespace API.Common.Util
{
    public class Utils
    {
        public static Boolean Compara(SolicitudUpdate.Command a, Solicitud b)
        {
            Boolean retorno = false;

            PropertyInfo[] propA = typeof(SolicitudUpdate.Command).GetProperties();
            PropertyInfo[] propB = typeof(Solicitud).GetProperties();
            string[] tipos = { "DateOnly", "StatusExpediente", "StatusSolicitud", "EnteraEmpleo", "Boolean", "String", "Int32" };
            foreach (PropertyInfo pA in propA)
            {
                foreach (PropertyInfo pB in propB)
                {
                    if (pA.Name.Equals(pB.Name))
                    {

                        string pt = pA.PropertyType.ToString();
                        int indice = Array.IndexOf(tipos, pt.Split(".")[1]);

                        if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                            return true;                       
                    }
                }
            }
            return false;
        }


        public static Boolean ComparaCambiosAspirante(AspiranteUpdate.Command a, Aspirante b)
        {
            Boolean retorno = false;

            PropertyInfo[] propA = typeof(AspiranteUpdate.Command).GetProperties();
            PropertyInfo[] propB = typeof(Aspirante).GetProperties();
            string[] tipos = { "DateOnly", "StatusExpediente", "StatusSolicitud", "EnteraEmpleo", "Boolean", "String", "Int32" };
            foreach (PropertyInfo pA in propA)
            {
                foreach (PropertyInfo pB in propB)
                {
                    if (pA.Name.Equals(pB.Name))
                    {

                        string pt = pA.PropertyType.ToString();
                        int indice = Array.IndexOf(tipos, pt.Split(".")[1]);

                        if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                            return true;
                    }

                }
            }
            return false;
        }


        public static string ObtenerDescripcion(Enum valor)
        {
            if (valor == null)
                throw new ArgumentNullException(nameof(valor));

            // Obtiene la información del campo del enum
            FieldInfo campo = valor.GetType().GetField(valor.ToString());

            if (campo != null)
            {
                // Busca el atributo Description
                DescriptionAttribute atributo =
                    (DescriptionAttribute)Attribute.GetCustomAttribute(campo, typeof(DescriptionAttribute));

                if (atributo != null)
                    return atributo.Description;
            }

            // Si no hay atributo, devuelve el nombre del enum
            return valor.ToString();
        }
    }
}
