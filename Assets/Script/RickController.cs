﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class RickController : MonoBehaviour
{
	[DisplayScriptableObjectProperties]
	public PlayerStats Stats;
	
	public bool _groundCheck;
	private Animator _anim;
	
	private Rigidbody2D _rb;

	// private bool _isJumping, _isDead;
	public float _jumpStartTime;
	private Collider2D LastCheckedRock;
	public bool _rockWasJumped;
	private int _rockLayer, _groundRickLayer;
	private Vector2 _startPos;

	private AudioSource _audioSource;

	public bool GroundCheck
	{
		get => _groundCheck;
		set => _groundCheck = value;
	}

	public float JumpStartTime
	{
		get => _jumpStartTime;
		set => _jumpStartTime = value;
	}

	public bool RockWasJumped
	{
		get => _rockWasJumped;
		set => _rockWasJumped = value;
	}

	public enum RickStates { walking, jumping, dead }

	// Use this for initialization
	public void Init ()
	{
		_anim = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody2D>();
		_audioSource = GetComponent<AudioSource>();
		_rockLayer = LayerMask.NameToLayer("Rock");	// int 
		_groundRickLayer = LayerMask.NameToLayer("GroundRick");
		_startPos = transform.position;
	}
	
	// Update is called once per frame
	// void Update ()
	// {
	// 	if (_isDead)
	// 		return;
	// 	if (Input.GetButton("Jump") || Input.touches.Length > 0)
	// 		if (!_isJumping)
	// 		{
	// 			_jumpStartTime = Time.time;
	// 			_isJumping = true;
	// 			_groundCheck = true;
	// 			DoJump();				
	// 		} 
	// 		else
	// 		{
	// 			var jumpEndTime = _jumpStartTime + Stats.JumpAddTime;
	// 			if (Time.time <= jumpEndTime)
	// 				DoJump();
	// 		}
	// 	if (_isJumping)
	// 		CheckRockUnderneath();
	// }

	public void CheckRockUnderneath()
	{
		var hit = Physics2D.Raycast(transform.position, Vector2.down, Stats.RockCheckLength,
			1 << _rockLayer); //1 << 11
		Debug.DrawRay(transform.position, Vector2.down * Stats.RockCheckLength, Color.green);

		if (hit.collider == null)
			return;
		//Debug.LogError("Collider hit: "+hit.collider.gameObject.name);
		var colliderHit = hit.collider;
		if ( !colliderHit || colliderHit == LastCheckedRock)
			return;

		_rockWasJumped = true;
		LastCheckedRock = colliderHit;
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == _groundRickLayer)
		{
			// _isJumping = false;
			if (_groundCheck)
				_groundCheck = false;
			if (_rockWasJumped)
			{
				Game.Data.AddScore();
				_rockWasJumped = false;
			}
			return;
		}
		Death();
	}

	public bool IsGroundLayer(LayerMask otherLayer) => otherLayer == _groundRickLayer;

	public bool IsOnGroundLayer(Collision2D collision)
	{
		return IsGroundLayer(collision.gameObject.layer);
	}

	public void DoJump()
	{
		_rb.AddForce(Vector2.up * Stats.JumpForce);
		PlaySoundFX(Stats.SoundFX.Jump);		
	}

	public void Death()
	{
		_anim.SetTrigger("isdead");
		// _isDead = true;
		PlaySoundFX(Stats.SoundFX.Death);
		Game.Data.Death();
		// if (Game.Data.Lives > 0)
		// 	StartCoroutine(Respawn());
	}

	public void PlaySoundFX(AudioClip clip)
	{
		_audioSource.clip = clip;
		_audioSource.Play();
	}


	public IEnumerator Respawn()
	{
		// IsRespawning = true;
		yield return new WaitForSeconds(Game.Data.RespawnTime);
		transform.position = _startPos;
		transform.rotation = Quaternion.identity;
		_rb.velocity = Vector2.zero;
		_rb.angularVelocity = 0f;
		_anim.SetTrigger("revive");
		// _isDead = false;		
	}	
}
