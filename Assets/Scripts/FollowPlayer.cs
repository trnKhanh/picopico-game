using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;

    [SerializeField] private float followSpeed = 2.0f;
    [SerializeField] private float yOffset = 2.0f;

    private Vector2 curOffset = Vector2.zero;

    private void Update()
    {
        Vector3 newPosition = target.position;
        newPosition.x += curOffset.x;
        newPosition.y += yOffset + curOffset.y;
        newPosition.z = transform.position.z;

        transform.position = Vector3.Slerp(transform.position, newPosition, followSpeed * Time.deltaTime);
    }

    public void SetOffset(Vector2 offset)
    {
        curOffset = offset;
    }
}
