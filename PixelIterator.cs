using System;
using System.Drawing;

namespace CommonGraphics
{
    public enum TraversalOrder
    {
        LeftToRight,  // current: x first, then y
        TopToBottom   // new: y first, then x
    }

    public class PixelIterator
    {
        private Point _start;
        private Point _end;
        private Point _cursor;
        private bool _valid;

        private TraversalOrder _order;

        public Point Cursor { get { return _cursor; } }

        public TraversalOrder Order { get { return _order; } set { _order = value; } }

        public PixelIterator()
        {
            _start = new Point();
            _end = new Point();
            _cursor = _start;
            _valid = false;
            _order = TraversalOrder.TopToBottom;
        }

        public PixelIterator(Point start, Point end, TraversalOrder order = TraversalOrder.TopToBottom)
        {
            _start = new Point(
                start.X < end.X ? start.X : end.X,
                start.Y < end.Y ? start.Y : end.Y
            );

            _end = new Point(
                start.X < end.X ? end.X - 1 : start.X,
                start.Y < end.Y ? end.Y - 1 : start.Y
            );

            _cursor = _start;
            _valid = true;
            _order = order;
        }

        public void Reset()
        {
            _cursor = _start;
        }

        public void Reset(Point newStart, Point newEnd)
        {
            _start = new Point(
                newStart.X < newEnd.X ? newStart.X : newEnd.X,
                newStart.Y < newEnd.Y ? newStart.Y : newEnd.Y
            );

            _end = new Point(
                newStart.X < newEnd.X ? newEnd.X - 1 : newStart.X,
                newStart.Y < newEnd.Y ? newEnd.Y - 1 : newStart.Y
            );

            _cursor = _start;
            _valid = true;
        }

        public void Step()
        {
            if (_cursor.Equals(_end))
                return;

            switch (_order)
            {
                case TraversalOrder.LeftToRight:
                    if (_cursor.Y < _end.Y)
                    {
                        _cursor.Y++;
                    }
                    else
                    {
                        if (_cursor.X < _end.X)
                        {
                            _cursor.Y = _start.Y;
                            _cursor.X++;
                        }
                    }
                    break;

                case TraversalOrder.TopToBottom:
                    if (_cursor.X < _end.X)
                    {
                        _cursor.X++;
                    }
                    else
                    {
                        if (_cursor.Y < _end.Y)
                        {
                            _cursor.X = _start.X;
                            _cursor.Y++;
                        }
                    }
                    break;
            }
        }

        public bool Done()
        {
            return _cursor.Equals(_end);
        }
    }
}
