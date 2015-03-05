using System;
using System.Collections.Generic;

#if Full_API
using Terraria;
#endif
namespace tdsm.api.Command
{
    public class CommandError : ApplicationException
    {
        public CommandError(string message) : base(message) { }
        public CommandError(string fmt, params object[] args) : base(String.Format(fmt, args)) { }
    }

    public class ArgumentList : List<string>
    {
        public object Plugin { get; set; }

        public ArgumentList() { }

        static readonly Dictionary<System.Type, string> typeNames = new Dictionary<System.Type, string>()
		{
			{ typeof(string),   "a string" },
			{ typeof(int),      "an integer number" },
			{ typeof(double),   "a number" },
			{ typeof(bool),     "a boolean value" },
			#if Full_API
			{ typeof(Player),   "an online player's name" },
			#endif
			{ typeof(TimeSpan), "a duration" },
		};

        static readonly Dictionary<string, bool> booleanValues = new Dictionary<string, bool>() {
			{ "true", true }, { "yes", true }, { "+", true }, { "1", true },
			{ "enable", true }, { "enabled", true }, { "on", true },
			
			{ "false", false }, { "no", false }, { "-", false }, { "0", false },
			{ "disable", false }, { "disabled", false }, { "off", false },
		};

        public string GetString(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            return this[at];
        }

        public bool TryGetString(int at, out string val)
        {
            val = null;

            if (at >= Count) return false;

            val = this[at];

            return true;
        }

        public int GetInt(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            int val;
            if (Int32.TryParse(this[at], out val))
            {
                return val;
            }

            throw new CommandError("An integer number was expected for argument {0}.", at + 1);
        }

        public bool TryGetInt(int at, out int val)
        {
            val = -1;

            if (at >= Count) return false;

            return Int32.TryParse(this[at], out val);
        }

        public double GetDouble(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            double val;
            if (Double.TryParse(this[at], out val))
            {
                return val;
            }

            throw new CommandError("A number was expected for argument {0}.", at + 1);
        }

        public bool TryGetDouble(int at, out double val)
        {
            val = -1;

            if (at >= Count) return false;

            return Double.TryParse(this[at], out val);
        }

        public TimeSpan GetDuration(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            TimeSpan val;
            if (TryGetDuration(at, out val))
                return val;

            throw new CommandError("A duration was expected for argument {0}.", at + 1);
        }

        public bool TryGetDuration(int at, out TimeSpan val)
        // TODO: Add support for composite duration literals, ie: 4h30m15s
        {
            val = TimeSpan.FromSeconds(0);

            if (at >= Count) return false;

            double value;
            double scale = 1.0;
            var str = this[at];

            if (str.EndsWith("ms"))
            {
                scale = 1e-3;
                str = str.Substring(0, str.Length - 2);
            }
            else if (str.EndsWith("us"))
            {
                scale = 1e-6;
                str = str.Substring(0, str.Length - 2);
            }
            else if (str.EndsWith("s"))
            {
                scale = 1.0;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.EndsWith("m"))
            {
                scale = 60.0;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.EndsWith("h"))
            {
                scale = 3600.0;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.EndsWith("d"))
            {
                scale = 24 * 3600.0;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.EndsWith("mo"))
            {
                scale = 30 * 24 * 3600.0;
                str = str.Substring(0, str.Length - 2);
            }
            else if (str.EndsWith("y"))
            {
                scale = 365 * 24 * 3600.0;
                str = str.Substring(0, str.Length - 1);
            }
            else if (str.EndsWith("yr"))
            {
                scale = 365 * 24 * 3600.0;
                str = str.Substring(0, str.Length - 2);
            }

            if (Double.TryParse(str, out value))
            {
                val = TimeSpan.FromSeconds(value * scale);
                return true;
            }

            return false;
        }

        public bool GetBool(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            var lower = this[at].ToLower();
            bool val;
            if (booleanValues.TryGetValue(lower, out val))
            {
                return val;
            }

            throw new CommandError("An boolean value was expected for argument {0}.", at + 1);
        }

