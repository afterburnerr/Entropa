using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entropa.World
{
    public sealed class SceneLoader : ISceneLoader
    {
        private sealed class Entry
        {
            public LocationDefinition Location;
            public AsyncOperation Operation;
            public bool Activated;
        }

        private readonly Dictionary<LocationDefinition, Entry> _entries =
            new Dictionary<LocationDefinition, Entry>();

        public event Action<LocationDefinition> LocationActivated;

        public event Action<LocationDefinition> LocationUnloaded;

        public async UniTask PreloadAsync(LocationDefinition location, CancellationToken cancellationToken = default)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (!_entries.TryGetValue(location, out Entry entry))
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(location.SceneName, LoadSceneMode.Additive);
                if (operation == null)
                {
                    throw new InvalidOperationException($"Scene '{location.SceneName}' is missing from build settings.");
                }

                operation.allowSceneActivation = false;
                entry = new Entry { Location = location, Operation = operation };
                _entries.Add(location, entry);
            }

            if (entry.Activated || entry.Operation == null)
            {
                return;
            }

            await UniTask.WaitUntil(() => entry.Operation == null || entry.Operation.progress >= 0.9f, cancellationToken: cancellationToken);
        }

        public async UniTask ActivateAsync(LocationDefinition location, CancellationToken cancellationToken = default)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            await PreloadAsync(location, cancellationToken);

            Entry entry = _entries[location];
            if (entry.Activated || entry.Operation == null)
            {
                return;
            }

            AsyncOperation operation = entry.Operation;
            operation.allowSceneActivation = true;
            await operation.ToUniTask(cancellationToken: cancellationToken);

            entry.Operation = null;
            entry.Activated = true;
            LocationActivated?.Invoke(location);
        }

        public async UniTask UnloadAsync(LocationDefinition location, CancellationToken cancellationToken = default)
        {
            if (location == null || !_entries.TryGetValue(location, out Entry entry))
            {
                return;
            }

            if (entry.Activated)
            {
                Scene scene = SceneManager.GetSceneByName(location.SceneName);
                if (scene.IsValid() && scene.isLoaded)
                {
                    AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                    if (operation != null)
                    {
                        await operation.ToUniTask(cancellationToken: cancellationToken);
                    }
                }
            }
            else if (entry.Operation != null && entry.Operation.progress < 0.9f)
            {
                entry.Operation.allowSceneActivation = true;
                await entry.Operation.ToUniTask(cancellationToken: cancellationToken);
                entry.Operation = null;
                Scene scene = SceneManager.GetSceneByName(location.SceneName);
                if (scene.IsValid() && scene.isLoaded)
                {
                    AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                    if (operation != null)
                    {
                        await operation.ToUniTask(cancellationToken: cancellationToken);
                    }
                }
            }

            _entries.Remove(location);
            LocationUnloaded?.Invoke(location);
        }

        public bool IsActivated(LocationDefinition location)
        {
            return location != null
                && _entries.TryGetValue(location, out Entry entry)
                && entry.Activated;
        }

        public bool TryGetLoaded(LocationDefinition location, out Scene scene)
        {
            scene = default;
            if (location == null || !_entries.TryGetValue(location, out Entry entry) || !entry.Activated)
            {
                return false;
            }

            scene = SceneManager.GetSceneByName(location.SceneName);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
