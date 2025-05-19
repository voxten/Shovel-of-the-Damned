using System.Collections;
using UnityEngine;

public class VentEnemy : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PlayEnemySound());
        StartCoroutine(PlayVentSound());
    }

    private IEnumerator PlayEnemySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.PlaySound3D(Sound.EnemyCrawl, transform);
            yield return new WaitForSeconds(1f);
        }
    }
    
    private IEnumerator PlayVentSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.PlaySound3D(Sound.EnemyVent, transform, 50f);
            yield return new WaitForSeconds(137);
        }
    }
}
