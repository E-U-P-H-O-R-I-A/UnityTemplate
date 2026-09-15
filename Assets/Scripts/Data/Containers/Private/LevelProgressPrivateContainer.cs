namespace Data
{
    public class LevelProgressPrivateContainer : PrivateContainer.Single<LevelProgressPrivateRecord>
    {
        protected override LevelProgressPrivateRecord CreateRecord() => new();
    }
}
