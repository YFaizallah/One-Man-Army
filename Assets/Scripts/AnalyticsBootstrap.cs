using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Threading.Tasks;

public class AnalyticsBootstrap : MonoBehaviour
{
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("UGS Analytics initialized");
        }
        catch (System.Exception e)
        {
            Debug.LogError("UGS Analytics init failed: " + e);
        }
    }
}
