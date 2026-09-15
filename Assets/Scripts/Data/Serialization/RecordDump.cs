using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class RecordDump
    {
        public List<RecordEntry> items = new();
    }
}