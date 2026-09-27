using Microsoft.EntityFrameworkCore;
using ClinicaMedEduardoMorenoMVCWeb.Models;

namespace ClinicaMedEduardoMorenoMVCWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext
            (DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Alergias> Alergias { get; set; }
        public DbSet<Antecedentes> Antecedentes { get; set; }
        public DbSet<ConsultaEnfermedades> ConsultaEnfermedades { get; set; }
        public DbSet<Consultas> Consultas { get; set; }
        public DbSet<ContactoEmergencia> ContactoEmergencias { get; set; }
        public DbSet<Enfermedades> Enfermedades { get; set; }
        public DbSet<ExpedienteAlergias> ExpedienteAlergias { get; set; }
        public DbSet<ExpedienteEnfermedades> ExpedienteEnfermedades { get; set; }
        public DbSet<Expedientes> Expedientes { get; set; }
        public DbSet<Medicamentos> Medicamentos { get; set; }
        public DbSet<Pacientes> Pacientes { get; set; }
        public DbSet<RecetaDetalle> RecetaDetalles { get; set; }
        public DbSet<Recetas> Recetas { get; set; }
        public DbSet<Responsables> Responsables { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
