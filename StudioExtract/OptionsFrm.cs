using System;
using System.Windows.Forms;
using OpenPainter.ColorPicker;

namespace StudioExtract
{
    public partial class OptionsFrm : Form
    {
        #region Properties
        #endregion

        #region Constructor
        public OptionsFrm()
        {
            InitializeComponent();

            LoadOptions();
        }
        #endregion

        #region Methods
        private void LoadOptions()
        {
            boxBkgColor.BackColor = Properties.Settings.Default.backgroundColor;
            chkBkgImage.Checked = Properties.Settings.Default.drawBkgImage;
            chkFrame.Checked = Properties.Settings.Default.drawFrame;
            chkScene.Checked = Properties.Settings.Default.drawSceneImage;
        }

        private void SaveOptions()
        {
            Properties.Settings.Default.backgroundColor = boxBkgColor.BackColor;
            Properties.Settings.Default.drawBkgImage = chkBkgImage.Checked;
            Properties.Settings.Default.drawFrame = chkFrame.Checked;
            Properties.Settings.Default.drawSceneImage = chkScene.Checked;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Events
        private void btnBkgColorChange_Click(object sender, EventArgs e)
        {
            var colorPicker = new frmColorPicker(boxBkgColor.BackColor);
            if (colorPicker.ShowDialog(this) == DialogResult.OK)
            {
                boxBkgColor.BackColor = colorPicker.PrimaryColor;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveOptions();
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}
