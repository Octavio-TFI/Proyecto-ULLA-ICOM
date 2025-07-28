using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public record AgentFunctionCall
    {
        public required string? PluginName { get; init; }

        public required string FunctionName { get; init; }

        public required Dictionary<string, object?>? Arguments { get; init; }

        public override string ToString()
        {
            var plugin = PluginName ?? "NoPlugin";
            var args = Arguments != null
                ? string.Join(
                    ", ",
                    Arguments.Select(kvp => $"{kvp.Key}: {kvp.Value ?? "null"}"))
                : "NoArguments";

            return $"Plugin: {plugin}, Function: {FunctionName}, Arguments: [{args}]";
        }
    }
}
