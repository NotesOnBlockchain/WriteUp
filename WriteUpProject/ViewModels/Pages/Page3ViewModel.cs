using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WriteUpProject.Navigation;

namespace WriteUpProject.ViewModels.Pages
{
    public class Page3ViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private string _dataFromPage1;
        private string _dataFromPage2;

        public string DataFromPage1
        {
            get => _dataFromPage1;
            set => SetProperty(ref _dataFromPage1, value);
        }

        public string DataFromPage2
        {
            get => _dataFromPage2;
            set => SetProperty(ref _dataFromPage2, value);
        }

        public ICommand NavigateBackCommand { get; }

        public Page3ViewModel(NavigationService navigationService, string dataFromPage1, string dataFromPage2)
        {
            _navigationService = navigationService;
            DataFromPage1 = dataFromPage1;
            DataFromPage2 = dataFromPage2;

            NavigateBackCommand = ReactiveCommand.Create(_navigationService.NavigateBack);
        }
    }
}
