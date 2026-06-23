using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Application.SolicitudSvs;
using API.UnidadEmpleo.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                        /*
                        switch (indice)
                        {
                            case 0:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 1:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 2:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 3:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 4:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 5:
                                //
                                //if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            case 6:
                                if (!pA.GetValue(a).Equals(pB.GetValue(b)))
                                    return true;
                                break;
                            default:
                                break;
                        }
                        */
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

    }
}
