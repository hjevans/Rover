
using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using RoverMaster.Platform;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.Sensors;

namespace RoverMaster
{
    public class Program
    {
        /** Serial object, this is constructed on the serial number. */
        static System.IO.Ports.SerialPort _uart;
        static bool newData;
        /** Cache for reading out bytes in serial driver. */
        static byte[] _rx = new byte[1024];

        public static void Main()
        {
            _uart = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 115200);
            _uart.Open();

            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor0 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(0);

            while (true)
            {   
                string[] stringReadings;
                int[] intReadings;

                Debug.Print("getting initial string readings");
                stringReadings = getStrReadings();
            
                if (newData == true)
                {
                    intReadings = getIntReadings(stringReadings);
                    if (intReadings[0] < 90)
                    {
                        //set motor to run forward until specified angle
                        Hardware._rightFrontShoulder.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, .25);
                        Debug.Print("Motor running, goal not met");
                        CTRE.Phoenix.Watchdog.Feed();
                    }
                    Debug.Print("Motor stopped, goal met");
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

      
        private static string[] getStrReadings()
        {
            int ndx = 0;
            char[] charsReadIn;
            char[] receivedChars = new char[1024];
            string str;
            bool recvInProgress = false;
            newData = false;

            while (_uart.BytesToRead > 0 && newData == false)
            {
                _uart.Read(_rx, 0, 1);
                charsReadIn = Encoding.UTF8.GetChars(_rx);
                if (recvInProgress == true)
                {
                    if (charsReadIn[0] != ';' && charsReadIn[0] != 'E')
                    {
                        receivedChars[ndx] = charsReadIn[0];
                        ndx++;
                        if (receivedChars[ndx - 1] == '-' && charsReadIn[0] == '-')
                        {
                            Debug.Print("two hyphens exception");
                            newData = false;
                        }
                        if (ndx >= 1023)
                        {
                            ndx = 1023 - 1;
                        }
                    }

                    else
                    {
                        if (charsReadIn[0] == 'E')
                        {
                            newData = false;
                        }
                        else
                        {
                            newData = true;
                        }
                        receivedChars[ndx] = '\0';
                        recvInProgress = false;
                        ndx = 0;
                    }
                }
                else if (charsReadIn[0] == 'E')
                {
                    recvInProgress = true;
                }
            }
            System.Threading.Thread.Sleep(10);
            
            str = new string(receivedChars);
            Debug.Print(str);
            string[] readings = str.Split('-');
            return readings;
        }

        private static int[] getIntReadings(string[] stringArray)
        {
            // parsing operations to convert string array readings to usable int values
            int[] readInt = new int[stringArray.Length];
            if (stringArray.Length > 1)
            {
                for (int i = 0; i < stringArray.Length; i++)
                {
                    Debug.Print("Raw string " + stringArray[i]);
                    readInt[i] = int.Parse(stringArray[i]);
                    Debug.Print("Converted to int as " + readInt[i].ToString());
                }
            }
            return readInt;
        }
    }
}
