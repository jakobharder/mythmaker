using System.Runtime.Serialization;

namespace MythMaker.Myth.Cards
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public abstract class SmallCard : Card
    {
        protected SmallCard(MythDocument document) : base(document)
        {
            cardSize = new System.Drawing.Size(590, 860);
        }

        [OnDeserialized]
        protected new void OnDeserialized(StreamingContext context)
        {
            base.OnDeserialized(context);

            cardSize = new System.Drawing.Size(590, 860);
        }
    }
}
