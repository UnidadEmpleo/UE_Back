using API.Common.Util;
using API.Seguridad.Controllers;
using API.UnidadEmpleo.Application.AspiranteApp;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.UnidadEmpleo.Controllers
{
    public class PdfController : BaseApiController
    {
        
        [AllowAnonymous]
        [HttpGet("presolicitud/{idSolicitud}")]
        public async Task<IActionResult> Presolicitud(int idSolicitud)
        {
            
            var resultado = await Mediator.Send(
                new GetPresolicitud.Query
                {
                    IdSolicitud = idSolicitud                    
                });

            PresolicitudDto solicitud = resultado.Value;
            byte[] pdfRetorno = null;
            if (solicitud != null)
            {
                byte[] presol = await filePresolicitud(solicitud);
                byte[] refes= await fileReferencias(solicitud.ReferenciaPersonal);
                pdfRetorno = UnirPdfs(presol, refes);
                return File(pdfRetorno, "application/pdf", "PRESOLICITUD_"+solicitud.Curp+"_"+solicitud.Nombre+".pdf");
            }
            
            return File(pdfRetorno,"application/pdf","REFERENCIAS_PERSONALES.pdf");

        }

        private void EscribirCheck(Document doc, bool marcado, float x, float y)
        {
            if (marcado)
            {
                Escribir(doc, "X", x, y, 12);
            }
        }

        private void EscribirCirculo(Document doc, bool marcado, float x, float y)
        {
            if (marcado)
            {
                Escribir(doc, "X", x, y, 12);
            }
        }


        private void Escribir(Document doc,string texto,float x,float y,float size = 10)
        {
            if (texto == null)
                texto = "-";
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            doc.ShowTextAligned(
                new Paragraph(texto)
                    .SetFont(font)
                    .SetFontSize(size),
                x,
                y,
                1,
                iText.Layout.Properties.TextAlignment.LEFT,
                iText.Layout.Properties.VerticalAlignment.BOTTOM,
                0);
        }

        public ReferenciaDto mapperFrom(Referencia rf)
        {
            return new ReferenciaDto
            {
                ApellidoPaterno = rf.Apellido_Paterno,
                ApellidoMaterno = rf.Apellido_Materno,
                Nombre = rf.Nombre,
                Calle = rf.Calle,
                Numero = rf.numero,
                EntreCalles = rf.EntreCalles,
                Colonia = rf.Colonia,
                CodigoPostal = rf.CodigoPostal+"",
                Estado = rf.Estado,
                Municipio = rf.Municipio,
                Parentesco = Utils.ObtenerDescripcion(rf.Parentesco),
                Telefono = rf.TelefonoLocal
            };
        }

        public async Task<byte[]>  filePresolicitud(PresolicitudDto datos)
        {

            
            var plantilla = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "Templates",
                "PRESOLICITUD.pdf");

            using var memoria = new MemoryStream();
            using var reader = new PdfReader(plantilla);
            using var writer = new PdfWriter(memoria);
            //using var pdf = new PdfDocument(reader, writer);

            using (PdfDocument pdf = new PdfDocument(reader, writer))
            {

                var document = new Document(pdf);

                // COORDENADAS

                // Nombre
                float xApellidoP = 60;
                float xApellidoM = 240;
                float xNombre = 420;
                float yNombres = 660;

                // CURP
                float xCurp = 50;
                float yCurp = 630;

                // RFC
                //float xRfc = 300;
                //float yRfc = 625;

                // Fecha nacimiento
                float xDiaNacimiento = 447;
                float xMesNacimiento = 503;
                float xAnioNacimiento = 560;
                float yNacimiento = 622;

                // Número de prealta
                float xPrealta = 500;
                float yPrealta = 738;

                // Fecha solicitud
                float xDiaSolicitud = 465;
                float xMesSolicitud = 512;
                float xAnioSolicitud = 558;
                float yFechaSolicitud = 704;

                // Domicilio
                float xCalle = 60;
                float xnumero = 500;
                float yCalle = 540;

                float xEntreCalles = 55;
                float yEntreCalles = 521;

                float xColonia = 30;
                float xCP = 200;
                float xEstado = 310;
                float xMunicipio = 472;
                float yColonia = 500;

                // Celular
                float xCelular = 280;
                float yCelular = 450;
                float xCasa = 70;

                float xRecado = 475;
                float yTelefonos = 435;


                // ESCOLARIDAD

                float xEscolaridad = 100;
                float xDocumento = 409;
                float yEscolaridad = 408;


                // ÚLTIMO EMPLEO

                float xEmpresa = 300;
                float yEmpresa = 365;

                float xDescripcion = 60;
                float yDescripcion = 340;

                float xPuesto = 425;

                float xJefe = 98;
                float yJefe = 318;

                float xTelefonoEmpresa = 315;

                float xFechaInicio = 435;
                float xFechaFin = 520;


                // MOTIVO DE BAJA

                float xMotivo = 108;
                float yMotivo = 293;


                // POLICÍA

                float xGradoInicioPolicia = 180;
                float yGradoInicioPolicia = 275;

                float xGradoFinalPolicia = 189;
                float yGradoFinalPolicia = 262;


                // MILITAR

                float xGradoInicioMilitar = 465;
                float yGradoInicioMilitar = 275;

                float xGradoFinalMilitar = 470;
                float yGradoFinalMilitar = 262;

                // DATOS PERSONALES
                EscribirCirculo(document, true, 42, 682);
                EscribirCirculo(document, false, 81, 682);
                EscribirCirculo(document, false, 120, 682);
                EscribirCirculo(document, false, 159, 682);

                Escribir(document, datos.ApellidoPaterno ?? "", xApellidoP, yNombres, 12);
                Escribir(document, datos.ApellidoMaterno ?? "", xApellidoM, yNombres, 12);
                Escribir(document, datos.Nombre ?? "", xNombre, yNombres, 12);

                Escribir(document, datos.Curp ?? "", xCurp, yCurp, 12);
                //Escribir(document, datos.Rfc ?? "", xRfc, yRfc, 11);

                // Fecha de nacimiento
                Escribir(document,
                datos.FechaNacimiento.Day.ToString("00"),
                xDiaNacimiento,
                yNacimiento);

                Escribir(document,
                datos.FechaNacimiento.Month.ToString("00"),
                xMesNacimiento,
                yNacimiento);

                Escribir(document,
                datos.FechaNacimiento.Year.ToString(),
                xAnioNacimiento,
                yNacimiento);

                // DOMICILIO

                Escribir(document, datos.Calle ?? "", xCalle, yCalle);

                // En datos se llama "numero"
                Escribir(document, datos.Numero ?? "", xnumero, yCalle);

                Escribir(document, datos.EntreCalles ?? "", xEntreCalles, yEntreCalles);

                Escribir(document, datos.Colonia ?? "", xColonia, yColonia);

                Escribir(document,
                    datos.CodigoPostal.ToString(),
                    xCP,
                    yColonia);

                Escribir(document, datos.Estado ?? "", xEstado, yColonia);

                Escribir(document, datos.Municipio ?? "", xMunicipio, yColonia);

                // TELÉFONO

                Escribir(document,
                    datos.TelefonoCelular ?? "",
                    xCelular,
                    yCelular);

                Escribir(document, datos.NumeroPrealta, xPrealta, yPrealta, 12);

                //=========================================
                // FECHA SOLICITUD
                //=========================================

                Escribir(document, datos.FechaSolicitud.Day.ToString("00"), xDiaSolicitud, yFechaSolicitud);
                Escribir(document, datos.FechaSolicitud.Month.ToString("00"), xMesSolicitud, yFechaSolicitud);
                Escribir(document, datos.FechaSolicitud.Year.ToString(), xAnioSolicitud, yFechaSolicitud);

                //=========================================
                // SEXO
                //=========================================

                var sexo = datos.Sexo?.ToUpper() ?? "";

                EscribirCirculo(document,
                    sexo.StartsWith("F") || sexo == "0",
                    255,
                    628);

                EscribirCirculo(document,
                    sexo.StartsWith("M") || sexo == "1",
                    265,
                    628);

                //=========================================
                // ESTADO CIVIL
                //=========================================

                EscribirCirculo(document, datos.EstadoCivil == "Soltero", 107, 601);
                EscribirCirculo(document, datos.EstadoCivil == "Casado", 152, 601);
                EscribirCirculo(document, datos.EstadoCivil == "Viudo", 208, 601);
                EscribirCirculo(document, datos.EstadoCivil == "UnionLibre", 275, 601);
                EscribirCirculo(document, datos.EstadoCivil == "Divorciado", 380, 601);

                //=========================================
                // CORPORACIÓN
                //=========================================

                float xCorporacion = 41;
                float yCorporacion = 710;


                //=========================================
                // CÓMO SE ENTERÓ
                //=========================================

                float xReingreso = 280;
                float yReingreso = 591;

                float xVolante = 400;
                float yVolante = 591;

                float xReclutador = 457;
                float yReclutador = 591;

                float xEmpresaParticular = 580;
                float yEmpresaParticular = 591;

                float xCentro = 111;
                float yCentro = 582;

                float xIniciativa = 245;
                float yIniciativa = 582;

                float xFeria = 340;
                float yFeria = 582;

                float xBolsa = 402;
                float yBolsa = 582;

                float xConocido = 580;
                float yConocido = 582;
                //=========================================
                // PENSIONADO
                //=========================================

                EscribirCirculo(document,
                    datos.PensionadoISSEMYM,
                    712,
                    597);

                // corpo
                Escribir(document, datos.CorporacionAlias, xCorporacion, yCorporacion, 10);

                //=========================================
                // COMO SE ENTERÓ
                //=========================================

                EscribirCirculo(document, datos.MedioEntero == 1, xReingreso, yReingreso);
                EscribirCirculo(document, datos.MedioEntero == 2, xVolante, yVolante);
                EscribirCirculo(document, datos.MedioEntero == 3, xReclutador, yReclutador);
                EscribirCirculo(document, datos.MedioEntero == 4, xEmpresaParticular, yEmpresaParticular);

                EscribirCirculo(document, datos.MedioEntero == 5, xCentro, yCentro);
                EscribirCirculo(document, datos.MedioEntero == 6, xIniciativa, yIniciativa);
                EscribirCirculo(document, datos.MedioEntero == 7, xFeria, yFeria);
                EscribirCirculo(document, datos.MedioEntero == 8, xBolsa, yBolsa);
                EscribirCirculo(document, datos.MedioEntero == 9, xConocido, yConocido);

                //=========================================
                // TELEFONOS
                //=========================================

                Escribir(document, datos.TelefonoCasa, xCasa, yCelular);
                Escribir(document, datos.TelefonoCelular, xCelular, yCelular);
                Escribir(document, datos.TelefonoRecado, xRecado, yCelular);

                //=========================================
                // ESCOLARIDAD
                //=========================================

                Escribir(document,
                    datos.Escolaridad,
                    xEscolaridad,
                    yEscolaridad);

                Escribir(document,
                    datos.DocumentoEscolaridad,
                    xDocumento,
                    yEscolaridad);

                EscribirCirculo(document,
                    datos.Concluida,
                    349,
                    408);

                EscribirCirculo(document,
                    datos.Trunca,
                    485,
                    388);

                //=========================================
                // ÚLTIMO EMPLEO
                //=========================================

                EscribirCirculo(document, datos.Gobierno, 70, 360);
                EscribirCirculo(document, datos.Privada, 173, 360);

                Escribir(document,
                    datos.Empresa,
                    xEmpresa,
                    yEmpresa);

                Escribir(document,
                    datos.DescripcionEmpresa,
                    xDescripcion,
                    yDescripcion);

                Escribir(document,
                    datos.Puesto,
                    xPuesto,
                    yDescripcion);

                Escribir(document,
                    datos.JefeInmediato,
                    xJefe,
                    yJefe);

                Escribir(document,
                    datos.TelefonoEmpresa,
                    xTelefonoEmpresa,
                    yJefe);

                //=========================================
                // FECHAS EMPLEO
                //=========================================

                Escribir(document,
                    datos.FechaIngreso.ToString("dd/MM/yyyy"),
                    xFechaInicio,
                    yJefe);

                Escribir(document,
                    datos.FechaSalida.ToString("dd/MM/yyyy"),
                    xFechaFin,
                    yJefe);

                //=========================================
                // MOTIVO BAJA
                //=========================================

                Escribir(document,
                    datos.MotivoBaja,
                    xMotivo,
                    yMotivo);

                //=========================================
                // POLICIA
                //=========================================

                EscribirCirculo(document,
                    datos.Policia,
                    85,
                    260);

                Escribir(document,
                    datos.GradoInicialPolicia,
                    xGradoInicioPolicia,
                    yGradoInicioPolicia);

                Escribir(document,
                    datos.GradoFinalPolicia,
                    xGradoFinalPolicia,
                    yGradoFinalPolicia);

                //=========================================
                // MILITAR
                //=========================================

                EscribirCirculo(document,
                    datos.Militar,
                    370,
                    260);

                Escribir(document,
                    datos.GradoInicialMilitar,
                    xGradoInicioMilitar,
                    yGradoInicioMilitar,
                    8);

                Escribir(document,
                    datos.GradoFinalMilitar,
                    xGradoFinalMilitar,
                    yGradoFinalMilitar,
                    8);

                //=========================================
                // DOCUMENTOS
                //=========================================

                // Columna izquierda
                EscribirCheck(document, datos.TarjetaEnvio, 29, 222);
                EscribirCheck(document, datos.Presolicitud, 29, 208);
                EscribirCheck(document, datos.Fotografias, 29, 196);
                EscribirCheck(document, datos.Croquis, 29, 162);
                EscribirCheck(document, datos.Referencias, 29, 149);
                EscribirCheck(document, datos.Dependientes, 29, 129);
                EscribirCheck(document, datos.Cartilla, 29, 116);
                EscribirCheck(document, datos.Certificado, 29, 103);

                // Columna derecha
                EscribirCheck(document, datos.ActaNacimiento, 325, 222);
                EscribirCheck(document, datos.NoPenales, 325, 208);
                EscribirCheck(document, datos.Comprobante, 325, 197);
                EscribirCheck(document, datos.Cartas, 325, 168);
                EscribirCheck(document, datos.CurpActualizada, 325, 149);
                EscribirCheck(document, datos.Ine, 325, 135);
                EscribirCheck(document, datos.Rfc, 325, 122);
            }
            

            return memoria.ToArray();
        }


        public async Task<byte[]> fileReferencias(List<Referencia> referenciaPersonal)
        {   
            var datos = new ReferenciasPersonalesDto();
            foreach (var r in referenciaPersonal)
            {
                if (r.Parentesco == Parentesco.Padre || r.Parentesco == Parentesco.Madre ||
                    r.Parentesco == Parentesco.Esposa || r.Parentesco == Parentesco.Hermano)
                    datos.Referencia1 = mapperFrom(r);

                if (r.Parentesco == Parentesco.Tio || r.Parentesco == Parentesco.Primo ||
                    r.Parentesco == Parentesco.Sobrino || r.Parentesco == Parentesco.Cuñado ||
                    r.Parentesco == Parentesco.Abuelo || r.Parentesco == Parentesco.Suegro)
                    datos.Referencia2 = mapperFrom(r);

                if (r.Parentesco == Parentesco.Amigo || r.Parentesco == Parentesco.Vecino ||
                    r.Parentesco == Parentesco.Conocido)
                    datos.Referencia3 = mapperFrom(r);
            }

            // Fin de datos ficticios

            var plantilla = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "Templates",
                "REFERENCIAS_PERSONALES.pdf");

            using var memoria = new MemoryStream();
            using var reader = new PdfReader(plantilla);
            using var writer = new PdfWriter(memoria);

            //using var pdf = new PdfDocument(reader, writer);
            using (PdfDocument pdf = new PdfDocument(reader, writer))
            {
                var document = new Document(pdf);
                DibujarReferencia(document, datos.Referencia1, 680);
                DibujarReferencia(document, datos.Referencia2, 445);
                DibujarReferencia(document, datos.Referencia3, 210);
            }

            return memoria.ToArray();              
        }


        private byte[] UnirPdfs(
                byte[] pdf1,
                byte[] pdf2)
        {
            using MemoryStream result = new();

            using PdfDocument destino = new(new PdfWriter(result));

            PdfMerger merger = new(destino);

            using PdfDocument origen1 = new(new PdfReader(new MemoryStream(pdf1)));

            merger.Merge(origen1,1,origen1.GetNumberOfPages());

            using PdfDocument origen2 =new(new PdfReader(new MemoryStream(pdf2)));

            merger.Merge(origen2,1,origen2.GetNumberOfPages());

            destino.Close();

            return result.ToArray();
        }




        [AllowAnonymous]
        [HttpGet("referencias/{idSolicitud}")]
        public async Task<IActionResult> Referencias(int idSolicitud)
        {
            var resultado = await Mediator.Send(
                new GetPresolicitud.Query
                {
                    IdSolicitud = idSolicitud
                });

            var solicitud = resultado.Value;
            var datos = new ReferenciasPersonalesDto();

            //PdfDocument mss = await fileReferencias(solicitud.ReferenciaPersonal);

            FileContentResult retorno = null;
            //if (mss.GetWriter()?.GetOutputStream() is MemoryStream ms) return File(ms.ToArray(),"application/pdf","PRESOLICITUD_" + solicitud.Curp + ".pdf");
            //referencias.Close();
            return null;
        }
        private void DibujarReferencia(Document document,ReferenciaDto r,float y)
        {
            //==========================
            // NOMBRE
            //==========================

            Escribir(document, r.ApellidoPaterno, 40, y - 10, 12);
            Escribir(document, r.ApellidoMaterno, 230, y - 10, 12);
            Escribir(document, r.Nombre, 410, y - 10, 12);

            //==========================
            // DOMICILIO
            //==========================

            Escribir(document, r.Calle, 40, y - 60, 12);
            Escribir(document, r.Numero, 500, y - 60, 12);

            Escribir(document, r.EntreCalles, 210, y - 90, 12);

            Escribir(document, r.Colonia, 20, y - 130, 12);

            Escribir(document, r.CodigoPostal, 200, y - 130, 12);

            Escribir(document, r.Estado, 290, y - 130, 12);

            Escribir(document, r.Municipio, 460, y - 130, 12);

            //==========================
            // PARENTESCO
            //==========================

            Escribir(document, r.Parentesco, 40, y - 180, 12);

            //==========================
            // TELÉFONO
            //==========================

            Escribir(document, r.Telefono, 410, y - 180, 12);
        }


    }


}