using System;
using System.Collections.Generic;
using System.IO;

namespace LearnBot
{
    class DataBase {
        public List<Record> data = new List<Record>();
        public void Add(Record record) {
            if (hasRecord(record))
            {
                data[getRecord(record)].count++;
            }
            else {
                data.Add(record);
            }
        }
        public void removeRecord(string input, string react)
        {
            int i = 0;
            while (i < data.Count)
            {
                if (data[i].input == input && data[i].react == react)
                {
                    if (data[i].count == 1)
                    {
                        data.RemoveAt(i);
                    }
                    else {
                        data[i].count--;
                    }
                    i = data.Count;
                }
                else
                {
                    i++;
                }
            }
        }
        public void deleteRecord(string input, string react) {
            int i = 0;
            while (i<data.Count)
            {
                if (data[i].input == input && data[i].react == react)
                {
                    data.RemoveAt(i);
                }
                else {
                    i++;
                }
            }
        }
        public bool hasRecord(Record record)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].react == record.react && data[i].input == record.input) {
                    return true;
                }
            }
            return false;
        }
        public int getRecord(Record record) {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].react == record.react && data[i].input == record.input)
                {
                    return i;
                }
            }
            return -1;
        }
        public string getResponse(string[] input,bool show) {

            DataBase respondBase = new DataBase();
            for (int i = 0; i < input.Length; i++)
            {
                if (haveReact(input[i]))
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        if (data[j].input == input[i]) {
                            for (int k = 0; k < data[j].count; k++)
                            {
                                respondBase.Add(new Record(data[j].react, ""));
                            }
                        }
                    }
                    //respondBase.Add(new Record(data[findMax(input[i])].react, ""));
                }
            }
            if (respondBase.data.Count != 0) {
                int a = -1;
                int max = 0;
                for (int i = 0; i < respondBase.data.Count; i++)
                {
                    if (respondBase.data[i].count > max)
                    {
                        max = respondBase.data[i].count;
                        a = i;
                    }
                }
                if (show) {
                    Console.WriteLine("Respond\t\tPriority");
                    for (int i = 0; i < respondBase.data.Count; i++)
                    {
                        Console.Write(respondBase.data[i].input + "\t\t" + respondBase.data[i].count + "\n");
                    }
                }
                Console.Write("Response:");
                return respondBase.data[a].input;
            }
            return "I dont know";
        }
        private bool haveReact(string str) {
            for (int i = 0; i < data.Count; i++)
            {
                if(data[i].input == str){
                    return true;
                }
            }
            return false;
        }
        private int findMax(string str) {
            int max = 0;
            int a = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].input == str) {
                    if(data[i].count>max) {
                        a = i;
                        max = data[i].count;
                    }
                }
            }
            return a;
        }
    }
    class Record {
        public string input;
        public string react;
        public int count = 1;
        public Record(string input, string react) {
            this.input = input;
            this.react = react;
        }
        public Record(string input, string react, int count)
        {
            this.input = input;
            this.react = react;
            this.count = count;
        }
    }
    class Program
    {
        static DataBase database = new DataBase();
        static bool show = true;
        static bool showout = false;
        static string appendMode = "union";
        static Random r = new Random();

        static bool randBool() {
            if (r.Next(2) == 1) {
                return true;
            }
            return false;
        }
        static string removeSpaces(ref string str)
        {
            while (str.StartsWith(" "))
            {
                str = str.Substring(1);
            }
            while (str.EndsWith(" "))
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }


        static void Draw()
        {
            if (show)
            {
                Console.WriteLine("Input\t\tReact\t\tPriority");
                for (int i = 0; i < database.data.Count; i++)
                {
                    Console.Write(database.data[i].input + "\t\t" + database.data[i].react + "\t\t" + database.data[i].count + "\n");
                }
            }
        }
        static void Open() {
            Console.WriteLine("File?");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file = Console.ReadLine();
            removeSpaces(ref file);
            if (!file.EndsWith(".mind"))
            {
                file += ".mind";
            }
            if (file.StartsWith("\\"))
            {
                file = file.Substring(1, file.Length - 1);
            }
            StreamReader sr;
            try
            {
                sr = new StreamReader(path + "\\" + file.Replace("\\", "\\\\"));
                bool OK = true;
                if (database.data.Count != 0)
                {
                    Console.WriteLine("Append to current base? y/n");
                    ConsoleKey key = Console.ReadKey().Key;
                    if (key == ConsoleKey.N)
                    {
                        Console.WriteLine("\nMode: Clean open");
                    }
                    else if (key == ConsoleKey.Y)
                    {
                        Console.WriteLine("\nMode: append to current database");
                        Console.WriteLine("Append mode:" + appendMode);
                    }
                    Console.WriteLine("OK? y/n");
                    if (ConsoleKey.Y == Console.ReadKey().Key)
                    {
                        if (key == ConsoleKey.N)
                        {
                            database.data.Clear();
                        }
                    }
                    else
                    {
                        OK = false;
                    }

                }
                if (OK)
                {
                    while (!sr.EndOfStream)
                    {
                        string[] line = sr.ReadLine().Split(',');
                        if (appendMode == "union")
                        {
                            Record temp = new Record(line[0], line[1], int.Parse(line[2]));
                            if (!database.hasRecord(temp))
                            {
                                database.Add(temp);
                            }
                        }
                        else
                        {
                            database.Add(new Record(line[0], line[1], int.Parse(line[2])));
                        }
                    }
                }
                sr.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(file + " not found at: " + path);
                Console.ReadKey();
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
        static void Save() {
            Console.WriteLine("Save name?");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file = Console.ReadLine();
            removeSpaces(ref file);
            if (!file.EndsWith(".mind"))
            {
                file += ".mind";
            }
            StreamWriter sr = new StreamWriter(path + "\\" + file);
            for (int i = 0; i < database.data.Count; i++)
            {
                sr.WriteLine(database.data[i].input + "," + database.data[i].react + "," + database.data[i].count);
            }
            sr.Close();
        }
        static void SwitchAppendMode()
        {
            appendMode = Console.ReadLine().ToLower();
            while (!(appendMode == "union" || appendMode == "sum"))
            {
                Console.WriteLine("Must be \"union\" or \"sum\"");
                appendMode = Console.ReadLine().ToLower();
            }
        }

        static void GenTest() {
            Console.WriteLine("Generated file name?");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file = Console.ReadLine();
            removeSpaces(ref file);
            if (!file.EndsWith(".mind"))
            {
                file += ".mind";
            }
            StreamWriter sr = new StreamWriter(path + "\\" + file);
            DataBase generatedBase = new DataBase();
            int simRecords = 100;
            for (int i = 0; i < simRecords; i++)
            {
                //TODO hizzárendeláési szabály írása a test-re 
                string In = "";
                string Out = "";
                switch (r.Next(3))
                {
                    case 0:
                        In = "Kő";
                        break;
                    case 1:
                        In = "Papír";
                        break;
                    case 2:
                        In = "Olló";
                        break;
                    default:
                        break;
                }
                if (In == "Kő") {
                    if (r.Next(100) > 30)
                    {
                        Out = "Papír";
                    }
                    else {
                        if (randBool()) {
                            Out = "Kő";
                        }
                        else {
                            Out = "Olló";
                        }
                    }
                }
                if (In == "Papír")
                {
                    if (r.Next(100) > 30)
                    {
                        Out = "Olló";
                    }
                    else
                    {
                        if (randBool())
                        {
                            Out = "Kő";
                        }
                        else
                        {
                            Out = "Papír";
                        }
                    }
                }
                if (In == "Olló")
                {
                    if (r.Next(100) > 30)
                    {
                        Out = "Kő";
                    }
                    else
                    {
                        if (randBool())
                        {
                            Out = "Papír";
                        }
                        else
                        {
                            Out = "Olló";
                        }
                    }
                }
                generatedBase.Add(new Record(In, Out));

            }

            for (int i = 0; i < generatedBase.data.Count; i++)
            {
                sr.WriteLine(generatedBase.data[i].input + "," + generatedBase.data[i].react + "," + generatedBase.data[i].count);
            }

            sr.Close();
            Console.WriteLine("File created as: " + file);
            Console.ReadKey();
        }
        static void ClearData() {
            Console.WriteLine("Are you sure? y/n");
            if (ConsoleKey.Y == Console.ReadKey().Key)
            {
                database.data.Clear();
            }
        }
        static void getData() {
            Console.WriteLine("Input:");
            string[] input = Console.ReadLine().Split(',');
            for (int i = 0; i < input.Length; i++)
            {
                removeSpaces(ref input[i]);
            }
            Console.WriteLine(database.getResponse(input, showout));
            Console.ReadKey();
        }

        static void removeRecord() {
            Console.WriteLine("Record:");
            Console.Write("input:"); string recordIn = Console.ReadLine();
            Console.Write("react:"); string recordRe = Console.ReadLine();
            removeSpaces(ref recordIn);
            removeSpaces(ref recordRe);
            database.removeRecord(recordIn, recordRe);
        }
        static void deleteRecord() {
            Console.WriteLine("Record:");
            Console.Write("input:"); string recordIn = Console.ReadLine();
            Console.Write("react:"); string recordRe = Console.ReadLine();
            Console.WriteLine("Are you sure? y/n");
            removeSpaces(ref recordIn);
            removeSpaces(ref recordRe);
            if (ConsoleKey.Y == Console.ReadKey().Key)
            {
                database.deleteRecord(recordIn, recordRe);
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Learn Bot";
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            string command = "";
            while(command!="quit") {

                Console.Clear();
                Console.WriteLine("NeuroTable:"+show+"\nThinkTable:"+showout+"\nAppend mode: "+appendMode.ToUpper()+"\n");
                Draw();
                Console.WriteLine("Write input or get to response:");
                command = Console.ReadLine();
                removeSpaces(ref command);

                if (command.ToLower() != "quit" &&  command!="") {
                    switch (command.ToLower())
                    {
                        case "get":
                            getData();
                            break;
                        case "append mode":
                            SwitchAppendMode();
                            break;
                        case "remove record":
                            removeRecord();
                            break;
                        case "delete record":
                            deleteRecord();
                            break;
                        case "show neuro":
                            show = !show;
                            break;
                        case "show think":
                            showout = !showout;
                            break;
                        case "save":
                            Save();
                            break;
                        case "open":
                            Open();
                            break;
                        case "clear data":
                            ClearData();
                            break;
                        case "generate":
                            GenTest();
                            break;
                        default:
                            string[] input = command.Split(',');
                            for (int i = 0; i < input.Length; i++)
                            {
                                removeSpaces(ref input[i]);
                            }
                            Console.Write("React:");
                            string[] react = Console.ReadLine().Split(',');
                            for (int i = 0; i < react.Length; i++)
                            {
                                removeSpaces(ref react[i]);
                                while (react[i] == "")
                                {

                                    Console.WriteLine("React can\'t be empty!");
                                    Console.Write("React:"); react[i] = Console.ReadLine();
                                }
                            }

                            for (int i = 0; i < input.Length; i++)
                            {
                                for (int j = 0; j < react.Length; j++)
                                {
                                    database.Add(new Record(input[i], react[j]));
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
