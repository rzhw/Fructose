﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby.Compiler.Ast;

namespace Fructose.Compiler
{
    public class Compiler
    {
        public const string IndentSpacing = "    ";
        static Dictionary<NodeTypes, AstNodeGenerator> generators;
        static Compiler()
        {
            generators = (from type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                          where type.IsSubclassOf(typeof(AstNodeGenerator))
                          select new
                          {
                              v = (AstNodeGenerator)Activator.CreateInstance(type),
                              k = type.GetCustomAttributes(false).OfType<GeneratorAttribute>().Single().NodeType
                          })
                         .ToDictionary(pair => pair.k, pair => pair.v);
        }

        int indentLevel = 0;
        StringBuilder sb = new StringBuilder();
        public SourceUnitTree tree { get; private set; }
        public Compiler(SourceUnitTree tree)
        {
            this.tree = tree;
        }

        internal void AppendLine(string fmt, params string[] args)
        {
            for (int i = 0; i < indentLevel; i++)
                sb.Append(IndentSpacing);

            if (args.Length > 0)
                sb.AppendFormat(fmt + "\n", args);
            else
                sb.AppendLine(fmt);
        }

        internal void Indent()
        {
            indentLevel++;
        }
        internal void Dedent()
        {
            indentLevel--;
        }

        public string Compile()
        {
            sb.AppendLine("<?php include('libfructose.php');\n$_stack = array();\n\n");

            foreach (var stmt in tree.Statements)
                CompileNode(stmt);

            return sb.ToString();
        }

        public void CompileNode(Node node)
        {
            CompileNode(node, NodeParent.Terminator);
        }
        public void CompileNode(Node node, NodeParent parents)
        {
            if (!generators.ContainsKey(node.NodeType))
                throw new NotImplementedException("NodeType " + node.NodeType + " not supported yet");

            generators[node.NodeType].Compile(this, node, parents);
        }
    }
}