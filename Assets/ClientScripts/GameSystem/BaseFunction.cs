using UnityEngine;

public class BaseFunction
{

    public static byte seq = 0x01;
    static  bool threadListenFlag = true;
    // master
    public static int g_numOfDevicesConnected;// 挂载电机/舵机数
    public static int[] g_devAddr = new int[Config.max_num_of_thrusters];// 挂载设备地址
    public static float g_pressure; // 水压
    public static float g_depth;// 水深
    public static float g_hygro1;// 湿度计1的读数
    public static float g_hygro2;// 湿度计2的读数
    public static float g_msBatteryVolts;// 电池电压
    public static int[] g_rpm = new int[Config.max_num_of_thrusters];// 电机转速
    public static float[] g_current = new float[Config.max_num_of_thrusters];// 电机电流
    // test: scanContent scanLen scanTimeIndent
    // slave
    public static int[] g_RPM = new int[Config.num_of_thrusters];// 电机转速
    public static float[] g_modifiedAngle = new float[Config.num_of_thrusters];// 磁角度编码器修正后角度
    public static float[] g_currentAngle = new float[Config.num_of_thrusters];// 磁编码盘当前角度
    public static float g_slvBatteryVolts;// 电池电压
    public static float[] g_rtCurrent = new float[Config.num_of_thrusters]; // 电机实时电流
    public static int[] g_motorPolarNum = new int[Config.num_of_thrusters];// 电机极对数
    public static int[] g_ratedRPM = new int[Config.num_of_thrusters];// 电机额定转速
    public static float[] g_ratedCurrent = new float[Config.num_of_thrusters];// 电机额定电流

    static int WIDTH = (8 * sizeof(ushort));

    static int POLYNOMIAL	=	0x8005;
    static ushort INITIAL_REMAINDER =	0x0000;
    static int FINAL_XOR_VALUE	=	0x0000;
    static int REFLECT_DATA	=	0;
    static  int REFLECT_REMAINDER	=0;
    static  ushort[] crcTable = { 0x0000, 0x8005, 0x800f, 0x000a, 0x801b, 0x001e, 0x0014, 0x8011, 0x8033, 0x0036, 0x003c, 0x8039, 0x0028, 0x802d, 0x8027, 0x0022, 0x8063, 0x0066, 0x006c, 0x8069, 0x0078, 0x807d, 0x8077, 0x0072, 0x0050, 0x8055, 0x805f, 0x005a, 0x804b, 0x004e, 0x0044, 0x8041, 0x80c3, 0x00c6, 0x00cc, 0x80c9, 0x00d8, 0x80dd, 0x80d7, 0x00d2, 0x00f0, 0x80f5, 0x80ff, 0x00fa, 0x80eb, 0x00ee, 0x00e4, 0x80e1, 0x00a0, 0x80a5, 0x80af, 0x00aa, 0x80bb, 0x00be, 0x00b4, 0x80b1, 0x8093, 0x0096, 0x009c, 0x8099, 0x0088, 0x808d, 0x8087, 0x0082, 0x8183, 0x0186, 0x018c, 0x8189, 0x0198, 0x819d, 0x8197, 0x0192, 0x01b0, 0x81b5, 0x81bf, 0x01ba, 0x81ab, 0x01ae, 0x01a4, 0x81a1, 0x01e0, 0x81e5, 0x81ef, 0x01ea, 0x81fb, 0x01fe, 0x01f4, 0x81f1, 0x81d3, 0x01d6, 0x01dc, 0x81d9, 0x01c8, 0x81cd, 0x81c7, 0x01c2, 0x0140, 0x8145, 0x814f, 0x014a, 0x815b, 0x015e, 0x0154, 0x8151, 0x8173, 0x0176, 0x017c, 0x8179, 0x0168, 0x816d, 0x8167, 0x0162, 0x8123, 0x0126, 0x012c, 0x8129, 0x0138, 0x813d, 0x8137, 0x0132, 0x0110, 0x8115, 0x811f, 0x011a, 0x810b, 0x010e, 0x0104, 0x8101, 0x8303, 0x0306, 0x030c, 0x8309, 0x0318, 0x831d, 0x8317, 0x0312, 0x0330, 0x8335, 0x833f, 0x033a, 0x832b, 0x032e, 0x0324, 0x8321, 0x0360, 0x8365, 0x836f, 0x036a, 0x837b, 0x037e, 0x0374, 0x8371, 0x8353, 0x0356, 0x035c, 0x8359, 0x0348, 0x834d, 0x8347, 0x0342, 0x03c0, 0x83c5, 0x83cf, 0x03ca, 0x83db, 0x03de, 0x03d4, 0x83d1, 0x83f3, 0x03f6, 0x03fc, 0x83f9, 0x03e8, 0x83ed, 0x83e7, 0x03e2, 0x83a3, 0x03a6, 0x03ac, 0x83a9, 0x03b8, 0x83bd, 0x83b7, 0x03b2, 0x0390, 0x8395, 0x839f, 0x039a, 0x838b, 0x038e, 0x0384, 0x8381, 0x0280, 0x8285, 0x828f, 0x028a, 0x829b, 0x029e, 0x0294, 0x8291, 0x82b3, 0x02b6, 0x02bc, 0x82b9, 0x02a8, 0x82ad, 0x82a7, 0x02a2, 0x82e3, 0x02e6, 0x02ec, 0x82e9, 0x02f8, 0x82fd, 0x82f7, 0x02f2, 0x02d0, 0x82d5, 0x82df, 0x02da, 0x82cb, 0x02ce, 0x02c4, 0x82c1, 0x8243, 0x0246, 0x024c, 0x8249, 0x0258, 0x825d, 0x8257, 0x0252, 0x0270, 0x8275, 0x827f, 0x027a, 0x826b, 0x026e, 0x0264, 0x8261, 0x0220, 0x8225, 0x822f, 0x022a, 0x823b, 0x023e, 0x0234, 0x8231, 0x8213, 0x0216, 0x021c, 0x8219, 0x0208, 0x820d, 0x8207, 0x0202 };

