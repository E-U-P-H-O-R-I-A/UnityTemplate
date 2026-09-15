namespace Signals
{
    public readonly struct LevelFinishedSignal
    {
        public readonly LevelResult result;

        public LevelFinishedSignal(LevelResult result)
        {
            this.result = result;
        }
    }
}
