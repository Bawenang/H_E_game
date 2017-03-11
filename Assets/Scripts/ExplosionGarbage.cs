using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionGarbage : MonoBehaviour {

    public explosion_fx fxPrefab;
    public bool isAlive = false;
    private Stack<explosion_fx> _explosionStack = new Stack<explosion_fx>();

    public int gemType;

    void Start()
    {
        //Precreate
        if (fxPrefab != null && isAlive)
        {
            for (int i = 0; i < 4; ++i)
            {
                explosion_fx fx = InstantiateFX(0.0f, 0.0f);
                fx.gameObject.SetActive(false);
                _explosionStack.Push(fx);
            }
        }
    }

    private explosion_fx InstantiateFX(float x, float y)
    {
        GameObject fxGo = (GameObject)Instantiate(   fxPrefab.gameObject,
                                                        new Vector3(x, -y, -1f),
                                                        Quaternion.identity);
        explosion_fx fxTemp = fxGo.GetComponent<explosion_fx>();
        fxTemp.my_garbage = this;
        return fxTemp;
        
    } 

    public void ShowExplosion(float x, float y)
    {
        if (_explosionStack.Count > 0) //if you can recycle a previous fx
        {
            _explosionStack.Pop().Show_me(new Vector3(x, -y, -1f));
        }
        else //create a new fx
        {
            explosion_fx fx_temp = InstantiateFX(x, y);
            fx_temp.Activate_me(this);
        }
    }

    public void GoBack(explosion_fx fx)
    {
        _explosionStack.Push(fx);
    }
}
