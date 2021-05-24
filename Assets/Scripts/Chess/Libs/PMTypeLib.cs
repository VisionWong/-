using System.Collections.Generic;

public class PMtypeData : IData
{
    public string name;
    public HashSet<PMType> effectiveSet = new HashSet<PMType>();
    public HashSet<PMType> halfEffectiveSet = new HashSet<PMType>();
    public HashSet<PMType> noEffectSet = new HashSet<PMType>();
}

public class PMTypeLib : DataLib<PMtypeData>
{
}
