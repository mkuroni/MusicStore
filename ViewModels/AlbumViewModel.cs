
using Avalonia.Media.Imaging;
using MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.ViewModels
{
    /// <summary>
    /// Le viewmodel englobe le model. Pattern commun.
    /// </summary>
    public class AlbumViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructeur d'un albumViewModel
        /// </summary>
        /// <param name="album">On a seulement besoin d'un album</param>
        public AlbumViewModel(Album album)
        {
            _album = album;
        }

        private readonly Album _album;
        private Bitmap? _cover;

        /// <summary>
        /// Nom du groupe ou de l'artiste de l'Album.
        /// </summary>
        public string Artist => _album.Artist;

        /// <summary>
        /// Titre de l'Album.
        /// </summary>
        public string Title => _album.Title;

        /// <summary>
        /// L'image de couverture de l'Album en bitmap pour économiser de l'Espace.
        /// </summary>
        public Bitmap? Cover
        {
            get => _cover;
            private set => this.RaiseAndSetIfChanged(ref _cover, value);
        }


        /// <summary>
        /// Sauvegarde localement notre bibliothèque.
        /// </summary>
        /// <returns>Rien. Attends la completion d'une tâche.</returns>
        public async Task SaveToDiskAsync()
        {
            await _album.SaveAsync();

            if(Cover != null)
            {
                var bitmap = Cover;

                await Task.Run(() =>
                {
                    using (var fs = _album.SaveCoverBitmapStream())
                    {
                        bitmap.Save(fs);
                    }
                });
            }
        }

        /// <summary>
        /// Charge l'image sur un autre thread.
        /// </summary>
        /// <returns>Rien (On attends que Task.Run soit terminé)</returns>
        public async Task LoadCover()
        {
            await using(var imageStream = await _album.LoadCoverBitmapAsync())
            {
                //Créé une image bitmap pour gérer un nombre significatif d'images sans avoir une énorme résolution et garder un ratio.
                Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
        }
    }
}
