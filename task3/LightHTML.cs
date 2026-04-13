using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task3_Observer
{
    public delegate void EventHandler(string eventName, LightElement sender);

    public abstract class LightNode
    {
        public abstract string OuterHtml(int indent = 0);
    }

    public class LightText : LightNode
    {
        private readonly string _text;
        public LightText(string text) => _text = text;

        public override string OuterHtml(int indent = 0)
            => new string(' ', indent * 2) + _text;
    }

    public class LightElement : LightNode
    {
        public string TagName { get; }
        public bool IsBlock { get; }
        public bool IsSelfClosing { get; }

        private readonly List<LightNode> _children = new();
        private readonly List<string> _cssClasses = new();

        private readonly Dictionary<string, List<EventHandler>> _eventListeners = new();

        public LightElement(string tagName, bool isBlock = true, bool isSelfClosing = false)
        {
            TagName = tagName;
            IsBlock = isBlock;
            IsSelfClosing = isSelfClosing;
        }

        public void AddEventListener(string eventName, EventHandler handler)
        {
            if (!_eventListeners.ContainsKey(eventName))
                _eventListeners[eventName] = new List<EventHandler>();

            _eventListeners[eventName].Add(handler);
            Console.WriteLine($"[Observer] Підписано обробник на подію '{eventName}' елемента <{TagName}>.");
        }

        public void RemoveEventListener(string eventName, EventHandler handler)
        {
            if (_eventListeners.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(handler);
                Console.WriteLine($"[Observer] Видалено обробник події '{eventName}' елемента <{TagName}>.");
            }
        }

        public void DispatchEvent(string eventName)
        {
            Console.WriteLine($"[Event] Подія '{eventName}' спрацювала на <{TagName}>.");

            if (_eventListeners.TryGetValue(eventName, out var handlers))
            {
                foreach (var handler in handlers)
                    handler(eventName, this);
            }
            else
            {
                Console.WriteLine($"  (Немає обробників для '{eventName}')");
            }
        }

        public LightElement AddChild(LightNode node)
        {
            _children.Add(node);
            return this;
        }

        public LightElement AddClass(string cssClass)
        {
            _cssClasses.Add(cssClass);
            return this;
        }

        public override string OuterHtml(int indent = 0)
        {
            var sb = new StringBuilder();
            string pad = new string(' ', indent * 2);
            string classAttr = _cssClasses.Count > 0
                ? $" class=\"{string.Join(" ", _cssClasses)}\""
                : "";

            if (IsSelfClosing)
            {
                sb.Append($"{pad}<{TagName}{classAttr} />");
            }
            else
            {
                string newline = IsBlock ? "\n" : "";
                sb.Append($"{pad}<{TagName}{classAttr}>{newline}");

                foreach (var child in _children)
                    sb.Append(child.OuterHtml(IsBlock ? indent + 1 : 0) + newline);

                sb.Append($"{pad}</{TagName}>");
            }

            return sb.ToString();
        }
    }
}