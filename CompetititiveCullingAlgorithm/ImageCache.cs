using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompetititiveCullingAlgorithm
{
    class ImageCache
    {
        public class RefCountedImage
        {
            private int refCount = 1;
            public String Path;
            public Task<Image> Task { get; set; }

            public RefCountedImage Ref()
            {
                Debug.Assert(refCount > 0); // not back from the dead
                refCount++;
                return this;
            }

            public void Unref()
            {
                Debug.Assert(refCount > 0); // not buried a second time
                if (--refCount == 0)
                    Task.Result.Dispose();
            }
        }

        LRUCache<string, RefCountedImage> imagesByPath = new LRUCache<string, RefCountedImage>(24);

        public ImageCache()
        {
            imagesByPath.ElementEvictedEvent += ImagesByPath_ElementEvictedEvent;
        }

        private void ImagesByPath_ElementEvictedEvent(string key, RefCountedImage value)
        {
            Console.WriteLine($"-{key}");
            value.Unref();
        }

        private Size GetMaxUsefulSize()
        {
            return new Size
            {
                Width = Screen.AllScreens.Max(screen => screen.Bounds.Width) / 2,
                Height = Screen.AllScreens.Max(screen => screen.Bounds.Height)
            };
        }

        private Size ZoomResize(Size container, Size image)
        {
            var ratioIfWidthAdjusted = (double)container.Width / image.Width;
            var ratioIfHeightAdjusted = (double)container.Height / image.Height;

            var widthIfHeightAdjusted = image.Width * ratioIfHeightAdjusted;
            var heightIfWidthAdjusted = image.Height * ratioIfHeightAdjusted;

            var zoomRatio = Math.Min(1.0, widthIfHeightAdjusted <= container.Width
                ? ratioIfHeightAdjusted : ratioIfWidthAdjusted);
            return new Size
            {
                Height = (int) Math.Round(zoomRatio * image.Height),
                Width = (int) Math.Round(zoomRatio * image.Width)
            };
        }

        private Image ResizeToUsefulSize(Image image)
        {
            Size size = ZoomResize(GetMaxUsefulSize(), image.Size);
            Bitmap resizedImage = new Bitmap(image, size);
            image.Dispose();
            return resizedImage;
        }

        // Caller must .Ref()
        public RefCountedImage LoadAsync(string path)
        {
            if (!imagesByPath.ContainsKey(path))
            {
                Console.WriteLine($"+{path}");
                var rcImage = new RefCountedImage { Path = path };
                rcImage.Task = Task.Run(() =>
                {
                    try
                    {
                        return ResizeToUsefulSize(Image.FromFile(path, true));
                    }
                    finally
                    {
                        // Loading the image (running this code) takes one reference, being in the cache takes another one,
                        // being used or awaited from the GUI takes a third one.
                        // Making running this code take one reference avoids NPE if the image is evicted before finishing 
                        // loading.
                        Console.WriteLine($"@{path}");
                        rcImage.Unref();
                    }
                });
                imagesByPath.Add(path, rcImage.Ref());
            }
            return imagesByPath.TryUse(path);
        }

        public void ReplaceCache(List<string> wantedPaths)
        {
            foreach (var path in wantedPaths.Reverse<string>())
                LoadAsync(path);
        }
    }
}
