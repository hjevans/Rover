/**
 * Example HERO application can reads a serial port and echos the bytes back.
 * After deploying this application, the user can open a serial terminal and type while the HERO echoes the typed keys back.
 * Use a USB to UART (TTL) cable like the Adafruit Raspberry PI or FTDI-TTL cable.
 * Use device manager to figure out which serial port number to select in your PC terminal program.
 * HERO Gadgeteer Port 1 is used in this example, but can be changed at the top of Main().
 */
using System;
using System.Threading;
using System.Text;
using System.Collections;
using Microsoft.SPOT;
using DriveStraightAuxiliary.Platform;

namespace HERO_Serial_Example4
{
    public class Program
    {
        static bool debug = false;
        static int SafetyDown = 0;
        static int OpDown = 1;
        static int Kneel = 2;
        static int OpUp = 3;
        static int SafetyUp = 4;

        static int RFElbow = 1;
        static int RFWrist = 2;
        static int RBElbow = 5;
        static int RBWrist = 6;
        static int LFElbow = 9;
        static int LFWrist = 10;
        static int LBElbow = 13;
        static int LBWrist = 14;

        static double wristPower = .35;
        static double elbowPower = .35;

        //static int[,] jointPositions = new int[16, 5];// { { 0,0,0,0,0 }, { 443, 439, 503, 513, 517 }, { 2727, 2735, 2755, 2773, 2780 }, { 0, 0, 0, 0, 0 },{ 0,0,0,0,0 }, { 2378, 2385, 2440, 2470, 2470 }, { 1393, 1386, 1351, 1337, 1333 },{ 0,0,0,0,0},{ 0,0,0,0,0 }, { 2625, 2635, 2699, 2713, 2716 }, { 746, 749, 770, 788, 795 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 925, 924, 841, 812, 809 }, { 1347, 1351, 1382, 1390, 1397 }, { 0, 0, 0, 0, 0 } };
        static int[][] jointPositions =
        {
            new int[] {0,0,0,0,0},  
            new int[] {450,470,517,519,520},
            new int[] {2727, 2747, 2758, 2755, 2780},
            new int[] {0,0,0,0,0},
            new int[] {0,0,0,0,0},
            new int[] {2427,2425,2424,2375,2360},//{2378,2385,2440,2470,2470},			
            new int[] {1330,1350,1351,1360,1400},//{1393,1386,1351,1337,1333}			
            new int[] {0,0,0,0,0},
            new int[] {0,0,0,0,0},
            new int[] {2625,2640,2707,2713,2716},
            new int[] {750,770,782,790,800},
            new int[] {0,0,0,0,0},
            new int[] {0,0,0,0,0},
            new int[] {850,855,857,919,939},//{925,914,841,812,809},			
            new int[] {1390,1389,1368,1351,1340},//{1347,1351,1382,1390,1397},			
            new int[] {0,0,0,0,0}
        };

        
        static int[] sensor = new int[16] { 99, 0, 1, 99, 99, 2, 3 ,99,99, 6, 7, 99, 99,4,5,99}; // sensor number in motor order number

        static CTRE.Phoenix.MotorControl.CAN.VictorSPX[] motors = new CTRE.Phoenix.MotorControl.CAN.VictorSPX[16] { null, new CTRE.Phoenix.MotorControl.CAN.VictorSPX(1), new CTRE.Phoenix.MotorControl.CAN.VictorSPX(2), null, null, new CTRE.Phoenix.MotorControl.CAN.VictorSPX(5), new CTRE.Phoenix.MotorControl.CAN.VictorSPX(6), null, null, new CTRE.Phoenix.MotorControl.CAN.VictorSPX(9), new CTRE.Phoenix.MotorControl.CAN.VictorSPX(10), null, null, new CTRE.Phoenix.MotorControl.CAN.VictorSPX(13), new CTRE.Phoenix.MotorControl.CAN.VictorSPX(14), null };
        static CTRE.Phoenix.MotorControl.CAN.TalonSRX[]  talons = new CTRE.Phoenix.MotorControl.CAN.TalonSRX[16] {  null, null, null, new CTRE.Phoenix.MotorControl.CAN.TalonSRX(3), null, null, null, new CTRE.Phoenix.MotorControl.CAN.TalonSRX(7), null, null, null, new CTRE.Phoenix.MotorControl.CAN.TalonSRX(11), null, null, null , new CTRE.Phoenix.MotorControl.CAN.TalonSRX(15) };


