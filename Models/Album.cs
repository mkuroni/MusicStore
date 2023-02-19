using iTunesSearch.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicStore.Models
{
    /// <summary>
    /// Représente un Album de la librairie iTunes.
    /// </summary>
    public class Album
    {
        private static HttpClient s_httpClient = new();
        private static iTunesSearchManager s_SearchManager = new();

        private string CachePath => $"./Cache/{Artist} - {Title}";

        public string Artist { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }

        /// <summary>
        /// Constructeur d'un album avec les trois informations qui nous intressent.
        /// </summary>
        /// <param name="artist">Le nom de l'artiste/groupe</param>
        /// <param name="title">Titre de l'album</param>
        /// <param name="coverUrl">Liens vers l'image de couverture</param>
        public Album(string artist, string title, string coverUrl)
        {
            Artist = artist;
            Title = title;
            CoverUrl = coverUrl;
        }

        private static async Task SaveToStreamAsync(Album data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
        }

        public static async Task<Album> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false))!;
        }

        public static async Task<IEnumerable<Album>> LoadCachedAsync()
        {
            if (!Directory.Exists("./Cache"))
                Directory.CreateDirectory("./Cache");

            var results = new List<Album>();

            foreach(var file in Directory.EnumerateFiles("./Cache"))
            {
                if (!string.IsNullOrWhiteSpace(new DirectoryInfo(file).Extension)) continue;

                await using var fs = File.OpenRead(file);
                results.Add(await Album.LoadFromStream(fs).ConfigureAwait(false));
            }

            return results;
        }

        /// <summary>
        /// Effectue une recherche d'albums
        /// </summary>
        /// <param name="searchTerm">Le filtre de la recherche</param>
        /// <returns>Les albums correspondants</returns>
        public static async Task<IEnumerable<Album>> SearchAsync(string searchTerm)
        {
            var query = await s_SearchManager.GetAlbumsAsync(searchTerm).ConfigureAwait(false);

            return query.Albums.Select(x =>
                new Album(x.ArtistName, x.CollectionName, x.ArtworkUrl100.Replace("100x100bb", "600x600bb")));
        }

        /// <summary>
        /// Charge l'image de couverture d'un album.
        /// </summary>
        /// <returns>Retourne un canal pour charger l'image bitmap.</returns>
        public async Task<Stream> LoadCoverBitmapAsync()
        {
            if(File.Exists(CachePath + ".bmp"))
            {
                return File.OpenRead(CachePath + ".bmp");
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);
            }
        }

        public async Task SaveAsync()
        {
            if (!Directory.Exists("./Cache"))
                Directory.CreateDirectory("./Cache");

            using(var fs = File.OpenWrite(CachePath))
            {
                await SaveToStreamAsync(this, fs);
            }
        }

        public Stream SaveCoverBitmapStream()
        {
            return File.OpenWrite(CachePath + ".bmp");
        }


    }
}
