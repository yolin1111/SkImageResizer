using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
//TODO 目前的 ResizeImagesAsync 非同步方法，其實是直接複製 ResizeImages 同步方法的內容而已，並不是「真」非同步方法喔！請修改 ResizeImagesAsync 非同步方法，用比較有效率的方式執行圖片縮放功能！
namespace SkImageResizer
{
    public class SKImageProcess
    {
        /// <summary>
        /// 進行圖片的縮放作業
        /// </summary>
        /// <param name="sourcePath">圖片來源目錄路徑</param>
        /// <param name="destPath">產生圖片目的目錄路徑</param>
        /// <param name="scale">縮放比例</param>
        public void ResizeImages(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                var bitmap = SKBitmap.Decode(filePath);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                using var scaledBitmap = bitmap.Resize(
                    new SKImageInfo(destinationWidth, destinationHeight),
                    SKFilterQuality.High);
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                data.SaveTo(s);
            }
        }

        public async Task ResizeImagesAsync(string sourcePath, string destPath, double scale)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            await Task.Yield();

            var allFiles = FindImages(sourcePath);
            foreach (var filePath in allFiles)
            {
                var bitmap = SKBitmap.Decode(filePath);
                var imgPhoto = SKImage.FromBitmap(bitmap);
                var imgName = Path.GetFileNameWithoutExtension(filePath);

                var sourceWidth = imgPhoto.Width;
                var sourceHeight = imgPhoto.Height;

                var destinationWidth = (int)(sourceWidth * scale);
                var destinationHeight = (int)(sourceHeight * scale);

                using var scaledBitmap = bitmap.Resize(
                    new SKImageInfo(destinationWidth, destinationHeight),
                    SKFilterQuality.High);
                using var scaledImage = SKImage.FromBitmap(scaledBitmap);
                using var data = scaledImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                using var s = File.OpenWrite(Path.Combine(destPath, imgName + ".jpg"));
                data.SaveTo(s);
            }
        }

        /// <summary>
        /// 清空目的目錄下的所有檔案與目錄
        /// </summary>
        /// <param name="destPath">目錄路徑</param>
        public void Clean(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            else
            {
                var allImageFiles = Directory.GetFiles(destPath, "*", SearchOption.AllDirectories);

                foreach (var item in allImageFiles)
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 找出指定目錄下的圖片
        /// </summary>
        /// <param name="srcPath">圖片來源目錄路徑</param>
        /// <returns></returns>
        public List<string> FindImages(string srcPath)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpg", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(srcPath, "*.jpeg", SearchOption.AllDirectories));
            return files;
        }
    }
}