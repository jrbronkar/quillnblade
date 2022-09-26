using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public GameObject PLAYER_outputPanel;
    public GameObject NPC_outputPanel;
    public TMP_InputField inputText;
    TMP_Text player_output;
    TMP_Text npc_output;

    // Start is called before the first frame update
    void Start()
    {
        player_output = PLAYER_outputPanel.GetComponent<TextMeshProUGUI>();
        npc_output =  NPC_outputPanel.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            SendMessageToChat();
    }   
    public void SendMessageToChat()
    {
        player_output.text = inputText.text;
        inputText.text = "";
        //npc_output.text = "I am the bartender in this town. Now what can I do for you?";
        Debug.Log("Entering response territory");
        StartCoroutine(GetNPCResponse(player_output.text));
    }
    private IEnumerator GetNPCResponse(string input)
    {
        Debug.Log("Entered");
        var dataToPost = new PostData()
        { 
            model = "text-davinci-002", 
            prompt =  "The following is a conversation with a bartender in medieval times. The bartender is kindly and wants to help you.\n\nHuman: \"Hi, who are you?\"\nBartender: \"I am Solomon, from Dorn. How can I be of service?\"\n\nHuman: " + input , 
            temperature = 0.7, 
            max_tokens = 256, 
            top_p = 1, 
            frequency_penalty = 0, 
            presence_penalty = 0 
        };
        var postRequest = CreateRequest("https://api.openai.com/v1/completions", RequestType.POST, dataToPost);
        yield return postRequest.SendWebRequest();
        Debug.Log(postRequest.downloadHandler.text);

        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(postRequest.downloadHandler.text);
        //Root deserializedPostData = JsonUtility.FromJson<Root>(postRequest.downloadHandler.text);
        npc_output.text = myDeserializedClass.choices[0].text;
    }

    private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null) {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null) {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer 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");

        return request;
    }

    public enum RequestType {
        GET = 0,
        POST = 1,
        PUT = 2
    }
    private void AttachHeader(UnityWebRequest request,string key,string value)
    {
        request.SetRequestHeader(key, value);
    }
    public class PostResult
    {
        public string success { get; set; }
    }

    [Serializable]
    public class Choice
    {
        public string text { get; set; }
        public int index { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    [Serializable]
    public class Root
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
    [Serializable]
    public class PostData {
        public string model;
        public string prompt;
        public double temperature;
        public int max_tokens;
        public int top_p;
        public int frequency_penalty;
        public int presence_penalty;
    }
}