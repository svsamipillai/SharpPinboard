using System;
using System.Collections.Generic;
using System.Linq;
using Pinboard.Types;
using RestSharp;

namespace Pinboard.Helpers
{
    public static class ParameterHelpers
    {
        public static Parameter ValueToGetArgument(string name, object value)
        {
            var parameter = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = name
            };

            if (value == null)
            {
            }
            else if (value is bool)
            {
                if ((bool) value)
                {
                    parameter.Value = "yes";
                }
                else
                {
                    parameter.Value = "no";
                }
            }
            else if (value is DateTime)
            {
                parameter.Value = ((DateTime) value).ToString("s") + "Z";
            }
            else if (value.GetType() == typeof (List<Tag>))
            {
                parameter.Value = string.Join(" ", ((List<Tag>) value).Select(x => x.tag));
            }
            else
            {
                throw new ApplicationException("Couldn't turn " + name + " into a GET argument, its type was " +
                                               value.GetType() + " with a value of " + value);
            }

            return parameter;
        }
    }
}