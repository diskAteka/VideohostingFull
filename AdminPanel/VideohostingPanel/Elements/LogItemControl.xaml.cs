using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using System.Windows.Media;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class LogItemControl : UserControl
    {
        ServerLog _serverLog;
        PanelHelper _panelHelper = MainWindow.init._panelHelper;
        int Id;
        int UserId;
        string Username;
        string Type;



        public LogItemControl(ServerLog serverLog)
        {
            InitializeComponent();
            _serverLog = serverLog;
            Id = _serverLog.Id;
            UserId = _serverLog.UserId;
            Username = _panelHelper.GetUsernameById(UserId);
            Type = _serverLog.Type;
            Draw();
        }

        private void Draw()
        {
            LogIdText.Text = $"Id: {Id}";
            UserInfoText.Text = $"Пользователь: {Username} (Id: {UserId})";
            EventTypeText.Text = Type;
            EventTypeCodeText.Text = Type;
        }
    }
}
