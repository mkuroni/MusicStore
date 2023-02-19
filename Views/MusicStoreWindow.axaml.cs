using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MusicStore.ViewModels;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace MusicStore.Views
{
    /// <summary>
    /// Vue r�active du MusicStoreViewModel.
    /// </summary>
    public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
    {
        public MusicStoreWindow()
        {
            InitializeComponent();

            //Subscribe � l'Appel d'une commande pour provoquer une r�action sur la vue courante.
            //Le r�sultat de BuyMusicCommand sera retourn� � l'appel de ShowDialog dans MainWindow.
            this.WhenActivated(d => d(ViewModel!.BuyMusicCommand.Subscribe(Close)));
        }
    }
}