        /** Serial object, this is constructed on the serial number. */
        static System.IO.Ports.SerialPort _uart;
        /** Ring buffer holding the bytes to transmit. */
        static byte[] _tx = new byte[1024];
        static int _txIn = 0;
        static int _txOut = 0;
        static int _txCnt = 0;
        static bool newData;
        /** Cache for reading out bytes in serial driver. */
        static byte[] _rx = new byte[1024];
        /* initial message to send to the terminal */
        //static byte[] _helloMsg = MakeByteArrayFromString("HERO_Serial_Example3 - Start Typing and HERO will echo the letters back.\r\n");
        /** @return the maximum number of bytes we can read*/
        private static int CalcRemainingCap()
        {
            /* firs calc the remaining capacity in the ring buffer */
            int rem = _tx.Length - _txCnt;
            /* cap the return to the maximum capacity of the rx array */
            if (rem > _rx.Length)
                rem = _rx.Length;
            return rem;
        }
        /** @param received byte to push into ring buffer */
        private static void PushByte(byte datum)
        {
            _tx[_txIn] = datum;
            if (++_txIn >= _tx.Length)
                _txIn = 0;
            ++_txCnt;
        }

        private static byte PopByte()
        {
            byte retval = _tx[_txOut];
            if (++_txOut >= _tx.Length)
                _txOut = 0;
            --_txCnt;
            return retval;
        }
        /** entry point of the application */
        public static void Main()
        {
            for (int i = 0; i < 16; i++)
            { 
                if (motors[i] == null) { }
                else
                {
                    motors[i].SetNeutralMode(CTRE.Phoenix.MotorControl.NeutralMode.Brake);
                }
            }
            

            CTRE.Phoenix.Controller.GameController myGamepad = new
            CTRE.Phoenix.Controller.GameController(new CTRE.Phoenix.UsbHostDevice(0));
            /* temporary array */
            byte[] scratch = new byte[1];
            byte[] toReadIn = new byte[10];
            _uart = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 115200);
            //_uart = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 9600);
            _uart.Open();

            //synch flags
            bool lowering = false;
            bool raising = true;
            bool step0 = true;
            bool step1 = false;
            bool step2 = false;
            bool step3 = false;
            bool kneel0 = false;
            bool kneel1 = false;
            bool kneel2 = false;
            bool kneel3 = false;
            bool eGoalReached = false;
            bool wGoalReached = false;

