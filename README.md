# Rover
Code for MTRE 4800 senior project Rover (quadruped-to-wheeled robot)

Hero Simple Example1: basic motor control example for up to 4 motors

Hero Serial Example4: HERO-side serial--receives potentiometer bytes, orders, converts to strings and ints to be used in motor control,controls the motors for walking-motion, kneeling, and teleoperated arcade drive

HeroSoftwareSerial.ino: Arduino-side serial--sends potentiometer readings

DriveStraightAuxiliary[Quadrature]: sample code for PID drive straight/arcade drive toggle. Edited here for use with 4 talons

RoverMaster: Master C# project is now Hero Serial Example4. There is nothing of much use in RoverMaster. Use Hero Serial Example4 instead.