        public bool TryGetBool(int at, out bool val)
        {
            val = false;

            if (at >= Count) return false;

            var lower = this[at].ToLower();
            return booleanValues.TryGetValue(lower, out val);
        }

#if Full_API
        public Player GetOnlinePlayer(int at)
        {
            if (at >= Count) throw new CommandError("Too few arguments given.");

            Player player = Tools.GetPlayerByName(this[at]);

            if (player != null)
                return player;

            var matches = Tools.FindPlayerByPart(this[at]);
            if (matches.Count == 1)
            {
                return matches.ToArray()[0];
            }

            throw new CommandError("A connected player's name was expected for argument {0}.", at + 1);
        }

        public bool TryGetOnlinePlayer(int at, out Player val)
        {
            val = null;

            if (at >= Count) return false;

            val = Tools.GetPlayerByName(this[at]);

            if (val != null)
                return true;

            var matches = Tools.FindPlayerByPart(this[at]);
            if (matches.Count == 1)
            {
                val = matches.ToArray()[0];
                return true;
            }

            return false;
        }
#endif

        bool TryParseAt<T>(int at, out T t)
        {
            t = default(T);

            if (typeof(T) == typeof(string))
            {
                string val;
                if (TryGetString(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;

            }
            else if (typeof(T) == typeof(int))
            {
                int val;
                if (TryGetInt(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(bool))
            {
                bool val;
                if (TryGetBool(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;
            }
#if Full_API
            else if (typeof(T) == typeof(Player))
            {
                Player val;
                if (TryGetOnlinePlayer(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;
            }
#endif
            else if (typeof(T) == typeof(double))
            {
                double val;
                if (TryGetDouble(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                TimeSpan val;
                if (TryGetDuration(at, out val))
                {
                    t = (T)(object)val;
                    return true;
                }
                return false;
            }
            else if (typeof(T) == typeof(WorldTime))
            {
                var val = WorldTime.Parse(this[at]);
                if (val != null)
                {
                    var gt = val.Value.GameTime;
                    if (gt >= WorldTime.TimeMin && gt <= WorldTime.TimeMax)
                    {
                        t = (T)(object)val.Value;
                        return true;
                    }
                    else throw new CommandError("Invalid time.");
                }
                return false;
            }

            throw new CommandError("Internal command error, type is unsupported by parser: {0}.", typeof(T).ToString());
        }

        public bool TryParseOne<T>(out T t)
        {
            t = default(T);

            if (Count != 1) return false;

            return TryParseAt(0, out t);
        }

        public bool TryParseOne<T>(string literal1, out T t, string literal2 = null)
        {
            t = default(T);
            int start = (literal1 != null ? 1 : 0);
            int args = 1 + start + (literal2 != null ? 1 : 0);

            if (Count != args) return false;

            if (literal1 != null && this[0] != literal1)
                return false;

            if (literal2 != null && this[start + 1] != literal2)
                return false;

            return TryParseAt(start, out t);
        }

        public bool TryParseTwo<T, U>(out T t, out U u)
        {
            t = default(T);
            u = default(U);

            if (Count != 2) return false;

            return TryParseAt(0, out t) && TryParseAt(1, out u);
        }

        public bool TryParseTwo<T, U>(string literal1, out T t, string literal2, out U u, string literal3 = null)
        {
            t = default(T);
            u = default(U);
            int arg1 = (literal1 != null ? 1 : 0);
            int arg2 = arg1 + (literal2 != null ? 1 : 0) + 1;
            int args = 2 + arg1 + (literal2 != null ? 1 : 0) + (literal3 != null ? 1 : 0);

            if (Count != args) return false;

            if (literal1 != null && this[0] != literal1)
                return false;

            if (literal2 != null && this[arg1 + 1] != literal2)
                return false;

            if (literal3 != null && this[arg2 + 1] != literal3)
                return false;

            return TryParseAt(arg1, out t) && TryParseAt(arg2, out u);
        }

        public bool TryParseThree<T, U, V>(out T t, out U u, out V v)
        {
            t = default(T);
            u = default(U);
            v = default(V);

            if (Count != 3) return false;

            return TryParseAt(0, out t) && TryParseAt(1, out u) && TryParseAt(2, out v);
        }

        public bool TryParseThree<T, U, V>(string literal1, out T t, string literal2, out U u, string literal3, out V v, string literal4 = null)
        {
            t = default(T);
            u = default(U);
            v = default(V);
            int arg1 = (literal1 != null ? 1 : 0);
            int arg2 = arg1 + (literal2 != null ? 1 : 0) + 1;
            int arg3 = arg2 + (literal3 != null ? 1 : 0) + 1;
            int args = arg3 + (literal4 != null ? 1 : 0) + 1;

            if (Count != args) return false;

            if (literal1 != null && this[0] != literal1)
                return false;

            if (literal2 != null && this[arg1 + 1] != literal2)
                return false;

            if (literal3 != null && this[arg2 + 1] != literal3)
                return false;

            if (literal4 != null && this[arg3 + 1] != literal4)
                return false;

            return TryParseAt(arg1, out t) && TryParseAt(arg2, out u) && TryParseAt(arg3, out v);
        }

        public bool TryParseFour<T, U, V, W>(out T t, out U u, out V v, out W w)
        {
            t = default(T);
            u = default(U);
            v = default(V);
            w = default(W);

            if (Count != 4) return false;

            return TryParseAt(0, out t) && TryParseAt(1, out u) && TryParseAt(2, out v) && TryParseAt(3, out w);
        }

        public void ParseNone()
        {
            if (Count != 0)
                throw new CommandError("No arguments expected.");
        }

        public void ParseOne<T>(out T t)
        {
            if (!TryParseOne<T>(out t))
                throw new CommandError("A single argument expected: {0}.", typeNames[typeof(T)]);
        }

        public void ParseOne<T>(string literal1, out T t, string literal2 = null)
        {
            if (!TryParseOne<T>(literal1, out t, literal2))
                throw new CommandError("Expected syntax: {0}<{1}>{2} .",
                    (literal1 != null ? literal1 + " " : String.Empty),
                    typeNames[typeof(T)],
                    (literal2 != null ? " " + literal2 : String.Empty));
        }

        public void ParseTwo<T, U>(out T t, out U u)
        {
            if (!TryParseTwo<T, U>(out t, out u))
                throw new CommandError("Two arguments expected: {0} and {1}.", typeNames[typeof(T)], typeNames[typeof(U)]);
        }

        public void ParseTwo<T, U>(string literal1, out T t, string literal2, out U u, string literal3 = null)
        {
            if (!TryParseTwo<T, U>(literal1, out t, literal2, out u, literal3))
                throw new CommandError("Expected syntax: {0}<{1}>{2} <{3}>{4} .",
                    (literal1 != null ? literal1 + " " : String.Empty),
                    typeNames[typeof(T)],
                    (literal2 != null ? " " + literal2 : String.Empty),
                    typeNames[typeof(U)],
                    (literal3 != null ? " " + literal3 : String.Empty));
        }

        public void ParseTwo<T, U>(out T t, string literal2, out U u, string literal3 = null)
        {
            ParseTwo(null, out t, literal2, out u, literal3);
        }

        public void ParseThree<T, U, V>(out T t, out U u, out V v)
        {
            if (!TryParseThree<T, U, V>(out t, out u, out v))
                throw new CommandError("Three arguments expected: {0}, {1} and {2}.",
                    typeNames[typeof(T)], typeNames[typeof(U)], typeNames[typeof(V)]);
        }

        public void ParseThree<T, U, V>(string literal1, out T t, string literal2, out U u, string literal3, out V v, string literal4 = null)
        {
            if (!TryParseThree<T, U, V>(literal1, out t, literal2, out u, literal3, out v, literal4))
                throw new CommandError("Expected syntax: {0}<{1}>{2} <{3}>{4} <{5}>{6} .",
                    (literal1 != null ? literal1 + " " : String.Empty),
                    typeNames[typeof(T)],
                    (literal2 != null ? " " + literal2 : String.Empty),
                    typeNames[typeof(U)],
                    (literal3 != null ? " " + literal3 : String.Empty),
                    typeNames[typeof(V)],
                    (literal4 != null ? " " + literal4 : String.Empty));
        }

        public void ParseThree<T, U, V>(out T t, string literal2, out U u, string literal3, out V v, string literal4 = null)
        {
            ParseThree(null, out t, literal2, out u, literal3, out v, literal4);
        }

        public void ParseFour<T, U, V, W>(out T t, out U u, out V v, out W w)
        {
            if (!TryParseFour<T, U, V, W>(out t, out u, out v, out w))
                throw new CommandError("Four arguments expected: {0}, {1}, {2} and {3}.",
                    typeNames[typeof(T)], typeNames[typeof(U)], typeNames[typeof(V)], typeNames[typeof(W)]);
        }

        public bool TryPop(string literal)
        {
            if (Count < 1) return false;

            if (this[0] == literal)
            {
                this.RemoveAt(0);
                return true;
            }

            return false;
        }

        public bool TryPopOne<T>(out T t)
        {
            t = default(T);

            if (Count < 1) return false;

            if (TryParseAt(0, out t))
            {
                this.RemoveAt(0);
                return true;
            }

            return false;
        }

        public bool TryPopOne<T>(string literal1, out T t, string literal2 = null)
        {
            t = default(T);
            int start = (literal1 != null ? 1 : 0);
            int args = 1 + start + (literal2 != null ? 1 : 0);

            if (Count < args) return false;

            if (literal1 != null && this[0] != literal1)
                return false;

            if (literal2 != null && this[start + 1] != literal2)
                return false;

            if (TryParseAt(start, out t))
            {
                this.RemoveRange(0, args);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Searched through the arguments for a match, then removes both the literal and the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="literal"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool TryPopAny<T>(string literal, out T t)
        {
            t = default(T);

            for (var i = 0; i < Count; i++)
            {
                if (this[i] == literal && TryParseAt(i + 1, out t))
                {
                    RemoveRange(i, 2);
                    return true;
                }
            }

            return false;
        }
    }

    public struct WorldTime
    {
        public const double TimeMax = 86400;
        public const double TimeMin = 0;

        public byte Hour { get; set; }
        public byte Minute { get; set; }
        public bool AM { get; set; }

        public double GameTime
        {
            get
            {
                var time = ((this.Hour * 60.0 * 60.0) + (this.Minute * 60.0));

                if (!this.AM)
                    time += 12.0 * 60.0 * 60.0;

                time -= 4.5 * 60.0 * 60.0; //Terrarian time (0) starts at 4:30 am
                if (time < 0) time = TimeMax + time;

                return time;
            }
        }

        /// <summary>
        /// Parses time in the format of HH:mm[am|pm]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static WorldTime? Parse(string input)
        {
            var split = input.Split(':');
            if (split.Length == 2)
            {
                byte hour, minute;
                if (Byte.TryParse(split[0], out hour) && split[1].Length > 2 && hour < 13)
                {
                    if (Byte.TryParse(split[1].Substring(0, split[1].Length - 2), out minute) && minute < 60)
                    {
                        var tk = split[1].Remove(0, split[1].Length - 2);

                        return new WorldTime()
                        {
                            Hour = hour,
                            Minute = minute,
                            AM = tk.ToLower() == "am"
                        };
                    }
                }
            }
            return null;
        }

        public static WorldTime? Parse(double time)
        {
            time += 4.5 * 60.0 * 60.0; //Push back up to real time

            bool am = false;
            if (time >= TimeMax)
            {
                time -= TimeMax;
                am = true;
            }
            else am = time <= 12.0 * 60.0 * 60.0;


            var hour = (int)(time / 60.0 / 60.0);
            var min = (int)((time - (hour * 60.0 * 60.0)) / 60.0);

            if (hour == 0) hour = 12;
            if (hour > 12) hour -= 12;

            return new WorldTime()
            {
                Hour = (byte)hour,
                Minute = (byte)min,
                AM = am
            };
        }

        public override string ToString()
        {
            return String.Format("{0}:{1:00} {2}", Hour, Minute, AM ? "AM" : "PM");
        }
    }
}