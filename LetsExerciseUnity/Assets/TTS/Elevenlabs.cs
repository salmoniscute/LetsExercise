using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;


[Serializable]
public class VoiceSettings
{
    public float stability;
    public float similarity_boost;
    public float style;
}

[Serializable]
public class TTSData
{
    public string text;
    public string model_id;
    public VoiceSettings voice_settings;
}


public class Elevenlabs : MonoBehaviour
{
  
   public ElevenLabsConfig config;
   public AudioSource audioSource;
   public string text;
 

    public void Start()
    {
        StartCoroutine(GenerateAndStreamAudio(text));
    }
    
    public IEnumerator GenerateAndStreamAudio(string text)
{
    string modelId = "eleven_multilingual_v2";
    string url = string.Format(config.ttsUrl, config.voiceId);


    TTSData ttsData = new TTSData
    {
        text = text.Trim(),
        model_id = modelId,
        voice_settings = new VoiceSettings
        {
            stability = 0.5f,
            similarity_boost = 0.75f,
            style = 0.35f
        }
    };

    string jsonData = JsonUtility.ToJson(ttsData);
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

    using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
    {
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerAudioClip(new Uri(url), AudioType.MPEG);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", config.apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            yield break;
        }

        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);

    if (audioClip != null)
        {
            audioSource.clip = audioClip; 
            PlayAudio(audioClip);
        // Wait for the audio clip to finish playing
        yield return new WaitForSeconds(audioClip.length * 0.1f);
        }
    else
    {
        // the audio is null so download the audio again
        yield return StartCoroutine(GenerateAndStreamAudio(text));
    }


        // Wait for the audio clip to finish playing
        yield return new WaitForSeconds(audioClip.length);
    
}

}

    private void PlayAudio(AudioClip audioClip)
    {
       audioSource.PlayOneShot(audioClip);
    }


}
