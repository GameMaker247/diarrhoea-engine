using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiarrhoeaEngine
{
    public class Map
    {
        public uint[,] tiles = null;
        public string[] textures = null;
        public uint SCALE = 1;

        public Map(string src)
        {
            string file = File.ReadAllText(src);
            Console.WriteLine(file);
            Regex tileList = new Regex("set [0-9]+ \"[a-zA-Z.-/]+[.png]\"");
            Regex layout = new Regex("get [0-9,]+");
            Regex scale = new Regex("size [0-9]+,[0-9]+,[0-9]+");

            uint WIDTH = uint.Parse(scale.Match(file).Value.Split("size ")[1].Split(',')[0]);
            uint HEIGHT = uint.Parse(scale.Match(file).Value.Split("size ")[1].Split(',')[1]);
            SCALE = uint.Parse(scale.Match(file).Value.Split("size ")[1].Split(',')[2]);

            textures = new string[tileList.Matches(file).Count];

            foreach (Match m in tileList.Matches(file))
            {
                uint num = uint.Parse(m.Value.Split("set ")[1].Split(' ')[0]);
                string texture = m.Value.Split(' ')[2].Split('"')[1].Split('"')[0];

                ShaderManager.CreateTexture(texture);
                textures[num] = texture;
            }
            
            MatchCollection layoutCollection = layout.Matches(file);
            tiles = new uint[WIDTH, HEIGHT];

            for (int i = 0; i < layoutCollection.Count; i++)
            {
                string[] _tiles = layoutCollection[i].Value.Split("get ")[1].Split(',');
                for(int x = 0; x < _tiles.Length; x++)
                {
                    tiles[i, x] = uint.Parse(_tiles[x]);
                }
            }
        }
    }
}
