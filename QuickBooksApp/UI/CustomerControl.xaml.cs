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
using System.Windows.Shapes;
using QuickBooks.Data.Controller;
using QuickBooks.Data.Entity;

namespace QuickBooksApp.Forms
{
    /// <summary>
    /// Interaction logic for CustomerControl.xaml
    /// </summary>
    public partial class CustomerControl 
    {
        public CustomerControl()
        {
            InitializeComponent();
        }

        public event EventHandler Close;
        public event EventHandler Reload;

        public string CustomerName
        {
            get => TxtCustomerName.Text;
            set => TxtCustomerName.Text = value;
        }

       

        public double? Balance
        {
            get => TxtBalance.Value;
            set => TxtBalance.Value = value;
        }

        public string CompanyName
        {
            get => TxtCompany.Text;
            set => TxtCompany.Text = value;
        }

        public string Email
        {
            get => TxtEmail.Text;
            set => TxtEmail.Text = value;
        }

        public string Phone
        {
            get => TxtPhone.Text;
            set => TxtPhone.Text = value;
        }

        public string Error
        {
            get => TxtError.Text;
            set => TxtError.Text = value;
        }

        public bool? IsActie
        {
            get => CheckBoxIsActive.IsChecked;
            set => CheckBoxIsActive.IsChecked = value;
        }

        private string Id { get; set; }
        private string EditSequence { get; set; }

        public CustomerEntity Customer
        {
            get
            {
                return new CustomerEntity()
                {
                    Email = Email,
                    Phone = Phone,
                    IsActive = IsActie,
                    ListID = Id,
                    Balance = Balance,
                    Name = CustomerName,
                    CompanyName = CompanyName,
                    EditSequence = SelectedCustomer?.EditSequence                    
                };
            }
        }

        public Operation FormOperation { get; set; }
        private CustomerEntity SelectedCustomer { get; set; }




        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(sender, e);
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CustomerName))
            {
                TxtCustomerName.Focus();
                Error = "Customer Name Field Required";
                return;
            }

            Dictionary<string, string> data;
            CustomerController maneger = new CustomerController();
            if (FormOperation == Operation.Edit)
            {
                data = maneger.Modify(Id, Customer);
            }
            else
            {
                data = maneger.Create(Customer);
            }

            string message = "";
            if (data.ContainsKey("Message"))
            {
                Reload?.Invoke(sender, e);
                Close?.Invoke(sender, e);
            }
            else
            {
                Error = data["Error"];

            }
        }

        private bool load = false;
        private void CmboCustomer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (load) return;
            CustomerEntity customer = (CustomerEntity)CmboCustomer.SelectedItem;
            LoadCustomer(customer);
        }


        private void CustomerForm_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (FormOperation == Operation.Edit)
            {
                CustomerController maneger = new CustomerController();
                var customers = maneger.GetAll();
                CmboCustomer.ItemsSource = customers;
                CmboCustomer.SelectedValuePath = "ListID";
                CmboCustomer.DisplayMemberPath = "FullName";

                CmboCustomer.SelectedValue = Id;
            }
            else
            {
                CmboCustomer.Visibility = Visibility.Collapsed;
                LabelCustomer.Visibility = Visibility.Collapsed;
            }

            load = false;
        }


        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void LoadCustomer(CustomerEntity customer = null)
        {
            if (customer != null)
            {
                Phone = customer.Phone;
                Email = customer.Email;
                IsActie = customer.IsActive;
                Id = customer.ListID;
                CompanyName = customer.CompanyName;
                CustomerName = customer.FullName;
                Balance = customer.Balance;
                EditSequence = customer.EditSequence;

                CmboCustomer.Visibility = Visibility.Visible;
            }
            else
            {
                CmboCustomer.Visibility = Visibility.Collapsed;
                LabelCustomer.Visibility = Visibility.Collapsed;
            }
        }



        public CustomerControl(Operation operation = Operation.Create, CustomerEntity customer = null)
        {
            load = true;
            InitializeComponent();

            FormOperation = operation;
            Id = customer?.ListID;
            EditSequence = customer?.EditSequence;
            SelectedCustomer = customer;
            LoadCustomer(customer);
        }

        private void TxtEmail_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsValidEmail(Email))
                Email = "";
        }
    }
}
