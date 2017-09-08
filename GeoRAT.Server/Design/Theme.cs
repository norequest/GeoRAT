using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

// Xylos Theme.
// Made by AeroRev9.
// 9 Controls.

internal sealed class Helpers
{
    public enum RoundingStyle : byte
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        TopRight,
        BottomRight
    }

    public static void CenterString(Graphics g, string T, Font f, Color c, Rectangle r)
    {
        SizeF sizeF = g.MeasureString(T, f);
        using (SolidBrush solidBrush = new SolidBrush(c))
        {
            g.DrawString(T, f, solidBrush, checked(new Point((int)Math.Round(unchecked((double)r.Width / 2.0 - (double)(sizeF.Width / 2f))), (int)Math.Round(unchecked((double)r.Height / 2.0 - (double)(sizeF.Height / 2f))))));
        }
    }

    public static Color ColorFromHex(string hex)
    {
        return Color.FromArgb(checked((int)long.Parse(string.Format("FFFFFFFFFF{0}", hex.Substring(1)), NumberStyles.HexNumber)));
    }

    public static Rectangle FullRectangle(Size s, bool subtract)
    {
        Rectangle result;
        if (subtract)
        {
            result = checked(new Rectangle(0, 0, s.Width - 1, s.Height - 1));
        }
        else
        {
            result = new Rectangle(0, 0, s.Width, s.Height);
        }
        return result;
    }

    public static GraphicsPath RoundRect(Rectangle rect, int rounding, Helpers.RoundingStyle style = Helpers.RoundingStyle.All)
    {
        GraphicsPath graphicsPath = new GraphicsPath();
        checked
        {
            int num = rounding * 2;
            graphicsPath.StartFigure();
            bool flag = rounding == 0;
            GraphicsPath result;
            if (flag)
            {
                graphicsPath.AddRectangle(rect);
                graphicsPath.CloseAllFigures();
                result = graphicsPath;
            }
            else
            {
                switch (style)
                {
                    case Helpers.RoundingStyle.All:
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Y, num, num), -180f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Y, num, num), -90f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Height - num + rect.Y, num, num), 0f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Height - num + rect.Y, num, num), 90f, 90f);
                        break;
                    case Helpers.RoundingStyle.Top:
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Y, num, num), -180f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Y, num, num), -90f, 90f);
                        graphicsPath.AddLine(new Point(rect.X + rect.Width, rect.Y + rect.Height), new Point(rect.X, rect.Y + rect.Height));
                        break;
                    case Helpers.RoundingStyle.Bottom:
                        graphicsPath.AddLine(new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y));
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Height - num + rect.Y, num, num), 0f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Height - num + rect.Y, num, num), 90f, 90f);
                        break;
                    case Helpers.RoundingStyle.Left:
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Y, num, num), -180f, 90f);
                        graphicsPath.AddLine(new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                        graphicsPath.AddArc(new Rectangle(rect.X, rect.Height - num + rect.Y, num, num), 90f, 90f);
                        break;
                    case Helpers.RoundingStyle.Right:
                        graphicsPath.AddLine(new Point(rect.X, rect.Y + rect.Height), new Point(rect.X, rect.Y));
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Y, num, num), -90f, 90f);
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Height - num + rect.Y, num, num), 0f, 90f);
                        break;
                    case Helpers.RoundingStyle.TopRight:
                        graphicsPath.AddLine(new Point(rect.X, rect.Y + 1), new Point(rect.X, rect.Y));
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Y, num, num), -90f, 90f);
                        graphicsPath.AddLine(new Point(rect.X + rect.Width, rect.Y + rect.Height - 1), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                        graphicsPath.AddLine(new Point(rect.X + 1, rect.Y + rect.Height), new Point(rect.X, rect.Y + rect.Height));
                        break;
                    case Helpers.RoundingStyle.BottomRight:
                        graphicsPath.AddLine(new Point(rect.X, rect.Y + 1), new Point(rect.X, rect.Y));
                        graphicsPath.AddLine(new Point(rect.X + rect.Width - 1, rect.Y), new Point(rect.X + rect.Width, rect.Y));
                        graphicsPath.AddArc(new Rectangle(rect.Width - num + rect.X, rect.Height - num + rect.Y, num, num), 0f, 90f);
                        graphicsPath.AddLine(new Point(rect.X + 1, rect.Y + rect.Height), new Point(rect.X, rect.Y + rect.Height));
                        break;
                }
                graphicsPath.CloseAllFigures();
                result = graphicsPath;
            }
            return result;
        }
    }
}
public class XylosTabControl : TabControl
{
    private Graphics _g;

    private Rectangle _rect;

    private int _overIndex;

    private bool _firstHeaderBorder;

    public bool FirstHeaderBorder
    {
        get;
        set;
    }

    private int OverIndex
    {
        get
        {
            return this._overIndex;
        }
        set
        {
            this._overIndex = value;
            base.Invalidate();
        }
    }

