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
    /// Interaction logic for RecordPage.xaml
    /// </summary>
    public partial class RecordPage : Page
    {
        private readonly string employeesFile = "employees.txt";

        public RecordPage()
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

            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Kérlek válassz státuszt!", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. Módosítsuk az employees.txt fájlban
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

            // 2. Naplózás napi fájlba
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string logFile = $"attendance_{dateStr}.txt";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"{name} | {department} | {newStatus} | {timestamp}";

            File.AppendAllText(logFile, logEntry + Environment.NewLine);

            MessageBox.Show("Státusz frissítve és naplózva.", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "Összes")
            {
                LoadEmployees();
                return;
            }

            EmployeesListBox.Items.Clear();
            if (File.Exists(employeesFile))
            {
                var lines = File.ReadAllLines(employeesFile);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3 && parts[2].Trim() == selectedStatus)
                    {
                        EmployeesListBox.Items.Add(line);
                    }
                }
            }
        }

        public void ExportAttendanceToCSV(string date)
        {
            string logFile = $"attendance_{date}.txt";
            string csvFile = $"attendance_{date}.csv";

            if (!File.Exists(logFile))
            {
                MessageBox.Show("A megadott naplófájl nem létezik.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var lines = File.ReadAllLines(logFile);
            var csvLines = new List<string> { "Név,Szervezeti egység,Státusz,Időbélyeg" };

            foreach (var line in lines)
            {
                var parts = line.Split('|').Select(p => p.Trim()).ToArray();
                if (parts.Length == 4)
                {
                    csvLines.Add(string.Join(",", parts));
                }
            }

            File.WriteAllLines(csvFile, csvLines);
            MessageBox.Show($"Exportálva: {csvFile}", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DashboardPage());
        }
    }
}

