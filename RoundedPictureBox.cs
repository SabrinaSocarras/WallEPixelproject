using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WallEPixelproject
{

    public class RoundedPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(0, 0, Width, Height));
            Region = new Region(path);
            base.OnPaint(e);
        }
    }
} 