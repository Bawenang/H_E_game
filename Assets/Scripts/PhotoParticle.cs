using UnityEngine;
using System.Collections;

public class PhotoParticle : MonoBehaviour {

    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private SugarParticle _sugarImage;

    public Chlorophyll parentChloro;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play()
    {
        _particle.Play();
        StartCoroutine(PlaySugar());
    }

    public IEnumerator PlaySugar()
    {
        yield return new WaitForSeconds(0.5f);
        _sugarImage.MoveToIcon();
    }
}
