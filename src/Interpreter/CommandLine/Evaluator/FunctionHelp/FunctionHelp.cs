using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IronyCalc.Interpreter.Evaluator
{
    static class FunctionHelper
    {
        public static List<FunctionHelpCategory> Help = new List<FunctionHelpCategory>();

        public static void RegisterFunctionGroup(string name, string description = "")
        {
            if (GroupNameExists(name))
                return;

            if (!GroupNameExists(name) && !AvoidRegister(name))
            {
                Help.Add(new FunctionHelpCategory()
                {
                    Description = description,
                    Name = name
                });
            }
        }

        public static void RegisterFunction(string groupName, string name, string description, string example, string[] shortcuts = null, bool canBeFixed = false)
        {
            RegisterFunctionGroup(groupName);
            FunctionHelpCategory group = Help.Where(item => item.Name == groupName).FirstOrDefault();

            if (group != null && !group.Nodes.Any(item => item.Name == name) && !AvoidRegister(groupName))
            {
                group.Nodes.Add(new FunctionHelpEntry() {
                    Name = name,
                    Description = description,
                    UsageExample = example,
                    CanBeFixed = canBeFixed,
                    Shortcuts = (shortcuts == null || shortcuts.Length == 0) ? null : shortcuts.ToList()
                });
            }
        }

        public static void RegisterStaticMembers(Type type, string groupName)
        {
            RegisterFunctionGroup("Constants");
            RegisterFunctionGroup(groupName);
            var groupNameProperties = groupName + " Properties";
            RegisterFunctionGroup(groupNameProperties);
            var group = Help.Where(item => item.Name == groupName).FirstOrDefault();
            var groupConstants = Help.Where(item => item.Name == "Constants").FirstOrDefault();
            var groupProperties = Help.Where(item => item.Name == groupNameProperties).FirstOrDefault();

            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
            {
                if (NameExists(member.Name) || AvoidRegister(member.Name))
                    continue;

                switch (member.MemberType)
                {
                    case MemberTypes.Method:
                        group.Nodes.Add(new FunctionHelpEntry()
                        {
                            Name = member.Name,
                        });
                        break;
                    case MemberTypes.Property:
                        groupProperties.Nodes.Add(new FunctionHelpEntry()
                        {
                            Name = member.Name,
                        });
                        break;
                    case MemberTypes.Field:
                        groupConstants.Nodes.Add(new FunctionHelpEntry()
                        {
                            Name = member.Name,
                        });
                        break;
                }
            }
        }

        private static bool GroupNameExists(string name)
        {
            return Help.Any(item => item.Name == name);
        }

        private static bool NameExists(string name)
        {
            return Help.Any(item => item.Nodes.Any(node => node.Name == name));
        }

        private static string[] _avoidRegisterByName = new string[] {
            "IEEERemainder",
            "BigMul",
            "DivRem",
            "Truncate",
            "Pow",
            "egg",
        };
        private static bool AvoidRegister(string name)
        {
            return _avoidRegisterByName.Contains(name);
        }
    }

    class FunctionHelpEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UsageExample { get; set; }
        public List<string> Shortcuts { get; set; }
        public bool CanBeFixed { get; set; }
    }

    class FunctionHelpCategory
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FunctionHelpEntry> Nodes { get; set; }

        public FunctionHelpCategory()
        {
            this.Nodes = new List<FunctionHelpEntry>();
        }
    }
}
