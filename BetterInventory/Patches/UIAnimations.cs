using System.Collections;
using UnityEngine;

namespace BetterInventory.Patches
{
    class ItemPulsing : MonoBehaviour
    {
        public Vector3 minScale = new Vector3(0.75f, 0.75f, 0.75f);
        public Vector3 maxScale = new Vector3(1.25f, 1.25f, 1.25f);
        public float scalingSpeed = 0.7f;
        public float scalingDuration = 0.5f;

        IEnumerator Start()
        {
            yield return RepeatLerping(transform.localScale, minScale, scalingDuration);
            while (true)
            {
                yield return RepeatLerping(minScale, maxScale, scalingDuration);
                yield return RepeatLerping(maxScale, minScale, scalingDuration);
            }
        }

        IEnumerator RepeatLerping(Vector3 startScale, Vector3 endScale, float time, bool destroyWhenDone = false)
        {
            float currentTime = 0.0f;
            float rate = (1f / time) * scalingSpeed;
            while (currentTime < 1f)
            {
                currentTime += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(startScale, endScale, currentTime);
                yield return null;
            }

            if(destroyWhenDone)
            {
                Destroy(this);
            }
        }

        public void StopAnimationAndDestroy()
        {
            transform.localScale = Vector3.one;

            StopAllCoroutines();
            Destroy(this);
        }
    }
}
