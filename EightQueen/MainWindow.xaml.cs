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

        private int[,] _table;
        private int _step = 0;        

        private List<Image> _eightQueen;

        private QueenAlgorythm _queen_alg;
        private BitmapImage _queen_bitmap = new BitmapImage(new Uri("q1.png", UriKind.RelativeOrAbsolute));
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();

            _eightQueen = new List<Image>();
            for (int i = 0; i < 8; i++)
            {
                
                Image img = new Image();
                img.Width = 80;
                img.Height = 80;
                img.Source = _queen_bitmap;
                img.Name = "q"+ i;
                _eightQueen.Add(img);
                grWaiterZone.Children.Add(img);                
            }
            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;

        }             
        void timer_Tick(object sender, EventArgs e)
        {

            _timer.Interval = TimeSpan.FromMilliseconds(1000 - sSpeed.Value);
            
            if (_step >= _queen_alg.GetSteps().Count())
            {
                _timer.Stop();
                if (_eightQueen.Where(p => p.Parent == grWaiterZone).Count() > 0)                
                    MessageBox.Show("A megadott kezdőhellyel az algoritmus sajnos nem talált megoldást!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Az algoritmus sikeres megoldást talált!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);                
                btnRemoveTable.IsEnabled = true;
                return;
            }
            
            _table = _queen_alg.GetSteps()[_step];            
            RemoveQueenFromChessTable();
            ConvertTable(_table);
            _step++;
        }
        private void AddQueenToChessTable(int row, int column)
        {
            Image q = _eightQueen.Where(p => p.Parent == grWaiterZone).First();
            grWaiterZone.Children.Remove(q);
            grChessTable.Children.Add(q);
            Grid.SetRow(q, row);
            Grid.SetColumn(q, column);
        }

        private void RemoveQueenFromChessTable()
        {
            foreach (var item in _eightQueen.Where(p => p.Parent == grChessTable))
            {
                item.Source = _queen_bitmap;
                grChessTable.Children.Remove(item);
                grWaiterZone.Children.Add(item);
            }
        }

        private void ConvertTable(int[,] table)
        {
            for (int i = 0; i < 8; i++)            
                for (int j = 0; j < 8; j++)
                    if (table[i, j] == 1)                                            
                        AddQueenToChessTable(i, j);                                                                        
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            int[] pos = new int[2];
            pos = QueenAlgorythm.GetNumPosition(tbFirstQueenPosition.Text);
            
            if (tbFirstQueenPosition.Text == string.Empty)
            {
                Random rnd = new Random();
                pos[0] = rnd.Next(0, 7);
                pos[1] = rnd.Next(0, 7);
                MessageBox.Show($"Nem adott meg kezdő helyet!\nRandom hely: {QueenAlgorythm.GetAlpPostion(pos)}","Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {                
                bool isValid = pos[0] != 99 && pos[1] != 99;
                if (!isValid) 
                {
                    MessageBox.Show($"Érvénytelen helyet adott meg!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            tbFirstQueenPosition.IsEnabled = false;
            btnStart.IsEnabled = false;

            _queen_alg = new QueenAlgorythm(pos[0], pos[1]);
            _queen_alg.Solve();
            _timer.Interval = TimeSpan.FromMilliseconds(1000 - sSpeed.Value);
            _timer.Start();

        }

        private void btnRemoveTable_Click(object sender, RoutedEventArgs e)
        {
            RemoveQueenFromChessTable();
            _step = 0;
            _queen_alg = null;
            tbFirstQueenPosition.Text = "";
            tbFirstQueenPosition.IsEnabled = true;
            btnStart.IsEnabled = true;
            btnRemoveTable.IsEnabled = false;

        }
    }
}
