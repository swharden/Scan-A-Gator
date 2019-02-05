﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanAGator
{
    public partial class Form1 : Form
    {
        string VERSION = Properties.Resources.ResourceManager.GetString("VERSION");

        public Form1()
        {
            InitializeComponent();
            lblStatus.Text = $"Scan-A-Gator version {VERSION}";
        }

        // ACTIONS /////////////////////////////////////////////////////////////////

        public void SetStatus(string message)
        {
            Console.WriteLine(message);
            lblStatus.Text = message;
        }

        public string PathFolder;
        public void SelectFolder(string pathFolder = null)
        {

            // ask for the path with a dialog if one isn't given
            if (pathFolder == null)
            {
                var diag = new FolderBrowserDialog();
                if (diag.ShowDialog() == DialogResult.OK)
                    pathFolder = diag.SelectedPath;
                else
                    return;
            }

            // ensure the path is valid
            pathFolder = System.IO.Path.GetFullPath(pathFolder);
            if (!System.IO.Directory.Exists(pathFolder))
            {
                SetStatus($"ERROR - experiment folder does not exist: {pathFolder}");
                return;
            }
            PathFolder = pathFolder;
            lsFolder = null;

            // populate the list of subfolders
            string[] folders = System.IO.Directory.GetDirectories(pathFolder);
            for (int i = 0; i < folders.Length; i++)
                folders[i] = System.IO.Path.GetFileName(folders[i]);
            lbFolders.Items.Clear();
            lbFolders.Items.AddRange(folders);
            SetStatus($"{folders.Length} sub-folders found in {PathFolder}");
        }

        public LineScanFolder lsFolder;
        public void LoadLinescan(string pathLinescan)
        {
            ClearLinescan(); // clear the old data so we can cleanly exit at any time

            pathLinescan = System.IO.Path.GetFullPath(pathLinescan);
            if (!System.IO.Directory.Exists(pathLinescan))
            {
                Console.WriteLine($"ERROR - linescan folder does not exist: {pathLinescan}");
                return;
            }
            string folderName = System.IO.Path.GetFileName(pathLinescan);
            SetStatus($"Loading {folderName} ...");

            // all the parsing and data loading is handled here
            lsFolder = new LineScanFolder(pathLinescan);
            if (!lsFolder.IsValidLinescan)
            {
                SetStatus($"Error - Invalid linescan folder: {folderName}");
                return;
            }

            // by this point everything is valid, so just populate the GUI based on it.
            SetStatus($"Successfully loaded linescan folder: {folderName}");
            Text = $"Scan-A-Gator - {folderName}";
            pbRef.BackgroundImage = lsFolder.GetRepresentativeBitmap();
        }

        public void ClearLinescan()
        {
            pbRef.BackgroundImage = null;
            pbLinescan.BackgroundImage = null;
            Text = $"Scan-A-Gator";
            lsFolder = null;
        }

        // GUI BINDINGS /////////////////////////////////////////////////////////////////

        private void Form1_Load(object sender, EventArgs e)
        {
            SelectFolder("../../../../data/linescans/");
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/swharden/Scan-A-Gator");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbFolders.SelectedIndex < 0)
                return;
            string pathLinescan = System.IO.Path.Combine(PathFolder, lbFolders.SelectedItem.ToString());
            LoadLinescan(pathLinescan);
        }

        private void btnFolderSelect_Click(object sender, EventArgs e)
        {
            SelectFolder();
        }

        private void btnFolderRefresh_Click(object sender, EventArgs e)
        {
            SelectFolder(PathFolder);
        }
    }
}