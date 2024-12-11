using Microsoft.EntityFrameworkCore;

namespace API.Models;

    public class AppDataContext : DbContext
    {
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<IMC> IMCs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }
