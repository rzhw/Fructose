﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IronRuby;
using IronRuby.Compiler;
using IronRuby.Builtins;
using IronRuby.Compiler.Ast;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;

namespace Fructose
{
    public class Parser
    {
        SourceUnitTree ast;
        ScriptSource source;
        ErrorSink errorSink;

        string sourcecode;

        public Parser(string Source) : this(Source, ErrorSink.Default) { }
        public Parser(string Source, ErrorSink errorSink)
        {
            source = Ruby.CreateRuntime().GetEngine("rb").CreateScriptSourceFromString(Source);
            this.errorSink = errorSink;

            sourcecode = Source;
        }

        public void Parse()
        {
            var srcunit = HostingHelpers.GetSourceUnit(source);
            var parser = new IronRuby.Compiler.Parser(errorSink);
            ast = parser.Parse(srcunit, new RubyCompilerOptions(), errorSink);
        }

        public string CompileToPHP()
        {
            var compiler = new Compiler.Compiler(ast);
            return compiler.Compile();
        }
    }
}