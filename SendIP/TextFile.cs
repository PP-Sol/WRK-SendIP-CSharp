using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;

namespace SendIP
{
    public static class TextFile
    {

        //funkce pro zapis textoveho souboru - pokud se podari zapsat vraci true jinak false
        public static bool writeFile(string name, ArrayList Obsah)
        {
            bool vrat = true;
            
            try
            {
                using (StreamWriter sw = new StreamWriter(name, false, new UnicodeEncoding(false, true)))
                {

                    for (int i = 0; i < Obsah.Count; i++)
                    {
                        sw.WriteLine(Obsah[i]);
                    }
                }
            }
            catch (Exception e)
            {
                vrat = false;
                //MessageBox.Show(e.ToString(), "Sheet Generator 1.0", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return vrat;
        }

        //funkce pro cteni textoveho souboru
        public static ArrayList readFile(string name)
        {
            ArrayList arrText = new ArrayList();

            try
            {
                StreamReader objReader = new StreamReader(name, true);
                string sLine = "";
                while (sLine != null)
                {

                    sLine = objReader.ReadLine();
                    if (sLine != null) arrText.Add(sLine);
                }
                objReader.Close();
            }
            catch (Exception e)
            {
                arrText = null;
            }

            return arrText;
        }

        //funkce pro cteni textoveho souboru
        public static string readFileStr(string name)
        {
            ArrayList arrText = new ArrayList();
            string vrat ="";
            int i;

            try
            {
                StreamReader objReader = new StreamReader(name, true);
                string sLine = "";
                while (sLine != null)
                {

                    sLine = objReader.ReadLine();
                    if (sLine != null) arrText.Add(sLine);
                }
                objReader.Close();

                for (i = 0; i < arrText.Count; i++)
                {
                    vrat = vrat + arrText[i].ToString()+"\r\n";
                }
            }
            catch (Exception e)
            {
                vrat = null;
            }

            return vrat;
        }
    }
}
