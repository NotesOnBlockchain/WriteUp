using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteUpProject.ViewModels;

namespace WriteUpProject.Navigation
{
    public class NavigationService
    {
        private readonly Stack<ViewModelBase> _navigationStack = new();
        public event Action<ViewModelBase>? CurrentViewModelChanged;

        public void NavigateTo(ViewModelBase viewModel)
        {
            _navigationStack.Push(viewModel);
            CurrentViewModelChanged?.Invoke(viewModel);
        }

        public void NavigateBack()
        {
            if (_navigationStack.Count > 1)
            {
                _navigationStack.Pop();
                var previousViewModel = _navigationStack.Peek();
                CurrentViewModelChanged?.Invoke(previousViewModel);
            }
        }

        public bool CanNavigateBack => _navigationStack.Count > 1;
    }
}
