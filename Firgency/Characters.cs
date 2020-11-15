using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Firgency
{
    public enum Campaign { Fire, Water }

    public abstract class Character
    {
        private int _blood;
        private int _column;

        public string Name { get; set; }
        public Campaign Campaign { get; }
        public int Range { get; protected set; }
        public int Row { get; set; }
        public int AttackNumber { get; set; }
        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                if (Column == 0 && Campaign == Campaign.Fire) { MainWindow.WaterLeft -= 1; MainWindow.Characters.Remove(this); }
            }
        }
        public Personality Personality { get; set; }
        public int Blood
        {
            get => _blood;
            set
            {
                _blood = value;
                if (Blood == 0)
                {
                    MainWindow.Characters.Remove(this);
                    if (Campaign == Campaign.Fire) MainWindow.FireLeft -= 1;
                }
            }
        }

        public Character(Campaign campaign, int row, int column, Personality personality)
        {
            Campaign = campaign;
            Row = row;
            Column = column;
            Personality = personality;
        }

        public Character(Campaign campaign)
        {
            Campaign = campaign;
            Personality = new None();
            Row = -1;
            Column = -1;
        }

        private bool AttackRangeCheck(Character character)
        {
            if (Math.Abs(character.Row - Row) + Math.Abs(Column - character.Column) > Range)
            {
                MessageBox.Show("超出攻击范围！");
                return false;
            }
            return true;
        }

        private bool PersonalityNoneCheck()
        {
            if (Personality is None)
            {
                MessageBox.Show("没有装配特性！");
                return false;
            }
            return true;
        }

        private bool ActionNumberEnough()
        {
            if (MainWindow.ActionNumber < 1)
            {
                MessageBox.Show("行动力不足！");
                return false;
            }
            return true;
        }

        private bool AttackNumberEnough()
        {
            if (AttackNumber <= 0)
            {
                MessageBox.Show("该角色攻击次数已用完！");
                return false;
            }
            return true;
        }

        protected abstract void AttackOperation(Character character);

        public void Attack(Character character)
        {
            if (AttackRangeCheck(character) && PersonalityNoneCheck() && ActionNumberEnough() && AttackNumberEnough())
            {
                if (Personality is Water && (character.Personality is Electricity || character.Personality is Oil || character.Personality is Metal))
                {
                    character.Personality.Trigger(character.Row, character.Column);
                }
                else if (Personality is CO2 && character.Personality is Metal)
                {
                    character.Personality.Trigger(character.Row, character.Column);
                }
                else
                {
                    AttackOperation(character);
                }
                MainWindow.UpdateMap();
                MainWindow.ActionNumber -= 1;
                AttackNumber -= 1;
            }
        }

        public void Move(int targetColumn, int targetRow)
        {
            // 找到当前位置到目标位置的最短路径，判断是否够，可以的话移动并扣除行动力，否则弹出行动力不足 或 无法移动的弹窗。
            // 不可以走的格子：所有有人的格子
            // 除此之外都是可以走的
            // 范围：c [0, 7]
            // r [0, 3]
            // 目标位置和现在位置肯定不相同
            // 思路：BFS
            int distance = -1;

            Queue<PointForBFS> points = new Queue<PointForBFS>();
            points.Enqueue(new PointForBFS(new Point(Column, Row), 0));
            bool[,] visited = new bool[8, 4];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    visited[i, j] = false;
                }
            }
            visited[Column, Row] = true;

            while (points.Count != 0)
            {
                PointForBFS point = points.Peek();
                if (point.Point.Row == targetRow && point.Point.Column == targetColumn)
                {
                    distance = point.Depth;
                    break;
                }
                if (point.Point.Row - 1 >= 0 && MainWindow.Characters.Find(r => r.Row == point.Point.Row - 1 && point.Point.Column == r.Column) == null && !visited[point.Point.Column, point.Point.Row - 1])
                {
                    points.Enqueue(new PointForBFS(new Point(point.Point.Column, point.Point.Row - 1), point.Depth + 1));
                    visited[point.Point.Column, point.Point.Row - 1] = true;
                }
                if (point.Point.Row + 1 <= 3 && MainWindow.Characters.Find(r => r.Row == point.Point.Row + 1 && point.Point.Column == r.Column) == null && !visited[point.Point.Column, point.Point.Row + 1])
                {
                    points.Enqueue(new PointForBFS(new Point(point.Point.Column, point.Point.Row + 1), point.Depth + 1));
                    visited[point.Point.Column, point.Point.Row + 1] = true;
                }
                if (point.Point.Column - 1 >= 0 && MainWindow.Characters.Find(r => r.Row == point.Point.Row && point.Point.Column - 1 == r.Column) == null && !visited[point.Point.Column - 1, point.Point.Row])
                {
                    points.Enqueue(new PointForBFS(new Point(point.Point.Column - 1, point.Point.Row), point.Depth + 1));
                    visited[point.Point.Column - 1, point.Point.Row] = true;
                }
                if (point.Point.Column + 1 <= 7 && MainWindow.Characters.Find(r => r.Row == point.Point.Row && point.Point.Column + 1 == r.Column) == null && !visited[point.Point.Column + 1, point.Point.Row])
                {
                    points.Enqueue(new PointForBFS(new Point(point.Point.Column + 1, point.Point.Row), point.Depth + 1));
                    visited[point.Point.Column + 1, point.Point.Row] = true;
                }
                points.Dequeue();
            }
            if (distance == -1) MessageBox.Show("该角色无法移动！");
            else
            {
                if (distance <= MainWindow.ActionNumber)
                {
                    MainWindow.ActionNumber -= distance;
                    Column = targetColumn;
                    Row = targetRow;
                }
                else MessageBox.Show("行动力不足！");
            }
        }
    }

    public class Short : Character
    {
        public Short(Campaign campaign, int row, int column, Personality personality) : base(campaign, row, column, personality)
        {
            if (campaign == Campaign.Fire) Name = "剑客";
            if (campaign == Campaign.Water) Name = "救援人员";
            Range = 1;
            Blood = 1;
            AttackNumber = 1;
        }

        public Short(Campaign campaign) : base(campaign)
        {
            if (campaign == Campaign.Fire) Name = "剑客";
            if (campaign == Campaign.Water) Name = "救援人员";
            Range = 1;
            Blood = 1;
            AttackNumber = 1;
        }

        protected override void AttackOperation(Character character)
        {
            if (MainWindow.Characters.Find(r=>r.Row.Equals(Row)&&r.Column.Equals(Column)) != null)
            {
                character.Blood -= 1;
                if (MainWindow.Characters.Find(r=>r.Row.Equals(character.Row)&&r.Column.Equals(character.Column)) == null)
                {
                    this.Row = character.Row;
                    this.Column = character.Column;
                }
            }
        }
    }

    public class Long : Character
    {
        public Long(Campaign campaign, int row, int column, Personality personality) : base(campaign, row, column, personality)
        {
            if (campaign == Campaign.Fire) Name = "弓箭手";
            if (campaign == Campaign.Water) Name = "远程支援";
            Range = 3;
            Blood = 1;
            AttackNumber = 1;
        }

        public Long(Campaign campaign) : base(campaign)
        {
            if (campaign == Campaign.Fire) Name = "弓箭手";
            if (campaign == Campaign.Water) Name = "远程支援";
            Range = 3;
            Blood = 1;
            AttackNumber = 1;
        }

        protected override void AttackOperation(Character character)
        {
            if (MainWindow.Characters.Find(r=>r.Row.Equals(Row)&&r.Column.Equals(Column)) != null)
            {
                character.Blood -= 1;
            }
        }
    }

    public class Group : Character
    {
        public Group(Campaign campaign, int row, int column, Personality personality) : base(campaign, row, column, personality)
        {
            if (campaign == Campaign.Fire) Name = "龙";
            if (campaign == Campaign.Water) Name = "水炮";
            Range = 2;
            Blood = 1;
            AttackNumber = 2;
        }

        public Group(Campaign campaign) : base(campaign)
        {
            if (campaign == Campaign.Fire) Name = "龙";
            if (campaign == Campaign.Water) Name = "水炮";
            Range = 2;
            Blood = 1;
            AttackNumber = 2;
        }

        protected override void AttackOperation(Character character)
        {
            if (MainWindow.Characters.Find(r => r.Row.Equals(Row) && r.Column.Equals(Column)) != null)
            {
                character.Blood -= 1;
            }
        }
    }

    public class Shield : Character
    {
        public Shield(Campaign campaign, int row, int column, Personality personality) : base(campaign, row, column, personality)
        {
            if (campaign == Campaign.Fire) Name = "盾兵";
            if (campaign == Campaign.Water) Name = "防火罩";
            Range = 0;
            Blood = 2;
            AttackNumber = 0;
        }

        public Shield(Campaign campaign) : base(campaign)
        {
            if (campaign == Campaign.Fire) Name = "盾兵";
            if (campaign == Campaign.Water) Name = "防火罩";
            Range = 0;
            Blood = 2;
            AttackNumber = 0;
        }

        public new void Attack(Character character)
        {
            MessageBox.Show($"{Name}不能进行攻击！");
        }

        protected override void AttackOperation(Character character) { }
    }
}
