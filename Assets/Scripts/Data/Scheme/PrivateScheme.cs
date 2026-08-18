using System;

namespace Data
{
    [Serializable]
    public abstract class PrivateScheme : IScheme
    {
        public abstract string ID { get; }
    }
}