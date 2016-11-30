using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace testcam
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CameraCapture());
        }
    }

    public class GridArea
    {
        public Point gridLocation = new Point();
        public Point centerCoords = new Point();
        public Point topLeftCoords = new Point();
        public int width, height;

        public GridArea(Point topLeftCoords, Point gridLocation, int width, int height)
        {
            this.topLeftCoords = topLeftCoords;
            this.gridLocation = gridLocation;

            centerCoords.X = width / 2;
            centerCoords.Y = height / 2;

            this.width = width;
            this.height = height;

        }

        public Rectangle getRectangle()
        {
            Rectangle rect = new Rectangle(topLeftCoords, new Size(width, height));

            return rect;
        }
    }

    public class GamePiece
    {
        public Point centerCoords = new Point();
        public Point topLeftCoords = new Point();
        public int width, height;

        public GamePiece(Point centerCoords, Point topLeftCoords, int width, int height)
        {

            this.centerCoords = centerCoords;
            this.topLeftCoords = topLeftCoords;
            this.width = width;
            this.height = height;
        }

        public Rectangle getRectangle()
        {
            Rectangle rect = new Rectangle(topLeftCoords, new Size(width, height));

            return rect;
        }

        public Rectangle getCenterRectangle()
        {
            Rectangle rect = new Rectangle(centerCoords, new Size(1, 1));

            return rect;
        }

    }


}

