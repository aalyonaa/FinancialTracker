using FinTracker.Assets;
using System;
using System.Windows;

namespace FinTracker
{
    /// <summary>
    /// Interaction logic for AddDeposit.xaml
    /// </summary>
    public partial class AddDeposit : Window
    {
        private readonly Storage _storage = Storage.GetStorage();
        readonly MainWindow _mainWindow;

        public AddDeposit(MainWindow mainWindow)
        {
            InitializeComponent();
            FillingComboBoxDepositSpendAsset();

            ComboBoxPeriod.Items.Add(Storage.period.Год);
            ComboBoxPeriod.Items.Add(Storage.period.Месяц);
            ComboBoxPeriod.Items.Add(Storage.period.Неделя);
            ComboBoxPeriod.Items.Add(Storage.period.День);

            ComboBoxPeriod.SelectedIndex = 0;

            DatePickerDepositStart.SelectedDate = DateTime.Today;
            DatePickerDepositStart.SelectedDate.Value.Date.ToShortDateString();
            _mainWindow = mainWindow;
        }

        public void FillingComboBoxDepositSpendAsset()
        {
            ComboBoxDepositSpendAsset.Items.Clear();
            foreach (AbstractAsset asset in _storage.actualUser.Assets)
            {
                ComboBoxDepositSpendAsset.Items.Add(asset.Name);
            }
        }

        private void ButtonSaveNewDeposit_Click(object sender, RoutedEventArgs e)
        {
            //User user = _storage.actualUser;
            if (CheckBoxСapitalization.IsChecked == false)
            {
                if (ComboBoxDepositSpendAsset.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите счет для начисления % по вкладу");
                    return;
                }
                else
                {
                    AbstractAsset asset = _storage.actualUser.GetAssetByName(ComboBoxDepositSpendAsset.SelectedItem.ToString());
                }
            }
            if (TextBoxNameAsset.Text != string.Empty && TextBoxBankName.Text != string.Empty && TextBoxDepositAmount.Text != string.Empty
                && TextBoxTermDeposit.Text != string.Empty && DatePickerDepositStart.Text != string.Empty && TextBoxPercent.Text != string.Empty)
            {

                Deposit deposit = new Deposit(TextBoxNameAsset.Text, TextBoxBankName.Text, Convert.ToDouble(TextBoxDepositAmount.Text), (bool)CheckBoxWithdrawable.IsChecked,
                    (bool)CheckBoxPutable.IsChecked, (bool)CheckBoxСapitalization.IsChecked, Convert.ToInt32(TextBoxTermDeposit.Text),
                    Convert.ToDateTime(DatePickerDepositStart.Text), Convert.ToDouble(TextBoxPercent.Text), (Storage.period)(ComboBoxPeriod.SelectedItem),
                    _storage.actualUser.GetAssetByName(ComboBoxDepositSpendAsset.SelectedItem.ToString()));

                _storage.actualUser.AddDeposit(deposit);

                _mainWindow.ListViewDeposit.Items.Add(deposit);
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы заполнили не все поля");
            }
        }

        public void CheckBoxСapitalizationIsChecked()
        {


            if (CheckBoxСapitalization.IsChecked == true)
            {
                ComboBoxDepositSpendAsset.IsEnabled = false;
            }
            else
            {
                ComboBoxDepositSpendAsset.IsEnabled = true;
            }
        }

        private void CheckBoxСapitalization_Checked(object sender, RoutedEventArgs e)
        {
            ComboBoxDepositSpendAsset.SelectedIndex = -1;
            ComboBoxDepositSpendAsset.IsEnabled = false;
        }

        private void CheckBoxСapitalization_Unchecked(object sender, RoutedEventArgs e)
        {
            ComboBoxDepositSpendAsset.IsEnabled = true;
        }
    }
}