            while (true)
            {
                string[] stringReadings;
                //string[] strCopy;
                int[] intReadings;
                //if (_rx != null && _uart.BytesToRead > 0)
                //{
                 if (debug) Debug.Print("getting initial string readings");
                stringReadings = getStrReadings();
                intReadings = getIntReadings(stringReadings);
                bool lastPress = false;
                bool lastPress1 = false;
                bool lastPress2 = false;
                bool pressed1;
                bool pressed2;
                bool pressed;
                bool RF_met = false;
                bool RF_met1 = false;
                bool RF_met2 = false;
                bool RF_met3 = false;
                //bool reachedLimit = false;

                //raise the legs
                /*
                if (raising && raise(0, intReadings))
                {
                    Debug.Print("*** Raised leg 0");
                    lowering = true;
                    raising = false;
                    
                }
                else
                {
                    Debug.Print("Elbow Goal: " + jointPositions[1][OpUp] + " -- currently: " + sensor[1]);
                    Debug.Print("Wrist Goal: " + jointPositions[2][OpUp] + " -- currently: " + sensor[2]);
                }

                if (lowering && lower(0, intReadings))
                {
                    Debug.Print("*** Lowered leg 0");
                    lowering = false;
                    raising = true;
                }
                else
                {
                    Debug.Print("LOWER Elbow Goal: " + jointPositions[1][OpUp] + " -- currently: " + sensor[1]);
                    Debug.Print("LOWER Wrist Goal: " + jointPositions[2][OpUp] + " -- currently: " + sensor[2]);
                }
                */
                 // comment the following line to enable step motion 
                //step0 = false;
                if(step0 && step(0, intReadings, ref raising, ref lowering, ref eGoalReached, ref wGoalReached)){
                    Debug.Print("Finished stepping");
                    raising = true;
                    lowering = false;
                    step0 = false;
                    step1 = true;
                }
                if (step1 && step(1, intReadings, ref raising, ref lowering, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished stepping");
                    raising = true;
                    lowering = false;
                    step1 = false;
                    step2 = true;
                }
                if (step2 && step(2, intReadings, ref raising, ref lowering, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished stepping");
                    raising = true;
                    lowering = false;
                    step2 = false;
                    step3 = true;
                }
                if (step3 && step(3, intReadings, ref raising, ref lowering, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished stepping");
                    raising = true;
                    lowering = false;
                    step3 = false;
                    step0 = false; // set true to repeat

                    kneel0 = true;
                }
                // comment the following to enable kneeling
                //kneel0 = false;
                //eGoalReached = false;
                //wGoalReached = false;
                if (kneel0 && kneel(0, intReadings, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished kneeling 0");
                    eGoalReached = false;
                    wGoalReached = false;
                    kneel0 = false;
                    kneel1 = true;
                }
                if (kneel1 && kneel(1, intReadings, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished kneeling 1");
                    eGoalReached = false;
                    wGoalReached = false;
                    kneel1 = false;
                    kneel2 = true;
                }
                if (kneel2 && kneel(2, intReadings, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished kneeling 2");
                    eGoalReached = false;
                    wGoalReached = false;
                    kneel2 = false;
                    kneel3 = true;
                }
                if (kneel3 && kneel(3, intReadings, ref eGoalReached, ref wGoalReached))
                {
                    Debug.Print("Finished kneeling 3");
                    eGoalReached = false;
                    wGoalReached = false;
                    kneel3 = false;
                    kneel0 = false; //set true to repeat
                }

                if (myGamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
                {
                    talons[3].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));
                    talons[7].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1));

                    talons[11].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(2));
                    talons[15].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(2));
                    Debug.Print(myGamepad.GetAxis(1).ToString());
                    Debug.Print(myGamepad.GetAxis(2).ToString());
                    CTRE.Phoenix.Watchdog.Feed();
                    /*
                    Debug.Print("gamepad connected");
                    pressed = myGamepad.GetButton(1);
                    pressed2 = myGamepad.GetButton(2);
                    if (pressed != lastPress || pressed == true)
                    {
                        Debug.Print("Button pressed, Kneeling selected");
                        //while(reachedLimit == false)

                        //{
                        stringReadings = getStrReadings();
                        intReadings = getIntReadings(stringReadings);
                        Debug.Print("potentiometer 2 is " + intReadings[2].ToString());
                        if (!RF_met1)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met1 = raise(1, intReadings,ref eGoalReached, ref wGoalReached);
                            Debug.Print("raising leg");
                            lastPress = pressed;
                        }
                        if (!RF_met2)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met2 = raise(2, intReadings, ref eGoalReached, ref wGoalReached);
                            Debug.Print("raising leg");
                            lastPress = pressed;
                        }
                        if (!RF_met)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met = raise(0, intReadings, ref eGoalReached, ref wGoalReached);
                            Debug.Print("raising leg");
                            lastPress = pressed;
                        }
                        if (!RF_met3)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met3 = raise(3, intReadings, ref eGoalReached, ref wGoalReached);
                            Debug.Print("raising leg");
                            lastPress = pressed;
                        }
                    }
                    //if (pressed2 == true)
                    //{
                        //motorRFT.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, myGamepad.GetAxis(1) / 2);
                        //Hardware._rightFrontTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*myGamepad.GetAxis(1), CTRE.Phoenix.MotorControl.DemandType.ArbitraryFeedForward, -1*myGamepad.GetAxis(2) / 2);
                        //Hardware._leftFrontTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*myGamepad.GetAxis(1), CTRE.Phoenix.MotorControl.DemandType.ArbitraryFeedForward, 1*myGamepad.GetAxis(2) / 2);
                        //Hardware._rightBackTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*myGamepad.GetAxis(1), CTRE.Phoenix.MotorControl.DemandType.ArbitraryFeedForward, -1*myGamepad.GetAxis(2) / 2);
                        //Hardware._leftBackTalon.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 1*myGamepad.GetAxis(1), CTRE.Phoenix.MotorControl.DemandType.ArbitraryFeedForward, 1*myGamepad.GetAxis(2) / 2);
                    //}
                    /*if (pressed1 != lastPress1 || pressed1 == true)
                    {
                        Debug.Print("Button pressed, Kneeling selected");
                        //while(reachedLimit == false)

                        //{
                        stringReadings = getStrReadings();
                        intReadings = getIntReadings(stringReadings);
                        Debug.Print("potentiometer 2 is " + intReadings[2].ToString());
                        if (!RF_met)
                        {
                            RF_met = raise(0, intReadings);
                            Debug.Print("raising leg");
                            lastPress1 = pressed1;
                        }
                        if (!RF_met3)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met3 = raise(3, intReadings);
                            Debug.Print("raising leg");
                            lastPress1 = pressed1;
                        }
                    }
                    if (pressed2 != lastPress2 || pressed2 == true)
                    {
                        Debug.Print("Button pressed, Kneeling selected");
                        //while(reachedLimit == false)

                        //{
                        stringReadings = getStrReadings();
                        intReadings = getIntReadings(stringReadings);
                        Debug.Print("potentiometer 2 is " + intReadings[2].ToString());
                        if (!RF_met1)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met1 = lower(1, intReadings);
                            Debug.Print("raising leg");
                            lastPress2 = pressed2;
                        }
                        if (!RF_met2)//&& !RF_met1 && !RF_met2 && !RF_met3)
                        {
                            RF_met2 = lower(2, intReadings);
                            Debug.Print("raising leg");
                            lastPress2 = pressed2;
                        }

                    }*/
                    System.Threading.Thread.Sleep(10);
                }
            }
        }
        // raise lower extend retract
        public static bool raise(int leg, int[] readings, ref bool eGoalReached, ref bool wGoalReached)
        {
            int elbow = leg * 4 + 1;
            int wrist = leg * 4 + 2;

            //bool eGoalReached = false;
            //bool wGoalReached = false;

            int eGoal = jointPositions[elbow][3];
            int wGoal = jointPositions[wrist][3];

            if (System.Math.Abs(readings[sensor[elbow]] - eGoal) <= 10)
            {
                eGoalReached = true;
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (System.Math.Abs(readings[sensor[wrist]] - wGoal) <= 10)
            {
                wGoalReached = true;
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (eGoalReached && wGoalReached)
            {
                return true;
            }
            // shift where limit numbers occur in array to keep forward==forward maybe necessary
            if (!eGoalReached && readings[sensor[elbow]] < eGoal){
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!eGoalReached && readings[sensor[elbow]] > eGoal){
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] < wGoal){
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, wristPower); // FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] > wGoal){
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*wristPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            return false;
        }
        // 770-855-7940 Mike Franklin
        public static bool lower(int leg, int[] readings, ref bool eGoalReached, ref bool wGoalReached)
        {
            int elbow = leg * 4 + 1;
            int wrist = leg * 4 + 2;

            //bool eGoalReached = false;
            //bool wGoalReached = false;

            int eGoal = jointPositions[elbow][1];
            int wGoal = jointPositions[wrist][1];

            if (System.Math.Abs(readings[sensor[elbow]] - eGoal) <= 10)
            {
                eGoalReached = true;
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (System.Math.Abs(readings[sensor[wrist]] - wGoal) <= 10)
            {
                wGoalReached = true;
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (eGoalReached && wGoalReached)
            {
                return true;
            }
            // shift where limit numbers occur in array to keep forward==forward maybe necessary
            if (!eGoalReached && readings[sensor[elbow]] < eGoal)
            {
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!eGoalReached && readings[sensor[elbow]] > eGoal)
            {
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] < wGoal)
            {
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, wristPower); // FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] > wGoal)
            {
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*wristPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            return false;
        }

        public static bool kneel(int leg, int[] readings, ref bool eGoalReached, ref bool wGoalReached)
        {
            int elbow = leg * 4 + 1;
            int wrist = leg * 4 + 2;

            //bool eGoalReached = false;
            //bool wGoalReached = false;

            int eGoal = jointPositions[elbow][2];
            int wGoal = jointPositions[wrist][2];

            if (System.Math.Abs(readings[sensor[elbow]] - eGoal) <= 5)
            {
                eGoalReached = true;
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (System.Math.Abs(readings[sensor[wrist]] - wGoal) <= 5)
            {
                wGoalReached = true;
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, 0);
            }
            if (eGoalReached && wGoalReached)
            {
                return true;
            }
            // shift where limit numbers occur in array to keep forward==forward maybe necessary
            if (!eGoalReached && readings[sensor[elbow]] < eGoal)
            {
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!eGoalReached && readings[sensor[elbow]] > eGoal)
            {
                motors[elbow].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*elbowPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] < wGoal)
            {
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, wristPower); // FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            if (!wGoalReached && readings[sensor[wrist]] > wGoal)
            {
                motors[wrist].Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, -1*wristPower); //FLIP THIS BACK!!!
                CTRE.Phoenix.Watchdog.Feed();
            }
            return false;
        }

        private static bool step(int leg,int[] readings, ref bool raising, ref bool lowering, ref bool eGoalReached, ref bool wGoalReached)
        {
            //raising = false;
            //lowering = true;
            int elbow = leg * 4 + 1;
            int wrist = leg * 4 + 2;
            if (raising && raise(leg, readings, ref eGoalReached, ref wGoalReached))
            {
                Debug.Print("*** Raised leg 0");
                lowering = true;
                raising = false;
                eGoalReached = false;
                wGoalReached = false;

            }
            else
            { 
                Debug.Print("Elbow Goal: " + jointPositions[elbow][OpUp] + " -- currently: " + sensor[elbow]);
                Debug.Print("Wrist Goal: " + jointPositions[wrist][OpUp] + " -- currently: " + sensor[wrist]);
            }

            if (lowering && lower(leg, readings, ref eGoalReached, ref wGoalReached))
            {
                Debug.Print("*** Lowered leg " + leg.ToString());
                lowering = false;
                raising = true;
                eGoalReached = false;
                wGoalReached = false;
                return true;
            }
            else
            {
                Debug.Print("LOWER Elbow Goal: " + jointPositions[elbow][OpUp] + " -- currently: " + sensor[elbow]);
                Debug.Print("LOWER Wrist Goal: " + jointPositions[wrist][OpUp] + " -- currently: " + sensor[wrist]);
            }
            return false;
        }

        private static string[] getStrReadings()
        {
            int ndx = 0;
            char[] charsReadIn;
            char[] receivedChars = new char[1024];
            string str;
            bool recvInProgress = false;
            newData = false;
            //_uart.Read(_rx, 0, 1);
            while(_uart.BytesToRead > 0 && newData == false)
            {
                _uart.Read(_rx, 0, 1);
                charsReadIn = Encoding.UTF8.GetChars(_rx);
                //receivedChars = new char[charsReadIn.Length];
                if (recvInProgress == true)
                {
                    if (charsReadIn[0] != ';' && charsReadIn[0] != 'E')
                    {
                        receivedChars[ndx] = charsReadIn[0];
                        ndx++;
                        if(receivedChars[ndx-1] == '-' && charsReadIn[0] == '-')
                        {
                            if (debug) Debug.Print("two hyphens exception"); 
                            //ndx = 0;
                            newData = false;
                        }
                        //if(receivedChars[ndx-1] == '-' && charsReadIn[0] == ';')
                        //{
                            //Debug.Print("left-out reading exception");
                            //newData = false;
                        //}
                        if (ndx >= 1023)
                        {
                            ndx = 1023 - 1;
                        }
                    }
                
                    else
                     {
                        //if (receivedChars[ndx] == '-' && charsReadIn[0] == ';')
                        //{
                          //  Debug.Print("left-out reading exception");
                            //newData = false;
                        //}
                        if (charsReadIn[0] == 'E')
                        {
                            newData = false;
                        }

                        else
                        {
                            newData = true;
                        }
                            
                        //}
                        receivedChars[ndx] = '\0';
                        recvInProgress = false;
                        ndx = 0;
                    }
                }
                else if (charsReadIn[0] == 'E')
                {
                    recvInProgress = true;
                }
                if (ndx < 41)
                {
                    newData = false;
                    //recvInProgress = false;
                }
            } 
            System.Threading.Thread.Sleep(10);
                str = new string(receivedChars);
                if (debug) Debug.Print(str);
                string[] readings = str.Split('-');
            return readings;
        }    
            
        private static int[] getIntReadings(string[] stringArray)
        {
            // parsing operations to convert string readings to usable int values
            int[] readInt = new int[8];
            //Debug.Print(str);

            //else
            //{

            //}
            if (stringArray.Length > 1)
            {
                //Debug.Print(readings[0]);
                //Debug.Print(readings[1]);
                //for(int i = 1; i < stringArray.Length -1; i++)
                for (int i = 0; i < readInt.Length; i++)
                {
                    if(debug) Debug.Print("Raw string " + stringArray[i]);
                    //for (int l = 0; l < stringArray[i].Length; l++)
                    //{
                      //  if (stringArray[i][l] == 'E')
                        //{
                          //  Debug.Print("Found an E at " + stringArray[i]);
                            //return [9999];
                        //}
                    //}
                    readInt[i] = int.Parse(stringArray[i+1]);
                    if(debug) Debug.Print("Converted to int as " + readInt[i].ToString());
                }
            }
            return readInt;
        }
    }
}

