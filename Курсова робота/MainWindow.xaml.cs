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

namespace Курсова_робота
{
    public partial class MainWindow : Window
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
            { "домашн", "Так, ми приймаємо гостей із домашніми тваринами! 🐶🐱" },
            { "wi-fi", "Так, у нашому готелі доступний безкоштовний Wi-Fi. 📶" },
            { "кондиціонер", "Так, всі наші номери оснащені кондиціонерами. ❄️" }
        };

        public MainWindow()
        {
            InitializeComponent();
            UserInput.KeyDown += UserInput_KeyDown;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newFontSize = Math.Max(12, this.ActualHeight / 40); 
            ChatHistory.Tag = newFontSize;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserMessage();
            }
        }

        private void ProcessUserMessage()
        {
            string userMessage = UserInput.Text.Trim();
            if (!string.IsNullOrEmpty(userMessage))
            {
                ChatHistory.Items.Add($"Ви: {userMessage}");
                string botResponse = GetResponse(userMessage);
                ChatHistory.Items.Add($"Бот: {botResponse}");
                UserInput.Clear();
            }
        }

        private string GetResponse(string message)
        {
            string lowerMessage = message.ToLower();
            foreach (var key in responses.Keys)
            {
                if (lowerMessage.Contains(key))
                {
                    return responses[key];
                }
            }
            return "Вибачте, я не розумію запитання. 🤔";
        }

    }
}
