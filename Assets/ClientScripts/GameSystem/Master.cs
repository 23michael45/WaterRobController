
using UnityEngine;

public class Master
{
    public int m_numOfDevicesConnected = (0);
    public float m_pressure = (0.0f);
    public float m_depth = (0.0f);
    public float m_hygro1 = (0.0f);
    public float m_hygro2 = (0.0f);
    public float m_batteryVolts = (0.0f);
    public byte scanContent = (0x00);
    public byte scanLen = (0x00);
    public byte scanTimeIndent = (0x00);

    string[] communicationErrorCode = {"fail to communicate with slave 0",
                     "fail to communicate with slave 1",
                     "fail to communicate with slave 2",
                     "fail to communicate with slave 3",
                     "fail to communicate with slave 4",
                     "fail to communicate with slave 5",
                     "fail to communicate with MPU",
                     "upstream data too much, FIFO overflow",
                     "downstream data too much, FIFO overflow"
    };
    // motors
    int[] A, B; // A0-A5, B0-B5, when upstream cmd is 0x00, data is 0b0000A5B5A4B4...A0B0, A indicates connected(1) or not(0), B indicates slave is motor or servo
    byte[] m_bits; // 16
    float[] m_rpm;
    float[] m_current;


    public Master()
    {
        A = new int[Config.max_num_of_thrusters];
        B = new int[Config.max_num_of_thrusters];
        m_bits = new byte[Config.max_num_of_thrusters * 2 + 4];
        m_rpm = new float[Config.max_num_of_thrusters];
        m_current = new float[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            A[i] = 0;
            B[i] = 0;
            m_rpm[i] = 0;
            m_current[i] = 0.0f;
        }
        for (int i = 0; i < Config.max_num_of_thrusters * 2 + 4; ++i)
            m_bits[i] = 0;
    }


    void bytes2bits(byte[] bytes) // 2 hex bytes to 16 binary bits
    {
        for (int i = 0; i < 16; ++i)
        {
            byte right = (byte)(0x01 << (7 - i + i / 8 * 8));
            byte b = (byte)(bytes[i / 8] & right);
            m_bits[i] = (b != 0) ? (byte)0x01 : (byte)0x00;

        }
    }

