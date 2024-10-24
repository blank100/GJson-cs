using System;
using System.Collections.Generic;

namespace Gal.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <para>author gouanlin</para>
    public class MultiTextReplacer
    {
        private Node m_Root = new() { };

        public void Enroll(string source, string target) {
            var tree = m_Root.children ??= new();
            var chars = source.AsSpan();
            while (true) {
                var first = chars[0];
                if (!tree.TryGetValue(first, out var child)) {
                    if (chars.Length != 1) tree.Add(first, child = new() { children = new() });
                    else {
                        tree.Add(first, new() { source = source, target = target });
                        return;
                    }
                } else if (chars.Length == 1) {
                    if (child.source != null) throw new($"集合中已存在要添加的字符串,{nameof(source)}:{source},{nameof(target)}:{target}");
                    child.source = source;
                    child.target = target;
                    return;
                }
                tree = child.children ??= new();
                chars = chars[1..];
            }
        }

        public void Unroll(string source) => Unroll(m_Root, source.AsSpan());

        private bool Unroll(Node node, ReadOnlySpan<char> chars) {
            if (node.children == null) return false;

            var tree = node.children;
            var first = chars[0];
            if (!tree.TryGetValue(first, out var child)) return false;
            if (chars.Length == 1) {
                if (child.children == null) {
                    tree.Remove(first);
                    return true;
                }
                child.source = child.target = null;
                return false;
            }
            if (!Unroll(child, chars[1..]) || child.children.Count != 0) return false;
            if (child.source == null) {
                tree.Remove(first);
                return true;
            }
            child.children = null;
            return false;
        }

        public string Replace(string text) => TryReplace(text, out var newText) ? newText : text;

        public bool TryReplace(string text, out string result) {
            var forecastLength = (int)(text.Length * 1.25f);
            if (forecastLength <= 256) {
                RefWriter<char> buffer = new(stackalloc char[forecastLength]);
                try {
                    if (Replace(ref buffer, text)) {
                        result = buffer.writtenSpan.ToString();
                        return true;
                    }
                    result = null;
                    return false;
                } finally {
                    buffer.Dispose();
                }
            } else {
                RefWriter<char> buffer = new(forecastLength);
                try {
                    if (Replace(ref buffer, text)) {
                        result = buffer.writtenSpan.ToString();
                        return true;
                    }
                    result = null;
                    return false;
                } finally {
                    buffer.Dispose();
                }
            }
        }

        private bool Replace(ref RefWriter<char> writer, ReadOnlySpan<char> text) {
            int start = 0, length = text.Length;
            while (start < length) {
                Node matched = null;
                var tree = m_Root.children;
                var found = -1;

                for (var i = start; i < length; i++) {
                    if (tree.TryGetValue(text[i], out var current)) {
                        if (found == -1) found = i;
                        if (current.target != null) {
                            matched = current;
                            if (current.children == null) goto success;
                        }
                        tree = current.children;
                    } else if (matched != null) goto success;
                    else {
                        tree = m_Root.children;
                        found = -1;
                    }
                }

                if (matched == null) break;

                success:
                writer.Write(text[start..found]);
                writer.Write(matched.target);
                start = found + matched.source.Length;
            }
            if (start == 0) return false;
            if (start < length) writer.Write(text[start..]);

            return true;
        }

        private class Node
        {
            public string source;
            public string target;
            public Dictionary<char, Node> children;
        }
    }
}