using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScrolling : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private Vector2 scrollDirection = Vector2.left;

    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    private void Start()
    {
        m_image.material.mainTextureOffset = Vector2.zero;
    }

    private void Update()
    {
        m_image.material.mainTextureOffset = scrollDirection.normalized * scrollSpeed * Time.time;
    }
}
