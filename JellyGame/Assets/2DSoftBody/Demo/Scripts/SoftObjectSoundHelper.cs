using UnityEngine;

public class SoftObjectSoundHelper : MonoBehaviour
{
	public InputManager InputManager;
	public Rigidbody2D Rigidbody2D;

	private AudioSource audioSource;

	private const float MinVelocityForSound = 0.1f;
	private const float MaxVelocityForSound = 30f;

	void Start()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (!audioSource.isPlaying && Rigidbody2D.velocity.magnitude > MinVelocityForSound)
		{
			var soundIndex = Random.Range(0, InputManager.CollideSounds.Length);
			audioSource.clip = InputManager.CollideSounds[soundIndex];
			var volume = Mathf.Lerp(0f, 1f, Rigidbody2D.velocity.magnitude / MaxVelocityForSound);
			audioSource.volume = volume;
			audioSource.Play();
		}
	}
}