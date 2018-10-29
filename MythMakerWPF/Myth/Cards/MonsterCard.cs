using MythMaker.Math;
using MythMaker.Myth.Elements;
using MythMaker.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace MythMaker.Myth.Cards
{
    [System.Diagnostics.DebuggerDisplay("MonsterCard: {Title}")]
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class MonsterCard : BigCard
    {
        #region DataMember Properties & Serialization
        [DataMember(Name = "Vitality", IsRequired = true)]
        int vitality = 1;
        [DataMember(Name = "TN", IsRequired = true)]
        int tn = 4;
        [DataMember(Name = "Movement", IsRequired = true)]
        int movement = 3;
        [DataMember(Name = "Courage", IsRequired = true)]
        int courage = 6;
        [DataMember(Name = "Rank", IsRequired = true)]
        string rankForSerialization;
        [DataMember(Name = "Abilities", IsRequired = true)]
        string abilities = "Ability 1: Description\r\n\r\nAbility 2: Description";
        [DataMember(Name = "Immunities", EmitDefaultValue = false)]
        string immunitiesForSerialization;
        [DataMember(Name = "Type", IsRequired = false)]
        string type = "";
        [DataMember(Name = "Class", EmitDefaultValue = false)]
        string classForSerialization;
        [DataMember(Name = "Priority", EmitDefaultValue = false)]
        string priorityForSerialization;
        [DataMember(Name = "Activation", IsRequired = true)]
        int activation = 1;
        [DataMember(Name = "AP", IsRequired = true)]
        int ap = 1;
        [DataMember(Name = "TnFactor", IsRequired = true)]
        int tnFactor = 1;
        [DataMember(Name = "Attacks", IsRequired = true)]
        MonsterAttack[] attacks = new MonsterAttack[] { };
        [DataMember(Name = "ColorHue", IsRequired = true)]
        int colorHue = 0;

        [OnSerializing]
        protected new void OnSerializing(StreamingContext context)
        {
            base.OnSerializing(context);

            classForSerialization = _class.Length > 0 ? string.Join(", ", _class) : null;
            priorityForSerialization = priority.Length > 0 ? string.Join(", ", priority) : null;
            immunitiesForSerialization = immunities.Length > 0 ? string.Join(", ", immunities) : null;
            rankForSerialization = rank.DisplayName;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            classForSerialization = "";
            priorityForSerialization = "";
            immunitiesForSerialization = "";
        }

        [OnDeserialized]
        public new void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);

            _class = classForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            priority = priorityForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            immunities = immunitiesForSerialization.Split(new string[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
            rank = rankForSerialization;
            if (abilityKeywords == null)
                abilityKeywords = new string[] { };
            if (generalAbilityKeywords == null)
                generalAbilityKeywords = new string[] { };
            if (abilities == null)
                abilities = "";
            foreach (var attack in attacks)
                attack.PropertyChanged += Attack_PropertyChanged;
            abilityKeywords = ParseAbilities(Abilities);
            UpdateAbilityKeywords(true);

            Init();
        }
        #endregion

        #region properties
        public int Vitality
        {
            get { return vitality; }
            set
            {
                vitality = value;
                OnCardPropertyChanged("Vitality");
            }
        }

        public int TN
        {
            get { return tn; }
            set
            {
                tn = value;
                OnCardPropertyChanged("TN");
            }
        }

        public int Movement
        {
            get { return movement; }
            set
            {
                movement = value;
                OnCardPropertyChanged("Movement");
            }
        }

        public int Courage
        {
            get { return courage; }
            set
            {
                courage = value;
                OnCardPropertyChanged("Courage");
            }
        }

        public bool AllowsCourage
        {
            get { return Rank <= MonsterRank.Commander; }
        }

        MonsterRank rank = MonsterRank.Default;
        public MonsterRank Rank
        {
            get { return rank; }
            set
            {
                rank = value;
                OnCardPropertyChanged("Rank");
                OnPropertyChanged("AllowsType");
                OnPropertyChanged("AllowsCourage");
                OnPropertyChanged("AllowsActivation");
                OnPropertyChanged("AllowsAP");
            }
        }

        public string Abilities
        {
            get { return abilities; }
            set
            {
                abilities = value;
                AbilityKeywords = ParseAbilities(abilities);
                OnCardPropertyChanged("Abilities", true);
            }
        }

        string[] immunities = new string[] { };
        public string[] Immunities
        {
            get { return immunities; }
            set
            {
                immunities = value;
                OnCardPropertyChanged("Immunities", true);
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnCardPropertyChanged("Type");
            }
        }

        public bool AllowsType
        {
            get
            {
                return (Rank != MonsterRank.Agent);
            }
        }

        string[] _class = new string[] { };
        public string[] Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnCardPropertyChanged("Class");
            }
        }

        string[] priority = new string[] { };
        public string[] Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnCardPropertyChanged("Priority");
            }
        }

        public Range Activation
        {
            get { return activation; }
            set
            {
                activation = value;
                OnCardPropertyChanged("Activation");
            }
        }
        public bool AllowsActivation
        {
            get { return Rank != MonsterRank.Boss; }
        }

        public int AP
        {
            get { return ap; }
            set
            {
                ap = value;
                OnCardPropertyChanged("AP");
            }
        }
        public bool AllowsAP
        {
            get { return Rank == MonsterRank.Boss; }
        }

        protected string[] abilityKeywords = new string[] { };
        public string[] AbilityKeywords
        {
            get { return abilityKeywords; }
            set
            {
                abilityKeywords = value;
                UpdateAbilityKeywords();
            }
        }

        protected string[] generalAbilityKeywords = new string[] { };
        public string[] GeneralAbilityKeywords
        {
            get { return generalAbilityKeywords; }
            set
            {
                generalAbilityKeywords = value;
                OnCardPropertyChanged("GeneralAbilityKeywords");
            }
        }

        public int TnFactor
        {
            get { return tnFactor; }
            set
            {
                tnFactor = value;
                OnCardPropertyChanged("TnFactor");
            }
        }

        public MonsterAttack[] Attacks
        {
            get { return attacks; }
            set
            {
                if (attacks == null)
                    throw new System.Exception();

                foreach (var attack in attacks)
                    attack.PropertyChanged -= Attack_PropertyChanged;
                attacks = value;
                foreach (var attack in attacks)
                    attack.PropertyChanged += Attack_PropertyChanged;
                OnCardPropertyChanged("Attacks");
            }
        }

        private void Attack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAbilityKeywords();
            OnCardPropertyChanged("Attacks");
        }

        public int ColorHue
        {
            get { return colorHue; }
            set
            {
                colorHue = value;
                OnCardPropertyChanged("ColorHue");
            }
        }
        #endregion

        #region Debug properties
        protected string debugRenderSpeed = "";
        public string DebugRenderSpeed
        {
            get { return debugRenderSpeed; }
            set
            {
                debugRenderSpeed = value;
                OnPropertyChanged("DebugRenderSpeed");
            }
        }

        protected string debugRenderBackSpeed = "";
        public string DebugRenderBackSpeed
        {
            get { return debugRenderBackSpeed; }
            set
            {
                debugRenderBackSpeed = value;
                OnPropertyChanged("DebugRenderBackSpeed");
            }
        }
        #endregion

        /* Common fonts */
        public static readonly Rendering.Font AttributesFont = new Rendering.Font(Rendering.FontFamily.Mason, 10.0f, 0);
        public static readonly Rendering.Font PriorityFont = new Rendering.Font(Rendering.FontFamily.Mason, 9.7f, 0);
        public static readonly Rendering.Font PriorityListFont = new Rendering.Font(Rendering.FontFamily.Helvetica, 6.0f, 30.0f);
        public static readonly Rendering.Font TypeRankFont = new Rendering.Font(Rendering.FontFamily.Mason, 6.2f, 0);
        public static readonly Rendering.Font ClassFont = new Rendering.Font(Rendering.FontFamily.Helvetica, 5.70f, 0);
        public static readonly Rendering.Font TitleFont = new Rendering.Font(Rendering.FontFamily.Mason, 13.5f, 0);
        public static readonly Rendering.Font AttackFont = new Rendering.Font(Rendering.FontFamily.Mason, 8.8f, 0);
        public static readonly Rendering.Font AttackDiceFont = new Rendering.Font(Rendering.FontFamily.Helvetica, 8.6f, 0);
        public static readonly Rendering.Font DescriptionFont = new Rendering.Font(Rendering.FontFamily.Helvetica, 6.8f, 31.0f);

        /* Mini-Boss, Boss, Agent specific fonts */
        public static readonly Rendering.Font BossTitleFont = new Rendering.Font(Rendering.FontFamily.Mason, 10.8f, 46);
        public static readonly Rendering.Font BossAttributesFont = new Rendering.Font(Rendering.FontFamily.Mason, 7.8f, 0);

        /* Common brushes */
        public static readonly SolidBrush RedBrush = new SolidBrush(Color.FromArgb(153, 27, 30));
        public static readonly SolidBrush WhiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
        public static readonly SolidBrush LightTanBrush = new SolidBrush(Color.FromArgb(240, 223, 192));
        public static readonly SolidBrush DarkBrush = new SolidBrush(Color.FromArgb(35, 31, 32));

        public MonsterCard(MythDocument document) : base(document)
        {
            Init();
            title = "New Monster";
            AbilityKeywords = ParseAbilities(abilities);
            RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutFront);
            RenderWorker.Instance.EnqueueCardUpdate(this, CardRenderType.CutBack);
        }

        void Init()
        {
            resourcePrefix = "monsters/";
            imageAutoScale = new Vector(700, 800);
            imageIdealScale = new Vector(600, 600);
        }

        public override Bitmap RenderFront(float scaling)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Render r = new Render(cardSize.Width, cardSize.Height, scaling);

            if (Rank <= MonsterRank.Commander)
                renderMinionBackground(r);
            else
                renderBossBackground(r);

            stopwatch.Stop();
            var elapsedTemplate = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // general part
            if (Rank <= MonsterRank.Commander)
                RenderMinionGeneral(r, r.Graphics);
            else
                RenderBossGeneral(r);

            // priority
            RenderPriority(r);

            // attacks
            RenderAttacks(r);

            // removeBleeding
            if (scaling > 1.1f) // todo, for now this hacky way
                r.ApplyFilter(new Rendering.Filters.RemoveBleeding(34));

            stopwatch.Stop();
            var elapsedContent = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // full image
            {
                fullImage = ImageConverter.GetBitmapSource(r.Bitmap);
                FullImage.Freeze();
                //OnPropertyChanged("TinyPreview");
            }

            stopwatch.Stop();
            var elapsedPreviews = stopwatch.ElapsedMilliseconds;

            System.Windows.Application.Current?.Dispatcher.InvokeAsync(() =>
            {
                DebugRenderSpeed = string.Format("Total {0}ms\r\nContent {1}ms", elapsedPreviews + elapsedContent + elapsedTemplate, elapsedContent);
            });

            return r.Bitmap;
        }

        public override void RenderPreview(float scaling = 2)
        {
            // tiny preview
            using (Render tiny = new Render(48, 48, scaling))
            {
                tiny.Graphics.Clear(Color.Transparent);

                if (Picture != null)
                {
                    float scalingFactor = Rank < MonsterRank.MiniBoss ? 0.5f : 0.6f;
                    Vector offset = Rank < MonsterRank.MiniBoss ? new Vector(0, 10) : new Vector(5, 17);
                    Bitmap itemPicture = Picture.Image.GetBitmap(Picture.ImagePreScaling * scalingFactor * 0.22f);
                    tiny.DrawImage(itemPicture, new Vector(24, 24) + offset, Alignment.MiddleCenter);
                }
                TinyPreview = ImageConverter.GetBitmapSource(tiny.Bitmap);
                TinyPreview.Freeze();
            }
        }

        private int renderBackCount = 0;
        public override Bitmap RenderBack(float scaling = 2)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            Render r = new Render(cardSize.Width, cardSize.Height, scaling);

            // colored background
            // todo: colorize
            r.DrawColorized(ImageCache.Get("resources/monster-minion-back-color.png"), new Vector(0, 0), ColorHue);
            r.DrawFastImage(ImageCache.Get("resources/monster-minion-back.png"), new Vector(0, 0));

            stopwatch.Stop();
            var elapsedTemplate = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // title
            {
                var font = new Rendering.Font(Rendering.FontFamily.Mason, 11.7f, 0);
                r.DrawString("Abilities", WhiteBrush, font, new Vector(408, 65), Alignment.TopCenter);
            }

            // abilities
            {
                var font = new Rendering.Font(Rendering.FontFamily.Helvetica, 6.8f, 38.0f, 20.0f);
                TextParsePattern2[] patterns = new TextParsePattern2[]
                {
                    new TextParsePattern2("[^,\n\r]+:", Rendering.FontStyle.ItalicBold)
                };
                Render.TextPiece[] pieces = r.ParseText(Abilities, font, patterns, Rendering.FontStyle.Italic);
                
                r.DrawText(pieces, DarkBrush, new Vector(70, 155), new Vector(678, 800));
            }

            // immunities
            if (Immunities.Length > 0)
            {
                // todo immunities is yellowish
                r.DrawString("Immunities: " + string.Join(", ", Immunities), WhiteBrush, new RectangleF(0, 1020, 816, 30), MythFontStyle.Centered | MythFontStyle.Regular);
            }

            // removeBleeding
            if (scaling > 1.1f) // todo, for now this hacky way
                r.ApplyFilter(new Rendering.Filters.RemoveBleeding(34));

            stopwatch.Stop();
            var elapsedContent = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // full image
            {
                backImage = ImageConverter.GetBitmapSource(r.Bitmap);
                backImage.Freeze();
                //OnPropertyChanged("TinyPreview");
            }

            stopwatch.Stop();
            var elapsedPreviews = stopwatch.ElapsedMilliseconds;

            renderBackCount++;
            
            System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                DebugRenderBackSpeed = string.Format("Total {0}ms\r\nContent {1}ms\r\nRender count {2}", 
                    elapsedPreviews + elapsedContent + elapsedTemplate, elapsedContent, renderBackCount);
            });

            return r.Bitmap;
        }

        private void renderMinionBackground(Render r)
        {
            // colored background
            // todo: colorize
            r.DrawColorized(ImageCache.Get("resources/monster-minion-background.png"), new Vector(0, 0), ColorHue);

            // image
            if (Picture != null)
            {
                // picture
                Bitmap itemPicture = Picture.Image.GetBitmap(Picture.ImagePreScaling);
                Bitmap itemShadow = DropShadow.CreateShadow(itemPicture, 12, 1.0f, 1.5f);
                Vector itemPicturePosition = new Vector(816 / 2, 330);
                Vector itemPictureOffset = new Vector(0, 0);// new PointF(1, 46);
                if (itemPicture != null)
                {
                    r.DrawImage(itemShadow, itemPicturePosition + itemPictureOffset, Alignment.MiddleCenter);
                    r.DrawImage(itemPicture, itemPicturePosition + itemPictureOffset, Alignment.MiddleCenter);
                }
            }

            // top
            r.DrawFastImage(ImageCache.Get("resources/monster-minion-top.png"), new Vector(0, 0));

            // text background 
            r.DrawFastImage(ImageCache.Get("resources/monster-minion-textbackground.png"), new Vector(0, 278));
        }

        private void renderBossBackground(Render r)
        {
            // colored background
            r.DrawColorized(ImageCache.Get("resources/monster-minion-background.png"), new Vector(0, 0), ColorHue);

            // image
            if (Picture != null)
            {
                // picture
                Bitmap itemPicture = Picture.Image.GetBitmap(Picture.ImagePreScaling * 0.7f);
                Bitmap itemShadow = DropShadow.CreateShadow(itemPicture, 12, 1.0f, 1.5f);
                Vector itemPicturePosition = new Vector(620, 230);
                Vector itemPictureOffset = new Vector(0, 0);// new PointF(1, 46);
                if (itemPicture != null)
                {
                    r.DrawImage(itemShadow, itemPicturePosition + itemPictureOffset, Alignment.MiddleCenter);
                    r.DrawImage(itemPicture, itemPicturePosition + itemPictureOffset, Alignment.MiddleCenter);
                }
            }

            // text background 
            if (Rank == MonsterRank.MiniBoss)
                r.DrawColorized(ImageCache.Get("resources/monster-miniboss-textbackground.png"), new Vector(0, 0), ColorHue);
            else
                r.DrawColorized(ImageCache.Get("resources/monster-boss-textbackground.png"), new Vector(0, 0), ColorHue);
        }
        private void RenderPriority(Render r)
        {
            // priority list
            {
                Vector titleOffset = (Rank > MonsterRank.Commander) ? new Vector(0, 150) : new Vector(0, 0);
                Vector listOffset = (Rank > MonsterRank.Commander) ? new Vector(0, 132) : new Vector(0, 0);
                if (Rank == MonsterRank.MiniBoss)
                {
                    titleOffset = new Vector(0, -155);
                    listOffset = new Vector(0, -155);
                }

                r.DrawString("Priority", WhiteBrush, PriorityFont, new Vector(670, 503) + titleOffset, Alignment.TopCenter);

                Image separator = ImageCache.Get("resources/monster-priority-separator.png");
                r.DrawImage(separator, new Vector(539, 555.5f) + titleOffset, Alignment.TopLeft);

                var priorityListFont = new Rendering.Font(Rendering.FontFamily.Helvetica, 7.0f, 45.0f);
                var position = new Vector(670, 578) + listOffset;
                foreach (var prio in priority)
                {
                    r.DrawString(prio, LightTanBrush, priorityListFont, position, Rendering.FontStyle.SemiBold, Alignment.TopCenter);
                    position.Y += priorityListFont.LineHeight;
                }
            }

            // activation
            if (AllowsActivation)
            {
                Image activation = ImageCache.Get("resources/activation.png");
                r.DrawImage(activation, new Vector(670, 938), Alignment.MiddleCenter);
                // todo render "tile" smaller
                r.DrawString(Activation.ToString(), RedBrush, AttributesFont, new Vector(670, 942), Alignment.MiddleCenter);
            }

            // ap
            if (AllowsAP && ap > 0)
            {
                Image img = ImageCache.Get("resources/boss-ap" + ap.ToString() + ".png");
                r.DrawImage(img, new Vector(669.5f, 928.5f), Alignment.MiddleCenter);
            }
        }

        private void RenderBossGeneral(Render r)
        {
            // title
            {
                // todo shadow
                var lines = string.Join("\r\n", Utils.TextUtils.SplitInHalf(Title));
                r.DrawText(lines, WhiteBrush, BossTitleFont, new Vector(153, 67), Alignment.TopLeft);
            }

            // monster type & class
            {
                // todo shadow
                var typeAndRank = "";
                if (Type.Length > 0 && AllowsType)
                    typeAndRank += "Type: " + Type + ", ";
                typeAndRank += "Rank: " + Rank.DisplayName;
                r.DrawString(typeAndRank, LightTanBrush, TypeRankFont, new Vector(408, 232),
                    Alignment.TopCenter);
                r.DrawString(string.Join(", ", Class), LightTanBrush, ClassFont, new Vector(409, 265),
                    Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
            }

            // TN
            {
                Image tn = ImageCache.Get("resources/boss-tn" + (TnFactor > 1 ? "2" : "1") + ".png");
                r.DrawImage(tn, new Vector(107, 96.5f), new Vector(53, 69), Alignment.MiddleCenter);
                float tnFactorOffset = TnFactor > 1 ? 6 : 0;
                r.DrawString(TN.ToString(), DarkBrush, BossAttributesFont, new Vector(107, 99.5f + tnFactorOffset), Alignment.MiddleCenter);
                var TnFactorFont = new Rendering.Font(Rendering.FontFamily.Mason, 3.8f, 0);
                if (TnFactor > 1)
                    r.DrawString(TnFactor.ToString() + "x", WhiteBrush, TnFactorFont, new Vector(106, 76.5f), Alignment.MiddleCenter);
            }

            // vitality
            {
                Image vitality = ImageCache.Get("resources/monster-vitality.png");
                r.DrawImage(vitality, new Vector(107, 185), new Vector(87, 87), Alignment.MiddleCenter);
                r.DrawString(Vitality.ToString(), WhiteBrush, BossAttributesFont, new Vector(107, 194), Alignment.MiddleCenter);
            }

            // Movement
            {
                Image movement = ImageCache.Get("resources/monster-movement.png");
                r.DrawImage(movement, new Vector(107.5f, 268), new Vector(70, 70), Alignment.MiddleCenter);
                r.DrawString(Movement.ToString(), DarkBrush, BossAttributesFont, new Vector(108, 271), Alignment.MiddleCenter);
            }
        }

        private void RenderMinionGeneral(Render r, Graphics g)
        {
            // icons
            /*float iconWidth = 0;
            if (Document != null && Document.Logo != null)
            {
                iconWidth = 60;
                var projectLogo = Document.Logo.GeneratePreview(new Size(48, 48));
                r.DrawImage(projectLogo, new PointF(476, 100), RendererAlign.MiddleCenter);
            }*/

            // title
            {
                // todo shadow
                r.DrawString(Title, WhiteBrush, TitleFont, new Vector(408, 295), Alignment.TopCenter);
            }

            // type
            /*Image itemType = ItemType != 0 ? System.Drawing.Image.FromFile("resources/item-type-" + this.ItemType + ".png") : null;
            if (itemType != null)
                r.DrawImage(itemType, new PointF(473, 187), RendererAlign.MiddleCenter);
                */
            // picture
            /*Bitmap itemPicture = Image != null ? Image.GetBitmap(ImagePreScaling) : null;
            Bitmap itemShadow = Image != null ? DropShadow.CreateShadow(itemPicture, 12, 1.0f, 1.5f) : null;
            PointF itemPicturePosition = new PointF(590 / 2 + 0.5f, 257.5f);
            SizeF itemPictureOffset = new SizeF(0, 0);// new PointF(1, 46);
            if (itemPicture != null)
            {
                r.DrawImage(itemShadow, itemPicturePosition + itemPictureOffset, RendererAlign.MiddleCenter);
                r.DrawImage(itemPicture, itemPicturePosition + itemPictureOffset, RendererAlign.MiddleCenter);
            }
            */

            // monster type & class
            {
                // todo shadow
                var typeAndRank = "";
                if (Type.Length > 0 && AllowsType)
                    typeAndRank += "Type: " + Type + ", ";
                typeAndRank += "Rank: " + Rank.DisplayName;
                r.DrawString(typeAndRank, LightTanBrush, TypeRankFont, new Vector(408, 357),
                    Alignment.TopCenter);
                r.DrawString(string.Join(", ", Class), LightTanBrush, ClassFont, new Vector(409, 390),
                    Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
            }

            // bonus text
            /*TextParsePattern[] patterns = new TextParsePattern[]
            {
                new TextParsePattern("[^,\n\r]+:", MythFontStyle.SemiBold)
            };
            r.DrawBox(Abilities, new RectangleF(92, 150, 700, 800), MythFontStyle.Italic, RendererAlign.TopLeft, patterns);
            */
            // TN
            if (TN > 0)
            {
                Image tn = ImageCache.Get("resources/monster-tn" + (TnFactor > 1 ? "2" : "1") + ".png");
                r.DrawImage(tn, new Vector(125.5f, 123.5f), Alignment.MiddleCenter);
                float tnFactorOffset = TnFactor > 1 ? 10 : 0;
                r.DrawString(TN.ToString(), DarkBrush, AttributesFont, new Vector(125.5f, 127.5f + tnFactorOffset), Alignment.MiddleCenter);
                var TnFactorFont = new Rendering.Font(Rendering.FontFamily.Mason, 5.75f, 0);
                if (TnFactor > 1)
                    r.DrawString(TnFactor.ToString() + "x", WhiteBrush, TnFactorFont, new Vector(125, 99.5f), Alignment.MiddleCenter);
            }
            
            // vitality
            if (Vitality > 0)
            {
                Image vitality = ImageCache.Get("resources/monster-vitality.png");
                r.DrawImage(vitality, new Vector(123.5f, 369), Alignment.MiddleCenter);
                r.DrawString(Vitality.ToString(), WhiteBrush, AttributesFont, new Vector(123.5f, 381), Alignment.MiddleCenter);
            }

            // Movement
            if (Movement > 0)
            {
                Image movement = ImageCache.Get("resources/monster-movement.png");
                r.DrawImage(movement, new Vector(680.2f, 130.2f), Alignment.MiddleCenter);
                r.DrawString(Movement.ToString(), DarkBrush, AttributesFont, new Vector(680.5f, 133.5f), Alignment.MiddleCenter);
            }

            // Courage
            if (Courage > 0 && AllowsCourage)
            {
                Image courage = ImageCache.Get("resources/monster-courage.png");
                r.DrawImage(courage, new Vector(680.5f, 377.5f), Alignment.MiddleCenter);
                r.DrawString(Courage.ToString(), DarkBrush, AttributesFont, new Vector(680.5f, 380.5f), Alignment.MiddleCenter);
            }
        }

        private void RenderAttacks(Render r)
        {
            Vector offset = (Rank > MonsterRank.Commander) ? new Vector(0, -154.5f) : new Vector(0, 0);
            Vector size = (Rank >= MonsterRank.Boss) ? new Vector(500, 290) : new Vector(520, 505);

            // general abilities
            Vector abilitiesMeasurements = new Vector(0, 0);
            if (GeneralAbilityKeywords.Length > 0)
            {
                var font = new Rendering.Font(Rendering.FontFamily.Helvetica, 6.8f, 31.0f);
                var text = string.Join(", ", GeneralAbilityKeywords);
                abilitiesMeasurements = r.DrawText(text, DarkBrush, font, new Vector(70, 498) + offset, size, Rendering.FontStyle.ItalicBold);
                abilitiesMeasurements.Y += 29;
            }

            // attacks
            Vector position = new Vector(65, 495 + abilitiesMeasurements.Y) + offset;
            foreach (var attack in Attacks)
            {
                Vector rendered = new Vector(0, 0);

                // dice
                Vector bossOffset = new Vector(Rank <= MonsterRank.MiniBoss ? 0 : 208, 0);
                r.DrawString(attack.Type.ToString(), RedBrush, AttackFont, position, Alignment.TopLeft);
                float distD10 = 85.5f;
                float distFD = attack.FD > 0 ? 73.0f : 0;
                if (attack.D10 > 0)
                {
                    r.DrawString(attack.D10.ToString(), RedBrush, AttackDiceFont, position + new Vector(339.5f - distD10 - distFD, 0.5f) + bossOffset, Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
                    r.DrawImage(ImageCache.Get("resources/attack-d10.png"), position + new Vector(381.0f - distD10 - distFD, -4.5f) + bossOffset, Alignment.TopCenter);
                }
                if (attack.FD > 0)
                {
                    r.DrawString(attack.FD.ToString(), RedBrush, AttackDiceFont, position + new Vector(339.5f - distFD, 0.5f) + bossOffset, Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
                    r.DrawImage(ImageCache.Get("resources/attack-fd.png"), position + new Vector(374.55f - distFD, -3.0f) + bossOffset, Alignment.TopCenter);
                }
                r.DrawString(attack.NumberOfAttacks.ToString(), RedBrush, AttackDiceFont, position + new Vector(339.5f, 0.5f) + bossOffset, Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
                r.DrawImage(ImageCache.Get("resources/attack-hit" + attack.HitFactor + ".png"), position + new Vector(372.0f, -4.5f) + bossOffset, Alignment.TopCenter);
                r.DrawString(attack.TN.ToString(), RedBrush, AttackDiceFont, position + new Vector(414, 0.5f) + bossOffset, Rendering.FontStyle.ItalicSemiBold, Alignment.TopCenter);
                r.DrawImage(ImageCache.Get("resources/attack-tn.png"), position + new Vector(451, -4.5f) + bossOffset, Alignment.TopCenter);

                // damage, range & text
                var font = new Rendering.Font(Rendering.FontFamily.Helvetica, 6.8f, 31.0f);
                var damageText = attack.PerSuccess ? (attack.Damage + " per success") : attack.Damage.ToString();
                var rangeText = attack.RangeIsMinimum ? (attack.Range + "+") : attack.Range.ToString();
                Render.TextPiece damageRangePiece = new Render.TextPiece("Damage: " + damageText + ", Range: " + rangeText, font, Rendering.FontStyle.Italic);
                Render.TextPiece[] pieces;

                var abilitiesText = string.Join(", ", attack.Abilities);
                if (abilitiesText.Length > 0)
                {
                    damageRangePiece.Text += ", ";
                    pieces = new Render.TextPiece[]
                    {
                        damageRangePiece,
                        new Render.TextPiece(abilitiesText, font, Rendering.FontStyle.ItalicBold)
                    };
                }
                else
                    pieces = new Render.TextPiece[] { damageRangePiece };

                Vector attackTextMeasured = r.DrawText(pieces, DarkBrush, position + new Vector(6, 62), new Vector(Rank <= MonsterRank.MiniBoss ? 470 : 780, 100));

                rendered.Y += attackTextMeasured.Y + 82;
                position += rendered;

                // fate recipe
                if (attack.HasFateRecipe)
                {
                    var background = ImageCache.Get("resources/monster-fate-recipe-background.png");
                    r.DrawImage(background, position + new Vector(-30, -10), Alignment.TopLeft);

                    Vector fateSize = new Vector(69, 0);
                    Vector orSize = new Vector(35, 0);
                    Vector fateOffset = position + new Vector(452.0f, -12.0f);

                    foreach (var die in attack.FateRecipeDice)
                        fateOffset -= die == "or" ? orSize : fateSize;
                    for (int i = 0; i < attack.FateRecipeDice.Length; i++)
                    {
                        if (attack.FateRecipeDice[i] == "or")
                        {
                            r.DrawString("or", DarkBrush, font, fateOffset + new Vector(7, 43), Rendering.FontStyle.Italic, Alignment.MiddleLeft);
                            fateOffset += orSize;
                        }
                        else
                        {
                            r.DrawImage(Fate.GetImage(attack.FateRecipeDice[i]), new Rect(fateOffset.X, fateOffset.Y, 83, 82), Alignment.TopLeft);
                            fateOffset += fateSize;
                        }
                    }

                    r.DrawString(attack.FateRecipe, DarkBrush, font, position + new Vector(2, 31), Rendering.FontStyle.ItalicBold, Alignment.MiddleLeft);
                }

                position.Y += 93;
            }
        }

        private string[] ParseAbilities(string text)
        {
            string[] paragraphs = text.Split(new string[] { "\r\n\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            List<string> abilities = new List<string>();
            foreach (var paragraph in paragraphs)
            {
                int dots = paragraph.IndexOf(":");
                if (dots > 0)
                {
                    abilities.Add(paragraph.Substring(0, dots));
                }
            }

            return abilities.ToArray();
        }

        private void UpdateAbilityKeywords(bool deserializing = false)
        {
            HashSet<string> used = new HashSet<string>();
            foreach (var attack in Attacks)
            {
                used.UnionWith(attack.Abilities);
                if (attack.FateRecipe.Length > 0)
                    used.Add(attack.FateRecipe);
            }
            

            List<string> remaining = new List<string>();
            foreach (var ability in AbilityKeywords)
            {
                if (!used.Contains(ability))
                    remaining.Add(ability);
            }

            if (deserializing)
                generalAbilityKeywords = remaining.ToArray();
            else
                GeneralAbilityKeywords = remaining.ToArray();
        }

        public new void SetImage(MythBitmap bitmap)
        {
            base.SetImage(bitmap);
            Color average = Rendering.Utils.BitmapUtils.GetAverageColor(bitmap.Image, new IntRect(0, 0, bitmap.Width, bitmap.Height));
            ColorHue = (int)(new ColorHSL(average).H * 60.0f);
        }
    }
}
