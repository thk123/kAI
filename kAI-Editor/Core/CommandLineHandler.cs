using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace kAI.Editor.Core
{
    /// <summary>
    /// An entry for the command line dictionary. 
    /// </summary>
    interface kAIICommandLineEntry
    {
        /// <summary>
        /// Is the command line entry a flag or an option
        /// - Flag: no parameters, just either on or off
        /// - Option: next argument should be the parameter for this option.
        /// </summary>
        bool IsFlag
        {
            get;
        }

        /// <summary>
        /// Should the action associated with this be done by default
        /// </summary>
        bool EnabledByDefault
        {
            get;
        }

        /// <summary>
        /// The string to search for in the command line options, eg p here means looking for 
        /// switch -p
        /// </summary>
        string FlagString
        {
            get;
        }

        /// <summary>
        /// Should the action be taken, ie has the switch been flipped
        /// </summary>
        bool ShouldTakeAction
        { 
            get;
        }

        /// <summary>
        /// Enable the flag, throws exception if an option. 
        /// </summary>
        void EnableFlag();

        /// <summary>
        /// Set the property, throws an exception if a flag.
        /// </summary>
        /// <param name="argument">The next argument to use. </param>
        void SetProperty(string argument);

        /// <summary>
        /// Take the action assoicated with this command line entry.
        /// </summary>
        void TakeAction();
    }

    /// <summary>
    /// Shell around a command line entry that stores whether the option/flag is enabled. 
    /// </summary>
    abstract class kAICommandLineEntry : kAIICommandLineEntry
    {
        bool mEnabled = false;

        /// <summary>
        /// Should the action be taken, ie has the switch been flipped
        /// </summary>
        public bool ShouldTakeAction
        {
            get
            {
                return EnabledByDefault || mEnabled;
            }
        }

        /// <summary>
        /// Is the command line entry a flag or an option
        /// - Flag: no parameters, just either on or off
        /// - Option: next argument should be the parameter for this option.
        /// </summary>
        public abstract bool IsFlag
        {
            get;
        }

        /// <summary>
        /// Should the action associated with this be done by default
        /// </summary>
        public abstract bool EnabledByDefault
        {
            get;
        }

        /// <summary>
        /// The string to search for in the command line options, eg p here means looking for 
        /// switch -p
        /// </summary>
        public abstract string FlagString
        {
            get;
        }

        /// <summary>
        /// Enable the flag, throws exception if an option. 
        /// </summary>
        public virtual void EnableFlag()
        {
            mEnabled = true;
        }

        /// <summary>
        /// Set the property, throws an exception if a flag.
        /// </summary>
        /// <param name="argument">The next argument to use. </param>
        public virtual void SetProperty(string argument)
        {
            mEnabled = true;
        }

        public abstract void TakeAction();
    }

    /// <summary>
    /// The command option to load a project at start
    /// Option: A string pointing to the location of the kAIProj file. 
    /// </summary>
    class kAILoadProjectOption : kAICommandLineEntry
    {
        string mProjectToLoad;

        /// <summary>
        /// Is an option. 
        /// </summary>
        public override bool IsFlag
        {
            get { return false; }
        }

        /// <summary>
        /// String - p
        /// </summary>
        public override string FlagString
        {
            get { return "p"; }
        }

        /// <summary>
        /// Not enabled by default. 
        /// </summary>
        public override bool EnabledByDefault
        {
            get { return false; }
        }

        public override void EnableFlag()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the project to load. 
        /// </summary>
        /// <param name="argument">Should be a path to a project. </param>
        public override void SetProperty(string argument)
        {
            base.SetProperty(argument);
            mProjectToLoad = argument;
        }

        /// <summary>
        /// Loads the project specified in the option. 
        /// </summary>
        public override void TakeAction()
        {
            FileInfo lProjectFile = new FileInfo(mProjectToLoad);
            if (lProjectFile.Exists)
            {
                GlobalServices.Editor.LoadProject(lProjectFile);
            }
            else
            {
                GlobalServices.Logger.LogWarning("Could not find project", new KeyValuePair<string, object>("Project", mProjectToLoad));
            }
        }
    }

    /// <summary>
    /// Handles the command line options. 
    /// </summary>
    class CommandLineHandler
    {
        static Dictionary<string, kAIICommandLineEntry> sProperties;

        /// <summary>
        /// Create the dictionary with each of the command line entries
        /// </summary>
        static CommandLineHandler()
        {
            sProperties = new Dictionary<string, kAIICommandLineEntry>();

            Type lInterface = typeof(kAIICommandLineEntry);
            IEnumerable<Type> lProperties = Assembly.GetExecutingAssembly().GetTypes().Where(lType => lInterface.IsAssignableFrom(lType));
            
            foreach (Type lType in lProperties)
            {
                if(!lType.IsAbstract)
                {
                    kAIICommandLineEntry lEntry = (kAIICommandLineEntry)Activator.CreateInstance(lType);
                    sProperties.Add(lEntry.FlagString, lEntry);
                }
            }
        }
       
        /// <summary>
        /// Process the command line arguments. 
        /// </summary>
        /// <param name="args">The arguments as they come in to the program.</param>
        public static void ProcessCommands(String[] args)
        {
            for (int i = 0; i < args.Length; ++i )
            {
                string arg = args[i];
                if (arg.StartsWith("-"))
                {
                    string lKey = arg.Substring(1);
                    kAIICommandLineEntry lEntry;
                    if (sProperties.TryGetValue(lKey, out lEntry))
                    {
                        if (lEntry.IsFlag)
                        {
                            lEntry.EnableFlag();
                        }
                        else
                        {
                            string lPropertyValue = args[i + 1];
                            ++i;

                            lEntry.SetProperty(lPropertyValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Take all the relevant actions as specified by the command line. 
        /// </summary>
        public static void TakeActions()
        {
            foreach (kAIICommandLineEntry lEntry in sProperties.Values)
            {
                if (lEntry.ShouldTakeAction)
                {
                    lEntry.TakeAction();
                }
            }
        }
    }
}
