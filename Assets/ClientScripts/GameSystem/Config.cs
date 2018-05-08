public class Config
{
    public static string username = "username";
    public static int portNum = 16;
    public static string portName = "COM1";
    public static int baudrate = 115200;
    public static float accel_gravity = 9.8f;
    public static int fluid_density = 1025;
    public static int atm_pressure = 101000;
    public static int max_pressure = 11;
    public static int max_depth = 100;
    public static int max_num_of_thrusters = 6;
    public static int num_of_thrusters = 3;
    public static int max_rpm = 16384;
    public static int[] motorIdx = { 0, 1, 2, 3, 4, 5 };
    // int *motorIdx = new int[max_num_of_thrusters];
    public static int max_voltage = 12;
    public static int max_current = 5;


    public static bool loadConfigFile(string cfg_file)
    {
        IniParser inifile = new IniParser(cfg_file);
        // read config
        username = inifile.GetSettingString("Base", "pilot");
        portNum = inifile.GetSettingInteger("Serialport", "port");
        portName = inifile.GetSettingString("Serialport", "portname");
        baudrate = inifile.GetSettingInteger("Serialport", "baudrate");
        accel_gravity = inifile.GetSettingFloat("Physical", "accel_gravity");
        fluid_density = inifile.GetSettingInteger("Physical", "fluid_density");
        atm_pressure = inifile.GetSettingInteger("Physical", "atm_pressure");
        max_pressure = inifile.GetSettingInteger("Physical", "max_pressure");
        max_depth = inifile.GetSettingInteger("Physical", "max_depth");
        max_num_of_thrusters = inifile.GetSettingInteger("Motors", "max_num_of_thrusters");
        num_of_thrusters = inifile.GetSettingInteger("Motors", "num_of_thrusters");
        max_rpm = inifile.GetSettingInteger("Motors", "max_rpm");

        int len;
        int[] idx = inifile.GetSettingIntList("Motors", "motor_idx");
        len = idx.Length;

        for (int i = 0; i < len; ++i)
        {
            motorIdx[i] = idx[i];

        }
        // delete [] idx;
        max_voltage = inifile.GetSettingInteger("Electrical", "max_voltage");
        max_current = inifile.GetSettingInteger("Electrical", "max_current");
        // free pointer
        return true;
    }
}