using System;

namespace Data
{
    public class TutorialPrivateModel : PrivateModel.Collection<TutorialPrivateScheme>
    {
        protected override TutorialPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out TutorialType type) 
                ? new TutorialPrivateScheme(type) 
                : null;
        }
    }
}
