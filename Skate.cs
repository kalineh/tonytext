using System;

// 
//   _0_
//    |
//   / 7
//  -o--o-

namespace tonytext
{
    class Skater
    {
        public enum State
        {
            Skating,
            Grinding,
            Manualing,
            Grabbing,
            Bailing,
        }

        private static int ManualCombo = 10;
        private static int ManualMultiplier = 1;
        private static int ManualContinue = 10;

        private static int GrindCombo = 10;
        private static int GrindMultiplier = 1;
        private static int GrindContinue = 10;

        private static int GrabCombo = 25;
        private static int GrabMultiplier = 1;
        private static int GrabContinue = 50;

        private static int KickflipCombo = 20;
        private static int KickflipMultiplier = 2;

        private static int JumpLand = 2;
        private static int JumpGrind = 3;
        private static int JumpKicker = 4;
        private static int JumpRamp = 6;

        private static int RideKicker = 1;
        private static int RideRamp = 2;

        private Random random;

        public string name;
        public int height;
        public int balance;
        public State state;
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
            Console.WriteLine("> state: {0}", state);
        }

        public void PrintSkaterConsole()
        {
            var comboText = "";
            if (combo > 0)
                comboText = string.Format(" ({0} x{1})", combo, multiplier);

            Console.Write("SKATER {0} - {1} points{2}", name, score, comboText, state, area.current);

            if (height == 0)
                Console.Write(" [{0} on {1}]", state, area.current);
            if (height > 0)
                Console.Write(" [airborne {0}m]", height);

            if (score > 0)
                Console.Write(" - ({0} x{1})", combo, multiplier);

            if (state == State.Grinding)
                Console.Write(" - grind balance {0}", balance);
            if (state == State.Manualing)
                Console.Write(" - manual balance {0}", balance);

            Console.WriteLine("");
        }

        public void PrintAreaConsole()
        {
            if (height > 1)
            {
                Console.WriteLine(" * big air");
                return;
            }

            if (height == 1)
            {
                Console.WriteLine(" * below is {0}", area.ahead);
                return;
            }

            if (state == State.Grinding)
            {
                Console.WriteLine(" * ahead is {0}", area.ahead);
                return;
            }

            if (area.ahead != Surface.Land)
                Console.WriteLine(" * ahead is {0}", area.ahead);
            if (area.left != Surface.Land)
                Console.WriteLine(" * left is {0}", area.left);
            if (area.right != Surface.Land)
                Console.WriteLine(" * right is {0}", area.right);
        }

        public void Act(Action action)
        {
            if (state == State.Bailing)
            {
                Console.WriteLine("SKATER {0} recover!", name);
                state = State.Skating;
                return;
            }

            Console.WriteLine("SKATER {0} does {1} (state: {2})", name, action, state);

            var prevHeight = height;
            var currHeight = height;

            currHeight = System.Math.Max(height - 1, 0);

            height = currHeight;

            if (state == State.Skating)
            {
                var bail = false;

                var dir = 0;
                var landed = (prevHeight == 1) && (currHeight == 0);

                if (action == Action.Left)
                    dir = -1;
                if (action == Action.Right)
                    dir = +1;

                area.Move(dir);

                if (prevHeight == 0)
                {
                    if (action == Action.Jump)
                    {
                        if (area.previous == Surface.Land)
                            height += JumpLand;
                        if (area.previous == Surface.Rail)
                            height += JumpLand;
                        if (area.previous == Surface.Kicker)
                            height += JumpKicker;
                        if (area.previous == Surface.Ramp)
                            height += JumpRamp;
                    }
                    else
                    {
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Kicker)
                            height += RideKicker;
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Ramp)
                            height += RideRamp;
                    }
                }

