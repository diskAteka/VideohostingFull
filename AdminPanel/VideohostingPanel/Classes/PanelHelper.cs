using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.Models;
using DataBaseConnection.Core.Domain.ResultModels;
using DataBaseConnection.Infrastructure;
using DataBaseConnection.Infrastructure.Interfaces;
using DataBaseConnection.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VideohostingPanel.Elements;
using VideohostingPanel.Windows;

namespace VideohostingPanel.Classes
{
    public class PanelHelper //Создается один на все приложение. Каждое окно в принимаемых параметрах получает ссылку на этот класс.
    {
        VideohostingDbContext _db = new VideohostingDbContext();

        private IGetObjectService _getObjectService;
        private IAddObjectService _addObjectService;
        private IDeleteObjectService _deleteObjectService;
        private IUpdateObjectService _updateObjectService;

        public static PanelHelper init;

        public PanelHelper()
        {
            _getObjectService = new GetObjectSevice(_db);
            _addObjectService = new AddObjectService(_db);
            _deleteObjectService = new DeleteObjectService(_db);
            _updateObjectService = new UpdateObjectService(_db);
            init = this;
        }

        public int CountOfRecords { get; set; }

        public StackPanel DrawElements(Tables table, StackPanel itemContainer, int from, int to, bool ascending = true)
        {
            // Логика отрисовки элементов на основе выбранной таблицы
            switch (table)
            {
                case Tables.Video:
                    List<Video> videos = _getObjectService.GetObjects<Video>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (Video v in videos)
                    {
                        itemContainer.Children.Add(new VideoItemControl(v));
                    }
                    return itemContainer;
                case Tables.User:
                    List<User> users = _getObjectService.GetObjects<User>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (User u in users)
                    {
                        itemContainer.Children.Add(new UserItemControl(u));
                    }
                    return itemContainer;
                case Tables.ServerLog:
                    List<ServerLog> logs = _getObjectService.GetObjects<ServerLog>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (ServerLog s in logs)
                    {
                        itemContainer.Children.Add(new LogItemControl(s));
                    }
                    return itemContainer;
                case Tables.Comment:
                    List<Comment> comments = _getObjectService.GetObjects<Comment>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (Comment c in comments)
                    {
                        itemContainer.Children.Add(new CommentItemControl(c));
                    }
                    return itemContainer;
                case Tables.Like:
                    List<Like> likes = _getObjectService.GetObjects<Like>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (Like l in likes)
                    {
                        itemContainer.Children.Add(new LikeItemControl(l));
                    }
                    return itemContainer;
                case Tables.DisLike:
                    List<DisLike> dislikes = _getObjectService.GetObjects<DisLike>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (DisLike d in dislikes)
                    {
                        itemContainer.Children.Add(new DislikeItemControl(d));
                    }
                    return itemContainer;
                case Tables.View:
                    List<View> views = _getObjectService.GetObjects<View>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (View v in views)
                    {
                        itemContainer.Children.Add(new ViewItemControl(v));
                    }
                    return itemContainer;
                case Tables.Emploee:
                    List<Employee> emploees = _getObjectService.GetObjects<Employee>(from, to, ascending);
                    itemContainer.Children.Clear();
                    foreach (Employee e in emploees)
                    {
                        itemContainer.Children.Add(new EmployeeItemControl(e));
                    }
                    return itemContainer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(table), table, null);
            }
        }//Метод отрисовывает данные из таблицы

