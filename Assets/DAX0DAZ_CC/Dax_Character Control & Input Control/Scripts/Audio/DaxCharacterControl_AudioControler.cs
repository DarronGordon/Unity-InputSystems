using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaxCharacterControl_AudioControler : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private CharacterControlData_SO characterData;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayStepSound()
    {
        audioSource.PlayOneShot(characterData.FootStepSounds[Random.Range(0, characterData.FootStepSounds.Count)]);
    }
    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(characterData.JumpingSounds[Random.Range(0, characterData.JumpingSounds.Count)]);
    }
    public void PlayLandSound()
    {
        audioSource.PlayOneShot(characterData.LandOnGroundSounds[Random.Range(0, characterData.LandOnGroundSounds.Count)]);
    }

}
