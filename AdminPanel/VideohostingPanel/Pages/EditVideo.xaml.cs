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

namespace VideohostingPanel.Pages
{
    public partial class EditVideo : Page
    {
        Video _video;
        public EditVideo(Video video)
        {
            InitializeComponent();
            _video = video;
        }
    }
}