        public int GetCountOfRecords(Tables table)
        {
            switch (table)
            {
                case Tables.Video:
                    return _getObjectService.GetCountOfRecords<Video>();
                case Tables.User:
                    return _getObjectService.GetCountOfRecords<User>();
                case Tables.ServerLog:
                    return _getObjectService.GetCountOfRecords<ServerLog>();
                case Tables.Comment:
                    return _getObjectService.GetCountOfRecords<Comment>();
                case Tables.Like:
                    return _getObjectService.GetCountOfRecords<Like>();
                case Tables.DisLike:
                    return _getObjectService.GetCountOfRecords<DisLike>();
                case Tables.View:
                    return _getObjectService.GetCountOfRecords<View>();
                case Tables.Emploee:
                    return _getObjectService.GetCountOfRecords<Employee>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(table), table, null);
            }
        }//Метод возвращает количество записей в таблице

        public IModel GetObjectById(Tables table, int id)
        {
            switch (table)
            {
                case Tables.Video:
                    return _getObjectService.GetObject<Video>(id);
                case Tables.User:
                    return _getObjectService.GetObject<User>(id);
                case Tables.ServerLog:
                    return _getObjectService.GetObject<ServerLog>(id);
                case Tables.Comment:
                    return _getObjectService.GetObject<Comment>(id);
                case Tables.Like:
                    return _getObjectService.GetObject<Like>(id);
                case Tables.DisLike:
                    return _getObjectService.GetObject<DisLike>(id);
                case Tables.View:
                    return _getObjectService.GetObject<View>(id);
                case Tables.Emploee:
                    return _getObjectService.GetObject<Employee>(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(table), table, null);
            }
        }//Метод возвращает объект по id

        public StackPanel DrawElement(IModel model, StackPanel itemContainer)
        {
            itemContainer.Children.Clear();
            switch (model)
            {
                case Video v:
                    itemContainer.Children.Add(new VideoItemControl(v));
                    return itemContainer;
                case User u:
                    itemContainer.Children.Add(new UserItemControl(u));
                    return itemContainer;
                case ServerLog s:
                    itemContainer.Children.Add(new LogItemControl(s));
                    return itemContainer;
                case Comment c:
                    itemContainer.Children.Add(new CommentItemControl(c));
                    return itemContainer;
                case Like l:
                    itemContainer.Children.Add(new LikeItemControl(l));
                    return itemContainer;
                case DisLike d:
                    itemContainer.Children.Add(new DislikeItemControl(d));
                    return itemContainer;
                case View v:
                    itemContainer.Children.Add(new ViewItemControl(v));
                    return itemContainer;
                case Employee e:
                    itemContainer.Children.Add(new EmployeeItemControl(e));
                    return itemContainer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }
        }//Метод отрисовывает один элемент в соответсвии с поиском

        public ComboBox DrawComboBox(ForeignKeys model, ComboBox comboBox, int id)
        {
            comboBox.Items.Clear();
            switch (model)
            {
                case ForeignKeys.User:
                    User user = _getObjectService.GetObject<User>(id) as User;
                    comboBox.Items.Add($"{user.Name} (ID: {user.Id})");
                    List<User> users = _getObjectService.GetObjects<User>(0, 10);
                    foreach (User u in users)
                        comboBox.Items.Add($"{u.Name} (ID: {u.Id})");
                    return comboBox;
                case ForeignKeys.Video:
                    Video video = _getObjectService.GetObject<Video>(id) as Video;
                    comboBox.Items.Add($"{video.Name}(ID: {video.Id})");
                    List<Video> videos = _getObjectService.GetObjects<Video>(0, 10);
                    foreach (Video v in videos)
                        comboBox.Items.Add($"{v.Name}(ID: {v.Id})");
                    return comboBox;
                default:
                    return comboBox;
            }

        } // Возвращет заполненный ComboBox

        public int GetIdFromComboBox(ComboBoxItem item)
        {
            Match match = Regex.Match(item.ToString(), @"ID:\s*(\d+)\)"); // Ругулярное выражение гарантирует что будет захвачено именно id
            if (match.Success)
            {
                string idString = match.Groups[1].Value;
                int id = int.Parse(idString);
                return id;
            }
            else return 0;
        }

        public void UpdateObject(IUpdateble obj)
        {
            switch (obj)
            {
                case User:
                    UpdateRezult rezult1 = _updateObjectService.Update<User>(obj as User);
                    if (rezult1.Success)
                        ShowReport(rezult1.AffectedTables);
                    break;
                case Video:
                    UpdateRezult rezult2 = _updateObjectService.Update<User>(obj as User);
                    if (rezult2.Success)
                        ShowReport(rezult2.AffectedTables);
                    break;
                case Comment:
                    UpdateRezult rezult3 = _updateObjectService.Update<Comment>(obj as Comment);
                    if (rezult3.Success)
                        ShowReport(rezult3.AffectedTables);
                    break;
                case Employee:
                    UpdateRezult rezult4 = _updateObjectService.Update<Employee>(obj as Employee);
                    if (rezult4.Success)
                        ShowReport(rezult4.AffectedTables);
                    break;
            }

        }

        private void ShowReport(List<string> rezult)
        {
            string tables = "";
            foreach (var t in rezult)
                tables += t + "\n";
            MessageBox.Show($"Обновление выполнено.\nЗатронутые таблицы:\n{tables}", "Обновление выполнено!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void DeleteObject(IDeleteble model)
        {
            switch (model)
            {
                case Video:
                    DeleteResult result1 = _deleteObjectService.Delete<Video>((model as Video).Id);
                    if (result1.Success)
                        ShowReport(result1.AffectedTables);
                    break;
                case Comment:
                    DeleteResult result2 = _deleteObjectService.Delete<Comment>((model as Comment).Id);
                    if (result2.Success)
                        ShowReport(result2.AffectedTables);
                    break;
                case User:
                    DeleteResult result3 = _deleteObjectService.Delete<User>((model as User).Id);
                    if (result3.Success)
                        ShowReport(result3.AffectedTables);
                    break;
                case Employee:
                    DeleteResult result4 = _deleteObjectService.Delete<Employee>((model as Employee).Id);
                    if (result4.Success)
                        ShowReport(result4.AffectedTables);
                    break;

            }
        }


        public string GetUsernameById(int id)
        {
            User user = _getObjectService.GetObject<User>(id) as User;
            if (user != null)
                return user.Name;
            else
                throw new ArgumentException($"Пользователь с id {id} не найден");
        }

        public string GetVideoTitleById(int id)
        {
            Video video = _getObjectService.GetObject<Video>(id) as Video;
            if (video != null)
                return video.Name;
            else
                throw new ArgumentException($"Видео с id {id} не найдено");
        }


        public void DeleteLike(Like like)
        {
            if(MessageBox.Show("Вы уверены что хотите удалить этот лайк? Это действие нельзя будет отменить.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;
            using (var transaction = _db.Database.BeginTransaction())
            {
                DeleteResult deleteResult = _deleteObjectService.Delete<Like>(like.Id);
                Video video = _getObjectService.GetObject<Video>(like.VideoId) as Video;
                video.Likes--;
                UpdateRezult updateResult = _updateObjectService.Update<Video>(video);

                if (deleteResult.Success && updateResult.Success)
                {
                    transaction.Commit();
                    ShowReport(deleteResult.AffectedTables);
                }
                else
                {
                    transaction.Rollback();
                    MessageBox.Show("Не удалось удалить лайк.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }// Метод удаляет лайк и обновляет количество лайков у видео. Действия выполняются атомарно. Если обе операции успешны, отображается отчет, иначе - сообщение об ошибке.

        public void DeleteDislike(DisLike dislike)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить этот дизлайк? Это действие нельзя будет отменить.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;
            using (var transaction = _db.Database.BeginTransaction())
            {
                DeleteResult deleteResult = _deleteObjectService.Delete<DisLike>(dislike.Id);
                Video video = _getObjectService.GetObject<Video>(dislike.VideoId) as Video;
                video.Dislikes--;
                UpdateRezult updateResult = _updateObjectService.Update<Video>(video);
                if (deleteResult.Success && updateResult.Success)
                {
                    transaction.Commit();
                    ShowReport(deleteResult.AffectedTables);
                }
                else
                {
                    transaction.Rollback();
                    MessageBox.Show("Не удалось удалить дизлайк.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }// Метод удаляет дизлайк и обновляет количество дизлайков у видео. Действия выполняются атомарно. Если обе операции успешны, отображается отчет, иначе - сообщение об ошибке.

        public void DeleteView(View view)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить этот просмотр? Это действие нельзя будет отменить.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;
            using (var transaction = _db.Database.BeginTransaction())
            {
                DeleteResult deleteResult = _deleteObjectService.Delete<View>(view.Id);
                Video video = _getObjectService.GetObject<Video>(view.VideoId) as Video;
                video.Views--;
                UpdateRezult updateResult = _updateObjectService.Update<Video>(video);
                if (deleteResult.Success && updateResult.Success)
                {
                    transaction.Commit();
                    ShowReport(deleteResult.AffectedTables);
                }
                else
                {
                    transaction.Rollback();
                    MessageBox.Show("Не удалось удалить просмотр.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }// Метод удаляет просмотр и обновляет количество просмотров у видео. Действия выполняются атомарно. Если обе операции успешны, отображается отчет, иначе - сообщение об ошибке.
    }
}
