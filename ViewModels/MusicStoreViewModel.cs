using Microsoft.VisualBasic;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicStore.ViewModels
{
    public class MusicStoreViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string? _searchText;
        private AlbumViewModel? _selectedAlbum;

        public MusicStoreViewModel()
        {
            SearchResults.Add(new AlbumViewModel());
            SearchResults.Add(new AlbumViewModel());
            SearchResults.Add(new AlbumViewModel());
        }

        /// <summary>
        /// Accesseur pour le texte à rechercher
        /// </summary>
        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        /// <summary>
        /// Accesseur pour afficher une progression visuelle
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        /// <summary>
        /// Le résultat de recherche à afficher. (Pas besoin de this.RaiseAndSet.. car observable)
        /// </summary>
        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new(); //Il faut absolument l'initialiser

        /// <summary>
        /// Accesseur sur l'album sélectionné dans notre liste
        /// </summary>
        public AlbumViewModel? SelectedAlbum
        {
            get => _selectedAlbum;
            set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }
    }
}