    public static ushort genCRC16(byte[] data, int nBytes)
    {
        ushort remainder = INITIAL_REMAINDER;
        byte tmp;
        int i;

        // divide the data by the polynomial, a byte at a time.
        for (i = 0; i < nBytes; ++i)
        {
            tmp = (byte)((data[i]) ^ (remainder >> (WIDTH - 8)));
            remainder = (ushort)(crcTable[tmp] ^ (remainder << 8));
        }

        // the final remainder is the CRC
        return (ushort)((remainder) ^ FINAL_XOR_VALUE);
    }

    /*
    for(int i=0; i<Config.max_num_of_thrusters; ++i)
    {
        g_devAddr[i] = 0;
        g_rpm[i] = 0;
        g_current[i] = 0.0;
    }

    for(int i=0; i<Config.num_of_thrusters; ++i)
    {
        g_RPM[i] = 0;
        g_modifiedAngle[i] = 0.0;
        g_currentAngle[i] = 0.0;
        g_rtCurrent[i] = 0.0;
        g_motorPolarNum[i] = 0;
        g_ratedRPM[i] = 0;
        g_ratedCurrent[i] = 0.0;
    }
    */

    // Q14 format process
    public static int floatToQ14(float a)
    {
        // return (int)(a * pow(2,14));
        return (int)Mathf.Round(a * Mathf.Pow(2, 14));
    }

    public static float Q14ToFloat(ushort a)
    {
        return (float)((short)a * Mathf.Pow(2, -14));
    }

    public static float Q14ToFloat(byte a, byte b)
    {
        ushort ua = (ushort)a;
        ua = (ushort)(ua << 8);
        ushort tmp = (ushort)(ua | b);
        return (float)((short)tmp * Mathf.Pow(2, -14));
    }

