using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Tema3CNN.Models;

namespace Tema3CNN.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private CNNModel _model;
        private string _prediction;
        private BitmapImage _loadedImage;

        public string Prediction
        {
            get => _prediction;
            set
            {
                _prediction = value;
                OnPropertyChanged(nameof(Prediction));
            }
        }

        public BitmapImage LoadedImage
        {
            get => _loadedImage;
            set
            {
                _loadedImage = value;
                OnPropertyChanged(nameof(LoadedImage));
            }
        }

        public ICommand LoadImageCommand { get; }
        public ICommand PredictCommand { get; }

        private string _imagePath;

        public MainViewModel()
        {
            _model = new CNNModel("cifar10_mobileNet.onnx");
            LoadImageCommand = new RelayCommand(LoadImage);
            PredictCommand = new RelayCommand(Predict);
        }

        private void LoadImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                LoadedImage = bitmap;
            }
        }

        private void Predict()
        {
            try
            {
                if (string.IsNullOrEmpty(_imagePath))
                {
                    System.Windows.MessageBox.Show("Vă rugăm să încărcați o imagine înainte de a prezice.");
                    return;
                }

                var input = CNNModel.ProcessImage(_imagePath);
                var result = _model.Predict(input);
                string prediction="";
                switch(result)
                {
                    case 0:
                        prediction = "Avion";
                        break;
                    case 1:
                        prediction = "Automobil";
                        break;
                    case 2:
                        prediction = "Pasăre";
                        break;
                    case 3:
                        prediction = "Pisică";
                        break;
                    case 4:
                        prediction = "Căprioară";
                        break;
                    case 5:
                        prediction = "Câine";
                        break;
                    case 6:
                        prediction = "Broască";
                        break;
                    case 7:
                        prediction = "Cal";
                        break;
                    case 8:
                        prediction = "Navă";
                        break;
                    case 9:
                        prediction = "Camion";
                        break;
                    default:
                        break;
                }

                 Prediction = $"Predicție: {prediction}";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la predicție: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
