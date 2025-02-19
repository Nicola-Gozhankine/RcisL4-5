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
    /// Логика взаимодействия для UpdatePasswordPage.xaml
    /// </summary>
    public partial class UpdatePasswordPage : Page
    {
        private Accounts account { get; set; }
        private ModelEF model { get; set; }
        public UpdatePasswordPage(ModelEF _model, Accounts _account)
        {
            account = _account;
            model = _model;
        }

        public UpdatePasswordPage()
        {
            InitializeComponent();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string psbox1text = PasswordBox1.Password; 
            string psbox2text = PasswordBox2.Password;
            if (IsValidData(psbox1text, psbox2text))
            {
                // Установка нового пароля для текущего аккаунта.
                account.Password = psbox1text;
                // Отметить, что пользователь больше не новый.
                account.NewUser = false;
                try
                {
                    // Сохранение изменений в базе данных.
                    model.SaveChanges();
                }
                catch (Exception ex)
                {
                    // В случае ошибки при сохранении выводится сообщение об ошибке.
                    MessageBox.Show(ex.Message);
                }

                // Сообщение о том, что пароль был успешно обновлен.
                MessageBox.Show("Пароль успешно изменён");
                // Переход на страницу пользователя.
                this.NavigationService.Navigate(new UserPage());
            }
        }
        private bool IsValidData(string password1, string password2)
        {
            // Если оба поля пустые, вывести предупреждение и вернуть false.
            if (String.IsNullOrWhiteSpace(password1) && String.IsNullOrWhiteSpace(password2))
            {
                MessageBox.Show("Заполните все поля");
                return false;
            }
            // Если оба пароля совпадают и отличаются от предыдущего пароля, вернуть true.
            if (password1 == password2 && account.Password != password1)
                return true;

            // В противном случае вывести сообщение об ошибке и вернуть false.
            MessageBox.Show("Пароли должны быть одинаковыми и не совпадать с предыдущим паролем");
            return false;
        }
    }
}
