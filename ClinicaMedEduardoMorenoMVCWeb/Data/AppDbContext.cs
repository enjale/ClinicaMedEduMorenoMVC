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

    }
}
