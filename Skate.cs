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
        private Random random;

        public string name;
        public int height;
        public int balance;
        public bool bail;
        public bool grind;
        public bool manual;
        public bool grab;
        public bool kickflip;
        public Area area;
        public int score;
        public int combo;
        public int multiplier;

        public Skater()
        {
            random = new Random();

            name = "Skater";
            area = new Area();
        }

        public void PrintDebug()
        {
            Console.WriteLine("SKATER {0} -- ({1} x{2})", name, combo, multiplier);
            Console.WriteLine("> AREA: {0}", area.current);
            Console.WriteLine("> score: {0}", score);
            Console.WriteLine("> height: {0}", height);
            Console.WriteLine("> balance: {0}", balance);
            Console.WriteLine("> grind: {0}", grind);
            Console.WriteLine("> manual: {0}", manual);
            Console.WriteLine("> grab: {0}", grab);
            Console.WriteLine("> kickflip: {0}", kickflip);
        }

        public void Print()
        {
            var comboText = "";
            if (combo > 0)
                comboText = string.Format(" ({0} x{1})", combo, multiplier);

            Console.Write("SKATER {0} - {1} points{2}", name, score, comboText);

            if (score > 0)
                Console.Write(" - ({0} x{1})", combo, multiplier);

            if (height > 0)
                Console.Write(" - airborne ({0}m)", height);

            if (bail)
                Console.Write(" - bailed!");
            if (grab)
                Console.Write(" - grabbing!");
            if (kickflip)
                Console.Write(" - kickflipping!");
            if (grind)
                Console.Write(" - grinding ({0})!", balance);
            if (manual)
                Console.Write(" - manualling ({0})!", balance);

            Console.WriteLine("");
        }

        public void PrintArea()
        {
            if (height > 0)
                Console.WriteLine("SKATER {0} is airborne!", name);

            if (height > 1)
                return;

            if (height == 1)
            {
                Console.WriteLine(" * below is {0}", area.ahead);
                return;
            }

            if (area.current != Surface.Land)
                Console.WriteLine(" * current is {0}", area.current);
            if (area.ahead != Surface.Land)
                Console.WriteLine(" * ahead is {0}", area.ahead);
            if (area.left != Surface.Land)
                Console.WriteLine(" * left is {0}", area.left);
            if (area.right != Surface.Land)
                Console.WriteLine(" * right is {0}", area.right);
        }

        public void Act(Action action)
        {
            if (bail)
            {
                Console.WriteLine("SKATER {0} recover!", name);
                bail = false;
                return;
            }

            Console.WriteLine("SKATER {0} does {1}", name, action);

            var dir = 0;
            var landed = false;
            var tryJump = false;
            var tryManual = false;
            var tryGrind = false;
            var tryGrab = false;
            var tryKickflip = false;

            switch (action)
            {
                case Action.None:
                    dir = 0;
                    break;

                case Action.Forward:
                    dir = 0;
                    break;

                case Action.Left:
                    dir = 1;
                    break;

                case Action.Right:
                    dir = 2;
                    break;

                case Action.Jump:
                    dir = 0;
                    tryJump = true;
                    break;

                case Action.Grind:
                    dir = 0;
                    tryGrind = true;
                    break;

                case Action.Manual:
                    dir = 0;
                    tryManual = true;
                    break;

                case Action.Kickflip:
                    dir = 0;
                    tryKickflip = true;
                    break;

                case Action.Grab:
                    dir = 0;
                    tryGrab = true;
                    break;
            }

            if (height == 0 && tryJump)
            {
                if (area.current == Surface.Land)
                    height += 2;
                if (area.current == Surface.Kicker)
                    height += 4;
                if (area.current == Surface.Ramp)
                    height += 6;

                manual = false;
                grind = false;
                balance = 0;
            }

            if (height > 0)
            {
                height -= 1;

                if (height == 0)
                    landed = true;
            }

            if (grab && !tryGrab)
                grab = false;

            if (grind && dir == 1)
                balance -= 1;
            if (grind && dir == 2)
                balance += 1;

            if (manual && dir == 1)
                balance -= 1;
            if (manual && dir == 2)
                balance += 1;

            if (grind)
            {
                var waver = random.Next() % 3;
                if (waver == 1)
                    balance -= 1;
                if (waver == 2)
                    balance += 1;
            }

            if (grind && balance < -1)
            {
                Bail();
                return;
            }

            if (grind && balance > +1)
            {
                Bail();
                return;
            }

            if (kickflip && height > 0)
                kickflip = false;

            if (grab && height > 0)
                combo += 10;

            if (manual && height <= 0 && area.current != Surface.Rail)
                combo += 5;
            if (grind && height <= 0 && area.current == Surface.Rail)
                combo += 5;

            if (height <= 0 && tryGrab)
            {
                Bail();
                return;
            }

            if (height <= 0 && tryKickflip)
            {
                Bail();
                return;
            }

            if (height <= 0 && (area.current == Surface.Land || area.current == Surface.Kicker || area.current == Surface.Ramp) && tryManual)
            {
                manual = true;
                combo += 10;
                multiplier += 1;
            }

            if (height <= 0 && (area.current == Surface.Rail) && tryGrind)
            {
                grind = true;
                combo += 10;
                multiplier += 1;
            }

            if (height > 0 && tryKickflip)
            {
                kickflip = true;
                combo += 10;
                multiplier += 2;
            }

            if (height > 0 && tryGrab)
            {
                grab = true;
                combo += 10;
                multiplier += 1;
            }

            if (landed && !manual)
                Land();

            area.Move(dir);
        }

        public void Land()
        {
            Console.WriteLine("SKATER {0} LANDED!");

            score += combo * multiplier;

            if (combo > 0)
                Console.WriteLine("  *** {0} points ***", combo * multiplier);

            combo = 0;
            multiplier = 0;
            bail = false;
            grind = false;
            manual = false;
            grab = false;
            kickflip = false;
        }

        public void Bail()
        {
            Console.WriteLine("SKATER {0} BAILED!");

            bail = true;
            combo = 0;
            multiplier = 0;
            grind = false;
            manual = false;
            grab = false;
            kickflip = false;
        }
    }

    public enum Surface
    {
        None,
        Land,
        Rail,
        Kicker,
        Ramp,
    }

    public enum Action
    {
        None,
        Forward,
        Left,
        Right,
        Jump,
        Grind,
        Manual,
        Kickflip,
        Grab,

        // 
        //   _0_
        //    |
        //   / 7
        // -------
        //  o   o   

    }

    public class Area
    {
        private Random random;

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

            switch (choice % 4)
            {
                case 0: return Surface.Land;
                case 1: return Surface.Rail;
                case 2: return Surface.Kicker;
                case 3: return Surface.Ramp;
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

    class ProgramWrapper
    {
        public static void MainWrapper(string[] args)
        {
            Console.WriteLine("SKATE TIME");

            Console.WriteLine("");
            Console.WriteLine("   _0_    ");
            Console.WriteLine("    |     ");
            Console.WriteLine("   / 7    ");
            Console.WriteLine(" -------  ");
            Console.WriteLine("  o   o   ");
            Console.WriteLine("");

            var skater = new Skater();

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("");

                skater.Print();
                skater.PrintArea();

                Console.WriteLine("Choose: [W] forward, [A] left, [D] right, [J] jump, [R] grind, [M] manual [G] grab [K] kickflip [ESC] quit");

                var key = Console.ReadKey(true);
                var action = Action.None;
                var exit = false;

                switch (key.Key)
                {
                    case ConsoleKey.Escape: exit = true; break;
                    case ConsoleKey.Enter: action = Action.None; break;
                    case ConsoleKey.W: action = Action.Forward; break;
                    case ConsoleKey.A: action = Action.Left; break;
                    case ConsoleKey.D: action = Action.Right; break;
                    case ConsoleKey.J: action = Action.Jump; break;
                    case ConsoleKey.R: action = Action.Grind; break;
                    case ConsoleKey.M: action = Action.Manual; break;
                    case ConsoleKey.K: action = Action.Kickflip; break;
                    case ConsoleKey.G: action = Action.Grab; break;
                }

                if (exit)
                    break;

                skater.Act(action);
            }

            Console.WriteLine("FINISH");
        }
    }
}
