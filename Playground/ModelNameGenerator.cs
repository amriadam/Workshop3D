namespace Playground;

internal sealed class ModelNameGenerator
{
    private int mSequence;

    public int GenerateName()
    {
        return ++mSequence;
    }
}
