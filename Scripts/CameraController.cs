using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class CameraController : MonoBehaviour
{
    public CinemachineBrain brain;
    public CinemachineBlendDefinition blendStyle = new CinemachineBlendDefinition (CinemachineBlendDefinition.Style.EaseInOut, 1.0f);
    private CinemachineVirtualCamera vcam;
    private float resetTime = 0.0f;
    private float resetTimeCounter = 0.0f;
    [SerializeField] private float enemyHitSlowMoIntensity = 0.75f;
    [SerializeField] private float enemyHitSlowMoDuration = 1f;
    [SerializeField] private float endGameSlowMoIntensity = 0.5f;
    [SerializeField] private float endGameSlowMoDuration = 5.0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;

        //Assign active camera as vcam. This should be the default player camera, and should have highest priority in the scene.
        vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        // Change Default Blend
        brain.m_DefaultBlend = blendStyle;

        AreaDetector.onEnterArea += SwitchToAreaCamera;
        AreaDetector.onExitArea += SwitchToPlayerCamera;
        Enemy.onHitPlayer += EnemyHitSlowMotion;
        PlayerCharacter.destroyPlayer += EndGameSlowMotion;
        BossController.bossDefeated += EndGameSlowMotion;
    }

    void SwitchToAreaCamera(GameObject cameraConfiner)
    {
        int priority = vcam.Priority;
        vcam.Priority -= 5;
        CinemachineVirtualCamera fixedCamera = cameraConfiner.GetComponentInChildren<CinemachineVirtualCamera>();
        fixedCamera.Priority = priority;

        cameraConfiner.GetComponent<AudioSource>().Play();

    }

    void SwitchToPlayerCamera(GameObject cameraConfiner)
    {
        CinemachineVirtualCamera fixedCamera = cameraConfiner.GetComponentInChildren<CinemachineVirtualCamera>();
        int priority = fixedCamera.Priority;
        vcam.Priority = priority;
        fixedCamera.Priority -= 5;

        cameraConfiner.GetComponent<AudioSource>().Stop();
    }

    void EnemyHitSlowMotion(float damage)
    {
        ChangeGameSpeed(enemyHitSlowMoIntensity, enemyHitSlowMoDuration);
    }

    void EndGameSlowMotion()
    {
        ChangeGameSpeed(endGameSlowMoIntensity, endGameSlowMoDuration);
    }

    // Change the timeScale to tscale.
    void ChangeGameSpeed(float tscale, float duration)
    {
        Time.timeScale = tscale;
        Invoke("RestoreGameSpeed", duration);
    }

    //Restore previous speed
    void RestoreGameSpeed()
    {
        Time.timeScale = 1.0f;
    }
}
