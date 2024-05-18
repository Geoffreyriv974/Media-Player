using Microsoft.EntityFrameworkCore;

public class MediaPlayerContext : DbContext
{
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Media> Medias { get; set; }

    public string DbPath { get; }

    public MediaPlayerContext()
    {
        var path = "C:\\Users\\Public\\Documents";
        DbPath = System.IO.Path.Join("C:\\Users\\Public\\Documents\\Media_Player.db");
        Database.EnsureCreated();
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Playlist
{
    public int PlaylistId { get; set; }
    public string Name { get; set; }

    public List<Media> Medias { get; } = new();
}

public class Media
{
    public int MediaId { get; set; }
    public string Title { get; set; }
    public string Artiste { get; set; }
    public string Path { get; set; }

    public int PlaylistId { get; set; }
    public Playlist Playlist { get; set; }
}