using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Seguridad.Domain.Core
{
    public class CalendarioSchema
    {
        [JsonPropertyName("fechaInicio")]
        public DateOnly FechaInicio { get; set; }

        [JsonPropertyName("fechaTermino")]
        public DateOnly FechaTermino { get; set; }

        [JsonPropertyName("Especialidades")]
        public List<EspecialidadSchema> Especialidades { get; set; } = [];
    }

    public class EspecialidadSchema
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("horario")]
        public HorarioSchema Horario { get; set; } = new();
    }

    public class HorarioSchema
    {
        [JsonPropertyName("mon")]
        public DiaHorarioSchema Lunes { get; set; } = new();

        [JsonPropertyName("tue")]
        public DiaHorarioSchema Martes { get; set; } = new();

        [JsonPropertyName("wed")]
        public DiaHorarioSchema Miercoles { get; set; } = new();

        [JsonPropertyName("thu")]
        public DiaHorarioSchema Jueves { get; set; } = new();

        [JsonPropertyName("fri")]
        public DiaHorarioSchema Viernes { get; set; } = new();

        [JsonPropertyName("sat")]
        public DiaHorarioSchema Sabado { get; set; } = new();

        [JsonPropertyName("sun")]
        public DiaHorarioSchema Domingo { get; set; } = new();
    }

    public class DiaHorarioSchema
    {
        [JsonPropertyName("available")]
        public bool Available { get; set; }

        [JsonPropertyName("intervals")]
        public List<IntervaloSchema> Intervals { get; set; } = [];
    }

    public class IntervaloSchema
    {
        [JsonPropertyName("start")]
        public string Start { get; set; } = string.Empty;

        [JsonPropertyName("end")]
        public string End { get; set; } = string.Empty;
    }

    public static class JsonSchemas
    {
        public static string SerializeCalendario(CalendarioSchema calendario)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(calendario, options);
        }

        public static CalendarioSchema? DeserializeCalendario(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<CalendarioSchema>(json, options);
        }

        // ---- Nuevos helpers para Consulta Médica ----
        public static string SerializeConsultaMedica(ConsultaMedicaSchema consulta)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(consulta, options);
        }

        public static ConsultaMedicaSchema? DeserializeConsultaMedica(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ConsultaMedicaSchema>(json, options);
        }

        // ---- Helpers para Receta Médica ----
        public static string SerializeRecetaMedica(RecetasMedicasSchema receta)
        {
            // Mantener nombres exactos con mayúsculas según el esquema esperado
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(receta, options);
        }

        public static RecetasMedicasSchema? DeserializeRecetaMedica(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<RecetasMedicasSchema>(json, options);
        }
    }

    public static class CalendarioSchemaValidator
    {
        public static bool TryValidate(CalendarioSchema calendario, out List<string> errors)
        {
            errors = new List<string>();

            if (calendario.FechaInicio > calendario.FechaTermino)
                errors.Add("La fecha de inicio no puede ser posterior a la fecha de término.");

            if (calendario.Especialidades == null || !calendario.Especialidades.Any())
                errors.Add("Debe haber al menos una especialidad.");

            foreach (var especialidad in calendario.Especialidades)
            {
                if (especialidad.Id <= 0)
                    errors.Add("El ID de la especialidad debe ser mayor que cero.");

                if (especialidad.Horario == null)
                    errors.Add($"La especialidad con ID {especialidad.Id} no tiene horario.");

                var dias = new[]
                {
                    especialidad.Horario.Lunes,
                    especialidad.Horario.Martes,
                    especialidad.Horario.Miercoles,
                    especialidad.Horario.Jueves,
                    especialidad.Horario.Viernes,
                    especialidad.Horario.Sabado,
                    especialidad.Horario.Domingo
                };

                foreach (var dia in dias)
                {
                    if (dia.Available && (dia.Intervals == null || !dia.Intervals.Any()))
                        errors.Add($"Día disponible sin intervalos definidos en especialidad {especialidad.Id}.");

                    foreach (var intervalo in dia.Intervals)
                    {
                        if (string.IsNullOrWhiteSpace(intervalo.Start) || string.IsNullOrWhiteSpace(intervalo.End))
                            errors.Add($"Intervalo con hora de inicio o fin vacía en especialidad {especialidad.Id}.");

                        if (!IsValidTimeFormat(intervalo.Start))
                            errors.Add($"Formato de hora inválido en 'start' ({intervalo.Start}) en especialidad {especialidad.Id}.");
                        if (!IsValidTimeFormat(intervalo.End))
                            errors.Add($"Formato de hora inválido en 'end' ({intervalo.End}) en especialidad {especialidad.Id}.");

                        if (IsValidTimeFormat(intervalo.Start) && IsValidTimeFormat(intervalo.End))
                        {
                            var start = TimeOnly.ParseExact(intervalo.Start, "HH:mm");
                            var end = TimeOnly.ParseExact(intervalo.End, "HH:mm");
                            if (start >= end)
                                errors.Add($"La hora de inicio debe ser menor que la hora de fin en especialidad {especialidad.Id}.");
                        }
                    }
                }
            }

            return errors.Count == 0;
        }

        private static bool IsValidTimeFormat(string time)
        {
            return TimeOnly.TryParseExact(time, "HH:mm", out _);
        }
    }

    // ---------------- Consulta Médica (regenerada según ejemplo JSON) ----------------
    public class ConsultaMedicaSchema
    {
        [JsonPropertyName("datosClinicos")]
        public DatosClinicosConsultaSchema DatosClinicos { get; set; } = new();

        [JsonPropertyName("antecedentes")]
        public AntecedentesConsultaSchema Antecedentes { get; set; } = new();

        [JsonPropertyName("exploracion")]
        public ExploracionConsultaSchema Exploracion { get; set; } = new();

        [JsonPropertyName("diagnosticoPronostico")]
        public DiagnosticoPronosticoConsultaSchema DiagnosticoPronostico { get; set; } = new();

        [JsonPropertyName("tratamientoCondiciones")]
        public TratamientoCondicionesConsultaSchema TratamientoCondiciones { get; set; } = new();
    }

    public class DatosClinicosConsultaSchema
    {
        [JsonPropertyName("fechaConsulta")]
        public DateOnly? FechaConsulta { get; set; }

        [JsonPropertyName("medicoAsignadoId")]
        public string? MedicoAsignadoId { get; set; }

        [JsonPropertyName("medicoAsignado")]
        public string? MedicoAsignado { get; set; }

        [JsonPropertyName("especialidadId")]
        public int? EspecialidadId { get; set; }

        [JsonPropertyName("especialidad")]
        public string? Especialidad { get; set; }

        [JsonPropertyName("clinicaId")]
        public int? ClinicaId { get; set; }

        [JsonPropertyName("clinica")]
        public string? Clinica { get; set; }

        // Valores modelados como string para reflejar exactamente el JSON de ejemplo
        [JsonPropertyName("talla")]
        public string? Talla { get; set; }

        [JsonPropertyName("peso")]
        public string? Peso { get; set; }

        [JsonPropertyName("imc")]
        public string? IMC { get; set; }

        [JsonPropertyName("glucosa")]
        public string? Glucosa { get; set; }

        [JsonPropertyName("pulso")]
        public string? Pulso { get; set; }

        [JsonPropertyName("presionArterial")]
        public string? PresionArterial { get; set; }

        [JsonPropertyName("frecuenciaCardiaca")]
        public string? FrecuenciaCardiaca { get; set; }

        [JsonPropertyName("temperatura")]
        public string? Temperatura { get; set; }

        [JsonPropertyName("saturacion")]
        public string? Saturacion { get; set; }

        [JsonPropertyName("descripcionPadecimientoHtml")]
        public string? DescripcionPadecimientoHtml { get; set; }

        [JsonPropertyName("seguimientoHtml")]
        public string? SeguimientoHtml { get; set; }

        [JsonPropertyName("observacionesHtml")]
        public string? ObservacionesHtml { get; set; }
    }

    public class AntecedentesConsultaSchema
    {
        [JsonPropertyName("antecedentesPatologicos")]
        public List<AntecedenteItemSchema> AntecedentesPatologicos { get; set; } = [];

        [JsonPropertyName("antecedentesNoPatologicos")]
        public List<AntecedenteItemSchema> AntecedentesNoPatologicos { get; set; } = [];

        [JsonPropertyName("antecedentesGinecoObstetricos")]
        public List<AntecedenteItemSchema> AntecedentesGinecoObstetricos { get; set; } = [];

        [JsonPropertyName("antecedentesFamiliares")]
        public List<AntecedenteDetalleFamiliarSchema> AntecedentesFamiliares { get; set; } = [];
    }

    public class AntecedenteItemSchema
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("fecha")]
        public DateOnly? Fecha { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Antecedente { get; set; }

        [JsonPropertyName("descripcionDelAntecedente")]
        public string? Descripcion { get; set; }
    }

    public class AntecedenteDetalleFamiliarSchema
    {
        [JsonPropertyName("fecha")]
        public DateOnly? Fecha { get; set; }

        [JsonPropertyName("antecedente")]
        public string? Antecedente { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("descripcionDelAntecedente")]
        public string? DescripcionDelAntecedente { get; set; }

        // Solo aplica para antecedentesFamiliares; es opcional
        [JsonPropertyName("parentesco")]
        public string? Parentesco { get; set; }
    }

    public class ExploracionConsultaSchema
    {
        [JsonPropertyName("exploracionFisica")]
        public string? ExploracionFisica { get; set; }

        [JsonPropertyName("indicacionesHigienodieteticas")]
        public string? IndicacionesHigienodieteticas { get; set; }

        [JsonPropertyName("observacionesExploracion")]
        public string? ObservacionesExploracion { get; set; }

        [JsonPropertyName("examenes")]
        public List<ExamenSolicitudSchema> Examenes { get; set; } = [];
    }

    public class ExamenSolicitudSchema
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("paquete")]
        public string? Paquete { get; set; }

        [JsonPropertyName("clave")]
        public string? Clave { get; set; }

        [JsonPropertyName("observaciones")]
        public string? Observaciones { get; set; }

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }
    }

    public class DiagnosticoPronosticoConsultaSchema
    {
        [JsonPropertyName("diagnostico")]
        public string? Diagnostico { get; set; }

        [JsonPropertyName("cieDiagnosticos")]
        public List<CieDiagnosticoSchema> CieDiagnosticos { get; set; } = [];

        [JsonPropertyName("pronostico")]
        public string? Pronostico { get; set; }
    }

    public class CieDiagnosticoSchema
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("cie10")]
        public string? Cie10 { get; set; }

        [JsonPropertyName("diagnostico")]
        public string? Diagnostico { get; set; }
    }

    public class TratamientoCondicionesConsultaSchema
    {
        [JsonPropertyName("medicamentos")]
        public List<MedicamentoTratamientoSchema> Medicamentos { get; set; } = [];

        // En el ejemplo vienen como string
        [JsonPropertyName("numeroReceta")]
        public string? NumeroReceta { get; set; }

        [JsonPropertyName("controlados")]
        public string? Controlados { get; set; }

        [JsonPropertyName("noControlados")]
        public string? NoControlados { get; set; }
    }

    public class MedicamentoTratamientoSchema
    {
        
        [JsonPropertyName("MedicamentoId")]
        public int? MedicamentoId { get; set; }

        [JsonPropertyName("sustanciaActiva")]
        public string? SustanciaActiva { get; set; }

        [JsonPropertyName("indicacion")]
        public string? Indicacion { get; set; }

        [JsonPropertyName("cantidad")]
        public int? Cantidad { get; set; }

        [JsonPropertyName("tipo")]
        public string? Tipo { get; set; }
    }

    // ---------------- Receta Médica ----------------
    public class RecetasMedicasSchema
    {
        [JsonPropertyName("Receta")]
        public List<RecetaSchema> Receta { get; set; } = [];
    }

    public class RecetaSchema
    {
        [JsonPropertyName("Folio")]
        public long Folio { get; set; }

        [JsonPropertyName("FolioClinica")]
        public string FolioClinica { get; set; }

        [JsonPropertyName("TipoReceta")]
        public TiposReceta TipoReceta { get; set; } = TiposReceta.Interna;

        [JsonPropertyName("Modificada")]
        public bool Modificada { get; set; } = false;

        [JsonPropertyName("Nueva")]
        public bool Nueva { get; set; } = false;

        [JsonPropertyName("Medicamentos")]
        public List<MedicamentoTratamientoSchema> Medicamentos { get; set; } = [];
    }

    public enum TiposReceta
    {
        Interna = 1,
        Externa = 2
    }
}
