using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;


namespace VideohostingPanel.Elements
{
    public partial class DislikeItemControl : UserControl
    {
        DisLike _disLike;
        PanelHelper PanelHelper = MainWindow.init._panelHelper;

        int Id;
        int UserId;
        string Username;
        int VideoId;
        string VideoTitle;


        public DislikeItemControl(DisLike disLike)
        {
            InitializeComponent();
            _disLike = disLike;
            Id = _disLike.Id;
            UserId = _disLike.UserId;
            Username = PanelHelper.GetUsernameById(UserId);
            VideoId = _disLike.VideoId;
            VideoTitle = PanelHelper.GetVideoTitleById(VideoId);
            DrawData();
        }

        private void DrawData()
        {
            UserNameText.Text = $"{Username}(Id: {UserId})";
            VideoNameText.Text = $"{VideoTitle}(Id: {VideoId})";
            ReactionIdText.Text = $"Id:{Id}";
        }

        private void DeleteDislike(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PanelHelper.DeleteDislike(_disLike);
        }
    }
}
