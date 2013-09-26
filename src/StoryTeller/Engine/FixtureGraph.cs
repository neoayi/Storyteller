﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuCore;
using StoryTeller.Execution;
using StoryTeller.Model;

namespace StoryTeller.Engine
{
    public class FixtureGraph
    {
        private static readonly Lazy<FixtureGraph> _current = new Lazy<FixtureGraph>(forAppDomain);

        private readonly Cache<string, Type> _fixtureTypes = new Cache<string,Type>(name => {
            throw new NonExistentFixtureException(name);
        });

        private readonly IList<Type> _systemTypes = new List<Type>();
        private static readonly Lazy<FixtureLibrary> _library ;

        static FixtureGraph()
        {
            _library = new Lazy<FixtureLibrary>(() => {
                var library = new FixtureLibrary();

                _current.Value._fixtureTypes.Each((key, type) => {
                    var fixtureStructure = library.FixtureFor(key);

                    try
                    {
                        var fixture = (IFixture)Activator.CreateInstance(type);
                        fixtureStructure.ReadFrom(fixture, library);    
                    }
                    catch (Exception ex)
                    {
                        fixtureStructure.LogError(ex);
                    }
                    
                });

                return library;
            });
        }

        public static FixtureLibrary Library
        {
            get { return _library.Value; }
        }

        public static FixtureGraph Current
        {
            get { return _current.Value; }
        }

        private FixtureGraph(IEnumerable<Assembly> assemblies)
        {
            var tasks = assemblies.Select(assem => Task.Factory.StartNew(() => new AssemblyResult(assem)));

            var results = tasks.Select(x => x.Result);

            // TODO -- blow up if duplicate fixture names

            results.SelectMany(x => x.FixtureTypes).Each(type => {
                _fixtureTypes[type.GetFixtureAlias()] = type;
            });

            _systemTypes.AddRange(results.SelectMany(x => x.SystemTypes));
        }


        public IFixture Build(string fixtureName)
        {
            return (IFixture) Activator.CreateInstance(_fixtureTypes[fixtureName]);
        }

        public IEnumerable<Type> SystemTypes
        {
            get { return _systemTypes; }
        }




        public static FixtureGraph ForAssemblies(params string[] names)
        {
            var assemblies = names.Select(Assembly.Load);
            return new FixtureGraph(assemblies);
        }

        private static FixtureGraph forAppDomain()
        {
            var assemblies = FixtureAssembly.FindApplicationAssemblies();

            return new FixtureGraph(assemblies);
        }
    }
}