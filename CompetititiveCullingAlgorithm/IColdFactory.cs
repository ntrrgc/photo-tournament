using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CompetititiveCullingAlgorithm
{
    interface IColdFactory<HotType, ColdType> where ColdType : ISerializable
    {
        ColdType MakeCold(HotType hot);
        HotType MakeHot(ColdType cold);
    }
}
