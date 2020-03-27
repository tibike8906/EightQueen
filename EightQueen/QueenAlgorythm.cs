using System;
using System.Collections.Generic;
using System.Linq;

namespace EightQueen
{

    class QueenAlgorythm
    {
        //A kezdő pozicó kovertáló metódusában engedélyezett betük és számok
        private static string[] _EnabledAlpa = { "A", "B", "C", "D", "E", "F", "G", "H" };
        private static string[] _EnabledNum = { "1", "2", "3", "4", "5", "6", "7", "8" };

        //A lépéseket tartalmazó lista
        private List<int[,]> _steps_of_table;
        //Az ideiglenes tábla melyeken a királynöket mozgatja az algoritmus ez van a lépés listába másolva lépésenként
        private int[,] _table;
        //A tábla mérete 8*8
        private const int _size_of_table = 8;
        //Kezdő sor és oszlop
        private int _start_row, _start_column;
                                        
        //Konstruktor a példányosításra lefutó metódus
        public QueenAlgorythm(int StartRow, int StartColumn)
        {
            //Beállítjuka változók kezdő értékét
            _start_row = StartRow;
            _start_column = StartColumn;            
            _table = new int[_size_of_table, _size_of_table];
            //Feltöltjük a táblát nullákkal kivéve a kezdő pozicót ott 1-lesz
            FillTable(_start_row, _start_column);
            //példányosítjuk a lépések táblát
            _steps_of_table = new List<int[,]>();
        }

        //A lépéseket adja vissza publikus
        public List<int[,]> GetSteps() { return _steps_of_table; }
        //Az indexeket adja vissza a Betű-szám típusú kezdő lépésből
        public static int[] GetNumPosition(string postion)
        {
            //két elemű int több 0 indexű a sor az 1-es az oszlopot takarja
            int[] pos = new int[2];
            //beállítjuk mind a kettőt 99-re így hibára elég ezt vissza adni
            pos[0] = 99;
            pos[1] = 99;

            //HA a mezőben nem két karakter van eleve hibás így vissza a 99-es pos
            if (postion.Length != 2) return pos;
            //Ha az első karakter nem eleme a _EnabledAlpa tömbnek vagy a második nem eleme a _EnabledNum tömbnek akkor hibás vissza a 99-es pos
            if (!_EnabledAlpa.Contains(postion.Substring(0, 1)) || !_EnabledNum.Contains(postion.Substring(1, 1))) return pos;
            //Megkeressük, hogy az első karakter hanyadik eleme a _EnabledAlpa tömbnek az az index lesz a sor index
            for (int i = 0; i < _EnabledAlpa.Length; i++)
            {
                if (_EnabledAlpa[i] == postion.Substring(0, 1))
                {
                    pos[0] = i;
                    break;
                }
            }
            //Oszlop indexhez elég a második karakter számmá konvertálni és kivonni egyett
            pos[1] = Int32.Parse(postion.Substring(1, 1)) - 1;
            //Vissza a pos minden rendben volt
            return pos;
        }
        //INdexből megcsinálja a Betű-Szám poziciót
        public static string GetAlpPostion(int[] position)
        {
            return _EnabledAlpa[position[0]] + _EnabledNum[position[1]].ToString();
        }
        //Algoritmus indítása erre azért van szükség mert a SolveNQ-egy rekurzív algoritmus és azt indítjuk el
        public void Solve()
        {             
            bool s = SolveNQ(_start_column);
        }
        private bool SolveNQ(int Column)
        {
            //Ha az oszlop nagyobb egyenlő mint a tábla mérete végig ment és true-val tér vissza
            if (Column >= _size_of_table)
                return true;

            //Végig megy a sorokonon
            for (int i = 0; i < _size_of_table; ++i)
            {
                //Ha biztonságba kihelyezhető a királynő
                if (IsSafe(i, Column, _table))
                {                                       
                    //Kiteszi a királynőt
                    _table[i, Column] = 1;
                    //Elmenti a lépést
                    _steps_of_table.Add(CopyTable());
                    //Vissza hívja magát a következő oszlopban
                    if (SolveNQ(Column + 1))
                        return true;
                    //Ha a rekürzió false volt akkor leveszi a királynőt
                    _table[i, Column] = 0;                                        
                }
            }
            
            return false;
        }
        //_table-t bele másoljuk egy új int tömbe mert máskülönben csak refernciaként helyezi a listába így mindeggyik elem (lépés) ugyan azt tartalmazza
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
        //A kirakott királynő biztonságát vizsgálja 
        public bool IsSafe(int row, int column, int[,] table)
        {
            int i, j;
            //Végig megy az összes oszlopon az adott sorban ha van királynő valahol false-t ad vissza
            for (i = 0; i < column; ++i)
                if (Convert.ToBoolean(table[row, i]))
                    return false;
            //Az átlókon mennek végig
            for (i = row, j = column; i >= 0 && j >= 0; --i, --j)
                if (Convert.ToBoolean(table[i, j]))
                    return false;
            
            for (i = row, j = column; j >= 0 && i < _size_of_table; ++i, --j)
                if (Convert.ToBoolean(table[i, j]))
                    return false;
            return true;
        }
        //Tábla feltöltése
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
