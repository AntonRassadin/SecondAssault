using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {

    private float startingHealth = 100;
    private float health;
    private RectTransform healthBar;
    private float fullWidth;

    public float StartingHealth { get { return startingHealth; } set { startingHealth = value; } }
    public float Health { get { return health; } set { health = value; } }

    private void Start()
    {
        healthBar = GetComponentInChildren<Image>().GetComponent<RectTransform>();
        fullWidth = healthBar.sizeDelta.x;
    }
    private void Update()
    {
        float width = fullWidth * (health / startingHealth);
        healthBar.sizeDelta = new Vector2(width, healthBar.sizeDelta.y);
        transform.LookAt(Camera.main.transform.position);
    }
}
