using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using System.Windows.Input;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class UnverifiedVideoItemControl : UserControl
    {
        Video _video;
        int Id;
        string Name;
        string AuthorName;
        DateTime UploadDate;
        string Description;

        public UnverifiedVideoItemControl(Video video)
        {
            InitializeComponent();
            _video = video;
            Id = video.Id;
            Name = video.Name;
            AuthorName = video.Author.Name;
            UploadDate = video.DateUpload;
            Description = string.IsNullOrEmpty(video.Description) ? "Описание отсутствует" : video.Description;

            Draw();
        }

        private void Draw()
        {
            VideoIdText.Text = $"Id: {Id}";
            VideoNameText.Text = $"Название: {Name}";
            AuthorNameText.Text = AuthorName;
            UploadDateText.Text = UploadDate.ToShortDateString();
            DescriptionText.Text = Description;

        }

        private void GoVerify(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPageHelper.OpenPage(Classes.Pages.Verify, _video);
        }
    }
}
