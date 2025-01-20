using System.Text;

namespace MangaTranslatorHelper
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private Point startPoint;
        private Rectangle currentRectangle;
        private List<Annotation> annotations = new List<Annotation>();
        private Annotation selectedAnnotation = null;


        public Form1()
        {
            InitializeComponent();

            contextMenuStrip1 = new ContextMenuStrip();
            contextMenuStrip1.Items.Add("Edit", null, EditAnnotation);
            contextMenuStrip1.Items.Add("Delete", null, DeleteAnnotation);

            pictureBox1.ContextMenuStrip = contextMenuStrip1;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image files|*.png;*.jpeg;*.jpg;*.bmp;*.gif|All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                annotations.Clear();
                pictureBox1.Image = Image.FromFile(ofd.FileName);
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
                    //MessageBox.Show($"Annotation selected: {selectedAnnotation.Label}");
                    pictureBox1.Invalidate();
                    return;
                }

                isDrawing = true;
                startPoint = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(startPoint.X - e.X);
                int height = Math.Abs(startPoint.Y - e.Y);
                currentRectangle = new Rectangle(x, y, width, height);

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                var label = PromptForLabel();
                //var label = "Test text";

                if (!string.IsNullOrEmpty(label))
                {
                    annotations.Add(new Annotation
                    {
                        Area = currentRectangle,
                        Label = label
                    });

                    //MessageBox.Show($"Area saved: {currentRectangle}, Mark: {label}");
                }

                currentRectangle = Rectangle.Empty;

            }
            else if (e.Button == MouseButtons.Right && selectedAnnotation != null)
            {
                contextMenuStrip1.Show(pictureBox1, e.Location);
                isDrawing = false;
            }

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

            foreach (var annotation in annotations)
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    e.Graphics.DrawRectangle(pen, annotation.Area);
                }

                using (Font font = new Font("Arial", 10))
                using (Brush brush = new SolidBrush(Color.Blue))
                {
                    e.Graphics.DrawString(annotation.Label, font, brush, annotation.Area);
                }
            }

            if (selectedAnnotation != null)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectedAnnotation.Area);
                }
            }
        }

        private string PromptForLabel()
        {
            using (var prompt = new Form())
            {
                prompt.Width = 500;
                prompt.Height = 500;
                prompt.Text = "Enter text";

                var textLabel = new Label() { Left = 20, Top = 15, Text = "Mark:" };
                var inputBox = new TextBox() { Left = 20, Top = 50, Width = 400, Height = 300, Multiline = true };

                var confirmation = new Button() { Text = "OK", Left = 280, Top = 350, Width = 80, Height = 80 };
                var translation = new Button() { Text = "Translate", Left = 120, Top = 350, Width = 120, Height = 80 };


                confirmation.Click += (sender, e) => { prompt.DialogResult = DialogResult.OK; prompt.Close(); };
                translation.Click += (sender, e) => { inputBox.Text = Translate(); };

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
                string newLabel = PromptForLabel();
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

        private void button3_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Save marking data";
            sfd.Filter = "Text files|*.txt|All files(*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveMarkingToFile(sfd.FileName);
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

        private string Translate()
        {
            return "Translation not implemented";
        }
    }
}