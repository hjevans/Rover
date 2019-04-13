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

namespace HERO_Serial_Example4
{
    public class Program
    {
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
        /** 
         * Pop the oldest byte out of the ring buffer.
         * Caller must ensure there is at least one byte to pop out by checking _txCnt.
         * @return the oldest byte in buffer.
         */

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
            /* temporary array */
            byte[] scratch = new byte[1];
            byte[] toReadIn = new byte[10];
            /* open the UART, select the com port based on the desired gadgeteer port.
             *   This utilizes the CTRE.IO Library.
             *   The full listing of COM ports on HERO can be viewed in CTRE.IO
             *   
             */
            _uart = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 115200);
            //_uart = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 9600);
            _uart.Open();
            /* send a message to the terminal for the user to see */
            //_uart.Write(_helloMsg, 0, _helloMsg.Length);
            /* loop forever */
            CTRE.Phoenix.MotorControl.CAN.VictorSPX motor0 = new
            CTRE.Phoenix.MotorControl.CAN.VictorSPX(0);
            while (true)
            {
                //Debug.Print("In loop");
                //System.Threading.Thread.Sleep(10);
                /* read bytes out of uart */
                //if (_uart.BytesToRead > 0)
                //{
                //Debug.Print("In if");
                /*
                int readCnt = _uart.Read(_rx, 0, CalcRemainingCap());
                for (int i = 0; i < readCnt; ++i)
                {
                    Debug.Print("In for");
                    PushByte(_rx[i]);
                }
                */
                Debug.Print("reading uart");
                //_uart.Read(_rx, 0, 13);
                //Debug.Print("BYTES TO READ = "+_uart.BytesToRead.ToString());
                //System.Threading.Thread.Sleep(10);
                //int readIn =_uart.Read(_rx, 0, 10);
                /*
                 * This section is in fucntion getStrReadings now. This function is called below.
                 * 
                char[] readIn = Encoding.UTF8.GetChars(_rx);
                    string str = new string(readIn);
                    string[] readings = str.Split('-');
                */
                string[] stringReadings;
                //string[] strCopy;
                int[] intReadings;
                //if (_rx != null && _uart.BytesToRead > 0)
                //{
                    Debug.Print("getting initial string readings");
                    stringReadings = getStrReadings();
                    //int[] intReadings;

                    //if (stringReadings[0] == "E" && stringReadings[3] == ";")
                    //{
                    if (newData == true)
                    {
                        intReadings = getIntReadings(stringReadings);
                    
                    // intReadings = getIntReadings(stringReadings);
                    //}

                    //else
                    //{
                    //  strCopy = stringReadings;
                    //stringReadings[0] = strCopy[2][1].ToString();
                    //Debug.Print("New index 0 " + stringReadings[0]);
                    //stringReadings[1] = strCopy[3] + strCopy[0];
                    //Debug.Print("New index 1 " + stringReadings[1]);
                    //stringReadings[2] = strCopy[1];
                    //Debug.Print("New index 2 " + stringReadings[2]);
                    //stringReadings[3] = strCopy[2][0].ToString();
                    //Debug.Print("New index 3 " + stringReadings[3]);

                    //intReadings = getIntReadings(stringReadings);
                    //}


                    if (intReadings[0] < 90)
                    //while (true)
                    {
                        //set motor to run forward until specified angle
                        motor0.Set(CTRE.Phoenix.MotorControl.ControlMode.PercentOutput, .25);

                        //_uart.Read(_rx, 0, 13);
                        //Debug.Print("BYTEs TO READ = " + _uart.BytesToRead.ToString());
                        //System.Threading.Thread.Sleep(10);
                        //if (_rx != null)
                        //{
                          //  stringReadings = getStrReadings();
                        //}
                        //if (stringReadings[0] == "E" && stringReadings[3] == ";")
                        //{
                        //intReadings = getIntReadings(stringReadings);
                        //}
                        //else
                        //{
                        //{
                        //hard coded to most common error for now


                        //      strCopy = stringReadings;
                        //    stringReadings[0] = strCopy[2][1].ToString();
                        //  Debug.Print("New index 0 " + stringReadings[0]);
                        //stringReadings[1] = strCopy[3] + strCopy[0];
                        //Debug.Print("New index 1 " + stringReadings[1]);
                        //stringReadings[2] = strCopy[1];
                        //Debug.Print("New index 2 " + stringReadings[2]);
                        //stringReadings[3] = strCopy[2][0].ToString();
                        //Debug.Print("New index 3 " + stringReadings[3]);

                        //intReadings = getIntReadings(stringReadings);
                        //}

                        //}


                        Debug.Print("Motor running, goal not met");

                        CTRE.Phoenix.Watchdog.Feed();
                    }
                    Debug.Print("Motor stopped, goal met");
                //}

                    /* wait a bit, keep the main loop time constant, this way you can add to this example (motor control for example). */
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        /**
         * Helper routine for creating byte arrays from strings.
         * @param msg string message to covnert.
         * @return byte array version of string.
         */


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
            /*
            while (_rx == null)
            {
                Debug.Print("rx is null");
                _uart.Read(_rx, 0, 13);
                System.Threading.Thread.Sleep(10);
            }
            Debug.Print("converting to chars");
            char[] readIn = Encoding.UTF8.GetChars(_rx);
            while (readIn[0] != 'E' || readIn[readIn.Length-1] != ';'||readIn.Length<13)
            {
                System.Threading.Thread.Sleep(10);
                Debug.Print("reordering");
                //string str = new string(readIn);
                str = new string(readIn);
                Debug.Print(str);
                //for (int n = 0; )
                _uart.Read(_rx, 0, 13);
                readIn = Encoding.UTF8.GetChars(_rx);
            }
            Debug.Print("in order");
            //System.Threading.Thread.Sleep(10);
            //char[] readIn = Encoding.UTF8.GetChars(_rx);
            //if (readIn[0] != 'E' && readIn[12] != ';')
            //{

            //}
            /*
            _uart.Read(_rx, 0, 12);
            char[] readIn = Encoding.UTF8.GetChars(_rx);
            char[] ordered = new char[readIn.Length];
            char[] orderedNew = new char[readIn.Length];
            int itr = 0;
            string test4 = new string(readIn);
            string[] noInit = new string[1];
            noInit[0] = "discarded" ;
            Debug.Print("initial reading: " + test4);
            if (readIn[0] != 'E')
            {
                Debug.Print("out of order");
                ordered = readIn;
                while (ordered[itr] != 'E' && itr < 13)
                {
                    Debug.Print("iteration " + itr);
                    char temp = ordered[0];
                    int k = 0;
                    for (k = 0; k < readIn.Length-1; k++)
                    {
                        orderedNew[k] = ordered[k + 1];
                    }
                    //orderedNew[k] = temp;
                    itr++;

                }
                if (itr >= 12)
                {
                    Debug.Print("no E found");
                    return noInit;
                }
                string test3 = new string(orderedNew);
                Debug.Print("new order: "+test3);
            }
            */
            //char[] readIn = Encoding.UTF8.GetChars(_rx);


            //string str = new string(readIn);
            //if(receivedChars[0] == '-' && receivedChars[receivedChars.Length-6] == '-' && receivedChars[receivedChars.Length-1] == '-')
            //{
                str = new string(receivedChars);
                Debug.Print(str);
                string[] readings = str.Split('-');
            //}
            //str = new string(receivedChars);
            //Debug.Print(str);
            //string[] readings = str.Split('-');
            return readings;
        }    
            
        private static int[] getIntReadings(string[] stringArray)
        {
            // parsing operations to convert string readings to usable int values
            int[] readInt = new int[stringArray.Length];
            //Debug.Print(str);

            //else
            //{

            //}
            if (stringArray.Length > 1)
            {
                //Debug.Print(readings[0]);
                //Debug.Print(readings[1]);
                for (int i = 1; i < stringArray.Length - 1; i++)
                {
                    Debug.Print("Raw string " + stringArray[i]);
                    for (int l = 0; l < stringArray[i].Length; l++)
                    {
                        if (stringArray[i][l] == 'E')
                        {
                            Debug.Print("Found an E at " + stringArray[i]);
                            //return [9999];
                        }
                    }
                    readInt[i - 1] = int.Parse(stringArray[i]);
                    Debug.Print("Converted to int as " + readInt[i - 1].ToString());
                }
            }
            return readInt;
        }
    }
}

