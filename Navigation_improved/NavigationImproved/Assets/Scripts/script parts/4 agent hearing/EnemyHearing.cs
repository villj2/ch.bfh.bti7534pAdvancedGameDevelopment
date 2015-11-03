using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class EnemyHearing : MonoBehaviour
{
    public delegate void SoundHeardDelegate(Vector3 position, float falloff);
    public SoundHeardDelegate soundHeard;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Soundwave")
            return;

        Soundwave soundwave = other.GetComponent<Soundwave>();
        if(soundwave != null && soundHeard != null)
            soundHeard(other.transform.position, soundwave.normalizedVolume);
    }
}
