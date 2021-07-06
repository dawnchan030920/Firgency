using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firgency
{
    public abstract class Personality
    {
        public string Name { get; protected set; }
        protected abstract void OnTrigger(int row, int column);

        public void Trigger(int row, int column)
        {
            OnTrigger(row, column);
            MainWindow.UpdateMap();
        }
    }

    public class None : Personality
    {
        protected override void OnTrigger(int row, int column) { }
        public None() { Name = "无"; }
    }

    public class Fire : Personality
    {
        protected override void OnTrigger(int row, int column) { }
        public Fire() { Name = "火"; }
    }

    public class Oil : Personality
    {
        protected override void OnTrigger(int row, int column)
        {
            Random random = new Random();
            int result = random.Next(0, 4);

            if (result == 0) // 上
            {
                // 如果上方没有越界
                if (row - 1 >= 0)
                {
                    // 如果上方位置有消防阵营的棋子
                    if ((MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row - 1)) != null)
                        && MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row - 1)).Campaign == Campaign.Water)
                    {
                        // 攻击
                        MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row - 1)).Blood -= 1;

                        // 如果攻击死亡
                        if (MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row - 1)) == null)
                        {
                            MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row)).Row -= 1; // 占领
                        }
                    }
                    // 如果没有
                    else if (MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row - 1)) == null) MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Row -= 1;
                }
            }
            else if (result == 1) // 下
            {
                if (row + 1 <= 3)
                {
                    // 如果上方位置有消防阵营的棋子
                    if ((MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row + 1)) != null)
                        && MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row + 1)).Campaign == Campaign.Water)
                    {
                        // 攻击
                        MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row + 1)).Blood -= 1;

                        // 如果攻击死亡
                        if (MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row + 1)) == null)
                        {
                            MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row)).Row += 1; // 占领
                        }
                    }
                    // 如果没有
                    else if (MainWindow.Characters.Find(r => r.Column.Equals(column)
                        && r.Row.Equals(row + 1)) == null) MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Row += 1;
                }
            }
            else if (result == 2) // 左
            {
                if (column - 1 >= 0)
                {
                    // 如果左方位置有消防阵营的棋子
                    if ((MainWindow.Characters.Find(r => r.Column.Equals(column - 1)
                        && r.Row.Equals(row)) != null)
                        && MainWindow.Characters.Find(r => r.Column.Equals(column - 1)
                        && r.Row.Equals(row)).Campaign == Campaign.Water)
                    {
                        // 攻击
                        MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)).Blood -= 1;

                        // 如果攻击死亡
                        if (MainWindow.Characters.Find(r => r.Column.Equals(column - 1) && r.Row.Equals(row)) == null)
                        {
                            MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row)).Column -= 1; // 占领
                        }
                    }
                    // 如果没有
                    else if (MainWindow.Characters.Find(r => r.Column.Equals(column - 1)
                        && r.Row.Equals(row)) == null) MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Column -= 1;
                }
            }
            else if (result == 3) // 右
            {
                if (column + 1 <= 7)
                {
                    // 如果右方位置有消防阵营的棋子
                    if ((MainWindow.Characters.Find(r => r.Column.Equals(column + 1)
                        && r.Row.Equals(row)) != null)
                        && MainWindow.Characters.Find(r => r.Column.Equals(column + 1)
                        && r.Row.Equals(row)).Campaign == Campaign.Water)
                    {
                        // 攻击
                        MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Blood -= 1;

                        // 如果攻击死亡
                        if (MainWindow.Characters.Find(r => r.Column.Equals(column + 1) && r.Row.Equals(row)) == null)
                        {
                            MainWindow.Characters.Find(r => r.Column.Equals(column) && r.Row.Equals(row)).Column += 1; // 占领
                        }
                    }
                    // 如果没有
                    else if (MainWindow.Characters.Find(r => r.Column.Equals(column + 1)
                        && r.Row.Equals(row)) == null) MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Column += 1;
                }
            }
        }
        public Oil() { Name = "油"; }
    }

    public class Electricity : Personality
    {
        protected override void OnTrigger(int row, int column)
        {
            if (row - 1 >= 0
                && MainWindow.Characters.Find(r => r.Row.Equals(row - 1) && r.Column.Equals(column)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row - 1) && r.Column.Equals(column)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row - 1) && r.Column.Equals(column)).Blood -= 1;
            }

            if (row + 1 <= 3
                && MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)).Blood -= 1;
            }

            if (column + 1 <= 7
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Blood -= 1;
            }

            if (column - 1 >= 0
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)).Blood -= 1;
            }
        }
        public Electricity() { Name = "电"; }
    }

    public class Metal : Personality
    {
        protected override void OnTrigger(int row, int column)
        {
            MainWindow.ActionCompensation = true;
        }
        public Metal() { Name = "金属"; }
    }

    public class Water : Personality
    {
        protected override void OnTrigger(int row, int column) { }
        public Water() { Name = "水"; }
    }

    public class CO2 : Personality
    {
        protected override void OnTrigger(int row, int column) { }
        public CO2() { Name = "CO2"; }
    }

    public class Powder: Personality
    {
        protected override void OnTrigger(int row, int column) { }
        public Powder() { Name = "干粉"; }
    }
}
