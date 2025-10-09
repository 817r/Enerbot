using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech;
using UnityEngine;
using UnityEngine.Video;
using FrostweepGames.Plugins.GoogleCloud.TextToSpeech.Example;

public class VideoHandler : MonoBehaviour
{
    public VideoPlayer vidPlayer;
    
    // Animation Start from xB then transition to xF
    public VideoClip[] vidClips_Male;
    public VideoClip[] vidClips_Female;
    // Clip Names by Idx
    // Idle 6F
    // Idle 6B
    // Talk 4F
    // Talk 4B
    // Talk 15F
    // Talk 15B

    [Header("References")]
    public Custom_GC_TextToSpeech_SimpleExample gcTTS_Example;
    public GameManager_AIMA gameManager;

    private void Start() {
        if(gameManager == null){
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager_AIMA>();
        }

        if(gameManager.genderAI == AIGender.Male){
            vidPlayer.clip = vidClips_Male[0];
            vidPlayer.isLooping = true;
            vidPlayer.Play();

        }else if(gameManager.genderAI == AIGender.Female){
            vidPlayer.clip = vidClips_Female[0];
            vidPlayer.isLooping = true;
            vidPlayer.Play();
        }

        StartTalking();
            
    }

    private void Update() {
        if(gameManager.genderAI == AIGender.Male){
            if(vidPlayer.clip != vidClips_Male[0]){
                // Change AI vid to go Idle after finish talking
                if(!vidPlayer.isPlaying){
                    GoIdle();
                }
                
            }

        }else if(gameManager.genderAI == AIGender.Female){
            if(vidPlayer.clip != vidClips_Female[0]){
                // Change AI vid to go Idle after finish talking
                if(!vidPlayer.isPlaying){
                    GoIdle();
                }
            
            }
        }

        

    }

    public void StartTalking(){
        vidPlayer.isLooping = true;

        if(vidPlayer.isPlaying){
            vidPlayer.Stop();
        }

        if(gameManager.genderAI == AIGender.Male){
            vidPlayer.clip = vidClips_Male[GetTalkVariant()];

        }else if(gameManager.genderAI == AIGender.Female){
            vidPlayer.clip = vidClips_Female[GetTalkVariant()];

        }

        vidPlayer.Play();

    }

    public void GoIdle(){
        if(gameManager.genderAI == AIGender.Male){
            vidPlayer.clip = vidClips_Male[0];

        }else if(gameManager.genderAI == AIGender.Female){
            vidPlayer.clip = vidClips_Female[0];

        }
        
        vidPlayer.isLooping = true;
        vidPlayer.Play();
        Debug.Log("Changing AI to Idle State, Vidplayer finish talking");
    }

    int GetTalkVariant(){
        int randomNum = Random.Range(1,4); // Random int from 1 - 3
        Debug.Log("Changed to Talk Variant " + randomNum);
        return randomNum;
    }

    public IEnumerator WaitGoIdle(float waitDuration){
        yield return new WaitForSeconds(waitDuration);
        Debug.Log("Changing AI to Idle State, Audio Source Finished");
        GoIdle();
    }
    IEnumerator WaitVidStopToPlay(){

        while(vidPlayer.isPlaying){
            yield return null;
        }

        vidPlayer.Play();
    }

}
