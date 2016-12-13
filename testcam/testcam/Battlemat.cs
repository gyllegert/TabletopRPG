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

        #region Declaring global variables
        float screenRatioW;
        float screenRatioH;

        GridArea[,] screenGrid = new GridArea[8, 8];
        PictureBox[,] screenPictureBoxArr = new PictureBox[8, 8];

        int moveSuggestionPieceNum = 0;

        Bitmap blue = new Bitmap(Properties.Resources.Blue);
        Bitmap red = new Bitmap(Properties.Resources.Red);
        Bitmap white = new Bitmap(Properties.Resources.White1);
        Bitmap teal = new Bitmap(Properties.Resources.Teal);

        List<Point> pieceGridPositions = new List<Point>();

        Enemy enemy1;
        PictureBox enemy1PicBox;

        #endregion

        public Battlemat()
        {
            InitializeComponent();
        }

        public void Battlemat_Load(object sender, EventArgs e)
        {
            //Making the form fullscreen by removing the border and maximizing
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            //Create an array of GridAreas and pictureboxes based on the screen resolution
            CreateScreenGrid(8, 8);


            enemy1 = new Enemy(screenGrid[5, 3].centerCoords, screenGrid[5, 3].topLeftCoords, 128, 128, 1);
            enemy1PicBox = CreatePictureBox(enemy1.topLeftCoords, red);

            //start the run method
            run();
        }

        public void run()
        {
            while (true)
            {
                //Updates the position of the pictureBoxes for each GamePiece
                bluePieceBox = updatePiecePositionOnGrid(bluePieceBox, CameraCapture.blueGrid, 0);
                greenPieceBox = updatePiecePositionOnGrid(greenPieceBox, CameraCapture.greenGrid, 1);
                tealPieceBox = updatePiecePositionOnGrid(tealPieceBox, CameraCapture.tealGrid, 2);

                //Make the movesuggestion grid
                MoveSuggestions(CameraCapture.blueGrid, 0, 2);

                //Forces the pictureboxes to update 
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
            //Moves a PictureBox to the location of a grid


            Point newP = new Point();
            //Goes through the screenGrid array to find the correct grid to move the PictureBox to
            foreach (GridArea gridArea in screenGrid)
            {
                //if the correct GridArea is found then the location of the PictureBox is changed
                //to the top left coordinates of the GridArea
                if (grid.gridLocation == gridArea.gridLocation)
                {
                    newP = gridArea.topLeftCoords;
                    picBox.Location = newP;

                    //adds the position of a GamePiece to a list if the GamePiece isn't already in the list
                    if (pieceGridPositions.Count < pieceNum + 1)
                    {
                        pieceGridPositions.Add(newP);
                    }

                    //if the position of the GamePiece is not the same as the one in the list
                    //then the information in the list is updated
                    if (pieceGridPositions[pieceNum] != newP)
                    {
                        pieceGridPositions[pieceNum] = newP;

                        //Checks if the GamePiece that has been moved is the same as the one that move suggestions
                        //are being made for. If so then the existing move suggestion grid is removed
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
            //Calculate the width and height of the GridAreas based on the resolution of the screen
            int gridWidth = Screen.AllScreens[1].Bounds.Height / numGridsX;
            int gridHeight = Screen.AllScreens[1].Bounds.Height / numGridsY;

            //The top left coordinates have a 128 pixel offset because the battlemat is square
            Point topLeft = new Point(128, 0);

            //Creates the GridArea array based on the given dimensions
            for (int y = 0; y < numGridsY; y++)
            {
                for (int x = 0; x < numGridsX; x++)
                {
                    //calculate the top left coordinates for a GridArea based on the loop control variables
                    Point tempPoint = topLeft;
                    tempPoint.X = tempPoint.X + (gridWidth * x);
                    tempPoint.Y = tempPoint.Y + (gridHeight * y);

                    //grid location is the same as array position
                    Point gridLocation = new Point(x, y);

                    //Creates the GridArea and puts it in the screenGrid Array
                    screenGrid[x, y] = new GridArea(tempPoint, gridLocation, gridWidth, gridHeight);

                    //Also creates a white picturebox at the same location as the current GridArea
                    //and puts it into an array. This is used later for movesuggestions
                    screenPictureBoxArr[x, y] = CreatePictureBox(tempPoint, white);
                }
            }
        }

        private void MoveSuggestions(GridArea grid, int pieceNum, int radius)
        {
            //Changes the images of pictureboxes around the given grid, with a given radius
            //grid size changes dynamically based on the radius
            for (int y = -radius; y < radius + 1; y++)
            {
                for (int x = -radius; x < radius + 1; x++)
                {
                    //if x and y are 0 then it¨s the same grid as the GamePiece and this 
                    //doesn't need to be changed
                    if (x == 0 && y == 0)
                    {
                    }
                    //The conditions make sure that we stay within the range of the screenGrid array
                    else if (grid.gridLocation.X >= -x && grid.gridLocation.Y >= -y && grid.gridLocation.X < screenGrid.GetLength(0) - x
                             && grid.gridLocation.Y < screenGrid.GetLength(1) - y)
                    {
                        //changes the image of a PictureBox
                        screenPictureBoxArr[grid.gridLocation.X + x, grid.gridLocation.Y + y].Image = blue;
                    }
                }
            }

            //Forces the PictureBoxes them to update
            foreach (PictureBox pb in screenPictureBoxArr)
            {
                pb.Refresh();
            }
            //makes sure that we know which gamepice the movesuggestion belongs to
            //This avoids the movesuggestions being removed is another GamePiece is moved
            moveSuggestionPieceNum = pieceNum;
        }

        private void DisposePictureBoxes()
        {
            //Goes trough all the PictureBoxes in screenPictureBoxArr and turns their image white.
            foreach (PictureBox pb in screenPictureBoxArr)
            {
                pb.Image = white;
            }
        }

        private PictureBox CreatePictureBox(Point topLeftCoords, Bitmap image)
        {
            //Creates a new PictureBox at the given location with the given image.
            //Also adds the PictureBox to the Control 
            PictureBox box = new PictureBox();
            box.Size = new Size(128, 128);
            box.Image = image;
            box.Location = topLeftCoords;
            this.Controls.Add(box);

            return box;
        }

    }
}
