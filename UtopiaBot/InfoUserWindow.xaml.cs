using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UtopiaBot
{
    /// <summary>
    /// Логика взаимодействия для InfoUserWindow.xaml
    /// </summary>
    public partial class InfoUserWindow : MetroWindow
    {
        Classes.Settings _settings;
        MainWindow _window;
        int _id;
        public InfoUserWindow(Classes.Settings settings, int id, MainWindow window)
        {
            InitializeComponent();
            _settings = settings;
            _id = id;
            _window = window;
            LoadUserInfo();
        }

        void LoadUserInfo()
        {
            var user = _settings.users[_id];
            GroupBoxUser.Header = "Информация (" + user.domain + ")";
            NameUser.Content = "Ник: " + user.domain;
            ChatIdUser.Content = "Chat ID: " + user.chatid;
            AuthStatusUser.Content = "Авторизация: " + user.auth.ToString();
            if (user.auth)
            {
                HostUtopiaUser.Text = user.utopia_host + ":" + user.utopia_port;
                TokenUtopiaUser.Password = user.utopia_token;
                UnAuthUser.IsEnabled = true;
            }
            else
            {
                HostUtopiaUser.Text = null;
                TokenUtopiaUser.Password = null;
                UnAuthUser.IsEnabled = false;
            }
        }

        private void TokenUtopiaUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void UnAuthUser_Click(object sender, RoutedEventArgs e)
        {
            var user = _settings.users[_id];
            user.utopia_host = null;
            user.utopia_token = null;
            user.utopia_port = 0;
            user.auth = false;
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings));
            LoadUserInfo();
            _window.LoadUsers();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Theme;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет"
            };

            MessageDialogResult result = await this.ShowMessageAsync("Удалить пользователя", "Вы уверены?",
            MessageDialogStyle.AffirmativeAndNegative, mySettings);
            switch (result)
            {
                case MessageDialogResult.Affirmative:
                    _settings.users.RemoveAt(_id);
                    File.WriteAllText("settings.json", JsonConvert.SerializeObject(_settings));
                    _window.LoadUsers();
                    base.Close();
                    break;
                case MessageDialogResult.Negative:

                    break;
            }
        }
    }
}
