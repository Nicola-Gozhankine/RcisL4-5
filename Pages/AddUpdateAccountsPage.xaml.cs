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
using WpfApp1.HelperClasses;
using WpfApp1.ModelClasses;


namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddUpdateAccountsPage.xaml
    /// </summary>
    public partial class AddUpdateAccountsPage : Page
    {
        private ModelEF model { get; set; }
        private TypePage type { get; set; }
        private Accounts account { get; set; }
        public AddUpdateAccountsPage(ModelEF _model)
        {
            InitializeComponent();
            model = _model;
            type = TypePage.Add;
            account = new Accounts(); // Создаем новую учетную запись.
                                      // Настройка заголовка страницы.
            textBlockName.Text = "Добавление нового пользователя";
            // Загружаем данные в источники представления.
            LoadDataViewSourses();
            // Устанавливаем начальные значения для элементов управления.
            LoadAddStartData();
        }
        public AddUpdateAccountsPage(ModelEF _model, Accounts _account)
        {
            // Инициализация компонентов страницы.
            InitializeComponent();
            // Присвоение значений свойствам.
            model = _model;
            type = TypePage.Update;
            account = _account; // Используем переданную учетную запись.
                                // Настройка заголовка страницы.
            textBlockName.Text = $"Изменения пользователя c ID {account.ID}";
            // Загружаем данные в источники представления.
            LoadDataViewSourses();
        }
        // Устанавливает начальные значения для элементов управления при добавлении новой учетной записи.
        private void LoadAddStartData()
        {
            // Устанавливаем количество неудачных попыток входа равным нулю.
            badLoginTrySlider.Value = 0;
            // Ставим галочку, указывающую, что это новый пользователь.
            newUserCheckBox.IsChecked = true;
            // Устанавливаем последнюю дату авторизации на текущее время.
            lastDateAuthorizationDatePicker.SelectedDate = DateTime.Now;
            // Скрываем компонент выбора даты последней авторизации.
            lastDateAuthorizationDatePicker.Visibility = Visibility.Collapsed;
            // Устанавливаем индекс выбранного элемента в выпадающих списках ролей и статусов.
            rolesComboBox.SelectedIndex = 0;
            statusesComboBox.SelectedIndex = 0;
        }
        // Загружает данные в источники представления.
        private void LoadDataViewSourses()
        {
            // Загружаем статусы в источник данных.
            (Resources["statusesViewSource"] as CollectionViewSource).Source = model.Statuses.ToList();
            // Загружаем роли в источник данных.
            (Resources["rolesViewSource"] as CollectionViewSource).Source = model.Roles.ToList();
            // Загружаем учетные записи в источник данных.
            (Resources["accountsViewSource"] as CollectionViewSource).Source = new List<Accounts>() { account };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        // Проверяет наличие недопустимых данных в полях формы.
        private bool InBoxesIsNotValidData()
        {
            // Проверяем, что поля Логин, Пароль и ФИО заполнены.
            if (String.IsNullOrWhiteSpace(account.Login)
            || String.IsNullOrWhiteSpace(account.Password)
            || String.IsNullOrWhiteSpace(account.FullName))
            {
                MessageBox.Show("Поля Логин, Пароль и ФИО должны быть обязательно заполнены");
                return true;
            }
            // Проверяем уникальность логина среди существующих учетных записей.
            if (model.Accounts.Any(x => x.ID != account.ID && x.Login == account.Login))
            {
                MessageBox.Show("Данный Логин уже существует");
                return true;
            }
            return false;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текущий экземпляр учетной записи из источника данных.
            var accountsViewSource = (CollectionViewSource)this.Resources["accountsViewSource"];
            account = (accountsViewSource.View.SourceCollection as List<Accounts>).First();
            // Проверяем допустимость введенных данных.
            if (InBoxesIsNotValidData())
                return;
            // В зависимости от типа страницы добавляем или обновляем учетную запись.
            if (type == TypePage.Add)
                model.Accounts.Add(account);
            try
            {
                // Сохраняем изменения в базе данных.
                model.SaveChanges();
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке, если сохранение прошло неуспешно.
                MessageBox.Show(ex.Message);
            }
            // Уведомляем пользователя о сохранении данных.
            MessageBox.Show("Данные сохранены");
            // Если это режим добавления, возвращаемся на предыдущую страницу.
            if (type == TypePage.Add)
                this.NavigationService.GoBack();
        }
    }
}
