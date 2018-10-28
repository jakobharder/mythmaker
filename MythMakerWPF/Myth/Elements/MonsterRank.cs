using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MythMaker.Myth.Elements
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/MythMaker.Myth")]
    public class MonsterRank
    {
        public static readonly MonsterRank[] Ranks = new MonsterRank[]
        {
            new MonsterRank(0) { DisplayName = "Minion" },
            new MonsterRank(1) { DisplayName = "Captain" },
            new MonsterRank(2) { DisplayName = "Commander" },
            new MonsterRank(3) { DisplayName = "Mini-Boss" },
            new MonsterRank(4) { DisplayName = "Boss" },
            new MonsterRank(5) { DisplayName = "Agent" }
        };
        public static MonsterRank Default { get { return Ranks[0]; } }

        public static MonsterRank Minion { get { return Ranks[0]; } }
        public static MonsterRank Captain { get { return Ranks[1]; } }
        public static MonsterRank Commander { get { return Ranks[2]; } }
        public static MonsterRank MiniBoss { get { return Ranks[3]; } }
        public static MonsterRank Boss { get { return Ranks[4]; } }
        public static MonsterRank Agent { get { return Ranks[5]; } }

        public string DisplayName;

        [DataMember]
        private int index;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            DisplayName = Ranks[index].DisplayName;
        }

        private MonsterRank(int index)
        {
            this.index = index;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static implicit operator MonsterRank(int rank)
        {
            if (rank < 0 || rank > Ranks.Length)
                rank = 0;
            return Ranks[rank];
        }

        public static implicit operator int(MonsterRank rank)
        {
            return rank.index;
        }

        public static implicit operator MonsterRank(string rankString)
        {
            foreach (var rank in Ranks)
            {
                if (rank.DisplayName == rankString)
                    return rank;
            }

            return Default;
        }

        public static bool operator ==(MonsterRank a, MonsterRank b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a as object == null || b as object == null)
                return false;
            return a.Equals(b);
        }

        public static bool operator <(MonsterRank a, MonsterRank b)
        {
            return a.index < b.index;
        }

        public static bool operator >(MonsterRank a, MonsterRank b)
        {
            return a.index > b.index;
        }

        public static bool operator <=(MonsterRank a, MonsterRank b)
        {
            return a.index <= b.index;
        }

        public static bool operator >=(MonsterRank a, MonsterRank b)
        {
            return a.index >= b.index;
        }

        public static bool operator !=(MonsterRank a, MonsterRank b)
        {
            if (ReferenceEquals(a, b))
                return false;
            if (a as object == null || b as object == null)
                return true;
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MonsterRank)) return false;

            return index == ((MonsterRank)obj).index;
        }

        public override int GetHashCode()
        {
            return index.GetHashCode();
        }
    }
}
