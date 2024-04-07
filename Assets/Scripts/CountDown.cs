using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CountDown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] float remainTime;
    int min = 0;
	int sec = 0;

    // Update is called once per frame
    void Update()
	{
		if (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
			min = Mathf.FloorToInt(remainTime / 60);
			sec = Mathf.FloorToInt(remainTime % 60);
		}
        else if (remainTime <= 0)
		{
			// Debug.Log("Exiting game...");
			Application.Quit();
		}
		timeText.text = string.Format("{0:00}:{1:00}", min, sec);
	}
}
