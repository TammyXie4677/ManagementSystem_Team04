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

namespace CustomerLoyaltyManagementSystem.Control
{
    /// <summary>
    /// Interaction logic for UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        public UsersControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var context = new managementsystem_dbEntities())
            {
                var customers = context.Customers.Select(c => new
                {
                    CustomerID = c.CustomerID,
                    Email = c.User.Email,
                    LoyaltyPoints = c.LoyaltyPoints,
                    Tier = c.Tier
                }).ToList();

                UsersListView.ItemsSource = customers;
            }
        }
    }
}
