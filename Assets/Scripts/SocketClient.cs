using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SocketClient : MonoBehaviour
{
    public Text txt;
    private TcpClient _client;
    private NetworkStream _stream;
    public Image loadbar;
    private int numMessages = 0;  // New variable to count the number of messages
    private int totalMessages = 4;  // Total number of messages, should match with server messages
    public float timeBetweenMessages = 0.5f; // Time in seconds


    private void Start()
    {
        StartCoroutine(TryConnectToServer());
    }

    private IEnumerator TryConnectToServer()
    {
        while (_client == null || !_client.Connected)
        {
            try
            {
                _client = new TcpClient("localhost", 1000);
                _stream = _client.GetStream();
                break;  // if connection is successful, exit the loop.
            }
            catch (SocketException se)
            {
                Debug.Log("Connection to server failed: " + se.Message);
            }
            catch (Exception e)
            {
                Debug.Log("Unexpected exception: " + e.Message);
            }
            yield return new WaitForSeconds(3);
        }
    }

    private IEnumerator SmoothFillLoadingBar(float targetAmount, float duration)
    {
        float initialAmount = loadbar.fillAmount;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            loadbar.fillAmount = Mathf.Lerp(initialAmount, targetAmount, timeElapsed / duration);
            yield return null;
        }

        loadbar.fillAmount = targetAmount;  // Ensure it reaches the target amount
    }


    private void Update()
    {
        if (_stream != null && _stream.DataAvailable)
        {
            byte[] buffer = new byte[1024];
            int byteCount = _stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Debug.Log("Received message from server: " + response);

            // If totalMessages is 0, try to parse the received message as the total number of messages
            if (totalMessages == 0)
            {
                if (int.TryParse(response, out int messageCount))
                {
                    totalMessages = messageCount;
                }
            }
            else
            {
                if (!int.TryParse(response, out _))
                {
                    txt.text = response;
                }
                numMessages++;
                StartCoroutine(SmoothFillLoadingBar((float)numMessages / totalMessages, timeBetweenMessages));

            }
        }
    }

    private void OnDestroy()
    {
        if (_stream != null)
        {
            _stream.Close();
            _stream = null;
        }
        if (_client != null)
        {
            _client.Close();
            _client = null;
        }
    }
}
