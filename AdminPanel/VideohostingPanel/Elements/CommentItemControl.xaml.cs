using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using System.Windows.Input;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class CommentItemControl : UserControl
    {
        Comment _comment;
        PanelHelper PanelHelper = MainWindow.init._panelHelper;


        int _userId;
        string _username;
        int _videoId;
        string _videoTitle;
        string _text;
        DateTime _date;

        
        public CommentItemControl(Comment comment)
        {
            InitializeComponent();
            _comment = comment;
            _userId = _comment.UserId;
            _username = PanelHelper.GetUsernameById(_userId);
            _videoId = _comment.VideoId;
            _videoTitle = PanelHelper.GetVideoTitleById(_videoId);
            _text = _comment.Text;
            _date = _comment.Date;
            DrawData();
        }

        private void GoChangeRecord(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPageHelper.OpenPage(Classes.Pages.EditComment, _comment);
        }

        private void DrawData()
        {
            AuthorNameText.Text = $"{_username}(Id: {_userId})";
            VideoNameText.Text = $"{_videoTitle}(Id: {_videoId})";
            CommentDateText.Text = _date.ToString();
            CommentText.Text = _text;
        }
    }
}