    public void masterUpstreamPacketAnalyze(DataPacket dp)
    {
        // if(dp.dataLen < 7)
        // {
        // 	printf("WRONG MASTER PACKET!\n");
        // 	showPacketContent(dp);
        // 	return;
        // }
        if (dp.devCode == 0x00)
        {
            switch (dp.data[0]) // cmd
            {
                case 0x00: // number & addr of devices, data is 0b0000A5B5A4B4...A0B0, Ai = 1, slave connected, Ai = 0, slave not connected, Bi = 1, slave is servo, Bi = 0, slave is motor
                    {
                        byte[] tmpData = { dp.data[1], dp.data[2] };
                        // 0000A5B5A4B4...A0B0 written in bits[16]
                        bytes2bits(tmpData);
                        // for(int i=0; i<16; ++i)
                        //     printf("%d",bits[i]);
                        // printf("\n");
                        // A[5~0] bits[4,6,8,10,12,14]
                        // B[5~0] bits[5,7,9,11,13,15]
                        // for(int i=MAX_NUM_OF_THRUSTERS-1, j=0; j<MAX_NUM_OF_THRUSTERS; --i, ++j)
                        for (int i = Config.max_num_of_thrusters - 1, j = 0; j < Config.max_num_of_thrusters; --i, ++j)
                        {
                            A[i] = m_bits[4 + j * 2];
                            B[i] = m_bits[5 + j * 2];
                            BaseFunction.g_devAddr[i] = A[i];
                        }
                        // number of devices
                        // printf("addr of devices:\n");
                        m_numOfDevicesConnected = 0;
                        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
                        {
                            // printf("%d\t",A[i]);
                            if (A[i] == 1)
                                m_numOfDevicesConnected++;
                        }
                        BaseFunction.g_numOfDevicesConnected = m_numOfDevicesConnected;
                        // printf("\n");
                        // addr of devices, still use A[i] to identify the addr
                        return;
                        break;
                    }
                case 0x01: // sendPressure() Q14 format
                    {
                        // pressure = maxPressure * Q14ToFloat(dp.data[1], dp.data[2]);
                        m_pressure = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        m_depth = BaseFunction.pressureToDepth(m_pressure);
                        BaseFunction.g_pressure = m_pressure;
                        BaseFunction.g_depth = m_depth;
                        return;
                        break;
                    }
                case 0x02: // sendHygro1() Q14 format
                    {
                        m_hygro1 = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_hygro1 = m_hygro1;
                        return;
                    }
                case 0x03: // sendHygro2() Q14 format
                    {
                        m_hygro2 = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_hygro2 = m_hygro2;
                        return;
                    }
                case 0x04: // sendBatteryVolt() Q14 format
                    {
                        m_batteryVolts = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]) * Config.max_voltage;
                        BaseFunction.g_msBatteryVolts = m_batteryVolts;
                        return;
                    }
                case 0x05: // send communication error code, data is 0x00 - 0x06
                    {
                        int x = (int)dp.data[1];
                        BaseFunction.sendErrorCode(communicationErrorCode[x]);
                        return;
                    }
                case 0x06:
                    {
                        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
                        {
                            m_rpm[i] = BaseFunction.Q14ToFloat(dp.data[2 * i + 1], dp.data[2 * i + 2]) * Config.max_rpm;
                            BaseFunction.g_rpm[i] = (int)m_rpm[i];
                        }
                        return;
                    }
                case 0x07:
                    {
                        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
                        {
                            m_current[i] = BaseFunction.Q14ToFloat(dp.data[2 * i + 1], dp.data[2 * i + 2]) * Config.max_current;
                            BaseFunction.g_current[i] = m_current[i];
                        }
                        return;
                    }
                default:
                    {
                        Debug.Log("Master upstream packet cmd out of range, invalid packet!\n");
                        break;
                    }
            }
        }
    }

    public DataPacket scanPeriodically(byte dev, byte len, byte deltat) // 0x05 + 0xAABBCC, AA: 0x00~0x15, BB: scan length <= 18 bytes, unit 1 byte, CC: scan time indent, unit 10ms, CC=0 means cancel scan
    {
        // byte tmpData[] = {0x05, scanContent, scanLen, scanTimeIndent};
        byte[] tmpData = { 0x05, dev, len, deltat };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x04, tmpData);
        return tmp;
    }

    // downstream functions only send cmd but don't analyze packet
    public DataPacket getDevNumAndAddr()
    {
        byte[] tmpData = { 0x00 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket getPressure()
    {
        byte[] tmpData = { 0x01 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket getHygro1()
    {
        byte[] tmpData = { 0x02 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket getHygro2()
    {
        byte[] tmpData = { 0x03 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket getBatteryVolts()
    {
        byte[] tmpData = { 0x04 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket setAllRPM(int s0, int s1, int s2, int s3, int s4, int s5) // 0x06 + 0xAAAABBBBCCCCDDDDEEEEFFFF, AAAA(slave 0) - FFFF(slave 5), Q14 format
    {
        if (s0 > Config.max_rpm)
            s0 = Config.max_rpm;
        if (s0 < -Config.max_rpm)
            s0 = -Config.max_rpm;
        if (s1 > Config.max_rpm)
            s1 = Config.max_rpm;
        if (s1 < -Config.max_rpm)
            s1 = -Config.max_rpm;
        if (s2 > Config.max_rpm)
            s2 = Config.max_rpm;
        if (s2 < -Config.max_rpm)
            s2 = -Config.max_rpm;
        if (s3 > Config.max_rpm)
            s3 = Config.max_rpm;
        if (s3 < -Config.max_rpm)
            s3 = -Config.max_rpm;
        if (s4 > Config.max_rpm)
            s4 = Config.max_rpm;
        if (s4 < -Config.max_rpm)
            s4 = -Config.max_rpm;
        if (s5 > Config.max_rpm)
            s5 = Config.max_rpm;
        if (s5 < -Config.max_rpm)
            s5 = -Config.max_rpm;
        // cast to uint16_t
        s0 = (ushort)(s0) & 0xffff;
        s1 = (ushort)(s1) & 0xffff;
        s2 = (ushort)(s2) & 0xffff;
        s3 = (ushort)(s3) & 0xffff;
        s4 = (ushort)(s4) & 0xffff;
        s5 = (ushort)(s5) & 0xffff;
        byte[] tmpData = { 0x06, (byte)(s0 >> 8), (byte)(s0), (byte)(s1 >> 8), (byte)s1, (byte)(s2 >> 8), (byte)s2, (byte)(s3 >> 8), (byte)s3, (byte)(s4 >> 8), (byte)s4, (byte)(s5 >> 8), (byte)s5 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x0d, tmpData);
        return tmp;
    }

    bool itemInArray(int x, int[] arr, int size)
    {
        for (int i = 0; i < size; ++i)
        {
            if (x == arr[i])
                return true;
        }
        return false;
    }

    public DataPacket getAllRPM() // getAllRPMOrAngle
    {
        byte[] tmpData = { 0x07 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }

    public DataPacket getAllCurrent()
    {
        byte[] tmpData = { 0x08 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, 0x00, 0x01, tmpData);
        return tmp;
    }
}