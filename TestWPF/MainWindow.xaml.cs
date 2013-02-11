using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Linq;
using Microsoft.Win32;

namespace TestWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly ICommand OpenCommand = new RoutedCommand("OpenCommand", typeof(MainWindow));

        private List<DataClass> data;
        private ComboBox[] answerComboBox;
        private int currentPage = 0;
        private int totalPage = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void createAnswerBox(int page)
        {
            //ComboBox[] answerComboBox;

            //初期化処理
            text.Text = "";
            answerGrid.Children.Clear();

            //グリッドの整形
            answerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            answerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            //ラベル
            var lb = new Label() { Content = "＜解答＞" };
            lb.SetValue(Grid.RowProperty, 0);
            lb.SetValue(Grid.ColumnProperty, 0);
            answerGrid.Children.Add(lb);
            
            for (int i = 0; i < data[page].Num; i++)
            {
                answerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            }

            //コンボボックスの生成
            answerComboBox = new ComboBox[data[page].Num];
            for (int i = 0; i < data[page].Num; i++)
            {
                //ラベル
                var indexLb = new Label() { Content = "(" + (i + 1) + ")" };
                indexLb.SetValue(Grid.RowProperty, i + 1);
                indexLb.SetValue(Grid.ColumnProperty, 0);
                answerGrid.Children.Add(indexLb);

                //ComboBox
                answerComboBox[i] = new ComboBox();
                answerComboBox[i].VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                answerComboBox[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                answerComboBox[i].SetValue(Grid.RowProperty, i + 1);
                answerComboBox[i].SetValue(Grid.ColumnProperty, 1);

                answerGrid.Children.Add(answerComboBox[i]);
            }

            //採点ボタン
            answerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            Button mkButton = new Button() { Content = "採点" }; 
            mkButton.SetValue(Grid.RowProperty, data[page].Num + 1);
            mkButton.SetValue(Grid.ColumnProperty, 0);
            answerGrid.Children.Add(mkButton);
            mkButton.Click += new RoutedEventHandler(mkButton_Click);

            //次の問題ボタン
            answerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            Button nextButton = new Button() { Content = "次へ" };
            nextButton.SetValue(Grid.RowProperty, data[page].Num + 1);
            nextButton.SetValue(Grid.ColumnProperty, 1);
            answerGrid.Children.Add(nextButton);
            nextButton.Click += new RoutedEventHandler(nextButton_Click);

            //問題の表示
            text.Text += "＜問題＞\n" + data[page].Qs + "\n\n" + "＜回答群＞\n";
            for (int i = 0; i < data[page].getAnserDataClassSize(); i++)
            {
                string s = data[page].Get(i).Mark + ":" + data[page].Get(i).Text;

                text.Text += s + '\t';

                for (int j = 0; j < data[page].Num; j++)
                {
                    answerComboBox[j].Items.Add(s);
                }
            }

            //初期値
            for (int i = 0; i < data[page].Num; i++)
            {
                answerComboBox[i].Text = answerComboBox[i].Items[0].ToString();
            }
        }

        //次の問題
        void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage + 1 < totalPage)
            {
                createAnswerBox(++currentPage);
            }
        }

        void mkButton_Click(object sender, RoutedEventArgs e)
        {
            bool flg = true;
            string[] r = data[currentPage].Right.Split(',');

            for (int i = 0; i < data[currentPage].Num && flg; i++)
            {
                if (answerComboBox[i].Text.Split(':')[0] != r[i]) flg = false;
            }

            if (flg)
            {
                MessageBox.Show("全問正解！");
            }
            else
            {
                MessageBox.Show("残念...。");
            }
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            data = new List<DataClass>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "";

            if (ofd.ShowDialog() == true)
            {
                int i = 0;
                XDocument xmlDoc = XDocument.Load(ofd.FileName);

                var t = from pg in xmlDoc.Descendants("Political")
                        select (string)pg.Element("Page");
                foreach (var p in t)
                {
                    totalPage = Int32.Parse(p);
                }

                var q = from c in xmlDoc.Descendants("Items")
                        select new
                        {
                            qs = (string)c.Element("Question").Value,
                            num = Int32.Parse(c.Element("Count").Value),
                            a = c.Element("Answer"),
                            r = (string)c.Element("Answer").Element("Right")
                        };
                        
                foreach (var obj in q)
                {
                    DataClass insData = new DataClass();

                    //変数挿入
                    insData.Qs = obj.qs;
                    insData.Id = i++;
                    insData.Num = obj.num;
                    insData.Right = obj.r;

                    var ans = from ext in obj.a.Descendants("Item")
                            select new
                            {
                                mark = (string)ext.Element("Mark").Value,
                                data = (string)ext.Element("Data").Value
                            };
                    foreach (var o in ans)
                    {
                        insData.Add(o.mark, o.data);
                    }

                    data.Add(insData);
                }
            }
            createAnswerBox(0);
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
