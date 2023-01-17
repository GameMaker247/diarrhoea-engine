using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;

using System.Drawing;

using DiarrhoeaEngine.ShitNet;
using System.Reflection.Metadata.Ecma335;
using System.Reflection;

namespace DiarrhoeaEngine
{
    public static class Program
    {
        public static Camera camera;
        public static Entity player;

        public static ControllerManager controls { get; private set; }
        public static GL GL;

        public static IWindow window;
        public static Vector2D<int> GetWindowSize()
        {
            return window.Size;
        }

        public static Matrix4X4<float> _view;
        public static Matrix4X4<float> _projection;
        private static Random rand = new Random();

        public static Renderer obj;
        public static Renderer example;
        public static Renderer player_renderer;

        public static Network network;
        public static bool isServer = false;
        public static bool connected = false;

        //public static event EventHandler<Entity> OnSpawnEntity;
        //OnSpawnEntity(null, new Entity(ID, ref stall, Position: pos, Rotation: new Vector3D<float>(0.0f, 0.0f, 0.0f), scale: 1.0f));
        //private static void SpawnEntity(object sender, Entity entity)
        //{
        //  WorldManager.SpawnEntity(entity);
        //}
        //OnSpawnEntity += new EventHandler<Entity>(SpawnEntity);

        private static unsafe void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default;

            options.Title = "Diarrhoea Engine -- V 0.0.1";
            options.Size = new Vector2D<int>(1080, 720);
            options.FramesPerSecond = 60;
            //options.WindowState = WindowState.Fullscreen;
            options.VSync= true;
            
            window = Window.Create(options);
           
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
         
            window.Run();
        }



        private static void OnLoad()
        {
            controls = new ControllerManager(window.CreateInput());
            //camera = new RTSCamera(GetWindowSize().X / 32, GetWindowSize().Y / 32, Vector3D<float>.Zero);
            camera = new FPSCamera(80.0f, GetWindowSize().X / GetWindowSize().Y, new Vector3D<float>(0.0f, 4.0f, 0.0f));

            GL = window.CreateOpenGL();

            GL.ClearColor(Color.Aqua);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthClamp);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            _view = camera.GetViewMatrix();
            _projection = camera.GetProjectionMatrix();

            WorldManager.LoadMap("../../../Maps/test.map");

            obj = new Renderer(Model.LoadNewModel("../../../Models/house.obj"), shader: "default", new string[] { "../../../Models/house.png" });
            Renderer stall = new Renderer(Model.LoadNewModel("../../../Models/stall.obj"), shader: "default", new string[] { "../../../Models/stallTexture.png" });
            example = new Renderer(Model.Square, shader: "fade", textures: new string[] { "../../../Images/retard.png", "../../../Images/bean.png" });
            player_renderer = new Renderer(Model.Square, textures: new string[] { "../../../Images/bean.png" });

            List<Renderer> tiles = new List<Renderer>();
            for(int i = 0; i < WorldManager.loaded.textures.Length; i++)
            {
                tiles.Add(new Renderer(Model.Square, textures: new string[] { WorldManager.loaded.textures[i] }));
            }

            for(int x = 0; x < WorldManager.loaded.tiles.GetLength(0); x++)
            {
                for(int z = 0; z < WorldManager.loaded.tiles.GetLength(1); z++)
                {
                    int index = (int)WorldManager.loaded.tiles[x,z];
                    Renderer tile = tiles[index];
                    WorldManager.SpawnEntity(new Entity("WorldTile", ref tile, Position: new Vector3D<float>(x * WorldManager.loaded.SCALE, 0, z * WorldManager.loaded.SCALE), Rotation: new Vector3D<float>(90.0f, 0.0f, 0.0f), scale: WorldManager.loaded.SCALE));
                }
            }

