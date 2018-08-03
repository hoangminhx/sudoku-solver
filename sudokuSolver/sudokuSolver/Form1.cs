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
        private List<Cell> cells;
        private Cell[,] tableCells;
        private Cell[][] tableDisplay;

        private List<int> FullNumbers;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FullNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            initDataSource();
            ShowValueToGrid();
        }

        private void initDataSource()
        {
            cells = new List<Cell>();
            tableDisplay = new Cell[9][];
            tableCells = new Cell[9, 9];

            for (int y = 0; y < 9; y++)
            {
                Cell[] row = new Cell[9];
                for (int x = 0; x < 9; x++)
                {
                    Cell cell = new Cell
                    {
                        X = x,
                        Y = y
                    };
                    row[x] = cell;
                    cells.Add(cell);
                    tableCells[y, x] = cell;
                }
                tableDisplay[y] = row;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetValueFromGrid();
            Solve();
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl txt = (DataGridViewTextBoxEditingControl)e.Control;
            txt.KeyUp -= new KeyEventHandler(Txt_KeyUp);
            txt.KeyUp += new KeyEventHandler(Txt_KeyUp);
        }

        private void Txt_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            dataGridView1.CurrentCell.Value = txt.Text;

            if (int.TryParse(txt.Text, out int temp))
            {
                DataGridViewCell currentCell = dataGridView1.CurrentCell;
                if (currentCell.ColumnIndex < 8)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[currentCell.RowIndex].Cells[currentCell.ColumnIndex + 1];
                }
                else if (currentCell.RowIndex < 8)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[currentCell.RowIndex + 1].Cells[0];
                }
            }
        }

        private void GetValueFromGrid()
        {
            for (int y = 0; y < 9; y++)
            {
                DataGridViewRow row = dataGridView1.Rows[y];
                for (int x = 0; x < 9; x++)
                {
                    DataGridViewCell cell = row.Cells[x];
                    tableCells[y, x].Value = int.Parse(cell.Value.ToString());
                }
            }
        }

        private void ShowValueToGrid()
        {
            for (int y = 0; y < 9; y++)
            {
                Cell[] row = new Cell[9];
                for (int x = 0; x < 9; x++)
                {
                    row[x] = tableCells[y, x];
                }
                tableDisplay[y] = row;
            }

            dataGridView1.DataSource = tableDisplay.Select(c => new DisplayObject
            {
                c0 = c[0].Value,
                c1 = c[1].Value,
                c2 = c[2].Value,
                c3 = c[3].Value,
                c4 = c[4].Value,
                c5 = c[5].Value,
                c6 = c[6].Value,
                c7 = c[7].Value,
                c8 = c[8].Value,
            }).ToList();
        }

        private void ResetChangeFlags()
        {
            foreach (Cell cell in cells)
            {
                cell.ResetChangeFlag();
            }
        }

        private void Solve()
        {
            #region solusion

            /// 1. tim tung dong xem co dong nao trong 1 o khong
            /// 2. tim tung cot xem co cot nao trong 1 o khong
            /// 3. tim 9 o lon xem co o nao trong 1 o khong
            /// 4. duyet tu 1 den 9
            ///    4.1 duyet qua cac dong lon
            ///       4.1.1 neu 2 trong 3 o lon co con so dang duyet thi
            ///          4.1.1.1 lay ra index cua 2 dong trung voi so dang duyet
            ///          4.1.1.2 lay o lon con lai, lay ra cac o nho trong cua dong ma khong thuoc index cua 2 dong trung voi so dang duyet
            ///          4.1.1.3 neu so luong lay ra duoc bang 1 thi
            ///             4.1.1.3.1 dien so dang duyet vao
            ///          4.1.1.4 neu so luong lon hon 1 thi duyet qua tung o
            ///             4.1.1.4.1 kiem tra so dang duyet tren cot cua o hien tai
            ///             neu ton tai thi xoa khoi danh sach ung vien
            ///             4.1.1.4.2 sau khi duyet qua tung o, neu danh sach ung vien la 1 thi
            ///                4.1.1.4.2.1 dien so dang duyet vao
            ///       4.1.2 neu chi co 1 o co so dang duyet thi
            ///          4.1.2.1 duyet qua cac o lon con lai tren dong do
            ///             4.1.2.1.1 lay ra cac o nho con trong ma khong thuoc index cua dong dang duyet
            ///             4.1.2.1.2 neu so luong o nho la 1 thi dien so dang duyet vao
            ///             4.1.2.1.3 neu so luong lon hon 1 thi lam tuong tu 4.1.1.4
            ///    4.2 duyet qua cac cot lon
            ///       4.2.1 neu 2 trong 3 o lon co con so dang duyet thi
            ///          4.2.1.1 lay ra index cua 2 cot trung voi so dang duyet
            ///          4.2.1.2 lay o lon con lai, lay ra cac o nho con trong cua cot ma khong thuoc index cua 2 cot trung voi so dang duyet
            ///          4.2.1.3 neu so luong lay ra duoc bang 1 thi
            ///             4.2.1.3.1 dien so dang duyet vao
            ///          4.2.1.4 neu so luong lon hon 1 thi duyet qua tung o
            ///             4.2.1.4.1 kiem tra so dang duyet tren dong cua o hien tai
            ///             neu ton tai thi xoa khoi danh sach ung vien
            ///             4.2.1.4.2 sau khi duyet qua tung o, neu danh sach ung vien la 1 thi
            ///                4.2.1.4.2.1 dien so dang duyet vao
            ///       4.2.2 neu chi co 1 o co so dang duyet thi
            ///          4.2.2.1 duyet qua cac o lon con lai tren cot do
            ///             4.2.2.1.1 lay ra cac o nho con trong ma khong thuoc index cua cot dang duyet
            ///             4.2.2.1.2 neu so luong bang 1 thi dien so dang duyet vao
            ///             4.2.2.1.3 neu so luong lon hon 1 thi lam tuong tu 4.2.1.4
            /// 5. duyet tung dong
            ///    5.1 lay ra cac o trong
            ///    5.2 duyet qua cac o trong
            ///       5.2.1 loai tru cac phan tu con thieu trong cot cua o hien tai
            ///          5.2.1.1 neu sau khi loai tru con 1 o thi dien so con lai vao o do
            /// 6. duyet qua tung cot (lam tuong tu voi dong)
            /// 7. duyet qua tung o
            ///    7.1 lay tat ca so tren cot
            ///    7.2 lay tat ca so tren dong
            ///    7.3 lay tat ca so tren o lon
            ///    7.4 loai tru
            ///    7.5 neu con lai 1 so thi dien vao
            /// 
            /// neu danh sach co thay doi thi cap nhat danh sach, lap lai tu dau 
            /// neu khong doi thi stop
            /// 
            /// neu ben tren chua giai het dc cac o
            /// 8. Note lai nhung so co kha nang ton tai trong 1 o
            /// 9. Duyet tu 1 den 9
            ///    9.1 duyet qua tung dong
            ///       9.1.1 dem xem so hien tai xuat hien bao nhieu lan
            ///       9.1.2 neu 1 lan thi dien vao o do luon
            ///          9.1.2.1 xoa so tuong ung trong note cua dong/cot/o lon
            ///    9.2 duyet qua tung cot
            ///       9.2.1 dem xem so hien tai xuat hien bao nhieu lan
            ///       9.2.2 neu 1 lan thi dien vao o do luon
            ///          9.2.2.1 xoa so tuong ung trong note cua dong/cot/o lon
            ///    9.3 duyet qua tung o lon
            ///       9.3.1 dem xem so hien tai xuat hien bao nhieu lan
            ///       9.3.2 neu 1 lan thi dien vao o do luon
            ///          9.3.2.1 xoa so tuong ung trong note cua dong/cot/o lon
            /// 10. Duyet qua tung o lon
            ///    10.1 tim nhung so trong note chi nam tren 1 dong
            ///    10.2 xoa so tuong ung trong note cua dong do
            ///    10.3 tim nhung so trong note chi nam tren 1 cot
            ///    10.4 xoa so tuong ung trong note cua cot do
            ///    10.5 lap lai tu buoc 9
            /// 11. Duyet qua tung o trong, neu note chi co 1 phan tu thi
            ///    11.1 dien vao o do luon
            ///    11.2 xoa so tuong ung trong note cua dong/cot/o lon

            #endregion

            //bool isNew = true;
            //while (isNew)
            //{
            //    ResetChangeFlags();

            //    //1
            //    CheckRowsHaveOneBlank();
            //    //2
            //    CheckColumnsHaveOneBlank();
            //    //3
            //    CheckBigCellsHaveOneBlank();

            //    //4
            //    CheckBigRowsAndBigCols();

            //    //5
            //    CheckBlankCelsOnRows();
            //    //6
            //    CheckBlankCelsOnCols();

            //    //7
            //    CheckIfOnlyOneNumberValid();

            //    isNew = cells.Any(c => c.IsChanged);
            //}

            //if (cells.Any(c => c.Value == 0))
            //{
            //    //8
            //    TakeNote();
            //    //9

            //    isNew = true;
            //    while (isNew)
            //    {
            //        ResetChangeFlags();

            //        CheckIfNumberInNoteIsUniqueInUnit();

            //        int cntOfNotes = cells.SelectMany(c => c.Note).Count();
            //        CheckNoteInBigCellToClearOtherNotes();
            //        CheckIfNoteHaveOneNumber();
            //        CheckIfSetOfNumbersInNoteIsUniqueInUnit();
            //        int newCntOfNote = cells.SelectMany(c => c.Note).Count();

            //        isNew = cells.Any(c => c.IsChanged) || cntOfNotes != newCntOfNote;
            //    }
            //}

            #region new idea

            TakeNote();
            bool isNew = true;

            while (isNew)
            {
                ResetChangeFlags();

                int cntOfNotes = cells.SelectMany(c => c.Note).Count();

                CheckIfNoteHaveOneItem();
                CheckIfOneNoteNumberInAUnit();

                int newCntOfNote = cells.SelectMany(c => c.Note).Count();

                isNew = cells.Any(c => c.IsChanged) || cntOfNotes != newCntOfNote;
            }

            #endregion

            ShowValueToGrid();
        }

        private void CheckIfNoteHaveOneItem()
        {
            var oneNoteCells = cells.Where(c => c.Note.Count == 1);
            foreach (var cell in oneNoteCells)
            {
                cell.Value = cell.Note.First();
                ClearMatchedNote(cell.Y, cell.X, cell.Value);
            }
        }

        /// <summary>
        /// check if the number in note only in an unit (row, col, big cell)
        /// </summary>
        private void CheckIfOneNoteNumberInAUnit()
        {
            var rows = GetRows();
            var cols = GetColumns();
            var bigCells = GetBigCells();

            for (int number = 1; number <= 9; number++)
            {
                var rowTemps = rows.Where(c => c.SelectMany(x => x.Note).Count(x => x == number) == 1);
                foreach (var row in rowTemps)
                {
                    var cell = row.First(c => c.Note.Contains(number));
                    cell.Value = number;
                    ClearMatchedNote(cell.Y, cell.X, number);
                }

                var colTemps = cols.Where(c => c.SelectMany(x => x.Note).Count(x => x == number) == 1);
                foreach (var col in colTemps)
                {
                    var cell = col.First(c => c.Note.Contains(number));
                    cell.Value = number;
                    ClearMatchedNote(cell.Y, cell.X, number);
                }

                var bigCellTemps = rows.Where(c => c.SelectMany(x => x.Note).Count(x => x == number) == 1);
                foreach (var bigCell in bigCellTemps)
                {
                    var cell = bigCell.First(c => c.Note.Contains(number));
                    cell.Value = number;
                    ClearMatchedNote(cell.Y, cell.X, number);
                }
            }
        }

        private void CheckIfNumberInNoteIsUniqueInUnit()
        {
            //Check if number in note is unique in a row/col/big cell
            for (int currentNum = 1; currentNum <= 9; currentNum++)
            {
                //9.1
                var rows = cells.GroupBy(c => c.Y).Select(c => c.ToList());
                foreach (var row in rows)
                {
                    var matchedCells = row.Where(c => c.Note.Contains(currentNum)).ToArray();
                    if (matchedCells.Length == 1)
                    {
                        matchedCells[0].Value = currentNum;
                        matchedCells[0].Note = new List<int>();
                        ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
                    }
                }

                //9.2
                var cols = cells.GroupBy(c => c.X).Select(c => c.ToList());
                foreach (var col in cols)
                {
                    var matchedCells = col.Where(c => c.Note.Contains(currentNum)).ToArray();
                    if (matchedCells.Length == 1)
                    {
                        matchedCells[0].Value = currentNum;
                        matchedCells[0].Note = new List<int>();
                        ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
                    }
                }

                //9.3
                var bigCells = GetBigCells();
                foreach (var bigCell in bigCells)
                {
                    var matchedCells = bigCell.Where(c => c.Note.Contains(currentNum)).ToArray();
                    if (matchedCells.Length == 1)
                    {
                        matchedCells[0].Value = currentNum;
                        matchedCells[0].Note = new List<int>();
                        ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
                    }
                }
            }
        }

        private void CheckIfSetOfNumbersInNoteIsUniqueInUnit()
        {
            ////Check if set of numbers in note is unique in a row/col/big cell
            //var blankCells = cells.Where(c => c.Value == 0);
            //foreach (var cell in blankCells)
            //{
            //    var currentNote = cell.Note;
            //    //9.1
            //    var rows = cells.GroupBy(c => c.Y).Select(c => c.ToList());
            //    foreach (var row in rows)
            //    {
            //        var matchedCells = row.Where(c => !currentNote.Except(c.Note).Any()).ToArray();
            //        if (matchedCells.Length == 1)
            //        {
            //            matchedCells[0].Value = currentNum;
            //            matchedCells[0].Note = new List<int>();
            //            ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
            //        }
            //    }

            //    //9.2
            //    var cols = cells.GroupBy(c => c.X).Select(c => c.ToList());
            //    foreach (var col in cols)
            //    {
            //        var matchedCells = col.Where(c => !currentNote.Except(c.Note).Any()).ToArray();
            //        if (matchedCells.Length == 1)
            //        {
            //            matchedCells[0].Value = currentNum;
            //            matchedCells[0].Note = new List<int>();
            //            ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
            //        }
            //    }

            //    //9.3
            //    var bigCells = GetBigCells();
            //    foreach (var bigCell in bigCells)
            //    {
            //        var matchedCells = bigCell.Where(c => !currentNote.Except(c.Note).Any()).ToArray();
            //        if (matchedCells.Length == 1)
            //        {
            //            matchedCells[0].Value = currentNum;
            //            matchedCells[0].Note = new List<int>();
            //            ClearMatchedNote(matchedCells[0].Y, matchedCells[0].X, currentNum);
            //        }
            //    }
            //}
        }

        private void CheckNoteInBigCellToClearOtherNotes()
        {
            var bigCells = GetBigCells();
            //10
            foreach (var bigCell in bigCells)
            {
                //10.1
                var rows = bigCell.GroupBy(c => c.Y).Select(c => c.ToList()).ToList();

                List<int> rowNotes = new List<int>();
                rowNotes.AddRange(rows[0].SelectMany(c => c.Note).Distinct());
                rowNotes.AddRange(rows[1].SelectMany(c => c.Note).Distinct());
                rowNotes.AddRange(rows[2].SelectMany(c => c.Note).Distinct());

                List<int> duplicateInRows = rowNotes.GroupBy(c => c).SelectMany(c => c.Skip(1)).Distinct().ToList();
                rowNotes.RemoveAll(c => duplicateInRows.Contains(c));
                if (rowNotes.Count > 0)
                {
                    foreach (int numNote in rowNotes)
                    {
                        Cell cellTemp = bigCell.First(c => c.Note.Contains(numNote));
                        //10.2
                        ClearMatchedNoteInOtherBigCellInBigRow(cellTemp.Y, cellTemp.BigX, numNote);
                    }
                }

                //10.3
                var cols = bigCell.GroupBy(c => c.X).Select(c => c.ToList()).ToList();

                List<int> colNotes = new List<int>();
                colNotes.AddRange(cols[0].SelectMany(c => c.Note).Distinct());
                colNotes.AddRange(cols[1].SelectMany(c => c.Note).Distinct());
                colNotes.AddRange(cols[2].SelectMany(c => c.Note).Distinct());

                List<int> duplicateInCols = colNotes.GroupBy(c => c).SelectMany(c => c.Skip(1)).Distinct().ToList();
                colNotes.RemoveAll(c => duplicateInCols.Contains(c));
                if (colNotes.Count > 0)
                {
                    foreach (int numNote in colNotes)
                    {
                        Cell cellTemp = bigCell.First(c => c.Note.Contains(numNote));
                        //10.4
                        ClearMatchedNoteInOtherBigCellInBigCol(cellTemp.X, cellTemp.BigY, numNote);
                    }
                }
            }
        }

        private void CheckIfNoteHaveOneNumber()
        {
            //11
            var oneNoteCells = cells.Where(c => c.Note.Count == 1);
            foreach (var cell in oneNoteCells)
            {
                //11.1
                cell.Value = cell.Note[0];
                //11.2
                ClearMatchedNote(cell.Y, cell.X, cell.Value);
            }
        }

        private void TakeNote()
        {
            var blankCells = cells.Where(c => c.Value == 0);
            foreach (var cell in blankCells)
            {
                cell.Note = GetNumbersValidForCell(cell.Y, cell.X);
            }
        }

        private void ClearMatchedNote(int y, int x, int num)
        {
            var row = cells.Where(c => c.Y == y && c.Note.Contains(num));
            foreach (var cell in row)
            {
                cell.Note.Remove(num);
            }

            var col = cells.Where(c => c.X == x && c.Note.Contains(num));
            foreach (var cell in col)
            {
                cell.Note.Remove(num);
            }

            var cellTemp = cells.First(c => c.X == x && c.Y == y);
            cellTemp.Note = new List<int>();

            var bigCell = cells.Where(c => c.BigX == cellTemp.BigX && c.BigY == cellTemp.BigY 
                                        && c.Note.Contains(num));
            foreach (var cell in bigCell)
            {
                cell.Note.Remove(num);
            }
        }

        private void ClearMatchedNoteInOtherBigCellInBigRow(int y, int bigX, int num)
        {
            var row = cells.Where(c => c.Y == y && c.BigX != bigX && c.Note.Contains(num));
            foreach (var cell in row)
            {
                cell.Note.Remove(num);
            }
        }

        private void ClearMatchedNoteInOtherBigCellInBigCol(int x, int bigY, int num)
        {
            var col = cells.Where(c => c.X == x && c.BigY != bigY && c.Note.Contains(num));
            foreach (var cell in col)
            {
                cell.Note.Remove(num);
            }
        }

        private void CheckRowsHaveOneBlank()
        {
            var rows = cells.GroupBy(c => c.Y).Select(g => g.ToList());
            foreach (var row in rows)
            {
                var zeroCells = row.Where(c => c.Value == 0).ToArray();
                if (zeroCells.Length == 1)
                {
                    var existNumbers = row.Where(c => c.Value != 0).Select(c => c.Value);
                    var missingNumbers = FullNumbers.Except(existNumbers);
                    zeroCells[0].Value = missingNumbers.ElementAt(0);
                }
            }
        }

        private void CheckColumnsHaveOneBlank()
        {
            var columns = cells.GroupBy(c => c.X).Select(g => g.ToList());
            foreach (var col in columns)
            {
                var zeroCells = col.Where(c => c.Value == 0).ToArray();
                if (zeroCells.Length == 1)
                {
                    var existNumbers = col.Where(c => c.Value != 0).Select(c => c.Value);
                    var missingNumbers = FullNumbers.Except(existNumbers);
                    zeroCells[0].Value = missingNumbers.ElementAt(0);
                }
            }
        }

        private void CheckBigCellsHaveOneBlank()
        {
            var bigCells = GetBigCells();
            foreach (var bigCell in bigCells)
            {
                var zeroCells = bigCell.Where(c => c.Value == 0).ToArray();
                if (zeroCells.Length == 1)
                {
                    var existNumbers = bigCell.Where(c => c.Value != 0).Select(c => c.Value);
                    var missingNumbers = FullNumbers.Except(existNumbers);
                    zeroCells[0].Value = missingNumbers.ElementAt(0);
                }
            }
        }

        private void CheckBigRowsAndBigCols()
        {
            for (int currentNumber = 1; currentNumber <= 9; currentNumber++)
            {
                //4.1
                var bigRows = cells.GroupBy(c => c.BigY).ToList();
                foreach (var bigRow in bigRows)
                {
                    int cntOfCurrentNum = bigRow.Count(c => c.Value == currentNumber);
                    //4.1.1
                    if (cntOfCurrentNum == 2)
                    {
                        var cellOfCurrentNum = bigRow.Where(c => c.Value == currentNumber);
                        //4.1.1.1
                        int[] rowIdsOfCurrentNum = cellOfCurrentNum.Select(c => c.Y).ToArray();

                        int[] bigColIdsOfCurrentNum = cellOfCurrentNum.Select(c => c.BigX).ToArray();
                        //4.1.1.2
                        var availableCells = bigRow.Where(c => !bigColIdsOfCurrentNum.Contains(c.BigX)
                                                            && !rowIdsOfCurrentNum.Contains(c.Y)
                                                            && c.Value == 0)
                                                   .ToArray();
                        //4.1.1.3
                        if (availableCells.Length == 1)
                        {
                            //4.1.1.3.1
                            availableCells[0].Value = currentNumber;
                        }
                        else if (availableCells.Length > 1) //4.1.1.4
                        {
                            Step4_1_1_4(availableCells, currentNumber);
                        }
                    }
                    //4.1.2
                    else if (cntOfCurrentNum == 1)
                    {
                        var matchedCell = bigRow.First(c => c.Value == currentNumber);
                        var bigCells = bigRow.GroupBy(c => c.BigX)
                                             .Where(c => c.Key != matchedCell.BigX);
                        //4.1.2.1
                        foreach (var bigCell in bigCells)
                        {
                            //4.1.2.1.1
                            var availableCells = bigCell.Where(c => c.Y != matchedCell.Y && c.Value == 0).ToArray();
                            //4.1.2.1.2
                            if (availableCells.Length == 1)
                            {
                                availableCells[0].Value = currentNumber;
                            }
                            else if (availableCells.Length > 1) //4.1.2.1.3
                            {
                                Step4_1_1_4(availableCells, currentNumber);
                            }
                        }
                    }
                }

                //4.2
                var bigCols = cells.GroupBy(c => c.BigX).ToList();
                foreach (var bigCol in bigCols)
                {
                    int cntOfCurrentNum = bigCol.Count(c => c.Value == currentNumber);
                    //4.2.1
                    if (cntOfCurrentNum == 2)
                    {
                        var cellOfCurrentNum = bigCol.Where(c => c.Value == currentNumber);
                        //4.2.1.1
                        int[] colIdsOfCurrentNum = cellOfCurrentNum.Select(c => c.X).ToArray();

                        int[] bigRowIdsOfCurrentNum = cellOfCurrentNum.Select(c => c.BigY).ToArray();
                        //4.2.1.2
                        var availableCells = bigCol.Where(c => !bigRowIdsOfCurrentNum.Contains(c.BigY)
                                                            && !colIdsOfCurrentNum.Contains(c.X)
                                                            && c.Value == 0)
                                                   .ToArray();
                        //4.2.1.3
                        if (availableCells.Length == 1)
                        {
                            //4.2.1.3.1
                            availableCells[0].Value = currentNumber;
                        }
                        else if (availableCells.Length > 1) //4.2.1.4
                        {
                            Step4_2_1_4(availableCells, currentNumber);
                        }
                    }
                    //4.2.2
                    else if (cntOfCurrentNum == 1)
                    {
                        var matchedCell = bigCol.First(c => c.Value == currentNumber);
                        var bigCells = bigCol.GroupBy(c => c.BigY)
                                             .Where(c => c.Key != matchedCell.BigY);
                        //4.2.2.1
                        foreach (var bigCell in bigCells)
                        {
                            //4.2.2.1.1
                            var availableCells = bigCell.Where(c => c.X != matchedCell.X && c.Value == 0).ToArray();
                            //4.2.2.1.2
                            if (availableCells.Length == 1)
                            {
                                availableCells[0].Value = currentNumber;
                            }
                            else if (availableCells.Length > 1) //4.2.2.1.3
                            {
                                Step4_2_1_4(availableCells, currentNumber);
                            }
                        }
                    }
                }
            }
        }

        private void Step4_1_1_4(IEnumerable<Cell> availableCells, int currentNumber)
        {
            //4.1.1.4.1
            //4.1.1.4.2
            var colIdOfAvailableCells = availableCells.Select(c => c.X);
            var colIdOfNotAvailableCells = cells.Where(c => colIdOfAvailableCells.Contains(c.X)
                                                     && c.Value != 0
                                                     && c.Value == currentNumber)
                                            .Select(c => c.X)
                                            .Distinct();
            var idOfAvailableCols = colIdOfAvailableCells.Except(colIdOfNotAvailableCells);

            //4.1.1.4.2.1
            if (idOfAvailableCols.Count() == 1 && availableCells.Count(c => c.X == idOfAvailableCols.ElementAt(0)) == 1)
            {
                var cell = availableCells.First(c => c.X == idOfAvailableCols.ElementAt(0));
                cell.Value = currentNumber;
            }
        }

        private void Step4_2_1_4(IEnumerable<Cell> availableCells, int currentNumber)
        {
            //4.2.1.4.1
            //4.2.1.4.2
            var rowIdOfAvailableCells = availableCells.Select(c => c.Y);
            var rowIdOfNotAvailableCells = cells.Where(c => rowIdOfAvailableCells.Contains(c.Y)
                                                     && c.Value != 0
                                                     && c.Value == currentNumber)
                                            .Select(c => c.Y)
                                            .Distinct();
            var idOfAvailableRows = rowIdOfAvailableCells.Except(rowIdOfNotAvailableCells);

            //4.2.1.4.2.1
            if (idOfAvailableRows.Count() == 1 && availableCells.Count(c => c.Y == idOfAvailableRows.ElementAt(0)) == 1)
            {
                var cell = availableCells.First(c => c.Y == idOfAvailableRows.ElementAt(0));
                cell.Value = currentNumber;
            }
        }

        private void CheckBlankCelsOnRows()
        {
            //5.1
            var blankCells = cells.Where(c => c.Value == 0);

            Dictionary<int, List<int>> dicMissingNumber = new Dictionary<int, List<int>>();
            for (int r = 0; r < 9; r++)
            {
                dicMissingNumber.Add(r, GetMissingNumbersOnRow(r));
            }

            //5.2
            foreach (var cell in blankCells)
            {
                List<int> numberNotValidForCells = GetNumbersNotValidForCell(cell.Y, cell.X);
                //5.2.1
                int[] numberValidForCells = dicMissingNumber[cell.Y].Except(numberNotValidForCells).ToArray();
                if (numberValidForCells.Length == 1)
                {
                    //5.2.1.1
                    cell.Value = numberValidForCells[0];
                }
            }
        }

        private List<int> GetMissingNumbersOnRow(int rowIndex)
        {
            List<int> result = new List<int>();

            var existingNumbers = cells.Where(c => c.Y == rowIndex && c.Value != 0).Select(c => c.Value);
            result = FullNumbers.Except(existingNumbers).ToList();

            return result;
        }

        private List<int> GetMissingNumbersOnCol(int colIndex)
        {
            List<int> result = new List<int>();

            var existingNumbers = cells.Where(c => c.X == colIndex && c.Value != 0).Select(c => c.Value);
            result = FullNumbers.Except(existingNumbers).ToList();

            return result;
        }

        private void CheckBlankCelsOnCols()
        {
            //6.1
            var blankCells = cells.Where(c => c.Value == 0);

            Dictionary<int, List<int>> dicMissingNumber = new Dictionary<int, List<int>>();
            for (int col = 0; col < 9; col++)
            {
                dicMissingNumber.Add(col, GetMissingNumbersOnCol(col));
            }

            //6.2
            foreach (var cell in blankCells)
            {
                List<int> numberNotValidForCells = GetNumbersNotValidForCell(cell.Y, cell.X);
                //6.2.1
                int[] numberValidForCells = dicMissingNumber[cell.X].Except(numberNotValidForCells).ToArray();
                if (numberValidForCells.Length == 1)
                {
                    //6.2.1.1
                    cell.Value = numberValidForCells[0];
                }
            }
        }

        private List<int> GetNumbersNotValidForCell(int y, int x)
        {
            List<int> result = new List<int>();

            Cell temp = new Cell { X = x, Y = y };

            result.AddRange(cells.Where(c => c.X == x && c.Value != 0).Select(c => c.Value));
            result.AddRange(cells.Where(c => c.Y == y && c.Value != 0).Select(c => c.Value));
            result.AddRange(cells.Where(c => c.BigX == temp.BigX && c.BigY == temp.BigY && c.Value != 0).Select(c => c.Value));
            result = result.Distinct().ToList();

            return result;
        }
        private List<int> GetNumbersValidForCell(int y, int x)
        {
            List<int> result = new List<int>();
            List<int> numbersNotValid = GetNumbersNotValidForCell(y, x);
            result = FullNumbers.Except(numbersNotValid).ToList();

            return result;
        }

        private void CheckIfOnlyOneNumberValid()
        {
            var blankCells = cells.Where(c => c.Value == 0);
            foreach (var cell in blankCells)
            {
                List<int> numberNotValidForCells = GetNumbersNotValidForCell(cell.Y, cell.X);
                int[] numberValidForCells = FullNumbers.Except(numberNotValidForCells).ToArray();
                if (numberValidForCells.Length == 1)
                {
                    cell.Value = numberValidForCells[0];
                }
            }
        }

        private IEnumerable<List<Cell>> GetRows()
        {
            return cells.GroupBy(c => c.Y).Select(g => g.ToList());
        }

        private IEnumerable<List<Cell>> GetColumns()
        {
            return cells.GroupBy(c => c.X).Select(g => g.ToList());
        }

        private IEnumerable<List<Cell>> GetBigCells()
        {
            return cells.GroupBy(c => new { c.BigX, c.BigY }).Select(g => g.ToList());
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = Color.BlueViolet;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.RowIndex > 2 && cell.RowIndex < 6
                        && cell.ColumnIndex > 2 && cell.ColumnIndex < 6)
                    {
                        cell.Style = style;
                    }
                }
            }
        }

    }

    public class DisplayObject
    {
        public int c0 { get; set; }
        public int c1 { get; set; }
        public int c2 { get; set; }
        public int c3 { get; set; }
        public int c4 { get; set; }
        public int c5 { get; set; }
        public int c6 { get; set; }
        public int c7 { get; set; }
        public int c8 { get; set; }
    }
}
