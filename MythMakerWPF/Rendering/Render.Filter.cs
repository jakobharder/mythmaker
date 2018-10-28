using System.Drawing;

namespace MythMaker.Rendering
{
    public interface IFilter
    {
        void Run(System.Drawing.Imaging.BitmapData bmpData, float scaling);
    }

    public partial class Render
    {
		public void ApplyFilter(IFilter filter)
        {
            var bmpData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), 
				System.Drawing.Imaging.ImageLockMode.ReadWrite, 
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            filter.Run(bmpData, scaling);

            Bitmap.UnlockBits(bmpData);
        }
    }
}
