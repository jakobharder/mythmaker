using MythMaker.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MythMaker.Myth.Cards
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public abstract class Card : INotifyPropertyChanged, IRenderCard, Utils.IDataChanged
    {
        #region DataMember & Serialization
        [DataMember(Name = "Title", IsRequired = true)]
        protected string title;
        [DataMember(Name = "Picture", EmitDefaultValue = false)]
        protected CardPicture picture;

        [OnSerializing]
        protected void OnSerializing(StreamingContext context)
        {
            picture?.Image?.UpdateID(MythDocument.Active.GetResourceID(resourcePrefix + FileTitle) + ".png");
        }
        #endregion

        #region DataMember Component Properties
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnCardPropertyChanged("Title");
            }
        }

        public CardPicture Picture
        {
            get { return picture; }
            set
            {
                if (picture != null)
                    picture.PropertyChanged -= Picture_PropertyChanged;
                picture = value;
                if (picture != null)
                    picture.PropertyChanged += Picture_PropertyChanged;
                OnCardPropertyChanged("Picture");
            }
        }

        private void Picture_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCardPropertyChanged("Picture");
        }
        #endregion

        #region Component Properties
        protected BitmapSource fullImage;
        public BitmapSource FullImage
        {
            get
            {
                if (fullImage == null)
                {
                    //Generate(new PointF(192, 192));
                }
                return fullImage;
            }
        }

        protected BitmapSource backImage;
        public BitmapSource BackImage
        {
            get
            {
                if (backImage == null)
                {
                    //GenerateBack(new PointF(192, 192));
                }
                return backImage;
            }
        }

        public BitmapSource TinyPreview { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler DataChanged;

        protected void OnCardPropertyChanged(string name, bool backSide = false)
        {
            if (!backSide)
            {
                RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutFront);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutBack);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            DataChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region MythDocument
        public MythDocument Document { get; private set; }

        public void LinkDocument(MythDocument document)
        {
            Document = document;
            if (Picture != null)
            {
                Picture.Image.Document = document;
                if (!Picture.Image.Validate())
                    Picture = null;
                else
                    RenderPreview();
            }
        }
        #endregion
        
        public string FileTitle
        {
            get
            {
                string str = Title.ToLower();
                str = str.Replace(" ", "-");
                str = str.Replace("'", "");
                str = str.Replace("&", "and");
                return str;
            }
        }

        protected Size cardSize;
        protected string resourcePrefix;

        protected Card(MythDocument document)
        {
            Document = document;
        }

        #region Rendering
        public abstract Bitmap RenderFront(float scaling = 2);
        public abstract void RenderPreview(float scaling = 2);
        public virtual Bitmap RenderBack(float scaling = 2) { return null; }

        public void Render(CardRenderType renderType, bool invoke = true)
        {
            if (renderType == CardRenderType.CutFront)
            {
                RenderFront();
                RenderPreview();
                if (invoke)
                {
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FullImage"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TinyPreview"));
                    });
                }
            }
            else
            {
                RenderBack();
                if (invoke)
                {
                    System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackImage"));
                    });
                }
            }
        }

        public void Refresh()
        {
            // refresh both
            RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutFront);
            RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutBack);
        }
        #endregion

        protected Math.Vector imageAutoScale;
        protected Math.Vector imageIdealScale;
        public void SetImage(MythBitmap bitmap)
        {
            var picture = new CardPicture();

            // calculate pre-scaling
            if (bitmap.Width < 150 && bitmap.Height < 150)
                picture.ImagePreScaling = new Math.Vector(2, 2);
            else if (bitmap.Width < 175 && bitmap.Height < 175)
                picture.ImagePreScaling = new Math.Vector(1.5f, 1.5f);
            else if (bitmap.Width > 500 || bitmap.Height > 450)
            {
                float scale = System.Math.Min(imageIdealScale.X / bitmap.Width, imageIdealScale.Y / bitmap.Height);
                picture.ImagePreScaling = new Math.Vector(scale, scale);
            }

            // reset manual scaling & position
            picture.ImageScaling = new Math.Vector(1, 1);
            picture.ImageOffset = new Math.Vector(0, 0);

            // set the image
            picture.Image = bitmap;

            Picture = picture;
        }
    }
}
