using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class VideoItemControl : UserControl
    {
        Video _video;
        PanelHelper _panelHelper = MainWindow.init._panelHelper;


        int Id;
        string Name;
        string AuthorName;
        DateTime UploadDate;
        bool IsVerified;
        int ViewCount;
        int LikeCount;
        int DislikeCount;


        public VideoItemControl(Video video)
        {
            InitializeComponent();
            _video = video;

            Id = _video.Id;
            Name = _video.Name;
            AuthorName = _panelHelper.GetUsernameById(_video.AuthorId);
            UploadDate = _video.DateUpload;
            IsVerified = _video.IsVerified;
            ViewCount = _video.Views;
            LikeCount = _video.Likes;
            DislikeCount = _video.Dislikes;
            Draw();
        }

        private void Draw()
        {
            VideoIdText.Text = $"Id: {Id}";
            VideoNameText.Text = Name;
            AuthorText.Text = $"Автор: {AuthorName}";
            DateText.Text = UploadDate.ToShortDateString();
            VerificationText.Text = IsVerified ? "Проверено" : "Не проверено";
            ViewsCountText.Text = $"Просмотры: {ViewCount}";
            LikesCountText.Text = $"Лайки: {LikeCount}";
            DislikesCountText.Text = $"Дизлайки: {DislikeCount}";
        }

        private void GoChangeVideo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPageHelper.OpenPage(Classes.Pages.EditVideo, _video);
        }
    }
}
