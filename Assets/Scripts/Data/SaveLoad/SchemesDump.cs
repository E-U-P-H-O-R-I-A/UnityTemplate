using System;
using System.Collections.Generic;
using Data.Model;

namespace Data.SaveLoad
{
    [Serializable]
    public class SchemesDump
    {
        public List<SchemeRecord> items = new();
    }
}