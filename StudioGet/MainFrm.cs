using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Illusion;
using Illusion.Card;

namespace StudioGet
{
    public partial class MainFrm : Form
    {
        #region Variables
        private CardExtractor extractor;
        private Dictionary<ICharaCard, Image> imagesCache;

        private Color defBkgColor = Color.FromArgb(126, 116, 146);
        private bool drawCardBkg = false;
        private bool drawCardFrame = true;
        #endregion

        #region Constructor
        public MainFrm()
        {
            InitializeComponent();

            extractor = new CardExtractor();
            imagesCache = new Dictionary<ICharaCard, Image>();

            string lastExtractPath = Properties.Settings.Default.lastExtractPath;
            if (string.IsNullOrEmpty(lastExtractPath))
            {
                lastExtractPath = Path.Combine(Assembly.GetExecutingAssembly().AssemblyDirectory(), "extract");
            }

            textBoxOutput.Text = lastExtractPath;
            buttonChangeImage.Enabled = false;
            buttonExport.Enabled = false;
        }
        #endregion

        #region Methods
        protected override void OnClosing(CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxOutput.Text))
            {
                Properties.Settings.Default.lastExtractPath = textBoxOutput.Text;
                Properties.Settings.Default.Save();
            }
            base.OnClosing(e);
        }

        private void EnableControls(bool enabled)
        {
            listView.Enabled = enabled;
            panelBottom.Enabled = enabled;
        }

        private Bitmap ResizeBitmap(Image image, int width, int height)
        {
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int destX = 0;
            int destY = 0;

            float nPercent;

            float nPercentW = ((float)width / (float)sourceWidth);
            float nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width -
                            (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height -
                            (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            using (Graphics graphic = Graphics.FromImage(bmPhoto))
            {
                graphic.Clear(Color.White);

                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.DrawImage(image,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(0, 0, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);
            }

            return bmPhoto;
        }

        private byte[] GenerateImage(string imageFilePath, int sex, bool drawBkg, bool drawFrame)
        {
            if (!string.IsNullOrEmpty(imageFilePath))
            {
                var image = Image.FromFile(imageFilePath);
                var width = 252;
                var height = 352;

                var imageBounds = new Rectangle(0, 0, width, height);

                int sourceWidth = image.Width;
                int sourceHeight = image.Height;
                int destX = 0;
                int destY = 0;

                float nPercent;

                float nPercentW = ((float)width / (float)sourceWidth);
                float nPercentH = ((float)height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = Convert.ToInt16((width -
                                (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = Convert.ToInt16((height -
                                (sourceHeight * nPercent)) / 2);
                }

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                using (var graphic = Graphics.FromImage(bitmap))
                {
                    graphic.Clear(defBkgColor);
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;

                    if (drawBkg)
                    {
                        graphic.DrawImage(Properties.Resources.card_bkg, imageBounds);
                    } 

                    graphic.DrawImage(image,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        new Rectangle(0, 0, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);

                    if (drawFrame)
                    {
                        graphic.DrawImage(Properties.Resources.card_frame, imageBounds);
                    }

                    var iconBounds = new Rectangle(124, 224, 128, 128);
                    switch (sex)
                    {
                        case 0:
                            graphic.DrawImage(Properties.Resources.chara_male, iconBounds);
                            break;
                        case 1:
                            graphic.DrawImage(Properties.Resources.chara_female, iconBounds);
                            break;
                        case 2:
                            graphic.DrawImage(Properties.Resources.card_coord, iconBounds);
                            break;
                        default:
                            break;
                    }

                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            return null;
        }

        private void SelectImage(ICharaCard card, bool forceChange)
        {
            if (!forceChange && imagesCache.ContainsKey(card))
            {
                pictureBox.Image = imagesCache[card];
            }
            else
            {
                Image image;
                if (card.PngData != null && card.PngData.Length > 0)
                {
                    image = Image.FromStream(new MemoryStream(card.PngData));
                }
                else
                {
                    var resName = (card.Sex == 0) ? "male.png" : "female.png";
                    var resStream = Assembly.GetExecutingAssembly().OpenManifestResourceStream(resName);

                    image = Image.FromStream(resStream);
                }

                if (imagesCache.ContainsKey(card))
                {
                    imagesCache[card] = image;
                }
                else
                {
                    imagesCache.Add(card, image);
                }
                pictureBox.Image = image;
            }
        }

        private void LoadFiles(string[] fileList)
        {
            if (fileList.Length > 0)
            {
                EnableControls(false);
                statusLabelStatus.Text = "Loading ...";
                statusProgressBar.Style = ProgressBarStyle.Marquee;

                backgroundWorkerLoad.RunWorkerAsync(fileList);
            }
        }
        
        private void DoExport(int mode, bool exportCoord)
        {
            if (string.IsNullOrEmpty(textBoxOutput.Text))
            {
                buttonBrowse.PerformClick();
                DoExport(mode, exportCoord);
                return;
            }

            List<ICharaCard> cards = new List<ICharaCard>();
            if (mode == 1)
            {
                for(int i = 0; i < listView.SelectedIndices.Count; i++)
                {
                    int index = listView.SelectedIndices[i];
                    cards.Add(extractor.Cards[index]);
                }
            }
            else
            {
                cards = extractor.Cards;
            }

            if (cards.Count > 0)
            {
                var data = new ExportData() 
                {
                    Cards = cards.ToArray(),
                    OutputPath = textBoxOutput.Text,
                    ExportCoordinate = exportCoord
                };

                EnableControls(false);
                statusLabelStatus.Text = "Exporting ...";
                backgroundWorkerExtract.RunWorkerAsync(data);
            }
        }
        #endregion

        #region Events

        #region ListView
        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                var fileList = files.Where(f => f.ToLowerInvariant().EndsWith(".png")).ToArray();
                LoadFiles(fileList);
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                int index = listView.SelectedIndices[0];
                var card = extractor.Cards[index];

                SelectImage(card, false);
                textName.Text = card.Name;
                textSex.Text = card.Sex == 0 ? "Male" : "Female";

                buttonChangeImage.Enabled = true;
            }
            else
            {
                buttonChangeImage.Enabled = false;
            }
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (listView.CheckedItems.Count == 0 || listView.CheckedItems.Count == listView.Items.Count)
            {
                buttonExport.Text = "Extract All";
            }
            else
            {
                buttonExport.Text = "Extract Selected";
            } 
        }
        #endregion

        #region Buttons
        private void buttonChangeImage_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count > 0)
            {
                var card = extractor.Cards[listView.SelectedIndices[0]];

                if (card != null)
                {
                    openFileDialogImage.Filter = "Image File|*.png;*.jpg;*.jpeg;*.bmp";
                    if (openFileDialogImage.ShowDialog(this) == DialogResult.OK)
                    {
                        card.PngData = GenerateImage(openFileDialogImage.FileName, card.Sex, drawCardBkg, drawCardFrame);
                        SelectImage(card, true);
                    }
                }
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxOutput.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            var mode = listView.CheckedItems.Count == 0 || listView.CheckedItems.Count == listView.Items.Count;
            DoExport(mode ? 0 : 1, checkBoxCoord.Checked);
        }
        #endregion

        #region BackgroundWorker
        private void backgroundWorkerLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = e.Argument as string[];

            if (files != null)
            {
                foreach (var f in files)
                {
                    var file = new FileInfo(f);
                    if (extractor.TryParse(file))
                    {
                        foreach (var card in extractor.Cards)
                        {
                            if (card.PngData == null || card.PngData.Length == 0)
                            {
                                card.PngData = GenerateImage(card.SourceFileName, card.Sex, drawCardBkg, drawCardFrame);
                            }
                        }
                    }
                }
            }
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listView.Groups.Clear();
            listView.Items.Clear();

            if (extractor.Cards.Count > 0)
            {
                var groups = extractor.Cards.GroupBy(x => Path.GetFileNameWithoutExtension(x.SourceFileName));

                foreach (var g in groups)
                {
                    var group = new ListViewGroup(g.Key);
                    var emu = g.GetEnumerator();
                    while (emu.MoveNext())
                    {
                        var c = emu.Current;
                        var item = listView.Items.Add(new ListViewItem(listView.Items.Count.ToString(), group));
                        item.SubItems.Add(c.Name);
                        item.SubItems.Add(c.Sex == 0 ? "Male" : "Female");
                        item.SubItems.Add(c.SourceFileName);
                    }

                    listView.Groups.Add(group);
                }
            }

            statusProgressBar.Style = ProgressBarStyle.Blocks;
            statusLabelStatus.Text = "Ready";

            buttonExport.Enabled = listView.Items.Count > 0;
            EnableControls(true);
        }

        private void backgroundWorkerExtract_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportData data = e.Argument as ExportData;
            if (data != null)
            {
                try
                {
                    var outDir = new DirectoryInfo(data.OutputPath);
                    if (!outDir.Exists)
                    {
                        outDir.Create();
                    }

                    for(int i = 0; i < data.Cards.Length; i++)
                    {
                        var card = data.Cards[i];
                        string fileName = card.GenerateFileName(CardTypes.Charater);
                        var charaFile = new FileInfo(Path.Combine(outDir.FullName, fileName));

                        card.Save(charaFile.Create());

                        if (data.ExportCoordinate)
                        {
                            fileName = card.GenerateFileName(CardTypes.Coordinate);
                            charaFile = new FileInfo(Path.Combine(outDir.FullName, fileName));

                            card.SaveCoordinate(charaFile.Create());
                        }

                        int progress = (int)(((float)i / (float)data.Cards.Length) * 100f);
                        backgroundWorkerExtract.ReportProgress(progress);
                    }
                }
                catch(Exception ex)
                {
                    e.Result = ex;
                }
            }
        }

        private void backgroundWorkerExtract_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorkerExtract_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Result is Exception)
            {
                MessageBox.Show(this, "Something wrong, can not export cards.", this.Text + " - Error", MessageBoxButtons.OK);
            }
            statusLabelStatus.Text = "Ready";
            EnableControls(true);
        }
        #endregion

        #region File Menu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "AI/HS2 Scene File|*.png";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string[] fileList = openFileDialog.FileNames;
                LoadFiles(fileList);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
        }

        private void exportSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoExport(1, checkBoxCoord.Checked);
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoExport(0, checkBoxCoord.Checked);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Edit Menu
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView.Groups.Clear();
            listView.Items.Clear();

            extractor = new CardExtractor();

            pictureBox.Image = null;
            textName.Text = "-";
            textSex.Text = "-";
            buttonChangeImage.Enabled = false;
            buttonExport.Enabled = false;

            imagesCache.Clear();
        }
        
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = true;
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = false;
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
        }
        #endregion

        #region Help Menu
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }
        #endregion

        #endregion

        #region Nested Types
        private class ExportData
        {
            #region Properties
            public ICharaCard[] Cards { get; set; }

            public string OutputPath { get; set; }

            public bool ExportCoordinate { get; set; }
            #endregion
        }
        #endregion
    }
}
