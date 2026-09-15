using System;

namespace Data
{
    [Serializable]
    public abstract class PrivateRecord : IRecord
    {
        public abstract string Id { get; }
    }
}