    public static float pressureToDepth(float pressure)
    {
        float fluidPressure = pressure - Config.atm_pressure;
        float depth = fluidPressure / Config.accel_gravity / Config.fluid_density;
        return depth;
    }

    // packet to bytes
    public static byte[] packet2bytes(DataPacket dp)
    {
        int packetLen = (int)dp.dataLen + 7;
        byte[] bytes = new byte[packetLen];
        int idx = 0;
        bytes[idx++] = dp.dataFrame1;
        bytes[idx++] = dp.dataFrame2;
        bytes[idx++] = dp.sequence;
        bytes[idx++] = dp.devCode;
        bytes[idx++] = dp.dataLen;
        // vector<byte> data = cmd + data
        for (int i = 0; i < (int)dp.dataLen; ++i)
            bytes[idx++] = dp.data[i];
        bytes[idx++] = (byte)(dp.CRC16 >> 8);
        bytes[idx++] = (byte)(dp.CRC16);
        return bytes;
    }

    // bytes to packet
    public static DataPacket bytes2packet(byte[] bytes)
    {
        DataPacket tmp = new DataPacket();
        // int packetLen = bytes[4] + 7;
        tmp.dataFrame1 = bytes[0];
        tmp.dataFrame2 = bytes[1];
        tmp.sequence = bytes[2];
        tmp.devCode = bytes[3];
        tmp.dataLen = bytes[4];
        for (int i = 0; i < tmp.dataLen; ++i)
            tmp.data.Add(bytes[i + 5]);
        tmp.CRC16 = (ushort)((bytes[tmp.dataLen + 5] << 8) | bytes[tmp.dataLen + 6]);
        return tmp;
    }

    // write downstream packet
    public static DataPacket writePacket(byte seq, byte devCode, byte dataLen, byte[] data)
    {
        DataPacket tmp = new DataPacket();
        tmp.dataFrame1 = 0xaa;
        tmp.dataFrame2 = 0x55;
        tmp.sequence = seq;
        tmp.devCode = devCode;
        tmp.dataLen = dataLen;
        for (int i = 0; i < (int)tmp.dataLen; ++i)
            tmp.data.Add(data[i]);
        // compute CRC16
        byte[] msg = new byte[dataLen + 3];
        msg[0] = tmp.sequence;
        msg[1] = tmp.devCode;
        msg[2] = tmp.dataLen;
        for (int i = 0; i < dataLen; ++i)
            msg[i + 3] = tmp.data[i];
        ushort CRC16 = genCRC16(msg, dataLen + 3);
        // printf("Computed CRC16 is:\n");
        // printf("%04x\n",CRC16);
        tmp.CRC16 = CRC16;
        return tmp;
    }

    public    static  bool checkCRC16(byte[] buf)
    {
        int dataLen = buf[4];
        byte[] tmp = new byte[dataLen + 3]; // compute crc without aa55
        Debug.Log("tmp is:\n");
        for (int i = 0; i < dataLen + 3; ++i)
        {
            tmp[i] = buf[i + 2];
        }
        Debug.Log("\n");
        ushort CRC16 = genCRC16(tmp, dataLen + 3);
        ushort bufCRC16 = (ushort)((buf[dataLen + 5] << 8) | buf[dataLen + 6]);
   
        if (CRC16 == bufCRC16)
            return true;
        else
            return false;
    }

    public static bool checkCRC16(DataPacket dp)
    {
        return checkCRC16(packet2bytes(dp));
    }

    // send error code
    public static void sendErrorCode(string errCode)
    {
        Debug.LogFormat("Error: %s\n", errCode);
    }

    public static void showPacketContent(DataPacket dp)
    {
        //Debug.Log("%02x %02x %02x %02x %02x ", dp.dataFrame1, dp.dataFrame2, dp.sequence, dp.devCode, dp.dataLen);
        //for (int i = 0; i < dp.dataLen; ++i)
            //Debug.Log("%02x ", dp.data[i]);
        //Debug.Log("%04x\n", dp.CRC16);
    }
}