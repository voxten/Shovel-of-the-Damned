using System.Collections;
using UnityEngine;

public class VentEnemy : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PlayEnemySound());
    }
    
    IEnumerator PlayEnemySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.PlaySound3D(Sound.Paper, transform);
            yield return new WaitForSeconds(1f);
        }
    }
}
