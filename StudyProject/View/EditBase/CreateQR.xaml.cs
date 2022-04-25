using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.Windows.Forms;
namespace StudyProject.View.EditBase
{
    /// <summary>
    /// Логика взаимодействия для CreateQR.xaml
    /// </summary>
    public partial class CreateQR : Window
    {

        private Model.Good good;
        public CreateQR(Model.Good good)
        {
            InitializeComponent();
            this.good = good;
        }
        private void GoodCountOnlyDigit(object sender, TextCompositionEventArgs e) //check of input in real time
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!GoodCount.Text.Contains(".")
               && GoodCount.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var good_ser = new BE.GoodSerialized(good, Convert.ToInt32(GoodCount.Text));
            string json = JsonSerializer.Serialize<BE.GoodSerialized>(good_ser);
            QRCodeEncoder encoder = new QRCodeEncoder(); //сreates an QRCodeEncoder object
            Bitmap qrcode = encoder.Encode(json); //encodes the data to the bitmap  = QR code of item
            SaveFileDialog save = new SaveFileDialog(); // "save" asks user for directory where to save 
            save.Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp"; //filter for formats to save QR in.
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK) //checks if the user chose an option "save"
            {
                qrcode.Save(save.FileName);
            }
            this.Close();
        }
    }
}
