using FMODUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ShopAudioHandler : MonoBehaviour
{
    [SerializeField]
    private EventReference ShopAction;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShopActionSound()
    {
        AudioManager.PlayOneShot(ShopAction);
    }
}

