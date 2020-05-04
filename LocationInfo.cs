using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public static class LocationInfo
    {
        public static List<int> locationIds = new List<int>();

        //Keeps track of locationId in the whole model to avoid duplications
        public static void AddLocationsIds(Template template)
        {
            for (int x = 0; x < template.Location.Count; x++)
            {
                locationIds.Add(GetNumberFromString(template.Location[x].Id));
            }
        }

        //Gets String and returns an integer number from the string
        private static int GetNumberFromString(string id)
        {
            return Int32.Parse(Regex.Match(id, @"\d+").Value);
        }

        //Returns new id to be assigned to new location
        public static string GetNewLocationId()
        {
            return ("id" + Convert.ToString(LocationInfo.locationIds.Count > 0 ? (LocationInfo.locationIds.Max() + 1) : 0));
        }
    }
    public static class LocationPoint
    {
        //Get source and target location and calculates the middle coordinates for X,Y with fraction 0.5 and returns new coordinates in a Tuple 
        public static Tuple<string, string> GetCoordinatesForLocation(Location sourceLoc, Location targetLoc, Transition sourceTransition)
        {
            
            int sourceX, sourceY;
            Nail nail;
            if (sourceTransition != null && sourceTransition.Nail != null && sourceTransition.Nail.Count > 0) // if true then take the center point between source location and first nail
            {
                nail = sourceTransition.Nail[sourceTransition.Nail.Count - 1];
                sourceX = Int32.Parse(nail.X);
                sourceY = Int32.Parse(nail.Y);
            }
            else // otherwise take the center point between source and target location
            {
                sourceX = Int32.Parse(sourceLoc.X);
                sourceY = Int32.Parse(sourceLoc.Y);
            }
            int targetX = Int32.Parse(targetLoc.X);
            int targetY = Int32.Parse(targetLoc.Y);

            //****** TODO : Remove ************************************************
            //richTextBox1.AppendText("Source Id : " + sourceLoc.Id + ", Target Id : " + targetLoc.Id + "\n");
            //richTextBox1.AppendText("Srouce X : " + sourceLoc.X + ", Target X : " + targetLoc.X + ", Center : " + X + "\n");
            //richTextBox1.AppendText("Srouce Y : " + sourceLoc.Y + ", Target Y : " + targetLoc.Y + ", Center : " + Y + "\n");
            //*********************************************************************

            return CalculateCenterPoint(sourceX, sourceY, targetX, targetY);
        }

        public static Tuple<string, string> CalculateCenterPoint(int sourceX, int sourceY, int targetX, int targetY) 
        {
            string X = string.Empty, Y = string.Empty;
            X = Convert.ToString(Math.Floor(sourceX + (0.5) * (targetX - sourceX)));
            Y = Convert.ToString(Math.Floor(sourceY + (0.5) * (targetY - sourceY)));
            return Tuple.Create(X, Y);
        }
    }
}
