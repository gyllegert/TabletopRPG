using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;



namespace testcam
{
    public partial class Battlemat : Form
    {
        float screenRatioW;
        float screenRatioH;

        GridArea[,] screenGrid = new GridArea[8, 8];


        public Battlemat()
        {
            InitializeComponent();
        }

        public void Battlemat_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            CreateScreenGrid(8, 8);

            while (true)
            {
               //Thread.Sleep(100);
                //MessageBox.Show("derp");
                bluePieceBox = updatePiecePositionOnGrid(bluePieceBox, CameraCapture.blueGrid);
                greenPieceBox = updatePiecePositionOnGrid(greenPieceBox, CameraCapture.greenGrid);
                tealPieceBox = updatePiecePositionOnGrid(tealPieceBox, CameraCapture.tealGrid);
                bluePieceBox.Refresh();
                greenPieceBox.Refresh();
                tealPieceBox.Refresh();
            }
            //run();

            //MessageBox.Show("box width is: " + CameraCapture.webcamResX + " box height is: " + CameraCapture.webcamResY + "\n"
            //                + " Second resolution width is: " + Screen.AllScreens[1].Bounds.Width + " and height is: " + Screen.AllScreens[1].Bounds.Height
            //                + " Reolutions ratio width is: " + screenRatioW + " and height is: " + screenRatioH);

        }

        public void run()
        {
            while (true)
            {
                Thread.Sleep(1000);
                //MessageBox.Show("bt.run");
                bluePieceBox = updatePiecePositionOnGrid(bluePieceBox, CameraCapture.blueGrid);
                greenPieceBox = updatePiecePositionOnGrid(greenPieceBox, CameraCapture.greenGrid);
                tealPieceBox = updatePiecePositionOnGrid(tealPieceBox, CameraCapture.tealGrid);
            }
        }

        private void bluePieceBox_Click(object sender, EventArgs e)
        {

        }


        private PictureBox updatePiecePosition(PictureBox picBox, GridArea grid)
        {
            float[] screenRatio = calculateResolutionRatio();

            Point newP = new Point(Convert.ToInt32((grid.topLeftCoords.X * screenRatio[0]) - 128), Convert.ToInt32(grid.topLeftCoords.Y * screenRatio[1]));

            picBox.Location = newP;

            return picBox;
        }

        private PictureBox updatePiecePositionOnGrid(PictureBox picBox, GridArea grid)
        {
            Point newP = new Point();

            foreach (GridArea gridArea in screenGrid)
            {
                if (grid.gridLocation == gridArea.gridLocation)
                {
                    newP = gridArea.topLeftCoords;
                }
            }

            picBox.Location = newP;

            return picBox;
        }

        private float[] calculateResolutionRatio()
        {
            float screen0W = CameraCapture.webcamResX;
            float screen0H = CameraCapture.webcamResY;

            float screen1W = Screen.AllScreens[1].Bounds.Width;
            float screen1H = Screen.AllScreens[1].Bounds.Height;

            screenRatioW = screen1W / screen0W;
            screenRatioH = screen1H / screen0H;

            float[] screenRatio = new float[] { screenRatioW, screenRatioH };

            return screenRatio;
        }

        private void CreateScreenGrid(int numGridsX, int numGridsY)
        {
            int gridWidth = Screen.AllScreens[1].Bounds.Height / numGridsX;
            int gridHeight = Screen.AllScreens[1].Bounds.Height / numGridsY;

            Point topLeft = new Point(128, 0);


            for (int y = 0; y < numGridsY; y++)
            {
                for (int x = 0; x < numGridsX; x++)
                {
                    Point tempPoint = topLeft;
                    tempPoint.X = tempPoint.X + (gridWidth * x);
                    tempPoint.Y = tempPoint.Y + (gridHeight * y);

                    Point gridLocation = new Point(x, y);

                    screenGrid[x, y] = new GridArea(tempPoint, gridLocation, gridWidth, gridHeight);
                }
            }
        }
    }
}
