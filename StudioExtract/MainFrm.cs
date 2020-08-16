using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Illusion;
using Illusion.Card;

namespace StudioExtract
{
    public partial class MainFrm : Form
    {
        #region Variables
        private CardExtractor extractor;
        private Dictionary<string, Image> sourceImages;
        private Dictionary<ICharaCard, Image> imagesCache;
        private Dictionary<ICharaCard, string> changeMap;
        private List<FileInfo> sourceFiles;

        private Color cardBkgColor = Color.FromArgb(126, 116, 146);
        private bool drawCardBkg = false;
        private bool drawCardFrame = true;
        private bool drawSceneImage = true;

        private SaveOptions saveOptions;
        #endregion

        #region Constructor
        public MainFrm()
        {
            InitializeComponent();

            extractor = new CardExtractor();
            sourceImages = new Dictionary<string, Image>();
            imagesCache = new Dictionary<ICharaCard, Image>();
            changeMap = new Dictionary<ICharaCard, string>();
            sourceFiles = new List<FileInfo>();
            buttonChangeImage.Enabled = false;
            buttonExtract.Enabled = false;

            saveOptions = new SaveOptions();

            LoadOptions();
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

        private void LoadOptions()
        {
            var settings = Properties.Settings.Default;
            string lastExtractPath = settings.lastExtractPath;
            if (string.IsNullOrEmpty(lastExtractPath))
            {
                lastExtractPath = Path.Combine(Assembly.GetExecutingAssembly().AssemblyDirectory(), "extract");
            }

            cardBkgColor = settings.backgroundColor;
            drawCardBkg = settings.drawBkgImage;
            drawCardFrame = settings.drawFrame;
            drawSceneImage = settings.drawSceneImage;

            textBoxOutput.Text = lastExtractPath;
            pictureBox.BackColor = cardBkgColor;
        }

        private void EnableControls(bool enabled)
        {
            listView.Enabled = enabled;
            panelBottom.Enabled = enabled;
        }

        private Image GenerateImage(string imageFilePath, int sex, bool drawBkg, bool drawFrame, bool drawImage)
        {
            Image result = null;
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
                    graphic.Clear(cardBkgColor);
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;

                    if (drawBkg)
                    {
                        graphic.DrawImage(Properties.Resources.card_bkg, imageBounds);
                    } 

                    if (drawImage)
                    {
                        graphic.DrawImage(image,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        new Rectangle(0, 0, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);
                    }

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

                    result = bitmap;
                }
            }
            return result;
        }

        private void SelectImage(ICharaCard card)
        {
            if (imagesCache.ContainsKey(card))
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
                    string imageFile = card.SourceFileName;
                    if (changeMap.ContainsKey(card))
                    {
                        imageFile = changeMap[card];
                    }
                    image = GenerateImage(imageFile, card.Sex, drawCardBkg, drawCardFrame, drawSceneImage);
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
        
        private void OnListItemSelect()
        {
            buttonChangeImage.Enabled = false;
            pictureBox.Image = null;
            textName.Text = "-";
            textSex.Text = "-";

            if (listView.SelectedItems.Count > 0)
            {
                var item = listView.SelectedItems[0];
                if (item.Tag != null)
                {
                    int index = (int)item.Tag;
                    if (index > -1)
                    {
                        var card = extractor.Cards[index];

                        SelectImage(card);
                        textName.Text = card.Name;
                        textSex.Text = card.Sex == 0 ? "Male" : "Female";

                        buttonChangeImage.Enabled = true;
                    }
                }
            }
        }

        private void DoExtract(int mode)
        {
            if (string.IsNullOrEmpty(textBoxOutput.Text))
            {
                buttonBrowse.PerformClick();
                DoExtract(mode);
                return;
            }

            List<ICharaCard> cards = new List<ICharaCard>();
            if (mode == 1)
            {
                for(int i = 0; i < listView.CheckedItems.Count; i++)
                {
                    var item = listView.CheckedItems[i];
                    if (item.Tag != null)
                    {
                        int index = (int)item.Tag;
                        if (index > -1) cards.Add(extractor.Cards[index]);
                    }
                }
            }
            else
            {
                cards = extractor.Cards;
            }

            if (cards.Count > 0)
            {
                var data = new ExtractData() 
                {
                    Cards = cards.ToArray(),
                    OutputPath = textBoxOutput.Text,
                    SaveOptions = saveOptions
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
            OnListItemSelect();
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag != null && (int)e.Item.Tag == -1)
            {
                e.Item.Checked = false;
                return;
            }

            if (listView.CheckedItems.Count == 0 || listView.CheckedItems.Count == listView.Items.Count)
            {
                buttonExtract.Text = "Extract All";
            }
            else
            {
                buttonExtract.Text = "Extract Selected";
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
                        string fileName = openFileDialogImage.FileName;
                        Image srcImage = Image.FromFile(fileName);
                        sourceImages.Add(fileName, srcImage);

                        if (changeMap.ContainsKey(card))
                        {
                            changeMap[card] = fileName;
                        }
                        else
                        {
                            changeMap.Add(card, fileName);
                        }

                        Image image = GenerateImage(fileName, card.Sex, drawCardBkg, drawCardFrame, drawSceneImage);
                        if (imagesCache.ContainsKey(card))
                        {
                            imagesCache[card] = image;
                        }
                        else
                        {
                            imagesCache.Add(card, image);
                        }
                        SelectImage(card);
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

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            var mode = listView.CheckedItems.Count == 0 || listView.CheckedItems.Count == listView.Items.Count;
            DoExtract(mode ? 0 : 1);
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
                        sourceImages.Add(file.FullName, Image.FromFile(file.FullName));
                    }
                    sourceFiles.Add(file);
                }

                e.Result = files;
            }
        }

