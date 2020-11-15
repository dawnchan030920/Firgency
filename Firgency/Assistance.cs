using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firgency
{
    public struct Point
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public Point(int column, int row)
        {
            Column = column;
            Row = row;
        }
    }

    public struct PointForBFS
    {
        public Point Point { get; set; }
        public int Depth { get; set; }

        public PointForBFS(Point point, int depth)
        {
            Point = point;
            Depth = depth;
        }
    }
}
