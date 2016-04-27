using UnityEngine;
using System.Collections;



public class VideoControl : MonoBehaviour {

	public float startOffset = 0;

	private MovieTexture movie;

	// Use this for initialization
	IEnumerator Start () {


		movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
		yield return new WaitForSeconds (startOffset);
		movie.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			if(movie.isPlaying)
				movie.Pause();
			else
				movie.Play();
		}
	}
}
