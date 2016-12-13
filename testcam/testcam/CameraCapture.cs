using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Cvb;

namespace testcam
{
    public partial class CameraCapture : Form
    {
        #region Decalaring global variables

        Capture capture = null;
        bool captureInProgress;

        public static int webcamResX = 640;
        public static int webcamResY = 480;

        Image<Hsv, Byte> ImgHsv;
        Image<Gray, Byte> ImgBlue;
        Image<Gray, Byte> ImgGreen;
        Image<Gray, Byte> ImgTeal;
        Image<Gray, Byte> ImgMagenta;

        Image<Rgb, byte> gridImg = null;

        GridArea[,] WebcamGrid = new GridArea[8, 8];
        public static GridArea blueGrid = null;
        public static GridArea tealGrid = null;
        public static GridArea greenGrid = null;

        public static GamePiece bluePiece = null;
        public static GamePiece tealPiece = null;
        public static GamePiece greenPiece = null;

        public static int boxWidth;
        public static int boxHeight;

        Rgb blue = new Rgb(0, 0, 255);
        Rgb teal = new Rgb(0, 255, 255);
        Rgb white = new Rgb(255, 255, 225);
        Rgb black = new Rgb(0, 0, 0);
        Rgb green = new Rgb(0, 255, 0);

        int blobAreaMinSize, blobAreaMaxSize;
        #endregion

        public CameraCapture()
        {
            InitializeComponent();
        }

        private void CameraCapture_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //Creates the capture and sets the capture properties
                capture = new Capture();
                capture.SetCaptureProperty(CapProp.FrameWidth, webcamResX);
                capture.SetCaptureProperty(CapProp.FrameHeight, webcamResY);
                capture.SetCaptureProperty(CapProp.AutoExposure, 0);
                capture.SetCaptureProperty(CapProp.WhiteBalanceBlueU, 4250);
            }
            catch (NullReferenceException excpt)
            {

                MessageBox.Show(excpt.Message);
            }

            //Determine the function of the "start" button depending on the state of the capture
            if (capture != null)
            {
                if (captureInProgress)
                {  //if camera is getting frames then stop the capture and set button Text
                    // "Start" for resuming capture
                    btnStart.Text = "Start";
                    Application.Idle -= ProcessFrame;
                }
                else
                {
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Stop" for pausing capture
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }

                captureInProgress = !captureInProgress;
            }

            //Debugging text, shows webcam resolution
            richTextBox1.AppendText("Width is: " + capture.GetCaptureProperty(CapProp.FrameWidth) + " Height is: " + capture.GetCaptureProperty(CapProp.FrameHeight));
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            //Get the latest image from the webcam
            Mat temp = capture.QueryFrame();

            //Convert from mat to Image<>. Mat doesn't contain the functionality needed
            Image<Bgr, Byte> ImageFrame = temp.ToImage<Bgr, byte>();

            //Converting to HSV format
            ImgHsv = ImageFrame.Convert<Hsv, Byte>();

            //Blur the image to reduce noise
            CvInvoke.MedianBlur(ImgHsv, ImgHsv, 9);

            //Creates new images that are thresholding for specific values to find the gamePieces 
            ImgBlue = FindRange(ImgHsv, 100, 110, 100, 255, 80, 255);
            ImgGreen = FindRange(ImgHsv, 30, 50, 0, 255, 80, 255);
            ImgTeal = FindRange(ImgHsv, 85, 95, 0, 255, 80, 255);
            ImgMagenta = FindRange(ImgHsv, 110, 130, 0, 255, 150, 255);

            //initialize the blobArea size variables
            blobAreaMinSize = 300;
            blobAreaMaxSize = 2000;

            //Creates the array of GridAreas that fits the bounding box of the screen
            gridImg = CreateScreenGrid(ImgHsv, 8, 8, black);

            //Finding the gamePieces 
            object[] blueArr = FindGamePiece(ImgBlue, blue, blobAreaMinSize, blobAreaMaxSize, blueGrid);
            object[] greenArr = FindGamePiece(ImgGreen, green, blobAreaMinSize, blobAreaMaxSize, greenGrid);
            object[] tealArr = FindGamePiece(ImgTeal, teal, blobAreaMinSize, blobAreaMaxSize, tealGrid);

            //Putting the objects into their respective variables 
            Image<Rgb, byte> bluePieceImg = (Image<Rgb, byte>)blueArr[0];
            blueGrid = (GridArea)blueArr[1];
            bluePiece = (GamePiece)blueArr[2];

            Image<Rgb, byte> greenPieceImg = (Image<Rgb, byte>)greenArr[0];
            greenGrid = (GridArea)greenArr[1];
            greenPiece = (GamePiece)greenArr[2];

            Image<Rgb, byte> tealPieceImg = (Image<Rgb, byte>)tealArr[0];
            tealGrid = (GridArea)tealArr[1];
            tealPiece = (GamePiece)tealArr[2];

