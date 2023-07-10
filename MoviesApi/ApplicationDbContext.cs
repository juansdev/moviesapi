using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Entities;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace MoviesApi;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Gender> Genders { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MoviesAuthors> MoviesAuthors { get; set; }
    public DbSet<MoviesGenders> MoviesGenders { get; set; }
    public DbSet<Cinema> Cinema { get; set; }
    public DbSet<MoviesCinemas> MoviesCinemas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesAuthors>().HasKey(author => new { author.AuthorId, author.MovieId });
        modelBuilder.Entity<MoviesGenders>().HasKey(author => new { author.GenderId, author.MovieId });
        modelBuilder.Entity<MoviesCinemas>()
            .HasKey(moviesCinemas => new { moviesCinemas.MovieId, moviesCinemas.CinemaId });
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }


    private void SeedData(ModelBuilder modelBuilder)
    {
        var rolAdminId = "a3090671-5367-4fb4-8e0d-0efdb99d7225";
        var userAdminId = "9571f5b8-5b04-4efa-be5a-9cf80c3ffda7";
        var rolAdmin = new IdentityRole
        {
            Id = rolAdminId,
            Name = "Admin",
            NormalizedName = "Admin"
        };
        var passwordHasher = new PasswordHasher<IdentityUser>();
        var userName = "juansdev72@gmail.com";
        var userAdmin = new IdentityUser
        {
            Id = userAdminId,
            UserName = userName,
            NormalizedUserName = userName,
            Email = userName,
            NormalizedEmail = userName,
            PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
        };
        // modelBuilder.Entity<IdentityUser>().HasData(userAdmin);
        // modelBuilder.Entity<IdentityRole>().HasData(rolAdmin);
        // modelBuilder.Entity<IdentityUserClaim<string>>()
        //     .HasData(new IdentityUserClaim<string>()
        // {
        //     Id = 1,
        //     ClaimType = ClaimTypes.Role,
        //     UserId = userAdminId,
        //     ClaimValue = "Admin"
        // });
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        modelBuilder.Entity<Cinema>().HasData(new List<Cinema>
        {
            new()
            {
                Id = 4, Name = "Sambil", Location = geometryFactory.CreatePoint(new Coordinate(-69.9118804, 18.4826214))
            },
            new()
            {
                Id = 5, Name = "Megacentro",
                Location = geometryFactory.CreatePoint(new Coordinate(-69.856427, 18.506934))
            },
            new()
            {
                Id = 6, Name = "Village East Cinema",
                Location = geometryFactory.CreatePoint(new Coordinate(-73.986227, 40.730898))
            }
        });
        var accion = new Gender { Id = 1, Name = "Acción" };
        var comedia = new Gender { Id = 2, Name = "Comedia" };
        var drama = new Gender { Id = 3, Name = "Drama" };
        var aventura = new Gender { Id = 4, Name = "Aventura" };
        var animation = new Gender { Id = 5, Name = "Animación" };
        var suspenso = new Gender { Id = 6, Name = "Suspenso" };
        var romance = new Gender { Id = 7, Name = "Romance" };

        modelBuilder.Entity<Gender>()
            .HasData(new List<Gender>
            {
                accion, comedia, drama, aventura, animation, suspenso, romance
            });

        var jimCarrey = new Author { Id = 5, Name = "Jim Carrey", BirthdayDate = new DateTime(1962, 01, 17) };
        var robertDowney = new Author { Id = 6, Name = "Robert Downey Jr.", BirthdayDate = new DateTime(1965, 4, 4) };
        var chrisEvans = new Author { Id = 7, Name = "Chris Evans", BirthdayDate = new DateTime(1981, 06, 13) };

        modelBuilder.Entity<Author>()
            .HasData(new List<Author>
            {
                jimCarrey, robertDowney, chrisEvans
            });

        var endgame = new Movie
        {
            Id = 2,
            Title = "Avengers: Endgame",
            InTheaters = true,
            ReleaseDate = new DateTime(2019, 04, 26)
        };

        var iw = new Movie
        {
            Id = 3,
            Title = "Avengers: Infinity Wars",
            InTheaters = false,
            ReleaseDate = new DateTime(2019, 04, 26)
        };

        var sonic = new Movie
        {
            Id = 4,
            Title = "Sonic the Hedgehog",
            InTheaters = false,
            ReleaseDate = new DateTime(2020, 02, 28)
        };
        var emma = new Movie
        {
            Id = 5,
            Title = "Emma",
            InTheaters = false,
            ReleaseDate = new DateTime(2020, 02, 21)
        };
        var wonderwoman = new Movie
        {
            Id = 6,
            Title = "Wonder Woman 1984",
            InTheaters = false,
            ReleaseDate = new DateTime(2020, 08, 14)
        };

        modelBuilder.Entity<Movie>()
            .HasData(new List<Movie>
            {
                endgame, iw, sonic, emma, wonderwoman
            });

        modelBuilder.Entity<MoviesGenders>().HasData(
            new List<MoviesGenders>
            {
                new() { MovieId = endgame.Id, GenderId = suspenso.Id },
                new() { MovieId = endgame.Id, GenderId = aventura.Id },
                new() { MovieId = iw.Id, GenderId = suspenso.Id },
                new() { MovieId = iw.Id, GenderId = aventura.Id },
                new() { MovieId = sonic.Id, GenderId = aventura.Id },
                new() { MovieId = emma.Id, GenderId = suspenso.Id },
                new() { MovieId = emma.Id, GenderId = romance.Id },
                new() { MovieId = wonderwoman.Id, GenderId = suspenso.Id },
                new() { MovieId = wonderwoman.Id, GenderId = aventura.Id }
            });

        modelBuilder.Entity<MoviesAuthors>().HasData(
            new List<MoviesAuthors>
            {
                new() { MovieId = endgame.Id, AuthorId = robertDowney.Id, Character = "Tony Stark", Order = 1 },
                new() { MovieId = endgame.Id, AuthorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2 },
                new() { MovieId = iw.Id, AuthorId = robertDowney.Id, Character = "Tony Stark", Order = 1 },
                new() { MovieId = iw.Id, AuthorId = chrisEvans.Id, Character = "Steve Rogers", Order = 2 },
                new() { MovieId = sonic.Id, AuthorId = jimCarrey.Id, Character = "Dr. Ivo Robotnik", Order = 1 }
            });
    }
}