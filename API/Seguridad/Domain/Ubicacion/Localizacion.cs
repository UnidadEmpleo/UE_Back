namespace API.Seguridad.Domain.Ubicacion
{
    public class Localizacion
    {
        public enum EstadosRepublica
        {
            NoDisponible = 0,
            [System.ComponentModel.DataAnnotations.Display(Name = "AGUASCALIENTES", Description = "AS")]
            Aguascalientes,
            [System.ComponentModel.DataAnnotations.Display(Name = "BAJA CALIFORNIA", Description = "BC")]
            BajaCalifornia,
            [System.ComponentModel.DataAnnotations.Display(Name = "BAJA CALIFORNIA SUR", Description = "BS")]
            BajaCaliforniaSur,
            [System.ComponentModel.DataAnnotations.Display(Name = "CAMPECHE", Description = "CC")]
            Campeche,
            [System.ComponentModel.DataAnnotations.Display(Name = "COAHUILA", Description = "CL")]
            Coahuila,
            [System.ComponentModel.DataAnnotations.Display(Name = "COLIMA", Description = "CM")]
            Colima,
            [System.ComponentModel.DataAnnotations.Display(Name = "CHIAPAS", Description = "CS")]
            Chiapas,
            [System.ComponentModel.DataAnnotations.Display(Name = "CHIHUAHUA", Description = "CH")]
            Chihuahua,
            [System.ComponentModel.DataAnnotations.Display(Name = "CIUDAD DE MÉXICO", Description = "DF")]
            CiudadDeMexico,
            [System.ComponentModel.DataAnnotations.Display(Name = "DURANGO", Description = "DG")]
            Durango,
            [System.ComponentModel.DataAnnotations.Display(Name = "GUANAJUATO", Description = "GT")]
            Guanajuato,
            [System.ComponentModel.DataAnnotations.Display(Name = "GUERRERO", Description = "GR")]
            Guerrero,
            [System.ComponentModel.DataAnnotations.Display(Name = "HIDALGO", Description = "HG")]
            Hidalgo,
            [System.ComponentModel.DataAnnotations.Display(Name = "JALISCO", Description = "JC")]
            Jalisco,
            [System.ComponentModel.DataAnnotations.Display(Name = "MÉXICO", Description = "MC")]
            Mexico,
            [System.ComponentModel.DataAnnotations.Display(Name = "MICHOACÁN", Description = "MN")]
            Michoacan,
            [System.ComponentModel.DataAnnotations.Display(Name = "MORELOS", Description = "MS")]
            Morelos,
            [System.ComponentModel.DataAnnotations.Display(Name = "NAYARIT", Description = "NT")]
            Nayarit,
            [System.ComponentModel.DataAnnotations.Display(Name = "NUEVO LEÓN", Description = "NL")]
            NuevoLeon,
            [System.ComponentModel.DataAnnotations.Display(Name = "OAXACA", Description = "OC")]
            Oaxaca,
            [System.ComponentModel.DataAnnotations.Display(Name = "PUEBLA", Description = "PL")]
            Puebla,
            [System.ComponentModel.DataAnnotations.Display(Name = "QUERÉTARO", Description = "QT")]
            Queretaro,
            [System.ComponentModel.DataAnnotations.Display(Name = "QUINTANA ROO", Description = "QR")]
            QuintanaRoo,
            [System.ComponentModel.DataAnnotations.Display(Name = "SAN LUIS POTOSÍ", Description = "SP")]
            SanLuisPotosi,
            [System.ComponentModel.DataAnnotations.Display(Name = "SINALOA", Description = "SL")]
            Sinaloa,
            [System.ComponentModel.DataAnnotations.Display(Name = "SONORA", Description = "SR")]
            Sonora,
            [System.ComponentModel.DataAnnotations.Display(Name = "TABASCO", Description = "TC")]
            Tabasco,
            [System.ComponentModel.DataAnnotations.Display(Name = "TAMAULIPAS", Description = "TS")]
            Tamaulipas,
            [System.ComponentModel.DataAnnotations.Display(Name = "TLAXCALA", Description = "TL")]
            Tlaxcala,
            [System.ComponentModel.DataAnnotations.Display(Name = "VERACRUZ", Description = "VZ")]
            Veracruz,
            [System.ComponentModel.DataAnnotations.Display(Name = "YUCATÁN", Description = "YN")]
            Yucatan,
            [System.ComponentModel.DataAnnotations.Display(Name = "ZACATECAS", Description = "ZS")]
            Zacatecas,
            [System.ComponentModel.DataAnnotations.Display(Name = "EXTRANJERO", Description = "EX")]
            Extranjero
        }
    }
}