            Entity retard = WorldManager.SpawnEntity(new Entity("Retard", ref stall));
            Entity nigg = WorldManager.SpawnEntity(new Entity("Retard", ref obj, Position: new Vector3D<float>(0.0f, 10.0f, 0.0f)));
            Entity bean = WorldManager.SpawnEntity(new Entity("Mr. Bean", ref player_renderer, Position: new Vector3D<float>(0.0f, 12.0f, 36.0f), Rotation: new Vector3D<float>(-25.0f, 45.0f, 0.0f), scale: 25.0f));
            //Entity player = WorldManager.SpawnEntity(new Entity("Player", ref player_renderer, Position: new Vector3D<float>(0.0f, -20.0f, 0.0f), Rotation: new Vector3D<float>(90.0f, 0.0f, 0.0f), scale: 25.0f));

            bean.Target = new Vector3D<float>(10.0f, 24.0f, 0.0f);

            controls.onKeyPressed += (Key key) =>
            {/*
                switch(key)
                {
                    case Key.W:
                        {
                            player.Position += Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * 6.0f / (float)Program.window.FramesPerSecond : Program.camera.Up * 6.0f / (float)Program.window.FramesPerSecond;
                        }
                        break;
                    case Key.S:
                        {
                            player.Position -= Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * 6.0f / (float)Program.window.FramesPerSecond : Program.camera.Up * 6.0f / (float)Program.window.FramesPerSecond;
                        }
                        break;
                    case Key.D: 
                        { 
                            player.Position += Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * 6.0f / (float)Program.window.FramesPerSecond;
                        } 
                        break;
                    case Key.A: 
                        { 
                            player.Position -= Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * 6.0f / (float)Program.window.FramesPerSecond;
                        }
                        break;
                }*/
            };

