using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MusicStore.ViewModels;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace MusicStore.Views
{
    /// <summary>
    /// Vue réactive du MusicStoreViewModel.
    /// </summary>
    public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
    {
        public MusicStoreWindow()
        {
            InitializeComponent();

            //Subscribe à l'Appel d'une commande pour provoquer une réaction sur la vue courante.
            //Le résultat de BuyMusicCommand sera retourné à l'appel de ShowDialog dans MainWindow.
            this.WhenActivated(d => d(ViewModel!.BuyMusicCommand.Subscribe(Close)));
        }
    }
}
