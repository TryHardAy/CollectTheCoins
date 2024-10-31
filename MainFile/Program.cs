using System.Text;
using Entities;

namespace Program
{
    
    class GameManager
    {
        Entity?[][] entityMap; // program krzyczy niżej, że może być nullem
        Player player;
        Enemy[] enemies;
        Coin?[] coins = new Coin[5];
        static char coinSign = '$';
        static char playerSign = 'P';
        string[] map;

        public GameManager( string path)
        {
            CreateEntityMap(path);

            enemies = new Enemy[2];

            player = createPlayer();
            entityMap[player.Y][player.X] = player;

        }
        Player createPlayer()
        {
            Random r = new Random();

            int count = r.Next(countNull()/4);

            for (int i = 0; i < entityMap.Length; i++)
            {
                for (int j = 0; j < entityMap[i].Length; j++)
                {
                    if (entityMap[i][j] is null)
                    {
                        count--;

                        if (count == 0)
                        {
                            return new Player(j, i, playerSign);
                        }
                    }
                }
            }

            throw new Exception();
        }
        string[] importMap(string path)
        {
            StreamReader sr = new StreamReader(path);

            string[] map = new string[
                Convert.ToInt32(sr.ReadLine())
                ];

            for (int i = 0; i < map.Length; i++)
            {
                map[i] = sr.ReadLine();
            }

            sr.Close();

            return map;
        }
        public void CreateEntityMap(string path)
        {

            Entity? whatsThere(int x, int y)
            {
                if (player.X == x && player.Y == y)
                    return player;

                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].X == x && enemies[i].Y == y)
                        return enemies[i];
                }

                for (int i = 0; i < coins.Length; i++)
                {
                    if (coins[i] is not null && coins[i]?.X == x && coins[i]?.Y == y)
                        return coins[i];
                }

                return null;
            }

            map = importMap(path);

            entityMap = new Entity?[map.Length][];
            Entity?[] tmp;

            for (int y = 0; y < map.Length; y++)
            {
                tmp = new Entity?[map[y].Length];

                for (int x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] != ' ')
                    {
                        tmp[x] = new Wall(x, y, map[y][x]);
                    }
                    else
                    {
                        tmp[x] = null;
                    }
                }

                entityMap[y] = tmp;
            }
        }
        public bool Swap(int x1, int y1, int x2, int y2)
        {
            if (entityMap[y1][x1] is Wall || entityMap[y2][x2] is Wall)
                return false;

            (entityMap[y1][x1], entityMap[y2][x2]) = (entityMap[y2][x2], entityMap[y1][x1]);
            return true;
        }
        public Entity? CheckPosition(int x, int y)
        {
            return entityMap[y][x];
        }
        int countNull()
        {
            int cnt = 0;

            for (int i = 0; i < entityMap.Length; i++)
            {
                for (int j = 0; j < entityMap[i].Length; j++)
                {
                    if (entityMap[i][j] is null)
                        cnt++;
                }
            }

            return cnt;
        }
        public void CreateCoin()
        {
            void placeCoin(int x, int y)
            {
                Coin coin = new Coin(x, y, coinSign);

                for (int k = 0; k < coins.Length; k++)
                {
                    if (coins[k] is null)
                    {
                        coins[k] = coin;
                        entityMap[y][x] = coin;
                        return;
                    }
                }
            }

            Random r = new Random();
            int count = countNull();
            count = r.Next(count);

            for (int i = 0; i < entityMap.Length; i++)
            {
                for (int j = 0; j < entityMap[i].Length; j++)
                {
                    if (entityMap[i][j] is null)
                    {
                        count--;

                        if (count == 0)
                        {
                            placeCoin(j, i);
                            return;
                        }
                    }
                }
            }
        }
        void removeCoin(Coin coin)
        {
            for (int i = 0; i < coins.Length;i++)
            {
                if (coin == coins[i])
                {
                    coins[i] = null;
                    return;
                }
            }
        }
        public bool MovePlayer(ConsoleKey key)
        {
            int xend, yend;

            if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow)
            {
                (xend, yend) = (player.X - 1, player.Y);
            }
            else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow)
            {
                (xend, yend) = (player.X + 1, player.Y);
            }
            else if (key == ConsoleKey.W || key == ConsoleKey.UpArrow)
            {
                (xend, yend) = (player.X, player.Y - 1);
            }
            else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow)
            {
                (xend, yend) = (player.X, player.Y + 1);
            }
            else
            {
                return false;
            }

            if (entityMap[yend][xend] is Coin)
            {
                player.IncreaseScore();

#pragma warning disable CS8604
                removeCoin(entityMap[yend][xend] as Coin);
#pragma warning restore CS8604
                CreateCoin();

                entityMap[yend][xend] = null;
            }

            if (Swap(player.X, player.Y, xend, yend))
            {
                player.ChangePosition(xend, yend);
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Score: {player.Score}, High Score: {player.HighScore}");
            
            for (int i = 0; i < entityMap.Length; i++)
            {
                for (int j = 0; j < entityMap[i].Length; j++)
                {
                    if (entityMap[i][j] is null)
                        stringBuilder.Append(' ');
                    else
                        stringBuilder.Append(entityMap[i][j]);
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = "map1.txt";
            ConsoleKey key;

            GameManager gameManager = new GameManager(path);
            gameManager.CreateCoin();
            gameManager.CreateCoin();

            while (true)
            {
                Console.WriteLine(gameManager);
                key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape)
                    break;

                gameManager.MovePlayer(key);

                Console.Clear();
            }
        }
    }
}
