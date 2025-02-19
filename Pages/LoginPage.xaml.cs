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
using WpfApp1.ModelClasses;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private ModelEF model { get; set; }
        public LoginPage()
        {
            InitializeComponent();
            model = new ModelEF();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение введенного логина и пароля.
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            // Поиск учетной записи по введенным данным.
            Accounts account = IsValidCredentials(login, password);
            // Если учетная запись найдена, то продолжить обработку.

            if (account != null)
            {
                // Обновляем дату последнего входа.
                SetNewEnter(account);
                // Определяем роль пользователя и выполняем соответствующие действия.
                switch (account.RoleID)
                {
                    case 1:
                        // Пользователь является администратором.
                        MessageBox.Show("Вы успешно авторизовались");
                        // Переходим на страницу администратора.
                        this.NavigationService.Navigate(new AdminPage(model));
                        break;
                    case 2:
                        // Сбрасываем количество неудачных попыток входа.
                        ZeroiseBadLoginTry(account);
                        MessageBox.Show("Вы успешно авторизовались");
                        // Если это первый вход пользователя, переходим на страницу изменения пароля.
                        if ((bool)account.NewUser)
                        {
                            this.NavigationService.Navigate(new UpdatePasswordPage(model, account));
                            break;
                        }
                        // Иначе переходим на обычную страницу пользователя.
                        this.NavigationService.Navigate(new UserPage());
                        break;
                    default:
                        // Роль не найдена.
                        MessageBox.Show("Роль не найдена");
                        break;
                }
            }
        }
        // Устанавливает новую дату последнего входа для учетной записи.
        private void SetNewEnter(Accounts account)
        {
            // Обновляем поле LastDateAuthorization текущей датой.
            account.LastDateAuthorization = DateTime.Now.Date;
            try
            {
                // Сохраняем изменения в базе данных.
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке, если сохранение прошло неуспешно.
                MessageBox.Show(ex.Message);
            }
        }
        // Сброс количества неудачных попыток входа до нуля.
        private void ZeroiseBadLoginTry(Accounts account)
        {
            // Обнуляем значение BadLoginTry.
            account.BadLoginTry = 0;

            try
            {
                // Сохраняем изменения в базе данных.
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке, если сохранение прошло неуспешно.
                MessageBox.Show(ex.Message);
            }
        }
        // Вычисляет интервал между двумя датами.
        private int GetDateInterval(DateTime lastautorizaation)
        {
            // Возвращает количество дней между текущим моментом и переданной датой.
            return (DateTime.Now - lastautorizaation).Days;
        }
        // Блокирует учетную запись.
        private void BlockAccount(Accounts account)
        {
            // Устанавливаем статус учетной записи как заблокированную (StatusID = 2).
            account.StatusID = 2;
            try
            {
                // Сохраняем изменения в базе данных.
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке, если сохранение прошло неуспешно.
                MessageBox.Show(ex.Message);
            }
            // Уведомляем пользователя о блокировке его учетной записи.
            MessageBox.Show("Вы заблокированы. Обратитесь к администратору");
        }
        // Добавляет одну попытку неудачного входа и проверяет общее количество таких попыток.
        private void AddTryBadLogin(Accounts account)
        {
            // Увеличиваем счетчик неудачных попыток входа.
            account.BadLoginTry += 1;
            try
            {
                // Сохраняем изменения в базе данных.
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке, если сохранение прошло неуспешно.
                MessageBox.Show(ex.Message);
            }
            // Если количество неудачных попыток достигло трех, блокируем учетную запись.
            if (account.BadLoginTry == 3)
            {

                BlockAccount(account);
                return;
            }
            // Формируем сообщение с оставшимся количеством попыток входа.
            string text = "Вы ввели неверный пароль.\n" +
            "Пожалуйста проверьте ещё раз введенные данные\n";
            // Для обычных пользователей показываем оставшиеся попытки входа.
            text += account.RoleID != 1 ? $"Оставшиеся попытки входа {3 - account.BadLoginTry}" : "";
            // Показываем сообщение пользователю.
            MessageBox.Show(text);
        }
        // Поиск учетной записи по логину и паролю.
        private Accounts SearchAccount(string login, string password)
        {
            // Находим первую учетную запись, соответствующую введенному логину и паролю.
            Accounts account = model.Accounts.FirstOrDefault(x => x.Login == login && x.Password == password);
            // Если учетная запись найдена, проверяем её состояние.
            if (account != null)
            {
                if (IsValidAccount(account))
                    return account;
            }
            else
            {
                // Если учетная запись не найдена, ищем только по логину.
                account = model.Accounts.FirstOrDefault(x => x.Login == login);
                IsValidLogin(account); // Проверяем логин.
            }
            return null;
        }
        // Проверяет состояние учетной записи.
        private bool IsValidAccount(Accounts accounts)
        {
            // Если учетная запись существует...
            if (accounts != null)
            {
                // Администраторы могут входить без ограничений по статусу.
                if (accounts.RoleID == 1 && accounts.StatusID != 2)
                    return true;
                // Если учетная запись заблокирована, сообщаем об этом пользователю.
                if (accounts.StatusID == 2)
                {
                    MessageBox.Show("Вы заблокированы. Обратитесь к администратору");
                    return false;
                }
                // Если последний вход был более 30 дней назад, блокируем учетную запись.
                if (GetDateInterval(accounts.LastDateAuthorization) >= 30)
                {
                    BlockAccount(accounts);
                    return false;
                }
            }

            return true;
        }
        // Проверяет правильность ввода логина.
        private void IsValidLogin(Accounts accounts)
        {
            // Если учетная запись найдена...
            if (accounts != null)
            {
                // Если она заблокирована, уведомляем пользователя.
                if (accounts.StatusID == 2)
                {
                    MessageBox.Show("Вы заблокированы. Обратитесь к администратору");
                    return;
                }
                // Добавляем попытку неудачного входа.
                AddTryBadLogin(accounts);
                return;
            }
            // Если учетная запись не найдена, выводим сообщение об ошибке.
            MessageBox.Show("Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные");
        }
        // Проверяет корректность введенных данных для входа.
        private Accounts IsValidCredentials(string login, string password)
        {
            // Если одно из полей пустое, выводим сообщение об ошибке.
            if (String.IsNullOrWhiteSpace(login) || String.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните поля Логин и Пароль");
                return null;
            }
            // Выполняем поиск учетной записи по введенным данным.
            Accounts account = SearchAccount(login, password);
            return account;
        }
    }
}
