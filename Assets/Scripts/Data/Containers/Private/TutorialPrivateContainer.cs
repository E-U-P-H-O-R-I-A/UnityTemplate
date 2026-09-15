using System;
using TutorialId = Data.TutorialPublicContainer.Id;

namespace Data
{
    public class TutorialPrivateContainer : PrivateContainer.Collection<TutorialPrivateRecord>
    {
        protected override TutorialPrivateRecord CreateRecordById(string id)
        {
            return Enum.TryParse(id, out TutorialId type) 
                ? new TutorialPrivateRecord(type) 
                : null;
        }
    }
}
