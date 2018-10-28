using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using MythMaker.Myth.Cards;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MythMaker.Myth
{
    public class MythDocumentChangedEventArgs : EventArgs
    {
        public MythDocument Old;
        public MythDocument New;
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class MythDocument : INotifyPropertyChanged
    {
        #region DataMember & Serliazation
        [DataMember(Name = "Title", IsRequired = true)]
        public string Title;

        [DataMember(Name = "Logo", EmitDefaultValue = false)]
        public MythBitmap Logo;

        [DataMember(Name = "ExtraKeywords", EmitDefaultValue = false)]
        private string extraKeywordsForSerialization;

        [DataMember(Name = "Items", IsRequired = true)]
        private Utils.ObservableDataList<ItemCard> items = new Utils.ObservableDataList<ItemCard>();

        [DataMember(Name = "Monsters", IsRequired = true)]
        private Utils.ObservableDataList<MonsterCard> monsters = new Utils.ObservableDataList<MonsterCard>();

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            extraKeywordsForSerialization = ExtraKeywords.Length > 0 ? string.Join(", ", ExtraKeywords) : null;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            extraKeywordsForSerialization = "";
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            ExtraKeywords = extraKeywordsForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

            items.CollectionChanged += Items_CollectionChanged;
            monsters.CollectionChanged += Items_CollectionChanged;
            InitAutoSave();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Modified = true;
        }
        #endregion

        public string[] ExtraKeywords = new string[] { };
        public string FileName { get; private set; }
        public bool HasFileName { get { return FileName != null; } }

        HashSet<string> resourceIDs = new HashSet<string>();

        public IEnumerable<ItemCard> Items
        {
            get { return items; }
        }

        public IEnumerable<MonsterCard> Monsters
        {
            get { return monsters; }
        }

        #region Modified Property & Auto Save
        private bool modified = false;
        private System.Windows.Threading.DispatcherTimer autosaveTimer;
        public bool Modified
        {
            get { return modified; }
            private set
            {
                modified = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Modified"));
            }
        }

        private void InitAutoSave()
        {
            autosaveTimer = new System.Windows.Threading.DispatcherTimer();
            autosaveTimer.Tick += AutosaveTimer_Tick;

            autosaveTimer.Interval = new TimeSpan(0, 0, 30);
            autosaveTimer.Start();
        }

        private void AutosaveTimer_Tick(object sender, EventArgs e)
        {
            AutoSave();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Active MythDocument
        public static event EventHandler<MythDocumentChangedEventArgs> OnActiveChanged;
        private static MythDocument active;
        public static MythDocument Active
        {
            get
            {
                return active;
            }
            set
            {
                var args = new MythDocumentChangedEventArgs() { Old = active, New = value };
                active = value;
                OnActiveChanged?.Invoke(active, args);
            }
        }
        #endregion

        public MythDocument()
        {
            Rendering.RenderWorker.Instance.Clear();

            Title = "untitled";
            items.CollectionChanged += Items_CollectionChanged;
            monsters.CollectionChanged += Items_CollectionChanged;
            InitAutoSave();
        }

        public static MythDocument Open(string file)
        {
            Rendering.RenderWorker.Instance.Clear();

            MythDocument doc;

            DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(MythDocument));
            using (FileStream fs = File.Open(file, FileMode.Open))
                doc = xmlSerializer.ReadObject(fs) as MythDocument ?? new MythDocument();
            doc.FileName = file;

            if (doc.Logo != null)
            {
                doc.Logo.Document = doc;
                if (!doc.Logo.Validate())
                    doc.Logo = null;
            }

            foreach (var item in doc.Items)
                item.LinkDocument(doc);
            foreach (var monster in doc.Monsters)
                monster.LinkDocument(doc);

            doc.resourceIDs = new HashSet<string>();

            return doc;
        }

        public void AutoSave()
        {
            if (Modified)
                Save(null);
        }

        public void Save(string file)
        {
            Rendering.RenderWorker.Instance.Pause();

            if (file == null)
                file = FileName;

            string tmp = file + ".tmp";

            // already set the filename, so that MythBitmaps are storing to the new temp
            FileName = file;
            Title = System.IO.Path.GetFileName(file);

            // clear resource IDs
            resourceIDs = new HashSet<string>();

            var settings = new System.Xml.XmlWriterSettings { Indent = true };
            using (var fs = System.Xml.XmlWriter.Create(tmp, settings))
            {
                DataContractSerializer xmlSerializer = new DataContractSerializer(typeof(MythDocument));
                xmlSerializer.WriteObject(fs, this);
            }
            if (File.Exists(file))
                File.Delete(file);
            File.Move(tmp, file);
            
            if (Directory.Exists(file + ".data"))
            {
                try
                {
                    Directory.Delete(file + ".data", true);
                }
                catch
                {
                    // todo when in explorer open, it will crash
                }
            }
            if (Directory.Exists(tmp + ".data"))
                Directory.Move(tmp + ".data", file + ".data");
            Modified = false;

            Rendering.RenderWorker.Instance.Continue();
        }

        public ItemCard NewItem()
        {
            var item = new ItemCard(this);
            items.Add(item);
            Modified = true;
            return item;
        }

        public MonsterCard NewMonster()
        {
            var monster = new MonsterCard(this);
            monsters.Add(monster);
            Modified = true;
            return monster;
        }

        public void ExportPrinterStudio(string path, bool bleeing)
        {
            void saveAllCards(string folderName, IEnumerable<Card> cards)
            {
                string folder = path + "/" +  folderName;
                Directory.CreateDirectory(folder);

                int i = 0;
                foreach (var card in cards)
                {
                    var bitmap = card.RenderFront(1);
                    string fileName = string.Format("{0}/front_{1:D3}_{2}.png", folder, i, card.FileTitle);
                    bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    bitmap = card.RenderBack(1);
                    if (bitmap != null)
                    {
                        fileName = string.Format("{0}/back_{1:D3}_{2}.png", folder, i, card.FileTitle);
                        bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    i++;
                }
            }

            Rendering.RenderWorker.Instance.Pause();
            saveAllCards("items", Items);
            saveAllCards("monsters", Monsters);
            Rendering.RenderWorker.Instance.Continue();
        }

        public string GetResourceID(string id)
        {
            if (resourceIDs.Contains(id))
            {
                int i = 1;
                while (resourceIDs.Contains(id + i))
                    i++;
                id = id + i;
            }
            resourceIDs.Add(id);
            return id;
        }
    }
}
