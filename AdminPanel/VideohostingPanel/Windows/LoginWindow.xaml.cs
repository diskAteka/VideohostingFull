using System;
using System.Windows;
using System.Windows.Input;
using VideohostingPanel.Windows;

namespace VideohostingPanel
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Для перетаскивания окна
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (username == "admin" && password == "admin")
            {
                // Успешный вход
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                // Показать сообщение об ошибке
                ErrorMessageBorder.Visibility = Visibility.Visible;

                // Анимация появления ошибки
                var storyboard = (System.Windows.Media.Animation.Storyboard)this.Resources["ShowErrorAnimation"];
                if (storyboard != null)
                {
                    storyboard.Begin(ErrorMessageBorder);
                }
            }
        }
    }
}