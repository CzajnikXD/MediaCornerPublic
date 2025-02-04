using System.Windows;

namespace MediaCornerWPF.View
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(double? height = null, double? width = null, TextAlignment? textAlignment = null)
        {
            InitializeComponent();

            this.Height = height ?? 210;
            this.Width = width ?? 300;

            TextAlignment alignment = textAlignment ?? TextAlignment.Center;
            MessageTextBlock.TextAlignment = alignment; 
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCloseMsg_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; 
            this.Close();
        }
    }
}