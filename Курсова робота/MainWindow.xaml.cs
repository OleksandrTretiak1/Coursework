using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Курсова_робота
{
    public partial class MainWindow : Window
    {
        private bool isDarkMode = false;

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(34, 34, 34));
                Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); 
                ThemeToggleButton.Content = "☀️";
            }
            else
            {
                Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); 
                Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
                ThemeToggleButton.Content = "🌙";
            }
        }

        private BotResponse botResponse = new EmojiResponse();
        public MainWindow()
        {
            InitializeComponent();

            ChatHistory.Items.Add("Бот: Доброго дня, чим я можу вам допомогти?😀");

            UserInput.Text = "Запитайте будь-що";
            UserInput.Foreground = Brushes.Gray;

            UserInput.GotFocus += (s, e) =>
            {
                if (UserInput.Text == "Запитайте будь-що")
                {
                    UserInput.Text = "";
                    UserInput.Foreground = Brushes.Black;
                }
            };

            UserInput.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(UserInput.Text))
                {
                    UserInput.Text = "Запитайте будь-що";
                    UserInput.Foreground = Brushes.Gray;
                }
            };

            UserInput.KeyDown += UserInput_KeyDown;

            //Loaded += (s, e) => UserInput.Focus(); Якщо нада відразу активувать діалог
        }



        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Max(12, this.ActualHeight / 40);
            ChatHistory.Tag = newFontSize;

            foreach (var item in FindVisualChildren<Button>(this))
            {
                if (item.Name != "ThemeToggleButton")
                {
                    item.FontSize = newFontSize;
                    item.MinWidth = Math.Max(80, this.ActualWidth / 10);
                }
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserInput.Text == "Запитайте будь-що" || string.IsNullOrWhiteSpace(UserInput.Text))
            {
                return; // Ігноруємо натискання кнопки, якщо поле містить підказку або пусте
            }

            ProcessUserMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (UserInput.Text == "Запитайте будь-що" || string.IsNullOrWhiteSpace(UserInput.Text))
                {
                    return; // Ігноруємо Enter, якщо користувач нічого не ввів або залишив стандартний текст
                }

                ProcessUserMessage();
            }
        }

        private async void ProcessUserMessage()
        {
            string userMessage = UserInput.Text.Trim();
            if (!string.IsNullOrEmpty(userMessage))
            {
                ChatHistory.Items.Add($"Ви: {userMessage}");
                UserInput.Clear();

                string botResponseText = botResponse.GetResponse(userMessage);
                int botMessageIndex = ChatHistory.Items.Add("Бот: ");

                string currentText = "Бот: ";
                foreach (char letter in botResponseText)
                {
                    await Task.Delay(50);
                    currentText += letter;
                    ChatHistory.Items[botMessageIndex] = currentText;
                }
            }
        }
        private void SaveChatHistory_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllLines("chat_history.txt", ChatHistory.Items.Cast<string>());
        }

        private void LoadChatHistory_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("chat_history.txt"))
            {
                ChatHistory.Items.Clear();
                foreach (var line in File.ReadAllLines("chat_history.txt"))
                {
                    ChatHistory.Items.Add(line);
                }
            }
        }

        private void ClearChatHistory_Click(object sender, RoutedEventArgs e)
        {
            ChatHistory.Items.Clear();
            ChatHistory.Items.Add("Бот: Доброго дня, чим я можу вам допомогти?😀");
        }

        // Допоміжний метод для знаходження всіх кнопок у вікні
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T variable)
                    {
                        yield return variable;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    public class BotResponse
    {
        public virtual string GetResponse(string message)
        {
            return "Вибачте, я не розумію ваш запит, спробуйте ще раз. 🤔";
        }
    }
    public class EmojiResponse : BotResponse
    {
        private Dictionary<string, string> responses = new Dictionary<string, string>
        {
            { "привіт", "Привіт! 😊 Як я можу вам допомогти?" },
            { "забронювати", "Ви можете забронювати номер на нашому сайті або зателефонувавши на рецепцію. 📞" },
            { "ціни", "Ціни залежать від категорії номера та сезону. 🏨" },
            { "послуги", "Наш готель пропонує спа, басейн, ресторан та багато іншого! 🍽️🏊" },
            { "дякую", "Будь ласка! Гарного дня! 🌞" },
            { "до побачення", "До зустрічі! 👋" },
            { "паркінг", "Так, у нас є безкоштовний паркінг для гостей. 🚗" },
            { "заїзд", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "виїзд", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "домашні", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "wi-fi", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "кондиціонер", "Так, всі наші номери оснащені кондиціонерами. ❄️" }
        };

        public override string GetResponse(string message)
        {
            string lowerMessage = message.ToLower();
            foreach (var key in responses.Keys)
            {
                if (lowerMessage.Contains(key))
                {
                    return responses[key];
                }
            }
            return base.GetResponse(message);
        }
    }
}
