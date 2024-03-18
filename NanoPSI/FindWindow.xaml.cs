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
using MySql.Data.MySqlClient;

namespace NanoPSI
{
    /// <summary>
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        // Change this for local server
        private const string connStr = "server=127.0.0.1;port=3306;database=sys;user=root;password=NinerGraduate-2024;";
        public string SelectedDate { get; private set; } // Property to store the selected date

        public FindWindow()
        {
            InitializeComponent();
            FindData();
        }

        private void FindData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); // Open the connection

                    // Define the SQL query to select all values from the "Saved Date/Time" column
                    string sql = $"SELECT `Saved Date/Time` FROM nanopsi;";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Use a HashSet to ensure all date-time values are unique
                            HashSet<string> dateTimeSet = new HashSet<string>();

                            while (reader.Read())
                            {
                                // Assuming the "Saved Date/Time" is stored in a recognizable format,
                                // convert it to string with both date and time
                                string dateTimeString = Convert.ToDateTime(reader["Saved Date/Time"]).ToString("yyyy-MM-dd HH:mm:ss");

                                // Add the dateTimeString to the HashSet to ensure uniqueness
                                dateTimeSet.Add(dateTimeString);
                            }

                            // Clear existing items in the comboBox
                            datasetComboBox.Items.Clear();

                            // Convert the HashSet to a list, sort it, and then add each item to the comboBox
                            var sortedDateTimeList = dateTimeSet.ToList();
                            sortedDateTimeList.Sort(); // This sorts in ascending order by default

                            foreach (var dateTime in sortedDateTimeList)
                            {
                                // Add each unique and sorted date-time string to the comboBox
                                datasetComboBox.Items.Add(dateTime);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while populating the ComboBox: {ex.Message}");
            }
        }

        private void loadDataButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Assuming datasetComboBox holds the date as a string directly
            if (datasetComboBox.SelectedItem != null)
            {
                SelectedDate = datasetComboBox.SelectedItem.ToString();

                // Ask for confirmation and then close this window if confirmed
                MessageBoxResult result = MessageBox.Show($"Load data for {SelectedDate}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    this.DialogResult = true; // Indicates success
                    //this.Close();
                }
            }
        }
    }
}
