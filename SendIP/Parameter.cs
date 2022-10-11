using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SendIP
{
    class Parameter
    {
        string name;
        string rootName;
        string familyName;
        string parVal;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string RootName
        {
            get
            {
                return rootName;
            }
            set
            {
                rootName = value;
            }
        }
        public string FamilyName
        {
            get
            {
                return familyName;
            }
            set
            {
                familyName = value;
            }
        }
        public string ParVal
        {
            get
            {
                return parVal;
            }
            set
            {
                parVal = value;
            }
        }
        public string FullName
        {
            get
            {
                string vrat = rootName + "." + familyName + "." + name;
                return vrat;
            }          
        }

        public Parameter()
        {
            name = "default";
            rootName = "default";
            familyName = "default";
            parVal = "default";      
        }

        public Parameter(string RootName, string FamilyName, string Name, string ParVal)
        {
            name = Name;
            rootName = RootName;
            familyName = FamilyName;
            parVal = ParVal;
        }


    }
}
