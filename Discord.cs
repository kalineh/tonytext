using System;

namespace tonytext
{
    class Discord
    {
        private static int nextCycle;

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
}
