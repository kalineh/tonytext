using System;

// 
//   _0_
//    |
//   / \
//  -o--o-

namespace tonytext
{
    class Skater
    {
        public string name;
        public float height;
        public bool grind;
        public bool manual;
        public Area area;
        public int score;
        public int multiplier;

        public Skater()
        {
            name = "Skater";
            area = new Area();
        }

        public void Print()
        {
            Console.WriteLine("SKATER {0} - {1} {2} {3} ({4} x{5})", name, height, grind ? "grinding" : "", manual ? "manualing" : "", score, multiplier);
        }

        public void Act(Action action)
        {
            Console.WriteLine("SKATER {0} does {0}", action);

            switch (action)
            {
                case Action.None:
                    area.Move(0);
                    break;

                case Action.Left:
                    area.Move(1);
                    break;

                case Action.Right:
                    area.Move(2);
                    break;

                case Action.Jump:
                    area.Move(0);
                    break;

                case Action.Grind:
                    area.Move(0);
                    break;

                case Action.Manual:
                    area.Move(0);
                    break;
            }
        }
    }

    public enum Surface
    {
        None,
        Air,
        Water,
        Land,
        Rail,
        Kicker,
        Ramp,
    }

    public enum Action
    {
        None,
        Left,
        Right,
        Jump,
        Grind,
        Manual,

        // 
        //   _0_
        //    |
        //   / 7
        // -------
        //  o   o   

    }

    public class Area
    {
        public Random random;

        public Surface current;
        public Surface ahead;
        public Surface left;
        public Surface right;

        public Area()
        {
            random = new Random();
            current = Surface.Land;
            ahead = RandomSurface();
            left = RandomSurface();
            right = RandomSurface();
        }

        public Surface RandomSurface()
        {
            var choice = random.Next();

            switch (choice % 6)
            {
                case 0: return Surface.Air;
                case 1: return Surface.Water;
                case 2: return Surface.Land;
                case 3: return Surface.Rail;
                case 4: return Surface.Kicker;
                case 5: return Surface.Ramp;
            }

            return Surface.None;
        }

        public void Move(int direction)
        {
            switch (direction)
            {
                case 0:
                    current = ahead;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;

                case 1:
                    current = left;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;

                case 2:
                    current = right;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;
            }
        }

        public void Print()
        {
            Console.WriteLine("AREA: Current: {0}, Ahead: {1}, Left: {2}, Right: {3}", current, ahead, left, right);
        }
    }

    class Program
    {
        // you are on a grind rail
        // you are x above ground
        // choose action
        // to your left you see a grind rail
        // ahead is a halfpipe

        static void Main(string[] args)
        {
            Console.WriteLine("SKATE TIME");

            var skater = new Skater();

            while (true)
            {
                skater.Print();
                skater.area.Print();

                Console.WriteLine("Choose: [W] forward, [A] left, [D] right, [J] jump, [G] grind, [M] manual [ESC] quit");

                var key = Console.ReadKey(true);
                var action = Action.None;
                var exit = false;

                switch (key.Key)
                {
                    case ConsoleKey.Escape: exit = true; break;
                    case ConsoleKey.W: action = Action.None; break;
                    case ConsoleKey.A: action = Action.Left; break;
                    case ConsoleKey.D: action = Action.Right; break;
                    case ConsoleKey.J: action = Action.Jump; break;
                    case ConsoleKey.G: action = Action.Grind; break;
                    case ConsoleKey.M: action = Action.Manual; break;
                }

                if (exit)
                    break;

                skater.Act(action);
            }

            Console.WriteLine("FINISH");
        }
    }
}
