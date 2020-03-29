using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float slowdownFactor = 0.05f; //controls how slow something moves
    public float slowdownLength;
    public GameObject player;

    private void Update()
    {
        /*
        Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime; 
        //add half to time scale so 2 seconds
                                                                          //unscaledDeltaTime not affected when changing timeScale
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);*/
    }
    public void DoSlowMotion()
    {
        Time.timeScale = slowdownFactor; // 1/0.05 = 20 (move 20 times slower)
        Time.fixedDeltaTime = Time.timeScale * 0.02f; //0.02f is the default for fixedUpdate adjustment
    }

    public IEnumerator SlowMoCo()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        yield return new WaitForSecondsRealtime(slowdownLength);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void SlowMo()
    {
        StartCoroutine("SlowMoCo");
    }
}
