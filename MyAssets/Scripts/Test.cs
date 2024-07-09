using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoBehaviour
{
    public GameObject obj;
    Renderer meshRenderer;
    Material[] materials;

    void Start()
    {
        if (obj != null)
        {
            meshRenderer = obj.GetComponent<Renderer>();
            materials = meshRenderer.materials; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Show1(); 

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            Show2(); 
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {

            Show3(); 
        }
    }

    void Show1()
    {
        if (materials != null && materials.Length > 0)
        {
            materials[0].DOOffset(Vector3.zero, 1).SetEase(Ease.Linear);
            materials[1].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
            materials[2].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
        }
    }
    void Show2()
    {
        if (materials != null && materials.Length > 0)
        {
            materials[1].DOOffset(Vector3.zero, 1).SetEase(Ease.Linear);
            materials[0].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
            materials[2].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
        }
    }
    void Show3()
    {
        if (materials != null && materials.Length > 0)
        {
            materials[2].DOOffset(Vector3.zero, 1).SetEase(Ease.Linear);
            materials[1].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
            materials[0].DOOffset(Vector3.down, 1).SetEase(Ease.Linear);
        }
    }

}
