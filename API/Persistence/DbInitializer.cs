
using API.Common.Domain;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Domain.Ubicacion;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models.CallRecords;


namespace API.Persistence
{
    public class DbInitializer
    {
        public static async Task SeedData(AppDbContext context,
            UserManager<AppUserIdentity> userManager,
            RoleManager<AppIdentityRole> roleManager,
            IConfiguration configuration)
        {


            if (!context.TipoRoles.Any())
            {
                DateTime fechaTodo = DateTime.Now;
                var tr = new List<TipoRolesNoUtil>
                {
                    new TipoRolesNoUtil{Id = 0, Name = "NoDefinido", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 1, Name = "Subdirector", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 2, Name = "Gerente", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 3, Name = "AtencionRegistro", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 4, Name = "Psicologo", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 5, Name = "Medico", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 6, Name = "Antidoping", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 7, Name = "Capturista", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},
                    new TipoRolesNoUtil{Id = 8, Name = "Administrador", Activo = true,FechaCreacion = fechaTodo, FechaUltimaActualizacion = fechaTodo},

                };
                await context.TipoRoles.AddRangeAsync(tr);
                await context.SaveChangesAsync();
            }


            // Crear roles predeterminados
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Admin", Activo = true, Descripcion = "Rol de Administradores", Value = "admon", TipoRol = TipoRoles.Administrador });
            }

            if (!await roleManager.RoleExistsAsync("Subdireccion"))
            {
                
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Subdireccion", Activo = true, Descripcion = "Rol de Subdireccion", Value = "subdireccion", TipoRol = TipoRoles.Subdirector });
            }

