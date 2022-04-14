using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LandmarkAI.Tags;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image file (*.png;*.jpg)|*.png;*.jpg;*jpeg|All files (*.*)|*.*"; // filtering by default only type image format is display
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); //set default folder when image button is click
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(filename));

                MakePredictionAsync(filename);
            }
            
        }

        private async void MakePredictionAsync(string filename)
        {
            string url = "https://southeastasia.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2befc128-331a-4925-857a-e9e40cf0ccd7/classify/iterations/Iteration1/image";
            string predictionKey = "3c0de30a27fe461ab6cc603dc9a470e5";
            string contentType = "application/octet-stream";

            var file = File.ReadAllBytes(filename);

            //Make request method
            using (HttpClient webClient = new HttpClient())
            {
                webClient.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    var responses = await webClient.PostAsync(url, content);

                    var responseString = await responses.Content.ReadAsStringAsync();
                    List<Prediction> myDeserializedClass = (JsonConvert.DeserializeObject<CustomVision>(responseString)).predictions;
                    predictionListView.ItemsSource = myDeserializedClass;
                }
            }
        }
    }
}
