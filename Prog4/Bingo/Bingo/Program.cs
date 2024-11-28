/* Name: Tyler Arista
 * ID: tja9
 * Course: CS212
 * Professor: Plantinga
 * Project: Program 4(Dutch Bingo)
 * Term: FALL 2022
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems)
                {
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ", name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges)
                {
                    Console.Write("{0} ", e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show all orphans
        private static void ShowOrphans()
        {
            Console.Write("Orphan(s): ");
            foreach (GraphNode n in rg.nodes)
            {
                if (n.GetEdges("hasParent").Count == 0)
                    Console.Write("\t" + n.Name);
            }
        }

        // Show a person's decendents
        private static void ShowDecendents(string name)
        {
            GraphNode root = rg.GetNode(name);
            List<GraphNode> DecendentNodes = new List<GraphNode>();
            DecendentNodes.Add(root);
            List<GraphNode> nextDecendentNodes = new List<GraphNode>();
            // marking generations
            int generation = 0;
            List<string> Children = new List<string>();
            try
            {
                if (root.GetEdges("hasChild").Count == 0 && root != null)
                {
                    Console.Write(name + " has no decendents");
                }
                else if (root != null)
                {
                    Console.Write(name + "'s decendents\n");
                    while (DecendentNodes.Count > 0)
                    {
                        Children = new List<string>();
                        foreach (GraphNode n in DecendentNodes)
                        {
                            foreach (GraphEdge e in n.GetEdges("hasChild"))
                            {
                                nextDecendentNodes.Add(e.ToNode());
                                Children.Add(e.To());
                            }
                        }
                        if (generation == 0 && Children.Count > 0)
                        {
                            Console.Write("Children: ");
                            Console.WriteLine(String.Join(", ", (String[])Children.ToArray()));
                        }
                        else if (generation > 0 && Children.Count > 0)
                        {
                            for (int i = generation; i > 1; i--)
                            {
                                Console.Write("Great ");
                            }
                            Console.Write("Grandchildren: ");
                            Console.WriteLine(String.Join(", ", (String[])Children.ToArray()));
                        }
                        DecendentNodes = nextDecendentNodes;
                        nextDecendentNodes = new List<GraphNode>();
                        generation++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read\n");
                Console.WriteLine(e.Message);
            }
        }

        // show all of the person's nth-cousins k times removed
        private static void ShowCousins(string name, int n, int k)
        {
            GraphNode person = rg.GetNode(name);
            if (person == null)
            {
                Console.Write(name + " not found");
            }
            else
            {
                Console.Write(name + "'s " + n + "th-cousins " + k + " times removed\n");
                Dictionary<GraphNode, bool> UpperGeneration = new Dictionary<GraphNode, bool>();
                Dictionary<GraphNode, bool> LowerGeneration = new Dictionary<GraphNode, bool>();
                Dictionary<string, bool> check = new Dictionary<string, bool>();
                UpperGeneration.Add(person, true);
                for (int i = 0; i < n + k + 1; i++)
                {
                    Dictionary<GraphNode, bool> generation = new Dictionary<GraphNode, bool>();
                    foreach (GraphNode gn in UpperGeneration.Keys)
                    {
                        foreach (GraphEdge e in gn.GetEdges("hasParent"))
                        {
                            if (!generation.ContainsKey(e.ToNode()))
                            {
                                generation.Add(e.ToNode(), true);
                            }
                            if (i < n)
                            {
                                check.Add(e.To(), true);
                            }
                        }
                    }
                    if (i == n && k != 0)
                    {
                        LowerGeneration = generation;
                    }
                    UpperGeneration = generation;
                }
                for (int i = 0; i < n + 1; i++)
                {
                    Dictionary<GraphNode, bool> generation = new Dictionary<GraphNode, bool>();
                    foreach (GraphNode gn in UpperGeneration.Keys)
                    {
                        if (check.ContainsKey(gn.GetName()))
                        {
                            continue;
                        }
                        else
                        {
                            check.Add(gn.GetName(), true);
                        }
                        foreach (GraphEdge e in gn.GetEdges("hasChild"))
                        {
                            if (!generation.ContainsKey(e.ToNode()))
                            {
                                generation.Add(e.ToNode(), true);
                            }
                        }
                    }
                    UpperGeneration = generation;
                }
                for (int i = 0; i < n + k + 1; i++)
                {
                    Dictionary<GraphNode, bool> generation = new Dictionary<GraphNode, bool>();
                    foreach (GraphNode gn in LowerGeneration.Keys)
                    {
                        if (check.ContainsKey(gn.GetName()))
                        {
                            continue;
                        }
                        else
                        {
                            check.Add(gn.GetName(), true);
                        }
                        foreach (GraphEdge e in gn.GetEdges("hasChild"))
                        {
                            if (!generation.ContainsKey(e.ToNode()))
                            {
                                generation.Add(e.ToNode(), true);
                            }
                        }
                    }
                    LowerGeneration = generation;
                }
                List<string> results = new List<string>();
                foreach (GraphNode gn in UpperGeneration.Keys)
                {
                    if (!check.ContainsKey(gn.GetName()))
                    {
                        results.Add(gn.GetName());
                    }
                }
                foreach (GraphNode gn in LowerGeneration.Keys)
                {
                    if (!check.ContainsKey(gn.GetName()))
                    {
                        results.Add(gn.GetName());
                    }
                }
                if (results.Count == 0)
                {
                    Console.WriteLine("not found");
                }
                else
                {
                    Console.WriteLine(String.Join(", ", (String[])results.ToArray()));
                }
            }
        }

        // Show the shortest path between two persons
        private static List<GraphNode> ShowBingo(string F, string T)
        {
            GraphNode From = rg.GetNode(F);
            GraphNode To = rg.GetNode(T);
            if (From == null || To == null || From == To)
            {
                return null;
            }
            else
            {
                List<GraphNode> path = new List<GraphNode>();
                List<List<GraphNode>> levels = new List<List<GraphNode>>();
                List<GraphNode> level = new List<GraphNode>();
                Dictionary<string, bool> check = new Dictionary<string, bool>();
                level.Add(From);
                levels.Add(level);
                int levelCount = 0;
                while (true)
                {
                    level = new List<GraphNode>();
                    foreach (GraphNode n in levels[levels.Count - 1])
                    {
                        foreach (GraphEdge e in n.GetEdges())
                        {
                            if (e.To() == T)
                            {
                                goto EndBuild;
                            }
                            if (!check.ContainsKey(e.To()))
                            {
                                level.Add(e.ToNode());
                                check.Add(e.To(), true);
                            }
                        }
                    }
                    levels.Add(level);
                }
            EndBuild:
                path.Add(To);
                levelCount = levels.Count - 1;
                while (path[path.Count - 1] != From && levelCount >= 0)
                {
                    foreach (GraphEdge e in path[path.Count - 1].GetEdges())
                    {
                        if (levels[levelCount].Contains(e.ToNode()))
                        {
                            path.Add(e.ToNode());
                            goto NextInWhile;
                        }
                    }
                    foreach (GraphNode n in levels[levelCount])
                    {
                        foreach (GraphEdge e in n.GetEdges())
                        {
                            if (e.ToNode() == path[path.Count - 1])
                            {
                                path.Add(n);
                                goto NextInWhile;
                            }
                        }
                    }
                NextInWhile:
                    levelCount--;
                }
                path.Reverse();
                if (path.Count == 1)
                {
                    path = null;
                }
                return path;
            }
        }

        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                // show the person's friends
                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // list all the people with no parents
                else if (command == "orphans")
                    ShowOrphans();

                // find a shortest chain of relationships between two persons
                else if (command == "bingo" && commandWords.Length > 1)
                {
                    try
                    {
                        if (rg.GetNode(commandWords[1]) != null && rg.GetNode(commandWords[2]) != null)
                        {
                            List<GraphNode> path = ShowBingo(commandWords[1], commandWords[2]);

                            for (int i = 1; i < path.Count; i++)
                            {
                                foreach (GraphEdge e in path[i - 1].GetEdges())
                                {
                                    if (e.ToNode() == path[i])
                                        Console.WriteLine(path[i - 1].GetName() + " is a " + e.GetLabel().Substring(3) + " of " + path[i].GetName());
                                }
                            }
                        }
                        else
                            Console.WriteLine(commandWords[1] + " has no relation to " + commandWords[2]);
                    }
                    catch (Exception e)
                    {
                        Console.Write("Unable to read\n");
                        Console.WriteLine(e.Message);
                    }
                }

                // show all of the person's decendents
                else if (command == "decendents" && commandWords.Length > 1)
                {
                    ShowDecendents(commandWords[1]);
                }

                // show all of the person's nth-cousins k times removed
                else if (command == "cousins" && commandWords.Length > 1)
                {
                    try
                    {
                        ShowCousins(commandWords[1], Int32.Parse(commandWords[2]), Int32.Parse(commandWords[3]));
                    }
                    catch (Exception e)
                    {
                        Console.Write("Unable to read\n");
                        Console.WriteLine(e.Message);
                    }
                }

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname],\n  friends [personname], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
