using System;
using System.Collections.Generic;
using API.Domain.Clinica;
using API.DTOs.Clinica;
using API.Seguridad.Domain.Core;
using Xunit;

namespace API.Tests.Domain.Clinica
{
    public class MedicoHelperTests
    {
        private MedicoDTO CreateMedico(
            string id,
            int especialidadId,
            DateOnly fechaInicio,
            DateOnly fechaTermino,
            DayOfWeek diaDisponible,
            string intervaloStart,
            string intervaloEnd)
        {
            var horario = new HorarioSchema();
            var diaHorario = new DiaHorarioSchema
            {
                Available = true,
                Intervals = new List<IntervaloSchema>
                {
                    new IntervaloSchema { Start = intervaloStart, End = intervaloEnd }
                }
            };

            switch (diaDisponible)
            {
                case DayOfWeek.Monday: horario.Lunes = diaHorario; break;
                case DayOfWeek.Tuesday: horario.Martes = diaHorario; break;
                case DayOfWeek.Wednesday: horario.Miercoles = diaHorario; break;
                case DayOfWeek.Thursday: horario.Jueves = diaHorario; break;
                case DayOfWeek.Friday: horario.Viernes = diaHorario; break;
                case DayOfWeek.Saturday: horario.Sabado = diaHorario; break;
                case DayOfWeek.Sunday: horario.Domingo = diaHorario; break;
            }

            var calendario = new CalendarioSchema
            {
                FechaInicio = fechaInicio,
                FechaTermino = fechaTermino,
                Especialidades = new List<EspecialidadSchema>
                {
                    new EspecialidadSchema
                    {
                        Id = especialidadId,
                        Horario = horario
                    }
                }
            };

            // MedicoDTO.CalendarioSchema is a getter, so we need to set Calendario as JSON or use a test subclass.
            // For simplicity, we use a subclass for testing.
            return new MedicoDTO_Test
            {
                Id = id,
                CalendarioSchemaTest = calendario
            };
        }

        // Subclass to override CalendarioSchema for testing
        private class MedicoDTO_Test : MedicoDTO
        {
            public CalendarioSchema? CalendarioSchemaTest { get; set; }
            public override CalendarioSchema? CalendarioSchema => CalendarioSchemaTest;
        }

        [Fact]
        public void Returns_Medico_When_All_Conditions_Match()
        {
            var medico = CreateMedico(
                id: "1",
                especialidadId: 10,
                fechaInicio: new DateOnly(2025, 4, 1),
                fechaTermino: new DateOnly(2025, 4, 30),
                diaDisponible: DayOfWeek.Tuesday,
                intervaloStart: "09:00",
                intervaloEnd: "17:00"
            );

            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 0, 0); // Tuesday

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Single(result);
            Assert.Equal("1", result[0].Id);
        }

        [Fact]
        public void Returns_Empty_When_Fecha_Fuera_De_Rango()
        {
            var medico = CreateMedico(
                id: "2",
                especialidadId: 10,
                fechaInicio: new DateOnly(2025, 4, 1),
                fechaTermino: new DateOnly(2025, 4, 30),
                diaDisponible: DayOfWeek.Tuesday,
                intervaloStart: "09:00",
                intervaloEnd: "17:00"
            );

            var fechaHoraInicio = new DateTime(2025, 5, 8, 10, 0, 0); // Out of range

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }

