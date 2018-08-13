using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MessageLog_Manager : MonoBehaviour {

    public int maxActiveMgs = 5;
    static Message[] messages;
    List<Message> storedMessages;

    // TODO: This class is a perfect candidate to listen for a global game event
    // When a message needs to be shown a global event can fire with the Message parameters
    // This manager listens for that event and processes it accordingly

        // For now... just using a singleton and a static method
    public static MessageLog_Manager instance { get; protected set; }

    MessageLogUI logUI;

    private void Awake()
    {
        instance = this;
        messages = new Message[maxActiveMgs];
        storedMessages = new List<Message>();
    }

    public static void NewMessage(string text, Color color)
    {
        if (instance == null)
        {
            return;
        }
        Message newMessage = new Message(text, color);

        // Store last message
        if (messages[messages.Length - 1].messageText != string.Empty)
        {
            instance.StoreMessage(messages[messages.Length - 1]);
            //storedMessages.Add(messages[messages.Length - 1]);
        }

         // Make each message equal the message before it
        for (int i = messages.Length - 1; i > 0; i--)
        {
            messages[i] = messages[i - 1];
        }
        // Set the [0] message to the new message
        messages[0] = newMessage;

        instance.UpdateMessages();

    }

    void UpdateMessages()
    {
        // Send messages to UI component
        if (logUI == null)
        {
            logUI = (MessageLogUI)UI_Manager.instance.GetUIComponent("MessageLog");
            if (logUI == null)
                return;
        }

        logUI.UpdateTexts(messages);
    }

    void StoreMessage(Message msg)
    {
        storedMessages.Add(messages[messages.Length - 1]);
    }

}

public struct Message
{
    public string messageText;
    public Color color;

    public Message(string messageText) : this()
    {
        this.messageText = messageText;
        color = Color.white;
    }

    public Message(string messageText, Color color)
    {
        this.messageText = messageText;
        this.color = color;
    }
}
