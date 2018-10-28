using System.Runtime.Serialization;

namespace MythMaker.Myth.Cards
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public abstract class BigCard : Card
    {
        protected BigCard(MythDocument document) : base(document)
        {
            cardSize = new System.Drawing.Size(816, 1110);
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            cardSize = new System.Drawing.Size(816, 1110);
        }
    }
}
