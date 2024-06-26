using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UDPReceive : MonoBehaviour
{
    Thread receiveHandThread, receiveAngleThread, receivePosThread , receiveWrongPartThread;
    UdpClient clientHand, clientAngle, clientPos , clientWrongPart;
    public int portHand = 5052, portAngle = 5051, portPos = 5054 , portWrongPart = 5056;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string dataHand, dataAngle, dataPos , dataWrongPart;
    public bool canReadNextLine = false;
    public bool canContinue = true;

    public ButtonEvent buttonEvent;

    public GameObject mouse;

    private float[] transformPosition = new float[3];

    public RectTransform canvasRectTransform;

    float canva_xMin;
    float canva_xMax;
    float canva_yMin;
    float canva_yMax;
    float canva_width;
    float canva_height;
    string[] parts = { "00", "00" };

    Scene m_Scene;
    Scene f_Scene;

    public CircleDrawer circleDrawer;

    public void Start()
    {
        receiveHandThread = new Thread(
            new ThreadStart(ReceiveHandData));
        receiveHandThread.IsBackground = true;
        receiveHandThread.Start();

        receiveAngleThread = new Thread(
            new ThreadStart(ReceiveAngleData));
        receiveAngleThread.IsBackground = true;
        receiveAngleThread.Start();

        receivePosThread = new Thread(
            new ThreadStart(ReceivePosData));
        receivePosThread.IsBackground = true;
        receivePosThread.Start();

        receiveWrongPartThread = new Thread(
            new ThreadStart(ReceiveWrongPartData));
        receiveWrongPartThread.IsBackground = true;
        receiveWrongPartThread.Start();

        transformPosition[0] = mouse.transform.localPosition.x;
        transformPosition[1] = mouse.transform.localPosition.y;
        transformPosition[2] = mouse.transform.localPosition.z;

        canva_xMin = canvasRectTransform.rect.xMin;
        Debug.Log(canva_xMin);
        canva_xMax = canvasRectTransform.rect.xMax;
        Debug.Log(canva_xMax);
        canva_yMin = canvasRectTransform.rect.yMin;
        canva_yMax = canvasRectTransform.rect.yMax;
        canva_width = canvasRectTransform.rect.width;
        canva_height = canvasRectTransform.rect.height;

        m_Scene = SceneManager.GetActiveScene();
        f_Scene = SceneManager.GetActiveScene();
    }


    // receive hand thread
    private void ReceiveHandData()
    {
        int screen_width = 360;
        int screen_height = 480;
        clientHand = new UdpClient(portHand);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = clientHand.Receive(ref anyIP);
                dataHand = Encoding.UTF8.GetString(dataByte);

                parts = dataHand.Trim('[', ']').Split(',');

                float normalizedValue1 = normalize(float.Parse(parts[0]), 100, screen_width - 70, canva_xMin, canva_xMax) + (canva_width / 2);
                float normalizedValue2 = canva_yMax - normalize(float.Parse(parts[1]), 0, screen_height - 275, canva_yMin, canva_yMax);
                string s = normalizedValue1.ToString() + "," + normalizedValue2.ToString();
                //Debug.Log(s);

      
                transformPosition[0] = normalizedValue1;
                transformPosition[1] = normalizedValue2;

                //Debug.Log(worldPos);

                if (printToConsole) { print(dataHand); }
               
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // receive angle thread
    private void ReceiveAngleData()
    {
        clientAngle = new UdpClient(portAngle);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 1);
                byte[] dataByte = clientAngle.Receive(ref anyIP);
                dataAngle = Encoding.UTF8.GetString(dataByte);
                if (dataAngle != "")
                {
                    canReadNextLine = true;
                }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // receive pos thread
    private void ReceivePosData()
    {
        clientPos = new UdpClient(portPos);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 2);
                byte[] dataByte = clientPos.Receive(ref anyIP);
                dataPos = Encoding.UTF8.GetString(dataByte);
                if (dataPos == "")
                {
                    canContinue = true;
                }
                else
                {
                    canContinue = false;
                }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private void ReceiveWrongPartData()
    {
        clientWrongPart = new UdpClient(portWrongPart);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 3);
                byte[] dataByte = clientWrongPart.Receive(ref anyIP);
                dataWrongPart = Encoding.UTF8.GetString(dataByte);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }

    }

    void Update()
    {
        m_Scene = SceneManager.GetActiveScene();

        if (m_Scene.buildIndex != f_Scene.buildIndex)
        {
            if (m_Scene.name != "SampleScene" && m_Scene.name != "CheckPosition")
            {
                mouse = GameObject.Find("Mouse");
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                canvasRectTransform = canvas.GetComponent<RectTransform>();
                transformPosition[0] = mouse.transform.localPosition.x;
                transformPosition[1] = mouse.transform.localPosition.y;
                transformPosition[2] = mouse.transform.localPosition.z;

                canva_xMin = canvasRectTransform.rect.xMin;
                canva_xMax = canvasRectTransform.rect.xMax;
                canva_yMin = canvasRectTransform.rect.yMin;
                canva_yMax = canvasRectTransform.rect.yMax;
                buttonEvent = GetComponent<ButtonEvent>();
                circleDrawer = GetComponent<CircleDrawer>();

            }

            
        }
        f_Scene = SceneManager.GetActiveScene();
       

        if (m_Scene.name != "SampleScene" && m_Scene.name != "CheckPosition")
        {
            //circleDrawer.StopIncreasing();
            mouse.transform.position = new Vector3(transformPosition[0], transformPosition[1], transformPosition[2]);
            buttonEvent.Check_if_button();

            if (circleDrawer.circleImage == null)
            {
                circleDrawer.circleImage = GameObject.Find("Circle").GetComponent<Image>();
            }
        }
    }

    float normalize(float value, float minFrom, float maxFrom, float minTo, float maxTo)
    {
        return (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;
    }


    

}