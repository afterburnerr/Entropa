namespace Entropa.World
{
    public interface ITransitionPointLocator
    {
        TransitionPoint Find(string sceneName, string pointId);
    }
}
