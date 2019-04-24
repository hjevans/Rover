using System;
using System.Threading;
using Microsoft.SPOT;
using DriveStraightAuxiliary.Platform;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.Sensors;

namespace DriveStraightAuxiliary
{
    public class Program
    {
        public static void Main()
        {
            /* Disable drivetrain/motors */
            Hardware._rightFrontTalon.Set(ControlMode.PercentOutput, 0);
            //Hardware._leftVictor.Set(ControlMode.PercentOutput, 0);
            Hardware._leftFrontTalon.Set(ControlMode.PercentOutput, 0);
			Hardware._rightBackTalon.Set(ControlMode.PercentOutput, 0);
			Hardware._leftBackTalon.Set(ControlMode.PercentOutput, 0);

			/* Set Neutral Mode */
			Hardware._rightFrontTalon.SetNeutralMode(NeutralMode.Brake);
            //Hardware._leftVictor.SetNeutralMode(NeutralMode.Brake);
            Hardware._leftFrontTalon.SetNeutralMode(NeutralMode.Brake);
			Hardware._rightBackTalon.SetNeutralMode(NeutralMode.Brake);
			Hardware._leftBackTalon.SetNeutralMode(NeutralMode.Brake);

			/** Feedback Sensor Configuration [Remote Difference] */

			/* Configure the drivetrain's left side Feedback Sensor as a Quadrature Encoder */
			Hardware._leftFrontTalon.ConfigSelectedFeedbackSensor(   FeedbackDevice.QuadEncoder,     // Local Feedback Source
                                                                Constants.PID_PRIMARY,          // PID Slot for Source [0, 1]
                                                                Constants.kTimeoutMs);          // Configuration Timeout
			Hardware._leftBackTalon.ConfigSelectedFeedbackSensor(FeedbackDevice.QuadEncoder,     // Local Feedback Source
																Constants.PID_PRIMARY,          // PID Slot for Source [0, 1]
																Constants.kTimeoutMs);
			/* Configure the left Talon's Selected Sensor to be a remote sensor for the right Talon */
			Hardware._rightFrontTalon.ConfigRemoteFeedbackFilter(Hardware._leftFrontTalon.GetDeviceID(),                              // Device ID of Source
                                                            RemoteSensorSource.RemoteSensorSource_TalonSRX_SelectedSensor,  // Remote Feedback Source
                                                            Constants.REMOTE_0,                                             // Source number [0, 1]
                                                            Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigRemoteFeedbackFilter(Hardware._leftBackTalon.GetDeviceID(),                              // Device ID of Source
															RemoteSensorSource.RemoteSensorSource_TalonSRX_SelectedSensor,  // Remote Feedback Source
															Constants.REMOTE_0,                                             // Source number [0, 1]
															Constants.kTimeoutMs);  // Configuration Timeout
			/* Setup Difference signal to be used for Turn PID */
			Hardware._rightFrontTalon.ConfigSensorTerm(SensorTerm.SensorTerm_Diff1, FeedbackDevice.RemoteSensor0, Constants.kTimeoutMs);    // Feedback Device of Remote Talon
            Hardware._rightFrontTalon.ConfigSensorTerm(SensorTerm.SensorTerm_Diff0, FeedbackDevice.QuadEncoder, Constants.kTimeoutMs);// Quadrature Encoder of current Talon

			Hardware._rightBackTalon.ConfigSensorTerm(SensorTerm.SensorTerm_Diff1, FeedbackDevice.RemoteSensor0, Constants.kTimeoutMs);    // Feedback Device of Remote Talon
			Hardware._rightBackTalon.ConfigSensorTerm(SensorTerm.SensorTerm_Diff0, FeedbackDevice.QuadEncoder, Constants.kTimeoutMs);      // Quadrature Encoder of current Talon

			/* Configure Difference [Difference between both QuadEncoders] to be used for Auxiliary PID Index */
			Hardware._rightFrontTalon.ConfigSelectedFeedbackSensor(FeedbackDevice.SensorDifference, Constants.PID_TURN, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigSelectedFeedbackSensor(FeedbackDevice.SensorDifference, Constants.PID_TURN, Constants.kTimeoutMs);

			/* Scale the Feedback Sensor using a coefficient */
			Hardware._rightFrontTalon.ConfigSelectedFeedbackCoefficient(Constants.kTurnTravelUnitsPerRotation / Constants.kEncoderUnitsPerRotation,  // Coefficient
                                                                    Constants.PID_TURN,                                                         // PID Slot of Source
                                                                    Constants.kTimeoutMs);  // Configuration Timeout
			Hardware._rightBackTalon.ConfigSelectedFeedbackCoefficient(Constants.kTurnTravelUnitsPerRotation / Constants.kEncoderUnitsPerRotation,  // Coefficient
																	Constants.PID_TURN,                                                         // PID Slot of Source
																	Constants.kTimeoutMs);                                                      // Configuration Timeout


			/* Configure output and sensor direction */
			Hardware._rightFrontTalon.SetInverted(true);
			Hardware._rightBackTalon.SetInverted(true);
            //Hardware._leftFrontTalon.SetInverted(false);    // Output on victor
			Hardware._leftBackTalon.SetInverted(true);
            Hardware._rightFrontTalon.SetSensorPhase(false);
            Hardware._leftFrontTalon.SetSensorPhase(true);   // Talon only used for sensor

            /* Set status frame periods to ensure we don't have stale data */
            Hardware._rightFrontTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_12_Feedback1, 10, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_14_Turn_PIDF1, 10, Constants.kTimeoutMs);
			Hardware._rightBackTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_12_Feedback1, 10, Constants.kTimeoutMs);
			Hardware._rightBackTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_14_Turn_PIDF1, 10, Constants.kTimeoutMs);

