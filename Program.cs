using System;
using System.Runtime.InteropServices;
using SNC;

namespace AndrewShawSNCPA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                //Car Blue 111.1 222.2 333.3 444.4 Toyota 1990 COMPACT REGULAR
                //Boat Black 11.1 22.2 33.3 44.4 Sail 55.5 Sailcrest
                Console.WriteLine("Please Specify the values you want to generate");

                Vehicle vehicle = new Vehicle();
                if (!vehicle.ConsolReadMembers()) { Console.WriteLine("INVALID VALUE INPUT, EXITING"); return; };

                List<WaypointLine> waypoints = GenerateWaypoints(vehicle);

                Console.WriteLine("Please Specify the file path you want as output");
                
                string filepath = Console.ReadLine();
                if(!File.Exists(filepath))
                {
                    File.Create(filepath);
                }

                while (!File.Exists(filepath)) { System.Threading.Thread.Sleep(10); }

                File.WriteAllText(filepath, assembleJNY(vehicle, waypoints));


                Console.WriteLine("Do you wish to exit now (Y/N)");
                if(Console.ReadLine() == "Y") { exit = true; }
            }
        }

        static string assembleJNY (Vehicle vehicle, List<WaypointLine> waypoints)
        {
            string output = "";

            output += vehicle.getVehicleString() + "\n";
            foreach(WaypointLine waypoint in waypoints)
            {
                output += waypoint.getWaypointString() + "\n";
            }
            return output;
        }

        static List<WaypointLine> GenerateWaypoints(Vehicle vehicle) 
        {
            Random rand = new Random();
            int numberOfWaypoints = 10 + rand.Next(21); // we want 10-30
            List<WaypointLine> waypoints = new List<WaypointLine>();
            int minTurns = 3;
            double currentBearing = rand.NextDouble()*360;
            double currentLat = 0;
            double currentLong = 0;
            int zone = 0;
            if (vehicle.Identifier.ToUpper() == "BOAT")
            {
                switch (rand.Next(4))
                {
                    case 0:
                        zone = 0;
                        //between 15.6 and 56.2
                        currentLat = 15.6 + (rand.NextDouble() * 40.6);
                        //between -49.8 and -23.1
                        currentLong = -49.8 + (rand.NextDouble() * 26.7);
                        break;
                    case 1:
                        zone = 1;
                        //between -48.8 and -6.9
                        currentLat = -48.8 + (rand.NextDouble() * 41.9);
                        //between -28.6 and 8.2
                        currentLong = -28.6 + (rand.NextDouble() * 36.8);
                        break;
                        break;
                    case 2:
                        zone = 2;
                        //between  -43.4 and 8.1
                        currentLat = -43.4 + (rand.NextDouble() * 51.5);
                        //between  -161.4 and -98.4  
                        currentLong = -161.4 + (rand.NextDouble() * 63.0);
                        break;
                        break;
                    case 3:
                        zone = 3;
                        //between -41.1 and -1.4, 
                        currentLat = -41.1 + (rand.NextDouble() * 39.7);
                        //between 62.2 and 94.5 
                        currentLong = 62.2 + (rand.NextDouble() * 32.3);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //between -90 and 90
                currentLat = (rand.NextDouble() * 180) - 90;
                //between 180 and -180
                currentLong = (rand.NextDouble() * 360) - 180;
            }
            double newBearing = 0;
            double newLat = 0;
            double newLong = 0;
            for (int i = 0; i < numberOfWaypoints; i++)
            {
                bool turn = false;
                if(minTurns > 0)
                {
                    if(rand.Next(numberOfWaypoints - i) <= minTurns) //randoming number of turns while forcing the minimum number to be met
                    {
                        turn = true;
                        minTurns--;
                    }
                }
                waypoints.Add(GenerateWaypoint(vehicle, zone, turn, rand, currentBearing, out newBearing, currentLat, out newLat, currentLong,out newLong));
            }
            return waypoints;
        }
        static WaypointLine GenerateWaypoint(Vehicle vehicle, int zone, bool turn, Random rand, double inputbearing, out double outputbearing, double inputLat, out double outputLat, double inputLong, out double outputLong)
        {
            WaypointLine waypoint = new WaypointLine();
            waypoint.turn = turn.ToString();
            outputbearing = 0;
            outputLat = 0;
            outputLong = 0;
            double duration = 0;

            bool validWaypoint = false;
            while (!validWaypoint)
            {
                double speed = RandSpeed(vehicle, rand);
                duration = 1.0 + rand.NextDouble() * 119.0; //no bounds mentioned for how long each waypoint must take, limiting it to between 1 minute and 120 minutes
                double bearing = RandBearing(vehicle, rand, inputbearing, turn);
                double distanceInFeet = (speed * 5280.0 * duration / 60.0);
                SNC.GeoCalc.GetEndingCoordinates(inputLat, inputLong, inputbearing, distanceInFeet, out outputLat, out outputLong, out outputbearing);
                if(vehicle.Identifier.ToUpper() == "BOAT")
                {
                    //if an invalid coordinate for a boat is generated it just regenerates a new waypoint
                    //Leaving it this way to prompt a discussion about taking the bearing and plotting out the intersection with the zone, which would be the obvious improvement
                    switch (zone)
                    {
                        case 0:
                            //between 15.6 and 56.2 
                            //between -49.8 and -23.1
                            if (outputLat >= 15.6 && outputLat <= 56.2 && outputLong >= -49.8 && outputLong <= -23.1)
                            {
                                validWaypoint = true;
                            }
                            break;
                        case 1:
                            //between -48.8 and -6.9
                            //between -28.6 and 8.2
                            if (outputLat >= -48.8 && outputLat <= -6.9 && outputLong >= -28.6 && outputLong <= 8.2)
                            {
                                validWaypoint = true;
                            }
                            break;
                        case 2:
                            //between  -43.4 and 8.1
                            //between  -161.4 and -98.4  
                            if (outputLat >= -43.4 && outputLat <= 8.1 && outputLong >= -161.4 && outputLong <= -98.4)
                            {
                                validWaypoint = true;
                            }
                            break;
                        case 3:
                            //between -41.1 and -1.4, 
                            //between 62.2 and 94.5 
                            if (outputLat >= -41.1 && outputLat <= -1.4 && outputLong >= 62.2 && outputLong <= 94.5)
                            {
                                validWaypoint = true;
                            }
                            break;
                        default:
                            validWaypoint = false;
                            break;
                    }
                }
                else
                {
                    validWaypoint = true;
                }
            }

            waypoint.Latitude = outputLat;
            waypoint.Longitude = outputLong;
            waypoint.DeltaTime = duration * 60.0; //in seconds, the example implies the total duration from beginning, but the description says "seconds elapsed since the last waypoint" 

            return waypoint;

        }
        static double RandSpeed(Vehicle vehicle, Random rand)
        {
            double speed = 0;
            if (vehicle.Identifier.ToUpper() == "CAR")
            {
                speed = 25 + rand.NextDouble()*35; //25-60mph
            }
            else if (vehicle.Identifier.ToUpper() == "BOAT")
            {
                switch (vehicle.Options.GetOption("POWER").ToUpper())
                {
                    case "MOTOR":
                        speed = 25 + rand.NextDouble()*35; //25-60mph
                        break;
                    case "SAIL":
                        speed = 15 + rand.NextDouble()*15; //15-30mph
                        break;
                    case "UNPOWERED":
                        speed = 1 + rand.NextDouble()*9; //1-10mph
                        break;
                }
            }
            return speed;
        }
        static double RandBearing(Vehicle vehicle, Random rand, double lastBearing, bool turn)
        {
            double newbearing = 0;
            if (vehicle.Identifier.ToUpper() == "CAR")
            {
                if(turn) 
                {
                    if (rand.Next(2) == 1)
                    {
                        newbearing = lastBearing + 90.0;
                    }
                    else
                    {
                        newbearing = lastBearing - 90.0;
                    }
                }
                else
                {
                    newbearing = lastBearing + (90.0 - (180.0 * rand.NextDouble())); //we want to adjust heading up to 90deg in either direction
                }
            }
            else if (vehicle.Identifier.ToUpper() == "BOAT")
            {
                if (turn)
                {
                    if (rand.Next(2) == 1)
                    {
                        newbearing = lastBearing + 30.0;
                    }
                    else
                    {
                        newbearing = lastBearing - 30.0;
                    }
                }
                else
                {
                    newbearing = lastBearing + (30.0 - (60.0 * rand.NextDouble())); //we want to adjust heading up to 90deg in either direction
                }
            }
            return newbearing;
        }
    }
}