using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class UserItemControl : UserControl
    {
        User _user;

        public UserItemControl(User user)
        {
            InitializeComponent();
            _user = user;
            Draw();
        }

        private void Draw()
        {
            if (_user == null) return;

            UserNameText.Text = _user.Name;
            UserEmailText.Text = _user.Email;
            UserIdText.Text = $"Id: {_user.Id}";
            RegisteredDateText.Text = _user.RegisteredAt.ToString("dd.MM.yyyy");
            if (_user.IsActive)
            {
                ActiveText.Text = "Активен";
                ActiveBadge.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xFF, 0x00));
                ActiveText.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0xFF, 0x88));
            }
            else
            {
                ActiveText.Text = "Неактивен";
                ActiveBadge.Background = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0x00, 0x00));
                ActiveText.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x88, 0x88));
            }
            if (_user.CanUpload)
            {
                UploadText.Text = "Может загружать";
                UploadBadge.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x8A, 0x2B, 0xE2));
                UploadText.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xB7, 0x94, 0xFF));
            }
            else
            {
                UploadText.Text = "Не может загружать";
                UploadBadge.Background = new SolidColorBrush(Color.FromArgb(0x33, 0x80, 0x80, 0x80));
                UploadText.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0xCC, 0xCC));
            }
        }

        private void GoChangeRecord(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPageHelper.OpenPage(Classes.Pages.EditUser, _user);
        }
    }
}