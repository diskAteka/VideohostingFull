using DataBaseConnection.Core.Domain.Interfaces;
using DataBaseConnection.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VideohostingPanel.Pages;

namespace VideohostingPanel.Classes
{
    public class OpenPageHelper
    {
        public Frame StartFrame { get; set; }
        public OpenPageHelper(Frame startFrame)
        {
            StartFrame = startFrame;
        }//Получает Frame при инициализации



        private void OpenPageForNewRecord(Pages page)
        {
            switch (page)
            {
                case Pages.EditVideo:
                    StartFrame.Navigate(new EditVideo(null));
                    break;
                case Pages.EditUser:
                    StartFrame.Navigate(new EditUser(null));
                    break;
                case Pages.EditComment:
                    StartFrame.Navigate(new EditComment(null));
                    break;
                case Pages.EditEmloyee:
                    StartFrame.Navigate(new EditEmployee(null));
                    break;
                default:
                    throw new ArgumentException("Неизвестная таблица");
            }
        } //Метод для открытия страницы редактирования в сценарии создания новой записи

        private void OpenPageForExistingRecord(Pages page, IModel model)
        {
            switch (table)
            {
                case Pages.EditVideo:
                    StartFrame.Navigate(new EditVideo(model as Video));
                    break;
                case Pages.EditUser:
                    StartFrame.Navigate(new EditUser(model as User));
                    break;
                case Pages.EditComment:
                    StartFrame.Navigate(new EditComment(model as Comment));
                    break;
                case Pages.EditEmloyee:
                    StartFrame.Navigate(new EditEmployee(model as Employee));
                    break;
                case Pages.Verify:
                    StartFrame.Navigate(new VerifyVideo(model as Video));
                    break;
                default:
                    throw new ArgumentException("Неизвестная таблица");
            }
        } //Метод для открытия страницы редактирования в сценарии редактирования существующей записи
        public void OpenPage(Pages page, IModel model = null) 
        {
            if (model == null)
                OpenPageForNewRecord(table);
            else
                OpenPageForExistingRecord(table, model);
        } //Публичный метод, реализующий полиморфизм этого класса
    }
}
