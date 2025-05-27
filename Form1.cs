using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KR_Algorithms
{
    public partial class Form1 : Form
    {
        private SplayTree tree = new SplayTree();
        private Timer animationTimer = new Timer();
        private const int NODE_RADIUS = 20;
        private const int HORIZONTAL_SPACING = 200;
        private const int VERTICAL_SPACING = 80;

        public Form1()
        {
            InitializeComponent();
            animationTimer.Interval = 16; // ~60 FPS
            animationTimer.Tick += (s, e) => pictureBox1.Invalidate();
            animationTimer.Start();
        }

        // Node class
        public class Node
        {
            public int Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float TargetX { get; set; }
            public float TargetY { get; set; }
            public bool Highlight { get; set; }
            public DateTime HighlightEnd { get; set; }

            public Node(int value)
            {
                Value = value;
                Left = null;
                Right = null;
                X = 0;
                Y = 0;
                TargetX = 0;
                TargetY = 0;
                Highlight = false;
            }
        }

        // SplayTree class
        public class SplayTree
        {
            public Node Root { get; set; }

            public SplayTree()
            {
                Root = null;
            }

            public void Insert(int value)
            {
                if (Root == null)
                {
                    Root = new Node(value);
                    return;
                }

                Root = Splay(value);

                if (Root.Value == value) return;

                Node newNode = new Node(value);
                if (value < Root.Value)
                {
                    newNode.Right = Root;
                    newNode.Left = Root.Left;
                    Root.Left = null;
                }
                else
                {
                    newNode.Left = Root;
                    newNode.Right = Root.Right;
                    Root.Right = null;
                }
                Root = newNode;
                HighlightNode(Root, 1000);
            }

            public Node Splay(int value)
            {
                if (Root == null) return null;

                Node dummy = new Node(0);
                Node left = dummy;
                Node right = dummy;
                Node current = Root;

                while (true)
                {
                    if (value < current.Value)
                    {
                        if (current.Left == null) break;

                        if (value < current.Left.Value)
                        {
                            Node temp = current.Left;
                            current.Left = temp.Right;
                            temp.Right = current;
                            current = temp;
                            if (current.Left == null) break;
                        }

                        right.Left = current;
                        right = current;
                        current = current.Left;
                    }
                    else if (value > current.Value)
                    {
                        if (current.Right == null) break;

                        if (value > current.Right.Value)
                        {
                            Node temp = current.Right;
                            current.Right = temp.Left;
                            temp.Left = current;
                            current = temp;
                            if (current.Right == null) break;
                        }

                        left.Right = current;
                        left = current;
                        current = current.Right;
                    }
                    else
                    {
                        break;
                    }
                }

                left.Right = current.Left;
                right.Left = current.Right;
                current.Left = dummy.Right;
                current.Right = dummy.Left;

                return current;
            }

            public void HighlightNode(Node node, int durationMs)
            {
                node.Highlight = true;
                node.HighlightEnd = DateTime.Now.AddMilliseconds(durationMs);
            }
        }

        private void DrawNode(Graphics g, Node node, Node parent, int level = 1, bool isLeft = false)
        {
            if (node == null) return;

            // Calculate target position
            if (parent == null)
            {
                node.TargetX = pictureBox1.Width / 2;
                node.TargetY = 50;
            }
            else
            {
                float spacing = HORIZONTAL_SPACING / (float)(level * 0.7);
                node.TargetX = parent.X + (isLeft ? -spacing : spacing);
                node.TargetY = parent.Y + VERTICAL_SPACING;
            }

            // Animate position
            node.X += (node.TargetX - node.X) * 0.1f;
            node.Y += (node.TargetY - node.Y) * 0.1f;

            // Draw connection to parent
            if (parent != null)
            {
                using (Pen pen = new Pen(Color.FromArgb(node.Highlight ? 255 : 128, 100, 100, 100)))
                {
                    g.DrawLine(pen, parent.X, parent.Y + NODE_RADIUS, node.X, node.Y - NODE_RADIUS);
                }
            }

            // Draw node
            using (Brush fillBrush = new SolidBrush(node.Highlight ? Color.FromArgb(235, 178, 87) : Color.White))
            using (Pen borderPen = new Pen(node.Highlight ? Color.FromArgb(235, 178, 87) : Color.Black))
            {
                g.FillEllipse(fillBrush, node.X - NODE_RADIUS, node.Y - NODE_RADIUS, NODE_RADIUS * 2, NODE_RADIUS * 2);
                g.DrawEllipse(borderPen, node.X - NODE_RADIUS, node.Y - NODE_RADIUS, NODE_RADIUS * 2, NODE_RADIUS * 2);
            }

            // Draw value
            using (Font font = new Font("Arial", 12))
            using (Brush textBrush = new SolidBrush(node.Highlight ? Color.FromArgb(235, 178, 87) : Color.Black))
            {
                g.DrawString(node.Value.ToString(), font, textBrush, node.X, node.Y, new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                });
            }

            // Update highlight status
            if (node.Highlight && DateTime.Now > node.HighlightEnd)
                node.Highlight = false;

            // Draw children
            DrawNode(g, node.Left, node, level + 1, true);
            DrawNode(g, node.Right, node, level + 1, false);
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int value))
            {
                tree.Insert(value);
                pictureBox1.Invalidate();
            }
        }

        private void splayButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int value))
            {
                tree.Root = tree.Splay(value);
                if (tree.Root != null && tree.Root.Value == value)
                    tree.HighlightNode(tree.Root, 1000);
                pictureBox1.Invalidate();
            }
        }

        private void randomButton_Click(object sender, EventArgs e)
        {
            tree.Root = null;
            Random rand = new Random();
            int count = rand.Next(5, 11);
            for (int i = 0; i < count; i++)
            {
                int value = rand.Next(1, 101);
                tree.Insert(value);
                pictureBox1.Invalidate();
                System.Threading.Thread.Sleep(500); // Simple delay for visual effect
                Application.DoEvents();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (tree.Root != null)
                DrawNode(e.Graphics, tree.Root, null);
        }

        private void theoryButton_Click(object sender, EventArgs e)
        {
            string theoryPath = Path.Combine(Application.StartupPath, "docs", "theory.html");
            WebViewerForm viewer = new WebViewerForm(theoryPath);
            viewer.Show();
        }

        private void quizButton_Click(object sender, EventArgs e)
        {
            string quizPath = Path.Combine(Application.StartupPath, "docs", "quiz.html");
            WebViewerForm viewer = new WebViewerForm(quizPath);
            viewer.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