            ShitNetCore.onReceiveMSG += async (NetMSGType type, string content) =>
            {
                if (network == null) return;

                switch (type)
                {
                    case NetMSGType.MOVE:
                        {
                            if (isServer)
                            {
                                Server server = (Server)network;

                                string POS = ShitNetCore.GetContentValue(content, "POS");
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                Console.WriteLine(POS);

                                float X = float.Parse(POS.Split('<')[1].Split(',')[0]);
                                float Y = float.Parse(POS.Split(',')[1]);
                                float Z = float.Parse(POS.Split(',')[2].Split('>')[0]);

                                Vector3D<float> pos = new Vector3D<float>(X, Y, Z);

                                if (WorldManager.FindEntity(ID) == null)
                                {
                                    WorldManager.SpawnEntity(new Entity(ID, ref stall, Position: pos, Rotation: new Vector3D<float>(0.0f, 0.0f, 0.0f), scale: 1.0f));
                                }
                                else
                                {
                                    Entity shit = WorldManager.FindEntity(ID);
                                    shit.Target = pos;
                                }
                                
                                await server.SendToAllBut($"MOVE(POS: {pos} | ID: {ID})", server.GetNetID(ID, GetNetIDType.ID));
                            }
                            else
                            {
                                string POS = ShitNetCore.GetContentValue(content, "POS");
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                float X = float.Parse(POS.Split('<')[1].Split(',')[0]);
                                float Y = float.Parse(POS.Split(',')[1]);
                                float Z = float.Parse(POS.Split(',')[2].Split('>')[0]);

                                Vector3D<float> pos = new Vector3D<float>(X, Y, Z);

                                if (WorldManager.FindEntity(ID) == null)
                                {
                                    WorldManager.SpawnEntity(new Entity(ID, ref stall, Position: pos, Rotation: new Vector3D<float>(0.0f, 0.0f, 0.0f), scale: 1.0f));
                                }
                                else
                                {
                                    Entity shit = WorldManager.FindEntity(ID);
                                    shit.Target = pos;
                                }
                            }
                        }
                        break;
                    case NetMSGType.SPAWN:
                        {
                            if (isServer)
                            {
                                Server server = (Server)network;

                                string POS = ShitNetCore.GetContentValue(content, "POS");
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                float X = float.Parse(POS.Split('<')[1].Split(',')[0]);
                                float Y = float.Parse(POS.Split(',')[1]);
                                float Z = float.Parse(POS.Split(',')[2].Split('>')[0]);

                                Vector3D<float> pos = new Vector3D<float>(X, Y, Z);

                                
                                WorldManager.SpawnEntity(new Entity(ID, ref stall, Position: pos, Rotation: new Vector3D<float>(0.0f, 0.0f, 0.0f), scale: 1.0f));

                                await server.SendTo($"SPAWN(POS: {camera.Position} | ID: SERVER)", server.GetNetID(ID, GetNetIDType.ID));
                                await server.SendTo($"SET_FLOAT(NAME: loop, VALUE: {loop})", server.GetNetID(ID, GetNetIDType.ID));
                                await server.SendToAllBut($"SPAWN(POS: {pos} | ID: {ID})", server.GetNetID(ID, GetNetIDType.ID));
                                server.clients.ForEach(async x =>
                                {
                                    Console.WriteLine(x.id);
                                    if (x.id != ID)
                                    {
                                        Console.WriteLine("Continue");
                                        Console.WriteLine(WorldManager.FindEntity(x.id));

                                        if(WorldManager.FindEntity(x.id) != null)
                                        {
                                            Console.WriteLine("IT EXISTST");

                                            await server.SendTo($"SPAWN(POS: {WorldManager.FindEntity(x.id).Position} | ID: {x.id})", server.GetNetID(ID, GetNetIDType.ID));
                                        }
                                    }
                                });
                            }
                            else
                            {
                                string POS = ShitNetCore.GetContentValue(content, "POS");
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                Console.WriteLine(POS);

                                float X = float.Parse(POS.Split('<')[1].Split(',')[0]);
                                float Y = float.Parse(POS.Split(',')[1]);
                                float Z = float.Parse(POS.Split(',')[2].Split('>')[0]);

                                Vector3D<float> pos = new Vector3D<float>(X, Y, Z);

                                WorldManager.SpawnEntity(new Entity(ID, ref stall, Position: pos, Rotation: new Vector3D<float>(0.0f, 0.0f, 0.0f), scale: 1.0f));
                            }
                        }
                        break;
                    case NetMSGType.ROTATE:
                        {
                            if (isServer)
                            {
                                Server server = (Server)network;

                                float PITCH = float.Parse(ShitNetCore.GetContentValue(content, "PITCH"));
                                float YAW = float.Parse(ShitNetCore.GetContentValue(content, "YAW"));
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                WorldManager.FindEntity(ID).Rotation = new Vector3D<float>(PITCH, YAW, 0.0f);
                                await server.SendToAllBut($"ROTATE(PITCH: {PITCH} | YAW: {YAW} | ID: {ID})", server.GetNetID(ID, GetNetIDType.ID));
                            }
                            else
                            {
                                float PITCH = float.Parse(ShitNetCore.GetContentValue(content, "PITCH"));
                                float YAW = float.Parse(ShitNetCore.GetContentValue(content, "YAW"));
                                string ID = ShitNetCore.GetContentValue(content, "ID");

                                WorldManager.FindEntity(ID).Rotation = new Vector3D<float>(PITCH, YAW, 0.0f);
                            }
                        }
                        break;
                    case NetMSGType.RECENTER:
                        {

                        }
                        break;
                    case NetMSGType.TXT:
                        {
                            if (isServer)
                            {
                                Server server = (Server)network;

                                string MSG = ShitNetCore.GetContentValue(content, "MSG");
                                NetID FROM = server.GetNetID(ShitNetCore.GetContentValue(content, "FROM"), GetNetIDType.ID);

                                Console.WriteLine($"USERNAME FOR MSG: {FROM.username}");

                                string _from = FROM.username == string.Empty ? FROM.id : FROM.username;
                                await server.SendToAllBut($"TXT(MSG: '{MSG}' | FROM: {_from})", FROM);

                                Console.WriteLine($"(SERVER) --- {_from}: {MSG}");
                            }
                            else
                            {
                                Client client = (Client)network;

                                string MSG = ShitNetCore.GetContentValue(content, "MSG");
                                string FROM = ShitNetCore.GetContentValue(content, "FROM");

                                Console.WriteLine($"(CLIENT) --- {FROM}: {MSG}");
                            }
                        }
                        break;
                    case NetMSGType.SET_ID:
                        {
                            Client client = (Client)network;
                            string ID = ShitNetCore.GetContentValue(content, "ID");
                            Console.WriteLine($"THE SERVER WANTS MY ID AS: {ID}");
                            client.id = new NetID(ID);
                            network = client;
                        }
                        break;
                    case NetMSGType.SET_USERNAME:
                        {
                            Server server = (Server)network;

                            string ID = ShitNetCore.GetContentValue(content, "ID");
                            string Username = ShitNetCore.GetContentValue(content, "USERNAME");

                            Console.WriteLine($"ID IS: {ID}");
                            Console.WriteLine($"USERNAME IS {Username}");

                            server.GetNetID(ID, GetNetIDType.ID).username = Username;
                        }
                        break;
                    case NetMSGType.SET_FLOAT:
                        {
                            string NAME = ShitNetCore.GetContentValue(content, "NAME");
                            float VALUE = float.Parse(ShitNetCore.GetContentValue(content, "VALUE"));

                            loop = VALUE;
                        }
                        break;
                }
            };

