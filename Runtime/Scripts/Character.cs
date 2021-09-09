using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AgatePris.ShmupsToolkit {
    [AddComponentMenu("Scripts/AgatePris.ShmupsToolkit/Shmups Toolkit Character")]
    public class Character : MonoBehaviour {
        [SerializeField] bool activateAllChildrenOfTransformOnDisable = true;
        [SerializeField] UnityEvent<Character> onEnable;
        [SerializeField] UnityEvent<Character> onAfterEnabledIfIsVisible;
        [SerializeField] UnityEvent<Character> onAfterEnabledIfIsInvisible;
        [SerializeField] UnityEvent<Character> onDisable;
        [SerializeField] UnityEvent<Character> onBecameVisible;
        [SerializeField] UnityEvent<Character> onBecameInvisible;
        bool afterEnabled;
        List<Renderer> renderers = new List<Renderer>();
        bool isVisibleCache;

        public bool IsVisible {
            get {
                foreach (var renderer in renderers) {
                    if (renderer.isVisible) {
                        return true;
                    }
                }
                return false;
            }
        }

        public static new void Destroy(Object obj) => Object.Destroy(obj);
        public void DestroyGameObject() => Destroy(gameObject);
        public static void SetAllChildrenActive(Transform transform, bool value) {
            for (var i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).gameObject.SetActive(value);
            }
        }
        public static void ActivateAllChildren(Transform transform) => SetAllChildrenActive(transform, true);
        public static void DeactivateAllChildren(Transform transform) => SetAllChildrenActive(transform, false);
        public void SetAllChildrenOfTransformsActive(bool value) => SetAllChildrenActive(transform, value);
        public void ActivateAllChildrenOfTransform() => SetAllChildrenActive(transform, true);
        public void DeactivateAllChildrenOfTransform() => SetAllChildrenActive(transform, false);

        void Awake() {
            foreach (var renderer in GetComponents<Renderer>()) {
                renderers.Add(renderer);
            }
            foreach (var renderer in GetComponentsInChildren<Renderer>()) {
                renderers.Add(renderer);
            }
        }
        void OnEnable() {
            afterEnabled = true;
            onEnable.Invoke(this);
        }
        void OnDisable() {
            if (activateAllChildrenOfTransformOnDisable) {
                ActivateAllChildrenOfTransform();
            }
            onDisable.Invoke(this);
        }
        void Update() {
            if (afterEnabled) {
                afterEnabled = false;
                isVisibleCache = IsVisible;
                if (isVisibleCache) {
                    onAfterEnabledIfIsVisible.Invoke(this);
                } else {
                    onAfterEnabledIfIsInvisible.Invoke(this);
                }
            }
            if (isVisibleCache != IsVisible) {
                isVisibleCache = !isVisibleCache;
                if (isVisibleCache) {
                    onBecameVisible.Invoke(this);
                } else {
                    onBecameInvisible.Invoke(this);
                }
            }
        }
    }
}
