using System;
using System.Drawing;
using System.Windows.Forms;

namespace KR_Algorithms
{
    public partial class WebViewerForm : Form
    {
        public WebViewerForm(string filePath)
        {
            InitializeComponent();
            webBrowser1.Url = new Uri(filePath);
            this.Size = new Size(800, 600);
        }
    }
}
