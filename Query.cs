using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player
{
    class Query
    {

        private MediaPlayerContext _dbContext;

        public Query(MediaPlayerContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public void CreatePlaylist(string name)
        {
            _dbContext.Add(new Playlist { Name = name });
            _dbContext.SaveChanges();
        }

        public Playlist GetPlaylist(int id) 
        {
            var playlist = _dbContext.Playlists.FirstOrDefault(p => p.PlaylistId == id);

            _dbContext.Entry(playlist).Collection(p => p.Medias).Load();

            return playlist;
        }

        public void AddMedia(Media media, int playlist_id)
        {
            var playlist = GetPlaylist(playlist_id);

            if (playlist != null)
            {
                playlist.Medias.Add(media);

                _dbContext.SaveChanges();
            }
        }

        public void DeletePlaylist(int id)
        {
            var playlist = GetPlaylist(id);

            if ( playlist != null)
            {
                _dbContext.Remove(playlist);

                _dbContext.SaveChanges();
            }

        }

        public List<Playlist> GetAllPlaylist()
        {
            var playlists = _dbContext.Playlists.ToList();

            foreach (var playlist in playlists)
            {
                _dbContext.Entry(playlist).Collection(p => p.Medias).Load();
            }

            return playlists;
        }

        public void DeleteMedia(int id)
        {
            var media = _dbContext.Medias.FirstOrDefault(m => m.MediaId == id);

            if (media != null)
            {
                _dbContext.Remove(media);

                _dbContext.SaveChanges();
            }
        }


    }
}
