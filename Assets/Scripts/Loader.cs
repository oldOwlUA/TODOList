using UnityEngine;
using UnityEngine.SceneManagement;


public class Loader : MonoBehaviour
{

    private void Start()
    {
        var load = SceneManager.LoadSceneAsync(1);
    }

}
