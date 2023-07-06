using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Gender> Genders { get; set; }
}