        [Fact]
        public void Returns_Empty_When_Especialidad_No_Coincide()
        {
            var medico = CreateMedico(
                id: "3",
                especialidadId: 20,
                fechaInicio: new DateOnly(2025, 4, 1),
                fechaTermino: new DateOnly(2025, 4, 30),
                diaDisponible: DayOfWeek.Tuesday,
                intervaloStart: "09:00",
                intervaloEnd: "17:00"
            );

            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 0, 0); // Tuesday

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }

        [Fact]
        public void Returns_Empty_When_Dia_No_Disponible()
        {
            var medico = CreateMedico(
                id: "4",
                especialidadId: 10,
                fechaInicio: new DateOnly(2025, 4, 1),
                fechaTermino: new DateOnly(2025, 4, 30),
                diaDisponible: DayOfWeek.Monday, // Only Monday available
                intervaloStart: "09:00",
                intervaloEnd: "17:00"
            );

            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 0, 0); // Tuesday

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }

        [Fact]
        public void Returns_Empty_When_Hora_Fuera_De_Intervalo()
        {
            var medico = CreateMedico(
                id: "5",
                especialidadId: 10,
                fechaInicio: new DateOnly(2025, 4, 1),
                fechaTermino: new DateOnly(2025, 4, 30),
                diaDisponible: DayOfWeek.Tuesday,
                intervaloStart: "09:00",
                intervaloEnd: "11:00"
            );

            var fechaHoraInicio = new DateTime(2025, 4, 8, 12, 0, 0); // 12:00, out of interval

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }

        // 1. Medico with multiple specialties, only one matches
        [Fact]
        public void Returns_Medico_When_One_Of_Multiple_Specialties_Matches()
        {
            var horario = new HorarioSchema
            {
                Martes = new DiaHorarioSchema
                {
                    Available = true,
                    Intervals = new List<IntervaloSchema>
                    {
                        new IntervaloSchema { Start = "08:00", End = "12:00" }
                    }
                }
            };

            var calendario = new CalendarioSchema
            {
                FechaInicio = new DateOnly(2025, 4, 1),
                FechaTermino = new DateOnly(2025, 4, 30),
                Especialidades = new List<EspecialidadSchema>
                {
                    new EspecialidadSchema { Id = 99, Horario = new HorarioSchema() }, // Not matching
                    new EspecialidadSchema { Id = 10, Horario = horario } // Matching
                }
            };

            var medico = new MedicoDTO_Test { Id = "multi", CalendarioSchemaTest = calendario };
            var fechaHoraInicio = new DateTime(2025, 4, 8, 9, 0, 0); // Tuesday

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Single(result);
            Assert.Equal("multi", result[0].Id);
        }

        // 2. Medico with multiple intervals, time matches only one
        [Fact]
        public void Returns_Medico_When_Time_Matches_Second_Interval()
        {
            var horario = new HorarioSchema
            {
                Martes = new DiaHorarioSchema
                {
                    Available = true,
                    Intervals = new List<IntervaloSchema>
                    {
                        new IntervaloSchema { Start = "08:00", End = "09:00" },
                        new IntervaloSchema { Start = "10:00", End = "12:00" }
                    }
                }
            };

            var calendario = new CalendarioSchema
            {
                FechaInicio = new DateOnly(2025, 4, 1),
                FechaTermino = new DateOnly(2025, 4, 30),
                Especialidades = new List<EspecialidadSchema>
                {
                    new EspecialidadSchema { Id = 10, Horario = horario }
                }
            };

            var medico = new MedicoDTO_Test { Id = "interval", CalendarioSchemaTest = calendario };
            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 30, 0); // Tuesday, matches second interval

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Single(result);
            Assert.Equal("interval", result[0].Id);
        }

        // 3. Medico with available day but empty intervals
        [Fact]
        public void Returns_Empty_When_Day_Available_But_No_Intervals()
        {
            var horario = new HorarioSchema
            {
                Martes = new DiaHorarioSchema
                {
                    Available = true,
                    Intervals = new List<IntervaloSchema>() // No intervals
                }
            };

            var calendario = new CalendarioSchema
            {
                FechaInicio = new DateOnly(2025, 4, 1),
                FechaTermino = new DateOnly(2025, 4, 30),
                Especialidades = new List<EspecialidadSchema>
                {
                    new EspecialidadSchema { Id = 10, Horario = horario }
                }
            };

            var medico = new MedicoDTO_Test { Id = "nointervals", CalendarioSchemaTest = calendario };
            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 0, 0); // Tuesday

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }

        // 4. Medico with overlapping intervals, time matches both
        [Fact]
        public void Returns_Medico_When_Time_Matches_Overlapping_Intervals()
        {
            var horario = new HorarioSchema
            {
                Martes = new DiaHorarioSchema
                {
                    Available = true,
                    Intervals = new List<IntervaloSchema>
                    {
                        new IntervaloSchema { Start = "08:00", End = "12:00" },
                        new IntervaloSchema { Start = "10:00", End = "14:00" }
                    }
                }
            };

            var calendario = new CalendarioSchema
            {
                FechaInicio = new DateOnly(2025, 4, 1),
                FechaTermino = new DateOnly(2025, 4, 30),
                Especialidades = new List<EspecialidadSchema>
                {
                    new EspecialidadSchema { Id = 10, Horario = horario }
                }
            };

            var medico = new MedicoDTO_Test { Id = "overlap", CalendarioSchemaTest = calendario };
            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 30, 0); // Tuesday, matches both intervals

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Single(result);
            Assert.Equal("overlap", result[0].Id);
        }

        // 5. Medico with null CalendarioSchema
        [Fact]
        public void Returns_Empty_When_CalendarioSchema_Is_Null()
        {
            var medico = new MedicoDTO_Test { Id = "nullcal", CalendarioSchemaTest = null };
            var fechaHoraInicio = new DateTime(2025, 4, 8, 10, 0, 0);

            var result = MedicoHelper.FiltrarPorEspecialidadYCalendario(
                new List<MedicoDTO> { medico }, 10, fechaHoraInicio);

            Assert.Empty(result);
        }
    }
}