			Hardware._leftFrontTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_2_Feedback0, 10, Constants.kTimeoutMs);
			Hardware._leftBackTalon.SetStatusFramePeriod(StatusFrameEnhanced.Status_2_Feedback0, 10, Constants.kTimeoutMs);

			/* Configure Neutral Deadband */
			Hardware._rightFrontTalon.ConfigNeutralDeadband(Constants.kNeutralDeadband, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigNeutralDeadband(Constants.kNeutralDeadband, Constants.kTimeoutMs);
			Hardware._leftFrontTalon.ConfigNeutralDeadband(Constants.kNeutralDeadband, Constants.kTimeoutMs);
			Hardware._leftBackTalon.ConfigNeutralDeadband(Constants.kNeutralDeadband, Constants.kTimeoutMs);

			/* Configure max peak output [Open and closed loop modes]
             * Can use configClosedLoopPeakOutput() for only closed Loop modes
             */
			Hardware._rightFrontTalon.ConfigPeakOutputForward(+1.0f, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.ConfigPeakOutputReverse(-1.0f, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigPeakOutputForward(+1.0f, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigPeakOutputReverse(-1.0f, Constants.kTimeoutMs);
			Hardware._leftFrontTalon.ConfigPeakOutputForward(+1.0f, Constants.kTimeoutMs);
            Hardware._leftFrontTalon.ConfigPeakOutputReverse(-1.0f, Constants.kTimeoutMs);
			Hardware._leftBackTalon.ConfigPeakOutputForward(+1.0f, Constants.kTimeoutMs);
			Hardware._leftBackTalon.ConfigPeakOutputReverse(-1.0f, Constants.kTimeoutMs);

			/* FPID Gains for turn closed loop */
			Hardware._rightFrontTalon.Config_kP(Constants.kSlot_Turning, Constants.kGains_Turning.kP, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.Config_kI(Constants.kSlot_Turning, Constants.kGains_Turning.kI, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.Config_kD(Constants.kSlot_Turning, Constants.kGains_Turning.kD, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.Config_kF(Constants.kSlot_Turning, Constants.kGains_Turning.kF, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.Config_IntegralZone(Constants.kSlot_Turning, (int)Constants.kGains_Turning.kIzone, Constants.kTimeoutMs);
            Hardware._rightFrontTalon.ConfigClosedLoopPeakOutput(Constants.kSlot_Turning, Constants.kGains_Turning.kPeakOutput, Constants.kTimeoutMs);
			Hardware._rightBackTalon.Config_kP(Constants.kSlot_Turning, Constants.kGains_Turning.kP, Constants.kTimeoutMs);
			Hardware._rightBackTalon.Config_kI(Constants.kSlot_Turning, Constants.kGains_Turning.kI, Constants.kTimeoutMs);
			Hardware._rightBackTalon.Config_kD(Constants.kSlot_Turning, Constants.kGains_Turning.kD, Constants.kTimeoutMs);
			Hardware._rightBackTalon.Config_kF(Constants.kSlot_Turning, Constants.kGains_Turning.kF, Constants.kTimeoutMs);
			Hardware._rightBackTalon.Config_IntegralZone(Constants.kSlot_Turning, (int)Constants.kGains_Turning.kIzone, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigClosedLoopPeakOutput(Constants.kSlot_Turning, Constants.kGains_Turning.kPeakOutput, Constants.kTimeoutMs);

			/* Allow an error of 0, always enforce closed loop */
			Hardware._rightFrontTalon.ConfigAllowableClosedloopError(Constants.kSlot_Turning, 0, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigAllowableClosedloopError(Constants.kSlot_Turning, 0, Constants.kTimeoutMs);

			/* 1ms per loop.  PID loop can be slowed down if need be. */
			int closedLoopTimeMs = 1;
            Hardware._rightFrontTalon.ConfigSetParameter(CTRE.Phoenix.LowLevel.ParamEnum.ePIDLoopPeriod, closedLoopTimeMs, 0x00, 0, Constants.kTimeoutMs);   // Primary
            Hardware._rightFrontTalon.ConfigSetParameter(CTRE.Phoenix.LowLevel.ParamEnum.ePIDLoopPeriod, closedLoopTimeMs, 0x00, 1, Constants.kTimeoutMs);   // Turn (Auxiliary)
			Hardware._rightBackTalon.ConfigSetParameter(CTRE.Phoenix.LowLevel.ParamEnum.ePIDLoopPeriod, closedLoopTimeMs, 0x00, 0, Constants.kTimeoutMs);   // Primary
			Hardware._rightBackTalon.ConfigSetParameter(CTRE.Phoenix.LowLevel.ParamEnum.ePIDLoopPeriod, closedLoopTimeMs, 0x00, 1, Constants.kTimeoutMs);   // Turn (Auxiliary)

			/* False means talon's local output is PID0 + PID1, and other side Talon is PID0 - PID1
             * True means talon's local output is PID0 - PID1, and other side Talon is PID0 + PID1
             */
			Hardware._rightFrontTalon.ConfigAuxPIDPolarity(false, Constants.kTimeoutMs);
			Hardware._rightBackTalon.ConfigAuxPIDPolarity(false, Constants.kTimeoutMs);
			/* Latched values to detect on-press events for buttons */
			bool[] _btns = new bool[Constants.kNumButtonsPlusOne];
            bool[] btns = new bool[Constants.kNumButtonsPlusOne];

            /* Initialize */
            bool _state = false;
            bool _firstCall = true;
            float _targetAngle = 0;

            ZeroSensors();

			while (true)
			{
				/* Enable motor controllers if gamepad connected */
				if (Hardware._gamepad.GetConnectionStatus() == CTRE.Phoenix.UsbDeviceConnection.Connected)
					CTRE.Phoenix.Watchdog.Feed();

				/* Gamepad value processing */
				float forward = -1 * Hardware._gamepad.GetAxis(1);
				float turn = 1 * Hardware._gamepad.GetAxis(2);
				CTRE.Phoenix.Util.Deadband(ref forward);
				CTRE.Phoenix.Util.Deadband(ref turn);

				/* Button processing */
				Hardware._gamepad.GetButtons(btns);
				if (btns[2] && !_btns[2])
				{
					_state = !_state;           // Toggle state
					_firstCall = true;          // State change, do first call operation
					_targetAngle = Hardware._rightFrontTalon.GetSelectedSensorPosition(1);
				}
				else if (btns[1] && !_btns[1])
				{
					ZeroSensors();              // Zero sensors
				}
				System.Array.Copy(btns, _btns, Constants.kNumButtonsPlusOne);

				if (!_state)
				{
					if (_firstCall)
						Debug.Print("This is basic Arcade Drive with Arbitrary Feed-forward.\n");

					/* Use Arbitrary FeedForward to create an Arcade Drive Control by modifying the forward output */
					Hardware._rightFrontTalon.Set(ControlMode.PercentOutput, forward, DemandType.ArbitraryFeedForward, -turn / 2);
					Hardware._leftFrontTalon.Set(ControlMode.PercentOutput, forward, DemandType.ArbitraryFeedForward, +turn / 2);
					Hardware._rightBackTalon.Set(ControlMode.PercentOutput, forward, DemandType.ArbitraryFeedForward, -turn / 2);
					Hardware._leftBackTalon.Set(ControlMode.PercentOutput, -forward, DemandType.ArbitraryFeedForward, +turn / 2);
				}
				
			//}
                else
                {
                    if (_firstCall)
                    {
                        Debug.Print("This is Drive Straight using the new Auxillary feature with QuadEncoders to maintain current heading.\n");

                        /* Determine which slot affects which PID */
                        Hardware._rightFrontTalon.SelectProfileSlot(Constants.kSlot_Turning, Constants.PID_TURN);
						Hardware._rightBackTalon.SelectProfileSlot(Constants.kSlot_Turning, Constants.PID_TURN);
					}

                    /* Configured for Percent Output with Auxiliary PID on right Talon's calculated difference between two QuadEncoders */
                    Hardware._rightFrontTalon.Set(ControlMode.PercentOutput, forward, DemandType.AuxPID, _targetAngle);
                    Hardware._leftFrontTalon.Follow(Hardware._rightFrontTalon, FollowerType.AuxOutput1);
					Hardware._rightBackTalon.Set(ControlMode.PercentOutput, forward, DemandType.AuxPID, _targetAngle);
					Hardware._leftBackTalon.Follow(Hardware._rightBackTalon, FollowerType.AuxOutput1);
				}
                _firstCall = false;

                Thread.Sleep(10);
            }
        }

        /** Zero all sensors used in Auxiliary Example */
        static void ZeroSensors()
        {
            Hardware._rightFrontTalon.GetSensorCollection().SetQuadraturePosition(0, Constants.kTimeoutMs);
            Hardware._leftFrontTalon.GetSensorCollection().SetQuadraturePosition(0, Constants.kTimeoutMs);
			Hardware._rightBackTalon.GetSensorCollection().SetQuadraturePosition(0, Constants.kTimeoutMs);
			Hardware._leftBackTalon.GetSensorCollection().SetQuadraturePosition(0, Constants.kTimeoutMs);
			//Hardware._pidgey.SetYaw(0, Constants.kTimeoutMs);
			//Hardware._pidgey.SetAccumZAngle(0, Constants.kTimeoutMs);
			Debug.Print("[Sensors] All sensors are zeroed.\n");
        }
    }
}
