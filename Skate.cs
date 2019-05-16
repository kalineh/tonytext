using System;

// 
//   _0_
//    |
//   / 7
//  -o--o-

namespace tonytext
{
    class Discord
    {
        public static Action DiscordGetVote()
        {
            Console.WriteLine("Vote: [WASD] move, [J] jump");
            Console.WriteLine("Vote: [R] grind, [M] manual [G] grab [K] kickflip");

            var key = Console.ReadKey(true);
            var action = Action.None;
            var exit = false;

            switch (key.Key)
            {
                case ConsoleKey.Escape: exit = true; break;
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

            Console.WriteLine(" * Winning Vote: {0}", action);
            Console.WriteLine("");

            return action;
        }

        public static void DiscordWaitCycle()
        {
            // while time < nextcycle; wait
            return;
        }

        public static void DiscordSetMessage(string msg)
        {
            Console.WriteLine("[DISCORD]: {0}", msg);
        }

        public static void DiscordSetReactions()
        {
            Console.WriteLine("[REACTIONS]");
        }

        public static string DiscordActionReaction(Action action)
        {
            switch (action)
            {
                case Action.Forward: return "up_arrow"; break;
                case Action.Backward: return "down_arrow"; break;
                case Action.Left: return "left_arrow"; break;
                case Action.Right: return "right_arrow"; break;
                case Action.Jump: return "jump"; break;
                case Action.Grind: return "grind"; break;
                case Action.Manual: return "manual"; break;
                case Action.Kickflip: return "kickflip"; break;
                case Action.Grab: return "grab"; break;
            }

            return "none";
        }
    }

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

        private static int TiltChance = 20;

        private Random random;

        public string name;
        public int height;
        public int balance;
        public State state;
        public Area area;
        public int score;
        public int combo;
        public int multiplier;

        public string report;

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
            var status = DiscordStatus();

            Console.Write(status);
            Console.WriteLine("");
        }

        public string DiscordStatus()
        {
            var status = string.Format("{0} - {1} points", name, score);

            // skater

            if (combo > 0)
                status += string.Format(" - combo ({0} x{1})", combo, multiplier);

            if (height == 0)
                status += string.Format(" [{0} on {1}]", state, area.current);
            if (height > 0)
                status += string.Format(" [airborne {0}m]", height);

            if (state == State.Grinding && balance < 0)
                status += string.Format(" [tilting left]");
            if (state == State.Grinding && balance > 0)
                status += string.Format(" [tilting right]");

            if (state == State.Manualing && balance < 0)
                status += string.Format(" [tilting back]");
            if (state == State.Manualing && balance > 0)
                status += string.Format(" [tilting forward]");
            if (state == State.Grabbing)
                status += string.Format(" [holding a grab]");

            if (state == State.Bailing)
                return status;

            // area

            if (height > 1)
                return status;

            if (height == 1)
            {
                if (area.ahead == Surface.Land) status += string.Format("\nAbout to land.");
                if (area.ahead == Surface.Rail) status += string.Format("\nAbout to land on a rail.");
                if (area.ahead == Surface.Kicker) status += string.Format("\nAbout to land on a kicker.");
                if (area.ahead == Surface.Ramp) status += string.Format("\nAbout to land on a ramp.");

                return status;
            }

            if (state == State.Grinding)
            {
                if (area.ahead == Surface.Land) status += string.Format("\nGrind rail ends ahead.");
                if (area.ahead == Surface.Rail) status += string.Format("\nGrind rail continues ahead.");
                if (area.ahead == Surface.Kicker) status += string.Format("\nGrind rail ends at a kicker ahead.");
                if (area.ahead == Surface.Ramp) status += string.Format("\nGrind rail ends at a ramp ahead.");
                return status;
            }

            if (area.current != Surface.Kicker && area.current != Surface.Ramp && area.ahead != Surface.Land)
            {
                if (area.ahead == Surface.Rail) status += string.Format("\nAhead is a grind rail.");
                if (area.ahead == Surface.Kicker) status += string.Format("\nAhead is a kicker.");
                if (area.ahead == Surface.Ramp) status += string.Format("\nAhead is a ramp.");
            }

            if (area.left != Surface.Land)
            {
                if (area.left == Surface.Rail) status += string.Format("\nThere is a grind rail to the left.");
                if (area.left == Surface.Kicker) status += string.Format("\nThere is a kicker to the left.");
                if (area.left == Surface.Ramp) status += string.Format("\nThere is a ramp to the left.");
            }

            if (area.right != Surface.Land)
            {
                if (area.right == Surface.Rail) status += string.Format("\nThere is a grind rail to the right.");
                if (area.right == Surface.Kicker) status += string.Format("\nThere is a kicker to the right.");
                if (area.right == Surface.Ramp) status += string.Format("\nThere is a ramp to the right.");
            }

            return status;
        }

