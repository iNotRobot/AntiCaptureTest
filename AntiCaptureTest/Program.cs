using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace AntiCaptureTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Decode();
        }
               
        static void Decode()
        {
            const int width = 320;
            const int BitInByte = 32;
            var file = new StreamReader("../../../../AntiCaptureTest/Data/src.txt");

            var heigh = 0;
            var line = file.ReadLine();
            do
            {
                heigh++;
            }
            while ((line = file.ReadLine()) != null);

            BitMatrix qrMatrix = new BitMatrix(width, heigh);

            file.BaseStream.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < heigh; i++)
            {
                var fileRow = file.ReadLine();
                BitArray row = new BitArray(width);

                for (int j = 0; j < row.Array.Length; j++)
                {
                    System.Collections.BitArray num = new System.Collections.BitArray(BitInByte);
                    for (int k = 0; k < BitInByte; k++)
                    {
                        num[k] = fileRow[j * BitInByte + 1] == '1';
                    }

                    int[] array = new int[1];
                    num.CopyTo(array, 0);
                    row.Array[j] = array[0];
                }

                qrMatrix.setRow(i, row);
            }

            file.Close();

            BarcodeWriter qrWrite = new BarcodeWriter();    //класс для кодирования QR в растровом файле
            var qrImage = qrWrite.Write(qrMatrix);   //создание изображения
            qrImage.Save("../../../../AntiCaptureTest/Data/Generated_QR.bmp", System.Drawing.Imaging.ImageFormat.Bmp);//сохранение изображения
            Console.WriteLine("Please open Generated_QR.bmp in Data folder");
            Console.ReadLine();
        }

        static void Encode(string toEncode)
        {
            const int width = 320;
            const int heigh = 320;
            QRCodeWriter qrEncode = new QRCodeWriter(); //создание QR кода

            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
            hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");   //добавление в коллекцию кодировки utf-8
            BitMatrix qrMatrix = qrEncode.encode(   //создание матрицы QR
                toEncode,                 //кодируемая строка
                BarcodeFormat.QR_CODE,  //формат кода, т.к. используется QRCodeWriter применяется QR_CODE
                width,                    //ширина
                heigh,                    //высота
                hints);                 //применение колекции поведений

            var file = new StreamWriter("../../../../AntiCaptureTest/Data/src.txt");
            for (int i = 0; i < qrMatrix.Dimension; i++)
            {
                BitArray row = new BitArray();
                row = qrMatrix.getRow(i, row);
                for (int j = 0; j < row.Array.Length; j++)
                {
                    var bytes = BitConverter.GetBytes(row.Array[j]);
                    System.Collections.BitArray y = new System.Collections.BitArray(bytes);
                    int[] bits = y.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();
                    foreach (var b in bits)
                    {
                        file.Write(b);
                    }
                }

                file.WriteLine();
            }

            file.Close();
        }

    }
}
