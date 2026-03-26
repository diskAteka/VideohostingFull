using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class ViewItemControl : UserControl
    {
        View _view;
        PanelHelper PanelHelper = MainWindow.init._panelHelper;

        int Id;
        int UserId;
        string Username;
        int VideoId;
        string VideoTitle;

        public ViewItemControl(View view)
        {
            InitializeComponent();
            _view = view;
            Id = _view.Id;
            UserId = _view.UserId;
            Username = PanelHelper.GetUsernameById(UserId);
            VideoId = _view.VideoId;
            VideoTitle = PanelHelper.GetVideoTitleById(VideoId);
            DrawData();
        }

        private void DrawData()
        {
            VideoNameText.Text = $"{VideoTitle}(Id: {VideoId})";
            UserInfoText.Text = $"Пользователь: {Username}(Id: {UserId})";
            ViewIdText.Text = $"Id: {Id}";
        }

        private void DeleteView(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}