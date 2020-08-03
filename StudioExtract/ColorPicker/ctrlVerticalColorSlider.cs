/*
 *  ctrlVerticalColorSlider.cs
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
/*****     Filename:          ctrlVerticalColorSlider.cs      *****/
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
using System.Data;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace OpenPainter.ColorPicker
{
	/// <summary>
	/// A vertical slider control that shows a range for a color property (a.k.a. Hue, Saturation, Brightness,
	/// Red, Green, Blue) and sends an event when the slider is changed.
	/// </summary>
	public class ctrlVerticalColorSlider : System.Windows.Forms.UserControl
	{
		public ctrlVerticalColorSlider()
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

				//	Redraw the control based on the new ColorComponent
				ResetSlider(true);
				RedrawAll();
			}
		}

        private AdobeColors.HSB	_hsb;
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
				ResetSlider(true);
				DrawContent();
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
                ResetSlider(true);
                DrawContent();
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ctrl1DColorBar
			// 
			this.Name = "ctrl1DColorBar";
			this.Size = new System.Drawing.Size(40, 264);
		}

		#endregion

        #region User Input

        private int _markerStartY = 0;
        private bool _isDragging = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button.HasFlag(MouseButtons.Left))
            {
                _isDragging = true;

                SliderMoved(e.Y);
            }
		}

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragging)
            {
                SliderMoved(e.Y);
            }
		}

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button.HasFlag(MouseButtons.Left))
            {
                _isDragging = false;

                SliderMoved(e.Y);
            }
		}

        private void SliderMoved(int y)
        {
            y -= 4;
            y = y.LimitInRange(0, this.Height - 9);

            if (y == _markerStartY)
            {
                return;
            }

            DrawSlider(y, false);	//	Redraw the slider
            ResetHSLRGB();			//	Reset the color

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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RedrawAll();
		}

		#endregion

		#region Rendering

		/// <summary>
		/// Redraws the background over the slider area on both sides of the control
		/// </summary>
		private void ClearSlider()
		{
			Graphics g = this.CreateGraphics();

			Brush brush = System.Drawing.SystemBrushes.Control;
			g.FillRectangle(brush, 0, 0, 8, this.Height);				//	clear left hand slider
			g.FillRectangle(brush, this.Width - 8, 0, 8, this.Height);	//	clear right hand slider
		}

		/// <summary>
		/// Draws the slider arrows on both sides of the control.
		/// </summary>
		/// <param name="position">position value of the slider, lowest being at the bottom.  The range
		/// is between 0 and the controls height-9.  The values will be adjusted if too large/small</param>
		/// <param name="Unconditional">If Unconditional is true, the slider is drawn, otherwise some logic 
		/// is performed to determine is drawing is really neccessary.</param>
        private void DrawSlider(int position, bool force)
        {
            position = position.LimitInRange(0, this.Height - 9);

            if (_markerStartY == position && !force)
            {
                return;
            }

            // Update the controls marker position.
            _markerStartY = position;	

            // Remove old slider.
            this.ClearSlider();		

            Graphics g = this.CreateGraphics();

            Pen pencil = new Pen(Color.FromArgb(116, 114, 106));
            Brush brush = Brushes.White;

            Point[] arrow = new Point[7];				//	 GGG
            arrow[0] = new Point(1, position);			//	G   G
            arrow[1] = new Point(3, position);			//	G    G
            arrow[2] = new Point(7, position + 4);		//	G     G
            arrow[3] = new Point(3, position + 8);		//	G      G
            arrow[4] = new Point(1, position + 8);		//	G     G
            arrow[5] = new Point(0, position + 7);		//	G    G
            arrow[6] = new Point(0, position + 1);		//	G   G
                                                        //	 GGG

            g.FillPolygon(brush, arrow);	//	Fill left arrow with white
            g.DrawPolygon(pencil, arrow);	//	Draw left arrow border with gray

                                                                //	    GGG
            arrow[0] = new Point(this.Width - 2, position);		//	   G   G
            arrow[1] = new Point(this.Width - 4, position);		//	  G    G
            arrow[2] = new Point(this.Width - 8, position + 4);	//	 G     G
            arrow[3] = new Point(this.Width - 4, position + 8);	//	G      G
            arrow[4] = new Point(this.Width - 2, position + 8);	//	 G     G
            arrow[5] = new Point(this.Width - 1, position + 7);	//	  G    G
            arrow[6] = new Point(this.Width - 1, position + 1);	//	   G   G
            //	    GGG

            g.FillPolygon(brush, arrow);	//	Fill right arrow with white
            g.DrawPolygon(pencil, arrow);	//	Draw right arrow border with gray
        }

		/// <summary>
		/// Draws the border around the control, in this case the border around the content area between
		/// the slider arrows.
		/// </summary>
        private void DrawBorder()
        {
            Graphics g = this.CreateGraphics();

            Pen pencil;

            //	To make the control look like Adobe Photoshop's the border around the control will be a gray line
            //	on the top and left side, a white line on the bottom and right side, and a black rectangle (line) 
            //	inside the gray/white rectangle

            pencil = new Pen(Color.FromArgb(172, 168, 153));	//	The same gray color used by Photoshop
            g.DrawLine(pencil, this.Width - 10, 2, 9, 2);	//	Draw top line
            g.DrawLine(pencil, 9, 2, 9, this.Height - 4);	//	Draw left hand line

            pencil = new Pen(Color.White);
            g.DrawLine(pencil, this.Width - 9, 2, this.Width - 9, this.Height - 3);	//	Draw right hand line
            g.DrawLine(pencil, this.Width - 9, this.Height - 3, 9, this.Height - 3);	//	Draw bottome line

            pencil = new Pen(Color.Black);
            g.DrawRectangle(pencil, 10, 3, this.Width - 20, this.Height - 7);	//	Draw inner black rectangle
        }

		/// <summary>
		/// Evaluates the DrawStyle of the control and calls the appropriate
		/// drawing function for content
		/// </summary>
		private void DrawContent()
		{
            Rectangle rect = new Rectangle(
                0,
                0,
                this.Width - 21,
                this.Height - 8);
            Bitmap map = GetColorStripBitmap(rect, (ColorComponent)_baseColorComponent);

            Graphics g = this.CreateGraphics();
            g.DrawImageUnscaled(map, 11, 4);

            map.Dispose();
        }

        private Bitmap GetColorStripBitmap(Rectangle rect, ColorComponent comp)
        {
            Bitmap map = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            BitmapData mapData = map.LockBits(
                new Rectangle(0, 0, map.Width, map.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            int height = this.Height - 8;

            unsafe
            {
                byte* pt0 = (byte*)mapData.Scan0;

                Parallel.For(rect.Top, rect.Bottom, y =>
                {
                    int bitmapY = y - rect.Top;

                    Color color;
                    switch (comp)
                    {
                        case ColorComponent.Hue:
                            color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                1.0 - (double)y / height,
                                1,
                                1));
                            break;

                        case ColorComponent.Saturation:
                            color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                _hsb.H,
                                1.0 - (double)y / height,
                                _hsb.B));
                            break;

                        case ColorComponent.Brightness:
                            color = AdobeColors.HSB_to_RGB(new AdobeColors.HSB(
                                _hsb.H,
                                _hsb.S,
                                1.0 - (double)y / height));
                            break;

                        case ColorComponent.Red:
                            int red = 255 - (int)Math.Round(255 * (double)y / height);
                            color = Color.FromArgb(
                                red,
                                _rgb.G,
                                _rgb.B);
                            break;

                        case ColorComponent.Green:
                            int green = 255 - (int)Math.Round(255 * (double)y / height);
                            color = Color.FromArgb(
                                _rgb.R,
                                green,
                                _rgb.B);
                            break;

                        case ColorComponent.Blue:
                            int blue = 255 - (int)Math.Round(255 * (double)y / height);
                            color = Color.FromArgb(
                                _rgb.R,
                                _rgb.G,
                                blue);
                            break;

                        default:
                            throw new ArgumentException();
                    }

                    if (_webSafeColorsOnly)
                    {
                        color = AdobeColors.GetNearestWebSafeColor(color);
                    }

                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        int bitmapX = x - rect.Left;

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
			DrawSlider(_markerStartY, true);
			DrawBorder();
            DrawContent();
		}

		/// <summary>
		/// Resets the vertical position of the slider to match the controls color.  Gives the option of redrawing the slider.
		/// </summary>
		/// <param name="redraw">Set to true if you want the function to redraw the slider after determining the best position</param>
        private void ResetSlider(bool redraw)
        {
            int height = this.Height - 8;

            switch (_baseColorComponent)
            {
                case ColorComponent.Hue:
                    _markerStartY = height - (int)Math.Round(height * _hsb.H);
                    break;
                case ColorComponent.Saturation:
                    _markerStartY = height - (int)Math.Round(height * _hsb.S);
                    break;
                case ColorComponent.Brightness:
                    _markerStartY = height - (int)Math.Round(height * _hsb.B);
                    break;
                case ColorComponent.Red:
                    _markerStartY = height - (int)Math.Round(height * _rgb.R / 255.0);
                    break;
                case ColorComponent.Green:
                    _markerStartY = height - (int)Math.Round(height * _rgb.G / 255.0);
                    break;
                case ColorComponent.Blue:
                    _markerStartY = height - (int)Math.Round(height * _rgb.B / 255.0);
                    break;
            }

            if (redraw)
            {
                DrawSlider(_markerStartY, true);
            }
        }


		/// <summary>
		/// Resets the controls color (both HSL and RGB variables) based on the current slider position
		/// </summary>
        private void ResetHSLRGB()
        {
            int height = this.Height - 9;

            switch (_baseColorComponent)
            {
                case ColorComponent.Hue:
                    _hsb.H = 1.0 - (double)_markerStartY / height;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;
                case ColorComponent.Saturation:
                    _hsb.S = 1.0 - (double)_markerStartY / height;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;
                case ColorComponent.Brightness:
                    _hsb.B = 1.0 - (double)_markerStartY / height;
                    _rgb = AdobeColors.HSB_to_RGB(_hsb);
                    break;
                case ColorComponent.Red:
                    _rgb = Color.FromArgb(255 - (int)Math.Round(255 * (double)_markerStartY / height), _rgb.G, _rgb.B);
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;
                case ColorComponent.Green:
                    _rgb = Color.FromArgb(_rgb.R, 255 - (int)Math.Round(255 * (double)_markerStartY / height), _rgb.B);
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;
                case ColorComponent.Blue:
                    _rgb = Color.FromArgb(_rgb.R, _rgb.G, 255 - (int)Math.Round(255 * (double)_markerStartY / height));
                    _hsb = AdobeColors.RGB_to_HSB(_rgb);
                    break;
            }
        }

		#endregion
	}
}
