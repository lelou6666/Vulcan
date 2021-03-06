﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

// A CAPTCHA is a method of proving that the user is a human, and not an automated system. 
// Returns a JPEG image with random digits and sets the session with the code. 
public partial class CAPTCHAimage_CAPTCHAimage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Prevent the captcha image from being cached.  Needed in IE.
        Response.Cache.SetNoStore();

        // if there is already a CAPTCHA value then reuse it. 
        if ((Session["CAPTCHA"] != null) && (Session["CAPTCHA"].ToString() != String.Empty))
        {
            Response.StatusCode = (int) System.Net.HttpStatusCode.NotModified;
        }
        else
        {
            string newCode = GenerateRandomCode();
            
            // Create a CAPTCHA image 
            CaptchaImage ci = new CaptchaImage(newCode, 200, 50, "Century Schoolbook");
            try 
	        {    
                // Change the response headers to output a JPEG image.
                Response.Clear();
                Response.ContentType = "image/jpeg";

                // Write the image to the response stream in JPEG format.
                ci.Image.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                Session["CAPTCHA"] = newCode;
            }
	        finally
	        {
                ci.Dispose();
	        }
        }
    }

    //
    // Returns a string of random digits.
    //
    private string GenerateRandomCode()
    {
        System.Security.Cryptography.RNGCryptoServiceProvider random = new System.Security.Cryptography.RNGCryptoServiceProvider();
        Byte[] bytes = new Byte[6]; // the length of the string
        random.GetBytes(bytes);
        int digit;
        string result = "";
        foreach (byte b in bytes)
        {
            digit = b % 10; 
            result += digit.ToString();
        }
        return result;
    }
}

//The following 
//Source code courtesy www.brainjar.com
public class CaptchaImage
{
    // Public properties (all read-only).
    public string Text
    {
        get { return this.text; }
    }
    public Bitmap Image
    {
        get { return this.image; }
    }
    public int Width
    {
        get { return this.width; }
    }
    public int Height
    {
        get { return this.height; }
    }

    // Internal properties.
    private string text;
    private int width;
    private int height;
    private string familyName;
    private Bitmap image;

    // For generating random numbers.
    private Random random = new Random();

    // ====================================================================
    // Initializes a new instance of the CaptchaImage class using the
    // specified text, width and height.
    // ====================================================================
    public CaptchaImage(string s, int width, int height)
    {
        this.text = s;
        this.SetDimensions(width, height);
        this.GenerateImage();
    }

    // ====================================================================
    // Initializes a new instance of the CaptchaImage class using the
    // specified text, width, height and font family.
    // ====================================================================
    public CaptchaImage(string s, int width, int height, string familyName)
    {
        this.text = s;
        this.SetDimensions(width, height);
        this.SetFamilyName(familyName);
        this.GenerateImage();
    }

    // ====================================================================
    // This member overrides Object.Finalize.
    // ====================================================================
    ~CaptchaImage()
    {
        Dispose(false);
    }

    // ====================================================================
    // Releases all resources used by this object.
    // ====================================================================
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    // ====================================================================
    // Custom Dispose method to clean up unmanaged resources.
    // ====================================================================
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            // Dispose of the bitmap.
            this.image.Dispose();
    }

    // ====================================================================
    // Sets the image width and height.
    // ====================================================================
    private void SetDimensions(int width, int height)
    {
        // Check the width and height.
        if (width <= 0)
            throw new ArgumentOutOfRangeException("width", width, "Argument out of range, must be greater than zero.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException("height", height, "Argument out of range, must be greater than zero.");
        this.width = width;
        this.height = height;
    }

    // ====================================================================
    // Sets the font used for the image text.
    // ====================================================================
    private void SetFamilyName(string familyName)
    {
        // If the named font is not installed, default to a system font.
        try
        {
            Font font = new Font(this.familyName, 12F);
            this.familyName = familyName;
            font.Dispose();
        }
        catch
        {
            this.familyName = System.Drawing.FontFamily.GenericSerif.Name;
        }
    }

    // ====================================================================
    // Creates the bitmap image.
    // ====================================================================
    private void GenerateImage()
    {
        // Create a new 32-bit bitmap image.
        Bitmap bitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);

        // Create a graphics object for drawing.
        Graphics g = Graphics.FromImage(bitmap);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle rect = new Rectangle(0, 0, this.width, this.height);

        // Fill in the background.
        HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White);
        g.FillRectangle(hatchBrush, rect);

        // Set up the text font.
        SizeF size;
        float fontSize = rect.Height + 1;
        Font font;
        // Adjust the font size until the text fits within the image.
        do
        {
            fontSize--;
            font = new Font(this.familyName, fontSize, FontStyle.Bold);
            size = g.MeasureString(this.text, font);
        } while (size.Width > rect.Width);

        // Set up the text format.
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        // Create a path using the text and warp it randomly.
        GraphicsPath path = new GraphicsPath();
        path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
        float v = 4F;
        PointF[] points =
			{
				new PointF(this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v)
			};
        Matrix matrix = new Matrix();
        matrix.Translate(0F, 0F);
        path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);

        // Draw the text.
        hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.LightGray, Color.DarkGray);
        g.FillPath(hatchBrush, path);

        // Add some random noise.
        int m = Math.Max(rect.Width, rect.Height);
        for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
        {
            int x = this.random.Next(rect.Width);
            int y = this.random.Next(rect.Height);
            int w = this.random.Next(m / 50);
            int h = this.random.Next(m / 50);
            g.FillEllipse(hatchBrush, x, y, w, h);
        }

        // Clean up.
        font.Dispose();
        hatchBrush.Dispose();
        g.Dispose();

        // Set the image.
        this.image = bitmap;
    }
}





