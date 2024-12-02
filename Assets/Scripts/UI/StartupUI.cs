using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupUI : MonoBehaviour
{
    [SerializeField] private float waitTime = 1.0f;

    private void Start()
    {
        StartCoroutine(Startup());
    }

    private IEnumerator Startup()
    {
        LoadingScreenUI.Instance.Show();
        yield return new WaitForSeconds(waitTime);

        SceneLoadingManager.Instance.LoadLocalDefaultScene();
    }
}
