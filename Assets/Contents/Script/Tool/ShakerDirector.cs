using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR.InteractionSystem;

[System.Serializable]
public class ShakerDirector : BartenderDirector
{
    private ParticleSystem _shakeEffect;
    bool _soundDelay;
    float time = 0.3f;
    public ShakerDirector(GameObject obj, ParticleSystem shakeEffect) : base(obj)
    {
        _shakeEffect = shakeEffect;
        _receiveData += Effect;
    }
    private void Effect(Vector3 data)
    {
        _shakeEffect.gameObject.transform.parent.LookAt(_shakeEffect.gameObject.transform.parent.position - data);
        _shakeEffect?.Play();
        var hand = _go.GetComponent<Interactable>()?.hoveringHand;
        if(!_soundDelay)
        {
            SoundManager.Instance?.PlaySound("Sound_ºŒ¿Ã≈∑1");
            CoroutineRunner.Instance.StartCoroutine(DelaySound());
        }
        //hand?.TriggerHapticPulse(100);
    }
    IEnumerator DelaySound()
    {
        _soundDelay = true;
        yield return new WaitForSeconds(0.5f);
        _soundDelay = false;
    }
}
