using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace Tema3CNN.Models
{
    public class CNNModel
    {
        private InferenceSession _session;

        public CNNModel(string modelPath)
        {
            try
            {
                _session = new InferenceSession(modelPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la încărcarea modelului ONNX: {ex.Message}");
                throw;
            }
        }

        public int Predict(float[] input)
        {
            try
            {
                var inputTensor = new DenseTensor<float>(input, new[] { 1, 3, 32, 32 });
                var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("input", inputTensor) };
                var results = _session.Run(inputs);
                var output = results.First().AsEnumerable<float>().ToArray();
                return Array.IndexOf(output, output.Max()); 
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la inferență: {ex.Message}");
                throw;
            }
        }


        public static float[] ProcessImage(string imagePath)
        {
            try
            {
                using var bmp = new Bitmap(imagePath);
                var resized = new Bitmap(bmp, new Size(32, 32));

                float[] normalized = new float[3 * 32 * 32];

                for (int c = 0; c < 3; c++) 
                {
                    for (int y = 0; y < 32; y++)
                    {
                        for (int x = 0; x < 32; x++)
                        {
                            Color pixel = resized.GetPixel(x, y);
                            float value = c switch
                            {
                                0 => pixel.R,   
                                1 => pixel.G,   
                                2 => pixel.B,   
                                _ => 0
                            };

                            normalized[c * 32 * 32 + y * 32 + x] = (value / 255.0f - 0.5f) / 0.5f;
                        }
                    }
                }

                return normalized;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Eroare la procesarea imaginii: {ex.Message}");
                throw;
            }
        }

    }
}
