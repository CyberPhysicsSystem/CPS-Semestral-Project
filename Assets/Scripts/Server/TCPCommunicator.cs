using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class TCPCommunicator : MonoBehaviour
{
    protected Promise promise;
    protected Socket socket;
    protected Encoding encoding;
    protected float ellapsed = 0;
    protected bool bHandshakeSuccessful = false;
    protected byte[] size_buffer = new byte[4];

    void Start()
    {
        ReadSettings();

        if (OpenSocket() != 0) return;
        FirstHandshake();
    }

    private void Update()
    {
        if (!bHandshakeSuccessful) return;
        MyUpdate();
    }

    public virtual void MyUpdate()
    {
        ellapsed += Time.deltaTime;
        if (ellapsed > 1)
        {
            try
            {
                ellapsed -= 1;
                Send("Hello");
                Debug.Log(Receive());
            }
            catch (SocketException e)
            {
                Debug.LogError("Connection has been cancelled from remote");
                Debug.Log(e.ToString());
                bHandshakeSuccessful = false;
            }
        }
    }

    private void OnDestroy()
    {
        if(bHandshakeSuccessful)
        {
            socket.Send(encoding.GetBytes(promise.end));
            socket.Close();
        }
    }

    protected void ReadSettings()
    {
        string path = "Assets/promise.json";
        StreamReader reader = new StreamReader(path);
        string contents = reader.ReadToEnd();
        reader.Close();

        promise = JsonUtility.FromJson<Promise>(contents);
        SetEncoding();
    }

    protected void SetEncoding()
    {
        switch (promise.encoding)
        {
            default:
            case "utf8":
                encoding = Encoding.UTF8;
                break;
        }
    }

    protected int OpenSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ip = IPAddress.Parse(promise.ip);
        IPEndPoint ep = new IPEndPoint(ip, promise.port);
        try
        {
            Debug.Log("Connecting to server...");
            socket.Connect(ep);
            return 0;
        }
        catch (SocketException e)
        {
            Debug.LogError("Connection Failed!");
            Debug.Log(e.ToString());
            return -1;
        }
    }

    protected void FirstHandshake()
    {
        Debug.Log("First handshake test");
        try
        {
            Send(promise.begin);
            string resp = Receive();
            if (resp.Equals(promise.begin))
            {
                Debug.Log("Connection Established");
                bHandshakeSuccessful = true;
            }
            else
            {
                Debug.Log("First hankshake failed");
                socket.Close();
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("Something went wrong!");
            Debug.Log(e.ToString());
        }
    }

    protected int GetSizeFromRemote()
    {
        socket.Receive(size_buffer);                        // receive response
        int size = BitConverter.ToInt32(size_buffer);       // convert response into int to see the value
        Array.Clear(size_buffer, 0, 4);                     // clear size buffer
        return size;
    }

    protected void Send(string data)
    {
        byte[] toSend = encoding.GetBytes(data);            // convert data into byte array
        socket.Send(BitConverter.GetBytes(toSend.Length));  // send size of the byte array
        if (GetSizeFromRemote() == toSend.Length)           // if the remote's recognition is correct
            socket.Send(toSend);                                // send the actual data
        else                                                // if not
            throw new SocketException();                        // throw exception
    }

    protected string Receive()
    {
        int size = GetSizeFromRemote();                     // read buffer size from remote
        socket.Send(BitConverter.GetBytes(size));           // send back the size I recognized
        byte[] data = new byte[size];                       // create the buffer matching data size
        socket.Receive(data);                               // read from remote
        string resp = encoding.GetString(data);             // convert back to string
        // resp = resp.Trim('\0');                          // trim string (probably not required)
        return resp;                                        // return it
    }
}
