using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompetititiveCullingAlgorithm
{
    class ImageCache
    {
        public class RefCountedImage
        {
            public RefCountedImage(Image image)
            {
                Image = image;
            }

            public Image Image { get; }

            private int refCount = 1;

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
                    Image.Dispose();
            }
        }

        Dictionary<string, Task<RefCountedImage>> imagesByPath = new Dictionary<string, Task<RefCountedImage>>();

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

        public Task<RefCountedImage> LoadAsync(string path)
        {
            if (!imagesByPath.ContainsKey(path))
            {
                Console.WriteLine($"+{path}");
                imagesByPath[path] = Task.Run(() => new RefCountedImage(ResizeToUsefulSize(Image.FromFile(path, true))));
            }
            return imagesByPath[path];
        }

        public void ReplaceCache(List<string> wantedPaths)
        {
            var wantedPathsSet = new HashSet<string>(wantedPaths);

            var unwantedCacheEntries = imagesByPath.Where(kv =>
                !wantedPathsSet.Contains(kv.Key) && kv.Value.IsCompleted).ToList();
            foreach (var entry in unwantedCacheEntries)
            {
                entry.Value.Result.Unref(); // TODO bug, may still being used by the window
                Console.WriteLine($"-{entry.Key}");
                imagesByPath.Remove(entry.Key);
            }

            foreach (var path in wantedPaths)
                LoadAsync(path);
        }
    }
}
