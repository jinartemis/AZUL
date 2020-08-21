using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BlinkText : MonoBehaviour
{
    private Text label = default;
    private Color color = default;
    private float delta = 1;
    [SerializeField, Header("Speed"), Range(.1f, 5.0f)]
    private float speed = 1.0f;

    private void Start()
    {
        label = this.GetComponent<Text>();
        color = label.color;
    }

    private void Update()
    {
        delta += Time.deltaTime * speed;
        color.a = (Mathf.Sin(delta) + 1) / 2;
        label.color = color;
    }

    public void SetColor(Color c)
    {
        color = c;
    }
}
