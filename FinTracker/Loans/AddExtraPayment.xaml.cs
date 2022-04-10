using FinTracker.Loans;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinTracker
{
    /// <summary>
    /// Interaction logic for AddExtraPayment.xaml
    /// </summary>
    public partial class AddExtraPayment : Window
    {
        readonly MainWindow _mainWindow;


        private readonly Storage _storage = Storage.GetStorage();
        public AddExtraPayment(MainWindow mainWindow)
        {
            InitializeComponent();
            FillingComboBoxExtraPaymentPurpose();
            DatePickerOfExtraPayment.SelectedDate = DateTime.Now;
            ButtonCreateExtraPayment_IsEnabled();
            _mainWindow = mainWindow;

        }

        public void FillingComboBoxExtraPaymentPurpose()
        {
            ComboBoxExtraPaymentPurpose.Items.Add("Уменьшение срока");
            ComboBoxExtraPaymentPurpose.Items.Add("Уменьшение платежа");
        }

        private void ButtonCreateExtraPayment_Click(object sender, RoutedEventArgs e)
        {

            Loan loan = ((Loan)_mainWindow.ListViewLoans.SelectedItem);
            LoanTransaction nLoanTransaction = new LoanTransaction(Storage.sign.spend, Convert.ToDouble(TextBoxAmountOfExtraPayment.Text), Convert.ToDateTime(DatePickerOfExtraPayment.SelectedDate),
                                                                  "", "Платёж по кредиту", Convert.ToString(ComboBoxExtraPaymentPurpose.SelectedItem));
            Transaction transaction = nLoanTransaction;


            if (ComboBoxExtraPaymentPurpose.SelectedItem == "Уменьшение платежа")
            {
                loan.DoExtraPaymentToDecreasePayment(Convert.ToDateTime(DatePickerOfExtraPayment.SelectedDate.Value), Convert.ToDouble(TextBoxAmountOfExtraPayment.Text));
                _mainWindow.ListViewLoans.Items.Refresh();
                this.Close();

            }
            else if (ComboBoxExtraPaymentPurpose.SelectedItem == "Уменьшение срока")
            {
                loan.DoExtraPaymentToDecreaseLoanTerm(Convert.ToDateTime(DatePickerOfExtraPayment.SelectedDate.Value), Convert.ToDouble(TextBoxAmountOfExtraPayment.Text));
                _mainWindow.ListViewLoans.Items.Refresh();
                this.Close();
            }
            ((Loan)_mainWindow.ListViewLoans.SelectedItem).Asset.Transactions.Add(transaction);
            _mainWindow.ListViewLoanPayments.Items.Add(nLoanTransaction);

        }

        private void ButtonCreateExtraPayment_IsEnabled()
        {
            if (TextBoxAmountOfExtraPayment.Text == "")
            {
                ButtonCreateExtraPayment.IsEnabled = false;
            }

            else if (TextBoxAmountOfExtraPayment.Text != "")
            {
                ButtonCreateExtraPayment.IsEnabled = true;
            }
        }

        private void TextBoxAmountOfExtraPayment_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonCreateExtraPayment_IsEnabled();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextBoxAmountOfExtraPayment_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            IsTextAllowed(TextBoxAmountOfExtraPayment.Text);
        }
    }
}
