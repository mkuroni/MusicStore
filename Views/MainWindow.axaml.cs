using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MusicStore.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace MusicStore.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        }


        /// <summary>
        /// Affiche en dialog avec un résultat en retour.
        /// </summary>
        /// <param name="interaction">L'affichage de dialog avec un model MusicStoreVM et un AlbumVM en retour</param>
        /// <returns>Un AlbumViewModel (album à acheter) ou null</returns>
        private async Task DoShowDialogAsync(InteractionContext<MusicStoreViewModel, AlbumViewModel?> interaction)
        {
            var dialog = new MusicStoreWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<AlbumViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}
