using System;
using System.Collections.Generic;
using System.Text;

namespace PgLib2.Schema.Dump
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    internal class PgDumpArgumentAttribute:Attribute
    {
        public PgDumpArgumentAttribute(string argumentName, bool addEqual)
        {
            ArgumentName = argumentName;
            AddEqual = addEqual;
        }
        public PgDumpArgumentAttribute(string argumentName):this(argumentName, false)
        {
        }
        public string ArgumentName { get; }
        public bool AddEqual {  get; set; }
    }
}
