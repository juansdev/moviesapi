using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;

namespace MoviesApi;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Gender> Genders { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Movie> Movies { get; set; }
}