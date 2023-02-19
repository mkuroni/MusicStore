
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
        private readonly Album _album;
        private Bitmap? _cover;

        public string Artist => _album.Artist;
        public string Title => _album.Title;

        public Bitmap? Cover
        {
            get => _cover;
            private set => this.RaiseAndSetIfChanged(ref _cover, value);
        }

        public AlbumViewModel(Album album)
        {
            _album = album;
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
