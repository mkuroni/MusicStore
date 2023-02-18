using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MusicStore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {

            BuyMusicCommand = ReactiveCommand.Create(() =>
                {
                });
        }
        public ICommand BuyMusicCommand { get; }
    }

}
