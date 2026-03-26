using DataBaseConnection.Core.Domain.Models;
using System.Windows.Controls;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Elements
{
    public partial class EmployeeItemControl : UserControl
    {
        Employee _employee;
        PanelHelper PanelHelper = MainWindow.init._panelHelper;
        int Id;
        string Login;
        string Password;
        Roles Role;

        public EmployeeItemControl(Employee emploee)
        {
            InitializeComponent();
            _employee = emploee;

            Id = emploee.Id;
            Login = emploee.UserName;
            Password = emploee.Password;
            Role = (Roles)Enum.Parse(typeof(Roles),emploee.Role);
            DrawData();
        }


        private void DrawData()
        {
            EmployeeIdText.Text = $"Id: {Id}";
            EmployeeLoginText.Text = $"Логин: {Login}";
            EmployeePasswordText.Text = $"Пароль: {Password}";
            RoleText.Text = $"Роль: {Role}";

            switch(Role)
            {
                case Roles.Admin:
                    RoleIconABorder.Visibility = System.Windows.Visibility.Visible;
                    RoleIconVBorder.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Roles.Verifier:
                    RoleIconABorder.Visibility = System.Windows.Visibility.Collapsed;
                    RoleIconVBorder.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void GoChangeRecord(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.init.OpenPageHelper.OpenPage(Classes.Pages.EditEmloyee, _employee);
        }
    }
}
