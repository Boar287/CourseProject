using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution=1;
        private GameEngine gameEngine;
        private SoundPlayer player;
        private BinaryFormatter formatter = new BinaryFormatter();
        private string MusicPath = "D:\\ProjectC#\\GameOfLife\\obj\\Debug\\PhoneMusic.wav";

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeSound()
        {
            player = new SoundPlayer();
            player.SoundLocation = MusicPath;
            player.Load();

            if(!checkBox1.Checked)
            player.PlayLooping();
        }
        private void TheBeginning()
        {
            if (timer1.Enabled)
                return;

            if (player == null)
                InitializeSound();

            NUDResolution.Enabled = false;
            NUDDensity.Enabled = false;
            resolution = (int)NUDResolution.Value;

            gameEngine = new GameEngine
                (
                       ROWS : pictureBox1.Height / resolution,
                       COLS : pictureBox1.Width / resolution,
                       Density: (int)NUDDensity.Minimum + (int)NUDDensity.Maximum - (int)NUDDensity.Value
                );

            lblNumberOfGen.Text = Convert.ToString(gameEngine.CurrentGeneration);
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void DrawCurrentGeneration()
        {
            graphics.Clear(System.Drawing.Color.Black);

            var field = gameEngine.GetCurrentGeneration();
            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (field[x, y])
                        graphics.FillRectangle(System.Drawing.Brushes.Crimson, x * resolution, y * resolution, resolution - 1, resolution - 1);
                }
            }
        }

        private void DrawNextGeneration()
        {
            DrawCurrentGeneration();         

            pictureBox1.Refresh();
            lblNumberOfGen.Text = Convert.ToString(gameEngine.CurrentGeneration);
            lblPopulation.Text = Convert.ToString(gameEngine.Population);
            gameEngine.NextGeneration();
        }

        private void StopGame()
        {
            if (!timer1.Enabled)
                return;
            timer1.Stop();
            NUDResolution.Enabled = true;
            NUDDensity.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawNextGeneration();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            TheBeginning();
            btnResume.Enabled = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopGame();
            if (lblNumberOfGen.Text != "0")
                btnResume.Enabled = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            var x = e.Location.X / resolution;
            var y = e.Location.Y / resolution;

            if (e.Button == MouseButtons.Left)
                gameEngine.AddCell(x, y);

            if (e.Button == MouseButtons.Right)
                gameEngine.RemoveCell(x, y);
        }

        private void btnResume_Click_1(object sender, EventArgs e)
        {
            timer1.Start();
            btnResume.Enabled = false;
        }

        private void NUDResolution_ValueChanged(object sender, EventArgs e)
        {
            btnResume.Enabled = false;
        }

        private void NUDDensity_ValueChanged(object sender, EventArgs e)
        {
            btnResume.Enabled = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)NUDTimer.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                player?.Stop();
            if (checkBox1.Checked == false)
                player?.PlayLooping();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnStop_Click(this, null);
            if (lblNumberOfGen.Text == "0")
            {
                MessageBox.Show("Empty saving could not be done", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            using(FileStream fs=new FileStream(saveFileDialog1.FileName,FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs,gameEngine);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            btnStop_Click(this, null);
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            using (FileStream fs=new FileStream(openFileDialog1.FileName, FileMode.OpenOrCreate))
            {
                try
                {
                    gameEngine = (GameEngine)formatter.Deserialize(fs);
                }
                catch
                {
                    MessageBox.Show("Wrong file was chosen", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                graphics = Graphics.FromImage(pictureBox1.Image);
                resolution = pictureBox1.Height / gameEngine.GetCurrentGeneration().GetLength(1);
                NUDResolution.Value = resolution;
                NUDDensity.Value = (int)NUDDensity.Minimum + (int)NUDDensity.Maximum-gameEngine.Density;
                btnResume.Enabled = true;
            }
            DrawCurrentGeneration();
            lblNumberOfGen.Text = Convert.ToString(gameEngine.CurrentGeneration);
            lblPopulation.Text = Convert.ToString(gameEngine.Population);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (timer1.Enabled || lblNumberOfGen.Text=="0")
                return;

            if (e.KeyCode == Keys.F5)
                DrawNextGeneration();
        }
    }
}
