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
            if (row - 1 >= 0
                && MainWindow.Characters.Find(r => r.Row.Equals(row - 1) && r.Column.Equals(column)) == null)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Row -= 1;
                MainWindow.Characters.Find(r => r.Row.Equals(row - 1) && r.Column.Equals(column)).Blood += 1;
            }

            else if (row + 1 <= 3
                && MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)) == null)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Row += 1;
                MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)).Blood += 1;
            }

            else if (column - 1 >= 0
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)) == null)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Column -= 1;
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column - 1)).Blood += 1;
            }

            else if (column + 1 <= 7
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)) == null)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Column += 1;
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Blood += 1;
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

            else if (row + 1 <= 3
                && MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row + 1) && r.Column.Equals(column)).Blood -= 1;
            }

            else if (column + 1 <= 7
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)) != null
                && MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Campaign == Campaign.Water)
            {
                MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column + 1)).Blood -= 1;
            }

            else if (column - 1 >= 0
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
            MainWindow.Characters.Find(r => r.Row.Equals(row) && r.Column.Equals(column)).Blood += 1;
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
