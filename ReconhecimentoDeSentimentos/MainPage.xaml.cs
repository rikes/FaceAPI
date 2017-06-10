using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Media.Capture;//Camera
using Windows.Storage;//Manipulacao do arquivo
using Windows.Storage.Streams;
using Windows.Graphics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;

using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;

using Windows.Storage.Pickers;

namespace ReconhecimentoDeSentimentos
{
    public sealed partial class MainPage : Page
    {
        CameraCaptureUI captureUI = new CameraCaptureUI();
        StorageFile photo;
        IRandomAccessStream imageStream;

        const string emotionKey = "53349a89ad7148cba0590fc965a39d96";
        EmotionServiceClient emotionService = new EmotionServiceClient(emotionKey);
        Emotion[] emotionResults;//É um array de json pois identificara todos os rostos
        
        public MainPage()
        {
            this.InitializeComponent();
            this.captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            this.captureUI.PhotoSettings.CroppedSizeInPixels = new Size(250, 200);


        }
        /*
         *Metodo responsavel por capturar uma imagem e enviar a MainPage
         * Mais informações: https://docs.microsoft.com/pt-br/windows/uwp/audio-video-camera/capture-photos-and-video-with-cameracaptureui
         */
        private async void take_photo(object sender, RoutedEventArgs e)
        {
            try
            {
                //Captura assincrona
                photo = await this.captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
                
                // Se o Usuario cancelou a captura da foto
                if (photo == null)
                {
                    
                    return;
                }
                else
                {
                    //Carrego a foto
                    this.imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softBitmap = await decoder.GetSoftwareBitmapAsync();
                    
                    //Converto com as exigencias de exibição na pagina XAML
                    SoftwareBitmap softBitmapBGR8 = SoftwareBitmap.Convert(softBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softBitmapBGR8);
                    
                    //Anexo ao campo "image" a foto armazenada
                    image.Source = bitmapSource;
                }


            }
            catch
            {
                //Envio a mensagem de erro pelo campo text que criei na tela
                output.Text = "Erro: taking photo";   
            }
         }

        private async void getEmotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emotionResults = await emotionService.RecognizeAsync(imageStream.AsStream());
                
                if (emotionResults != null)
                {
                    //Para testes só usurei um, mas poderia haver mais;
                    int i = 0;
                    foreach (var p in emotionResults)
                    {
                        
                        var score = p.Scores;
                        output.Text += "Your Emotions are for photo #" +i+ "  : \n" +

                         "Happiness: " + String.Format("{0:0.##}", score.Happiness * 100) + " %" + "\n" +

                         "Sadness: " + String.Format("{0:0.##}", score.Sadness * 100) + " %" + "\n" +

                         "Surprise: " + String.Format("{0:0.##}", score.Surprise * 100) + " %" + "\n" +

                         "Anger: " + String.Format("{0:0.##}", score.Anger * 100) + " %" + "\n" +

                         "Contempt: " + String.Format("{0:0.##}", score.Contempt * 100) + " %" + "\n" +

                         "Disgust: " + String.Format("{0:0.##}", score.Disgust * 100) + " %" + "\n" +

                         "Fear: " + String.Format("{0:0.##}", score.Fear * 100) + " %" + "\n" +

                         "Neutral: " + String.Format("{0:0.##}", score.Neutral * 100) + " %" + "\n";
                       i++;
                    }
                    

                }
            }
            catch(Exception ex)
            {
                output.Text = "Erro: Check Emotions \n" + ex.Message + "\n";
            }


        }

        private async void getPhotoLocal_Click(object sender, RoutedEventArgs e)
        {

            FileOpenPicker open = new FileOpenPicker();
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");
            // Open a stream for the selected file 
            StorageFile file = await open.PickSingleFileAsync();
            
            // Ensure a file was selected 
            if (file != null)
            {
                imageStream = await file.OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                SoftwareBitmap softBitmap = await decoder.GetSoftwareBitmapAsync();
                
                //Converto com as exigencias de exibição na pagina XAML
                SoftwareBitmap softBitmapBGR8 = SoftwareBitmap.Convert(softBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(softBitmapBGR8);

                //Anexo ao campo "image"
                image.Source = bitmapSource;

                /*
                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                await bitmapImage.SetSourceAsync(fileStream);
                //Scenario2Image.Source = bitmapImage;
                image.Source = bitmapImage; */

            }
            else
            {
                output.Text = "Cancelou o envio de imagem";
            }
        }
    }
}
