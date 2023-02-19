
using MusicStore.Models;
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

        public string Artist => _album.Artist;
        public string Title => _album.Title;

        public AlbumViewModel(Album album)
        {
            _album = album;
        }
    }
}
