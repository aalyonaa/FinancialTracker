using FinTracker.Assets;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinTracker.Loans
{
    /// <summary>
    /// Interaction logic for EditLoanWindow.xaml
    /// </summary>
    public partial class EditLoanWindow : Window
    {
        readonly MainWindow _mainWindow;
        private readonly Storage _storage = Storage.GetStorage();
        public EditLoanWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            DatePickerLoanStartEdit.SelectedDate = DateTime.Now;
            FillingComboBoxLoanAssetEdit();
            FillingComboboxLoanStatusEdit();
            FillingEditLoanWindow();
            ButtonEditLoan_IsEnabled();
        }

        public void FillingComboBoxLoanAssetEdit()
        {
            foreach (AbstractAsset asset in _storage.actualUser.Assets)
            {
                ComboBoxLoanAssetEdit.Items.Add(asset.Name);
            }
        }

        public void FillingComboboxLoanStatusEdit()
        {
            ComboBoxLoanStatusEdit.Items.Add("Не выплачен");
            ComboBoxLoanStatusEdit.Items.Add("Выплачен");

        }

        public void FillingEditLoanWindow()
        {

            Loan loan = ((Loan)_mainWindow.ListViewLoans.SelectedItem);
            DatePickerLoanStartEdit.SelectedDate = Convert.ToDateTime(loan.ActualPaymentDateTime);
            ComboBoxLoanAssetEdit.SelectedItem = Convert.ToString(loan.Asset.Name);
            TextBoxLoanCreditorNameEdit.Text = loan.CreditorsName;
            TextBoxLoanPercentEdit.Text = Convert.ToString(loan.Percent);
            TextBoxLoanPeriodEdit.Text = Convert.ToString(loan.Period);
            TextBoxLoanAmountEdit.Text = Convert.ToString(loan.Amount);
            ComboBoxLoanStatusEdit.SelectedItem = loan.Status;
            TextBoxRemainingTermEdit.Text = Convert.ToString(loan.RemainingTerm);
        }

        private void ButtonEditLoan_Click(object sender, RoutedEventArgs e)
        {
            User user = _storage.actualUser;
            AbstractAsset asset = user.GetAssetByName(ComboBoxLoanAssetEdit.SelectedItem.ToString());
            Loan loan = ((Loan)_mainWindow.ListViewLoans.SelectedItem);
            loan.ActualPaymentDateTime = Convert.ToDateTime(DatePickerLoanStartEdit.SelectedDate);
            loan.Asset = asset;
            loan.CreditorsName = TextBoxLoanCreditorNameEdit.Text;
            loan.Percent = Convert.ToDouble(TextBoxLoanPercentEdit.Text);
            loan.Period = Convert.ToDouble(TextBoxLoanPeriodEdit.Text);
            loan.Amount = Convert.ToDouble(TextBoxLoanAmountEdit.Text);
            loan.Status = Convert.ToString(ComboBoxLoanStatusEdit.SelectedItem);
            loan.RemainingTerm = Convert.ToDouble(TextBoxRemainingTermEdit.Text);
            _mainWindow.ListViewLoans.Items.Refresh();
            _mainWindow.LabelRemainingDays.Content = Convert.ToString((loan.ActualPaymentDateTime - DateTime.Today).TotalDays);
            loan.TotalAmountOfPercents = loan.Amount * ((loan.Percent / 1200) * loan.Period);
            _mainWindow.LabelTotalAmountOfPercents.Content = Math.Round(loan.TotalAmountOfPercents, 2);
            this.Close();
        }

        private void ButtonEditLoan_IsEnabled()
        {
            if (TextBoxLoanCreditorNameEdit.Text == "" ||
                TextBoxLoanPercentEdit.Text == "" ||
                TextBoxLoanPeriodEdit.Text == "" ||
                TextBoxRemainingTermEdit.Text == "" ||
                TextBoxLoanAmountEdit.Text == "")
            {
                ButtonEditLoan.IsEnabled = false;
            }

            else if (TextBoxLoanCreditorNameEdit.Text != "" &&
                TextBoxLoanPercentEdit.Text != "" &&
                TextBoxLoanPeriodEdit.Text != "" &&
                TextBoxRemainingTermEdit.Text != "" &&
                TextBoxLoanAmountEdit.Text != "")
            {
                ButtonEditLoan.IsEnabled = true;
            }
        }

        private void TextBoxLoanCreditorNameEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEditLoan_IsEnabled();
        }

        private void TextBoxLoanPercentEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEditLoan_IsEnabled();
        }

        private void TextBoxLoanPeriodEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEditLoan_IsEnabled();
        }

        private void TextBoxLoanAmountEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEditLoan_IsEnabled();
        }

        private void TextBoxRemainingTermEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonEditLoan_IsEnabled();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxLoanPercent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            IsTextAllowed(TextBoxLoanPercentEdit.Text);
        }

        private void TextBoxLoanPeriod_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            IsTextAllowed(TextBoxLoanPeriodEdit.Text);
        }

        private void TextBoxLoanAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            IsTextAllowed(TextBoxLoanAmountEdit.Text);
        }

        private void TextBoxRemainingTerm_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            IsTextAllowed(TextBoxRemainingTermEdit.Text);
        }


    }
}
