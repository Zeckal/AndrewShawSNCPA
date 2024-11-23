using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndrewShawSNCPA
{
    public class Vehicle
    {
        public string Identifier { get; set; }
        public string Descriptor { get; set; }
        public float Weight { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }
        public Options Options { get; set; }

        public bool ConsolReadMembers()
        {
            float parseOutput;
            Console.WriteLine("Please Input IDENTIFIER");
            Identifier = Console.ReadLine(); //must be CAR or BOAT
            if (Identifier.ToUpper() == "CAR")
            {
                Options = new CarOptions();
            }
            else if (Identifier.ToUpper() == "BOAT")
            {
                Options = new BoatOptions();
            }
            else
            { 
                return false; 
            }

            Console.WriteLine("Please Input DESCRIPTOR"); //any text accepted
            Descriptor = Console.ReadLine();

            Console.WriteLine("Please Input WEIGHT"); //int in pounds
            if( !float.TryParse(Console.ReadLine(),out parseOutput) ) { return false; }
            Weight = parseOutput;

            Console.WriteLine("Please Input WIDTH"); //int in pounds
            if (!float.TryParse(Console.ReadLine(), out parseOutput)) { return false; }
            Width = parseOutput;

            Console.WriteLine("Please Input HEIGHT"); //int in feet
            if (!float.TryParse(Console.ReadLine(), out parseOutput)) { return false; }
            Height = parseOutput;

            Console.WriteLine("Please Input LENGTH"); //int in feet
            if (!float.TryParse(Console.ReadLine(), out parseOutput)) { return false; }
            Length = parseOutput;

            return Options.ConsolReadOptions();
        }

        public string getVehicleString()
        {
            string output = "";
            output += Identifier + "," + Descriptor + "," + Weight.ToString() + "," + Width.ToString() + "," + Height.ToString() + "," + Length.ToString();
            output += Options.GetAllOptionsString();
            return output;
        }
    }
    public class Options
    {
        virtual public bool ConsolReadOptions()
        {
            //function to be overloaded by child classes
            throw new NotImplementedException("ConsolReadOptions was used on the empty parent option class");
        }
        virtual public string GetOption (string Name)
        {
            //function to be overloaded by child classes
            throw new NotImplementedException("GetOption was used on the empty parent option class");
        }
        virtual public bool SetOption(string Name, string Value)
        {
            //function to be overloaded by child classes
            throw new NotImplementedException("SetOption was used on the empty parent option class");
        }
        virtual public string GetAllOptionsString()
        {
            //function to be overloaded by child classes
            throw new NotImplementedException("GetAllOptionsString was used on the empty parent option class");
        }
    }

    public class CarOptions : Options
    {
        //Documentation says 6 but only documents 4 additional fields
        private string Manufacturer { get; set; }
        private string ModelYear { get; set; } //can be converted to INT
        private string BodyStyle { get; set; }
        private string Fuel { get; set; }

        public override string GetOption(string Name)
        {
            switch (Name.ToUpper())
            {
                case "MANUFACTURER":
                    return Manufacturer;
                case "MODELYEAR":
                    return ModelYear;
                    break;
                case "BODYSTYLE":
                    return BodyStyle;
                    break;
                case "FUEL":
                    return Fuel;
                    break;
                default:
                    return "";
            }
        }
        public override bool SetOption(string Name, string Value)
        {
            switch (Name.ToUpper())
            {
                case "MANUFACTURER":
                    Manufacturer = Value;
                    if (Value.Length <= 0) { return false; }
                    return true;
                case "MODELYEAR":
                    ModelYear = Value;
                    UInt32 parseOutput;
                    if (!UInt32.TryParse(ModelYear, out parseOutput)) { return false; }
                    return true;
                case "BODYSTYLE":
                    BodyStyle = Value;
                    //This list should be an ENUM if they were used anywhere else
                    if (Value.ToUpper() != "COMPACT" && 
                        Value.ToUpper() != "COUPE" && 
                        Value.ToUpper() != "SEDAN" && 
                        Value.ToUpper() != "SPORTS" && 
                        Value.ToUpper() != "CROSSOVER" && 
                        Value.ToUpper() != "SUV" && 
                        Value.ToUpper() != "MINIVAN" && 
                        Value.ToUpper() != "VAN" && 
                        Value.ToUpper() != "TRUCK" && 
                        Value.ToUpper() != "BUS" && 
                        Value.ToUpper() != "SEMI") 
                    { return false; }
                    return true;
                case "FUEL":
                    Fuel = Value;
                    //This list should be an ENUM if they were used anywhere else
                    if (Value.ToUpper() != "REGULAR" && Value.ToUpper() != "DIESEL" && Value.ToUpper() != "HYBRID" && Value.ToUpper() != "ELECTRIC") { return false; }
                    return true;
                default:
                    return false;
            }
        }
        public override string GetAllOptionsString()
        {
            //dynamic version could be done with a list of strings or tuple objects
            string output = "[";
            output += Manufacturer + ",";
            output += ModelYear + ",";
            output += BodyStyle + ",";
            output += Fuel;
            output += "]";
            return output;
        }

        public override bool ConsolReadOptions()
        {
            Console.WriteLine("Please Input MANUFACTURER"); //any text accepted
            if (!SetOption("MANUFACTURER", Console.ReadLine())) { return false; }
            Console.WriteLine("Please Input MODELYEAR"); //must be an int
            if (!SetOption("MODELYEAR", Console.ReadLine())) { return false; }
            Console.WriteLine("Please Input BODYSTYLE"); //set list of options
            if (!SetOption("BODYSTYLE", Console.ReadLine())) { return false; }
            Console.WriteLine("Please Input FUEL"); //set list of options
            if (!SetOption("FUEL", Console.ReadLine())) { return false; }
            return true;
        }
    }

    public class BoatOptions : Options
    {
        //Documentation says 6 but only documents 4 additional fields
        private string Power { get; set; }
        private string Draft { get; set; } //can be converted to INT
        private string Manufacturer { get; set; }

        public override string GetOption(string Name)
        {
            switch (Name.ToUpper())
            {
                case "POWER":
                    return Power;
                case "DRAFT":
                    return Draft;
                    break;
                case "MANUFACTURER":
                    return Manufacturer;
                    break;
                default:
                    return "";
            }
        }
        public override bool SetOption(string Name, string Value)
        {
            switch (Name.ToUpper())
            {
                case "POWER":
                    Power = Value;
                    //This list should be an ENUM if they were used anywhere else
                    if (Value.ToUpper() != "UNPOWERED" && Value.ToUpper() != "SAIL" && Value.ToUpper() != "MOTOR") { return false; }
                    return true;
                case "DRAFT":
                    Draft = Value;
                    float parseOutput;
                    if(!float.TryParse(Draft,out parseOutput)) { return false; }
                    if (Value.Length <= 0) { return false; }
                    return true;
                case "MANUFACTURER":
                    Manufacturer = Value;
                    if (Value.Length <= 0) { return false; }
                    return true;
                default:
                    return false;
            }
        }

        public override string GetAllOptionsString()
        {
            //dynamic version could be done with a list of strings or tuple objects
            string output = "[";
            output += Power + ",";
            output += Draft + ",";
            output += Manufacturer;
            output += "]";
            return output;
        }

        public override bool ConsolReadOptions()
        {
            Console.WriteLine("Please Input POWER"); //set list of options
            if (!SetOption("POWER", Console.ReadLine())) { return false; }
            Console.WriteLine("Please Input DRAFT"); //Float value
            if (!SetOption("DRAFT", Console.ReadLine())) { return false; }
            Console.WriteLine("Please Input MANUFACTURER"); //any text accepted
            if (!SetOption("MANUFACTURER", Console.ReadLine())) { return false; }
            return true;
        }
    }

    internal class WaypointLine
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }   
        public double DeltaTime { get; set; }
        public string turn { get; set; }
        public string getWaypointString()
        {
            string output = "";
            output += Latitude.ToString() + "," + Longitude.ToString() + "," + DeltaTime.ToString() + "," + turn;
            return output;
        }
    }
}
