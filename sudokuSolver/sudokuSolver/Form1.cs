using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sudokuSolver
{
    public partial class Form1 : Form
    {
        private Cell[,] table;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initDataSource();
            dataGridView1.DataSource
        }

        private void initDataSource()
        {
            table = new Cell[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    table[row, col] = new Cell { X = col, Y = row, Value = col + 1 };
                }
            }
        }
    }
}
