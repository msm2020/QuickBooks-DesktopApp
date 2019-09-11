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
using MahApps.Metro.Controls.Dialogs;
using QuickBooks.Data.Controller;
using QuickBooks.Data.Entity;
using QuickBooksApp.Forms;

namespace QuickBooksApp
{

    public enum Operation { Create, Edit }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadingList();
        }


        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            CustomerFormPanel.Children.Clear();
            var customer = new CustomerControl();
            customer.Close += async (sender1, e1) =>
            {
                await LoadingList();
            };
            customer.Close += (sender1, e1) =>
            {
                CustomerFormFlyout.IsOpen = false;                
            };
            CustomerFormPanel.Children.Add(customer);
            CustomerFormFlyout.IsOpen = true;
        }





        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CustomerFormPanel.Children.Clear();
            var item = sender as FrameworkElement;
            var entity = item?.DataContext as CustomerEntity;
            var customer = new CustomerControl(Operation.Edit, entity);
            customer.Close += async (sender1, e1) =>
            {
                await LoadingList();
            };
            customer.Close += (sender1, e1) =>
            {
                CustomerFormFlyout.IsOpen = false;
            };
            CustomerFormPanel.Children.Add(customer);
            CustomerFormFlyout.IsOpen = true;
        }

        private async void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" };
            var result = await this.ShowMessageAsync("", "Are you sure to delete Customer ?",
                MessageDialogStyle.AffirmativeAndNegative, settings);


            //var result = MessageBox.Show("Are you sure to delete Customer?", "", MessageBoxButton.YesNo);
            if (result == MessageDialogResult.Affirmative)
            {
                var item = sender as FrameworkElement;
                var customer = item?.DataContext as CustomerEntity;
                CustomerController maneger = new CustomerController();
                if (customer != null)
                {
                    var data = maneger.Delete(customer.ListID);
                    string message = "";
                    if (data.ContainsKey("Message"))
                        message = data["Message"];
                    if (data.ContainsKey("Error"))
                        message = data["Error"];

                    await this.ShowMessageAsync("", message);


                    if (message.Contains("successful"))
                    {
                        await LoadingList();
                    }
                }
            }
        }

        private void ShowLoading()
        {
            Dispatcher.Invoke(() => Loading.Visibility= Visibility.Visible );
        }

        private void HideLoading()
        {
            Dispatcher.Invoke(() => Loading.Visibility = Visibility.Collapsed);
        }

        private async Task LoadingList()
        {
            ShowLoading();
            try
            {
                List<CustomerEntity> customers = new List<CustomerEntity>();
                await Task.Run(() =>
                {
                    CustomerController maneger = new CustomerController();
                    customers = maneger.GetAll();
                });

                Dispatcher.Invoke(() => Customergrid.ItemsSource = customers);
            }
            finally
            {
                HideLoading();
            }
        }

    }
}
