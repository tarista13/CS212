﻿/* Name: Tyler Arista
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

namespace Bingo
{
    /// <summary>
    /// Represents a labeled, directed edge in a RelationshipGraph
    /// </summary>
    class GraphEdge
    {
        public string Label { get; private set; }
        private GraphNode fromNode, toNode;

        // constructor
        public GraphEdge(GraphNode from, GraphNode to, string myLabel)
        {
            fromNode = from;
            toNode = to;
            Label = myLabel;
        }

        // return the name of the "to" person in the relationship
        public string To()
        {
            return toNode.Name;
        }

        // return a toNode
        public GraphNode ToNode()
        {
            return toNode;
        }

        // return string form of edge
        public override string ToString()
        {
            string result = fromNode.Name + " --(" + Label + ")--> " + toNode.Name;
            return result;
        }

        // return a label
        public string GetLabel()
        {
            return Label;
        }
    }
}

