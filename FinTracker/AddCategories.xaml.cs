using System.Windows;

namespace FinTracker
{
    /// <summary>
    /// Interaction logic for AddCategories.xaml
    /// </summary>
    public partial class AddCategories : Window
    {
        private readonly Storage _storage = Storage.GetStorage();
        readonly MainWindow _mainWindow;
        public AddCategories(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.RadioButtonIncome.IsChecked == true)
            {
                _storage.actualUser.CategoriesIncome.Add(TextBoxNewCategory.Text);
                _mainWindow.FillCategories(_storage.actualUser.CategoriesIncome);
            }
            else if (_mainWindow.RadioButtonСonsumption.IsChecked == true)
            {
                _storage.actualUser.CategoriesSpend.Add(TextBoxNewCategory.Text);
                _mainWindow.FillCategories(_storage.actualUser.CategoriesSpend);
            }
            this.Close();
        }


    }
}