                switch (action)
                {
                    case Action.None:
                        break;

                    case Action.Forward:
                        break;

                    case Action.Backward:
                        break;

                    case Action.Left:
                        break;

                    case Action.Right:
                        break;

                    case Action.Jump:
                        if (height == 0)
                        {
                            if (area.current == Surface.Land)
                                height += JumpLand;
                            if (area.current == Surface.Rail)
                                height += JumpLand;
                            if (area.current == Surface.Kicker)
                                height += JumpKicker;
                            if (area.current == Surface.Ramp)
                                height += JumpRamp;
                        }
                        break;

                    case Action.Grind:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                            combo += GrindCombo;
                            multiplier += GrindMultiplier;
                        }
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current != Surface.Rail)
                        {
                            state = State.Manualing;
                            balance = 0;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                        }
                        if (area.current == Surface.Rail)
                            bail = true;
                        break;

                    case Action.Kickflip:
                        if (height == 0)
                        {
                            bail = true;
                            break;
                        }

                        combo += KickflipCombo;
                        multiplier += KickflipMultiplier;
                        break;

                    case Action.Grab:
                        if (height == 0)
                        {
                            bail = true;
                            break;
                        }

                        combo += GrabCombo;
                        multiplier += GrabMultiplier;
                        state = State.Grabbing;
                        break;
                }

                if (bail)
                {
                    Bail();
                    return;
                }

                if (landed && state == State.Skating)
                {
                    Land();
                    return;
                }

                return;
            }

            if (state == State.Grinding)
            {
                var landed = (prevHeight == 1) && (currHeight == 0);

                var bail = false;

                area.Move(0);

                switch (action)
                {
                    case Action.None:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                        }
                        break;

                    case Action.Forward:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                        }
                        break;

                    case Action.Backward:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                        }
                        break;

                    case Action.Left:
                        balance -= 1;

                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                        }
                        break;

                    case Action.Right:
                        balance += 1;

                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                        }
                        break;

                    case Action.Jump:
                        height += JumpGrind;
                        state = State.Skating;
                        break;

                    case Action.Grind:
                        combo += GrindContinue;
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current != Surface.Rail)
                        {
                            state = State.Manualing;
                            balance = 0;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                        }

                        if (area.current == Surface.Rail)
                            bail = true;

                        break;

                    case Action.Kickflip:
                        if (height > 0)
                        {
                            state = State.Skating;
                            combo += KickflipCombo;
                            multiplier += KickflipMultiplier;
                        }
                        if (height == 0)
                            bail = true;
                        break;

                    case Action.Grab:
                        if (height > 0)
                        {
                            state = State.Grabbing;
                            combo += GrabCombo;
                            multiplier += GrabMultiplier;
                        }

                        if (height == 0)
                            bail = true;
                        break;
                }

                if (balance < -1)
                    bail = true;
                if (balance > +1)
                    bail = true;

                if (bail)
                {
                    Bail();
                    return;
                }

                if (landed && state == State.Skating)
                {
                    Land();
                    return;
                }

                return;
            }

            if (state == State.Manualing)
            {
                var bail = false;
                var dir = 0;

                if (action == Action.Left)
                    dir = -1;
                if (action == Action.Right)
                    dir = +1;

                area.Move(dir);

                if (prevHeight == 0)
                {
                    if (action == Action.Jump)
                    {
                        if (area.previous == Surface.Land)
                            height += JumpLand;
                        if (area.previous == Surface.Rail)
                            height += JumpLand;
                        if (area.previous == Surface.Kicker)
                            height += JumpKicker;
                        if (area.previous == Surface.Ramp)
                            height += JumpRamp;
                    }
                    else
                    {
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Kicker)
                            height += RideKicker;
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Ramp)
                            height += RideRamp;
                    }
                }

                switch (action)
                {
                    case Action.None:
                        if (area.current == Surface.Rail)
                            bail = true;
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        break;

                    case Action.Forward:
                        balance += 1;
                        if (area.current == Surface.Rail)
                            bail = true;
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        break;

                    case Action.Backward:
                        balance -= 1;
                        if (area.current == Surface.Rail)
                            bail = true;
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        break;

                    case Action.Left:
                        if (area.current == Surface.Rail)
                            bail = true;
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        break;

                    case Action.Right:
                        if (area.current == Surface.Rail)
                            bail = true;
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        break;

                    case Action.Jump:
                        if (height == 0)
                        {
                            if (area.current == Surface.Land)
                                height += JumpLand;
                            if (area.current == Surface.Rail)
                                height += JumpLand;
                            if (area.current == Surface.Kicker)
                                height += JumpKicker;
                            if (area.current == Surface.Ramp)
                                height += JumpRamp;
                        }
                        state = State.Skating;
                        break;

                    case Action.Grind:
                        if (area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                        }
                        break;

                    case Action.Manual:
                        if (area.current == Surface.Rail)
                            bail = true;
                        break;

                    case Action.Kickflip:
                        bail = true;
                        break;

                    case Action.Grab:
                        bail = true;
                        break;
                }

                if (balance < -1)
                    bail = true;
                if (balance > +1)
                    bail = true;

                if (bail)
                {
                    Bail();
                    return;
                }

                return;
            }

            if (state == State.Grabbing)
            {
                var landed = (prevHeight == 1) && (currHeight == 0);

                var bail = false;

                area.Move(0);

                switch (action)
                {
                    case Action.None:
                        state = State.Skating;
                        break;

                    case Action.Forward:
                        state = State.Skating;
                        break;

                    case Action.Backward:
                        state = State.Skating;
                        break;

                    case Action.Left:
                        state = State.Skating;
                        break;

                    case Action.Right:
                        state = State.Skating;
                        break;

                    case Action.Jump:
                        state = State.Skating;
                        if (area.current == Surface.Land)
                            height += JumpLand;
                        if (area.current == Surface.Rail)
                            height += JumpLand;
                        if (area.current == Surface.Kicker)
                            height += JumpKicker;
                        if (area.current == Surface.Ramp)
                            height += JumpRamp;
                        break;

                    case Action.Grind:
                        if (area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                            break;
                        }

                        state = State.Skating;
                        break;

                    case Action.Manual:
                        if (area.current == Surface.Rail)
                        {
                            bail = true;
                            break;
                        }

                        if (height == 0)
                        {
                            state = State.Manualing;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                        }
                        break;

                    case Action.Kickflip:
                        bail = true;
                        break;

                    case Action.Grab:
                        if (height == 0)
                            bail = true;
                        combo += GrabContinue;
                        break;
                }

                if (bail)
                {
                    Bail();
                    return;
                }

                if (landed && state == State.Skating)
                {
                    Land();
                    return;
                }

                return;
            }

            Console.WriteLine("SKATER {0} does {1}", name, action);

            return;
        }

        public void Land()
        {
            Console.WriteLine("SKATER {0} LANDED!", name);

            score += combo * multiplier;

            if (combo > 0)
                Console.WriteLine("  *** {0} points ***", combo * multiplier);

            combo = 0;
            multiplier = 0;
            state = State.Skating;
            balance = 0;
        }

        public void Bail()
        {
            Console.WriteLine("SKATER {0} BAILED!", name);

            combo = 0;
            multiplier = 0;
            state = State.Bailing;
            balance = 0;
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
        Backward,
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

        public Surface previous;
        public Surface current;
        public Surface ahead;
        public Surface left;
        public Surface right;

        public Area()
        {
            random = new Random();

            previous = Surface.Land;
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
                    previous = current;
                    current = ahead;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;

                case -1:
                    previous = current;
                    current = left;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;

                case +1:
                    previous = current;
                    current = right;
                    ahead = RandomSurface();
                    left = RandomSurface();
                    right = RandomSurface();
                    break;
            }
        }

        public void Print()
        {
            Console.WriteLine("AREA: Previous: {0}, Current: {1}, Ahead: {2}, Left: {3}, Right: {4}", previous, current, ahead, left, right);
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

                Console.WriteLine("Choose: [WASD] move, [J] jump, [R] grind, [M] manual [G] grab [K] kickflip [ESC] quit");

                var key = Console.ReadKey(true);
                var action = Action.None;
                var exit = false;

                switch (key.Key)
                {
                    case ConsoleKey.Escape: exit = true; break;
                    case ConsoleKey.Enter: action = Action.None; break;
                    case ConsoleKey.W: action = Action.Forward; break;
                    case ConsoleKey.S: action = Action.Backward; break;
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
