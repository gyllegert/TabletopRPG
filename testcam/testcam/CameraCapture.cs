﻿using System;
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
        Capture capture = null;
        bool captureInProgress;

        Image<Hsv, Byte> ImgHsv;
        Image<Gray, Byte> ImgBlue;
        Image<Gray, Byte> ImgGreen;
        Image<Gray, Byte> ImgTeal;
        Image<Gray, Byte> ImgMagenta;

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
                //capture.SetCaptureProperty(CapProp.Exposure, 0.1);
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
            ImgGreen = FindRange(ImgHsv, 60, 80, 0, 255, 150, 255);
            ImgTeal = FindRange(ImgHsv, 80, 90, 0, 255, 150, 255);
            ImgMagenta = FindRange(ImgHsv, 110, 130, 0, 255, 150, 255);

            int blobAreaMinSize = 500;
            int blobAreaMaxSize = 2000;

            captureBoxOrg.Image = ImageFrame;
            //captureBoxBlue.Image = FindBlobs(ImgBlue, 0, 0, 255, blobAreaMinSize);
            captureBoxGreen.Image = FindBlobs(ImgGreen, 0, 255, 0, blobAreaMinSize);
            captureBoxTeal.Image = FindBlobs(ImgTeal, 0, 255, 255, blobAreaMinSize);
            //captureBoxMagenta.Image = FindBlobs(ImgMagenta, 255, 0, 255, blobAreaMinSize);

            captureBoxMagenta.Image = FindGamePiece(ImgBlue, 0, 0, 255, blobAreaMinSize, blobAreaMaxSize);

            captureBoxBlue.Image = CreateScreenGrid(ImgHsv, 8, 8);

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

        public Image<Rgb, byte> FindBlobs(Image<Gray, byte> imgGray, int r, int g, int b, int minBlobArea)
        {

            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = new Rgb(r, g, b);

            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > minBlobArea)
                {
                    blobImg.Draw(targetBlob.BoundingBox, val, 1);
                }
            }

            return blobImg;
        }

        public Image<Rgb, byte> FindGamePiece(Image<Gray, byte> imgGray, int r, int g, int b, int minBlobArea, int maxBlobArea)
        {


            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = new Rgb(r, g, b);

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


            return blobImg;
        }

        public Image<Rgb, byte> CreateScreenGrid(Image<Hsv, byte> ImgHsv, int numGridsX, int numGridsY)
        {

            Image<Gray, byte> imgGray = FindValue(ImgHsv, 200, 255);

            CvBlobDetector detector = new CvBlobDetector();
            CvBlobs resultingBlobs = new CvBlobs();

            uint numBlobsFound = detector.Detect(imgGray, resultingBlobs);

            Image<Rgb, byte> blobImg = imgGray.Convert<Rgb, byte>();
            Rgb val = new Rgb(255, 255, 255);

            Rectangle boundingBox = new Rectangle();

            foreach (CvBlob targetBlob in resultingBlobs.Values)
            {
                if (targetBlob.Area > 1000)
                {
                    blobImg.Draw(targetBlob.BoundingBox, val, 3);
                    boundingBox = targetBlob.BoundingBox;
                }
               
            }

            Point topLeft = boundingBox.Location;
            int boxWidth = boundingBox.Width;
            int boxHeight = boundingBox.Height;

            int gridWidth = boxWidth / numGridsX;
            int gridHeight = boxHeight / numGridsY;

            GridArea[,] WebcamGrid = new GridArea[8, 8];

            Rgb gridRGB = new Rgb(0, 0, 0);

            for (int y = 0; y < numGridsY; y++)
            {
                for (int x = 0; x < numGridsX; x++)
                {
                    Point tempPoint = topLeft;
                    tempPoint.X = tempPoint.X + (gridWidth * x);
                    tempPoint.Y = tempPoint.Y + (gridHeight * y);

                    Point gridLocation = new Point(x, y);

                    WebcamGrid[x, y] = new GridArea(tempPoint, gridLocation, gridWidth, gridHeight);
                    blobImg.Draw(WebcamGrid[x, y].getRectangle(), gridRGB, 2);
                }
            }

            return blobImg;
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

    }
}
