using System;

namespace Data
{
    [Serializable]
    public abstract class PublicRecord : IRecord
    {
        public abstract string Id { get; }
    }
}
