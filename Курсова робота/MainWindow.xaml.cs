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
        private int messageCount = 0;
        private JokeResponse jokeBot = new JokeResponse();
        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(34, 34, 34));
                Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); 
                ThemeToggleButton.Content = "☀️";

                Style darkStyle = (Style)FindResource("DarkButtonStyle");

                SaveButton.Style = darkStyle;
                LoadButton.Style = darkStyle;
                ClearButton.Style = darkStyle;
                CheckInOutButton.Style = darkStyle;
                ParkingButton.Style = darkStyle;
                PetsButton.Style = darkStyle;
            }
            else
            {
                Resources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(255, 255, 255)); 
                Resources["ForegroundBrush"] = new SolidColorBrush(Color.FromRgb(0, 0, 0)); 
                ThemeToggleButton.Content = "🌙";

                Style lightStyle = (Style)FindResource("LightButtonStyle");

                SaveButton.Style = lightStyle;
                LoadButton.Style = lightStyle;
                ClearButton.Style = lightStyle;
                CheckInOutButton.Style = lightStyle;
                ParkingButton.Style = lightStyle;
                PetsButton.Style = lightStyle;
            }
        }

        private List<BotResponse> botResponses = new List<BotResponse>
        {
            new EmojiResponse(),
            new JokeResponse()
        };
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

        private class CheckInOutInfo
        {
            public string GetInfo() => "Час заїзду з 14:00, виїзд до 12:00. 🕒";
        }

        private class ParkingInfo
        {
            public string GetInfo() => "У нас є безкоштовний паркінг для гостей. 🚗";
        }

        private class PetsInfo
        {
            public string GetInfo() => "Ми приймаємо гостей із домашніми тваринами! 🐶🐱";
        }

        // Обробники подій кнопок
        private void CheckInOut_Click(object sender, RoutedEventArgs e)
        {
            var info = new CheckInOutInfo().GetInfo();
            ChatHistory.Items.Add("Бот: " + info);
        }

        private void Parking_Click(object sender, RoutedEventArgs e)
        {
            var info = new ParkingInfo().GetInfo();
            ChatHistory.Items.Add("Бот: " + info);
        }

        private void Pets_Click(object sender, RoutedEventArgs e)
        {
            var info = new PetsInfo().GetInfo();
            ChatHistory.Items.Add("Бот: " + info);
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

                string response = null;
                foreach (var bot in botResponses)
                {
                    response = bot.GetResponse(userMessage);
                    if (response != null && !response.StartsWith("Вибачте"))
                        break;
                }

                if (string.IsNullOrEmpty(response))
                    response = "Вибачте, я не розумію ваш запит, спробуйте ще раз.";

                messageCount++;
                if (messageCount % 3 == 0)
                {
                    string joke = jokeBot.GetResponse("жарт");
                    response += "\nДо речі, ось вам жарт: " + joke;
                }

                int botMessageIndex = ChatHistory.Items.Add("Бот: ");

                string currentText = "Бот: ";
                foreach (char letter in response)
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

    public abstract class BotResponse
    {
        public abstract string GetResponse(string message);
    }
    public class EmojiResponse : BotResponse
    {
        private Dictionary<string, string> responses = new Dictionary<string, string>
        {
            { "привіт", "Привіт! 😊 Як я можу вам допомогти?" },
            { "доброго дня", "Доброго дня! 😊 Як я можу вам допомогти?" },
            { "доброго ранку", "Доброго ранку! 😊 Як я можу вам допомогти?" },
            { "доброго вечора", "Доброго вечора! 😊 Як я можу вам допомогти?" },
            { "добрий день", "Доброго дня! 😊 Як я можу вам допомогти?" },
            { "добрий ранок", "Доброго ранку! 😊 Як я можу вам допомогти?" },
            { "добрий вечір", "Доброго вечора! 😊 Як я можу вам допомогти?" },
            { "хай", "Хелоу! 😊 Як я можу вам допомогти?" },
            { "здоров", "Привіт,привіт! 😊 Як я можу вам допомогти?" },
            { "забронювати", "Ви можете забронювати номер на нашому сайті або зателефонувавши на рецепцію. 📞" },
            { "арендувати", "Ви можете забронювати номер на нашому сайті або зателефонувавши на рецепцію. 📞" },
            { "бронювання", "Ви можете забронювати номер на нашому сайті або зателефонувавши на рецепцію. 📞" },
            { "аренда", "Ви можете забронювати номер на нашому сайті або зателефонувавши на рецепцію. 📞" },
            { "ціни", "Ціни залежать від категорії номера та сезону. 🏨" },
            { "люкс", "Номер Люкс коштує 2500 грн за ніч і включає сніданок, джакузі та вид на озеро. 🏨" },
            { "напівлюкс", "Номер Напівлюкс коштує 1800 грн за ніч. Комфорт і зручності гарантовані. 🛏️" },
            { "стандарт", "Номер Стандарт коштує 1200 грн за ніч. Ідеальний вибір для коротких подорожей. 🚿" },
            { "економ", "Номер Економ коштує 800 грн за ніч. Бюджетно та зручно! 💸" },
            { "пентхаус", "Пентхаус коштує 5000 грн за ніч. Ексклюзивний комфорт для VIP-гостей. ✨" },
            { "послуги", "Наш готель пропонує спа, басейн, ресторан та багато іншого! 🍽️🏊" },
            { "дякую", "Будь ласка! Гарного дня! 🌞" },
            { "величезне дякую", "Будьласочка! Гарного дня! 🌞" },
            { "до побачення", "До зустрічі! 👋" },
            { "паркінг", "Так, у нас є безкоштовний паркінг для гостей. 🚗" },
            { "місце для машини", "Так, у нас є безкоштовний паркінг для гостей. 🚗" },
            { "парковка", "Так, у нас є безкоштовний паркінг для гостей. 🚗" },
            { "припаркуватися", "Так, у нас є безкоштовний паркінг для гостей. 🚗" },
            { "заїзд", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "заселення", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "приїзд", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "виїзд", "Час заїзду з 14:00, а виїзду до 12:00. 🕒" },
            { "домашні тварини", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "домашніми тваринами", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "домашніми тваринками", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "собаку", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "кота", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "песика", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "кішку", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "wi-fi", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "вайфай", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "wifi", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "wi fi", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "охолодження", "Так, всі наші номери оснащені кондиціонерами. ❄️" },
            { "отеплення", "Так, всі наші номери оснащені кондиціонерами. ❄️" },
            { "комфортна температура", "Так, всі наші номери оснащені кондиціонерами. ❄️" },
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
            return "Вибачте, я не розумію ваш запит, спробуйте ще раз.";
        }
    }
    public class JokeResponse : BotResponse
    {
        private List<string> jokes = new List<string>
        {
             "Чому комп'ютер не може схуднути? Бо він завжди тримає кеш! 😄" ,
             "ІТ-шник зайшов у бар... і побачив 404 – бар не знайдено. 🍻" ,
             "Я би розповів ще один жарт про баги, але він іноді працює, а іноді – ні. 🐞" ,
             "Чому програмісти ненавидять природу? Бо там забагато багів! 🌳" ,
             "Після 10 годин дебагу ти розумієш, що проблема – це ти. 🤷‍♂️" ,
             "Як називається найвеселіший елемент у програмуванні? LOL-кейшен 😆"
        };

        private Random random = new Random();
        public override string GetResponse(string message)
        {
            if (message.ToLower().Contains("жарт"))
            {
                int index = random.Next(jokes.Count);
                return jokes[index];
            }

            return "Вибачте, я не розумію ваш запит, спробуйте ще раз.";
        }
    }
}
