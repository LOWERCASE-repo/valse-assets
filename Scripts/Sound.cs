using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

  private AudioSource speaker;

  private int sampleRate = 44100;
  private float rateMod;

  [SerializeField]
  private float duration;

  [SerializeField]
  [Range(0f, 1f)]
  private float wave,
    attack, decay, sustain, release, compression;
  
  [SerializeField]
  private AnimationCurve volume;
  
  [SerializeField]
  private float[] pitches;
  [SerializeField]
  private AnimationCurve pitch;
  
  private float EvalSine(float time) {
    return Mathf.Sin(time * Mathf.PI * 2f);
  }

  private float EvalTriangle(float time) {
    time %= 1f;
    if (time < 0.25f) {
      return time * 4f;
    } else if (time < 0.75f) {
      return (time - 0.50f) * -4f;
    } else {
      return (time - 0.75f) * 4f - 1f;
    }
  }

  private float EvalSquare(float time) {
    time %= 1f;
    return Mathf.Round(time) * 2f - 1f;
  }

  private float EvalSaw(float time) {
    time %= 1f;
    if (time < 0.5f) {
      return time;
    } else {
      return time - 1f;
    }
  }
  
  private float[] random;
  private float EvalNoise(float time) {
    time %= 1f;
    return random[(int)(time * random.Length)];
  }

  private float EvalTimbre(float time, float wave) {
    time %= 1f;
    if (wave < 0.2f) {
      return EvalSine(time);
    } else if (wave < 0.4f) {
      return EvalTriangle(time);
    } else if (wave < 0.6f) {
      return EvalSquare(time);
    } else if (wave < 0.8f) {
      return EvalSaw(time);
    } else {
      return EvalNoise(time);
    }
  }

  private void ApplyEnvelope() {
    Keyframe[] keys = new Keyframe[5];
    for (int i = 0; i < 5; i++) {
      keys[i] = new Keyframe(i / 4f, 0f);
      keys[i].weightedMode = WeightedMode.Both;
      keys[i].inWeight = 0f;
      keys[i].outWeight = 0f;
    }
    keys[1].time = attack * 0.5f;
    keys[1].value = 1f;
    keys[2].time = decay * 0.5f + 0.5f;
    keys[2].value = sustain;
    keys[3].time = release * 0.5f + 0.5f;
    keys[3].value = keys[2].value;
    volume.keys = keys;
  }
  
  private void ApplyPitches() {
    Keyframe[] keys = new Keyframe[pitches.Length];
    for (int i = 0; i < pitches.Length; i++) {
      keys[i] = new Keyframe((float)i / (float)(pitches.Length - 1), pitches[i]);
      keys[i].weightedMode = WeightedMode.None;
    }
    pitch.keys = keys;
  }

  private float EvalFrequency(float time) {
    time %= 1f;
    time = Mathf.Pow(time, 6f);
    return time * 18980 + 20;
  }
  
  // 20000 / x * 5

  private int pos = 0;
  private void OnAudioRead(float[] data) {
    for (int i = 0; i < data.Length; i++, ++pos) {
      int randPos = pos % sampleRate;
      float totalTime = pos / (duration * sampleRate);
      float cycleTime = (EvalFrequency(pitch.Evaluate(totalTime)) * pos) * rateMod;
      data[i] = EvalTimbre(cycleTime, wave) * volume.Evaluate(totalTime);
    }
  }

  private void Awake() {
    speaker = GetComponent<AudioSource>();
    volume = new AnimationCurve();
    pitch = new AnimationCurve();
    rateMod = 1f / sampleRate;
    random = new float[sampleRate];
    for (int i = 0; i < random.Length; i++) {
      random[i] = Random.value * 2f - 1f;
    }
  }

  private AudioClip clip;
  private void FixedUpdate() {
    if (Input.GetKeyDown("space")) {
      duration = Random.value * 10f;
      wave = Random.value;
      attack = Random.value;
      decay = Random.value;
      sustain = Random.value;
      release = Random.value;
      pitches = new float[1 + (int)(Random.value * Random.value * 10f)];
      for (int i = 0; i < pitches.Length; i++) {
        pitches[i] = Random.value;
      }
      ApplyEnvelope();
      ApplyPitches();
      clip = AudioClip.Create("Clip", (int)(sampleRate * duration), 1, sampleRate, true, OnAudioRead, position => pos = position);
      speaker.PlayOneShot(clip);
    }
  }
}
