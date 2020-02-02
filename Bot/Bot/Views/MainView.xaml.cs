
using Bot.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bot.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentPage
    {
        private readonly MainViewModel _mainViewModel;
        public MainView()
        {
            InitializeComponent();
            _mainViewModel = BindingContext as MainViewModel;
            _mainViewModel.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var messages = _mainViewModel.Messages;
            var newMessage = messages[messages.Count - 1];
            Device.BeginInvokeOnMainThread(() =>
            {
                chatListView.ScrollTo(newMessage, ScrollToPosition.End, true);
            });
        }
    }
}