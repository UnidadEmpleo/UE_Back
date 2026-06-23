using API.Common.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace API.UnidadEmpleo.Persistence
{
    public static class UnidadEmpleoDbInitializer
    {
        public static async Task SeedData(UnidadEmpleoDbContext context)
        {
                
            var g = new Region { Id = 0, region = "Gerencia" }; var r0 = new Region { Id = 100, region = "Región 0" };
            var r1 = new Region {Id=1,region="REGION  I"}; var r2 = new Region {Id=2,region="REGION  II"}; var r3 = new Region {Id=3,region="REGION  III"}; var r4 = new Region { Id = 4, region = "REGION  IV" };
            var r5 = new Region {Id=5,region="REGION  V"}; var r6 =new Region {Id=6,region="REGION  VI"}; var r7 = new Region {Id=7,region="REGION  VII"}; var r8 = new Region { Id = 8, region = "REGION  VIII" };
            var r9 = new Region {Id=9,region="REGION  IX"}; var r10 =new Region {Id=10,region="REGION  X"}; var r11 = new Region {Id=11,region="REGION  XI"}; var r12 = new Region { Id = 12, region = "REGION  XII" };
            var r13 = new Region {Id=13,region="REGION  XIII"}; var r14 =new Region {Id=14,region="REGION  XIV"}; var r15 = new Region {Id=15,region="REGION  XV"}; var r16 = new Region { Id = 16, region = "REGION  XVI" };
            var r17 = new Region {Id=17,region="REGION  XVII"}; var r18 =new Region {Id=18,region="REGION  XVIII"}; var r19 = new Region {Id=19,region="REGION  XIX"}; var r20 = new Region { Id = 20, region = "REGION  XX" };
            var r21 = new Region {Id=21,region="REGION  XXI"}; var r22 =new Region {Id=22,region="REGION  XXII"}; var r23 = new Region {Id=23,region="REGION  XXIII"}; var r24 = new Region { Id = 24, region = "REGION  XXIV" };
            var r25 = new Region {Id=25,region="REGION  XXV"}; var r26 =new Region {Id=26,region="REGION  XXVI"}; var r27 = new Region {Id=27,region="REGION  XXVII"}; var r28 = new Region { Id = 28, region = "REGION  XXVIII" };
            var r29 = new Region {Id=29,region="REGION  XXIX"}; var r30 =new Region {Id=30,region="REGION  XXX"}; var r31 = new Region {Id=31,region="REGION  XXXI"}; var r32 = new Region { Id = 32, region = "REGION  XXXII" };
            var r33 = new Region {Id=33,region="REGION  XXXIII"}; var r34 =new Region {Id=34,region="REGION  XXXIV"}; var r35 = new Region {Id=35,region="REGION  XXXV"}; var r36 = new Region { Id = 36, region = "REGION  XXXVI" };
            var r37 = new Region {Id=37,region="REGION  XXXVII"}; var r38 =new Region {Id=38,region="REGION  XXXVIII"}; var r39 = new Region {Id=39,region="REGION  XXXIX"}; var r40 = new Region { Id = 40, region = "REGION  XL" };
            var r41 = new Region {Id=41,region="REGION  XLI"}; var r42 =new Region {Id=42,region="REGION  XLII"}; var r43 = new Region {Id=43,region="REGION  XLIII"}; var r44 = new Region { Id = 44, region = "REGION  XLIV" };
            var r45 = new Region {Id=45,region="REGION  XLV"}; var r46 =new Region {Id=46,region="REGION  XLVI"}; var r47 = new Region {Id=47,region="REGION  XLVII"}; var r48 = new Region { Id = 48, region = "REGION  XLVIII" };
            var r49 = new Region {Id=49,region="REGION  XLIX"}; var r50 =new Region {Id=50,region="REGION  L"}; var r51 = new Region {Id=51,region="REGION  LI"}; var r52 = new Region { Id = 52, region = "REGION  LII" };
            var r53 = new Region {Id=53,region="REGION  LIII"}; var r54 =new Region {Id=54,region="REGION  LIV"}; var r55 = new Region {Id=55,region="REGION  LV"}; var r56 = new Region { Id = 56, region = "REGION  LVI" };
            var r57 = new Region {Id=57,region="REGION  LVII"}; var r58 =new Region {Id=58,region="REGION  LVIII"}; var r59 = new Region {Id=59,region="REGION  LIX"}; var r60 = new Region { Id = 60, region = "REGION  LX" };
            var r61 = new Region {Id=61,region="REGION  LXI"}; var r62 =new Region {Id=62,region="REGION  LXII"}; var r63 = new Region {Id=63,region="REGION  LXIII"}; var r64 = new Region { Id = 64, region = "REGION  LXIV" };
            var r65 = new Region {Id=65,region="REGION  LXV"}; var r66 =new Region {Id=66,region="REGION  LXVI"}; var r67 = new Region {Id=67,region="REGION  LXVII"}; var r68 = new Region { Id = 68, region = "REGION  LXVIII" };
            var r69 = new Region { Id = 69, region = "REGION  LXIX" };

            var regiones = new List<Region>
                {g,r0,r1,r2,r3,r4,r5,r6,r7,r18,r9,
                r10,r11,r12,r13,r14,r15,r16,r17,r18,r19,
                r20,r21,r22,r23,r24,r25,r26,r27,r28,r29,
                r30,r31,r32,r33,r34,r35,r36,r37,r38,r39,
                r40,r41,r42,r43,r44,r45,r46,r47,r48,r49,
                r50,r51,r52,r53,r54,r55,r56,r57,r58,r59,
                r60,r61,r62,r63,r64,r65,r66,r67,r68,r69,
            };

            /*if (!await context.Region.AnyAsync())
            {
                await context.Region.AddRangeAsync(regiones);
                await context.SaveChangesAsync();
            }
            */


            if (!await context.Corporacion.AnyAsync())
            {
                var LermaRegiones = new List<Region> {g,r1,r2,r3,r4,r5,r6,r7,r18,r9,r10,r11,r12,r13,r14,r15,r16,r17,r18,r19,r20,r21,r22};

                var AuxiliarRegiones = new List<Region> {g,r1,r2,r3,r4,r5,r6,r7,r18,r9,r10,r11,r12,r13,r14,r15,r16,r17,r18,r19,r20,r21,r22,r23,r24,r25,r26,r27,r28,r29,
                r30,r31,r32,r33,r34,r35,r36,r37,r38,r39,r40,r41,r42,r43,r44,r45,r46,r47,r48,r49,r50,r51};

                var corporaciones = new List<Cuerpo>
                {
                    new Cuerpo { Id="CGSIBCVCT", Nombre = "Cuerpo de Guardias de Seguridad Industrial, Bancaria y Comercial del Valle Cuautitlan Texcoco",
                        Calle ="Prolongación Galeana",Numero = 28,
                        alias = "Loma",
                        Pais ="México",CodigoPostal = 54060,Estado ="Estado de México",
                        Municipio ="Tlalnepantla",Colonia ="La Loma", Regiones = regiones
                    },
                    new Cuerpo { Id="CVAUEM", Nombre = "Cuerpo de Vigilancia Auxiliar y Urbana del Estado de México",
                        Calle = "Avenida Tlalnepantla",Numero = 4,
                        alias = "Auxiliar",
                        Pais = "México",CodigoPostal = 54160,Estado = "Estado de México",
                        Municipio = "Tlalnepantla",Colonia = "San Felipe Ixtacala", Regiones = AuxiliarRegiones
                    },
                    new Cuerpo { Id="CGSIBCVT", Nombre = "Cuerpo de Guardias de Seguridad Industrial, Bancaria y Comercial del Valle de Toluca",
                        Calle = "Atotonilco S/N",Numero = 0,
                        alias = "Loma",
                        Pais = "México",CodigoPostal = 52000,Estado = "Estado de México",
                        Municipio = "Lerma de Villada",Colonia = "Parque Industrial Lerma", Regiones = LermaRegiones
                    }
                };
                await context.Corporacion.AddRangeAsync(corporaciones);
                await context.SaveChangesAsync();
                /*
                Cuerpo loma = await context.Set<Cuerpo>().FindAsync("CGSIBCVCT");
                loma.Regiones = regiones;
                var result = await context.SaveChangesAsync() > 0;

                Cuerpo auxiliar= await context.Set<Cuerpo>().FindAsync("CVAUEM");
                auxiliar.Regiones = AuxiliarRegiones;
                var result1 = await context.SaveChangesAsync() > 0;

                Cuerpo lerma = await context.Set<Cuerpo>().FindAsync("CGSIBCVT");
                lerma.Regiones = LermaRegiones;
                var result2 = await context.SaveChangesAsync() > 0;
                */

            }
        }
    }
}
