using System;

namespace Data.Scheme
{
    [Serializable]
    public abstract class BasePublicScheme : IScheme
    {
        public abstract int ID { get; }
    }
}