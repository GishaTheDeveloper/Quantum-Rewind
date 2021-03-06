﻿using System.Collections;
using UnityEngine;
public class EnergyCluster : MonoBehaviour
{
    public int energyValue;
    [Space]
    public int speed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Anomaly"))
        {
            switch (other.GetComponent<Anomaly>().anomalyType)
            {
                // If triggered by Replicate.
                case AnomalyType.Replicate:
                    DestroyCluster();

                    AudioManager.Instance.PlaySFX("Energy_Destroy");
                    break;
                // If triggered by Original.
                case AnomalyType.Original:
                    other.GetComponent<AnomalyOriginal>().ResetLifeTime();
                    SendCluster(EnergyManager.Instance.NowBattery);

                    PostProcessingController.Instance.TriggerChromaticAberration(0.65f, 7f, true);
                    AudioManager.Instance.PlaySFX("Energy_Collect");
                    break;
            }
            EffectsEmitter.Emit("Small_Green_Explosion", transform.position);
        }
    }

    void DestroyCluster()
    {
        Destroy(gameObject);
    }

    void SendCluster(Battery target)
    {
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(ClusterTransfering(target));
    }

    IEnumerator ClusterTransfering(Battery target)
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, target.transform.position) == 0)
            {
                target.ChargeUp();
                DestroyCluster();
                break;
            }
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, step);
            yield return null;
        }
    }
}
