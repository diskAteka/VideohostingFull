using DataBaseConnection.Core.Domain.Models;
using System.Windows;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Pages
{
    public partial class EditComment : Page
    {
        Comment _comment;
        PanelHelper _panelHelper = MainWindow.init._panelHelper;

        int VideoId;
        int UserId;
        string Text;

        public EditComment(Comment comment)
        {
            InitializeComponent();
            _comment = comment;
            VideoId = _comment.VideoId;
            UserId = _comment.UserId;
            Text = _comment.Text;
            LoadData();
        }

        private void LoadData()
        {
            P_Id.Text = _comment.Id.ToString();
            P_VideoComboBox = _panelHelper.DrawComboBox(ForeignKeys.Video, P_VideoComboBox, _comment.VideoId);
            P_UserComboBox = _panelHelper.DrawComboBox(ForeignKeys.User, P_UserComboBox, _comment.UserId);
            P_CommentTextTextBox.Text = _comment.Text;
            P_DateTextBox.Text = _comment.Date.ToString();
        }

        private void SearchUserById(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!int.TryParse(P_UserSearchIdTextBox.Text, out int id))
                MessageBox.Show("Неверный ввод!");
            P_UserComboBox = _panelHelper.DrawComboBox(ForeignKeys.User, P_UserComboBox, id);
        }

        private void SearchVideoById(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!int.TryParse(P_VideoSearchIdTextBox.Text, out int id))
                MessageBox.Show("Неверный ввод!");
            P_VideoComboBox = _panelHelper.DrawComboBox(ForeignKeys.User, P_VideoComboBox, id);
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            string message = "";
            int newVideoId = _panelHelper.GetIdFromComboBox((ComboBoxItem)P_VideoComboBox.SelectedItem);
            int newUserId = _panelHelper.GetIdFromComboBox((ComboBoxItem)P_UserComboBox.SelectedItem);
            if (VideoId != newVideoId)
                message += $"Видео: {VideoId} -> {newVideoId}\n";
            if (UserId != newUserId)
                message += $"Автор комментария: {UserId} -> {newVideoId}\n";
            if (Text != P_CommentTextTextBox.Text)
                message += $"Техт комментария: {new string(Text.Take(20).ToArray())}\n" +
                    $"-> {new string(P_CommentTextTextBox.Text.Take(20).ToArray())}";
            if (MessageBox.Show("Подтвердите изменения:\n" + message, "Подтверждение изменений", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            _comment.UserId = newUserId;
            _comment.VideoId = newVideoId;
            _comment.Text = Text;
            _panelHelper.UpdateObject(_comment);

            MainWindow.init.LoadData(Tables.Comment, MainWindow.init.sortDesc);
            MainWindow.init.MainFrame.GoBack();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            MainWindow.init.MainFrame.GoBack();
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены? Это действие необратимо", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            _panelHelper.DeleteObject(_comment);
        }
    }
}
