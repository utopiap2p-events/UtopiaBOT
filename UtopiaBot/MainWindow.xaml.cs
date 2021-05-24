using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace UtopiaBot
{
    public partial class MainWindow : MetroWindow
    {
        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        bool started = false;
        static ITelegramBotClient TGBot;
        private string tg_token;
        public string StartUpPath = Environment.CurrentDirectory;
        private bool notify = false;
        public Classes.Settings settings = new Classes.Settings();

        public MainWindow(string tg_token)
        {
            InitializeComponent();
            settings.tg_token = tg_token;
            settings.users = new List<Classes.User>();
            TGBot = new TelegramBotClient(tg_token);
            InitWatcher();
            LoadUsers();
            LoadBotInfo();

            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/UtopiaBot;component/Images/icon.ico")).Stream;
            ni.Icon = new System.Drawing.Icon(iconStream);
            ni.Text = "Utopia BOT / Дв. клик чтобы открыть";
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        void LoadBotInfo()
        {
            var me = TGBot.GetMeAsync().Result;
            BotName.Content = "Назв: " + me.FirstName + " " + me.LastName;
            BotID.Content = "ID: " + me.Id;
        }

        private void AddLog(string text, int prefix = 0)
        {
            string _prefix;
            switch (prefix)
            {
                case 0: _prefix = "[INFO]"; break;
                case 1: _prefix = "[ERR]"; break;
                case 2: _prefix = "[SUCC]"; break;

                default: _prefix = "[INFO]"; break;
            }

            Dispatcher.BeginInvoke((Action)delegate
            {
                LogBox.Text += _prefix + " " + text + "\r\n";

                if (AutScrollLog.IsChecked == true)
                {
                    LogBox.SelectionStart = LogBox.Text.Length;
                    LogBox.ScrollToEnd();
                }
            });
        }

        private void InitWatcher()
        {
            TGBot.OnMessage += MsgReciever;
            TGBot.OnCallbackQuery += TGBot_OnCallbackQuery;
            //TGBot.StartReceiving();
        }

        private async void TGBot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var callbackQuery = e.CallbackQuery;
            switch (callbackQuery.Data) {
                case "close":
                await TGBot.DeleteMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId
                    );
                    break;
            }
        
        }

        /* private async void TGBot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
         {
             var callbackQuery = e.CallbackQuery;

             await TGBot.DeleteMessageAsync(
                 e.CallbackQuery.Message.Chat.Id,
                 e.CallbackQuery.Message.MessageId
                 );
         }*/

        bool CheckUserExists(int chatid)
        {
            for(int i = 0; i < settings.users.Count; i++)
            {
                if(settings.users[i].chatid == chatid)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadUsers()
        {
            if (!File.Exists("settings.json") || File.ReadAllText("settings.json") == "")
            {
                base.Hide();
                SetupWindow window = new SetupWindow();
                window.Show();
                return;
            }
            Dispatcher.BeginInvoke((Action)delegate
            {
                lstUsers.Items.Clear();
                settings.users.Clear();

                var users = JsonConvert.DeserializeObject<Classes.Settings>(File.ReadAllText("settings.json"));
                if (users.users != null)
                {
                    for (int i = 0; i < users.users.Count; i++)
                    {
                        string auth = "-";
                        if (users.users[i].auth)
                        {
                            auth = "+";
                        }
                        else
                        {
                            auth = "-";
                        }
                        lstUsers.Items.Add(new Classes.lstUser { AuthUser = auth, ChatIdUser = users.users[i].chatid, DomainUser = users.users[i].domain, IDUser = users.users[i].id });
                        settings.users.Add(new Classes.User { auth = users.users[i].auth, chatid = users.users[i].chatid, domain = users.users[i].domain, id = users.users[i].id, utopia_host = users.users[i].utopia_host, utopia_port = users.users[i].utopia_port, utopia_token = users.users[i].utopia_token });
                    }
                }
            });
        }

        void AddUser(string nick, int chatid)
        {
            settings.users.Add(new Classes.User { auth = false, chatid = chatid, domain = nick, id = settings.users.Count + 1 });
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
            LoadUsers();
        }

        public void Notify(string title, string text, int type)
        {
            if (notify == false)
                return;
            var notificationManager = new NotificationManager();
            switch (type)
            {
                case 0:
                    notificationManager.Show(new NotificationContent
                    {
                        Title = title,
                        Message = text,
                        Type = NotificationType.Success,

                    });
                    break;
                case 1:
                    notificationManager.Show(new NotificationContent
                    {
                        Title = title,
                        Message = text,
                        Type = NotificationType.Error,

                    });
                    break;
                case 2:
                    notificationManager.Show(new NotificationContent
                    {
                        Title = title,
                        Message = text,
                        Type = NotificationType.Warning,

                    });
                    break;
                case 3:
                    notificationManager.Show(new NotificationContent
                    {
                        Title = title,
                        Message = text,
                        Type = NotificationType.Information,

                    });
                    break;
                default:
                    notificationManager.Show(new NotificationContent
                    {
                        Title = title,
                        Message = text,
                        Type = NotificationType.Information,

                    });
                    break;
            }
        }

        public int FindUser(int chatid)
        {
            for(int i = 0; i < settings.users.Count + 1; i++)
            {
                var user = settings.users[i];
                if(user.chatid == chatid)
                {
                    return user.id;
                }
            }
            return 0;
        }

        public string RandString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private async void MsgReciever(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (!CheckUserExists(Convert.ToInt32(e.Message.Chat.Id)))
            {
                AddLog("Новый пользователь: " + e.Message.Chat.Username);
                Notify("Информация", "Новый пользователь: " + e.Message.Chat.Username, 3);
                AddUser(e.Message.Chat.Username, Convert.ToInt32(e.Message.Chat.Id));
                Thread.Sleep(100);
            }

            AddLog(String.Format("Новое сообщение ({0}): {1}", e.Message.Chat.Username, e.Message.Text));
            
            string body = "";
            int id = (FindUser(Convert.ToInt32(e.Message.Chat.Id)) - 1);
            var user = settings.users[id];

            switch (e.Message.Text.Split(' ')[0].ToLower())
            {
                case "/auth":
                    if (!settings.users[id].auth)
                    {
                        string[] auth_data = e.Message.Text.Split(' ');
                        if (auth_data.Length <= 2)
                        {
                            body = "\u274cНеверный синтаксис!" +
                                "\n*Использование:* /auth <Хост> <Токен> (/auth 127.0.0.1:20000 6B15...)";
                        }
                        else
                        {
                            switch (Utopia.Methods.CheckValid(auth_data[1], auth_data[2]))
                            {
                                case 200:
                                    //Auth success
                                    Notify("Аккаунт Utopia", "Пользователь (" + settings.users[id].domain + ") добавил аккаунт Utopia", 3);
                                    body = "\u2705Авторизация успешна!" +
                                        "\n\n" +
                                        "*Вам стал доступен основной функционал.*";
                                    string[] apidata = auth_data[1].Split(':');
                                    settings.users[id].auth = true;
                                    settings.users[id].utopia_host = apidata[0];
                                    settings.users[id].utopia_port = Convert.ToInt32(apidata[1]);
                                    settings.users[id].utopia_token = auth_data[2];
                                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
                                    LoadUsers();
                                    break;
                                case 300:
                                    body = "\u274cНевозможно подключится к серверу" +
                                        "\n\n" +
                                        "*Список возможных причин:*" +
                                        "\n- Вы неправильно настроили режим API" +
                                        "\n- Порт API - закрыт";
                                    break;
                                case 400:
                                    body = "\u274cВведеный токен - не верный";
                                    break;
                                case 500:
                                    body = "\u274cНеизвестная ошибка" +
                                        "\n\n" +
                                        "*Обратитесь к администратору*";
                                    break;
                                default:
                                    body = "\u274cНеизвестная ошибка" +
                                        "\n\n" +
                                        "*Обратитесь к администратору*";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        body = "\u274c*Ваш аккаунт уже подключен*";
                    }
                    break;
                case "/account":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        body = Utopia.Methods.getAccountInfo(user.utopia_host + ":" + user.utopia_port, user.utopia_token);
                    }
                    break;
                case "/setstatus":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        string[] status_data = e.Message.Text.Split(' ');
                        if (status_data.Length <= 1)
                        {
                            body = "\u274cНеверный синтаксис!" +
                                "\n*Использование:* /setstatus <ID Статуса>" +
                                "\n" +
                                "\n\ud83c\udd94*ID Статусов:*" +
                                "\n1 - В сети" +
                                "\n2 - Нет на месте" +
                                "\n3 - Не беспокоить" +
                                "\n4 - Невидимка" +
                                "\n5 - Не в сети";
                        }
                        else
                        {
                            body = Utopia.Methods.setStatus(user.utopia_host + ":" + user.utopia_port, user.utopia_token, status_data[1]);
                        }
                    }
                    break;
                case "/unauth":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        user.auth = false;
                        user.utopia_host = null;
                        user.utopia_port = 0;
                        user.utopia_token = null;
                        File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
                        LoadUsers();
                        if(user.auth == false)
                        {
                            Notify("Аккаунт Utopia", "Пользователь (" + settings.users[id].domain + ") удалил аккаунт Utopia", 2);
                            AddLog(String.Format("Пользователь ({0}) удалил авторизацию", user.domain));
                            body = "\u2705Ваш аккаунт был удален!" +
                                "\n" +
                                "\n*Теперь вы можете повторно авторизоватся*";
                        }
                        else
                        {
                            body = "\u274cОшибка удаления аккаунта!" +
                                "\n" +
                                "\n*Обратитесь к администратору!*";
                        }
                    }
                    break;
                case "/cards":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        string[] card_data = e.Message.Text.Split(' ');
                        if (card_data.Length <= 1)
                        {
                            body =
                                "\ud83d\udcb3Справка по _вашим_ картам" +
                                "\n*Использование:* /cards <Действие> <Аргументы>" +
                                "\n" +
                                "\n\ud83d\udcce*Список действий:*" +
                                "\nlist - Список карт, аргументы не требуются" +
                                "\ndel - Удалить карту (Аргументы: ID Карты)" +
                                "\nadd - Создать новую карту (Аргументы: Назв. карты)" +
                                "\n" +
                                "\n*Пример:* /cards add TestCard";
                        }
                        else
                        {
                            string arg = "";
                            if (card_data.Length >= 3)
                            {
                                arg = card_data[2];
                            }
                            body = Utopia.Methods.getCards(user.utopia_host + ":" + user.utopia_port, user.utopia_token, card_data[1], arg);

                        }

                    }
                    break;
                case "/finance":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        body = Utopia.Methods.getFinanceSystemInformation(user.utopia_host + ":" + user.utopia_port, user.utopia_token);
                    }
                    break;
                case "/contacts":
                    if (user.auth != true)
                    {
                        body = "\u274cУ вас нет прав для выполнения этого действия";
                    }
                    else
                    {
                        string[] status_data = e.Message.Text.Split(' ');
                        if (status_data.Length <= 1)
                        {
                            body = 
                                "\ud83d\udcd4Справка по _вашим_ контактам" +
                                "\n*Использование:* /contacts <Действие> <Аргументы>" +
                                "\n" +
                                "\n\ud83d\udcce*Список действий:*" +
                                "\nlist - Список контактов, аргументы не требуются" +
                                "\ndel - Удалить контакт (Аргументы: публичный ключ)" +
                                "\nava - Аватар пользователя (Аргументы: публичный ключ)" +
                                "\n" +
                                "\n*Пример:* /contacts ava 1B7F...";
                        }
                        else
                        {
                            string pb_key = "";
                            if(status_data.Length >= 3) { 
                                pb_key = status_data[2];
                            }
                            body = Utopia.Methods.getContacts(user.utopia_host + ":" + user.utopia_port, user.utopia_token, status_data[1], pb_key);
                            if(status_data[1] == "ava")
                            {
                                if(body == "*err*")
                                {
                                    body = "\u274cОшибка получения аватара пользователя!" +
                                        "\n" +
                                        "\n*Возможно, вы неправильно указали* _публичный ключ_";
                                }
                                else
                                {
                                    byte[] bytes = Convert.FromBase64String(body);
                                    string randstr = RandString(6);
                                    if (!Directory.Exists("img"))
                                    {
                                        Directory.CreateDirectory("img");
                                    }
                                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(bytes));
                                        image.Save("img\\" + randstr + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);  // Or Png
                                    string img_path = StartUpPath + "\\img\\" + randstr + ".jpg";

                                    FileStream fs = System.IO.File.OpenRead(img_path);
                                    InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, "ava.png");

                                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                        // first row
                                        new []
                                        {
                                            InlineKeyboardButton.WithCallbackData("\u274cЗакрыть", "close"),
                                            InlineKeyboardButton.WithUrl("Сайт Utopia 2P2", "https://u.is/ru")
                                        },
                                    });
                                    await TGBot.SendPhotoAsync(
                                      chatId: e.Message.Chat,
                                      photo: inputOnlineFile,
                                      caption: "\ud83d\udc64*Аватар пользователя*",
                                      parseMode: ParseMode.Markdown
                                    );
                                    File.Delete(img_path);
                                    return;
                                }
                            }
                        }
                    }
                    break;
                default:
                    if (settings.users[id].auth != true)
                    {
                        body =
                            "\n\u26a1\ufe0f*Приветствую*" +
                            "\nЯ - бот, созданный специально для" +
                            "\nхакатона *Utopia.*" +
                            "\n" +
                            "\n\ud83d\udc64Статус: *Не авторизован*" +
                            "\n\ud83c\udd94ID: " + id +
                            "\n" +
                            "\n\ud83d\udee0*Список доступных команд:*" +
                            "\n/auth <Хост> <Токен> (/auth 127.0.0.1:20000 6B15...)";
                    }
                    else
                    {
                        body =
                            "\n\u26a1\ufe0f*Приветствую*" +
                            "\nЯ - бот, созданный специально для" +
                            "\nхакатона *Utopia.*" +
                            "\n" +
                            "\n\ud83d\udc64Статус: *Авторизован*" +
                            "\n\ud83c\udd94ID: " + id +
                            "\n" +
                            "\n\ud83d\udee0*Список доступных команд:*" +
                            "\n" +
                            "\n\ud83d\udc64*Аккаунт*" +
                            "\n/account - Информация об аккаунте" +
                            "\n/setstatus <ID Статуса> - Установить статус" +
                            "\n" +
                            "\n\ud83d\udcb4*Финансы*" +
                            "\n/cards - Справка по _вашим_ картам" +
                            "\n/finance - Финансовая система"+
                            "\n" +
                            "\n\ud83d\udcd4*Контакты*" +
                            "\n/contacts - Справка по _вашим_ контактам" +
                            "\n" +
                            "\n\u270f\ufe0f*Другое*" +
                            "\n/unauth - Удалить текущий аккаунт _Utopia_";
                    }
                    await TGBot.SendTextMessageAsync(
                      chatId: e.Message.Chat, // or a chat id: 123456789
                      text: body,
                      parseMode: ParseMode.Markdown,
                      disableNotification: true,
                      replyToMessageId: e.Message.MessageId,
                      replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                        "Сайт Utopia 2P2",
                        "https://u.is/ru"
                      ))
                    );
                    return;
            }
            SendMessage(e.Message.Chat, body, ParseMode.Markdown, e.Message.MessageId);
        }

        private async void SendMessage(Telegram.Bot.Types.Chat chat, string text, ParseMode parseMode = ParseMode.Default, int replyid = 0)
        {
            try
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("\u274cЗакрыть", "close"),
                        InlineKeyboardButton.WithUrl("Сайт Utopia 2P2", "https://u.is/ru")
                    },
                });
                await TGBot.SendTextMessageAsync(
                  chatId: chat,
                  text: text,
                  parseMode: parseMode,
                  replyToMessageId: replyid,
                  replyMarkup: inlineKeyboard
                );
            }
            catch
            {
                AddLog("Ошибка отправки сообщения!", 1);
                Notify("Ошибка", "Ошибка отправки сообщения!", 1);
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if(lstUsers.SelectedIndex == -1)
            {
                ItemDel.IsEnabled = false;
                ItemInfo.IsEnabled = false;
            }
            else
            {
                ItemDel.IsEnabled = true;
                ItemInfo.IsEnabled = true;
            }
        }

        private void ItemDel_Click(object sender, RoutedEventArgs e)
        {
            var user = settings.users[lstUsers.SelectedIndex];
            AddLog(String.Format("Пользователь ({0}) был удален из базы!", user.domain));
            settings.users.RemoveAt(lstUsers.SelectedIndex);
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
            LoadUsers();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                FirstAuxiliaryButtonText = "Отмена"
            };

            MessageDialogResult result = await this.ShowMessageAsync("Смена бота", "Вы собираетесь сменить бота.\r\nУдалить текущих пользователей?",
            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);
            switch (result)
            {
                case MessageDialogResult.FirstAuxiliary:
                    //Don't logout
                    return;
                case MessageDialogResult.Affirmative:
                    //Logout & Delete all user's
                    settings.tg_token = "*del*";
                    settings.users = null;
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
                    break;
                case MessageDialogResult.Negative:
                    //Logout
                    settings.tg_token = "*del*";
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
                    break;
            }
            if (started)
            {
                TGBot.StopReceiving();
            }
            base.Hide();
            SetupWindow window = new SetupWindow();
            window.Show();
        }

        private void ItemInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoUserWindow window = new InfoUserWindow(settings, lstUsers.SelectedIndex, this);
            window.ShowDialog();
        }

        private void StateBotBox_Toggled(object sender, RoutedEventArgs e)
        {
            if(StateBotBox.IsOn == true)
            {
                TGBot.StartReceiving();
            }
            else if(StateBotBox.IsOn == false)
            {
                TGBot.StopReceiving();
            }
        }

        private void NotifyBox_Checked(object sender, RoutedEventArgs e)
        {
            if(NotifyBox.IsChecked == true)
            {
                notify = true;
                Notify("Оповещение", "Оповещения включены!", 3);
            }
            else
            {
                notify = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ni.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            ni.BalloonTipText = "Программа скрыта в трей. Дв. клик по иконке отроет глав. меню";
            ni.BalloonTipTitle = "Программа скрыта";
            ni.Visible = true;
            ni.ShowBalloonTip(30000);


            base.Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            LogBox.Text = "";
        }

        private void LogBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LogBox.Text != "")
            {
                ClearBtn.IsEnabled = true;
                SaveBtn.IsEnabled = true;
            }
            else
            {
                ClearBtn.IsEnabled = false;
                SaveBtn.IsEnabled = false;

            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Log file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            File.WriteAllText(saveFileDialog.FileName, LogBox.Text);
        }
    }
}
