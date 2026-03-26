using DataBaseConnection.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using VideohostingPanel.Classes;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Pages
{
    public partial class EditEmployee : Page 
    {
        Employee _employee;
        PanelHelper _panelHelper;

        int Id;
        string UserName;
        string Password;
        Roles Role;

        public EditEmployee(Employee emploee)
        {
            InitializeComponent();
            _panelHelper = MainWindow.init._panelHelper;
            _employee = emploee;
            LoadData();
            Id = emploee.Id;
            UserName = emploee.UserName;
            Password = emploee.Password;
        }


        public void LoadData()
        {
            P_Id.Text = _employee.Id.ToString();
            P_UserName.Text = _employee.UserName.ToString();
            P_Password.Password = _employee.Password.ToString();
            if(_employee.Role == Roles.Admin.ToString())
            {
                Role = Roles.Admin;
                P_RoleComboBox.SelectedIndex = 0;
            }                                                                                
            else
            {
                Role = Roles.Verifier;
                P_RoleComboBox.SelectedIndex = 0;
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены? Это действие необратимо", "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            _panelHelper.DeleteObject(_employee);
        }

    }
}
