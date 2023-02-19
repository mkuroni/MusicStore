using Microsoft.VisualBasic;
using MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicStore.ViewModels
{
    public class MusicStoreViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string? _searchText;
        private AlbumViewModel? _selectedAlbum;
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Effectue la recherche selon l'entrée de l'utilisateur, puis met le contenu dans le résultat.
        /// </summary>
        /// <param name="s"></param>
        private async void DoSearch(string s)
        {
            IsBusy = true; //Pour l'indicateur visuel
            SearchResults.Clear();

            if(!string.IsNullOrWhiteSpace(s)) 
            {
                var albums = await Album.SearchAsync(s);

                foreach (var album in albums) 
                {
                    var vm = new AlbumViewModel(album); //La liste d'albums est une liste de viewmodel
                    SearchResults.Add(vm);
                }
            }
            IsBusy = false; //Poiur l'indicateur visuel
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

        /// <summary>
        /// Commande qui n'a pas besoin de paramètres, mais qui retourne un AlbumViewModel ou null.
        /// </summary>
        public ReactiveCommand<Unit, AlbumViewModel?> BuyMusicCommand { get; }
        public MusicStoreViewModel()
        {
            this.WhenAnyValue(x => x.SearchText) //Donné grace au ReactiveUI, prends la lambda de la propriété à observer.
                .Throttle(TimeSpan.FromMilliseconds(400)) //On ne veux pas déclancher une recherche trop rapidement si l'utilisateur tape vite (Un délais).
                .ObserveOn(RxApp.MainThreadScheduler) //S'assure de rester sur le thread du UI.
                .Subscribe(DoSearch!);

            BuyMusicCommand = ReactiveCommand.Create(() =>
            {
                return SelectedAlbum;
            });
        }
    }
}
