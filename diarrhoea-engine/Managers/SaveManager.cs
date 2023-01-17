using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public static class SaveManager
    {
        public static string saveFile;
        public static void RemoveAttribute(string name)
        {
            string[] lines = File.ReadAllLines(saveFile);
            string result = "";

            Regex search = new Regex($"{name}: [a-zA-z 0-9 . > < ,]+");

            for (int i = 0; i < lines.Length; i++)
            {
                if(i != 0) result += "\n";

                if (search.IsMatch(lines[i])) continue;
                result += lines[i];
            }

            byte[] data = Encoding.UTF8.GetBytes(result);
            FileStream stream = File.OpenWrite(saveFile);

            stream.Position = 0;
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
        }

        public static void EditAttribute(string name, object value)
        {
            string[] lines = File.ReadAllLines(saveFile);
            string result = "";

            Regex search = new Regex($"{name}: [a-zA-z 0-9 . > < ,]+");

            for(int i = 0; i < lines.Length; i++) 
            {
                if(i != 0) result += "\n";

                if (search.IsMatch(lines[i])) result += name + ": " + value;
                else result += lines[i];
            }

            byte[] data = Encoding.UTF8.GetBytes(result);
            FileStream stream = File.OpenWrite(saveFile);

            stream.Position = 0;
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
        }
        
        public static void AddAttribute(string name, object value)
        {
            string[] lines = File.ReadAllLines(saveFile);
            Regex search = new Regex($"{name}: [a-zA-z 0-9 . > < ,]+");

            for (int i = 0; i < lines.Length; i++)
            {
                if (search.IsMatch(lines[i])) { throw new Exception("THIS ATRRIBUTE ALREADY EXISTS"); }
            }

            FileStream stream = File.OpenWrite(saveFile);
            stream.Position = stream.Length;
            string result = string.Empty;
            if (stream.Position != 0) result += "\n";
            result += $"{name}: {value}";

            byte[] data = Encoding.UTF8.GetBytes(result);
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
        } 

        public static string GetAttribute(string name)
        {
            string[] lines = File.ReadAllLines(saveFile);
            Regex search = new Regex($"{name}: [a-zA-z 0-9 . > < ,]+");

            for (int i = 0; i < lines.Length; i++)
            {
                if (search.IsMatch(lines[i]))
                {
                    return lines[i].Split($"{name}: ")[1];
                }
            }

            return string.Empty;
        }
    }
}
