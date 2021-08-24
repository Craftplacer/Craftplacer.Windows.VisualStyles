using WfPadding = System.Windows.Forms.Padding;
using WpfPadding = System.Windows.Thickness;

namespace Craftplacer.Windows.VisualStyles
{
    public struct Padding
    {
        public static readonly Padding Empty = new(0, 0, 0, 0);

        public int Left, Top, Right, Bottom;

        public Padding(int all) : this(all, all, all, all)
        {
        }

        public Padding(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Horizontal => Left + Right;

        public int Vertical => Top + Bottom;

        public static implicit operator Padding(WfPadding padding)
        {
            return new(padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        public static implicit operator Padding(WpfPadding padding)
        {
            return new((int)padding.Left, (int)padding.Top, (int)padding.Right, (int)padding.Bottom);
        }

        public static implicit operator WfPadding(Padding padding)
        {
            return new(padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        public static implicit operator WpfPadding(Padding padding)
        {
            return new(padding.Left, padding.Top, padding.Right, padding.Bottom);
        }
    }
}