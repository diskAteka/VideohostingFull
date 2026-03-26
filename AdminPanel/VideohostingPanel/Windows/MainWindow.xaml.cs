using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.Models;
using System.Windows;
using System.Windows.Input;
using VideohostingPanel.Classes;

namespace VideohostingPanel.Windows
{
    public partial class MainWindow : Window
    {
        public Tables SelectedTable;
        public static MainWindow init;
        public PanelHelper _panelHelper;
        public OpenPageHelper OpenPageHelper;


        private int from = 0;
        private int to = 10;
        private int page = 1;
        private int recordCount = 0;

        public bool sortDesc = true;
        public MainWindow()
        {
            InitializeComponent();
            init = this;
            if (TablesComboBox.SelectedIndex != -1)
                SelectedTable = (Tables)TablesComboBox.SelectedIndex;
            else
                SelectedTable = Tables.Video; // Установка значения по умолчанию
            LoadData(SelectedTable, sortDesc);
            _panelHelper = new PanelHelper(); // Инициализация PanelHelper
            SortAscButton.IsEnabled = false;
            OpenPageHelper = new OpenPageHelper(MainFrame); // Инициализация OpenPageHelper
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
                // Игнорируем, если перетаскивание не удалось
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            bool isYes = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            if (isYes)
                Application.Current.Shutdown();
        }// Логика закрытия приложения

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            bool isYes = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            if (isYes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }// Логика выхода из аккаунта

        private void SortAscButton_Click(object sender, RoutedEventArgs e)
        {
            if (!sortDesc) // Если уже отсортировано по возрастанию, то не сортируем снова
                return;
            else
            {
                SortDescButton.IsEnabled = true; // Разблокируем кнопку сортировки по убыванию
                SortAscButton.IsEnabled = false;// Блокируем кнопку сортировки по возрастанию, так как уже отсортировано по возрастанию
                LoadData(SelectedTable, false); // Сортируем по возрастанию
                sortDesc = true;
            }
        } // Сортирует по убыванию

        private void SortDescButton_Click(object sender, RoutedEventArgs e)
        {
            if(sortDesc) // Если уже отсортировано по убыванию, то не сортируем снова
                return;
            else
            {
                SortDescButton.IsEnabled = false; // Блокируем кнопку сортировки по убыванию, так как уже отсортировано по убыванию
                SortAscButton.IsEnabled = true; // Разблокируем кнопку сортировки по возрастанию
                LoadData(SelectedTable, true); // Сортируем по убыванию
                sortDesc = false;
            }
        } // Сортирует по возрастанию

        public void LoadData(Tables table, bool ascending)
        {
            ItemsContainer = _panelHelper.DrawElements(table, ItemsContainer, from, to, ascending);
            recordCount = _panelHelper.GetCountOfRecords(table);
            if (recordCount <= to)
                LoadMoreButton.IsEnabled = false;
            else
                LoadMoreButton.IsEnabled = true;
            if(from == 0)
                LoadPreviousButton.IsEnabled = false;
            else
                LoadPreviousButton.IsEnabled = true;
        }// Заполняет StackPanel записями с указанным типом сортировки. Доступен из других страниц.

        private void SearchById(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(SearchIdTextBox.Text, out int id))
            {
                IModel model = _panelHelper.GetObjectById(SelectedTable, id);
                if (model != null)
                    ItemsContainer.Children.Add(_panelHelper.DrawElement(model, ItemsContainer));// Отобразить только найденный элемент
                else
                    MessageBox.Show("Запись с таким ID не найдена.");
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректный ID.");
            }
        } // Ищет запись с указанным Id

        private void NextRecords(object sender, RoutedEventArgs e)
        {
            from += 10;
            to += 10;
            page++;
            PageCount.Text = $"Страница: {page}";
            LoadData(SelectedTable, sortDesc);
        } // Метод для загрузки следующей страницы записей

        private void PreviousRecords(object sender, RoutedEventArgs e)
        {
            from -= 10;
            to -= 10;
            LoadData(SelectedTable, sortDesc);
            page--;
            PageCount.Text = $"Страница: {page}";
        } // Метод для загрузки предыдущей страницы записей

        private void LoadElements(object sender, RoutedEventArgs e)
        {
            SelectedTable = (Tables)TablesComboBox.SelectedIndex;
            LoadData(SelectedTable, sortDesc);
        } // Метод отвечает за выбор таблицы для редактирования или создания новых записей.
    }
}