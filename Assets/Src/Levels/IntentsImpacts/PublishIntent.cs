namespace Levels.IntentsImpacts
{
    public delegate bool PublishIntent<in TIntent>(TIntent intent) where TIntent : IIntent;
}