using System.Text;
using System.Windows.Forms;

namespace MangaTranslatorHelper
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private Point startPoint;
        private Rectangle currentRectangle;

        private List<Annotation> annotations = new List<Annotation>();
        private Annotation selectedAnnotation = null;

        private string imageFilename;

        private bool isDragging = false;
        private Point dragStart;
        private Annotation draggedAnnotation = null;

        private bool isResizing = false;
        private const int resizeMargin = 10;
        //private Rectangle resizeRectangle;
        private Annotation resizingAnnotation = null;
        private ResizeDirection resizeDirection = ResizeDirection.None;

        private enum ResizeDirection
        {
            None, Left, Right, Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight
        }


        public Form1()
        {
            InitializeComponent();

            contextMenuStrip1 = new ContextMenuStrip();
            contextMenuStrip1.Items.Add("Edit", null, EditAnnotation);
            contextMenuStrip1.Items.Add("Delete", null, DeleteAnnotation);

            pictureBox1.ContextMenuStrip = contextMenuStrip1;

        }

        private void OpenImage(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image files|*.png;*.jpeg;*.jpg;*.bmp;*.gif|All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                annotations.Clear();
                imageFilename = ofd.FileName;
                pictureBox1.Image = Image.FromFile(imageFilename);
                toolStripStatusLabel1.Text = $"Image loaded {ofd.FileName}";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedAnnotation = annotations.FirstOrDefault(a => a.Area.Contains(e.Location));



                if (selectedAnnotation != null)
                {
                    resizeDirection = GetResizeDirection(selectedAnnotation, e.Location);

                    if (resizeDirection != ResizeDirection.None)
                    {
                        isResizing = true;
                        resizingAnnotation = selectedAnnotation;
                        dragStart = e.Location;
                        return;
                    }

                    isDragging = true;
                    dragStart = e.Location;
                    draggedAnnotation = selectedAnnotation;
                    return;
                }

                isDrawing = true;
                startPoint = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && resizingAnnotation != null)
            {
                int offsetX = e.X - dragStart.X;
                int offsetY = e.Y - dragStart.Y;
                Rectangle rect = resizingAnnotation.Area;

                switch (resizeDirection)
                {
                    case ResizeDirection.Left:
                        rect = new Rectangle(rect.X + offsetX, rect.Y, rect.Width - offsetX, rect.Height);
                        break;
                    case ResizeDirection.Right:
                        rect = new Rectangle(rect.X, rect.Y, rect.Width + offsetX, rect.Height);
                        break;
                    case ResizeDirection.Top:
                        rect = new Rectangle(rect.X, rect.Y + offsetY, rect.Width, rect.Height - offsetY);
                        break;
                    case ResizeDirection.Bottom:
                        rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + offsetY);
                        break;
                    case ResizeDirection.TopLeft:
                        rect = new Rectangle(rect.X + offsetX, rect.Y + offsetY, rect.Width - offsetX, rect.Height - offsetY);
                        break;
                    case ResizeDirection.TopRight:
                        rect = new Rectangle(rect.X, rect.Y + offsetY, rect.Width + offsetX, rect.Height - offsetY);
                        break;
                    case ResizeDirection.BottomLeft:
                        rect = new Rectangle(rect.X + offsetX, rect.Y, rect.Width - offsetX, rect.Height + offsetY);
                        break;
                    case ResizeDirection.BottomRight:
                        rect = new Rectangle(rect.X, rect.Y, rect.Width + offsetX, rect.Height + offsetY);
                        break;
                }

                if (rect.Width > 20 && rect.Height > 20)
                {
                    resizingAnnotation.Area = rect;
                    dragStart = e.Location;
                }

                pictureBox1.Invalidate();
            }
            else if (isDrawing)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);
                currentRectangle = new Rectangle(x, y, width, height);

                pictureBox1.Invalidate();
            }
            else if (isDragging)
            {
                int offsetX = e.X - dragStart.X;
                int offsetY = e.Y - dragStart.Y;

                draggedAnnotation.Area = new Rectangle(
                    draggedAnnotation.Area.X + offsetX,
                    draggedAnnotation.Area.Y + offsetY,
                    draggedAnnotation.Area.Width,
                    draggedAnnotation.Area.Height);
                dragStart = e.Location;

                pictureBox1.Invalidate();
            };

            DrawResizeCursor(e, pictureBox1);
        }

        private void DrawResizeCursor(MouseEventArgs e, PictureBox pictureBox)
        {
            foreach (var annotation in annotations)
            {
                ResizeDirection direction = GetResizeDirection(annotation, e.Location);

                if (direction != ResizeDirection.None)
                {
                    switch (direction)
                    {
                        case ResizeDirection.Left:
                        case ResizeDirection.Right:
                            pictureBox.Cursor = Cursors.SizeWE;
                            break;
                        case ResizeDirection.Top:
                        case ResizeDirection.Bottom:
                            pictureBox.Cursor = Cursors.SizeNS;
                            break;
                        //case ResizeDirection.TopLeft:
                        //case ResizeDirection.BottomRight:
                        //    pictureBox.Cursor = Cursors.SizeNWSE;
                        //    break;
                        //case ResizeDirection.TopRight:
                        //case ResizeDirection.BottomLeft:
                        //    pictureBox.Cursor = Cursors.SizeNESW;
                        //    break;
                    }
                    return;
                }
            }

            pictureBox.Cursor = Cursors.Default;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                var label = PromptForLabel(currentRectangle);

                if (!string.IsNullOrEmpty(label))
                {
                    annotations.Add(new Annotation
                    {
                        Area = currentRectangle,
                        Label = label
                    });
                }
                currentRectangle = Rectangle.Empty;
            }
            else if (isDragging)
            {
                isDragging = false;
                draggedAnnotation = null;
            }
            //else if (e.Button == MouseButtons.Right && selectedAnnotation != null)
            //{
            //    contextMenuStrip1.Show(pictureBox1, e.Location);
            //    isDrawing = false;
            //}

            isDragging = false;
            isResizing = false;
            draggedAnnotation = null;
            resizingAnnotation = null;

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (isDrawing)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, currentRectangle);
                }
            }

            DrawAnnotations(e);

            if (selectedAnnotation != null)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectedAnnotation.Area);
                }
            }
        }

        private void DrawAnnotations(PaintEventArgs e)
        {
            foreach (var annotation in annotations)
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    e.Graphics.DrawRectangle(pen, annotation.Area);
                    e.Graphics.FillRectangle(Brushes.White, annotation.Area);
                }

                using (var font = GetOptimalFont(e.Graphics, annotation.Label, annotation.Area, new Font("Arial", 24)))
                {
                    e.Graphics.DrawString(annotation.Label, font, Brushes.Black, annotation.Area);
                }
            }
        }

        private string PromptForLabel(Rectangle rectangle, string text = "")
        {
            using (var prompt = new Form())
            {
                prompt.Width = 500;
                prompt.Height = 500;
                prompt.Text = "Enter text";

                var textLabel = new Label() { Left = 20, Top = 15, Text = "Mark:" };
                var inputBox = new TextBox() { Left = 20, Top = 50, Width = 400, Height = 300, Multiline = true };
                inputBox.Text = text;

                var confirmation = new Button() { Text = "OK", Left = 280, Top = 350, Width = 80, Height = 80 };
                var translation = new Button() { Text = "Translate", Left = 120, Top = 350, Width = 120, Height = 80 };


                confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
                translation.Click += (sender, e) => { Translate(inputBox, rectangle); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(inputBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(translation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : string.Empty;
            }
        }

        private void EditAnnotation(object sender, EventArgs e)
        {
            if (selectedAnnotation != null)
            {
                var newLabel = PromptForLabel(selectedAnnotation.Area, selectedAnnotation.Label);
                if (!string.IsNullOrEmpty(newLabel))
                {
                    selectedAnnotation.Label = newLabel;
                    pictureBox1.Invalidate();
                }
            }
        }

        private void DeleteAnnotation(object sender, EventArgs e)
        {
            if (selectedAnnotation != null)
            {
                annotations.Remove(selectedAnnotation);
                selectedAnnotation = null;
                pictureBox1.Invalidate();
            }
        }

        private void SaveMarkingFile(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Save marking data";
            sfd.Filter = "Text files|*.txt|All files(*.*)|*.*";
            sfd.FileName = imageFilename.Split('\\').Last().Split('.').First() + ".txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveMarkingToFile(sfd.FileName);
                toolStripStatusLabel1.Text = $"Annotations saved {sfd.FileName}";
            }
        }

        private void SaveMarkingToFile(string fileName)
        {
            if (annotations.Any())
            {

                var sb = new StringBuilder();

                foreach (var annotation in annotations)
                {
                    sb.AppendLine($"[{annotation.Area.Left},{annotation.Area.Top}][{annotation.Area.Width},{annotation.Area.Height}]");
                    sb.AppendLine(annotation.Label);
                }

                File.WriteAllText(fileName, sb.ToString());
            }
        }

        private async Task Translate(TextBox textBox, Rectangle rectangle)
        {
            if (rectangle != null)
            {
                var crop = CropImage(pictureBox1.Image, rectangle);
                textBox.Text = "Please wait...";

                string apiKey = "Insert_API_key_here";
                var image = crop;

                var processor = new GeminiImageProcessor(apiKey);
                textBox.Text = await processor.ProcessImageAsync(image);
            }
            else
            {
                textBox.Text = "Nothing selected";
            };
        }

        static Bitmap CropImage(Image image, Rectangle cropArea)
        {
            var bitmap = new Bitmap(cropArea.Width, cropArea.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, new Rectangle(0, 0, cropArea.Width, cropArea.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        private Font GetOptimalFont(Graphics g, string text, Rectangle rect, Font baseFont, float maxSize = 24, float minSize = 6)
        {
            var fontSize = maxSize;
            var font = new Font(baseFont.FontFamily, fontSize, baseFont.Style);

            var textSize = g.MeasureString(text, font);

            while ((textSize.Width > rect.Width || textSize.Height > rect.Height) && fontSize > minSize)
            {
                fontSize -= 0.5f;
                font = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
                textSize = g.MeasureString(text, font);
            }

            return font;
        }

        private void LoadMarking(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Text files|*.txt|All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var newAnnotations = new List<Annotation>();

                var l = File.ReadAllLines(ofd.FileName);

                for (int i = 0; i < l.Length; i++)
                {
                    if (string.IsNullOrEmpty(l[i]))
                    {
                        continue;
                    }

                    var coords = l[i].Split(new char[] { '[', ']', ',' }).Where(x => x != "").Select(x => int.Parse(x)).ToArray();
                    var text = l[i + 1];
                    i++;

                    newAnnotations.Add(
                        new Annotation
                        {
                            Label = text,
                            Area = new Rectangle(coords[0], coords[1], coords[2], coords[3])
                        });
                }

                annotations = newAnnotations;
                pictureBox1.Invalidate();
                toolStripStatusLabel1.Text = $"Load annotations {ofd.FileName}";
            }
        }

        private ResizeDirection GetResizeDirection(Annotation annotation, Point mousePosition)
        {
            Rectangle rect = annotation.Area;

            bool left = Math.Abs(mousePosition.X - rect.Left) <= resizeMargin;
            bool right = Math.Abs(mousePosition.X - rect.Right) <= resizeMargin;
            bool top = Math.Abs(mousePosition.Y - rect.Top) <= resizeMargin;
            bool bottom = Math.Abs(mousePosition.Y - rect.Bottom) <= resizeMargin;

            if (top && left) return ResizeDirection.TopLeft;
            if (top && right) return ResizeDirection.TopRight;
            if (bottom && left) return ResizeDirection.BottomLeft;
            if (bottom && right) return ResizeDirection.BottomRight;
            if (left) return ResizeDirection.Left;
            if (right) return ResizeDirection.Right;
            if (top) return ResizeDirection.Top;
            if (bottom) return ResizeDirection.Bottom;

            return ResizeDirection.None;
        }

    }
}