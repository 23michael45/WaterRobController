using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class SerialPortOperation : MonoBehaviour
{

    public static SerialPortOperation mInstance;
    public enum OPS {
        IDLE = 0,
        UP,
        DOWN,
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT,
        LEFT_TURN,
        RIGHT_TURN,
        BACKWARD_LEFT_TURN,
        BACKWARD_RIGHT_TURN
    }

    public OPS curStatus = OPS.IDLE;
    int curRPM = 0;

    Coroutine _UpdateCoroutine;

    public void goUp(SerialPortController sp, Master ms, int rpm) // go up only set middle rpm
    {
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == Config.motorIdx[0])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.UP;
    }

    public void goDown(SerialPortController sp, Master ms, int rpm)
    {
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == Config.motorIdx[0])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.DOWN;
    }

    public void moveForward(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5 && Config.num_of_thrusters != 3)
            return;

        int[] idx = new int[2];
        if (Config.num_of_thrusters == 5) // select motors to run: lowerleft, lowerright -> motorIdex[3,4]
        {
            idx[0] = Config.motorIdx[3];
            idx[1] = Config.motorIdx[4];
        }
        else if (Config.num_of_thrusters == 3)
        {
            idx[0] = Config.motorIdx[1];
            idx[1] = Config.motorIdx[2];
        }
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == idx[0] || i == idx[1])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.FORWARD;
    }

    public void moveBackward(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5 && Config.num_of_thrusters != 3)
            return;
        // select motors to run: upperleft, upperright -> motorIdex[1,2]
        int[] idx = { Config.motorIdx[1], Config.motorIdx[2] };
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (Config.num_of_thrusters == 5)
            {
                if (i == idx[0] || i == idx[1])
                    rpmToSet[i] = rpm;
                else
                    rpmToSet[i] = 0;
            }
            else if (Config.num_of_thrusters == 3)
            {
                if (i == idx[0] || i == idx[1])
                    rpmToSet[i] = -rpm;
                else
                    rpmToSet[i] = 0;
            }
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.BACKWARD;
    }

    public void turnLeft(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5 && Config.num_of_thrusters != 3)
            return;
        int[] idx = new int[2];
        if (Config.num_of_thrusters == 5) // select motors to run: lowerleft, lowerright -> motorIdex[3,4]
        {
            idx[0] = Config.motorIdx[3];
            idx[1] = Config.motorIdx[4];
        }
        else if (Config.num_of_thrusters == 3)
        {
            idx[0] = Config.motorIdx[1];
            idx[1] = Config.motorIdx[2];
        }

        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == idx[0])
                rpmToSet[i] = rpm;
            else if (i == idx[1])
                rpmToSet[i] = -rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.LEFT_TURN;
    }

    public void turnRight(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5 && Config.num_of_thrusters != 3)
            return;
        int[] idx = new int[2];
        if (Config.num_of_thrusters == 5) // select motors to run: lowerleft, lowerright -> motorIdex[3,4]
        {
            idx[0] = Config.motorIdx[3];
            idx[1] = Config.motorIdx[4];
        }
        else if (Config.num_of_thrusters == 3)
        {
            idx[0] = Config.motorIdx[1];
            idx[1] = Config.motorIdx[2];
        }

        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == idx[0])
                rpmToSet[i] = -rpm;
            else if (i == idx[1])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.RIGHT_TURN;
    }

    public void brake(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.setAllRPM(0, 0, 0, 0, 0, 0));
        // curStatus = IDLE;
    }

    public void changeSpeed(OPS status, SerialPortController sp, Master ms)
    {
        switch (status)
        {
            case OPS.UP:
                goUp(sp, ms, curRPM);
                break;
            case OPS.DOWN:
                goDown(sp, ms, curRPM);
                break;
            case OPS.FORWARD:
                moveForward(sp, ms, curRPM);
                break;
            case OPS.BACKWARD:
                moveBackward(sp, ms, curRPM);
                break;
            case OPS.LEFT_TURN:
                turnLeft(sp, ms, curRPM);
                break;
            case OPS.RIGHT_TURN:
                turnRight(sp, ms, curRPM);
                break;

            case OPS.LEFT:
                if (Config.num_of_thrusters == 5)
                {
                    moveLeft(sp, ms, curRPM);
                }
                break;
            case OPS.RIGHT:
                if (Config.num_of_thrusters == 5)
                {
                    moveRight(sp, ms, curRPM);
                }
                break;
            default:
                break;
        }

    }






    public void moveLeft(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5)
            return;
        // select motors to run: upperright, lowerright -> motorIdex[2,4]
        int[] idx = { Config.motorIdx[2], Config.motorIdx[4] };
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == idx[0] || i == idx[1])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.LEFT;
    }

    public void moveRight(SerialPortController sp, Master ms, int rpm)
    {
        if (Config.num_of_thrusters != 5)
            return;
        // select motors to run: upperleft, lowerleft -> motorIdex[1,3]
        int[] idx = { Config.motorIdx[1], Config.motorIdx[3] };
        int[] rpmToSet = new int[Config.max_num_of_thrusters];
        for (int i = 0; i < Config.max_num_of_thrusters; ++i)
        {
            if (i == idx[0] || i == idx[1])
                rpmToSet[i] = rpm;
            else
                rpmToSet[i] = 0;
        }
        sp.sendDownstreamPacket(ms.setAllRPM(rpmToSet[0], rpmToSet[1], rpmToSet[2], rpmToSet[3], rpmToSet[4], rpmToSet[5]));
        curStatus = OPS.RIGHT;
    }

    public void turnBackLeft(SerialPortController sp, Master ms, int rpm)
    {
        // select motors to run: upperleft, upperright -> motorIdex[1,2]
        curStatus = OPS.BACKWARD_LEFT_TURN;
    }

    public void turnBackRight(SerialPortController sp, Master ms, int rpm)
    {
        // select motors to run: upperleft, upperright -> motorIdex[1,2]
        curStatus = OPS.BACKWARD_RIGHT_TURN;
    }



    #region Event

    public void OnForward(SerialPortController sp, Master ms)
    {
        curRPM = Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        moveForward(sp, ms, curRPM);
    }
    public void OnBackward(SerialPortController sp, Master ms)
    {
        Debug.LogFormat("current RPM: %d\n", curRPM);
        moveBackward(sp, ms, curRPM);
    }
    public void OnLeft(SerialPortController sp, Master ms)
    {
        if (Config.num_of_thrusters == 5)
        {
            curRPM = Config.max_rpm / 10;
            Debug.LogFormat("current RPM: %d\n", curRPM);
            moveLeft(sp, ms, curRPM);
        }
    }
    public void OnRight(SerialPortController sp, Master ms)
    {
        if (Config.num_of_thrusters == 5)
        {
            curRPM = Config.max_rpm / 10;
            Debug.LogFormat("current RPM: %d\n", curRPM);
            moveRight(sp, ms, curRPM);
        }
    }

    public void OnDown(SerialPortController sp, Master ms)
    {
        curRPM = Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        goDown(sp, ms, curRPM);
    }
    public void OnUp(SerialPortController sp, Master ms)
    {
        curRPM = -Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        goUp(sp, ms, curRPM);
    }
    public void OnIdle(SerialPortController sp, Master ms)
    {
        brake(sp, ms);
    }
    public void OnSpeedUp(SerialPortController sp, Master ms)
    {
        curRPM += Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        changeSpeed(curStatus, sp, ms);
    }
    public void OnSpeedDown(SerialPortController sp, Master ms)
    {
        curRPM -= Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        changeSpeed(curStatus, sp, ms);
    }


    public void OngetDevNumAndAddr(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getDevNumAndAddr());
    }
    public void OngetPressure(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getPressure());
    }
    public void OngetHygro1(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getHygro1());
    }
    public void OngetHygro2(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getHygro2());
    }
    public void OngetAllRPM(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getAllRPM());
    }
    public void OngetAllCurrent(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getAllCurrent());
    }
    public void OngetBatteryVolts(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.getBatteryVolts());
    }
    public void DefaultscanPeriodically(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.scanPeriodically(0x16, 0x0c, 4));
        sp.sendDownstreamPacket(ms.scanPeriodically(0x17, 0x0c, 4));
    }
    public void StopscanPeriodically(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.scanPeriodically(0x16, 0x0c, 0));
        sp.sendDownstreamPacket(ms.scanPeriodically(0x17, 0x0c, 0));
    }
    public void OnTurnLeft(SerialPortController sp, Master ms)
    {
        curRPM = Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        turnLeft(sp, ms, curRPM);
    }
    public void OnTurnRight(SerialPortController sp, Master ms)
    {
        curRPM = Config.max_rpm / 10;
        Debug.LogFormat("current RPM: %d\n", curRPM);
        turnRight(sp, ms, curRPM);
    }

    public void OnMonsterMode(SerialPortController sp, Master ms)
    {
        sp.sendDownstreamPacket(ms.setAllRPM(5000, 5000, 5000, 5000, 5000, 0));
    }
    public void OnRealMonsterMode(SerialPortController sp, Master ms)
    {
        int randRPM = UnityEngine.Random.Range(0, 1000);
        for (int i = 0; i < 10; ++i)
        {
            randRPM = UnityEngine.Random.Range(0, 1000) % 11384 + 5000;
            if (UnityEngine.Random.Range(0, 1000) % 2 == 0)
                sp.sendDownstreamPacket(ms.setAllRPM(randRPM, randRPM, randRPM, randRPM, randRPM, 0));
            else
            {
                randRPM = -randRPM;
                sp.sendDownstreamPacket(ms.setAllRPM(randRPM, randRPM, randRPM, randRPM, randRPM, 0));
            }
            Debug.LogFormat("rpm: %d\n", randRPM);
        }
    }
    #endregion
    private void Awake()
    {
        mInstance = this;
    }

    private void Start()
    {
        _UpdateCoroutine = StartCoroutine(UpdateData());
    }
    private void OnDestroy()
    {
        StopCoroutine(_UpdateCoroutine);
    }
    IEnumerator UpdateData()
    {
        while(true)
        {
            OngetAllCurrent(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetAllRPM(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetBatteryVolts(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetDevNumAndAddr(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetHygro1(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetHygro2(SerialPortController.mInstance, SerialPortController.mInstance.ms);
            OngetPressure(SerialPortController.mInstance, SerialPortController.mInstance.ms);

            yield return new WaitForSeconds(0.5f);

        }
    }
    


    private void Update()
    {
        
        if(Input.GetKey(KeyCode.A))
        {
            OnLeft(SerialPortController.mInstance, SerialPortController.mInstance.ms);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            OnRight(SerialPortController.mInstance, SerialPortController.mInstance.ms);
        }
        else if (Input.GetKey(KeyCode.S))
        {

            OnBackward(SerialPortController.mInstance, SerialPortController.mInstance.ms);
        }
        else if (Input.GetKey(KeyCode.W))
        {

            OnForward(SerialPortController.mInstance, SerialPortController.mInstance.ms);
        }

        else if (Input.GetKey(KeyCode.Space))
        {
            OnIdle(SerialPortController.mInstance, SerialPortController.mInstance.ms);
        }
    }


}