        public void Act(Action action)
        {
            if (state == State.Bailing)
            {
                report = string.Format("{0} gets back up.", name);
                state = State.Skating;
                return;
            }

            report = "";
            //report = string.Format("{0} is {1}.", name, state);

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
                        {
                            height += JumpLand;
                            report += string.Format("{0} does an ollie! ", name);
                        }

                        if (area.previous == Surface.Rail)
                        {
                            height += JumpLand;
                            report += string.Format("{0} does an ollie! ", name);
                        }

                        if (area.previous == Surface.Kicker)
                        {
                            height += JumpKicker;
                            report += string.Format("{0} jumps off a kicker! ", name);
                        }

                        if (area.previous == Surface.Ramp)
                        {
                            height += JumpRamp;
                            report += string.Format("{0} jumps off a ramp! ", name);
                        }
                    }
                    else
                    {
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Kicker)
                        {
                            height += RideKicker;
                            report += string.Format("{0} rides off a kicker. ", name);
                        }

                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Ramp)
                        {
                            height += RideRamp;
                            report += string.Format("{0} rides off a ramp. ", name);
                        }
                    }
                }

                switch (action)
                {
                    case Action.None:
                        if (height == 0) report += string.Format("{0} skates forward.", name);
                        break;

                    case Action.Forward:
                        if (height == 0) report += string.Format("{0} skates forward.", name);
                        break;

                    case Action.Backward:
                        if (height == 0) report += string.Format("{0} skates forward.", name);
                        break;

                    case Action.Left:
                        if (height == 0) report += string.Format("{0} skates to the left.", name);
                        break;

                    case Action.Right:
                        if (height == 0) report += string.Format("{0} skates to the right.", name);
                        break;

                    case Action.Jump:
                        if (height == 0)
                        {
                            if (area.current == Surface.Land)
                            {
                                height += JumpLand;
                                report += string.Format("{0} does an ollie!", name);
                            }

                            if (area.current == Surface.Rail)
                            {
                                height += JumpLand;
                                report += string.Format("{0} does an ollie!", name);
                            }

                            if (area.current == Surface.Kicker)
                            {
                                height += JumpKicker;
                                report += string.Format("{0} does an ollie!", name);
                            }

                            if (area.current == Surface.Ramp)
                            {
                                height += JumpRamp;
                                report += string.Format("{0} does an ollie!", name);
                            }
                        }
                        break;

                    case Action.Grind:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                            combo += GrindCombo;
                            multiplier += GrindMultiplier;
                            if (landed)
                                report += string.Format("{0} lands into a rail grind!", name);
                            else
                                report += string.Format("{0} starts grinding a rail!", name);
                        }
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current != Surface.Rail)
                        {
                            state = State.Manualing;
                            balance = 0;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                            if (landed)
                                report += string.Format("{0} lands into a manual!", name);
                            else
                                report += string.Format("{0} starts a manual!", name);
                        }

                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} tries to manual into a grind rail and bails.", name);
                            break;
                        }

                        break;

                    case Action.Kickflip:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} tries to kickflip into the floor and bails.", name);
                            break;
                        }

                        combo += KickflipCombo;
                        multiplier += KickflipMultiplier;
                        report += string.Format("{0} does a kickflip!", name);
                        break;

                    case Action.Grab:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} tries to do a grab and bails.", name);
                            break;
                        }

                        combo += GrabCombo;
                        multiplier += GrabMultiplier;
                        state = State.Grabbing;
                        report += string.Format("{0} starts doing a grab!", name);
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

                var tilt = 0;
                var roll = random.Next() % 100;

                if (roll < Skater.TiltChance)
                    tilt = -1;
                if (roll > (100 - Skater.TiltChance))
                    tilt = +1;

                var bail = false;

                area.Move(0);

                switch (action)
                {
                    case Action.None:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        balance += tilt;
                        report += string.Format("{0} keeps grinding!", name);
                        break;

                    case Action.Forward:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        balance += tilt;
                        report += string.Format("{0} keeps grinding!", name);
                        break;

                    case Action.Backward:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        balance += tilt;
                        report += string.Format("{0} keeps grinding!", name);
                        break;

                    case Action.Left:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        balance -= 1;
                        if (balance == 0)
                            report += string.Format("{0} keeps grinding and adjusts balance!", name);
                        else
                            report += string.Format("{0} keeps grinding!", name);
                        break;

                    case Action.Right:
                        if (area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        balance += 1;
                        if (balance == 0)
                            report += string.Format("{0} keeps grinding and adjusts balance!", name);
                        else
                            report += string.Format("{0} keeps grinding!", name);
                        break;

                    case Action.Jump:
                        height += JumpGrind;
                        state = State.Skating;
                        report += string.Format("{0} jumps from the grind rail!", name);
                        break;

                    case Action.Grind:
                        if (height == 0 && area.current != Surface.Rail)
                        {
                            state = State.Skating;
                            landed = true;
                            break;
                        }
                        combo += GrindContinue;
                        balance += tilt;
                        report += string.Format(" {0} keeps grinding!", name);
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current != Surface.Rail)
                        {
                            state = State.Manualing;
                            balance = 0;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                            report += string.Format(" {0} hops into a manual!", name);
                            break;
                        }

                        if (area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format(" {0} tries to manual on a rail and bails.", name);
                            break;
                        }

                        break;

                    case Action.Kickflip:
                        if (height > 0)
                        {
                            state = State.Skating;
                            combo += KickflipCombo;
                            multiplier += KickflipMultiplier;
                            report += string.Format(" {0} does a kickflip!", name);
                            break;
                        }
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format(" {0} tries to kickflip and bails.", name);
                            break;
                        }
                        break;

                    case Action.Grab:
                        if (height > 0)
                        {
                            state = State.Grabbing;
                            combo += GrabCombo;
                            multiplier += GrabMultiplier;
                            report += string.Format(" {0} starts a grab!", name);
                            break;
                        }

                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format(" {0} tries to grab into the floor and bails!", name);
                            break;
                        }

                        break;
                }

                if (balance < -1 && state == State.Grinding)
                {
                    bail = true;
                    report += string.Format(" {0} loses balance and bails!", name);
                }

                if (balance > +1 && state == State.Grinding)
                {
                    bail = true;
                    report += string.Format(" {0} loses balance and bails!", name);
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

            if (state == State.Manualing)
            {
                var tilt = 0;
                var roll = random.Next() % 100;

                if (roll < Skater.TiltChance)
                    tilt = -1;
                if (roll > (100 - Skater.TiltChance))
                    tilt = +1;

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
                        {
                            height += JumpLand;
                            report += string.Format("{0} jumps from a manual! ", name);
                        }
                        if (area.previous == Surface.Rail)
                        {
                            height += JumpLand;
                            report += string.Format("{0} jumps from a manual! ", name);
                        }
                        if (area.previous == Surface.Kicker)
                        {
                            height += JumpKicker;
                            report += string.Format("{0} jumps off a kicker while manualing! ", name);
                        }
                        if (area.previous == Surface.Ramp)
                        {
                            height += JumpRamp;
                            report += string.Format("{0} jumps off a ramp while manualing! ", name);
                        }
                    }
                    else
                    {
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Kicker)
                        {
                            height += RideKicker;
                            report += string.Format("{0} rides off a kicker while manualing! ", name);
                        }
                        if (action != Action.Left && action != Action.Right && area.previous == Surface.Ramp)
                        {
                            height += RideRamp;
                            report += string.Format("{0} rides off a kicker while manualing! ", name);
                        }
                    }
                }

                switch (action)
                {
                    case Action.None:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        balance += tilt;
                        report += string.Format("{0} keeps manualing!", name);
                        break;

                    case Action.Forward:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        balance += 1;
                        if (balance == 0)
                            report += string.Format("{0} keeps manualing and adjusts balance!", name);
                        else
                            report += string.Format("{0} keeps manualing!", name);
                        break;

                    case Action.Backward:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        balance -= 1;
                        if (balance == 0)
                            report += string.Format("{0} keeps manualing and adjusts balance!", name);
                        else
                            report += string.Format("{0} keeps manualing!", name);
                        break;

                    case Action.Left:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        balance += tilt;
                        report += string.Format("{0} keeps manualing and turns left!", name);
                        break;

                    case Action.Right:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        if (height > 0)
                        {
                            state = State.Skating;
                            break;
                        }
                        combo += ManualContinue;
                        balance += tilt;
                        report += string.Format("{0} keeps manualing and turns right!", name);
                        break;

                    case Action.Jump:
                        if (height == 0)
                        {
                            if (area.current == Surface.Land)
                            {
                                height += JumpLand;
                                report += string.Format("{0} manuals into an ollie!", name);
                            }
                            if (area.current == Surface.Rail)
                            {
                                height += JumpLand;
                                report += string.Format("{0} manuals into an ollie!", name);
                            }
                            if (area.current == Surface.Kicker)
                            {
                                height += JumpKicker;
                                report += string.Format("{0} manuals into a kicker jump!", name);
                            }
                            if (area.current == Surface.Ramp)
                            {
                                height += JumpRamp;
                                report += string.Format("{0} manuals into a ramp jump!", name);
                            }
                        }
                        state = State.Skating;
                        break;

                    case Action.Grind:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                            combo += GrindCombo;
                            multiplier += GrindMultiplier;
                            report += string.Format("{0} goes from manual into a grind!", name);
                            break;
                        }
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} manuals into a rail and bails!", name);
                            break;
                        }
                        balance += tilt;
                        combo += ManualContinue;
                        report += string.Format("{0} keeps manualing!", name);
                        break;

                    case Action.Kickflip:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} tries to kickflip into the floor and bails!", name);
                            break;
                        }
                        combo += KickflipCombo;
                        multiplier += KickflipMultiplier;
                        state = State.Skating;
                        report += string.Format("{0} does a kickflip from a manual!", name);
                        break;

                    case Action.Grab:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} tries to do a grab into the floor and bails!", name);
                            break;
                        }
                        combo += GrabCombo;
                        multiplier += GrabMultiplier;
                        state = State.Grabbing;
                        report += string.Format("{0} starts a grab from a manual!", name);
                        break;
                }

                if (balance < -1 && state == State.Manualing)
                    bail = true;
                if (balance > +1 && state == State.Manualing)
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
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Forward:
                        state = State.Skating;
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Backward:
                        state = State.Skating;
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Left:
                        state = State.Skating;
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Right:
                        state = State.Skating;
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Jump:
                        state = State.Skating;
                        if (height == 0)
                        {
                            if (area.current == Surface.Land)
                            {
                                height += JumpLand;
                                report += string.Format("{0} lands grab trick and does an ollie!", name);
                            }
                            if (area.current == Surface.Rail)
                            {
                                height += JumpLand;
                                report += string.Format("{0} lands grab trick and does an ollie!", name);
                            }
                            if (area.current == Surface.Kicker)
                            {
                                height += JumpKicker;
                                report += string.Format("{0} lands grab trick and jumps off a kicker!", name);
                            }
                            if (area.current == Surface.Ramp)
                            {
                                height += JumpRamp;
                                report += string.Format("{0} lands grab trick and jumps off a ramp!", name);
                            }
                        }
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Grind:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            state = State.Grinding;
                            balance = 0;
                            combo += GrindCombo;
                            multiplier += GrindMultiplier;
                            report += string.Format("{0} ends grab trick into a rail grind!", name);
                            break;
                        }

                        state = State.Skating;
                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Manual:
                        if (height == 0 && area.current == Surface.Rail)
                        {
                            bail = true;
                            report += string.Format("{0} ends grab trick and tries to manual into a rail and bails.", name);
                            break;
                        }

                        if (height == 0)
                        {
                            state = State.Manualing;
                            combo += ManualCombo;
                            multiplier += ManualMultiplier;
                            report += string.Format("{0} ends grab trick lands into a manual!", name);
                            break;
                        }

                        report += string.Format("{0} ends grab trick.", name);
                        break;

                    case Action.Kickflip:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} ends grab trick tries to kickflip into the ground and bails!", name);
                            break;
                        }

                        state = State.Skating;
                        combo += KickflipCombo;
                        multiplier += KickflipCombo;
                        report += string.Format("{0} ends grab trick and does a kickflip!", name);
                        break;

                    case Action.Grab:
                        if (height == 0)
                        {
                            bail = true;
                            report += string.Format("{0} tries to hold grab trick into the floor and bails!", name);
                            break;
                        }
                        combo += GrabContinue;
                        report += string.Format("{0} keeps holding grab!", name);
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
        }

        public void Land()
        {
            score += combo * multiplier;

            if (combo > 0)
                report += string.Format("\n\n *** Landed for {0} points! ***\n", combo * multiplier);
            else
                report += string.Format("\nLanded on {0}.", area.current);

            combo = 0;
            multiplier = 0;
            state = State.Skating;
            balance = 0;
        }

        public void Bail()
        {
            report += string.Format("\n\n*** {0} BAILED! ***\n", name);

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

        private static Surface[] NextLand = new Surface[] { Surface.Land, Surface.Land, Surface.Land, Surface.Land, Surface.Rail, Surface.Rail, Surface.Kicker, Surface.Kicker, Surface.Ramp, };
        private static Surface[] NextRail = new Surface[] { Surface.Land, Surface.Land, Surface.Rail, Surface.Rail, Surface.Rail, Surface.Rail, Surface.Kicker, Surface.Ramp, };
        private static Surface[] NextKicker = new Surface[] { Surface.Land, };
        private static Surface[] NextRamp = new Surface[] { Surface.Land, };

        public Area()
        {
            random = new Random();

            previous = Surface.Land;
            current = Surface.Land;
            ahead = RandomSurface();
            left = RandomSurface();
            right = RandomSurface();
        }

        public Surface NextSurface(Surface surface)
        {
            var pool = NextLand;

            switch (surface)
            {
                case Surface.Land: pool = NextLand; break;
                case Surface.Rail: pool = NextRail; break;
                case Surface.Kicker: pool = NextKicker; break;
                case Surface.Ramp: pool = NextRail; break;
            }

            var index = random.Next() % pool.Length;
            var result = pool[index];

            return result;
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
                    left = RandomSurface();
                    right = RandomSurface();
                    ahead = NextSurface(current);
                    break;

                case -1:
                    previous = current;
                    current = left;
                    left = RandomSurface();
                    right = RandomSurface();
                    ahead = NextSurface(current);
                    break;

                case +1:
                    previous = current;
                    current = right;
                    left = RandomSurface();
                    right = RandomSurface();
                    ahead = NextSurface(current);
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

            skater.name = "Tony Text";

            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("");

                skater.PrintSkaterConsole();

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

        public static Action DiscordGetVote()
        {
            Console.WriteLine("Vote: [WASD] move, [Space] jump");
            Console.WriteLine("Vote: [R] grind, [M] manual [G] grab [K] kickflip");

            var key = Console.ReadKey(true);
            var action = Action.None;
            var exit = false;

            switch (key.Key)
            {
                case ConsoleKey.Escape: exit = true; break;
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

            Console.WriteLine(" *");
            Console.WriteLine(" * Winning Vote: {0}", action);
            Console.WriteLine(" *");

            return action;
        }

        public static void DiscordWrapper(string[] args)
        {
            var name = "Tony Text";

            if (args.Length > 0)
                name = args[0];

            var message = "";

            message += String.Format("\n```");
            message += String.Format("\n SKATE TIME ");
            message += String.Format("\n");
            message += String.Format("\n   _0_    ");
            message += String.Format("\n    |     ");
            message += String.Format("\n   / 7    ");
            message += String.Format("\n -------  ");
            message += String.Format("\n  o   o   ");
            message += String.Format("\n```");

            Discord.DiscordSetMessage(message);
            Discord.DiscordWaitCycle();

            message = string.Format("Skater {0} kicks off!", name);

            Discord.DiscordSetMessage(message);
            Discord.DiscordWaitCycle();

            var skater = new Skater();

            skater.name = name;

            while (true)
            {
                message = skater.DiscordStatus();

                Discord.DiscordSetMessage(message);
                Discord.DiscordSetReactions();
                Discord.DiscordWaitCycle();

                var action = Discord.DiscordGetVote();

                skater.Act(action);

                if (!String.IsNullOrWhiteSpace(skater.report))
                {
                    Discord.DiscordSetMessage(skater.report);
                    Discord.DiscordWaitCycle();
                }
            }

            Discord.DiscordSetMessage("Finished!");
        }
    }
}
