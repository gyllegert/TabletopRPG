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

        public static int boxWidth;
        public static int boxHeight;

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
                capture = new Capture();
                capture.SetCaptureProperty(CapProp.FrameWidth, 640);
                capture.SetCaptureProperty(CapProp.FrameHeight, 480);
                capture.SetCaptureProperty(CapProp.AutoExposure, 0);
                capture.SetCaptureProperty(CapProp.WhiteBalanceBlueU, 4250);
            }
            catch (NullReferenceException excpt)
            {

                MessageBox.Show(excpt.Message);
            }

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
            richTextBox1.AppendText("Width is:" + capture.GetCaptureProperty(CapProp.FrameWidth) + "Height is:" + capture.GetCaptureProperty(CapProp.FrameHeight));
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat temp = capture.QueryFrame();
            Image<Bgr, Byte> ImageFrame = temp.ToImage<Bgr, byte>();

            ImgHsv = ImageFrame.Convert<Hsv, Byte>();

            CvInvoke.MedianBlur(ImgHsv, ImgHsv, 9);

            ImgBlue = FindRange(ImgHsv, 95, 105, 100, 255, 80, 255);
            ImgGreen = FindRange(ImgHsv, 30, 50, 0, 255, 80, 255);
            ImgTeal = FindRange(ImgHsv, 10, 25, 0, 255, 80, 255);
            ImgMagenta = FindRange(ImgHsv, 110, 130, 0, 255, 150, 255);

            int blobAreaMinSize = 500;
            int blobAreaMaxSize = 2000;

            Rgb blue = new Rgb(0, 0, 255);
            Rgb teal = new Rgb(0, 255, 255);
            Rgb white = new Rgb(255, 255, 225);
            Rgb black = new Rgb(0, 0, 0);
            Rgb green = new Rgb(0, 255, 0);

            gridImg = CreateScreenGrid(ImgHsv, 8, 8, black);

            object[] objArr = FindGamePiece(ImgBlue, blue, blobAreaMinSize, blobAreaMaxSize, blueGrid);
            object[] greenArr = FindGamePiece(ImgGreen, green, blobAreaMinSize, blobAreaMaxSize, greenGrid);

            Image<Rgb, byte> bluePieceImg = (Image<Rgb, byte>)objArr[0];
            blueGrid = (GridArea)objArr[1];

            Image<Rgb, byte> greenPieceImg = (Image<Rgb, byte>)greenArr[0];
            greenGrid = (GridArea)greenArr[1];

            gridImg = ColorGrid(gridImg, blueGrid, blue);
            gridImg = ColorGrid(gridImg, greenGrid, green);

            captureBoxOrg.Image = ImageFrame;
            //captureBoxBlue.Image = FindBlobs(ImgBlue, 0, 0, 255, blobAreaMinSize);
            captureBoxGreen.Image = greenPieceImg;
            captureBoxTeal.Image = FindBlobs(ImgTeal, teal, blobAreaMinSize);
            //captureBoxMagenta.Image = FindBlobs(ImgMagenta, 255, 0, 255, blobAreaMinSize);

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
            Image<Gray, Byte>[] channels = ImgHsv.Split();
            Image<Gray, Byte> imgVal = channels[2];

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
            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = rgb;

            Rectangle boundingBox = new Rectangle();
            Point centerCoords = new Point();

            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > minBlobArea && targetBlob.Area < maxBlobArea)
                {
                    blobImg.Draw(targetBlob.BoundingBox, val, 1);
                    boundingBox = targetBlob.BoundingBox;
                    centerCoords = Point.Round(targetBlob.Centroid);
                }
            }

            Point topLeft = boundingBox.Location;
            int pieceWidth = boundingBox.Width;
            int pieceHeight = boundingBox.Height;

            GamePiece piece = new GamePiece(centerCoords, topLeft, pieceWidth, pieceHeight);

            blobImg.Draw(piece.getCenterRectangle(), val, 2);

            closestGrid = piece.GetGrid(WebcamGrid);

            object[] objArr = new object[] { blobImg, closestGrid };

            return objArr;
        }

        public Image<Rgb, byte> CreateScreenGrid(Image<Hsv, byte> ImgHsv, int numGridsX, int numGridsY, Rgb rgb)
        {
            Image<Gray, byte> imgGray = FindValue(ImgHsv, 200, 255);

            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();

            Rectangle boundingBox = new Rectangle();

            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > 1000)
                {
                    blobImg.Draw(targetBlob.BoundingBox, new Rgb(255, 255, 255), 3);
                    boundingBox = targetBlob.BoundingBox;
                }
            }

            Point topLeft = boundingBox.Location;
            boxWidth = boundingBox.Width;
            boxHeight = boundingBox.Height;

            int gridWidth = boxWidth / numGridsX;
            int gridHeight = boxHeight / numGridsY;

            for (int y = 0; y < numGridsY; y++)
            {
                for (int x = 0; x < numGridsX; x++)
                {
                    Point tempPoint = topLeft;
                    tempPoint.X = tempPoint.X + (gridWidth * x);
                    tempPoint.Y = tempPoint.Y + (gridHeight * y);

                    Point gridLocation = new Point(x, y);

                    WebcamGrid[x, y] = new GridArea(tempPoint, gridLocation, gridWidth, gridHeight, rgb);
                    blobImg = ColorGrid(blobImg, WebcamGrid[x, y], rgb);
                    blobImg.Draw(WebcamGrid[x, y].getCenterRectangle(), rgb, 2);
                }
            }
            return blobImg;
        }

        public Image<Rgb, byte> ColorGrid(Image<Rgb, byte> imgRGB, GridArea grid, Rgb rgb)
        {
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
            Battlemat bt = new Battlemat();
            Screen[] screens = Screen.AllScreens;
            Rectangle location = screens[1].Bounds;
            bt.StartPosition = FormStartPosition.Manual;
            bt.SetBounds(location.X, location.Y, 1024, 1024);
            bt.Show();

            
        }
    }
}
