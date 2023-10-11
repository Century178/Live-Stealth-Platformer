using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        //Scuffed level loading kk
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