            controls.onKeyClicked += (Key x) =>
            {
                switch (x)
                {
                    case Key.H:
                        {
                            network = ShitNetCore.CreateServer(4444, ServerTester);
                            isServer = true;
                            connected= true;
                        }
                        break;
                    case Key.J:
                        {
                            Console.Write("Connect to: ");
                            string ip = Console.ReadLine();

                            network = ShitNetCore.CreateClient(ip, 4444, ClientTester);
                            connected = true;
                        }
                        break;
                }
            };

            controls.onKeyPressed += async (Key x) =>
            {
                switch (x)
                {
                    case Key.W:
                        {
                            camera.Movement(new Vector2D<float>(0.0f, 1.0f));/*
                            if(connected)
                            {
                                if(isServer)
                                {
                                    Server server = network as Server;
                                    await server.SendToAll($"MOVE(POS: {camera.Position} | ID: SERVER)");
                                }
                                else
                                {
                                    Client client = network as Client;
                                    await client.Send($"MOVE(POS: {camera.Position} | ID: {client.id.id})");
                                }
                            }*/
                        };
                        break;
                    case Key.A:
                        {
                            camera.Movement(new Vector2D<float>(-1.0f, 0.0f));/*
                            if (connected)
                            {
                                if (isServer)
                                {
                                    Server server = network as Server;
                                    await server.SendToAll($"MOVE(POS: {camera.Position} | ID: SERVER)");
                                }
                                else
                                {
                                    Client client = network as Client;
                                    await client.Send($"MOVE(POS: {camera.Position} | ID: {client.id.id})");
                                }
                            }*/
                        };
                        break;
                    case Key.S:
                        {
                            camera.Movement(new Vector2D<float>(0.0f, -1.0f));/*
                            if (connected)
                            {
                                if (isServer)
                                {
                                    Server server = network as Server;
                                    await server.SendToAll($"MOVE(POS: {camera.Position} | ID: SERVER)");
                                }
                                else
                                {
                                    //Client client = network as Client;
                                    //await client.Send($"MOVE(POS: {camera.Position} | ID: {client.id.id})");
                                }
                            }*/
                        };
                        break;
                    case Key.D:
                        {
                            camera.Movement(new Vector2D<float>(1.0f, 0.0f));
                            /*
                            if (connected)
                            {
                                if (isServer)
                                {
                                    Server server = network as Server;
                                    await server.SendToAll($"MOVE(POS: {camera.Position} | ID: SERVER)");
                                }
                                else
                                {
                                    //Client client = network as Client;
                                    //await client.Send($"MOVE(POS: {camera.Position} | ID: {client.id.id})");
                                }
                            }*/
                        };
                        break;
                    case Key.Escape:
                        {
                            Program.window.Close();
                        }
                        break;
                    case Key.B:
                        {
                            WorldManager.SpawnEntity(new Entity("Mr. Bean Fly", ref Program.player_renderer, Position: Program.camera.Position, Rotation: new Vector3D<float>(Program.camera.Pitch, Program.camera.Yaw, 0.0f), scale: 1.0f));
                        }
                        break;

                }
            };

            SaveManager.saveFile = "../../retard.save";
           // SaveManager.AddAttribute("Nigger", 10);
           // SaveManager.AddAttribute("Faggot", camera.Position);
            //SaveManager.AddAttribute("Shitty", camera.Pitch);

