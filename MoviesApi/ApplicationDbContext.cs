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
    public DbSet<MoviesAuthors> MoviesAuthors { get; set; }
    public DbSet<MoviesGenders> MoviesGenders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesAuthors>().HasKey(author => new { author.AuthorId, author.MovieId });
        modelBuilder.Entity<MoviesGenders>().HasKey(author => new { author.GenderId, author.MovieId });
        base.OnModelCreating(modelBuilder);
    }
}