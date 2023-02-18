using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace MusicStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();

            BuyMusicCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new MusicStoreViewModel(); //Le contenu qui sera soufflé.
                var result = await ShowDialog.Handle(store); //Le retour async du contenu (AlbumVM ou null), lorsqu'on quittera le MusicStoreViewModel.
            });
        }

        /// <summary>
        /// Commande d'achat.
        /// </summary>
        public ICommand BuyMusicCommand { get; }

        /// <summary>
        /// Interaction qui affiche un magasin de musique et retournera un AlbumViewModel ou rien si annulé.
        /// </summary>
        public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }
    }

}
