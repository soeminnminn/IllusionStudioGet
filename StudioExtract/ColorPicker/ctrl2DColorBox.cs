/*
 *  ctrl2DColorBox.cs
 *  
 *  Copyright (c) 2007-2010, OpenPainter.org, and based on the work of
 *                2005 Danny Blanchard (scrabcakes@gmail.com)
 *  
 *  The contents of this file are subject to the Mozilla Public License
 *  Version 1.1 (the "License"); you may not use this file except in
 *  compliance with the License. You may obtain a copy of the License at
 *  
 *  http://www.mozilla.org/MPL/
 *  
 *  Software distributed under the License is distributed on an "AS IS"
 *  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
 *  the License for the specific language governing rights and limitations
 *  under the License.
 *  
 *  The Original Code is OpenPainter.
 *  
 *  The Initial Developer of the Original Code is OpenPainter.org.
 *  All Rights Reserved.
 */

/******************************************************************/
/*****                                                        *****/
/*****     Project:           Adobe Color Picker Clone 1      *****/
/*****     Filename:          ctrl2DColorBox.cs               *****/
/*****     Original Author:   Danny Blanchard                 *****/
/*****                        - scrabcakes@gmail.com          *****/
/*****     Updates:	                                          *****/
/*****      3/28/2005 - Initial Version : Danny Blanchard     *****/
/*****      July 2010 - Updated by OpenPainter.org            *****/
/*****                                                        *****/
/******************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace OpenPainter.ColorPicker
{
	/// <summary>
	/// The 2D Square with a circle marker for selecting colors.
	/// </summary>
	internal class ctrl2DColorBox : System.Windows.Forms.UserControl
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ctrl2DColorBox"/> class.
        /// </summary>
		public ctrl2DColorBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			//	Initialize Colors
			_hsb = new AdobeColors.HSB();
			_hsb.H = 1.0;
			_hsb.S = 1.0;
			_hsb.B = 1.0;

			_rgb = AdobeColors.HSB_to_RGB(_hsb);
			
            _baseColorComponent = ColorComponent.Hue;
		}

        private ColorComponent _baseColorComponent = ColorComponent.Hue;
        /// <summary>
        /// Gets or sets the base color component which is fixed.
        /// </summary>
        public ColorComponent BaseColorComponent
        {
            get
            {
                return _baseColorComponent;
            }
            set
            {
                _baseColorComponent = value;

                // Redraw the control based on the new color component.
                ResetMarker(true);
                RedrawAll();
            }
        }

        private AdobeColors.HSB _hsb;
        /// <summary>
        /// Gets or sets the color in HSB mode. <see cref="RGB"/> property will be accordingly updated.
        /// </summary>
        public AdobeColors.HSB HSB
        {
            get
            {
                return _hsb;
            }
            set
            {
                _hsb = value;
                _rgb = AdobeColors.HSB_to_RGB(_hsb);

                //	Redraw the control based on the new color.
                ResetMarker(true);
                RedrawAll();
            }
        }

        private Color _rgb;
        /// <summary>
        /// Gets or sets the color in RGB mode. <see cref="HSB"/> property will be accordingly updated.
        /// </summary>
        public Color RGB
        {
            get
            {
                return _rgb;
            }
            set
            {
                _rgb = value;
                _hsb = AdobeColors.RGB_to_HSB(_rgb);

                //	Redraw the control based on the new color.
                ResetMarker(true);
                RedrawAll();
            }
        }

        private bool _webSafeColorsOnly = false;
        /// <summary>
        /// Gets or sets a boolean value that indicates where only the web colors are available.
        /// </summary>
        public bool WebSafeColorsOnly
        {
            get
            {
                return _webSafeColorsOnly;
            }
            set
            {
                _webSafeColorsOnly = value;
                RedrawAll();
            }
        }

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ctrl2DColorBox
			// 
			this.Name = "ctrl2DColorBox";
			this.Size = new System.Drawing.Size(260, 260);
		}

		#endregion

        #region User Input

        private int _markerX = 0;
        private int _markerY = 0;
        private bool _isDragging = false;
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button.HasFlag(MouseButtons.Left))	
            {
                _isDragging = true;

                MarkerMoved(e.X - 2, e.Y - 2);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging)
            {
                MarkerMoved(e.X - 2, e.Y - 2);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button.HasFlag(MouseButtons.Left) && _isDragging)
            {
                _isDragging = false;

                MarkerMoved(e.X - 2, e.Y - 2);
            }
		}

        private void MarkerMoved(int x, int y)
        {
            x = x.LimitInRange(0, 255);
            y = y.LimitInRange(0, 255);

            if (x == _markerX && y == _markerY)
            {
                //	If the marker hasn't moved, no need to redraw it.
                //	or send a scroll notification
                return;
            }

            // Redraw the marker.
            DrawMarker(x, y, true);
            // Reset the color.
            ResetHSLRGB();

            OnSelectionChanged(EventArgs.Empty);
        }

        #endregion

        #region Control Event Overrides

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RedrawAll();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RedrawAll();
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when the selected color has been changed.
        /// </summary>
		public event EventHandler SelectionChanged;

        /// <summary>
        /// Raises the <see cref="SelectionChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
            {
                EventHandler handler = SelectionChanged;
                handler(this, e);
            }
        }

		#endregion

		#region Rendering

		private void ClearMarker()
		{
            int x1 = _markerX - 5;
            int y1 = _markerY - 5;
            int x2 = _markerX + 5;
            int y2 = _markerY + 5;

            x1 = Math.Max(0, x1);
            y1 = Math.Max(0, y1);
            x2 = Math.Min(x2, 255);
            y2 = Math.Min(y2, 255);

            Rectangle rect = new Rectangle(
                x1, 
                y1, 
                x2 - x1 + 1, 
                y2 - y1 + 1);

            Bitmap map = GetColorPlaneBitmap(rect, _baseColorComponent);

            Graphics g = this.CreateGraphics();
            g.DrawImageUnscaled(map, x1 + 2, y1 + 2);

            map.Dispose();
		}

		private void DrawMarker(int x, int y, bool force)
		{
            x = x.LimitInRange(0, 255);
            y = y.LimitInRange(0, 255);

            if (_markerY == y && _markerX == x && !force)
            {
                return;
            }
                                                         
			ClearMarker();												

			_markerX = x;
			_markerY = y;

			Graphics g = this.CreateGraphics();

            //	The selected color determines the color of the marker drawn over
            //	it (black or white)
            Pen pen;
            AdobeColors.HSB _hsl = GetColor(x, y);	
            if (_hsl.B < (double)200 / 255)
            {
                pen = new Pen(Color.White);									//	White marker if selected color is dark
            }
            else if (_hsl.H < (double)26 / 360 || _hsl.H > (double)200 / 360)
            {
                if (_hsl.S > (double)70 / 255)
                {
                    pen = new Pen(Color.White);
                }
                else
                {
                    pen = new Pen(Color.Black);								//	Else use a black marker for lighter colors
                }
            }
            else
            {
                pen = new Pen(Color.Black);
            }

			g.DrawEllipse(pen, x - 3, y - 3, 10, 10);						//	Draw the marker : 11 x 11 circle

			DrawBorder();		//	Force the border to be redrawn, just in case the marker has been drawn over it.
		}

        private void DrawBorder()
        {
            Graphics g = this.CreateGraphics();

            Pen pencil;

            //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
            //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
            //	inside the gray/white rectangle

            pencil = new Pen(Color.FromArgb(172, 168, 153));	//	The same gray color used by Photoshop
            g.DrawLine(pencil, this.Width - 2, 0, 0, 0);	//	Draw top line
            g.DrawLine(pencil, 0, 0, 0, this.Height - 2);	//	Draw left hand line

            pencil = new Pen(Color.White);
            g.DrawLine(pencil, this.Width - 1, 0, this.Width - 1, this.Height - 1);	//	Draw right hand line
            g.DrawLine(pencil, this.Width - 1, this.Height - 1, 0, this.Height - 1);	//	Draw bottome line

            pencil = new Pen(Color.Black);
            g.DrawRectangle(pencil, 1, 1, this.Width - 3, this.Height - 3);	//	Draw inner black rectangle
        }

		private void DrawContent()
		{
            Rectangle rect = new Rectangle(0, 0, 256, 256);
            Bitmap map = GetColorPlaneBitmap(rect, _baseColorComponent);

            Graphics g = this.CreateGraphics();
            g.DrawImageUnscaled(map, 2, 2);

            map.Dispose();
		}

        private Bitmap GetColorPlaneBitmap(Rectangle rect, ColorComponent comp)
        {
            Bitmap map = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            BitmapData mapData = map.LockBits(
                new Rectangle(0, 0, map.Width, map.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* pt0 = (byte*)mapData.Scan0;

                Parallel.For(rect.Top, rect.Bottom, y =>
                {
                    int bitmapY = y - rect.Top;

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        int bitmapX = x - rect.Left;

                        Color color;
                        switch (comp)
                        {
                            case ColorComponent.Hue:
                                color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                    _hsb.H,
                                    x / 255.0,
                                    1 - y / 255.0));
                                break;

                            case ColorComponent.Saturation:
                                color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                    x / 255.0,
                                    _hsb.S,
                                    1 - y / 255.0));
                                break;

                            case ColorComponent.Brightness:
                                color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                    x / 255.0,
                                    1 - y / 255.0,
                                    _hsb.B));
                                break;

                            case ColorComponent.Red:
                                color = Color.FromArgb(
                                    _rgb.R,
                                    x,
                                    255 - y);
                                break;

                            case ColorComponent.Green:
                                color = Color.FromArgb(
                                    x,
                                    _rgb.G,
                                    255 - y);
                                break;

                            case ColorComponent.Blue:
                                color = Color.FromArgb(
                                    x,
                                    255 - y,
                                    _rgb.B);
                                break;

                            default:
                                throw new ArgumentException();
                        }

                        if (_webSafeColorsOnly)
                        {
                            color = AdobeColors.GetNearestWebSafeColor(color);
                        }

                        byte* pt = pt0 + mapData.Stride * bitmapY + 3 * bitmapX;
                        pt[2] = color.R;
                        pt[1] = color.G;
                        pt[0] = color.B;
                    }
                });
            }

            map.UnlockBits(mapData);
            return map;
        }

		private void RedrawAll()
		{
			DrawBorder();
            DrawContent();
			DrawMarker(_markerX, _markerY, true);
		}

        private void ResetMarker(bool redraw)
        {
            switch (_baseColorComponent)
            {
                case ColorComponent.Hue:
                    _markerX = (int)Math.Round(255 * _hsb.S);
                    _markerY = (int)Math.Round(255 * (1.0 - _hsb.B));
                    break;

                case ColorComponent.Saturation:
                    _markerX = (int)Math.Round(255 * _hsb.H);
                    _markerY = (int)Math.Round(255 * (1.0 - _hsb.B));
                    break;

                case ColorComponent.Brightness:
                    _markerX = (int)Math.Round(255 * _hsb.H);
                    _markerY = (int)Math.Round(255 * (1.0 - _hsb.S));
                    break;

                case ColorComponent.Red:
                    _markerX = _rgb.B;
                    _markerY = 255 - _rgb.G;
                    break;

                case ColorComponent.Green:
                    _markerX = _rgb.B;
                    _markerY = 255 - _rgb.R;
                    break;

                case ColorComponent.Blue:
                    _markerX = _rgb.R;
                    _markerY = 255 - _rgb.G;
                    break;
            }

            if (redraw)
            {
                DrawMarker(_markerX, _markerY, true);
            }
        }

        private void ResetHSLRGB()
        {
            int red, 
                green, 
                blue;

            switch (_baseColorComponent)
            {
                case ColorComponent.Hue:
                    _hsb.S = _markerX / 255.0;
                    _hsb.B = 1.0 - _markerY / 255.0;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;

                case ColorComponent.Saturation:
                    _hsb.H = _markerX / 255.0;
                    _hsb.B = 1.0 - _markerY / 255.0;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;

                case ColorComponent.Brightness:
                    _hsb.H = _markerX / 255.0;
                    _hsb.S = 1.0 - _markerY / 255.0;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;

                case ColorComponent.Red:
                    blue = _markerX;
                    green = 255 - _markerY;
                    _rgb = Color.FromArgb(_rgb.R, green, blue);
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;

                case ColorComponent.Green:
                    blue = _markerX;
                    red = 255 - _markerY;
                    _rgb = Color.FromArgb(red, _rgb.G, blue);
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;

                case ColorComponent.Blue:
                    red = _markerX;
                    green = 255 - _markerY;
                    _rgb = Color.FromArgb(red, green, _rgb.B);
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;
            }
        }

		private AdobeColors.HSB GetColor(int x, int y)
		{
			AdobeColors.HSB _hsb = new AdobeColors.HSB();

            switch (_baseColorComponent)
            {
                case ColorComponent.Hue:
                    _hsb.H = _hsb.H;
                    _hsb.S = x / 255.0;
                    _hsb.B = 1.0 - y / 255.0;
                    break;

                case ColorComponent.Saturation:
                    _hsb.S = _hsb.S;
                    _hsb.H = x / 255.0;
                    _hsb.B = 1.0 - (double)y / 255;
                    break;

                case ColorComponent.Brightness:
                    _hsb.B = _hsb.B;
                    _hsb.H = x / 255.0;
                    _hsb.S = 1.0 - (double)y / 255;
                    break;

                case ColorComponent.Red:
                    _hsb = AdobeColors.RGB_to_HSB(Color.FromArgb(_rgb.R, 255 - y, x));
                    break;

                case ColorComponent.Green:
                    _hsb = AdobeColors.RGB_to_HSB(Color.FromArgb(255 - y, _rgb.G, x));
                    break;

                case ColorComponent.Blue:
                    _hsb = AdobeColors.RGB_to_HSB(Color.FromArgb(x, 255 - y, _rgb.B));
                    break;
            }

			return _hsb;
		}

		#endregion
    }
}
