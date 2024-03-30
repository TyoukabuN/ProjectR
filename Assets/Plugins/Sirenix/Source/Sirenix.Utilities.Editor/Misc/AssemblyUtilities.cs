//-----------------------------------------------------------------------
// <copyright file="AssemblyUtilities.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.Utilities
{
#pragma warning disable

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// A utility class for finding types in various asssembly.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public static class AssemblyUtilities
    {
        private static string[] userAssemblyPrefixes = new string[]
        {
            "Assembly-CSharp",
            "Assembly-UnityScript",
            "Assembly-Boo",
            "Assembly-CSharp-Editor",
            "Assembly-UnityScript-Editor",
            "Assembly-Boo-Editor",
        };
        private static string[] pluginAssemblyPrefixes = new string[]
        {
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp-Editor-firstpass",
            "Assembly-UnityScript-firstpass",
            "Assembly-UnityScript-Editor-firstpass",
            "Assembly-Boo-firstpass",
            "Assembly-Boo-Editor-firstpass",
        };

        private static readonly Dictionary<Assembly, bool> IsDynamicCache = new Dictionary<Assembly, bool>(ReferenceEqualityComparer<Assembly>.Default);
        private static readonly object IS_DYNAMIC_CACHE_LOCK = new object();
        private static readonly object ASSEMBLY_TYPE_FLAG_LOOKUP_LOCK = new object();


        private static Assembly unityEngineAssembly = typeof(UnityEngine.Object).Assembly;
        private static Assembly unityEditorAssembly = typeof(UnityEditor.Editor).Assembly;
        private static DirectoryInfo projectFolderDirectory;
        private static DirectoryInfo scriptAssembliesDirectory;
        private static Dictionary<Assembly, AssemblyTypeFlags> assemblyTypeFlagLookup = new Dictionary<Assembly, AssemblyTypeFlags>(100);

        static AssemblyUtilities()
        {
            var dataPath = Environment.CurrentDirectory.Replace("\\", "//").Replace("//", "/").TrimEnd('/') + "/Assets";
            var scriptAssembliesPath = Environment.CurrentDirectory.Replace("\\", "//").Replace("//", "/").TrimEnd('/') + "/Library/ScriptAssemblies";
            projectFolderDirectory = new DirectoryInfo(dataPath);
            scriptAssembliesDirectory = new DirectoryInfo(scriptAssembliesPath);
        }

        [Obsolete("Reload is no longer supported.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void Reload() { }

        /// <summary>
        /// Gets an <see cref="ImmutableList"/> of all assemblies in the current <see cref="System.AppDomain"/>.
        /// </summary>
        /// <returns>An <see cref="ImmutableList"/> of all assemblies in the current <see cref="AppDomain"/>.</returns>
        public static ImmutableList<Assembly> GetAllAssemblies()
        {
            return new ImmutableList<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Gets the <see cref="AssemblyTypeFlags"/> for a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The <see cref="AssemblyTypeFlags"/> for a given assembly.</returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is null.</exception>
        public static AssemblyTypeFlags GetAssemblyTypeFlag(this Assembly assembly)
        {
            if (assembly == null) throw new NullReferenceException("assembly");

            lock (ASSEMBLY_TYPE_FLAG_LOOKUP_LOCK)
            {
                AssemblyTypeFlags result;

                if (assemblyTypeFlagLookup.TryGetValue(assembly, out result) == false)
                {
                    result = GetAssemblyTypeFlagNoLookup(assembly);

                    assemblyTypeFlagLookup[assembly] = result;
                }

                return result;
            }
        }

        private static AssemblyTypeFlags GetAssemblyTypeFlagNoLookup(Assembly assembly)
        {
            AssemblyTypeFlags result;
            string path = assembly.GetAssemblyDirectory();
            string name = assembly.FullName.ToLower(CultureInfo.InvariantCulture);

            bool isInScriptAssemblies = false;
            bool isInProject = false;

            if (path != null && Directory.Exists(path))
            {
                var pathDir = new DirectoryInfo(path);

                isInScriptAssemblies = pathDir.FullName == scriptAssembliesDirectory.FullName;
                isInProject = projectFolderDirectory.HasSubDirectory(pathDir);
            }

            bool isUserScriptAssembly = name.StartsWithAnyOf(userAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);
            bool isPluginScriptAssembly = name.StartsWithAnyOf(pluginAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);
            bool isGame = assembly.IsDependentOn(unityEngineAssembly);
            bool isPlugin = isPluginScriptAssembly || isInProject || (!isUserScriptAssembly && isInScriptAssemblies);

            // HACK: Odin and other assemblies, but easpecially Odin, needs to be registered as a plugin if it's installed as a package from the Unity PackageManager.
            // However there doesn't seemt to be any good way of figuring that out.

            // TODO: Find a good way of figuring if it's a plugin when located installed as a package.
            // Maybe it would be easier to figure out whether something was a Unity type, and then have plugin as fallback, instead of ther other way around, which
            // is how it works now.
            if (!isPlugin && name.StartsWith("sirenix."))
            {
                isPlugin = true;
            }

            bool isUser = !isPlugin && isUserScriptAssembly;

#if UNITY_EDITOR
            bool isEditor = isUser ? name.Contains("-editor") : assembly.IsDependentOn(unityEditorAssembly);

            if (isUser)
            {
                isEditor = name.Contains("-editor");
            }
            else
            {
                isEditor = assembly.IsDependentOn(unityEditorAssembly);
            }
#else
                bool isEditor = false;
#endif
            if (!isGame && !isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.OtherTypes;
            }
            else if (isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.UnityEditorTypes;
            }
            else if (isGame && !isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.UnityTypes;
            }
            else if (isEditor && isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.PluginEditorTypes;
            }
            else if (!isEditor && isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.PluginTypes;
            }
            else if (isEditor && isUser)
            {
                result = AssemblyTypeFlags.UserEditorTypes;
            }
            else if (!isEditor && isUser)
            {
                result = AssemblyTypeFlags.UserTypes;
            }
            else
            {
                result = AssemblyTypeFlags.OtherTypes;
            }

            return result;
        }

        /// <summary>
        /// Determines whether an assembly is depended on another assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="otherAssembly">The other assembly.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="assembly"/> has a reference in <paramref name="otherAssembly"/> or <paramref name="assembly"/> is the same as <paramref name="otherAssembly"/>.
        /// </returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is null.</exception>
        /// <exception cref="System.NullReferenceException"><paramref name="otherAssembly"/> is null.</exception>
        public static bool IsDependentOn(this Assembly assembly, Assembly otherAssembly)
        {
            if (assembly == null) throw new NullReferenceException("assembly");
            if (otherAssembly == null) throw new NullReferenceException("otherAssembly");

            if (assembly == otherAssembly)
            {
                return true;
            }

            var otherName = otherAssembly.GetName().ToString();
            var referencedAsssemblies = assembly.GetReferencedAssemblies();

            for (int i = 0; i < referencedAsssemblies.Length; i++)
            {
                if (otherName == referencedAsssemblies[i].ToString())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the assembly module is a of type <see cref="System.Reflection.Emit.ModuleBuilder"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        ///   <c>true</c> if the specified assembly of type <see cref="System.Reflection.Emit.ModuleBuilder"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">assembly</exception>
        public static bool IsDynamic(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            bool result;

            lock (IS_DYNAMIC_CACHE_LOCK)
            {
                if (!IsDynamicCache.TryGetValue(assembly, out result))
                {
                    try
                    {
                        // Will cover both System.Reflection.Emit.AssemblyBuilder and System.Reflection.Emit.InternalAssemblyBuilder
                        result = assembly.GetType().FullName.EndsWith("AssemblyBuilder") || assembly.Location == null || assembly.Location == "";
                    }
                    catch
                    {
                        result = true;
                    }

                    IsDynamicCache.Add(assembly, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the full file path to a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full file path to a given assembly, or <c>Null</c> if no file path was found.</returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is Null.</exception>
        public static string GetAssemblyDirectory(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            var path = assembly.GetAssemblyFilePath();
            if (path == null)
            {
                return null;
            }

            try
            {
                return Path.GetDirectoryName(path);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the full directory path to a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full directory path in which a given assembly is located, or <c>Null</c> if no file path was found.</returns>
        public static string GetAssemblyFilePath(this Assembly assembly)
        {
            if (assembly == null) return null;
            if (assembly.IsDynamic()) return null;
            if (assembly.CodeBase == null) return null;

            var filePrefix = @"file:///";
            var path = assembly.CodeBase;

            if (path.StartsWith(filePrefix, StringComparison.InvariantCultureIgnoreCase))
            {
                path = path.Substring(filePrefix.Length);
                path = path.Replace('\\', '/');

                if (File.Exists(path))
                {
                    return path;
                }

                if (!Path.IsPathRooted(path))
                {
                    if (File.Exists("/" + path))
                    {
                        path = "/" + path;
                    }
                    else
                    {
                        path = Path.GetFullPath(path);
                    }
                }

                if (!File.Exists(path))
                {
                    try
                    {
                        path = assembly.Location;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return path;
                }

                if (File.Exists(path))
                {
                    return path;
                }
            }

            if (File.Exists(assembly.Location))
            {
                return assembly.Location;
            }

            return null;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="fullName">The full name of the type, with or without any assembly information.</param>
        public static Type GetTypeByCachedFullName(string name)
        {
            return Sirenix.Serialization.TwoWaySerializationBinder.Default.BindToType(name);
        }

        /// <summary>
        /// Get types from the current AppDomain with a specified <see cref="AssemblyTypeFlags"/> filter.
        /// </summary>
        /// <param name="assemblyTypeFlags">The <see cref="AssemblyTypeFlags"/> filters.</param>
        /// <returns>Types from the current AppDomain with the specified <see cref="AssemblyTypeFlags"/> filters.</returns>
        public static IEnumerable<Type> GetTypes(AssemblyTypeFlags assemblyTypeFlags)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var flag = GetAssemblyTypeFlag(assembly);

                if ((flag & assemblyTypeFlags) != 0)
                {
                    foreach (var type in assembly.SafeGetTypes())
                    {
                        yield return type;
                    }
                }
            }
        }
        
        private static bool StartsWithAnyOf(this string str, IEnumerable<string> values, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            var iList = values as IList<string>;

            if (iList != null)
            {
                int count = iList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (str.StartsWith(iList[i], comparisonType))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var value in values)
                {
                    if (str.StartsWith(value, comparisonType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
#endif