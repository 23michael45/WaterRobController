using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialPortController : MonoBehaviour {


    public static SerialPortController mInstance;

    private static Mutex mutex = new Mutex();
    

    private Thread receiveThread;  //用于接收消息的线程
    private Thread sendThread;     //用于发送消息的线程
                                   // Use this for initialization

    public string ComName = "";
    public int PortRate = 115200;

    Queue<byte> PortBuffer = new Queue<byte>();
    Queue<DataPacket> CmdQueue = new Queue<DataPacket>();


    SerialPort _SerialPort;

    public Master ms = new Master();
    public Slave slv = new Slave();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        mInstance = this;
    }

    void Start () {

        Config.loadConfigFile(Application.streamingAssetsPath + "/config.conf");

        ComName = Config.portName;
        PortRate = Config.baudrate;

        _SerialPort = new SerialPort(ComName, PortRate);  //这里的"COM4"是我的设备，可以在设备管理器查看。
                                            //9600是默认的波特率，需要和Arduino对应，其它的构造参数可以不用配置。
        _SerialPort.ReadTimeout = 500;
        _SerialPort.Open();
        StartThread();
	}

    private void OnDestroy()
    {
        if(_SerialPort != null)
        {
            _SerialPort.Close();
            _SerialPort = null;
        }

        receiveThread.Abort();
    }
    void StartThread()
    {
        receiveThread = new Thread(ReceiveThread);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        //sendThread = new Thread(SendThread);
        //sendThread.IsBackground = true;
        //sendThread.Start();
        
    }
    private void SendThread()
    {
        int i = 0;
        while (true)
        {
            Thread.Sleep(20);
            this._SerialPort.DiscardInBuffer();
            if (i > 255)
                i = 0;
            _SerialPort.WriteLine(i++.ToString());
            Debug.Log(i++.ToString());
        }
    }
    private void ReceiveThread()
    {
        while (true)
        {
            if (this._SerialPort != null && this._SerialPort.IsOpen)
            {
                try
                {
                    int charRec = _SerialPort.ReadByte();            //SerialPort读取数据有多种方法，我这里根据需要使用了ReadLine()

                    mutex.WaitOne();

                    PortBuffer.Enqueue((byte)charRec);

                    mutex.ReleaseMutex();
                    
                }
                catch
                {
                    //continue;
                }
            }
            else
            {
                break;
            }
        }
    }

    public void sendDownstreamPacket(DataPacket p)
    {
        int size = p.dataLen + 7;
        byte[] buf = BaseFunction.packet2bytes(p);
        this._SerialPort.Write( buf, 0 ,size);
       
    }
    void analyzeUpstreamPacket(DataPacket dp)
    {
        switch (dp.devCode)
        {
            case 0x00: // master
                       // printf("analyzing master upstream packet: ");
                       // showPacketContent(dp);
                ms.masterUpstreamPacketAnalyze(dp); // another switch
                break;
            case 0x01: // mpu
                Debug.Log("analyzing MPU upstream packet...\n");
                break;
            case 0x07: // slave 0
            case 0x08: // slave 1
            case 0x09: // slave 2
            case 0x0a: // slave 3
            case 0x0b: // slave 4
            case 0x0c: // slave 5
                       // printf("analyzing slave upstream packet: ");
                       // showPacketContent(dp);
                slv.slaveUpstreamPacketAnalyze(dp); // another switch
                                                     // printf("global motor %d polar num is %d\n", dp.devCode-7, g_motorPolarNum[dp.devCode-7]);
                break;
            default:
                Debug.Log("no device code match, can't analyze upstream packet!\n");
                break;
        }
    }
    void Parse()
    {
        if (PortBuffer.Count > 0)
        {
            byte dataFrame1 = PortBuffer.Dequeue();
            if (dataFrame1 == 0xaa)
            {
                byte dataFrame2 = PortBuffer.Dequeue();
                if (dataFrame2 == 0x55)
                {


                    byte sequence = PortBuffer.Dequeue();
                    DataPacket dp = new DataPacket();
                    dp.dataFrame1 = dataFrame1;
                    dp.dataFrame2 = dataFrame2;
                    dp.sequence = sequence;

                    byte devCode = PortBuffer.Dequeue();
                    dp.devCode = devCode;

                    byte dataLen = PortBuffer.Dequeue();
                    dp.dataLen = dataLen;
                    int idx = 0;
                    ///// debug here
                    while ((dp.dataLen - idx) > 0 && PortBuffer.Count > 0) // cmd + data
                    {
                        byte d = PortBuffer.Dequeue();
                        dp.data.Add(d);
                        // printf("data[%d]: %02x\n", idx, rev[0]);
                        idx++;

                    }
                    byte high = PortBuffer.Dequeue();
                    byte low = PortBuffer.Dequeue();
                    dp.CRC16 = (ushort)(high << 8 | low);
                    // check crc here
                    // analyze packet dp
                    analyzeUpstreamPacket(dp);
                                               
                       
                }
                else if (dataFrame2 == 0xaa)
                {

                }
            }



        }
    }



    // Update is called once per frame
    void Update () {

        mutex.WaitOne();
        Parse();
        mutex.ReleaseMutex();
    }

    byte[] ReadCmd(byte b1, byte b2)
    {
        int len = 9;
        byte[] _RawData = new byte[len];
        _RawData[0] = b1;
        _RawData[1] = b2;
        for (int i = 2;i < len;i++)
        {
            _RawData[i] = PortBuffer.Dequeue();
        }
        return _RawData;
    }
    
}

public class DataPacket
{
    public byte dataFrame1;
    public byte dataFrame2;
    public byte sequence;

    public byte devCode;
    public byte dataLen;
    public ushort CRC16;

    public List<byte> data = new List<byte>();
    public DataPacket()
    {
        Parse();
    }

    public virtual void Parse()
    {

    }
}