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

        /// <summary>
        /// Pour utiliser une API.
        /// </summary>
        private static HttpClient s_httpClient = new();

        /// <summary>
        /// Pour utiliser l'API de recherche iTunes
        /// </summary>
        private static iTunesSearchManager s_SearchManager = new();

        /// <summary>
        /// Le chemin pour retrouver/écrire l'album en cache.
        /// </summary>
        private string CachePath => $"./Cache/{Artist} - {Title}";

        /// <summary>
        /// Le nom de l'artiste de l'Album.
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// Le titre de l'album.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// L'adresse pour retrouver l'image de couverture de l'Album.
        /// </summary>
        public string CoverUrl { get; set; }

        /// <summary>
        /// Sauvegarde un canal pour retrouver l'image de couverture
        /// </summary>
        /// <returns></returns>
        public Stream SaveCoverBitmapStream()
        {
            return File.OpenWrite(CachePath + ".bmp");
        }

        /// <summary>
        /// Charge l'image de couverture d'un album.
        /// </summary>
        /// <returns>Retourne un canal pour charger l'image bitmap.</returns>
        public async Task<Stream> LoadCoverBitmapAsync()
        {
            if (File.Exists(CachePath + ".bmp"))
            {
                return File.OpenRead(CachePath + ".bmp");
            }
            else
            {
                var data = await s_httpClient.GetByteArrayAsync(CoverUrl);
                return new MemoryStream(data);
            }
        }

        /// <summary>
        /// Sauvegarde des albums localement.
        /// </summary>
        /// <returns>Attends que la sauvegarde ait été effectuée</returns>
        public async Task SaveAsync()
        {
            if (!Directory.Exists("./Cache"))
                Directory.CreateDirectory("./Cache");

            using (var fs = File.OpenWrite(CachePath))
            {
                await SaveToStreamAsync(this, fs);
            }
        }

        /// <summary>
        /// Sauvegarde un album dans le canal de sauvegarde locale.
        /// </summary>
        /// <param name="data">L'album à sauvegardé</param>
        /// <param name="stream">Le canal d'écriture</param>
        /// <returns>La tâche d'écriture</returns>
        private static async Task SaveToStreamAsync(Album data, Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, data).ConfigureAwait(false);
        }

        /// <summary>
        /// Charge les données du canal de sauvegarde local.
        /// </summary>
        /// <param name="stream">Le canal de sauvegarde</param>
        /// <returns>Les albums trouvés</returns>
        public static async Task<Album> LoadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Album>(stream).ConfigureAwait(false))!;
        }

        /// <summary>
        /// Retrouve les albums de la cache.
        /// </summary>
        /// <returns>Les albums sauvegardés</returns>
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
    }
}
