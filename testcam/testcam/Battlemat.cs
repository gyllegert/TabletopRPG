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


        public Battlemat()
        {
            InitializeComponent();
        }

        private void Battlemat_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            bluePieceBox = updatePiecePosition(bluePieceBox, CameraCapture.greenGrid);

            MessageBox.Show("box width is: " + CameraCapture.boxWidth + " box height is: " + CameraCapture.boxHeight + "\n"
                            + "Second resolution width is: " + Screen.AllScreens[1].Bounds.Width + " and height is: " + Screen.AllScreens[1].Bounds.Height 
                            + "Reolutions ratio width is: " + screenRatioW + "and height is: " + screenRatioH);

        }

        private void bluePieceBox_Click(object sender, EventArgs e)
        {

        }


        private PictureBox updatePiecePosition(PictureBox picBox, GridArea grid)
        {
            float[] screenRatio = calculateResolutionRatio();

            Point newP = new Point(Convert.ToInt32(grid.topLeftCoords.X * screenRatio[0]-223), Convert.ToInt32(grid.topLeftCoords.Y * screenRatio[1]-96));

            picBox.Location = newP;

            return picBox;
        }

        private float[] calculateResolutionRatio()
        {
            float screen0W = CameraCapture.boxWidth;
            float screen0H = CameraCapture.boxHeight;

            float screen1W = 1024;
            float screen1H = Screen.AllScreens[1].Bounds.Height;

            screenRatioW = screen1W / screen0W;
            screenRatioH = screen1H / screen0H;

            float[] screenRatio = new float[] { screenRatioW, screenRatioH };

            return screenRatio;
        }
    }
}
