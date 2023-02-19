using MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace MusicStore.ViewModels
{
    /// <summary>
    /// Logique pour affichage de la fen�tre principale.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructeur principal
        /// </summary>
        public MainWindowViewModel()
        {
            RxApp.MainThreadScheduler.Schedule(LoadAlbums);

            this.WhenAnyValue(x => x.Albums.Count) //� chaque changements du count d'albums, CollectionEmpty est mis � jour
                .Subscribe(x => CollectionEmpty = x == 0);

            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel(); //Le contenu qui sera souffl�.
                var result = await ShowDialog.Handle(store); //Le retour async du contenu (AlbumVM ou null), lorsqu'on quittera le MusicStoreViewModel.
                
                if(result != null) //Si on a achet� un album, on l'Ajoute � notre collection d'albums.
                {
                    Albums.Add(result);
                    await result.SaveToDiskAsync(); //Enregistre le changement localement.
                }
            });
        }

        private bool _collectionEmpty;

        /// <summary>
        /// Accesseur pour savoir si on a achet� des albums ou non.
        /// </summary>
        public bool CollectionEmpty
        {
            get => _collectionEmpty;
            set => this.RaiseAndSetIfChanged(ref _collectionEmpty, value);
        }

        /// <summary>
        /// Notre liste d'albums achet�s observables.
        /// </summary>
        public ObservableCollection<AlbumViewModel> Albums { get; } = new();

        /// <summary>
        /// Commande d'achat.
        /// </summary>
        public ICommand BuyMusicCommand { get; }

        /// <summary>
        /// Interaction qui affiche un magasin de musique et retournera un AlbumViewModel ou rien si annul�.
        /// </summary>
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

        /// <summary>
        /// Charge la liste d'Albums de l'API et les transforment en AlbumViewModels � ajouter � notre liste d'albums.
        /// </summary>
        private async void LoadAlbums()
        {
            var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));
            foreach(var album in albums)
            {
                Albums.Add(album);
            }

            foreach(var album in Albums.ToList())
            {
                await album.LoadCover();
            }
        }
    }

}
