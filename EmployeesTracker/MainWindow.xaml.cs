using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace EmployeesTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Employee> employees = new ObservableCollection<Employee>();
        private int employeeCounter = 1;
        private readonly string dataFile = "employees.json";

        public class Employee
        {
            public int EmployeeId { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
            public DateTime Date { get; set; }
            public string Department { get; set; }
            public string ImagePath { get; set; } // opcionális, ha képet is szeretnél tárolni
        }

        public MainWindow()
        {
            InitializeComponent();

            LoadEmployees();
            AttendanceDataGrid.ItemsSource = employees;

            // Az ablak Closing eseményének kezelése
            this.Closing += Window_Closing;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Kérlek, add meg a nevet.");
                return;
            }

            if (DepartmentComboBox.SelectedItem is ComboBoxItem selectedDepartmentItem)
            {
                string department = selectedDepartmentItem.Content.ToString();

                Employee newEmployee = new Employee
                {
                    EmployeeId = employeeCounter++,
                    Name = name,
                    Department = department,
                    Status = "", // alapértelmezett státusz lehet üres
                    Date = default, // vagy DateTime.MinValue, vagy egyszerűen ne állítsd be itt
                };

                employees.Add(newEmployee);

                NameTextBox.Clear();
                DepartmentComboBox.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Kérlek, válassz szervezeti egységet.");
            }
        }

        private void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (AttendanceDataGrid.SelectedItem is Employee selectedEmployee)
            {
                if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatusItem)
                {
                    string newStatus = selectedStatusItem.Content.ToString();
                    selectedEmployee.Status = newStatus;
                    selectedEmployee.Date = DateTime.Now;  // <-- itt frissítjük a dátumot

                    // Frissítjük a DataGrid-et, hogy azonnal látszódjon a változás
                    AttendanceDataGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Kérlek, válassz státuszt.");
                }
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy alkalmazottat a táblázatból.");
            }
        }

        private void LoadEmployees()
        {
            if (File.Exists(dataFile))
            {
                try
                {
                    string json = File.ReadAllText(dataFile);
                    var loadedEmployees = JsonSerializer.Deserialize<ObservableCollection<Employee>>(json);
                    if (loadedEmployees != null)
                    {
                        employees = loadedEmployees;
                        AttendanceDataGrid.ItemsSource = employees;
                        if (employees.Count > 0)
                        {
                            employeeCounter = employees[^1].EmployeeId + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba az adatok betöltésekor: {ex.Message}");
                }
            }
        }

        private void SaveEmployees()
        {
            try
            {
                string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatok mentésekor: {ex.Message}");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveEmployees();
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            if (AttendanceDataGrid.SelectedItem is Employee selectedEmployee)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Képfájlok (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|Minden fájl (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    string imagePath = openFileDialog.FileName;
                    selectedEmployee.ImagePath = imagePath;

                    // Frissítsük a DataGrid-et, hogy megjelenjen az új kép
                    AttendanceDataGrid.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy alkalmazottat a listából!");
            }
        }
    }
}