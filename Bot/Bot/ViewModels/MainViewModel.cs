using Bot.Helpers;
using Bot.Models;
using Bot.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Bot.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _message;

        public BotServiceHelper _botServiceHelper;
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public int MyProperty { get; set; }
        public ICommand SendCommand { get; set; }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Init().ConfigureAwait(false);
        }

        private void _botServiceHelper_MessageRecieved(object sender, BotResponseEventArgs e)
        {
            e.Activities?.Where(activity => activity.From.Id != "user1")
                .ForEach(activity => Messages.Add(new ChatMessage
                {
                    Text = activity.Text,
                    IsIncoming = true
                }));
        }

        private async Task Init()
        {
            SendCommand = new Command(async () => await Send());
            Messages = new ObservableCollection<ChatMessage>();
            _botServiceHelper = new BotServiceHelper();
            await _botServiceHelper.CreateConversation();
            _botServiceHelper.MessageRecieved += _botServiceHelper_MessageRecieved;
        }

        private async Task Send()
        {
            Messages.Add(new ChatMessage()
            {
                Text = Message,
                IsIncoming = false
            });

            await _botServiceHelper.SendActivity(_message);
            Message = string.Empty;
        }
    }
}
