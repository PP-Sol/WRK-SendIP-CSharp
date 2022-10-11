using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SendIP
{
    class Settings
    {
        List<Parameter> parameters;

        public List<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public Settings()
        {
            parameters = new List<Parameter>();
        }

        public int Add(string RootName, string FamilyName, string Name, string Val)
        {
            int vrat = -1;

            try
            {
                Parameter par = new Parameter(RootName, FamilyName, Name, Val);
                parameters.Add(par);
            }
            catch (Exception e) { }

            return vrat;
        }

        public string getParByName(string fullName)
        {
            string vrat = " ";
            int i;

            for (i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].FullName == fullName)
                {
                    vrat = parameters[i].ParVal;
                    break;
                }
            }

            return vrat;
        }

        public bool IsParByName(string fullName)
        {
            bool vrat = false;
            int i;

            for (i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].FullName == fullName)
                {
                    vrat = true;
                    break;
                }
            }

            return vrat;
        }

        public static string replaceTextRgx(string source, string patt, string replacement)
        {
            string vrat;
            int i;
            Regex rgx = new Regex(patt);

            vrat = rgx.Replace(source, replacement);

            return vrat;
        }

        public static string getTextRgx(string source, string patt)
        {
            string vrat;
            Regex rgx = new Regex(patt);
            Match mtch1 = rgx.Match(source);
            vrat = mtch1.Value;

            return vrat;
        }

        public static Parameter Parse(string str)
        {
            Parameter pom = new Parameter() ;

            try
            {
                if ((str != null) && (str != " "))
                {
                    string patt1 = @"[\s]*[\[]{1}[\s]*[\w]*[\.]+";
                    string repl1 = @"[\s]*[\[]{1}[\s]*";
                    string sub1 = getTextRgx(@str, patt1);
                    sub1 = replaceTextRgx(sub1, repl1, "");
                    sub1 = sub1.Replace(".", "");


                    string patt2 = @"[.]+[\w]*[\.]+";
                    string repl2 = @"";
                    string sub2 = getTextRgx(@str, patt2);
                    sub2 = sub2.Replace(".", "");


                    string patt3 = @"[.]+[\w]*[\s]*[\]]+";
                    string repl3 = @"[\s]*[\]]+[\s]*";
                    string sub3 = getTextRgx(@str, patt3);
                    sub3 = sub3.Replace(".", "");
                    sub3 = replaceTextRgx(sub3, repl3, "");

                    string patt4 = "[=]+[\\s]*[\"]+[\\s]*[^\\s]*[\\s]*[\"]+";
                    string repl4 = @"[=]+[\s]*";
                    string sub4 = getTextRgx(@str, patt4);
                    sub4 = replaceTextRgx(sub4, repl4, "");
                    sub4 = sub4.Replace("\"", "");

                    pom = new Parameter(sub1, sub2, sub3, sub4);
                }
                else
                {
                    pom = null;
                }
            }
            catch (Exception e) 
            {
                pom = null;
            }
            
            return pom;
        }

        public int UpdatePar(string fullName, string par)
        {
            int vrat = -1;
            int i;

            try
            {
                for (i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].FullName == fullName)
                    {
                        parameters[i].ParVal = par;
                        vrat = 1;
                        break;
                    }
                }
            }
            catch (Exception e) { }

            return vrat;
        
        }

        public int Update(string path)
        {
            int vrat = -1;
            int i;
            Parameter tempPar = new Parameter();

            try
            {
                ArrayList settFile = TextFile.readFile(path);

                if (settFile != null)
                {
                    for (i = 0; i < settFile.Count; i++)
                    {
                        tempPar = Parse(settFile[i].ToString());

                        if (tempPar != null)
                        {
                            if (IsParByName(tempPar.FullName))
                            {
                                UpdatePar(tempPar.FullName, tempPar.ParVal);
                            }
                            else
                            {
                                Add(tempPar.RootName, tempPar.FamilyName, tempPar.Name, tempPar.ParVal);
                            }
                        
                        }
                    
                    }
                    vrat = 1;
                }
            }
            catch (Exception e) { }

            return vrat;
        }

    }
}
