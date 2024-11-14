
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

public class DataVisualizationApp : Form
{
    private Chart chart;
    private ComboBox filterComboBox;
    private Button loadButton;
    private DataTable dataTable;

    public DataVisualizationApp()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Data Visualization App";
        this.Size = new System.Drawing.Size(800, 600);

        chart = new Chart { Dock = DockStyle.Fill };
        chart.ChartAreas.Add(new ChartArea("MainArea"));
        chart.Series.Add(new Series("Data") { ChartType = SeriesChartType.Line });
        this.Controls.Add(chart);

        filterComboBox = new ComboBox { Dock = DockStyle.Top };
        this.Controls.Add(filterComboBox);

        loadButton = new Button { Text = "Load Data", Dock = DockStyle.Top };
        loadButton.Click += LoadButton_Click;
        this.Controls.Add(loadButton);
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            Title = "Select a CSV file"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            LoadData(openFileDialog.FileName);
            UpdateChart();
        }
    }

    private void LoadData(string filePath)
    {
        dataTable = new DataTable();
        var lines = File.ReadAllLines(filePath);

        if (lines.Length > 0)
        {
            var headers = lines[0].Split(',');
            foreach (var header in headers)
                dataTable.Columns.Add(header);

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');
                dataTable.Rows.Add(data);
            }
        }

        filterComboBox.Items.Clear();
        filterComboBox.Items.AddRange(dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray());
        filterComboBox.SelectedIndexChanged += (s, args) => UpdateChart();
    }

    private void UpdateChart()
    {
        if (filterComboBox.SelectedItem == null || dataTable == null) return;

        string columnName = filterComboBox.SelectedItem.ToString();
        chart.Series["Data"].Points.Clear();

        foreach (DataRow row in dataTable.Rows)
        {
            if (decimal.TryParse(row[columnName].ToString(), out decimal value))
            {
                chart.Series["Data"].Points.AddY(value);
            }
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new DataVisualizationApp());
    }
}
