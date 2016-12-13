using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace testcam
{
    public partial class Battlemat : Form
    {
        float screenRatioW;
        float screenRatioH;

        GridArea[,] screenGrid = new GridArea[8, 8];
        PictureBox[,] screenPictureBoxArr = new PictureBox[8, 8];

        int moveSuggestionPieceNum = 0;

        Bitmap blue = new Bitmap(Properties.Resources.Blue);
        Bitmap red = new Bitmap(Properties.Resources.Red);
        Bitmap white = new Bitmap(Properties.Resources.White);
        Bitmap teal = new Bitmap(Properties.Resources.Teal);

        List<Point> pieceGridPositions = new List<Point>();

        Enemy enemy1;
        PictureBox enemy1PicBox;

        public Battlemat()
        {
            InitializeComponent();
        }

        public void Battlemat_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            CreateScreenGrid(8, 8);

            enemy1 = new Enemy(screenGrid[5, 3].centerCoords, screenGrid[5, 3].topLeftCoords, 128, 128, 1);
            enemy1PicBox = CreatePictureBox(enemy1.topLeftCoords, red);

            run();
        }

        public void run()
        {
            while (true)
            {

                bluePieceBox = updatePiecePositionOnGrid(bluePieceBox, CameraCapture.blueGrid, 0);
                greenPieceBox = updatePiecePositionOnGrid(greenPieceBox, CameraCapture.greenGrid, 1);
                tealPieceBox = updatePiecePositionOnGrid(tealPieceBox, CameraCapture.tealGrid, 2);

                MoveSuggestions(CameraCapture.blueGrid, 0, 3);

                bluePieceBox.Refresh();
                greenPieceBox.Refresh();
                tealPieceBox.Refresh();
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

        private PictureBox updatePiecePositionOnGrid(PictureBox picBox, GridArea grid, int pieceNum)
        {
            Point newP = new Point();

            foreach (GridArea gridArea in screenGrid)
            {
                if (grid.gridLocation == gridArea.gridLocation)
                {
                    newP = gridArea.topLeftCoords;
                    picBox.Location = newP;

                    if (pieceGridPositions.Count < pieceNum + 1)
                    {
                        pieceGridPositions.Add(newP);
                    }

                    if (pieceGridPositions[pieceNum] != newP)
                    {
                        pieceGridPositions[pieceNum] = newP;

                        if (moveSuggestionPieceNum == pieceNum)
                        {
                            DisposePictureBoxes();
                        }

                    }

                }
            }

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

                    screenPictureBoxArr[x, y] = CreatePictureBox(tempPoint, white);
                }
            }
        }

        private void MoveSuggestions(GridArea grid, int pieceNum, int radius)
        {

            for (int y = -radius; y < radius + 1; y++)
            {
                for (int x = -radius; x < radius + 1; x++)
                {
                    if(x == 0 && y == 0)
                    {

                    }
                    else if (grid.gridLocation.X >= -x && grid.gridLocation.Y >= -y && grid.gridLocation.X < screenGrid.GetLength(0) - x 
                             && grid.gridLocation.Y < screenGrid.GetLength(1) - y)
                    {

                        screenPictureBoxArr[grid.gridLocation.X + x, grid.gridLocation.Y + y].Image = blue;
                        //CreatePictureBox(screenGrid[grid.gridLocation.X + x, grid.gridLocation.Y + y].topLeftCoords, Properties.Resources.Blue);

                    }

                }
            }

            //Adds the pictureboxes to the form and forces them to be shown
            foreach (PictureBox pb in screenPictureBoxArr)
            {
                pb.Refresh();
            }
            moveSuggestionPieceNum = pieceNum;
        }

        private void DisposePictureBoxes()
        {
            foreach (PictureBox pb in screenPictureBoxArr)
            {
                pb.Image = white;
                //this.Controls.Remove(pb);
                //pb.Dispose();
            }
        }

        private PictureBox CreatePictureBox(Point topLeftCoords, Bitmap image)
        {
            PictureBox box = new PictureBox();
            box.Size = new Size(128, 128);
            box.Image = image;
            box.Location = topLeftCoords;
            this.Controls.Add(box);

            return box;
        }

    }
}
