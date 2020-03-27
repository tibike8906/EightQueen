using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace EightQueen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //A királynő Bitmapja az összes image ezt használja
        private BitmapImage _queen_bitmap = new BitmapImage(new Uri("q1.png", UriKind.RelativeOrAbsolute));
        //A sakktábla egy vektor az algoritmus táblája ebbe van bele töltve
        private int[,] _table;
        //Miután az algoritmus elkészítette a lépéseket, ezt növelve megyünk végig rajta
        private int _step = 0;        
        //A királynők (képek) tára ebben a listában lehet szűrni hogy éppen hol vannak stb,
        private List<Image> _eightQueen;
        //Maga az algoritmus példánya
        private QueenAlgorythm _queen_alg;
        //Időzítő példánya
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            //Példányosítjuk a királynők listáját és feltöltjük 8 image-el 
            _eightQueen = new List<Image>();
            for (int i = 0; i < 8; i++)
            {
                
                Image img = new Image();

                img.Width = 80;
                img.Height = 80;
                img.Source = _queen_bitmap;
                img.Name = "q"+ i;
                _eightQueen.Add(img);
                //Alap esetben a várakozó zónába kerülnek később innen lehet elvenni és a táblára rakni
                grWaiterZone.Children.Add(img);                
            }
            //A timer példányosítása
            _timer = new DispatcherTimer();
            //A timer időzítőjére a timer_Tick metódus fut le
            _timer.Tick += timer_Tick;

        }             
        void timer_Tick(object sender, EventArgs e)
        {
            //Erre azért van szükség, hogy újra állítja az időzítés a csúszkáról így menet közben is lehet sebességet állítani
            //A csúszkán 1000 a max pont fordított a timerrel mert úgy gyorsabb ha minnél kisebb ezért 1000-ből vonjuk ki a csúszka értékét
            //és az a sebesség
            _timer.Interval = TimeSpan.FromMilliseconds(1000 - sSpeed.Value);
            //Ha lépés nagyobb egyenlő az összes lépés számával vége az időzítésnek
            if (_step >= _queen_alg.GetSteps().Count())
            {
                //Leállítjuk az időzítőt
                _timer.Stop();
                //ha maradt még királynő a waiter zónában akkor az algoritmus nem talált megoldást, ellenkezőleg talált megoldást
                if (_eightQueen.Where(p => p.Parent == grWaiterZone).Count() > 0)                
                    MessageBox.Show("A megadott kezdőhellyel az algoritmus sajnos nem talált megoldást!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else                    
                    MessageBox.Show("Az algoritmus sikeres megoldást talált!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);                
                //Mind a két esetben engedélyezzük a tábla törlés gombot
                btnRemoveTable.IsEnabled = true;
                //befelyezzük a metódust
                return;
            }
            //Feltöltjük a lépésnek megfelelő táblát a _table-be
            _table = _queen_alg.GetSteps()[_step];            
            //Lekapjuk a királynőket a tábláról
            RemoveQueenFromChessTable();
            //Átkonvertáluk a Grid-re a tábla tartalmát
            ConvertTable(_table);
            //Növeljük a lépést így a következő tick-en már azt olvassa
            _step++;
        }

        //Az összes királynő levétele a tábláról
        private void RemoveQueenFromChessTable()
        {
            //végig megy azokon a királynőkön amelyeknek a szülő objektuma a grChessTable
            foreach (var item in _eightQueen.Where(p => p.Parent == grChessTable))
            {
                //Levétel a tábláról
                grChessTable.Children.Remove(item);
                //Waiter zónába rakja
                grWaiterZone.Children.Add(item);
            }
        }
        //Tábla konvertálása a Gridre
        private void ConvertTable(int[,] table)
        {
            //Végig megy minden soron és oszlopon ha az érték 1 akkor leveszi a királynőt a waiter zónából és kirakja a chess table gridre
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (table[i, j] == 1)
                    {
                        Image q = _eightQueen.Where(p => p.Parent == grWaiterZone).First();
                        grWaiterZone.Children.Remove(q);
                        grChessTable.Children.Add(q);
                        Grid.SetRow(q, i);
                        Grid.SetColumn(q, j);
                    }                                                                                          
        }
        //Start gomb
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //A kezdő pozicióhoz egy két elemes int tömb
            int[] pos = new int[2];
            //a tbFirstQueenPosition-ben megafott Betű - Szám párost átkovertája indexekké ha 9 9 a tömb tartalma akkor érvénytelen a pozició
            pos = QueenAlgorythm.GetNumPosition(tbFirstQueenPosition.Text);
            //HA üres a tbFirstQueenPosition akkor egy random pozicióval kezd
            if (tbFirstQueenPosition.Text == string.Empty)
            {
                Random rnd = new Random();
                pos[0] = rnd.Next(0, 7);
                pos[1] = rnd.Next(0, 7);
                MessageBox.Show($"Nem adott meg kezdő helyet!\nRandom hely: {QueenAlgorythm.GetAlpPostion(pos)}","Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {   //Ha van benne érték akkor valid ha nem 99  különben vége a metódusnak          
                bool isValid = pos[0] != 99 && pos[1] != 99;
                if (!isValid) 
                {
                    MessageBox.Show($"Érvénytelen helyet adott meg!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            //LEtiltjuk a kezdő pozició bevíteli mezőjét
            tbFirstQueenPosition.IsEnabled = false;
            //letiltjuk a start gombot
            btnStart.IsEnabled = false;
            //példányosítjuk az algoritmust, a kezdő pozicióval
            _queen_alg = new QueenAlgorythm(pos[0], pos[1]);
            //Meghívjuk a Solve metódust az elkészíti az összes lépést
            _queen_alg.Solve();
            //beállítjuk az időzítő sebességét
            _timer.Interval = TimeSpan.FromMilliseconds(1000 - sSpeed.Value);
            //elindítjuk az időzítőt
            _timer.Start();

        }

        private void btnRemoveTable_Click(object sender, RoutedEventArgs e)
        {
            //Levesszük az összes királynőt a tábláróé
            RemoveQueenFromChessTable();
            //A lépéseket nullázuk
            _step = 0;
            //A példányt nullázuk
            _queen_alg = null;
            //Üresre tesszüka kezdő poziciót
            tbFirstQueenPosition.Text = "";
            //Engedélyezzük a bevíteli mezőt
            tbFirstQueenPosition.IsEnabled = true;
            //Engedélyezzük a startgombot
            btnStart.IsEnabled = true;
            //letiltjuk tábla törlése gombot
            btnRemoveTable.IsEnabled = false;

        }
    }
}
