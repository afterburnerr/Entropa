namespace Entropa.Core
{
    public interface IIntentSource
    {
        CharacterIntent GetIntent(float deltaTime);
    }
}