            if (!await roleManager.RoleExistsAsync("Gerencia"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Gerencia", Activo = true, Descripcion = "Rol de Gerente", Value = "gerente", TipoRol = TipoRoles.Gerente });
            }

            if (!await roleManager.RoleExistsAsync("Medico"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Medico", Activo = true, Descripcion = "Rol de médico", Value = "medico", TipoRol = TipoRoles.Medico });
            }

            if (!await roleManager.RoleExistsAsync("AtencionRegistro"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "AtencionRegistro", Activo = true, Descripcion = "Rol de Atencion y Registro", Value = "AtencionRegistro", TipoRol = TipoRoles.AtencionRegistro });
            }

            if (!await roleManager.RoleExistsAsync("Psicologo"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Psicologo", Activo = true, Descripcion = "Rol de Psicologo", Value = "Psicologo", TipoRol = TipoRoles.Psicologo });
            }

            if (!await roleManager.RoleExistsAsync("Antidoping"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Antidoping", Activo = true, Descripcion = "Rol de Antidoping", Value = "Antidoping", TipoRol = TipoRoles.Antidoping });
            }

            if (!await roleManager.RoleExistsAsync("Capturista"))
            {
                await roleManager.CreateAsync(new AppIdentityRole { Name = "Capturista", Activo = true, Descripcion = "Rol de Capturista", Value = "Capturista", TipoRol = TipoRoles.Capturista });
            }

            var adminGrupoId = Guid.NewGuid().ToString();
            var subdireccionGrupoId = Guid.NewGuid().ToString();
            var medicosGrupoId = Guid.NewGuid().ToString();
            var gerenciaGrupoId = Guid.NewGuid().ToString();
            var atencionRegistroGrupoId = Guid.NewGuid().ToString();
            var psicologoGrupoId = Guid.NewGuid().ToString();
            var antidopingGrupoId = Guid.NewGuid().ToString();
            var capturistaGrupoId = Guid.NewGuid().ToString();
            
            
            if (!await context.Grupos.AnyAsync()) 
            {
                var grupos = new List<Grupo>
                {
                    new Grupo
                    {
                        Id = adminGrupoId,
                        Nombre = "ADMIN",
                        Descr = "Grupo para Administradores del Sistema",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = medicosGrupoId,
                        Nombre = "MEDICOS",
                        Descr = "Grupo de Usuarios tipo Medicos",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = subdireccionGrupoId,
                        Nombre = "SUBDIRECTOR",
                        Descr = "Grupo de Usuarios tipo Subdirector",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = gerenciaGrupoId,
                        Nombre = "GERENCIA",
                        Descr = "Grupo de Usuarios tipo Gerente",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = atencionRegistroGrupoId,
                        Nombre = "ATENCIONREGISTRO",
                        Descr = "Grupo de Usuarios tipo Atención y Registro",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = psicologoGrupoId,
                        Nombre = "PSICOLOGO",
                        Descr = "Grupo de Usuarios tipo Psicologo",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = antidopingGrupoId,
                        Nombre = "ANTIDOPING",
                        Descr = "Grupo de Usuarios tipo Antidoping",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new Grupo
                    {
                        Id = capturistaGrupoId,
                        Nombre = "CAPTURISTA",
                        Descr = "Grupo de Usuarios tipo Capturista",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    }
                };
                await context.Grupos.AddRangeAsync(grupos);
                await context.SaveChangesAsync();
            }

            // Crear un usuario de ejemplo
            if (!userManager.Users.Any())
            {
                var user = new AppUserIdentity
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Password123!");
                await userManager.AddToRoleAsync(user, "Admin");

                var atencionUser = new AppUserIdentity
                {
                    UserName = "atencionregistro",
                    Email = "atencionregistro@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(atencionUser, "Password123!");
                await userManager.AddToRoleAsync(atencionUser, "AtencionRegistro");

                var capturistaUser = new AppUserIdentity
                {
                    UserName = "capturista",
                    Email = "capturista@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };  
                await userManager.CreateAsync(capturistaUser, "Password123!");
                await userManager.AddToRoleAsync(capturistaUser, "Capturista");


                var medicoUser = new AppUserIdentity
                {
                    UserName = "medico",
                    Email = "medico@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(medicoUser, "Password123!");
                await userManager.AddToRoleAsync(medicoUser, "Medico");

                var psicologoUser = new AppUserIdentity
                {
                    UserName = "psicologo",
                    Email = "psicologo@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(psicologoUser, "Password123!");
                await userManager.AddToRoleAsync(psicologoUser, "Psicologo");

                var antidopingUser = new AppUserIdentity
                {
                    UserName = "antidoping",
                    Email = "antidoping@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(antidopingUser, "Password123!");
                await userManager.AddToRoleAsync(antidopingUser, "Antidoping");

                var gerenteUser = new AppUserIdentity
                {
                    UserName = "gerente",
                    Email = "gerente@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(gerenteUser, "Password123!");
                await userManager.AddToRoleAsync(gerenteUser, "Gerencia");

                var subdirectorUser = new AppUserIdentity
                {
                    UserName = "subdirector",
                    Email = "subdirector@cusaem.gob.mx",
                    EmailConfirmed = true,
                    TwoFactorEnabled = false
                };
                await userManager.CreateAsync(subdirectorUser, "Password123!");
                await userManager.AddToRoleAsync(subdirectorUser, "Subdireccion");

            }

            string usuarioAdminId = Guid.NewGuid().ToString();
            var usuarioSubdireccionId = Guid.NewGuid().ToString();
            var usuarioGerenciaId = Guid.NewGuid().ToString();
            string usuarioMedicoId = Guid.NewGuid().ToString();
            var usuarioAtencionRegistroId = Guid.NewGuid().ToString();
            var usuarioPsicologoId = Guid.NewGuid().ToString();
            var usuarioAntidopingId = Guid.NewGuid().ToString();
            var usuarioCapturistaId = Guid.NewGuid().ToString();
            

            if (!context.Usuarios.Any())
            {
                var adminUser = await userManager.FindByNameAsync("admin");
                var usuario = new Usuario
                {
                    Id = usuarioAdminId,
                    Nombre = "Administrador",
                    PrimerApellido = "CUSAEM",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Admin")).Id,
                    GrupoId = adminGrupoId,
                    AppUserIdentityId = adminUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuario);

                var medicoUser = await userManager.FindByNameAsync("medico");
                var usuarioMedico = new Usuario
                {
                    Id = usuarioMedicoId,
                    Nombre = "Medico",
                    PrimerApellido = "CUSAEM",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Medico")).Id,
                    GrupoId = medicosGrupoId,
                    AppUserIdentityId = medicoUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuarioMedico);

                var subdirectorUser = await userManager.FindByNameAsync("subdirector");
                var usuariosubdirector = new Usuario
                {
                    Id = usuarioSubdireccionId,
                    Nombre = "Subdirector",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Subdireccion")).Id,
                    GrupoId = subdireccionGrupoId,
                    AppUserIdentityId = subdirectorUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuariosubdirector);
                
                var gerenteUser = await userManager.FindByNameAsync("gerente");
                var usuarioGerente = new Usuario
                {
                    Id = usuarioGerenciaId,
                    Nombre = "Gerente",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Gerencia")).Id,
                    GrupoId = gerenciaGrupoId,
                    AppUserIdentityId = gerenteUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuarioGerente);

                var atencionrUser = await userManager.FindByNameAsync("atencionregistro");
                var usuarioatencionyregistro = new Usuario
                {
                    Id = usuarioAtencionRegistroId,
                    Nombre = "Atencion",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("AtencionRegistro")).Id,
                    GrupoId = atencionRegistroGrupoId,
                    AppUserIdentityId = atencionrUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuarioatencionyregistro);
                
                var psicologoUser = await userManager.FindByNameAsync("psicologo");
                var usuaripsicologo = new Usuario
                {
                    Id = usuarioPsicologoId,
                    Nombre = "Psicologo",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Psicologo")).Id,
                    GrupoId = psicologoGrupoId,
                    AppUserIdentityId = psicologoUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuaripsicologo);

                var antidopingUser = await userManager.FindByNameAsync("antidoping");
                var usuarioantidoping = new Usuario
                {
                    Id = usuarioAntidopingId,
                    Nombre = "Antidoping",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Antidoping")).Id,
                    GrupoId = antidopingGrupoId,
                    AppUserIdentityId = antidopingUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuarioantidoping);

                var capturistaUser = await userManager.FindByNameAsync("capturista");
                var usuariocapturista = new Usuario
                {
                    Id = usuarioCapturistaId,
                    Nombre = "capturista",
                    PrimerApellido = "app",
                    TiempoInactividad = 15,
                    Activo = true,
                    EsUsuarioAD = false,
                    RolId = (await roleManager.FindByNameAsync("Capturista")).Id,
                    GrupoId = capturistaGrupoId,
                    AppUserIdentityId = capturistaUser.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                };
                await context.Usuarios.AddAsync(usuariocapturista);
                

                await context.SaveChangesAsync();
            }

            // Crear corporaciones predeterminadas
            if (!context.Corporaciones.Any())
            {
                var corporaciones = new List<Corporacion>
                {
                    new Corporacion
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombre = "Loma",
                        Descripcion = "Corporación la Loma",
                    },
                    new Corporacion
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombre = "Santa Rosa",
                        Descripcion = "Corporación Santa Rosa",
                    }
                    ,
                    new Corporacion
                    {
                        Id = Guid.NewGuid().ToString(),
                        Nombre = "Unidad de Empleo",
                        Descripcion = "Unidad de Empleo",
                    }
                };
                await context.Corporaciones.AddRangeAsync(corporaciones);
                await context.SaveChangesAsync();
            }

            if (!context.UsuarioCorporaciones.Any())
            {
                var corporaciones = await context.Corporaciones.ToListAsync();
                var usuarioCorporaciones = corporaciones.Select(c => new UsuarioCorporacion
                {
                    UsuarioId = usuarioAdminId,
                    CorporacionId = c.Id
                }).ToList();
                await context.UsuarioCorporaciones.AddRangeAsync(usuarioCorporaciones);
                await context.SaveChangesAsync();
            }

            if (!context.EntidadFederativas.Any())
            {
                var entidades = new List<EntidadFederativa>
                {
                    new EntidadFederativa { Id = 0, Nombre = "NO DISPONIBLE", Abreviacion = "" },
                    new EntidadFederativa { Id = 1, Nombre = "AGUASCALIENTES", Abreviacion = "AS" },
                    new EntidadFederativa { Id = 2, Nombre = "BAJA CALIFORNIA", Abreviacion = "BC" },
                    new EntidadFederativa { Id = 3, Nombre = "BAJA CALIFORNIA SUR", Abreviacion = "BS" },
                    new EntidadFederativa { Id = 4, Nombre = "CAMPECHE", Abreviacion = "CC" },
                    new EntidadFederativa { Id = 5, Nombre = "COAHUILA", Abreviacion = "CL" },
                    new EntidadFederativa { Id = 6, Nombre = "COLIMA", Abreviacion = "CM" },
                    new EntidadFederativa { Id = 7, Nombre = "CHIAPAS", Abreviacion = "CS" },
                    new EntidadFederativa { Id = 8, Nombre = "CHIHUAHUA", Abreviacion = "CH" },
                    new EntidadFederativa { Id = 9, Nombre = "CIUDAD DE MÉXICO", Abreviacion = "DF" },
                    new EntidadFederativa { Id = 10, Nombre = "DURANGO", Abreviacion = "DG" },
                    new EntidadFederativa { Id = 11, Nombre = "GUANAJUATO", Abreviacion = "GT" },
                    new EntidadFederativa { Id = 12, Nombre = "GUERRERO", Abreviacion = "GR" },
                    new EntidadFederativa { Id = 13, Nombre = "HIDALGO", Abreviacion = "HG" },
                    new EntidadFederativa { Id = 14, Nombre = "JALISCO", Abreviacion = "JC" },
                    new EntidadFederativa { Id = 15, Nombre = "MÉXICO", Abreviacion = "MC" },
                    new EntidadFederativa { Id = 16, Nombre = "MICHOACÁN", Abreviacion = "MN" },
                    new EntidadFederativa { Id = 17, Nombre = "MORELOS", Abreviacion = "MS" },
                    new EntidadFederativa { Id = 18, Nombre = "NAYARIT", Abreviacion = "NT" },
                    new EntidadFederativa { Id = 19, Nombre = "NUEVO LEÓN", Abreviacion = "NL" },
                    new EntidadFederativa { Id = 20, Nombre = "OAXACA", Abreviacion = "OC" },
                    new EntidadFederativa { Id = 21, Nombre = "PUEBLA", Abreviacion = "PL" },
                    new EntidadFederativa { Id = 22, Nombre = "QUERÉTARO", Abreviacion = "QT" },
                    new EntidadFederativa { Id = 23, Nombre = "QUINTANA ROO", Abreviacion = "QR" },
                    new EntidadFederativa { Id = 24, Nombre = "SAN LUIS POTOSÍ", Abreviacion = "SP" },
                    new EntidadFederativa { Id = 25, Nombre = "SINALOA", Abreviacion = "SL" },
                    new EntidadFederativa { Id = 26, Nombre = "SONORA", Abreviacion = "SR" },
                    new EntidadFederativa { Id = 27, Nombre = "TABASCO", Abreviacion = "TC" },
                    new EntidadFederativa { Id = 28, Nombre = "TAMAULIPAS", Abreviacion = "TS" },
                    new EntidadFederativa { Id = 29, Nombre = "TLAXCALA", Abreviacion = "TL" },
                    new EntidadFederativa { Id = 30, Nombre = "VERACRUZ", Abreviacion = "VZ" },
                    new EntidadFederativa { Id = 31, Nombre = "YUCATÁN", Abreviacion = "YN" },
                    new EntidadFederativa { Id = 32, Nombre = "ZACATECAS", Abreviacion = "ZS" },
                    new EntidadFederativa { Id = 33, Nombre = "EXTRANJERO", Abreviacion = "EX" }
                };
                await context.EntidadFederativas.AddRangeAsync(entidades);
                await context.SaveChangesAsync();
            }

            if (!context.Sistemas.Any())
            {
                var sistemas = new List<Sistema>
                {
                    new Sistema
                    {
                        Id = 1,
                        Nombre = "Sistema Administrativo",
                        Descripcion = "Sistema para la gestión administrativa",
                        Activo = true
                    },
                    new Sistema
                    {
                        Id = 2,
                        Nombre = "Sistema Clínico",
                        Descripcion = "Sistema para la gestión clínica",
                        Activo = true
                    },
                    new Sistema
                    {
                        Id = 3,
                        Nombre = "Sistema Unidad de Empleo",
                        Descripcion = "Sistema para la gestión de aspirantes a causar alta en los CUSAEM",
                        Activo = true
                    },

                };
                await context.Sistemas.AddRangeAsync(sistemas);
                await context.SaveChangesAsync();
            }

            if (!context.BasesDatos.Any())
            {
                var serverName = configuration["DatabaseSettings:ServerName"] ?? "(localdb)\\MSSQLLocalDB";

                var basesDatos = new List<BaseDatos>
                {
                    new BaseDatos
                    {
                        Id = 1,
                        Nombre = "BaseDatosAdministrativa",
                        Descripcion = "Base de datos para el sistema administrativo",
                        DatabaseName = "BaseDatosAdministrativa",
                        ServerName = serverName,
                    },
                    new BaseDatos
                    {
                        Id = 2,
                        Nombre = "BaseDatosClinica",
                        Descripcion = "Base de datos para el sistema clínico",
                        DatabaseName = "Clinica",
                        ServerName = serverName,
                    },
                    new BaseDatos
                    {
                        Id = 3,
                        Nombre = "BaseDatosUnidadEmpleo",
                        Descripcion = "Base de datos para el sistema clínico",
                        DatabaseName = "UnidadEmpleo",
                        ServerName = serverName,
                    }

                };
                await context.BasesDatos.AddRangeAsync(basesDatos);
                await context.SaveChangesAsync();
            }

            if (!context.CorporacionSistemaBDs.Any())
            {
                var coporacionSistemaBDs = new List<CorporacionSistemaBD>
                {
                     new CorporacionSistemaBD
                    {
                        CorporacionId = context.Corporaciones.FirstOrDefault(d=>d.Nombre == "Unidad de Empleo").Id,
                        SistemaId = 3,
                        BaseDatosId = 3
                    }
                };
                await context.CorporacionSistemaBDs.AddRangeAsync(coporacionSistemaBDs);
                await context.SaveChangesAsync();
            }
            
            // Seed Estados Civiles
            // Seed Estados Civiles
            var estadosCiviles = new List<EstadoCivil>
            {
                new EstadoCivil
                {
                    Id = 0,
                    Nombre = "Otro",
                    Descripcion = "Otro estado civil no especificado",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 1,
                    Nombre = "Soltero",
                    Descripcion = "Persona soltera",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 2,
                    Nombre = "Casado",
                    Descripcion = "Persona casada",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 3,
                    Nombre = "Divorciado",
                    Descripcion = "Persona divorciada",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 4,
                    Nombre = "Viudo",
                    Descripcion = "Persona viuda",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 5,
                    Nombre = "Unión Libre",
                    Descripcion = "Persona en unión libre",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 6,
                    Nombre = "Separado",
                    Descripcion = "Persona separada",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                },
                new EstadoCivil
                {
                    Id = 7,
                    Nombre = "Concubinato",
                    Descripcion = "Persona en concubinato",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaActualizacion = DateTime.UtcNow
                }
            };

            foreach (var estado in estadosCiviles)
            {
                if (!context.EstadosCiviles.Any(e => e.Nombre == estado.Nombre))
                {
                    await context.EstadosCiviles.AddAsync(estado);
                }
            }
            await context.SaveChangesAsync();

            // Seed Niveles Educativos
            if (!context.NivelesEducativos.Any())
            {
                var nivelesEducativos = new List<NivelEducativo>
                {
                    new NivelEducativo
                    {
                        Id = 0,
                        Nombre = "Otro",
                        Descripcion = "Otro nivel educativo no especificado",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 1,
                        Nombre = "Primaria",
                        Descripcion = "Educación primaria",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 2,
                        Nombre = "Secundaria",
                        Descripcion = "Educación secundaria",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 3,
                        Nombre = "Preparatoria",
                        Descripcion = "Educación preparatoria o bachillerato",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 4,
                        Nombre = "Licenciatura",
                        Descripcion = "Educación universitaria de licenciatura",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 5,
                        Nombre = "Maestría",
                        Descripcion = "Posgrado de maestría",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    },
                    new NivelEducativo
                    {
                        Id = 6,
                        Nombre = "Doctorado",
                        Descripcion = "Posgrado de doctorado",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow,
                        FechaUltimaActualizacion = DateTime.UtcNow
                    }
                };
                await context.NivelesEducativos.AddRangeAsync(nivelesEducativos);
                await context.SaveChangesAsync();
            }

            
           // Crear procesos predeterminados
           if (!context.Procesos.Any())
           {
               // FASE 1: Insertar procesos PADRES (sin ProcesoPadreId)
               var procesosPadres = new List<Proceso>
               {
                   new Proceso
                   {
                       Descr = "Gestion de Usuario",
                       Tipo = "A",
                       Icono = "groups",
                       Activo = true,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 1
                   },
                   new Proceso
                   {
                       Descr = "Subdireccion",
                       Tipo = "A",
                       Icono = "book",
                       Activo = true,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":false,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 3
                   },
                   new Proceso
                   {
                       Descr = "Gerencia",
                       Tipo = "A",
                       Icono = "book",
                       Activo = true,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 3
                   },
                   new Proceso
                   {
                       Descr = "Atencion y Registro",
                       Tipo = "A",
                       Icono = "badge",
                       Activo = false,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 3
                   },

                   new Proceso
                   {
                       Descr = "Registro",
                       Tipo = "A",
                       Icono = "books",
                       Activo = false,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 3
                   },
                   new Proceso
                   {
                       Descr = "Psicologo",
                       Tipo = "A",
                       Icono = "book-bookmark",
                       Activo = false,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 3
                   },
                   new Proceso
                   {
                       Descr = "Medico",
                       Tipo = "A",
                       Icono = "badge",
                       Activo = true,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":false,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 3
                   },
                   new Proceso
                   {
                       Descr = "Antidoping",
                       Tipo = "A",
                       Icono = "book",
                       Activo = true,
                       Ruta = null,
                       ProcesoPadreId = null,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = null,
                       SistemaId = 3
                   }

               };

               await context.Procesos.AddRangeAsync(procesosPadres);
               await context.SaveChangesAsync();

               // Obtener los IDs de los procesos padres recién insertados

               var registro = await context.Procesos.FirstAsync(p => p.Descr == "Registro" && p.Tipo == "A");
               var gestionUsuario = await context.Procesos.FirstAsync(p => p.Descr == "Gestion de Usuario" && p.Tipo == "A");
               var subdireccion = await context.Procesos.FirstAsync(p => p.Descr == "Subdireccion" && p.Tipo == "A");
               var gerencia = await context.Procesos.FirstAsync(p => p.Descr == "Gerencia" && p.Tipo == "A");
               var atencion = await context.Procesos.FirstAsync(p => p.Descr == "Atencion y Registro" && p.Tipo == "A");
               var psicologo = await context.Procesos.FirstAsync(p => p.Descr == "Psicologo" && p.Tipo == "A");
               var medico = await context.Procesos.FirstAsync(p => p.Descr == "Medico" && p.Tipo == "A");
               var antidoping = await context.Procesos.FirstAsync(p => p.Descr == "Antidoping" && p.Tipo == "A");

               // FASE 2: Insertar procesos HIJOS (con ProcesoPadreId válidos)
               var procesosHijos = new List<Proceso>
               {
                   //GESTION DE USUARIO
                   new Proceso
                   {
                       Descr = "Roles",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "RolesList",
                       ProcesoPadreId = gestionUsuario.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":true,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 1
                   },

                   new Proceso
                   {
                       Descr = "Usuarios",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "UsuarioList",
                       ProcesoPadreId = gestionUsuario.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":true,\"Editar\":true,\"Inactivar\":true,\"SeleccionarRol\":true,\"SeleccionarGrupo\":true,\"Agregar permisos\":true}]",
                       SistemaId = 1
                   },
                   new Proceso
                   {
                       Descr = "Grupos",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "GruposList",
                       ProcesoPadreId = gestionUsuario.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":true,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 1
                   },
                   new Proceso
                   {
                       Descr = "Proceso",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "ProcesoList",
                       ProcesoPadreId = gestionUsuario.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":true,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 1
                   },




                   //ASPIRANTES
                   new Proceso
                   {
                       Descr = "Aspirantes",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "AspirantesList",
                       ProcesoPadreId = registro.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":false,\"Editar\":true,\"Borrar\":false,\"Evaluar\":false}]",
                       SistemaId = 3
                   },

                   new Proceso
                   {
                       Descr = "Evaluaciones",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "EvaluacionActiva",
                       ProcesoPadreId = atencion.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Evaluar\":false,\"Quitar\":true,\"Finalizar\":false}]",
                       SistemaId = 3
                   },

                   new Proceso
                   {
                       Descr = "Evaluación Psicológica",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "Evaluacionpsicologica",
                       ProcesoPadreId = psicologo.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Editar\":true}]",
                       SistemaId = 3
                   },

                   new Proceso
                   {
                       Descr = "Evaluación Médica",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "Evaluacionmedica",
                       ProcesoPadreId = medico.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Editar\":true}]",
                       SistemaId = 3
                   },

                    new Proceso
                   {
                       Descr = "Evaluación Antidoping y PIE",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "Evaluacionpantidoping",
                       ProcesoPadreId = antidoping.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Editar\":true}]",
                       SistemaId = 3
                   },

                   //Gerencia
                   new Proceso
                   {
                       Descr = "Reporte Diario",
                       Tipo = "P",
                       Icono = null,
                       Activo = true,
                       Ruta = "ReporteDiario",
                       ProcesoPadreId = gerencia.Id,
                       FechaCreacion = DateTime.UtcNow,
                       FechaUltimaActualizacion = DateTime.UtcNow,
                       Acciones = "[{\"Agregar\":false,\"Editar\":true,\"Borrar\":false}]",
                       SistemaId = 2
                   }
               };

               await context.Procesos.AddRangeAsync(procesosHijos);
               await context.SaveChangesAsync();
           }
           
            // ******************* falta arreglar esta parte
            
            if (!context.RolesProcesos.Any())
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                var allProcesos = await context.Procesos.ToListAsync();
                var rolesProcesos = allProcesos.Select(p => new RolProceso
                {
                    RolId = adminRole.Id,
                    ProcesoId = p.Id
                }).ToList();
                await context.RolesProcesos.AddRangeAsync(rolesProcesos);

                var archivistaRole = await roleManager.FindByNameAsync("Archivista clinico");
                var archivoClinico = allProcesos.FirstOrDefault(p => p.Descr == "Archivo Clinico");
                var archivistaProcesos = allProcesos
                    .Where(p => p.SistemaId == 2 && p.ProcesoPadreId == archivoClinico?.Id || p.Id == archivoClinico?.Id)
                    .Select(p => new RolProceso
                    {
                        RolId = archivistaRole.Id,
                        ProcesoId = p.Id
                    }).ToList();    
                await context.RolesProcesos.AddRangeAsync(archivistaProcesos);
                
                await context.SaveChangesAsync();
            }

            if (!context.Permisos.Any())
            {
                var allRolesProcesos = await context.RolesProcesos.ToListAsync();

                var procesoUsuarios = await context.Procesos
                    .Where(p => p.Ruta == "UsuarioList")
                    .FirstOrDefaultAsync();

                string accionUsuario = "[{\"Agregar\":true,\"Editar\":true,\"Inactivar\":true,\"SeleccionarRol\":true,\"SeleccionarGrupo\":true,\"Agregar permisos\":true}]";

                var permisos = allRolesProcesos.Select(rp => new Permiso
                {
                    UsuarioId = usuarioAdminId,
                    RolId = rp.RolId,
                    ProcesoId = rp.ProcesoId,
                    Acceso = procesoUsuarios != null && procesoUsuarios.Id == rp.ProcesoId 
                        ? accionUsuario 
                        : context.Procesos.First(p => p.Id == rp.ProcesoId).Acciones ?? string.Empty,
                }).ToList();
                await context.Permisos.AddRangeAsync(permisos);
                await context.SaveChangesAsync();
            }

        }
    }
}