namespace API.UnidadEmpleo.DTO
{
    public class UltimoEmpleoDto
    {
        public int IdSoliciud { get; set; }
        public Boolean Gobierno { get; set; }
        public Boolean Privada { get; set; }
        public string NombreEmpresa { get; set; }
        public string DescripcionEmpresa { get; set; }
        public string Puesto { get; set; }
        public string JefeInmediato { get; set; }
        public string TelefonoEmpleo { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFinal { get; set; }
        public string MotivoBaja { get; set; }

        public Boolean Policia { get; set; }
        public string GradoInicioPolicia { get; set; }
        public string GradoFinalPolicia { get; set; }


        public Boolean Militar { get; set; }
        public string GradoInicioMilitar { get; set; }
        public string GradoFinalMilitar { get; set; }
    }
}
