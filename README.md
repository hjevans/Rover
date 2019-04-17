# Rover
Code for MTRE 4800 senior project Rover (quadruped-to-wheeled robot)

Hero Simple Example1: basic motor control example for up to 4 motors
Hero Serial Example4: HERO-side serial--receives potentiometer bytes, orders, converts to strings and ints to be used in motor control
HeroSoftwareSerial.ino: Arduino-side serial--sends potentiometer readings

DriveStraightAuxiliary[Quadrature]: sample code for PID drive straight/arcade drive toggle. Edited here for use with 4 talons

RoverMaster: Master C# project for the whole project.Right now, contains auxiliary C# .cs files with gains, hardware definitions from DriveStraightAuxiliary and simplified from serial example