            //Draws a rectangle on the screen grid to show where the gamePieces are
            gridImg = ColorGrid(gridImg, blueGrid, blue);
            gridImg = ColorGrid(gridImg, greenGrid, green);
            gridImg = ColorGrid(gridImg, tealGrid, teal);

            //Shows the images on the CameraCapture form
            captureBoxOrg.Image = ImageFrame;
            captureBoxGreen.Image = greenPieceImg;
            captureBoxTeal.Image = tealPieceImg;
            captureBoxBlue.Image = gridImg;
            captureBoxMagenta.Image = bluePieceImg;
        }

        public Image<Gray, byte> FindRange(Image<Hsv, byte> ImgHsv, int HMin, int HMax, int VMin, int VMax, int SMin, int SMax)
        {
            //Split input HSV img into separate channels
            Image<Gray, Byte>[] channels = ImgHsv.Split();
            Image<Gray, Byte> imgHue = channels[0];
            Image<Gray, Byte> imgSat = channels[1];
            Image<Gray, Byte> imgVal = channels[2];

            //Filter out colors that aren't inside the given range
            Image<Gray, byte> huefilter = imgHue.InRange(new Gray(HMin), new Gray(HMax));

            //Filter out low saturation colors
            Image<Gray, byte> satfilter = imgSat.InRange(new Gray(SMin), new Gray(SMax));

            //Filter out colors that aren't bright
            Image<Gray, byte> valfilter = imgVal.InRange(new Gray(VMin), new Gray(VMax));

            //Put the three channels back together
            Image<Gray, byte> FImg = huefilter.And(valfilter).And(satfilter);

            return FImg;
        }

        public Image<Gray, byte> FindValue(Image<Hsv, byte> ImgHsv, int VMin, int VMax)
        {
            //Thresholds the input image within the gives value range

            //The image is split into its three channels
            Image<Gray, Byte>[] channels = ImgHsv.Split();

            //Value is the last channel in an Hsv image
            Image<Gray, Byte> imgVal = channels[2];

            //Uses the InRange method to threshold within the given value range
            Image<Gray, byte> valfilter = imgVal.InRange(new Gray(VMin), new Gray(VMax));

            return valfilter;

        }

        public Image<Rgb, byte> FindBlobs(Image<Gray, byte> imgGray, Rgb rgb, int minBlobArea)
        {

            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = rgb;

            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > minBlobArea)
                {
                    blobImg.Draw(targetBlob.BoundingBox, val, 1);
                }
            }

            return blobImg;
        }

        public object[] FindGamePiece(Image<Gray, byte> imgGray, Rgb rgb, int minBlobArea, int maxBlobArea, GridArea closestGrid)
        {

            //Create a blob detector and a CvBlobs to contain the blobs in
            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            //Detects blobs and puts them in resultingBlobs
            detector.Detect(imgGray, resultingBlobs);

            //Converts from Gray to RGB
            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = rgb;

            //Create variables needed for GamePiece object creation
            Rectangle boundingBox = new Rectangle();
            Point centerCoords = new Point();

            //Goes through all blobs found and if the blob is of the correct size, then its bounding box is drawn on blobImg
            //and the variables boundingBox and centerCoords get their values from the blob
            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > minBlobArea && targetBlob.Area < maxBlobArea)
                {
                    blobImg.Draw(targetBlob.BoundingBox, val, 1);
                    boundingBox = targetBlob.BoundingBox;
                    centerCoords = Point.Round(targetBlob.Centroid);
                }
            }

            //More varaibles for the gamePiece, all based on the boundingBox
            Point topLeft = boundingBox.Location;
            int pieceWidth = boundingBox.Width;
            int pieceHeight = boundingBox.Height;

            //Create the GamePiece object
            GamePiece piece = new GamePiece(centerCoords, topLeft, pieceWidth, pieceHeight);

            //Draw the center of the GamePiece on blobImg
            blobImg.Draw(piece.getCenterRectangle(), val, 2);

            //Gets the gridPosition of the GamePiece on the webcamGrid
            closestGrid = piece.GetGrid(WebcamGrid);

            //puts the image, GridArea and GamePiece in an object array
            object[] objArr = new object[] { blobImg, closestGrid, piece };

            return objArr;
        }

        public Image<Rgb, byte> CreateScreenGrid(Image<Hsv, byte> ImgHsv, int numGridsX, int numGridsY, Rgb rgb)
        {
            //Threshold ImgHsv for anything with a value above 199 
            //Since battlemat background is white no color threshold needed 
            Image<Gray, byte> imgGray = FindValue(ImgHsv, 200, 255);

            //Create a blob detector and a CvBlobs to contain the blobs in
            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            //Detects blobs and puts them in resultingBlobs
            detector.Detect(imgGray, resultingBlobs);

            //Converts from Gray to RGB
            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();

            //Create variable needed for creation of the GradArea array
            Rectangle boundingBox = new Rectangle();

            //Goes through all blobs to find the blob for the screen and if the blob is of the correct size, then its bounding box is drawn on blobImg
            //and the variable boundingBox gets its value from the blob
            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > 100000)
                {
                    blobImg.Draw(targetBlob.BoundingBox, new Rgb(255, 255, 255), 3);
                    boundingBox = targetBlob.BoundingBox;
                }
            }

            //Define variables needed for GridArea creation
            Point topLeft = boundingBox.Location;
            boxWidth = boundingBox.Width;
            boxHeight = boundingBox.Height;

            int gridWidth = boxWidth / numGridsX;
            int gridHeight = boxHeight / numGridsY;

            //Creates the GridAreas and puts them in the WebcamGrid array
            for (int y = 0; y < numGridsY; y++)
            {
                for (int x = 0; x < numGridsX; x++)
                {
                    //Create the top left coordinates for the GridArea, based on the loop control variables 
                    //and the bounding box of the screen
                    Point tempPoint = topLeft;
                    tempPoint.X = tempPoint.X + (gridWidth * x);
                    tempPoint.Y = tempPoint.Y + (gridHeight * y);

                    //The gridLocation is the same as the same as the GridAreas position in the array
                    Point gridLocation = new Point(x, y);

                    //Creates the GridAreas
                    WebcamGrid[x, y] = new GridArea(tempPoint, gridLocation, gridWidth, gridHeight, rgb);

                    //Draws rectangles of the GridAreas on blobImg, as a visual aid when debugging
                    //The center of the GridAreas are also drawn. Also to help with debugging and functionality testing
                    blobImg = ColorGrid(blobImg, WebcamGrid[x, y], rgb);
                    blobImg.Draw(WebcamGrid[x, y].getCenterRectangle(), rgb, 2);
                }
            }
            return blobImg;
        }

        public Image<Rgb, byte> ColorGrid(Image<Rgb, byte> imgRGB, GridArea grid, Rgb rgb)
        {
            //Uses the Image<>.Draw method to draw a rectangle on the given GridArea
            //using the given Rgb color
            imgRGB.Draw(grid.getRectangle(), rgb, 2);
            return imgRGB;
        }

        public Image<Hsv, byte> FindColor(Image<Hsv, byte> ImgHsv, int HMin, int HMax, int saturation, int value)
        {
            for (int i = 0; i < ImgHsv.Cols; i++)
            {
                for (int j = 0; j < ImgHsv.Rows; j++)
                {
                    if (ImgHsv.Data[j, i, 0] > HMin && ImgHsv.Data[j, i, 0] < HMax && ImgHsv.Data[j, i, 1] > saturation && ImgHsv.Data[j, i, 2] > value)
                    {
                        ImgHsv.Data[j, i, 0] = 0;
                        ImgHsv.Data[j, i, 1] = 0;
                        ImgHsv.Data[j, i, 2] = 255;
                    }
                    else
                    {
                        ImgHsv.Data[j, i, 0] = 0;
                        ImgHsv.Data[j, i, 1] = 0;
                        ImgHsv.Data[j, i, 2] = 0;
                    }
                }
            }
            return ImgHsv;
        }

        public Image<Hsv, byte> FindRed(Image<Hsv, byte> ImgHsv, int HMin, int HMax, int HMin2, int HMax2, int saturation, int value)
        {
            for (int i = 0; i < ImgHsv.Cols; i++)
            {
                for (int j = 0; j < ImgHsv.Rows; j++)
                {
                    if (((ImgHsv.Data[j, i, 0] > HMin && ImgHsv.Data[j, i, 0] < HMax) ||
                        (ImgHsv.Data[j, i, 0] > HMin2 && ImgHsv.Data[j, i, 0] < HMax2)) && ImgHsv.Data[j, i, 1] > saturation && ImgHsv.Data[j, i, 2] > value)
                    {
                        ImgHsv.Data[j, i, 0] = 0;
                        ImgHsv.Data[j, i, 1] = 0;
                        ImgHsv.Data[j, i, 2] = 255;
                    }
                    else
                    {
                        ImgHsv.Data[j, i, 0] = 0;
                        ImgHsv.Data[j, i, 1] = 0;
                        ImgHsv.Data[j, i, 2] = 0;
                    }
                }
            }
            return ImgHsv;
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void Start_Battlemat_Click(object sender, EventArgs e)
        {
            //Creates a new thred to handle the Battlemat form
            ThreadStart ts = new ThreadStart(startBattlemat);
            Thread thread = new Thread(ts);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        private void startBattlemat()
        {
            //Create the Battlematform
            Battlemat bt = new Battlemat();
            //Get the array of screens in the system
            Screen[] screens = Screen.AllScreens;
            
            //Puts the form on the second screen
            Rectangle location = screens[1].Bounds;
            bt.StartPosition = FormStartPosition.Manual;
            bt.SetBounds(location.X, location.Y, 1280, 1024);
            bt.Show();
        }
    }
}
