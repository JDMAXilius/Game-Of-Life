using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPortfolio
{
    public partial class Form1 : Form
    {
        //
        int width = 30;
        int height = 30;

        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color gridColor1 = Color.Black;
        Color cellColor = Color.Gray;
        Color cellColor1 = Color.DarkGreen;
        Color cellColor2 = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
       
        int count = 0;

        // Checking if it is Finite or Toraidal
        bool checkCount = false;

        // Alive Cells counts
        int aliveCells = 0;

        // Seeds counts
        int seed = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            //timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.cellcolor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            gridColor = Properties.Settings.Default.GridColor;
            gridColor1 = Properties.Settings.Default.GridColor1;
            universe = new bool[Properties.Settings.Default.UniverseX, Properties.Settings.Default.UniverseY];
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            //for (int i = y - 1; i <= y + 1; i++)
            //{
            //    for (int j = x - 1; j <= x + 1; j++)
            //    {
            //        if ((i < universe.GetLength(1) && i >= 0) && (j < universe.GetLength(0) && j >= 0))
            //        {
            //            if (universe[j, i] == true && !(i == y && j == x))
            //            {
            //                count += 1;
            //            }

            //            //if (universe[i, j] == false)
            //            //{
            //            //    count -= 1;
            //            //}
            //        }
            //    }
            //}

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }

            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        xCheck = universe.GetLength(0) - 1;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        yCheck = universe.GetLength(1) - 1;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= universe.GetLength(0))
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= universe.GetLength(1))
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }

            return count;

        }

        // Calculate the next generation of cells

        private void NextGeneration()
        {
           
            aliveCells = 0;
           
            //string CountNeighborstype = " ";

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;

                    if (checkCount == false)
                    {
                        count = CountNeighbors(x, y);
                        //CountNeighborstype = "Finite";
                    }
                    else
                    {
                        count = CountNeighborsToroidal(x, y);
                        //CountNeighborstype = "Toroidal";
                    }

                    if (count < 2 && universe[x, y] == true)
                    {
                        scratchPad[x, y] = false;
                        //CellsCount--;
                    }

                    if (count > 3 && universe[x, y] == true)
                    {
                        scratchPad[x, y] = false;
                        //CellsCount--;
                    }

                    if ((count == 2 || count == 3) && universe[x, y] == true)
                    {
                        scratchPad[x, y] = true;
                        //CellsCount++;
                    }

                    if ((count == 3) && universe[x, y] == false)
                    {
                        scratchPad[x, y] = true;
                        //CellsCount++;
                    }

                    if (scratchPad[x, y] == true)
                    {
                        aliveCells++;
                        //CellsCount++;
                    }

                }
            }

            // swap universe and scratchPad
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            graphicsPanel1.Invalidate();

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();
            toolStripStatusLabel3.Text = "Interval= " + timer.Interval.ToString();
            label1.Text = "Generations = " + generations.ToString();
            label2.Text = "Cell Count = " + aliveCells.ToString();

            //label3.Text = "Boundry Type = " + CountNeighborstype.ToString();
            label4.Text = $"Universe Size = Width = {universe.GetLength(0)}, Height = {universe.GetLength(1)}";
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            Pen gridPen1 = new Pen(gridColor1, 2);
            //string num = " s ";
            //num = aliveCells;

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush cellBrush1 = new SolidBrush(cellColor1);
            Brush cellBrush2 = new SolidBrush(cellColor2);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    //
                    RectangleF cellRect1 = Rectangle.Empty;
                    cellRect1.X = x * cellWidth * 10;
                    cellRect1.Y = y * cellHeight * 10;
                    cellRect1.Width = cellWidth * 10;
                    cellRect1.Height = cellHeight * 10;
  
                    Font font = new Font("Arial", 9f);

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    e.Graphics.DrawRectangle(gridPen1, cellRect1.X, cellRect1.Y, cellRect1.Width, cellRect1.Height);
                    //graphicsPanel1.Invalidate();

                    if (checkCount == false)
                    {
                        count = CountNeighbors(x, y);
                    }
                    else
                    {
                        count = CountNeighborsToroidal(x, y);
                    }

                    if (count >= 1)
                    {
                        if (universe[x, y])
                        {
                            if (count==2||count==3)
                            {
                                e.Graphics.DrawString(count.ToString(), font, cellBrush1, cellRect, stringFormat);
                            }
                            else
                            {
                                e.Graphics.DrawString(count.ToString(), font, cellBrush2, cellRect, stringFormat);
                            }
                        }
                        else
                        {
                            if (count==3)
                            {
                                e.Graphics.DrawString(count.ToString(), font, cellBrush1, cellRect, stringFormat);
                            }
                            else
                            {
                                e.Graphics.DrawString(count.ToString(), font, cellBrush2, cellRect, stringFormat);
                            }
                        }
                    }
                }
                //graphicsPanel1.Invalidate();
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();

                // Cell Count
                if (!universe[(int)x, (int)y] == false)
                {
                    aliveCells++;
                }
                else
                {
                    aliveCells--;
                }
                toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();
                label2.Text = "Cell Count = " + aliveCells.ToString();
            }

            //graphicsPanel1.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    scratchPad[x, y] = false;
                    timer.Enabled = false;
                    generations = 0;
                    aliveCells = 0;

                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();

                    label1.Text = "Generations = " + generations.ToString();
                    label2.Text = "Cell Count = " + aliveCells.ToString();
                }
            }

            graphicsPanel1.Invalidate();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Start();
            //timer.Enabled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }

            graphicsPanel1.Invalidate();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //timer.Stop();
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.

                //writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x,y]==true)
                        {
                            writer.Write('O');

                        }

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x,y]==false)
                        {
                            writer.Write('.');
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                    //writer.WriteLine("!This is my comment.");
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!")==true)
                    {
                       
                    }
                    else
                    {
                        maxHeight++;
                        if (row.Length>maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int yPos = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!") == true)
                    {

                    }
                    else
                    {
                        maxHeight++;
                        if (row.Length > maxWidth)
                        {
                            maxWidth = row.Length;
                        }
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos]== 'O')
                        {
                            universe[xPos, yPos] = true;
                        }
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                        if (row[xPos]==',')
                        {
                            universe[xPos, yPos] = false;
                        }
                    }
                    yPos++;
                }
                graphicsPanel1.Invalidate();
                // Close the file.
                reader.Close();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void runToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }

        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void fromSeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sed_Dialog menu = new Sed_Dialog();
            seed = 0;

            if (DialogResult.OK == menu.ShowDialog())
            {

                seed = (int)menu.RandomSeed;
                Random rng = new Random(seed);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        universe[x, y] = false;
                        if (rng.Next(0, 5) == 0)
                        {
                            universe[x, y] = true;
                        }
                    }
                }

            }

            generations = 0;
            aliveCells = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();
            toolStripStatusLabel2.Text = "Seed = " + seed.ToString();

            timer.Interval = 100;
            timer.Enabled = false;
            graphicsPanel1.Invalidate();
        }

        private void toraidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkCount = true;
            finiteToolStripMenuItem.Checked = false;
            toraidalToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
            label3.Text = "Boundry Type = Toraidal";
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkCount = false;
            finiteToolStripMenuItem.Checked = true;
            toraidalToolStripMenuItem.Checked = false;
            graphicsPanel1.Invalidate();
            label3.Text = "Boundry Type = Finite";
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;

            if (gridToolStripMenuItem.Checked == false)
            {
                gridColor = Color.Black;
                gridColor1 = Color.Black;
                gridToolStripMenuItem.Checked = true;
            }
            else
            {
                gridColor = Color.Transparent;
                gridColor1 = Color.Transparent;
                gridToolStripMenuItem.Checked = false;

            }

            graphicsPanel1.Invalidate();
        }

        private void fromCurrentSeadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Sed_Dialog menu = new Sed_Dialog();
            //seed = (int)menu.RandomSeed;
            Random rng = new Random(seed);
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    if (rng.Next(0, 5) == 0)
                    {
                        universe[x, y] = true;
                    }
                }
            }

            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //seed = 0;
            Sed_Dialog menu = new Sed_Dialog();
            //seed = (int)menu.RandomSeed;
            //timer.Interval = (Timer)menu.RandomSeed;
            Random rng = new Random(DateTime.Now.Second);
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    if (rng.Next(0, 5) == 0)
                    {
                        universe[x, y] = true;
                    }
                }
            }

            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //width = 15;
            //height = 15;
            OptionsDialog menu = new OptionsDialog();
            //menu.Interval(timer);
            menu.Interval = timer.Interval;
            menu.Width = width;
            menu.Height = height;
            if (DialogResult.OK == menu.ShowDialog())
            {
                width = (int)menu.Width;
                height = (int)menu.Height;
                timer.Interval = (int)menu.Interval;
                universe = new bool[width, height];
                scratchPad = new bool[width, height];
                timer.Enabled = false;
                generations = 0;
                aliveCells = 0;
                toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();
                toolStripStatusLabel3.Text = "Interval= " + timer.Interval.ToString();
                label1.Text = "Generations = " + generations.ToString();
                label2.Text = "Cell Count = " + aliveCells.ToString();
                label4.Text = $"Universe Size = Width = {universe.GetLength(0)}, Height = {universe.GetLength(1)}";
            }

            graphicsPanel1.Invalidate();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.cellcolor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            gridColor = Properties.Settings.Default.GridColor;
            gridColor1 = Properties.Settings.Default.GridColor1;

            universe = new bool[Properties.Settings.Default.UniverseX, Properties.Settings.Default.UniverseY];


            //timer.Interval = 100;
            //width = 15;
            //height = 15;
            //gridColor = Color.Black;
            //cellColor = Color.Gray;
            //graphicsPanel1.BackColor = Color.White;
            //universe = new bool[width, height];
            timer.Enabled = false;
            generations = 0;
            aliveCells = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel1.Text = "aliveCells = " + aliveCells.ToString();
            toolStripStatusLabel3.Text = "Interval= " + timer.Interval.ToString();
            label1.Text = "Generations = " + generations.ToString();
            label2.Text = "Cell Count = " + aliveCells.ToString();
            label4.Text = $"Universe Size = Width = {universe.GetLength(0)}, Height = {universe.GetLength(1)}";
            graphicsPanel1.Invalidate();
        }

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            generations = 0;
            RunToDialog menu = new RunToDialog();
            menu.Generation = generations;
            if (DialogResult.OK == menu.ShowDialog())
            {
                generations = (int)menu.Generation;
            }
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            label1.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hUDToolStripMenuItem.Checked == false)
            {
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                hUDToolStripMenuItem.Checked = true;
            }
            else
            {
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                hUDToolStripMenuItem.Checked = false;
            }
            graphicsPanel1.Invalidate();
        }

        private void gridColorX10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor1;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor1 = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void neightborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            ColorDialog dlg1 = new ColorDialog();
            dlg.Color = cellColor1;
            dlg1.Color = cellColor2;
            if (neightborCountToolStripMenuItem.Checked == false)
            {
                cellColor1 = Color.Transparent;
                cellColor2 = Color.Transparent;
                neightborCountToolStripMenuItem.Checked = true;
            }
            else
            {
                cellColor1 = Color.DarkGreen;
                cellColor2 = Color.Red;
                neightborCountToolStripMenuItem.Checked = false;
            }
            graphicsPanel1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.cellcolor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            gridColor = Properties.Settings.Default.GridColor;
            gridColor1 = Properties.Settings.Default.GridColor1;

            universe = new bool[Properties.Settings.Default.UniverseX, Properties.Settings.Default.UniverseY];
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.cellcolor = cellColor;
            Properties.Settings.Default.TimerInterval = timer.Interval;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.GridColor1 = gridColor1;
            //bool[Properties.Settings.Default.UniverseX, Properties.Settings.Default.UniverseY] = universe;

            Properties.Settings.Default.Save();
        }
    }
}
