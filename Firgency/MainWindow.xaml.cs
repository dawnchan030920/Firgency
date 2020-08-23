using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.ComponentModel;

namespace Firgency
{
    enum CurrentType
    {
        CharacterChosen,
        PersonalityChosen,
        None,
        Character
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler ActionNumberChanged;
        public static event EventHandler FireLeftChanged;
        public static event EventHandler WaterLeftChanged;

        public void UpdateCampaign()
        {
            if (campaign == Firgency.Campaign.Fire)
            {
                Campaign.Text = "火灾阵营";
                Campaign.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (campaign == Firgency.Campaign.Water)
            {
                Campaign.Text = "消防阵营";
                Campaign.Foreground = new SolidColorBrush(Colors.Blue);
            }
        }

        const int waterActionNumber = 4;
        const int fireActionNumber = 3;
        const int compensation = 1;
        const int waterLeftDefault = 5;

        private CurrentType current;

        public static bool ActionCompensation;

        static TextBlock[,] blocks = new TextBlock[4, 8];

        public static List<Character> Characters;

        private static int _actionNumber;
        public static int ActionNumber
        {
            get => _actionNumber;
            set
            {
                _actionNumber = value;
                ActionNumberChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static Campaign campaign { get; set; }

        public Character characterChosen;
        public Personality personalityChosen;
        public Character character;

        private static int _fireLeft;
        public static int FireLeft
        {
            get => _fireLeft;
            set
            {
                _fireLeft = value;
                FireLeftChanged?.Invoke(null, EventArgs.Empty);
                if (FireLeft <= 0) MessageBox.Show("消防阵营胜利！");
            }
        }

        private static int _waterLeft;
        public static int WaterLeft
        {
            get => _waterLeft;
            set
            {
                _waterLeft = value;
                WaterLeftChanged?.Invoke(null, EventArgs.Empty);
                if (WaterLeft <= 0) MessageBox.Show("火灾阵营胜利！");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) // 初始化
        {
            InitMap();
            Characters = new List<Character>();
            campaign = Firgency.Campaign.Fire;
            UpdateCampaign();
            ActionNumber = fireActionNumber;
            ActionCompensation = false;
            WaterLeft = waterLeftDefault;
        }

        public void InitMap() // 初始化地图
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    blocks[i, j] = new TextBlock();
                    Map.Children.Add(blocks[i, j]);
                    blocks[i, j].SetValue(Grid.RowProperty, i);
                    blocks[i, j].SetValue(Grid.ColumnProperty, j);
                    blocks[i, j].MouseLeftButtonDown += Grid_Click;
                }
            }
        }

        private void GetCharacter_Click(object sender, RoutedEventArgs e) // 抽取角色
        {
            if (ActionNumber < 1) { MessageBox.Show("行动力不足！"); return; }

            Random random = new Random();
            int result = random.Next(0, 50);
            if (result < 20) characterChosen = new Short(campaign);
            else if (result < 35) characterChosen = new Long(campaign);
            else if (result < 45) characterChosen = new Group(campaign);
            else characterChosen = new Group(campaign);

            ActionNumber -= 1;

            CharacterGet.Text = characterChosen.Name;
        }

        private void GetPersonality_Click(object sender, RoutedEventArgs e) // 抽取特性
        {
            if (ActionNumber < 2) { MessageBox.Show("行动力不足！"); return; }

            Random random = new Random();
            int result = random.Next(0, 25);

            if (campaign == Firgency.Campaign.Fire)
            {
                if (result < 10) personalityChosen = new Fire();
                else if (result < 15) personalityChosen = new Electricity();
                else if (result < 20) personalityChosen = new Metal();
                else if (result < 25) personalityChosen = new Oil();
            }
            else if (campaign == Firgency.Campaign.Water)
            {
                if (result < 15) personalityChosen = new Water();
                else if (result < 23) personalityChosen = new CO2();
                else if (result < 25) personalityChosen = new Powder();
            }

            ActionNumber -= 2;

            PersonalityGet.Text = personalityChosen.Name;
        }

        private void NextRound_Click(object sender, RoutedEventArgs e) // 下一回合
        {
            // 更改阵营
            if (campaign == Firgency.Campaign.Fire) campaign = Firgency.Campaign.Water;
            else if (campaign == Firgency.Campaign.Water) campaign = Firgency.Campaign.Fire;
            UpdateCampaign();

            // 更换行动力点数
            if (campaign == Firgency.Campaign.Fire)
            {
                if (ActionCompensation) { ActionNumber = fireActionNumber + compensation; ActionCompensation = false; }
                else ActionNumber = fireActionNumber;
            }
            else if (campaign == Firgency.Campaign.Water)
            {
                ActionNumber = waterActionNumber;
            }

            // 刷新地图
            UpdateMap();

            // 刷新角色获取
            CharacterGet.Text = "获取角色";
            PersonalityGet.Text = "获取特性";

            // 刷新攻击次数
            foreach (Character group in Characters.FindAll(r => r is Group)) group.AttackNumber = 2;
            foreach (Character other in Characters.FindAll(r => !(r is Group))) other.AttackNumber = 1;

            // 选择清零
            characterChosen = null;
            character = null;
            personalityChosen = null;
            current = CurrentType.None;
        }

        private void Grid_Click(object sender, RoutedEventArgs e) // TODO: 选中地图中的一格
        {
            // 获取sender的行和列
            TextBlock block = sender as TextBlock;
            int row = (int)block.GetValue(Grid.RowProperty);
            int column = (int)block.GetValue(Grid.ColumnProperty);

            // 单击为空
            if (Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)) == null)
            {
                // 原来选中抽取角色
                if (current == CurrentType.CharacterChosen)
                {
                    // 是否在可以放置范围内
                    if ((characterChosen.Campaign == Firgency.Campaign.Fire && column >= 4 && column <= 7) ||
                        (characterChosen.Campaign == Firgency.Campaign.Water && column >= 0 && column <= 3))
                    {
                        characterChosen.Column = column;
                        characterChosen.Row = row;
                        Characters.Add(characterChosen);
                        if (characterChosen.Campaign == Firgency.Campaign.Fire) FireLeft += 1;
                        characterChosen = null;
                        current = CurrentType.None;

                        CharacterGet.Text = "获取角色";
                        UpdateMap();
                    }
                    else MessageBox.Show("此处无法放置！");
                }
                // 原来选中抽取特性
                else if (current == CurrentType.PersonalityChosen)
                {
                    MessageBox.Show("无法将特性放置在空格上！");
                }
                // 原来选中角色
                else if (current == CurrentType.Character)
                {
                    // 是否斜着走
                    if (character.Column - column != 0 && character.Row - row != 0) MessageBox.Show("不能斜着走！");
                    else
                    {
                        // 行动力是否足够
                        if (ActionNumber >= Math.Abs(character.Column - column) + Math.Abs(character.Row - row))
                        {
                            ActionNumber -= Math.Abs(character.Column - column) + Math.Abs(character.Row - row);

                            character.Row = row;
                            character.Column = column;

                            UpdateMap();
                        }
                        else MessageBox.Show("行动力不足！");
                    }
                }
                // 原来什么都没选中
            }
            // 单击选中角色
            else
            {
                Character characterNow = Characters.Find(r => r.Row == row && r.Column == column);
                // 相同阵营
                if (characterNow.Campaign == campaign)
                {
                    // 选中特性
                    if (current == CurrentType.PersonalityChosen)
                    {
                        characterNow.Personality = personalityChosen;
                        current = CurrentType.None;

                        PersonalityGet.Text = "特性获取";
                        UpdateMap();
                    }
                    // 否则就选中这个角色
                    else
                    {
                        current = CurrentType.Character;
                        character = characterNow;
                    }
                }
                // 不同阵营
                else
                {
                    // 选中抽取特性
                    if (current == CurrentType.PersonalityChosen)
                    {
                        MessageBox.Show("我不觉得你可以把特性附到对方角色上！");
                    }
                    // 选中抽取角色
                    else if (current == CurrentType.CharacterChosen)
                    {
                        MessageBox.Show("你并不可以碾在对手身上！");
                    }
                    // 选中角色
                    else if (current == CurrentType.Character)
                    {
                        character.Attack(characterNow);
                    }
                    else if (current == CurrentType.None)
                    {
                        MessageBox.Show("你并不可以选中对手！");
                    }
                }
            }
        }

        private void CharacterGet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // 选中抽取的角色
        {
            if (characterChosen == null) current = CurrentType.None;
            else current = CurrentType.CharacterChosen;
        }

        private void PersonalityGet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) // 选中抽取的特性
        {
            if (personalityChosen == null) current = CurrentType.None;
            else current = CurrentType.PersonalityChosen;
        }

        public static void UpdateMap()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    blocks[i, j].Text = string.Empty;
                }
            }
            foreach (Character character in Characters)
            {
                if (campaign == Firgency.Campaign.Fire)
                {
                    if (character.Campaign == Firgency.Campaign.Fire) blocks[character.Row, character.Column].Text = $"{character.Name}\n{character.Personality.Name}";
                    else if (character.Campaign == Firgency.Campaign.Water) blocks[character.Row, character.Column].Text = $"{character.Name}";
                }
                if (campaign == Firgency.Campaign.Water)
                {
                    if (character.Campaign == Firgency.Campaign.Water) blocks[character.Row, character.Column].Text = $"{character.Name}\n{character.Personality.Name}";
                    else if (character.Campaign == Firgency.Campaign.Fire) blocks[character.Row, character.Column].Text = $"{character.Name}";
                }
            }
        }
    }
}
