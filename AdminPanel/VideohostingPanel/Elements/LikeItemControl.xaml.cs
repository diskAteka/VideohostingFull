using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class LikeItemControl : UserControl
    {
        Like _like;
        PanelHelper PanelHelper = MainWindow.init._panelHelper;

        int Id;
        int UserId;
        string Username;
        int VideoId;
        string VideoTitle;

        public LikeItemControl(Like like)
        {
            InitializeComponent();
            _like = like;
            Id = _like.Id;
            UserId = _like.UserId;
            Username = PanelHelper.GetUsernameById(UserId);
            VideoId = _like.VideoId;
            VideoTitle = PanelHelper.GetVideoTitleById(VideoId);
            DrawData();
        }

        private void DrawData()
        {
            UserNameText.Text = $"{Username}(Id: {UserId})";
            VideoNameText.Text = $"{VideoTitle}(Id: {VideoId})";
            ReactionIdText.Text = $"Id:{Id}";
        }

        private void DeleteLike(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PanelHelper.DeleteLike(_like);
        }
    }
}