    public XylosTabControl()
    {
        this._overIndex = -1;
        this.DoubleBuffered = true;
        base.Alignment = TabAlignment.Left;
        base.SizeMode = TabSizeMode.Fixed;
        base.ItemSize = new Size(40, 180);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        base.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        e.Control.BackColor = Color.White;
        e.Control.ForeColor = Helpers.ColorFromHex("#7C858E");
        e.Control.Font = new Font("Segoe UI", 9f);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Helpers.ColorFromHex("#33373B"));
        checked
        {
            int num = base.TabPages.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                this._rect = base.GetTabRect(i);
                bool flag = string.IsNullOrEmpty(Conversions.ToString(base.TabPages[i].Tag));
                if (flag)
                {
                    bool flag2 = base.SelectedIndex == i;
                    if (flag2)
                    {
                        using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#2B2F33")))
                        {
                            using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#BECCD9")))
                            {
                                using (Font font = new Font("Segoe UI semibold", 9f))
                                {
                                    this._g.FillRectangle(solidBrush, new Rectangle(this._rect.X - 5, this._rect.Y + 1, this._rect.Width + 7, this._rect.Height));
                                    this._g.DrawString(base.TabPages[i].Text, font, solidBrush2, new Point(this._rect.X + 50 + (base.ItemSize.Height - 180), this._rect.Y + 12));
                                }
                            }
                        }
                    }
                    else
                    {
                        using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#919BA6")))
                        {
                            using (Font font2 = new Font("Segoe UI semibold", 9f))
                            {
                                this._g.DrawString(base.TabPages[i].Text, font2, solidBrush3, new Point(this._rect.X + 50 + (base.ItemSize.Height - 180), this._rect.Y + 12));
                            }
                        }
                    }
                    bool flag3 = this.OverIndex != -1 & base.SelectedIndex != this.OverIndex;
                    if (flag3)
                    {
                        using (SolidBrush solidBrush4 = new SolidBrush(Helpers.ColorFromHex("#2F3338")))
                        {
                            using (SolidBrush solidBrush5 = new SolidBrush(Helpers.ColorFromHex("#919BA6")))
                            {
                                using (Font font3 = new Font("Segoe UI semibold", 9f))
                                {
                                    this._g.FillRectangle(solidBrush4, new Rectangle(base.GetTabRect(this.OverIndex).X - 5, base.GetTabRect(this.OverIndex).Y + 1, base.GetTabRect(this.OverIndex).Width + 7, base.GetTabRect(this.OverIndex).Height));
                                    this._g.DrawString(base.TabPages[this.OverIndex].Text, font3, solidBrush5, new Point(base.GetTabRect(this.OverIndex).X + 50 + (base.ItemSize.Height - 180), base.GetTabRect(this.OverIndex).Y + 12));
                                }
                            }
                        }
                        bool flag4 = !Information.IsNothing(base.ImageList);
                        if (flag4)
                        {
                            bool flag5 = base.TabPages[this.OverIndex].ImageIndex >= 0;
                            if (flag5)
                            {
                                this._g.DrawImage(base.ImageList.Images[base.TabPages[this.OverIndex].ImageIndex], new Rectangle(base.GetTabRect(this.OverIndex).X + 25 + (base.ItemSize.Height - 180), (int)Math.Round(unchecked((double)base.GetTabRect(this.OverIndex).Y + ((double)base.GetTabRect(this.OverIndex).Height / 2.0 - 9.0))), 16, 16));
                            }
                        }
                    }
                    bool flag6 = !Information.IsNothing(base.ImageList);
                    if (flag6)
                    {
                        bool flag7 = base.TabPages[i].ImageIndex >= 0;
                        if (flag7)
                        {
                            this._g.DrawImage(base.ImageList.Images[base.TabPages[i].ImageIndex], new Rectangle(this._rect.X + 25 + (base.ItemSize.Height - 180), (int)Math.Round(unchecked((double)this._rect.Y + ((double)this._rect.Height / 2.0 - 9.0))), 16, 16));
                        }
                    }
                }
                else
                {
                    using (SolidBrush solidBrush6 = new SolidBrush(Helpers.ColorFromHex("#6A7279")))
                    {
                        using (Font font4 = new Font("Segoe UI", 7f, FontStyle.Bold))
                        {
                            using (Pen pen = new Pen(Helpers.ColorFromHex("#2B2F33")))
                            {
                                bool firstHeaderBorder = this.FirstHeaderBorder;
                                if (firstHeaderBorder)
                                {
                                    this._g.DrawLine(pen, new Point(this._rect.X - 5, this._rect.Y + 1), new Point(this._rect.Width + 7, this._rect.Y + 1));
                                }
                                else
                                {
                                    bool flag8 = i != 0;
                                    if (flag8)
                                    {
                                        this._g.DrawLine(pen, new Point(this._rect.X - 5, this._rect.Y + 1), new Point(this._rect.Width + 7, this._rect.Y + 1));
                                    }
                                }
                                this._g.DrawString(base.TabPages[i].Text.ToUpper(), font4, solidBrush6, new Point(this._rect.X + 25 + (base.ItemSize.Height - 180), this._rect.Y + 16));
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void OnSelecting(TabControlCancelEventArgs e)
    {
        base.OnSelecting(e);
        bool flag = !Information.IsNothing(e.TabPage);
        if (flag)
        {
            bool flag2 = !string.IsNullOrEmpty(Conversions.ToString(e.TabPage.Tag));
            if (flag2)
            {
                e.Cancel = true;
            }
            else
            {
                this.OverIndex = -1;
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        checked
        {
            int num = base.TabPages.Count - 1;
            for (int i = 0; i <= num; i++)
            {
                bool flag = base.GetTabRect(i).Contains(e.Location) & base.SelectedIndex != i & string.IsNullOrEmpty(Conversions.ToString(base.TabPages[i].Tag));
                if (flag)
                {
                    this.OverIndex = i;
                    break;
                }
                this.OverIndex = -1;
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        this.OverIndex = -1;
    }
}
[DefaultEvent("TextChanged")]
public class XylosTextBox : Control
{
    public enum MouseState : byte
    {
        None,
        Over,
        Down
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never), AccessedThroughProperty("TB"), CompilerGenerated]
    private TextBox _tb;

    private Graphics _g;

    private XylosTextBox.MouseState _state;

    private bool _isDown;

    private bool _enabledCalc;

    private bool _allowpassword;

    private int _maxChars;

    private HorizontalAlignment _textAlignment;

    private bool _multiLine;

    private bool _readOnly;

    public virtual TextBox Tb
    {
        [CompilerGenerated]
        get
        {
            return this._tb;
        }
        [CompilerGenerated]
        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            EventHandler value2 = delegate (object a0, EventArgs a1)
            {
                this.TextChangeTb();
            };
            TextBox tB = this._tb;
            if (tB != null)
            {
                tB.TextChanged -= value2;
            }
            this._tb = value;
            tB = this._tb;
            if (tB != null)
            {
                tB.TextChanged += value2;
            }
        }
    }

    public new bool Enabled
    {
        get
        {
            return this.EnabledCalc;
        }
        set
        {
            this.Tb.Enabled = value;
            this._enabledCalc = value;
            base.Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc
    {
        get
        {
            return this._enabledCalc;
        }
        set
        {
            this.Enabled = value;
            base.Invalidate();
        }
    }

    public bool UseSystemPasswordChar
    {
        get
        {
            return this._allowpassword;
        }
        set
        {
            this.Tb.UseSystemPasswordChar = this.UseSystemPasswordChar;
            this._allowpassword = value;
            base.Invalidate();
        }
    }

    public int MaxLength
    {
        get
        {
            return this._maxChars;
        }
        set
        {
            this._maxChars = value;
            this.Tb.MaxLength = this.MaxLength;
            base.Invalidate();
        }
    }

    public HorizontalAlignment TextAlign
    {
        get
        {
            return this._textAlignment;
        }
        set
        {
            this._textAlignment = value;
            base.Invalidate();
        }
    }

    public bool MultiLine
    {
        get
        {
            return this._multiLine;
        }
        set
        {
            this._multiLine = value;
            this.Tb.Multiline = value;
            this.OnResize(EventArgs.Empty);
            base.Invalidate();
        }
    }

    public bool ReadOnly
    {
        get
        {
            return this._readOnly;
        }
        set
        {
            this._readOnly = value;
            bool flag = this.Tb != null;
            if (flag)
            {
                this.Tb.ReadOnly = value;
            }
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        base.Invalidate();
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        base.Invalidate();
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        this.Tb.ForeColor = this.ForeColor;
        base.Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        this.Tb.Font = this.Font;
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        this.Tb.Focus();
    }

    private void TextChangeTb()
    {
        this.Text = this.Tb.Text;
    }

    private void TextChng()
    {
        this.Tb.Text = this.Text;
    }

    public void NewTextBox()
    {
        TextBox tB = this.Tb;
        tB.Text = string.Empty;
        tB.BackColor = Color.White;
        tB.ForeColor = Helpers.ColorFromHex("#7C858E");
        tB.TextAlign = HorizontalAlignment.Left;
        tB.BorderStyle = BorderStyle.None;
        tB.Location = new Point(3, 3);
        tB.Font = new Font("Segoe UI", 9f);
        tB.Size = checked(new Size(base.Width - 3, base.Height - 3));
        tB.UseSystemPasswordChar = this.UseSystemPasswordChar;
    }

    public XylosTextBox()
    {
        base.TextChanged += delegate (object a0, EventArgs a1)
        {
            this.TextChng();
        };
        this.Tb = new TextBox();
        this._allowpassword = false;
        this._maxChars = 32767;
        this._multiLine = false;
        this._readOnly = false;
        this.NewTextBox();
        base.Controls.Add(this.Tb);
        base.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
        this.DoubleBuffered = true;
        this.TextAlign = HorizontalAlignment.Left;
        this.ForeColor = Helpers.ColorFromHex("#7C858E");
        this.Font = new Font("Segoe UI", 9f);
        base.Size = new Size(130, 29);
        this.Enabled = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Color.White);
        bool enabled = this.Enabled;
        if (enabled)
        {
            this.Tb.ForeColor = Helpers.ColorFromHex("#7C858E");
            bool flag = this._state == XylosTextBox.MouseState.Down;
            if (flag)
            {
                using (Pen pen = new Pen(Helpers.ColorFromHex("#78B7E6")))
                {
                    this._g.DrawPath(pen, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 12, Helpers.RoundingStyle.All));
                }
            }
            else
            {
                using (Pen pen2 = new Pen(Helpers.ColorFromHex("#D0D5D9")))
                {
                    this._g.DrawPath(pen2, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 12, Helpers.RoundingStyle.All));
                }
            }
        }
        else
        {
            this.Tb.ForeColor = Helpers.ColorFromHex("#7C858E");
            using (Pen pen3 = new Pen(Helpers.ColorFromHex("#E1E1E2")))
            {
                this._g.DrawPath(pen3, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 12, Helpers.RoundingStyle.All));
            }
        }
        this.Tb.TextAlign = this.TextAlign;
        this.Tb.UseSystemPasswordChar = this.UseSystemPasswordChar;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        bool flag = !this.MultiLine;
        checked
        {
            if (flag)
            {
                int height = this.Tb.Height;
                this.Tb.Location = new Point(10, (int)Math.Round(unchecked((double)base.Height / 2.0 - (double)height / 2.0 - 0.0)));
                this.Tb.Size = new Size(base.Width - 20, height);
            }
            else
            {
                this.Tb.Location = new Point(10, 10);
                this.Tb.Size = new Size(base.Width - 20, base.Height - 20);
            }
        }
    }

    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        this._state = XylosTextBox.MouseState.Down;
        base.Invalidate();
    }

    protected override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        this._state = XylosTextBox.MouseState.None;
        base.Invalidate();
    }
}
public class XylosButton : Control
{
    public enum MouseState : byte
    {
        None,
        Over,
        Down
    }

    public delegate void ClickEventHandler(object sender, EventArgs e);

    private Graphics _g;

    private XylosButton.MouseState _state;

    private bool _enabledCalc;

    [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
    private XylosButton.ClickEventHandler _clickEvent;

    public new event XylosButton.ClickEventHandler Click
    {
        [CompilerGenerated]
        add
        {
            XylosButton.ClickEventHandler clickEventHandler = this._clickEvent;
            XylosButton.ClickEventHandler clickEventHandler2;
            do
            {
                clickEventHandler2 = clickEventHandler;
                XylosButton.ClickEventHandler value2 = (XylosButton.ClickEventHandler)Delegate.Combine(clickEventHandler2, value);
                clickEventHandler = Interlocked.CompareExchange<XylosButton.ClickEventHandler>(ref this._clickEvent, value2, clickEventHandler2);
            }
            while (clickEventHandler != clickEventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            XylosButton.ClickEventHandler clickEventHandler = this._clickEvent;
            XylosButton.ClickEventHandler clickEventHandler2;
            do
            {
                clickEventHandler2 = clickEventHandler;
                XylosButton.ClickEventHandler value2 = (XylosButton.ClickEventHandler)Delegate.Remove(clickEventHandler2, value);
                clickEventHandler = Interlocked.CompareExchange<XylosButton.ClickEventHandler>(ref this._clickEvent, value2, clickEventHandler2);
            }
            while (clickEventHandler != clickEventHandler2);
        }
    }

    public new bool Enabled
    {
        get
        {
            return this.EnabledCalc;
        }
        set
        {
            this._enabledCalc = value;
            base.Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc
    {
        get
        {
            return this._enabledCalc;
        }
        set
        {
            this.Enabled = value;
            base.Invalidate();
        }
    }

    public XylosButton()
    {
        this.DoubleBuffered = true;
        this.Enabled = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        bool enabled = this.Enabled;
        if (enabled)
        {
            XylosButton.MouseState state = this._state;
            if (state != XylosButton.MouseState.Over)
            {
                if (state != XylosButton.MouseState.Down)
                {
                    using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#F6F6F6")))
                    {
                        this._g.FillPath(solidBrush, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                    }
                }
                else
                {
                    using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#F0F0F0")))
                    {
                        this._g.FillPath(solidBrush2, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                    }
                }
            }
            else
            {
                using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#FDFDFD")))
                {
                    this._g.FillPath(solidBrush3, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                }
            }
            using (Font font = new Font("Segoe UI", 9f))
            {
                using (Pen pen = new Pen(Helpers.ColorFromHex("#C3C3C3")))
                {
                    this._g.DrawPath(pen, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                    Helpers.CenterString(this._g, this.Text, font, Helpers.ColorFromHex("#7C858E"), Helpers.FullRectangle(base.Size, false));
                }
            }
        }
        else
        {
            using (SolidBrush solidBrush4 = new SolidBrush(Helpers.ColorFromHex("#F3F4F7")))
            {
                using (Pen pen2 = new Pen(Helpers.ColorFromHex("#DCDCDC")))
                {
                    using (Font font2 = new Font("Segoe UI", 9f))
                    {
                        this._g.FillPath(solidBrush4, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                        this._g.DrawPath(pen2, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                        Helpers.CenterString(this._g, this.Text, font2, Helpers.ColorFromHex("#D0D3D7"), Helpers.FullRectangle(base.Size, false));
                    }
                }
            }
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        this._state = XylosButton.MouseState.Over;
        base.Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        this._state = XylosButton.MouseState.None;
        base.Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        bool enabled = this.Enabled;
        if (enabled)
        {
            XylosButton.ClickEventHandler clickEvent = this._clickEvent;
            if (clickEvent != null)
            {
                clickEvent(this, e);
            }
        }
        this._state = XylosButton.MouseState.Over;
        base.Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        this._state = XylosButton.MouseState.Down;
        base.Invalidate();
    }
}
[DefaultEvent("CheckedChanged")]
public class XylosCheckBox : Control
{
    public delegate void CheckedChangedEventHandler(object sender, EventArgs e);

    [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
    private XylosCheckBox.CheckedChangedEventHandler _checkedChangedEvent;

    private bool _checked;

    private bool _enabledCalc;

    private Graphics _g;

    private string _b64Enabled;

    private string _b64Disabled;

    public event XylosCheckBox.CheckedChangedEventHandler CheckedChanged
    {
        [CompilerGenerated]
        add
        {
            XylosCheckBox.CheckedChangedEventHandler checkedChangedEventHandler = this._checkedChangedEvent;
            XylosCheckBox.CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                XylosCheckBox.CheckedChangedEventHandler value2 = (XylosCheckBox.CheckedChangedEventHandler)Delegate.Combine(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange<XylosCheckBox.CheckedChangedEventHandler>(ref this._checkedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while (checkedChangedEventHandler != checkedChangedEventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            XylosCheckBox.CheckedChangedEventHandler checkedChangedEventHandler = this._checkedChangedEvent;
            XylosCheckBox.CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                XylosCheckBox.CheckedChangedEventHandler value2 = (XylosCheckBox.CheckedChangedEventHandler)Delegate.Remove(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange<XylosCheckBox.CheckedChangedEventHandler>(ref this._checkedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while (checkedChangedEventHandler != checkedChangedEventHandler2);
        }
    }

    public bool Checked
    {
        get
        {
            return this._checked;
        }
        set
        {
            this._checked = value;
            base.Invalidate();
        }
    }

    public new bool Enabled
    {
        get
        {
            return this.EnabledCalc;
        }
        set
        {
            this._enabledCalc = value;
            bool enabled = this.Enabled;
            if (enabled)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
            base.Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc
    {
        get
        {
            return this._enabledCalc;
        }
        set
        {
            this.Enabled = value;
            base.Invalidate();
        }
    }

    public XylosCheckBox()
    {
        this._b64Enabled = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA00lEQVQ4T6WTwQ2CMBSG30/07Ci6gY7gxZoIiYADuAIrsIDpQQ/cHMERZBOuXHimDSWALYL01EO/L//724JmLszk6S+BCOIExFsmL50sEH4kAZxVciYuJgnacD16Plpgg8tFtYMILntQdSXiZ3aXqa1UF/yUsoDw4wKglQaZZPa4RW3JEKzO4RjEbyJaN1BL8gvWgsMp3ADeq0lRJ2FimLZNYWpmFbudUJdolXTLyG2wTmDODUiccEfgSDIIfwmMxAMStS+XHPZn7l/z6Ifk+nSzBR8zi2d9JmVXSgAAAABJRU5ErkJggg==";
        this._b64Disabled = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA1UlEQVQ4T6WTzQ2CQBCF56EnLpaiXvUAJBRgB2oFtkALdEAJnoVEMIGzdEIFjNkFN4DLn+xpD/N9efMWQAsPFvL0lyBMUg8MiwzyZwuiJAuI6CyTMxezBC24EuSTBTp4xaaN6JWdqKQbge6udfB1pfbBjrMvEMZZAdCm3ilw7eO1KRmCxRyiOH0TsFUQs5KMwVLweKY7ALFKUZUTECD6qdquCxM7i9jNhLJEraQ5xZzrYJngO9crGYBbAm2SEfhHoCQGeeK+Ls1Ld+fuM0/+kPp+usWCD10idEOGa4QuAAAAAElFTkSuQmCC";
        this.DoubleBuffered = true;
        this.Enabled = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Color.White);
        bool enabled = this.Enabled;
        if (enabled)
        {
            using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#F3F4F7")))
            {
                using (Pen pen = new Pen(Helpers.ColorFromHex("#D0D5D9")))
                {
                    using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#7C858E")))
                    {
                        using (Font font = new Font("Segoe UI", 9f))
                        {
                            this._g.FillPath(solidBrush, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 3, Helpers.RoundingStyle.All));
                            this._g.DrawPath(pen, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 3, Helpers.RoundingStyle.All));
                            this._g.DrawString(this.Text, font, solidBrush2, new Point(25, 0));
                        }
                    }
                }
            }
            bool @checked = this.Checked;
            if (@checked)
            {
                using (Image image = Image.FromStream(new MemoryStream(Convert.FromBase64String(this._b64Enabled))))
                {
                    this._g.DrawImage(image, new Rectangle(3, 3, 11, 11));
                }
            }
        }
        else
        {
            using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#F5F5F8")))
            {
                using (Pen pen2 = new Pen(Helpers.ColorFromHex("#E1E1E2")))
                {
                    using (SolidBrush solidBrush4 = new SolidBrush(Helpers.ColorFromHex("#D0D3D7")))
                    {
                        using (Font font2 = new Font("Segoe UI", 9f))
                        {
                            this._g.FillPath(solidBrush3, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 3, Helpers.RoundingStyle.All));
                            this._g.DrawPath(pen2, Helpers.RoundRect(new Rectangle(0, 0, 16, 16), 3, Helpers.RoundingStyle.All));
                            this._g.DrawString(this.Text, font2, solidBrush4, new Point(25, 0));
                        }
                    }
                }
            }
            bool checked2 = this.Checked;
            if (checked2)
            {
                using (Image image2 = Image.FromStream(new MemoryStream(Convert.FromBase64String(this._b64Disabled))))
                {
                    this._g.DrawImage(image2, new Rectangle(3, 3, 11, 11));
                }
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        bool enabled = this.Enabled;
        if (enabled)
        {
            this.Checked = !this.Checked;
            XylosCheckBox.CheckedChangedEventHandler checkedChangedEvent = this._checkedChangedEvent;
            if (checkedChangedEvent != null)
            {
                checkedChangedEvent(this, e);
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        base.Size = new Size(base.Width, 18);
    }
}
public class XylosCombobox : ComboBox
{
    private Graphics _g;

    private Rectangle _rect;

    private bool _enabledCalc;

    public new bool Enabled
    {
        get
        {
            return this.EnabledCalc;
        }
        set
        {
            this._enabledCalc = value;
            base.Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc
    {
        get
        {
            return this._enabledCalc;
        }
        set
        {
            base.Enabled = value;
            this.Enabled = value;
            base.Invalidate();
        }
    }

    public XylosCombobox()
    {
        this.DoubleBuffered = true;
        base.DropDownStyle = ComboBoxStyle.DropDownList;
        this.Cursor = Cursors.Hand;
        this.Enabled = true;
        base.DrawMode = DrawMode.OwnerDrawFixed;
        base.ItemHeight = 20;
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        base.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Color.White);
        bool enabled = this.Enabled;
        checked
        {
            if (enabled)
            {
                using (Pen pen = new Pen(Helpers.ColorFromHex("#D0D5D9")))
                {
                    using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#7C858E")))
                    {
                        using (Font font = new Font("Marlett", 13f))
                        {
                            this._g.DrawPath(pen, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 6, Helpers.RoundingStyle.All));
                            this._g.DrawString("6", font, solidBrush, new Point(base.Width - 22, 3));
                        }
                    }
                }
            }
            else
            {
                using (Pen pen2 = new Pen(Helpers.ColorFromHex("#E1E1E2")))
                {
                    using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#D0D3D7")))
                    {
                        using (Font font2 = new Font("Marlett", 13f))
                        {
                            this._g.DrawPath(pen2, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 6, Helpers.RoundingStyle.All));
                            this._g.DrawString("6", font2, solidBrush2, new Point(base.Width - 22, 3));
                        }
                    }
                }
            }
            bool flag = !Information.IsNothing(base.Items);
            if (flag)
            {
                using (Font font3 = new Font("Segoe UI", 9f))
                {
                    using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#7C858E")))
                    {
                        bool enabled2 = this.Enabled;
                        if (enabled2)
                        {
                            bool flag2 = this.SelectedIndex != -1;
                            if (flag2)
                            {
                                this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[this.SelectedIndex])), font3, solidBrush3, new Point(7, 4));
                            }
                            else
                            {
                                try
                                {
                                    this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[0])), font3, solidBrush3, new Point(7, 4));
                                }
                                catch (Exception arg2720)
                                {
                                    ProjectData.SetProjectError(arg2720);
                                    ProjectData.ClearProjectError();
                                }
                            }
                        }
                        else
                        {
                            using (SolidBrush solidBrush4 = new SolidBrush(Helpers.ColorFromHex("#D0D3D7")))
                            {
                                bool flag3 = this.SelectedIndex != -1;
                                if (flag3)
                                {
                                    this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[this.SelectedIndex])), font3, solidBrush4, new Point(7, 4));
                                }
                                else
                                {
                                    this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[0])), font3, solidBrush4, new Point(7, 4));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        base.OnDrawItem(e);
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        bool enabled = this.Enabled;
        checked
        {
            if (enabled)
            {
                e.DrawBackground();
                this._rect = e.Bounds;
                try
                {
                    using (new Font("Segoe UI", 9f))
                    {
                        using (new Pen(Helpers.ColorFromHex("#D0D5D9")))
                        {
                            bool flag = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                            if (flag)
                            {
                                using (new SolidBrush(Color.White))
                                {
                                    using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#78B7E6")))
                                    {
                                        this._g.FillRectangle(solidBrush2, this._rect);
                                        this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[e.Index])), new Font("Segoe UI", 9f), Brushes.White, new Point(this._rect.X + 5, this._rect.Y + 1));
                                    }
                                }
                            }
                            else
                            {
                                using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#7C858E")))
                                {
                                    this._g.FillRectangle(Brushes.White, this._rect);
                                    this._g.DrawString(base.GetItemText(RuntimeHelpers.GetObjectValue(base.Items[e.Index])), new Font("Segoe UI", 9f), solidBrush3, new Point(this._rect.X + 5, this._rect.Y + 1));
                                }
                            }
                        }
                    }
                }
                catch (Exception arg_1F10)
                {
                    ProjectData.SetProjectError(arg_1F10);
                    ProjectData.ClearProjectError();
                }
            }
        }
    }

    protected override void OnSelectedItemChanged(EventArgs e)
    {
        base.OnSelectedItemChanged(e);
        base.Invalidate();
    }
}
public class XylosNotice : TextBox
{
    private Graphics _g;

    private string _b64;

    public XylosNotice()
    {
        this._b64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABL0lEQVQ4T5VT0VGDQBB9e2cBdGBSgTIDEr9MCw7pI0kFtgB9yFiC+KWMmREqMOnAAuDWOfAiudzhyA/svtvH7Xu7BOv5eH2atVKtwbwk0LWGGVyDqLzoRB7e3u/HJTQOdm+PGYjWNuk4ZkIW36RbkzsS7KqiBnB1Usw49DHh8oQEXMfJKhwgAM4/Mw7RIp0NeLG3ScCcR4vVhnTPnVCf9rUZeImTdKnz71VREnBnn5FKzMnX95jA2V6vLufkBQFESTq0WBXsEla7owmcoC6QJMKW2oCUePY5M0lAjK0iBAQ8TBGc2/d7+uvnM/AQNF4Rp4bpiGkRfTb2Gigx12+XzQb3D9JfBGaQzHWm7HS000RJ2i/av5fJjPDZMplErwl1GxDpMTbL1YC5lCwze52/AQFekh7wKBpGAAAAAElFTkSuQmCC";
        this.DoubleBuffered = true;
        base.Enabled = false;
        base.ReadOnly = true;
        base.BorderStyle = BorderStyle.None;
        this.Multiline = true;
        this.Cursor = Cursors.Default;
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        base.SetStyle(ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Color.White);
        using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#FFFDE8")))
        {
            using (Pen pen = new Pen(Helpers.ColorFromHex("#F2F3F7")))
            {
                using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#B9B595")))
                {
                    using (Font font = new Font("Segoe UI", 9f))
                    {
                        this._g.FillPath(solidBrush, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                        this._g.DrawPath(pen, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 3, Helpers.RoundingStyle.All));
                        this._g.DrawString(this.Text, font, solidBrush2, new Point(30, 6));
                    }
                }
            }
        }
        using (Image image = Image.FromStream(new MemoryStream(Convert.FromBase64String(this._b64))))
        {
            this._g.DrawImage(image, new Rectangle(8, checked((int)Math.Round(unchecked((double)base.Height / 2.0 - 8.0))), 16, 16));
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
    }
}
public class XylosProgressBar : Control
{
    private int _val;

    private int _min;

    private int _max;

    [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
    private Color _stripes;

    [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
    private Color _backgroundColor;

    public Color Stripes
    {
        get;
        set;
    }

    public Color BackgroundColor
    {
        get;
        set;
    }

    public int Value
    {
        get
        {
            return this._val;
        }
        set
        {
            this._val = value;
            base.Invalidate();
        }
    }

    public int Minimum
    {
        get
        {
            return this._min;
        }
        set
        {
            this._min = value;
            base.Invalidate();
        }
    }

    public int Maximum
    {
        get
        {
            return this._max;
        }
        set
        {
            this._max = value;
            base.Invalidate();
        }
    }

    public XylosProgressBar()
    {
        this._val = 0;
        this._min = 0;
        this._max = 100;
        this.Stripes = Color.DarkGreen;
        this.BackgroundColor = Color.Green;
        this.DoubleBuffered = true;
        this.Maximum = 100;
        this.Minimum = 0;
        this.Value = 0;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics graphics = e.Graphics;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        graphics.Clear(Color.White);
        using (Pen pen = new Pen(Helpers.ColorFromHex("#D0D5D9")))
        {
            graphics.DrawPath(pen, Helpers.RoundRect(Helpers.FullRectangle(base.Size, true), 6, Helpers.RoundingStyle.All));
        }
        bool flag = this.Value != 0;
        if (flag)
        {
            using (HatchBrush hatchBrush = new HatchBrush(HatchStyle.LightUpwardDiagonal, this.Stripes, this.BackgroundColor))
            {
                graphics.FillPath(hatchBrush, Helpers.RoundRect(checked(new Rectangle(0, 0, (int)Math.Round(unchecked((double)this.Value / (double)this.Maximum * (double)base.Width - 1.0)), base.Height - 1)), 6, Helpers.RoundingStyle.All));
            }
        }
    }
}
[DefaultEvent("CheckedChanged")]
public class XylosRadioButton : Control
{
    public delegate void CheckedChangedEventHandler(object sender, EventArgs e);

    [DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
    private XylosRadioButton.CheckedChangedEventHandler _checkedChangedEvent;

    private bool _checked;

    private bool _enabledCalc;

    private Graphics _g;

    public event XylosRadioButton.CheckedChangedEventHandler CheckedChanged
    {
        [CompilerGenerated]
        add
        {
            XylosRadioButton.CheckedChangedEventHandler checkedChangedEventHandler = this._checkedChangedEvent;
            XylosRadioButton.CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                XylosRadioButton.CheckedChangedEventHandler value2 = (XylosRadioButton.CheckedChangedEventHandler)Delegate.Combine(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange<XylosRadioButton.CheckedChangedEventHandler>(ref this._checkedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while (checkedChangedEventHandler != checkedChangedEventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            XylosRadioButton.CheckedChangedEventHandler checkedChangedEventHandler = this._checkedChangedEvent;
            XylosRadioButton.CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                XylosRadioButton.CheckedChangedEventHandler value2 = (XylosRadioButton.CheckedChangedEventHandler)Delegate.Remove(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange<XylosRadioButton.CheckedChangedEventHandler>(ref this._checkedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while (checkedChangedEventHandler != checkedChangedEventHandler2);
        }
    }

    public bool Checked
    {
        get
        {
            return this._checked;
        }
        set
        {
            this._checked = value;
            base.Invalidate();
        }
    }

    public new bool Enabled
    {
        get
        {
            return this.EnabledCalc;
        }
        set
        {
            this._enabledCalc = value;
            bool enabled = this.Enabled;
            if (enabled)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
            base.Invalidate();
        }
    }

    [DisplayName("Enabled")]
    public bool EnabledCalc
    {
        get
        {
            return this._enabledCalc;
        }
        set
        {
            this.Enabled = value;
            base.Invalidate();
        }
    }

    public XylosRadioButton()
    {
        this.DoubleBuffered = true;
        this.Enabled = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        this._g.Clear(Color.White);
        bool enabled = this.Enabled;
        if (enabled)
        {
            using (SolidBrush solidBrush = new SolidBrush(Helpers.ColorFromHex("#F3F4F7")))
            {
                using (Pen pen = new Pen(Helpers.ColorFromHex("#D0D5D9")))
                {
                    using (SolidBrush solidBrush2 = new SolidBrush(Helpers.ColorFromHex("#7C858E")))
                    {
                        using (Font font = new Font("Segoe UI", 9f))
                        {
                            this._g.FillEllipse(solidBrush, new Rectangle(0, 0, 16, 16));
                            this._g.DrawEllipse(pen, new Rectangle(0, 0, 16, 16));
                            this._g.DrawString(this.Text, font, solidBrush2, new Point(25, 0));
                        }
                    }
                }
            }
            bool @checked = this.Checked;
            if (@checked)
            {
                using (SolidBrush solidBrush3 = new SolidBrush(Helpers.ColorFromHex("#575C62")))
                {
                    this._g.FillEllipse(solidBrush3, new Rectangle(4, 4, 8, 8));
                }
            }
        }
        else
        {
            using (SolidBrush solidBrush4 = new SolidBrush(Helpers.ColorFromHex("#F5F5F8")))
            {
                using (Pen pen2 = new Pen(Helpers.ColorFromHex("#E1E1E2")))
                {
                    using (SolidBrush solidBrush5 = new SolidBrush(Helpers.ColorFromHex("#D0D3D7")))
                    {
                        using (Font font2 = new Font("Segoe UI", 9f))
                        {
                            this._g.FillEllipse(solidBrush4, new Rectangle(0, 0, 16, 16));
                            this._g.DrawEllipse(pen2, new Rectangle(0, 0, 16, 16));
                            this._g.DrawString(this.Text, font2, solidBrush5, new Point(25, 0));
                        }
                    }
                }
            }
            bool checked2 = this.Checked;
            if (checked2)
            {
                using (SolidBrush solidBrush6 = new SolidBrush(Helpers.ColorFromHex("#BCC1C6")))
                {
                    this._g.FillEllipse(solidBrush6, new Rectangle(4, 4, 8, 8));
                }
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        bool enabled = this.Enabled;
        if (enabled)
        {
            try
            {
                IEnumerator enumerator = base.Parent.Controls.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Control control = (Control)enumerator.Current;
                    bool flag = control is XylosRadioButton;
                    if (flag)
                    {
                        ((XylosRadioButton)control).Checked = false;
                    }
                }
            }
            finally
            {

            }
            this.Checked = !this.Checked;
            XylosRadioButton.CheckedChangedEventHandler checkedChangedEvent = this._checkedChangedEvent;
            if (checkedChangedEvent != null)
            {
                checkedChangedEvent(this, e);
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        base.Size = new Size(base.Width, 18);
    }
}
public class XylosSeparator : Control
{
    private Graphics _g;

    public XylosSeparator()
    {
        this.DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        this._g = e.Graphics;
        this._g.SmoothingMode = SmoothingMode.HighQuality;
        this._g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        base.OnPaint(e);
        using (Pen pen = new Pen(Helpers.ColorFromHex("#EBEBEC")))
        {
            this._g.DrawLine(pen, new Point(0, 0), new Point(base.Width, 0));
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        base.Size = new Size(base.Width, 2);
    }
}