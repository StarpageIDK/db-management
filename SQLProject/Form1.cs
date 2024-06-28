using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace SQLProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button9.Enabled = false;
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
        }

        private SQLiteConnection SQLiteConn;
        private DataTable dTable;

        private void MainForm_Load(object sender, EventArgs e)
        {
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
        }

        private bool OpenDBFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Текстовый файл (*.db)|*.db|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SQLiteConn = new SQLiteConnection("Data Source=" + openFileDialog.FileName + ";Version=3;");
                SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = SQLiteConn;
                return true;
            }
            else return false;
        }


        private void GetTableNames()
        {
            string SQLQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader reader = command.ExecuteReader();
            comboBox1.Items.Clear();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader[0].ToString());
            }
        }


        private string SQL_AllTable()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] order by 1";
        }




        private string SQL_FilterByManufacture()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " + "WHERE Производитель = \"" + comboBox3.SelectedItem + "\";";
        }



        private string SQL_FilterByProduct()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " + "WHERE [Кол-во] <=2;";
        }


        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            adapter.Fill(dTable);

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            for (int col = 0; col < dTable.Columns.Count; col++)
            {
                string ColName = dTable.Columns[col].ColumnName;
                dataGridView1.Columns.Add(ColName, ColName);
                dataGridView1.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                dataGridView1.Rows.Add(dTable.Rows[row].ItemArray);
            }
        }


        private void GetTableColumns()
        {
            string SQLQuery = "PRAGMA table_info(\"" + comboBox1.SelectedItem + "\");";
            SQLiteCommand command = new SQLiteCommand(SQLQuery, SQLiteConn);
            SQLiteDataReader read = command.ExecuteReader();
            comboBox2.Items.Clear();
            while (read.Read())
            {
                comboBox2.Items.Add((string)read[1]);
            }
        }

        private void GetManufactures()
        {
            int kol = 0;
            string s1, s2;
            comboBox3.Items.Clear();
            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    s1 = (string)dTable.Rows[row].ItemArray[2];
                    s2 = (string)comboBox3.Items[i];
                    if (String.Compare(s1, s2) == 0) kol++;
                }
                if (kol == 0) comboBox3.Items.Add(dTable.Rows[row].ItemArray[2]); else kol = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OpenDBFile() == true)
            {
                GetTableNames();
                comboBox1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button9.Enabled = true;
            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            radioButton5.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;

            ShowTable(SQL_AllTable());
            GetTableColumns();
            GetManufactures();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите поле для расчета", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double max;
            double min;
            double sum = 0;
            double value;
            try
            {
                max = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
                min = Convert.ToDouble(dTable.Rows[0].ItemArray[comboBox2.SelectedIndex]);
            }
            catch
            {
                MessageBox.Show("Поле не является числовым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int row = 0; row < dTable.Rows.Count; row++)
            {
                value = Convert.ToDouble(dTable.Rows[row].ItemArray[comboBox2.SelectedIndex]);
                if (value > max) max = value;
                if (value < min) min = value;
                sum += value;
            }

            string MyMessage = "";
            if ((sender as Button).Name == "button3")
                MyMessage = "Минимальное значение в поле " + comboBox2.Text + " = " + min.ToString();
            if ((sender as Button).Name == "button4")
                MyMessage = "Максимальное значение в поле " + comboBox2.Text + " = " + max.ToString();
            if ((sender as Button).Name == "button5")
                MyMessage = "Среднее значение в поле " + comboBox2.Text + " = " + (sum / dTable.Rows.Count).ToString();
            if ((sender as Button).Name == "button6")
                MyMessage = "Сумма значений в поле " + comboBox2.Text + " = " + sum.ToString();
            MessageBox.Show(MyMessage, "Расчеты", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1 && radioButton2.Checked == true)
            {
                MessageBox.Show("Выбирите производителя товара", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (radioButton1.Checked == true)
                ShowTable(SQL_AllTable());
            if (radioButton2.Checked == true)
                ShowTable(SQL_FilterByManufacture());
            if (radioButton3.Checked == true)
                ShowTable(SQL_FilterByProduct());
            if(radioButton4.Checked == true)
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                    ShowTable(FilterByPrice());
                else
                    MessageBox.Show("Не введён промежуток", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(radioButton5.Checked==true)
            {
                if (textBox1.Text != "" && textBox2.Text != "")
                    ShowTable(FilterByNum());
                else
                    MessageBox.Show("Не введён промежуток", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private string FilterByPrice()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " + "WHERE [Цена] >=" + Convert.ToInt32(textBox1.Text) + " AND Цена <=" + Convert.ToInt32(textBox2.Text);
        }

        private string FilterByNum()
        {
            return "SELECT * FROM [" + comboBox1.SelectedItem + "] " + "WHERE [Кол-во] >=" +
                Convert.ToInt32(textBox1.Text) + " AND [Кол-во] <=" + Convert.ToInt32(textBox2.Text);
        }

        bool PriceShown = false;

        private void button9_Click(object sender, EventArgs e)
        {
            string fileName = "test.txt";
            ;

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int col = 0; col < dTable.Columns.Count; col++)
                {
                    string textToAdd = dTable.Columns[col].ColumnName;

                    writer.Write(textToAdd + "//");

                    Console.WriteLine(textToAdd);
                }
                writer.Write("\n");
                for (int row = 0; row < dTable.Rows.Count; row++)
                {
                    for(int col = 0; col < dTable.Columns.Count;col++)
                    {
                        string textToAdd = dTable.Rows[row].ItemArray[col].ToString();
                        writer.Write(textToAdd + "//");
                    }
                    writer.Write("\n");
                }
            }
            
            

        }

        /*private void button8_Click(object sender, EventArgs e)
        {
            if (PriceShown == false)
            {
                chart1.Enabled = true;
                chart1.Visible = true;
                chart1.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series("Цены"));
                chart1.Series["Цены"].Enabled = true;
                for (int row = 0; row < dataGridView1.Rows.Count - 1; row++)
                {
                    chart1.Series["Цены"].Points.AddXY(row+1,dataGridView1.Rows[row].Cells[3].Value);
                    string LabelString = Convert.ToString(dataGridView1.Rows[row].Cells[1].Value) + " " +
                        Convert.ToString(dataGridView1.Rows[row].Cells[2].Value);
                    chart1.Series["Цены"].Points[row].Label = LabelString;
                }
                PriceShown = true;
            }
            else
            {
                chart1.Series.Clear();
                chart1.Visible=false;
                chart1.Enabled=false; 
                PriceShown = false;
            }
        }*/
    }
}
