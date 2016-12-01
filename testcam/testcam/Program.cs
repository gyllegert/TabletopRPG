using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

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
        public Rgb color = new Rgb(255, 255, 255);

        public GridArea(Point topLeftCoords, Point gridLocation, int width, int height)
        {
            this.topLeftCoords = topLeftCoords;
            this.gridLocation = gridLocation;

            centerCoords.X = width / 2;
            centerCoords.Y = height / 2;

            this.width = width;
            this.height = height;
        }

        public GridArea(Point topLeftCoords, Point gridLocation, int width, int height, Rgb color)
        {
            this.topLeftCoords = topLeftCoords;
            this.gridLocation = gridLocation;

            centerCoords.X = (width * (gridLocation.X + 2));
            centerCoords.Y = (height * (gridLocation.Y + 1));

            this.width = width;
            this.height = height;

            this.color = color;
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

        public GridArea GetGrid(GridArea[,] gridAreaArr)
        {
            GridArea gridArea = null;

            double vectorX, vectorY;
            int vectorL = 0;

            for (int y = 0; y < gridAreaArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridAreaArr.GetLength(0); x++)
                {
                    vectorX = gridAreaArr[x, y].centerCoords.X - centerCoords.X;
                    vectorY = gridAreaArr[x, y].centerCoords.Y - centerCoords.Y;

                    int temp = Convert.ToInt32(Math.Sqrt((Math.Pow(vectorX, 2) + Math.Pow(vectorY, 2))));
                    if (vectorL == 0)
                    {
                        vectorL = temp;
                        gridArea = gridAreaArr[x, y];
                    }
                    else if (temp < vectorL)
                    {
                        vectorL = temp;
                        gridArea = gridAreaArr[x, y];
                    }
                }
            }
            return gridArea;
        }
    }
}

