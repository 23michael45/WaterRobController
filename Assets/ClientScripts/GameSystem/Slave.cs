using UnityEngine;

public class Slave
{
    int[] m_RPM;
    float[] m_modifiedAngle;
    float[] m_currentAngle;
    float m_batteryVolts;
    float[] m_rtCurrent; // real-time current
    int[] m_motorPolarNum;
    int[] m_ratedRPM;
    float[] m_ratedCurrent;
    string[] deviceErrorCode = {"peak current sensor zero current deviation too large",
                   "effective current sensor zero current deviation too large",
                   "MOS upper part damaged",
                   "MOS lower part damaged",
                   "magnet encoding disk exception",
                   "current over max",
                   "lack phase",
                   "battery voltage protection too low",
                   "battery voltage protection too high",
                   "chip temperature too high"};


    public Slave()
    {
        m_RPM = new int[Config.num_of_thrusters];
        m_modifiedAngle = new float[Config.num_of_thrusters];
        m_currentAngle = new float[Config.num_of_thrusters];
        m_rtCurrent = new float[Config.num_of_thrusters];
        m_motorPolarNum = new int[Config.num_of_thrusters];
        m_ratedRPM = new int[Config.num_of_thrusters];
        m_ratedCurrent = new float[Config.num_of_thrusters];
        for (int i = 0; i < Config.num_of_thrusters; ++i)
        {
            m_RPM[i] = 0;
            m_modifiedAngle[i] = 0.0f;
            m_currentAngle[i] = 0.0f;
            m_rtCurrent[i] = 0.0f;
            m_motorPolarNum[i] = 0;
            m_ratedRPM[i] = 0;
            m_ratedCurrent[i] = 0.0f;
        }
    }


    public void slaveUpstreamPacketAnalyze(DataPacket dp)
    {
        // if(dp.dataLen < 7)
        // {
        // 	Debug.Log("WRONG SLAVE PACKET!\n");
        // 	return;
        // }
        int idx = (int)dp.devCode - 7; // slave 0-5, devCode 7-12
        if (idx >= 0 && idx <= 5)
        {
            switch (dp.data[0]) // cmd
            {
                case 0x00:
                    {
                        // current motorRPM or servoAngle, -1.0~1.0 rpm or -pi~pi angle
                        // +: forward, up, right; -: backward, down, left
                        // +0.5*pi: horizontal forward, right-leaning
                        // -0.5*pi: horizontal backward, left-leaning
                        // RPMorAngle[idx] = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        m_RPM[idx] = (int)BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]) * Config.max_rpm;
                        BaseFunction.g_RPM[idx] = m_RPM[idx];
                        return;
                    }
                case 0x01: // 角度编码器修正角度
                    {
                        m_modifiedAngle[idx] = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_modifiedAngle[idx] = m_modifiedAngle[idx];
                        return;
                    }
                case 0x02: // 当前角度编码器角度
                    {
                        m_currentAngle[idx] = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_currentAngle[idx] = m_currentAngle[idx];
                        return;
                    }
                case 0x03: // battery voltage, full: 11.1V, Q14 format
                    {
                        m_batteryVolts = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_msBatteryVolts = m_batteryVolts;
                        return;
                    }
                case 0x04: // real-time current
                    {
                        m_rtCurrent[idx] = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_rtCurrent[idx] = m_rtCurrent[idx];
                        return;
                    }
                case 0x05: // motor极对数p
                    {
                        m_motorPolarNum[idx] = (int)BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        // update global vars
                        BaseFunction.g_motorPolarNum[idx] = m_motorPolarNum[idx];
                        return;
                    }
                case 0x06: // 标定为+1.0的速度
                    {
                        m_ratedRPM[idx] = (int)BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_ratedRPM[idx] = m_ratedRPM[idx];
                        return;
                    }
                case 0x07: // 标定为+1.0的消耗电流
                    {
                        m_ratedCurrent[idx] = BaseFunction.Q14ToFloat(dp.data[1], dp.data[2]);
                        BaseFunction.g_ratedCurrent[idx] = m_ratedCurrent[idx];
                        return;
                    }
                case 0x08: // device error code
                    {
                        int x = (int)dp.data[1];
                        BaseFunction.sendErrorCode(deviceErrorCode[x]);
                        return;
                    }
                default:
                    {
                        Debug.Log("Slave upstream packet cmd out of range, invalid packet!\n");
                        break;
                    }
            }
        }
    }

    DataPacket setRPM(byte devCode, ushort rpm) // 0x00 + Q14 +-1.0 rpm or +-pi angle
    {
        if (rpm > Config.max_rpm)
            rpm =(ushort) Config.max_rpm;
        if (rpm < -Config.max_rpm)
            rpm = (ushort)-Config.max_rpm;
        devCode += 7;
        byte[] tmpData = { 0x00, (byte)(rpm >> 8), (byte)(rpm & 0xff) };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x03, tmpData);
        return tmp;
    }

    DataPacket setAngle(byte devCode, byte angle) // 0x00 + Q14 +-1.0 rpm or +-pi angle
    {
        devCode += 7; // input 0~5, actual 7~12
        byte[] tmpData = { 0x00, angle };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x02, tmpData);
        return tmp;
    }

    DataPacket getRPMorAngle(byte devCode) // 0x01 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x01 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket getModifiedAngle(byte devCode) // 0x02 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x02 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket setModifiedAngle(byte devCode, byte modifiedAngle) // 0x03 + Q14 +-pi angle
    {
        devCode += 7;
        byte[] tmpData = { 0x03, modifiedAngle };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x02, tmpData);
        return tmp;
    }

    DataPacket getCurrentAngle(byte devCode) // 0x04 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x04 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket getMotorPolarNum(byte devCode) // 0x05 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x05 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket setMotorPolarNum(byte devCode, byte polarNum) // 0x06 + polar num
    {
        devCode += 7;
        byte[] tmpData = { 0x06, polarNum };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x02, tmpData);
        return tmp;
    }

    DataPacket getRatedRPM(byte devCode) // 0x07 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x07 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket setRatedRPM(byte devCode, ushort ratedRPM) // 0x08 + rated rpm, say ratedRPM = 3000 or 0x0bb8
    {
        devCode += 7;
        byte[] tmpData = { 0x08, (byte)(ratedRPM >> 8), (byte)(ratedRPM & 0x00ff) };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x03, tmpData);
        return tmp;
    }

    DataPacket getRatedCurrent(byte devCode) // 0x09 + emptyData
    {
        devCode += 7;
        byte[] tmpData = { 0x09 };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x01, tmpData);
        return tmp;
    }

    DataPacket setRatedCurrent(byte devCode, ushort ratedCurrent) // 0x0a + rated current, unit 10mA
    {
        devCode += 7;
        byte[] tmpData = { 0x0a, (byte)(ratedCurrent >> 8), (byte)(ratedCurrent & 0x00ff) };
        DataPacket tmp = BaseFunction.writePacket(BaseFunction.seq++, devCode, 0x03, tmpData);
        return tmp;
    }
}