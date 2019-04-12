using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Hero_Simple_Application1
{
    public class Program
    {
        //static AnalogInput potentiometer0 = new AnalogInput(CTRE.HERO.IO.Port1.Analog_Pin5);

        public static void Main()
        {
            //double analogRead0;
            /* simple counter to print and watch using the debugger */
            //int counter = 0;
            /* loop forever */

            //static System.IO.Ports.SerialPort _uart
               // System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 115200);

            CTRE.Phoenix.Controller.GameController myGamepad = new
            CTRE.Phoenix.Controller.GameController(new CTRE.Phoenix.UsbHostDevice(0));

            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor0 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(0);

            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor1 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(1);

            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor2 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(2);

            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor3 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(3);

            while (true)
            {
                /* print the three analog inputs as three columns */
                //analogRead0 = potentiometer0.Read();
                //Debug.Print("Motor 0 position: " + analogRead0);
                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                //if ((analogRead0 > 0)&&(analogRead0 < .400))
                {

                    Debug.Print("axis:" + myGamepad.GetAxis(1));
                    motor0.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));
                    //motor3.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 1);
                    //motor1.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput,.5);
                    //analogRead0 = potentiometer0.Read();
                    //Debug.Print("Motor 0 position: " + analogRead0);
                    CTRE.Phoenix.Watchdog.Feed();
                    //if ((analogRead0 > .570) && (analogRead0 < .575))
                      //  {
                        //motor1.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 1);
                    //}
                    //else
                    //{
                      //  motor1.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
                    //}
                    //CTRE.Phoenix.Watchdog.Feed();

                }
                //Debug.Print("Counter Value: " + counter);

                /* increment counter */
               // ++counter; /* try to land a breakpoint here and hover over 'counter' to see it's current value.  Or add it to the Watch Tab */

                /* wait a bit */
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
