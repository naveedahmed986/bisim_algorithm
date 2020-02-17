using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bisimulation_Desktop
{
    public class NdLocation
    {
        List<Location> ndLocations = new List<Location>();

        public List<Location> getNdLocations()
        {
            return ndLocations;
        }

        public void AddNdLocation(Location location)
        {
            ndLocations.Add(location);
        }

    }

    public class NdTransition
    {
        List<Transition> ndTransitions = new List<Transition>();

        public List<Transition> getNdTransitions()
        {
            return ndTransitions;
        }

        public void AddNdTransition(Transition transition)
        {
            ndTransitions.Add(transition);
        }

    }

}
