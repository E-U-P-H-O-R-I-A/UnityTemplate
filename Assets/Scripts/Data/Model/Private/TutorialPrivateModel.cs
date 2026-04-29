using System;
using Data.Scheme.Private;
using Data.Scheme.Public;

namespace Data.Model.Private
{
    public class TutorialPrivateModel : BaseCollectionPrivateModel<TutorialPrivateScheme>
    {
        protected override TutorialPrivateScheme CreateSchemeById(string id)
        {
            return Enum.TryParse(id, out TutorialType type) 
                ? new TutorialPrivateScheme(type) 
                : null;
        }
    }
}