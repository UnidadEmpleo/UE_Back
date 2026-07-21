namespace API.UnidadEmpleo.DTO
{
    public class IndicadorDto
    {
        public int key { get; set; }
        public string label { get; set; }
        public int value { get; set; }
        public double amount { get; set; }
        public string suffix { get; set; }
        public bool mainIsCurrency { get; set; } = false;
        public string meta { get; set;}

    }
}