using System;
using System.Collections.Generic;

namespace Lab4.Task5_Memento
{
    public class TextDocumentMemento
    {
        internal string Content { get; }
        internal string Title { get; }
        internal DateTime SavedAt { get; }

        internal TextDocumentMemento(string content, string title)
        {
            Content = content;
            Title = title;
            SavedAt = DateTime.Now;
        }

        public override string ToString()
            => $"Знімок: '{Title}' [{SavedAt:HH:mm:ss}] ({Content.Length} символів)";
    }

    public class TextDocument
    {
        public string Title { get; private set; }
        public string Content { get; private set; }

        public TextDocument(string title, string content = "")
        {
            Title = title;
            Content = content;
        }

        public void SetTitle(string title)
        {
            Console.WriteLine($"[Document] Заголовок змінено: '{Title}' → '{title}'");
            Title = title;
        }

        public void AppendText(string text)
        {
            Content += text;
            Console.WriteLine($"[Document] Додано текст. Довжина документа: {Content.Length} символів.");
        }

        public void ReplaceContent(string newContent)
        {
            Console.WriteLine($"[Document] Вміст повністю замінено.");
            Content = newContent;
        }

        public TextDocumentMemento Save()
        {
            Console.WriteLine($"[Document] Збережено знімок стану.");
            return new TextDocumentMemento(Content, Title);
        }

        public void Restore(TextDocumentMemento memento)
        {
            Title = memento.Title;
            Content = memento.Content;
            Console.WriteLine($"[Document]  Відновлено стан: '{Title}' ({Content.Length} символів).");
        }

        public void PrintState()
        {
            Console.WriteLine($"\n--- Поточний стан документа ---");
            Console.WriteLine($"  Заголовок : {Title}");
            Console.WriteLine($"  Вміст     : {(Content.Length > 60 ? Content[..60] + "..." : Content)}");
            Console.WriteLine($"  Символів  : {Content.Length}");
            Console.WriteLine($"--------------------------------");
        }
    }

    public class TextEditor
    {
        private readonly TextDocument _document;
        private readonly Stack<TextDocumentMemento> _history = new();

        public TextEditor(TextDocument document)
        {
            _document = document;
            Console.WriteLine($"[Editor] Відкрито документ: '{document.Title}'");
        }
        public void SaveState()
        {
            var memento = _document.Save();
            _history.Push(memento);
            Console.WriteLine($"[Editor] Стан збережено. Знімків в історії: {_history.Count}");
        }
        public void Undo()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("[Editor]   Немає збережених станів для скасування.");
                return;
            }

            var memento = _history.Pop();
            _document.Restore(memento);
            Console.WriteLine($"[Editor] Скасовано. Залишилось знімків: {_history.Count}");
        }
        public void PrintHistory()
        {
            Console.WriteLine("\n=== Історія редагування ===");
            if (_history.Count == 0)
            {
                Console.WriteLine("  (Історія порожня)");
                return;
            }

            int i = 1;
            foreach (var m in _history)
                Console.WriteLine($"  {i++}. {m}");
        }
        public void TypeText(string text) => _document.AppendText(text);
        public void ChangeTitle(string title) => _document.SetTitle(title);
        public void PrintDocument() => _document.PrintState();
    }
}