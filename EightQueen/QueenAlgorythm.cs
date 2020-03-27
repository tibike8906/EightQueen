using System;
using System.Collections.Generic;
using System.Linq;

namespace EightQueen
{

    class QueenAlgorythm
    {
        private static string[] _EnabledAlpa = { "A", "B", "C", "D", "E", "F", "G", "H" };
        private static string[] _EnabledNum = { "1", "2", "3", "4", "5", "6", "7", "8" };

        private List<int[,]> _steps_of_table;
        private int[,] _table;

        private const int _size_of_table = 8;
        
        private int _start_row, _start_column;
                                        
        public QueenAlgorythm(int StartRow, int StartColumn)
        {
            _start_row = StartRow;
            _start_column = StartColumn;            
            _table = new int[_size_of_table, _size_of_table];
            FillTable(_start_row, _start_column);
            _steps_of_table = new List<int[,]>();
        }
        public List<int[,]> GetSteps() { return _steps_of_table; }
        public static int[] GetNumPosition(string postion)
        {
            int[] pos = new int[2];
            pos[0] = 99;
            pos[1] = 99;


            if (postion.Length != 2) return pos;
            if (!_EnabledAlpa.Contains(postion.Substring(0, 1)) || !_EnabledNum.Contains(postion.Substring(1, 1))) return pos;
            for (int i = 0; i < _EnabledAlpa.Length; i++)
            {
                if (_EnabledAlpa[i] == postion.Substring(0, 1))
                {
                    pos[0] = i;
                    break;
                }
            }
            pos[1] = Int32.Parse(postion.Substring(1, 1)) - 1;
            return pos;
        }
        public static string GetAlpPostion(int[] position)
        {
            return _EnabledAlpa[position[0]] + _EnabledNum[position[1]].ToString();
        }

        public void Solve()
        {             
            bool s = SolveNQ(_start_column);
        }
        private bool SolveNQ(int Column)
        {
            if (Column >= _size_of_table)
                return true;


            
            for (int i = 0; i < _size_of_table; ++i)
            {
                if (IsSafe(i, Column, _table))
                {                                                            
                    _table[i, Column] = 1;                    
                    _steps_of_table.Add(CopyTable());
                    if (SolveNQ(Column + 1))
                        return true;

                    _table[i, Column] = 0;                                        
                }
            }
            
            return false;
        }
        private int[,] CopyTable()
        {
            int[,] c = new int[_size_of_table, _size_of_table];
            for (int i = 0; i < _size_of_table; i++)
            {
                for (int j = 0; j < _size_of_table; j++)
                {
                    c[i, j] = _table[i, j];
                }
            }
            return c;
        }
        public bool IsSafe(int row, int column, int[,] table)
        {
            int i, j;
            for (i = 0; i < column; ++i)
                if (Convert.ToBoolean(table[row, i]))
                    return false;

            for (i = row, j = column; i >= 0 && j >= 0; --i, --j)
                if (Convert.ToBoolean(table[i, j]))
                    return false;

            for (i = row, j = column; j >= 0 && i < _size_of_table; ++i, --j)
                if (Convert.ToBoolean(table[i, j]))
                    return false;
            return true;
        }
        private void FillTable(int StartRow, int StartColumn)
        {
            for (int i = 0; i < _size_of_table; i++)
            {
                for (int j = 0; j < _size_of_table; j++)
                {
                    if (i == StartRow && j == StartColumn)
                        _table[i, j] = 1;
                    else
                        _table[i, j] = 0;
                }
            }
        }
    }
}
