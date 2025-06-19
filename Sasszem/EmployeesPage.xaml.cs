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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;


namespace Sasszem
{
    /// <summary>
    /// Interaction logic for EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private readonly string employeesFile = "employees.txt";

        public EmployeesPage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeesListBox.Items.Clear();

            if (File.Exists(employeesFile))
            {
                var lines = File.ReadAllLines(employeesFile);
                foreach (var line in lines)
                {
                    EmployeesListBox.Items.Add(line);
                }
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string name = EmployeeNameTextBox.Text.Trim();
            string department = (DepartmentComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(department) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Kérlek, töltsd ki az összes mezőt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string entry = $"{name} | {department} | {status}";

            File.AppendAllText(employeesFile, entry + Environment.NewLine);
            EmployeesListBox.Items.Add(entry);

            EmployeeNameTextBox.Clear();
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesListBox.SelectedItem == null)
            {
                MessageBox.Show("Nincs kijelölt alkalmazott.", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string selected = EmployeesListBox.SelectedItem.ToString();
            EmployeesListBox.Items.Remove(selected);

            var allLines = File.ReadAllLines(employeesFile).ToList();
            allLines.RemoveAll(line => line == selected);
            File.WriteAllLines(employeesFile, allLines);
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesListBox.SelectedItem == null)
            {
                MessageBox.Show("Kérlek válassz ki egy alkalmazottat!", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string selected = EmployeesListBox.SelectedItem.ToString();
            string[] parts = selected.Split('|');

            if (parts.Length < 3)
            {
                MessageBox.Show("Hibás formátum az alkalmazotti sorban.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = parts[0].Trim();
            string department = parts[1].Trim();
            string newStatus = (ModifyStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string updatedEntry = $"{name} | {department} | {newStatus}";

            var allLines = File.ReadAllLines(employeesFile).ToList();
            for (int i = 0; i < allLines.Count; i++)
            {
                if (allLines[i].StartsWith($"{name} | {department}"))
                {
                    allLines[i] = updatedEntry;
                    break;
                }
            }

            File.WriteAllLines(employeesFile, allLines);

            int selectedIndex = EmployeesListBox.SelectedIndex;
            EmployeesListBox.Items[selectedIndex] = updatedEntry;
        }

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            // Itt implementáld a navigációt a főmenübe
            NavigationService?.Navigate(new DashboardPage());
        }
    }
}
