using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RestSharp.Contrib;
using RestSharp;
using Pinboard.Types;

namespace Pinboard.Helpers
{
    public static class ParameterHelpers
    {
        public static Parameter ValueToGetArgument(string name, object value)
        {
            Parameter parameter = new Parameter();
            parameter.Type = ParameterType.GetOrPost;
            parameter.Name = name;

            //strings should be added directly now

            if (value == null)
                value = String.Empty;
    
            else if (value.GetType() == typeof(bool))
            {
                if ((bool)value)
                    parameter.Value = "yes";
                else parameter.Value = "no";
            }

            else if (value.GetType() == typeof(DateTime))
            {
                parameter.Value = ((DateTime)value).ToString("s") + "Z"; //as per http://stackoverflow.com/questions/1728404/date-format-yyyy-mm-ddthhmmssz
            }

            else if (value.GetType() == typeof(List<Tag>))
            {
                parameter.Value = string.Join(" ",((List<Tag>)value).Select(x=>x.tag)); //Hopefully keep this compatible with older .NET 
            }

            else
            {
                throw new ApplicationException("Couldn't turn " + name + " into a GET argument, its type was " + value.GetType().ToString() + " with a value of " + value.ToString());
            }

            return parameter;
        }

    }
}
