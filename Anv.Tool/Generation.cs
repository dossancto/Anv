using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace Anv.Tool;

public static class Generation
{
    public static AnvTree GenerateTree(string env, bool doubleQuoteSeparator = false)
    {
        var tree = new AnvTree()
        {
            Name = "AppEnv"
        };

        var lines = env.Split('\n');

        foreach (var line in lines)
        {
            var l = line.Trim();

            if (l.StartsWith("#")) continue;

            var lineTokens = l.Split("=");

            var envName = lineTokens.First();

            var tokens = envName.Split(doubleQuoteSeparator ? "__" : ".");

            string? comment = null;

            if (l.Contains(" #"))
            {
                comment = l.Split(" #").LastOrDefault()?.TrimStart();
            }

            ParseTokens(tree, tokens, comment, doubleQuoteSeparator);
        }

        return tree;
    }

    private static void ParseTokens(AnvTree fatherNode, string[] lines, string? comment, bool useUnderlineSeparator = false, int depth = 0)
    {
        var token = lines.ElementAtOrDefault(depth);

        if (token is null)
        {
            return;
        }

        var node = fatherNode.Nodes.FirstOrDefault(x => x.Name == token);

        if (node is not null)
        {
            ParseTokens(node, lines, comment, useUnderlineSeparator, depth + 1);

            return;
        }

        fatherNode.Nodes.Add(new AnvTree
        {
            Name = token,
            Comment = comment,
            FullName = string.Join(useUnderlineSeparator ? "__" : ".", lines.Take(depth + 1))
        });

        var recentlyAdded = fatherNode.Nodes.Last();

        ParseTokens(recentlyAdded, lines, comment, useUnderlineSeparator, depth + 1);

        return;
    }

    public static void BuildAnvClass(AnvTree tree, StringBuilder sb)
    {
        if (string.IsNullOrWhiteSpace(tree.Name))
        {
            return;
        }

        sb.AppendLine($"public static partial class {tree.Name} {{");

        foreach (var n in tree.Nodes)
        {
            if (!n.IsEnv)
            {
                BuildAnvClass(n, sb);
                continue;
            }

            if (n.Comment is not null)
            {
                sb.AppendLine($"/// <summary>");
                sb.AppendLine($"/// {n.Comment}");
                sb.AppendLine($"/// </summary>");
            }
            sb.AppendLine($"public static readonly AnvEnv {n.Name} = new(\"{n.FullName}\");");
        }

        sb.AppendLine("}");
    }

    public static string FormatCSharpCode(this string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        var workspace = new AdhocWorkspace();
        var options = workspace.Options;

        var formattedRoot = Formatter.Format(root, workspace, options);

        return formattedRoot.ToFullString();
    }
}

public record AnvTree
{
    public List<AnvTree> Nodes { get; set; } = [];

    public bool IsEnv => Nodes.Count == 0 && !string.IsNullOrWhiteSpace(Name);

    public string? Name { get; set; }
    public string? Comment { get; set; }
    public string? FullName { get; set; }
}
