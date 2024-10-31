namespace Entities
{
    public abstract class Entity
    {
        protected int x, y;
        char sign;

        public Entity(int x, int y, char sign)
        {
            this.x = x;
            this.y = y;
            this.sign = sign;
        }
        public int X
        {
            get { return x; }
        }
        public int Y
        {
            get { return y; }
        }
        public override string ToString()
        {
            return Convert.ToString(sign);
        }
    }
    public class Coin : Entity
    {
        public Coin(int x, int y, char sign)
            : base(x, y, sign)
        {

        }
    }
    public class Wall : Entity
    {
        public Wall(int x, int y, char sign)
            : base(x, y, sign)
        {

        }
    }
    public abstract class MovingEntity : Entity
    {
        public MovingEntity(int x, int y, char sign)
            : base(x, y, sign)
        {

        }
        public void ChangePosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public class Player : MovingEntity
    {
        int score;
        int highScore;
        static string high_Score_Path = "high_Score.txt";

        public Player(int x, int y, char sign)
            : base(x, y, sign)
        {
            score = 0;
            highScore = importScore(high_Score_Path);
        }
        int importScore(string path)
        {
            if (!File.Exists(path))
            {
                return 0;
            }

            StreamReader sr = new StreamReader(path);

            int result = Convert.ToInt32(sr.ReadLine());

            sr.Close();

            return result;
        }
        public void SaveHighScore()
        {
            StreamWriter sw = new StreamWriter(high_Score_Path);

            sw.WriteLine(highScore);

            sw.Close();
        }
        public void IncreaseScore()
        {
            score += 1;

            if (highScore < score)
            {
                highScore = score;
            }

            SaveHighScore();
        }
        public int Score
        {
            get { return score; }
        }
        public int HighScore
        {
            get { return highScore; }
        }
    }
    public class Enemy : MovingEntity
    {
        public Enemy(int x, int y, char sign)
            : base(x, y, sign)
        {

        }
    }
}