            Console.WriteLine(SaveManager.GetAttribute("Faggot"));
            SaveManager.EditAttribute("Faggot", "FUCK OFF");
            Console.WriteLine(SaveManager.GetAttribute("Faggot"));
            /*
            player.onUpdate += () =>
            {
                //if (controls.keysPressed.Contains(Key.W)) player.Position += Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * 6.0f / (float)Program.window.FramesPerSecond : Program.camera.Up * 6.0f / (float)Program.window.FramesPerSecond;
                //if (controls.keysPressed.Contains(Key.S)) player.Position -= Program.camera.GetType() == typeof(FPSCamera) ? Program.camera.Forward * 6.0f / (float)Program.window.FramesPerSecond : Program.camera.Up * 6.0f / (float)Program.window.FramesPerSecond;
                //if (controls.keysPressed.Contains(Key.A)) player.Position -= Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * 6.0f / (float)Program.window.FramesPerSecond;
                //if (controls.keysPressed.Contains(Key.D)) player.Position += Vector3D.Normalize<float>(Vector3D.Cross<float>(Program.camera.Forward, Program.camera.Up)) * 6.0f / (float)Program.window.FramesPerSecond;
            };
            */
        }

        private static void OnRender(double obj)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _view = camera.GetViewMatrix();
            _projection = camera.GetProjectionMatrix();

            WorldManager.Initialize();
            WorldManager.Render();
        }


        public static float loop = 0.0f;
        private static bool up = true;
        private static float loopSpeed = 6.0f;
        private static void OnUpdate(double obj)
        {
            controls.Update();
            
            WorldManager.Update();
            WorldManager.FindEntity("Mr. Bean").Rotation = new Vector3D<float>(0.0f, 0.0f, loop * 360.0f);

            Console.WriteLine($"FPS: {window.FramesPerSecond}");

            if (up)
            {
                loop += 1.0f / loopSpeed / (float)window.FramesPerSecond;
                if (loop >= 1.0f) up = false;
            }
            else
            {
                loop -= 1.0f / loopSpeed / (float)window.FramesPerSecond;
                if (loop <= 0.0f) up = true;
            }
            
        }

        private static async void ServerTester()
        {
            while (network == null) { }

            Server server = network as Server;
            Console.Clear();
            Console.WriteLine("!!!STARTED SERVER!!!");

            ThreadStart start = new ThreadStart(ServerMoveTest);
            Thread thread = new Thread(start);
            thread.Start();

            while (true)
            {
                string command = Console.ReadLine();
                await server.SendToAll($"TXT(MSG: '{command}' | FROM: SERVER");
            }
        }

        private static async void ClientTester()
        {
            while (network == null) { }

            Client client = network as Client;
            await client.GetID();

            Console.WriteLine($"GOT ID: {client.id.id}");
            Console.WriteLine("CONNECTED!!!");

            await client.Send($"SPAWN(POS: {camera.Position}, ID: {client.id.id})");

            ThreadStart start = new ThreadStart(MoveTest);
            Thread thread = new Thread(start);
            thread.Start();

            while (true)
            {
                string command = Console.ReadLine();

                switch(command)
                {
                    case string s when (s.StartsWith("USERNAME:")):
                        {
                            await client.Send($"SET_USERNAME(ID: {client.id.id} | USERNAME: {command.Split("USERNAME:")[1]})");
                        }
                        break;
                    case string s when (s.StartsWith("TXT:")):
                        {
                            await client.Send($"TXT(MSG: '{command.Split("TXT:")[1]}' | FROM: {client.id.id})");
                        }
                        break;
                }
            }
        }

        private static async void MoveTest()
        {
            Client client = network as Client;
            Thread.Sleep(1000);
            await client.Send($"MOVE(POS: {Program.camera.Position} | ID: {client.id.id})");
            MoveTest();
        }

        private static async void ServerMoveTest()
        {
            Server server = network as Server;
            Thread.Sleep(1000);
            await server.SendToAll($"MOVE(POS: {Program.camera.Position} | ID: SERVER)");
            ServerMoveTest();
        }
    }
}