using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudokuSolver
{
    public class Cell
    {
        public Cell()
        {

        }

        public int X { get; set; }

        private int _bigX;
        public int BigX
        {
            get
            {
                switch (X)
                {
                    case 0:
                    case 1:
                    case 2:
                        _bigX = 0;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        _bigX = 1;
                        break;
                    default:
                        _bigX = 2;
                        break;
                }
                return _bigX;
            }
        }


        public int Y { get; set; }

        private int _bigY;
        public int BigY
        {
            get
            {
                switch (Y)
                {
                    case 0:
                    case 1:
                    case 2:
                        _bigY = 0;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        _bigY = 1;
                        break;
                    default:
                        _bigY = 2;
                        break;
                }
                return _bigY;
            }
        }


        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsChanged = true;
            }
        }



        public bool IsChanged { get; private set; }

        public void ResetChangeFlag()
        {
            IsChanged = false;
        }
    }
}
