using UnityEngine;
using System.Collections;

public class SugarParticle : MonoBehaviour {

    [SerializeField] private Rigidbody _body;
    [SerializeField] private OxygenParticle[] _oxyParticles = new OxygenParticle[6]; 

    private Vector3 _oriPos;

    public float speed = 5.0f;
	// Use this for initialization
	void Awake () {
        _oriPos = this.transform.position;

    }

    public void MoveToIcon()
    {
        this.gameObject.SetActive(true);
        Vector3 vel = Vector3.up * speed;
        _body.AddForce(vel, ForceMode.Impulse);


        for (int i = 0; i < _oxyParticles.Length; ++i)
        {
            float randX = Random.Range(-100.0f, 100.0f);
            float randY = Random.Range(-100.0f, 100.0f);
            Vector2 randDir = new Vector2(randX, randY);
            randDir.Normalize();
            _oxyParticles[i].StartMove(randDir);
        }
    }

    public void SendBack()
    {
        this.transform.position = _oriPos;
        _body.velocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }

}
