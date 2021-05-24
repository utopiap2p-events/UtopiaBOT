using MahApps.Metro.Controls;
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
using Telegram.Bot;
using Telegram.Bot.Args;
using Newtonsoft.Json;
using System.Diagnostics;

namespace UtopiaBot
{
    /// <summary>
    /// Логика взаимодействия для SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : MetroWindow
    {
        static ITelegramBotClient TGBot;
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
                TGbotLog.Text += _prefix + " " + text + "\r\n";
            });
        }

        bool CheckTGValid (string token)
        {
            try
            {
                TGBot = new TelegramBotClient(token);
                var me = TGBot.GetMeAsync().Result;
            }
            catch (System.AggregateException a)
            {
                return false;
            }
            catch (System.ArgumentException a)
            {
                return false;
            }
            return true;
        }
        
        public SetupWindow()
        {
            if (!File.Exists("settings.json"))
            {
                InitializeComponent();
            }
            else
            {
                var json = JsonConvert.DeserializeObject<Classes.Settings>(File.ReadAllText("settings.json"));
                if (json != null)
                {
                    if (json.tg_token != "*del*")
                    {
                        if (CheckTGValid(json.tg_token))
                        {
                            base.Hide();
                            MainWindow window = new MainWindow(json.tg_token);
                            window.Show();
                        }
                        else
                        {
                            InitializeComponent();
                            AddLog("Прошлый токен не валидный, добавьте заного!", 1);
                        }
                    }
                    else
                    {
                        InitializeComponent();
                    }
                }
                else
                {
                    File.Delete("settings.json");
                    InitializeComponent();
                    AddLog("Файл настроек был поврежден, заполните все заного!", 1);
                }
            }
        }

        private void SaveSettings(string tg_token)
        {
            Classes.Settings cfg = new Classes.Settings();

            if (!File.Exists("settings.json"))
            {
                cfg.tg_token = tg_token;
            }
            else
            {
                var settings = JsonConvert.DeserializeObject<Classes.Settings>(File.ReadAllText("settings.json"));
                cfg.tg_token = tg_token;
                cfg.users = settings.users;
            }
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(cfg));

        }

        private void AuthTGBot_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                AddLog("Попытка авторизации...");
                Dispatcher.BeginInvoke((Action)delegate { AuthTGBot.IsEnabled = false; });
                try
                {
                    TGBot = new TelegramBotClient(TGToken.Password);
                    var me = TGBot.GetMeAsync().Result;
                }
                catch (System.AggregateException a)
                {
                    AddLog("Не верный токен!", 1);
                    Dispatcher.BeginInvoke((Action)delegate { AuthTGBot.IsEnabled = true; TGToken.Password = ""; });
                    return;
                }
                catch (System.ArgumentException a)
                {
                    AddLog("Не верный формат токена!", 1);
                    Dispatcher.BeginInvoke((Action)delegate { AuthTGBot.IsEnabled = true; TGToken.Password = ""; });
                    return;
                }
                AddLog("Успешно, токен верный!\r\nМожете переходить к главному окну программы.", 2);
                SaveSettings(TGToken.Password);
                Dispatcher.BeginInvoke((Action)delegate { NextBtn.IsEnabled = true; TGToken.IsEnabled = false; });
            });
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            base.Hide();
            MainWindow window = new MainWindow(TGToken.Password);
            window.Show();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://teletype.in/@vityasteam/I4W7Pt2zqRS");
        }
    }
}