        private void backgroundWorkerLoad_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listView.Groups.Clear();
            listView.Items.Clear();

            var groups = extractor.Cards.GroupBy(x => Path.GetFileNameWithoutExtension(x.SourceFileName));

            foreach (var f in sourceFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(f.FullName);
                var grp = groups.FirstOrDefault(x => x.Key == fileName);
                if (grp != null)
                {
                    var group = new ListViewGroup(grp.Key);
                    var emu = grp.GetEnumerator();
                    while (emu.MoveNext())
                    {
                        int idx = listView.Items.Count + 1;
                        var c = emu.Current;
                        var item = listView.Items.Add(new ListViewItem(idx.ToString(), group));
                        item.Tag = extractor.Cards.IndexOf(c);
                        item.SubItems.Add(c.Name);
                        item.SubItems.Add(c.Sex == 0 ? "Male" : "Female");
                        item.SubItems.Add(c.Game);
                        item.SubItems.Add(c.SourceFileName);
                    }

                    listView.Groups.Add(group);
                }
                else
                {
                    var group = new ListViewGroup(fileName);
                    var item = new ListViewItem("", group);
                    item.Tag = -1;
                    item.SubItems.Add("no charater found");
                    listView.Items.Add(item);
                    listView.Groups.Add(group);
                }
            }

            statusProgressBar.Style = ProgressBarStyle.Blocks;
            statusLabelStatus.Text = "Ready";

            buttonExtract.Enabled = listView.Items.Count > 0;
            EnableControls(true);
        }

        private void backgroundWorkerExtract_DoWork(object sender, DoWorkEventArgs e)
        {
            ExtractData data = e.Argument as ExtractData;
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
                        string fileName = card.GenerateFileName();
                        var charaFile = new FileInfo(Path.Combine(outDir.FullName, fileName));

                        int attempt = 0;
                        while (charaFile.Exists)
                        {
                            if (attempt > 0)
                            {
                                string newName = fileName.Replace(charaFile.Extension, $" ({attempt}){charaFile.Extension}");
                                charaFile = new FileInfo(Path.Combine(outDir.FullName, newName));
                            }
                            else
                            {
                                Thread.Sleep(100);
                                fileName = card.GenerateFileName();
                                charaFile = new FileInfo(Path.Combine(outDir.FullName, fileName));
                            }
                            attempt++;
                        }

                        Image image = null;
                        if (imagesCache.ContainsKey(card))
                        {
                            image = imagesCache[card];
                        }
                        else if (changeMap.ContainsKey(card))
                        {
                            image = GenerateImage(changeMap[card], card.Sex, drawCardBkg, drawCardFrame, drawSceneImage);
                        }
                        else
                        {
                            image = GenerateImage(card.SourceFileName, card.Sex, drawCardBkg, drawCardFrame, drawSceneImage);
                        }

                        if (image != null)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                image.Save(stream, ImageFormat.Png);
                                card.PngData = stream.ToArray();
                            }

                            card.Save(charaFile.Create(), data.SaveOptions);
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

            backgroundWorkerExtract.ReportProgress(100);
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
        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool enabled = listView.Items.Count > 0;

            extractAllToolStripMenuItem.Enabled = enabled;
            extractSelectedToolStripMenuItem.Enabled = enabled && listView.CheckedItems.Count > 0;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Illusion Scene File|*.png";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string[] fileList = openFileDialog.FileNames;
                LoadFiles(fileList);
            }
        }

        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoExtract(1);
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoExtract(0);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Edit Menu
        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool enabled = listView.Items.Count > 0;

            clearToolStripMenuItem.Enabled = enabled;
            selectAllToolStripMenuItem.Enabled = enabled;
            selectNoneToolStripMenuItem.Enabled = enabled;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView.Groups.Clear();
            listView.Items.Clear();

            extractor = null;
            extractor = new CardExtractor();

            pictureBox.Image = null;
            textName.Text = "-";
            textSex.Text = "-";
            buttonChangeImage.Enabled = false;
            buttonExtract.Enabled = false;

            sourceImages.Clear();
            imagesCache.Clear();
            sourceFiles.Clear();
            changeMap.Clear();
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
            var optionsDialog = new OptionsFrm();
            if (optionsDialog.ShowDialog(this) == DialogResult.OK)
            {
                Properties.Settings.Default.Reload();
                LoadOptions();
                imagesCache.Clear();

                OnListItemSelect();
            }
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
        private class ExtractData
        {
            #region Properties
            public ICharaCard[] Cards { get; set; }

            public string OutputPath { get; set; }

            public SaveOptions SaveOptions { get; set; }
            #endregion
        }
        #endregion
    }
}
