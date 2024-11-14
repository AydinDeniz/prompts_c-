
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

public class ImageEditorApp : Form
{
    private PictureBox pictureBox;
    private Bitmap originalImage;
    private Bitmap editedImage;

    private Button loadButton;
    private Button saveButton;
    private Button rotateButton;
    private Button grayscaleButton;
    private NumericUpDown resizeNumeric;

    public ImageEditorApp()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Image Editor App";
        this.Size = new Size(800, 600);

        pictureBox = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom };
        this.Controls.Add(pictureBox);

        loadButton = new Button { Text = "Load Image", Dock = DockStyle.Top };
        loadButton.Click += LoadButton_Click;
        this.Controls.Add(loadButton);

        saveButton = new Button { Text = "Save Image", Dock = DockStyle.Top };
        saveButton.Click += SaveButton_Click;
        this.Controls.Add(saveButton);

        rotateButton = new Button { Text = "Rotate 90°", Dock = DockStyle.Top };
        rotateButton.Click += RotateButton_Click;
        this.Controls.Add(rotateButton);

        grayscaleButton = new Button { Text = "Grayscale", Dock = DockStyle.Top };
        grayscaleButton.Click += GrayscaleButton_Click;
        this.Controls.Add(grayscaleButton);

        resizeNumeric = new NumericUpDown { Dock = DockStyle.Top, Minimum = 10, Maximum = 200, Value = 100 };
        resizeNumeric.ValueChanged += ResizeNumeric_ValueChanged;
        this.Controls.Add(resizeNumeric);
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" };
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            originalImage = new Bitmap(openFileDialog.FileName);
            editedImage = (Bitmap)originalImage.Clone();
            pictureBox.Image = editedImage;
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (editedImage == null) return;

        SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp" };
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            ImageFormat format = saveFileDialog.FilterIndex switch
            {
                1 => ImageFormat.Jpeg,
                2 => ImageFormat.Png,
                3 => ImageFormat.Bmp,
                _ => ImageFormat.Jpeg
            };
            editedImage.Save(saveFileDialog.FileName, format);
        }
    }

    private void RotateButton_Click(object sender, EventArgs e)
    {
        if (editedImage == null) return;

        editedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
        pictureBox.Image = editedImage;
    }

    private void GrayscaleButton_Click(object sender, EventArgs e)
    {
        if (editedImage == null) return;

        for (int y = 0; y < editedImage.Height; y++)
        {
            for (int x = 0; x < editedImage.Width; x++)
            {
                Color originalColor = editedImage.GetPixel(x, y);
                int grayScale = (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                Color grayColor = Color.FromArgb(grayScale, grayScale, grayScale);
                editedImage.SetPixel(x, y, grayColor);
            }
        }
        pictureBox.Image = editedImage;
    }

    private void ResizeNumeric_ValueChanged(object sender, EventArgs e)
    {
        if (originalImage == null) return;

        float scale = (float)resizeNumeric.Value / 100;
        int newWidth = (int)(originalImage.Width * scale);
        int newHeight = (int)(originalImage.Height * scale);
        editedImage = new Bitmap(originalImage, newWidth, newHeight);
        pictureBox.Image = editedImage;
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new ImageEditorApp());
    }
}
