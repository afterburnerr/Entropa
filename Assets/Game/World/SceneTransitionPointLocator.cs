using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entropa.World
{
    public sealed class SceneTransitionPointLocator : ITransitionPointLocator
    {
        public TransitionPoint Find(string sceneName, string pointId)
        {
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(pointId))
            {
                return null;
            }

            return Find(SceneManager.GetSceneByName(sceneName), pointId);
        }

        public TransitionPoint Find(Scene scene, string pointId)
        {
            if (string.IsNullOrEmpty(pointId) || !scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                TransitionPoint[] points = roots[i].GetComponentsInChildren<TransitionPoint>(true);
                for (int j = 0; j < points.Length; j++)
                {
                    if (points[j].PointId == pointId)
                    {
                        return points[j];
                    }
                }
            }

            return null;
        }
    }
}
