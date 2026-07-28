using System;

namespace Data
{
    [Serializable]
    public abstract class PublicScheme : IScheme
    {
        public abstract string ID { get; }
    }
}
